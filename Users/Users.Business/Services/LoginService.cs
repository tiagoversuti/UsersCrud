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
    public class LoginService : BaseService, ILoginService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public LoginService(IConfiguration configuration,
                            IUserRepository userRepository,
                            INotifier notifier) : base(notifier)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public string Authenticate(LoginDto loginDto)
        {
            var user = _userRepository.GetByLogin(loginDto.Login);

            if (user is null || !user.ValidatePassword(loginDto.Password))
            {
                Notify("Invalid login or password.");
                return null;
            }

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var secret = _configuration.GetSection("Secret").Value;
            if (string.IsNullOrEmpty(secret))
            {
                Notify("Secret key missing.");
                return null;
            }

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
                Notify("Token is missing.");
                return null;
            }

            var secret = _configuration.GetSection("Secret").Value;
            if (string.IsNullOrWhiteSpace(secret))
            {
                Notify("Secret key missing.");
                return null;
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

                if (user.Login != login)
                {
                    Notify("The informed login doesn't correspond with the user id.");
                    return null;
                }

                return new UserDto
                {
                    Id = user.Id,
                    Login = user.Login,
                    Name = user.Name
                };
            }
            catch (Exception e)
            {
                Notify("An error has occurred: " + e.Message);
                return null;
            }
        }
    }
}
