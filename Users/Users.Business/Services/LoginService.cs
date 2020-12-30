using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Users.Business.Contracts;
using Users.Business.Interfaces;
using Users.Business.Models;

namespace Users.Business.Services
{
    public class LoginService : ILoginService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public LoginService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public string Authenticate(LoginDto loginDto)
        {
            var user = _userRepository.GetByLogin(loginDto.Login);

            if (user is null)
                throw new ArgumentException("Invalid login or password.");

            if (!user.ValidatePassword(loginDto.Password))
                throw new ArgumentException("Invalid login or password.");

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var secret = _configuration.GetSection("Secret").Value;
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("Secret key missing.");

            var payload = new Dictionary<string, object>
            {
                { "id", user.Id },
                { "login", user.Login },
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);

            return token;
        }

        public UserDto ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var secret = _configuration.GetSection("Secret").Value;
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("Secret key missing.");
            }

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                IDictionary<string, object> tokenData = decoder.DecodeToObject(token, secret, verify: true);

                var id = Guid.Parse(tokenData["id"].ToString());
                var login = tokenData["login"].ToString();

                var user = _userRepository.GetById(id);

                if (user.Login!= login)
                    return null;

                return new UserDto
                {
                    Id = user.Id,
                    Login = user.Login,
                    Name = user.Name
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
