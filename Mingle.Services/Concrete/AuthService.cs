using AutoMapper;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs;
using Mingle.Services.Exceptions;

namespace Mingle.Services.Concrete
{
    public sealed class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository authRepository, IUserRepository userRepository, IMapper mapper)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public async Task SignUpAsync(SignUpRequest dto) 
        {
            var userCredential = await _authRepository.SignUpUserAsync(dto.Email, dto.Password, dto.DisplayName);

            if (userCredential != null)
            {
                var user = _mapper.Map<User>(dto);
                await _userRepository.CreateUserAsync(userCredential.User.Uid, user);
            }
            else
            {
                throw new BadRequestException("Kullancı eklenemedi.");
            }
        }


        public async Task SignInAsync(SignInRequest dto)
        {
            var userCredential = await _authRepository.SignInUserAsync(dto.Email, dto.Password);

            // JWT Üretilecek
        }
    }
}