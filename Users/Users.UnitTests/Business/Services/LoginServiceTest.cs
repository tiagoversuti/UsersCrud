using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using Users.Business.Contracts;
using Users.Business.Interfaces;
using Users.Business.Models;
using Users.Business.Services;

namespace Users.UnitTests.Business.Services
{
    [TestClass]
    public class LoginServiceTest
    {
        private readonly LoginService _service;

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;

        public LoginServiceTest()
        {
            _configuration = Substitute.For<IConfiguration>();
            _repository = Substitute.For<IUserRepository>();

            _service = new LoginService(_configuration, _repository);
        }

        [TestMethod]
        public void Authenticate_WhenLoginIsInvalid_ShouldThrowException()
        {
            //Arrange            
            _repository.GetByLogin(Arg.Any<string>()).Returns(null as User);

            var loginDto = new LoginDto
            {
                Login = "InvalidLogin",
                Password = "Password"
            };

            //Act
            Action action = () => _service.Authenticate(loginDto);

            //Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("Invalid login or password.");
        }

        [TestMethod]
        public void Authenticate_WhenPasswordIsInvalid_ShouldThrowException()
        {
            //Arrange
            var login = "Login";
            _repository.GetByLogin(Arg.Is(login)).Returns(new User(Guid.NewGuid(), "Name", login, "Password"));

            var loginDto = new LoginDto
            {
                Login = login,
                Password = "InvalidPassword"
            };

            //Act
            Action action = () => _service.Authenticate(loginDto);

            //Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("Invalid login or password.");
        }

        [TestMethod]
        public void Authenticate_WhenLoginAndPasswordAreValid_ShouldGenerateToken()
        {
            //Arrange
            var login = "Login";
            var password = "Password";
            _repository.GetByLogin(Arg.Is(login)).Returns(new User(Guid.NewGuid(), "Name", login, password));

            var configurationSection = Substitute.For<IConfigurationSection>();
            _configuration.GetSection("Secret").Returns(configurationSection);
            configurationSection.Value.Returns("UsersSecret");

            var loginDto = new LoginDto
            {
                Login = login,
                Password = password
            };

            //Act
            var token = _service.Authenticate(loginDto);

            //Assert
            token.Should().NotBeNullOrEmpty();
        }
    }
}
