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
    public sealed class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthManager _authManager;
        private readonly IJwtManager _jwtManager;
        private readonly IMapper _mapper;


        public AuthService(IAuthRepository authRepository, IUserRepository userRepository, IAuthManager authManager, IJwtManager jwtManager, IMapper mapper)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _authManager = authManager;
            _jwtManager = jwtManager;
            _mapper = mapper;
        }


        public async Task SignUpAsync(SignUp dto)
        {
            var userCredential = await _authRepository.CreateUserAsync(dto.Email, dto.Password, dto.DisplayName);
            var user = _mapper.Map<User>(dto);

            await _userRepository.CreateUserAsync(userCredential.User.Uid, user);
        }


        public async Task<string> SignInEmailAsync(SignInEmail dto)
        {
            var userCredential = await _authRepository.SignInWithEmailAsync(dto.Email, dto.Password);

            return await Task.Run(() => _jwtManager.GenerateToken(userCredential.User.Uid));
        }


        public async Task<string> SignInGoogleAsync(SignInGoogle dto)
        {
            var (isValid, errorMessage) = await Task.Run(() => _authManager.ValidateGoogle(dto));

            if (!isValid)
            {
                throw new BadRequestException(errorMessage);
            }
            else
            {
                throw new Exception("Test Gayet Başarılı");
            }

            var user = _mapper.Map<User>(dto.User.ProviderData);
            await _userRepository.CreateUserAsync(dto.User.Uid, user);

            return await Task.Run(() => _jwtManager.GenerateToken(dto.User.Uid));
        }

        
        public async Task<string> SignInFacebookAsync(string accessToken)
        {
            var userCredential = await _authRepository.SignInWithFacebookAsync(accessToken);
            var user = _mapper.Map<User>(userCredential);

            await _userRepository.CreateUserAsync(userCredential.User.Uid, user);

            return _jwtManager.GenerateToken(userCredential.User.Uid);
        }


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