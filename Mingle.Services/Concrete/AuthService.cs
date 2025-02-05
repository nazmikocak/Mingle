﻿using AutoMapper;
using Mingle.Core.Abstract;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Utilities;

namespace Mingle.Services.Concrete
{
    public sealed class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtManager _jwtManager;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository authRepository, IUserRepository userRepository, IJwtManager jwtManager, IMapper mapper)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _jwtManager = jwtManager;
            _mapper = mapper;
        }


        public async Task SignUpAsync(SignUp dto)
        {
            var userCredential = await _authRepository.CreateUserAsync(dto.Email, dto.Password, dto.DisplayName);
            var user = _mapper.Map<User>(dto);

            await _userRepository.CreateUserAsync(userCredential.User.Uid, user);
        }


        public async Task<string> SignInEmailAsync(SignIn dto)
        {
            var userCredential = await _authRepository.SignInWithEmailAsync(dto.Email, dto.Password);

            return await Task.Run(() => _jwtManager.GenerateToken(userCredential.User.Uid));
        }


        public async Task<string> SignInGoogleAsync(string accessToken)
        {
            FieldValidationHelper.ValidateRequiredFields((accessToken, "accessToken"));

            var userCredential = await _authRepository.SignInWithGoogleAsync(accessToken);
            var user = _mapper.Map<User>(userCredential);

            await _userRepository.CreateUserAsync(userCredential.User.Uid, user);

            return _jwtManager.GenerateToken(userCredential.User.Uid);
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

            await _authRepository.ResetEmailPasswordAsync(email);
        }
    }
}