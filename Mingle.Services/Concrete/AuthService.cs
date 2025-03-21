using AutoMapper;
using Mingle.Core.Abstract;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;
using Mingle.Shared.DTOs.Request;

namespace Mingle.Services.Concrete
{
    /// <summary>
    /// Kimlik doğrulama işlemlerini yöneten servis sınıfıdır.
    /// Kullanıcı kaydı, giriş işlemleri ve şifre sıfırlama gibi işlemleri içerir.
    /// </summary>
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IAuthManager _authManager;
        private readonly IJwtManager _jwtManager;
        private readonly IMapper _mapper;



        /// <summary>
        /// AuthService sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="authRepository">Kimlik doğrulama işlemleri için kullanılan repository.</param>
        /// <param name="userRepository">Kullanıcı yönetimi için kullanılan repository.</param>
        /// <param name="authManager">Kimlik doğrulama sağlayıcılarını yöneten servis.</param>
        /// <param name="jwtManager">JWT token yönetimi sağlayan servis.</param>
        /// <param name="mapper">DTO ile varlık nesneleri arasında dönüşüm sağlayan AutoMapper.</param>
        public AuthService(IAuthRepository authRepository, IUserRepository userRepository, IAuthManager authManager, IJwtManager jwtManager, IMapper mapper)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _authManager = authManager;
            _jwtManager = jwtManager;
            _mapper = mapper;
        }



        /// <summary>
        /// Yeni bir kullanıcı kaydı oluşturur.
        /// </summary>
        /// <param name="dto">Kayıt bilgilerini içeren DTO nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir <see cref="Task"/> nesnesi.</returns>
        public async Task SignUpAsync(SignUp dto)
        {
            var userCredential = await _authRepository.CreateUserAsync(dto.Email, dto.Password, dto.DisplayName);
            var user = _mapper.Map<User>(dto);

            await _userRepository.CreateUserAsync(userCredential.User.Uid, user);
        }



        /// <summary>
        /// E-posta ve şifre ile kullanıcı girişini gerçekleştirir.
        /// </summary>
        /// <param name="dto">Giriş bilgilerini içeren DTO nesnesi.</param>
        /// <returns>JWT token değerini içeren string.</returns>
        public async Task<string> SignInEmailAsync(SignInEmail dto)
        {
            var userCredential = await _authRepository.SignInWithEmailAsync(dto.Email, dto.Password);

            return await Task.Run(() => _jwtManager.GenerateToken(userCredential.User.Uid));
        }



        /// <summary>
        /// Google hesabı ile kullanıcı girişini gerçekleştirir.
        /// </summary>
        /// <param name="dto">Google kimlik doğrulama bilgilerini içeren DTO nesnesi.</param>
        /// <returns>JWT token değerini içeren string.</returns>
        /// <exception cref="BadRequestException">Geçersiz Google sağlayıcısı durumunda fırlatılır.</exception>
        public async Task<string> SignInGoogleAsync(SignInProvider dto)
        {
            var (isValid, errorMessage) = await Task.Run(() => _authManager.ValidateGoogleProvider(dto));

            if (!isValid)
            {
                throw new BadRequestException(errorMessage);
            }

            var user = await _userRepository.GetUserByIdAsync(dto.Uid);

            if (user == null)
            {
                user = _mapper.Map<User>(dto.ProviderData[0]);
                user.ProviderId = "google.com";

                await _userRepository.CreateUserAsync(dto.Uid, user);
            }

            return await Task.Run(() => _jwtManager.GenerateToken(dto.Uid));
        }



        /// <summary>
        /// Facebook hesabı ile kullanıcı girişini gerçekleştirir.
        /// </summary>
        /// <param name="dto">Facebook kimlik doğrulama bilgilerini içeren DTO nesnesi.</param>
        /// <returns>JWT token değerini içeren string.</returns>
        /// <exception cref="BadRequestException">Geçersiz Facebook sağlayıcısı durumunda fırlatılır.</exception>
        public async Task<string> SignInFacebookAsync(SignInProvider dto)
        {
            var (isValid, errorMessage) = await Task.Run(() => _authManager.ValidateFacebookProvider(dto));

            if (!isValid)
            {
                throw new BadRequestException(errorMessage);
            }

            var user = await _userRepository.GetUserByIdAsync(dto.Uid);

            if (user == null)
            {
                user = _mapper.Map<User>(dto.ProviderData[0]);
                user.ProviderId = "facebook.com";

                await _userRepository.CreateUserAsync(dto.Uid, user);
            }

            return await Task.Run(() => _jwtManager.GenerateToken(dto.Uid));
        }



        /// <summary>
        /// Kullanıcının şifresini sıfırlar.
        /// </summary>
        /// <param name="email">Şifre sıfırlama isteği yapılacak e-posta adresi.</param>
        /// <returns>Asenkron işlemi temsil eden bir <see cref="Task"/> nesnesi.</returns>
        /// <exception cref="NotFoundException">Belirtilen e-posta adresine sahip kullanıcı bulunamazsa fırlatılır.</exception>
        public async Task ResetPasswordAsync(string email)
        {
            FieldValidationHelper.ValidateEmailFormat(email);

            var usersSnapshot = await _userRepository.GetAllUsersAsync();
            var user = usersSnapshot.Where(user => user.Object.Email.Equals(email));

            if (user.Any())
            {
                await _authRepository.ResetEmailPasswordAsync(email);
            }
            else
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }
        }
    }
}