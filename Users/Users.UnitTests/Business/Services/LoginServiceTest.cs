using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using Users.Business;
using Users.Business.Contracts;
using Users.Business.Interfaces;
using Users.Business.Models;
using Users.Business.Notifications;
using Users.Business.Services;

namespace Users.UnitTests.Business.Services
{
    [TestClass]
    public class LoginServiceTest
    {
        private readonly LoginService _service;

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;
        private readonly INotifier _notifier;

        public LoginServiceTest()
        {
            _configuration = Substitute.For<IConfiguration>();
            _repository = Substitute.For<IUserRepository>();
            _notifier = Substitute.For<INotifier>();

            _service = new LoginService(_configuration, _repository, _notifier);
        }

        [TestMethod]
        public void Authenticate_WhenLoginIsInvalid_ShouldNotGenerateToken()
        {
            //Arrange            
            _repository.GetByLogin(Arg.Any<string>()).Returns(null as User);

            var loginDto = new LoginDto
            {
                Login = "InvalidLogin",
                Password = "Password"
            };

            //Act
            var token = _service.Authenticate(loginDto);

            //Assert
            token.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "Invalid login or password."));
        }

        [TestMethod]
        public void Authenticate_WhenPasswordIsInvalid_ShouldNotGenerateToken()
        {
            //Arrange
            var login = "Login";
            _repository.GetByLogin(Arg.Is(login)).Returns(new User(Guid.NewGuid(), "Name", login, Hash.Generate("Password")));

            var loginDto = new LoginDto
            {
                Login = login,
                Password = "InvalidPassword"
            };

            //Act
            var token = _service.Authenticate(loginDto);

            //Assert
            token.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "Invalid login or password."));
        }

        [TestMethod]
        public void Authenticate_WhenSecretIsEmpty_ShouldNotGenerateToken()
        {
            //Arrange
            var login = "Login";
            var password = "Password";
            _repository.GetByLogin(Arg.Is(login)).Returns(new User(Guid.NewGuid(), "Name", login, Hash.Generate(password)));

            var loginDto = new LoginDto
            {
                Login = login,
                Password = password
            };

            //Act
            var token = _service.Authenticate(loginDto);

            //Assert
            token.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "Secret key missing."));
        }

        [TestMethod]
        public void Authenticate_WhenLoginAndPasswordAreValid_ShouldGenerateToken()
        {
            //Arrange
            var login = "Login";
            var password = "Password";
            _repository.GetByLogin(Arg.Is(login)).Returns(new User(Guid.NewGuid(), "Name", login, Hash.Generate(password)));

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

        [TestMethod]
        public void ValidateToken_WhenTokenIsNullOrEmpty_ShouldReturnNull()
        {
            //Arrange
            string token1 = null;
            var token2 = "";

            //Act
            var result1 = _service.ValidateToken(token1);
            var result2 = _service.ValidateToken(token2);

            //Assert
            result1.Should().BeNull();
            result2.Should().BeNull();
            _notifier.Received(2).Handle(Arg.Is<Notification>(n => n.Message == "Token is missing."));
        }

        [TestMethod]
        public void ValidateToken_WhenSecretIsEmpty_ShouldReturnNull()
        {
            //Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjI2NThFNUQyLUQ0NUItNDI5MC05RkRBLTVENzA4MkQzNEI3MyIsImxvZ2luIjoiTG9naW4ifQ.TXgEUAMG71JUwvM2EzSkUi38cycRvYzKCunVqMM_Abc";

            //Act
            var result = _service.ValidateToken(token);

            //Assert
            result.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "Secret key missing."));
        }

        [TestMethod]
        public void ValidateToken_WhenLoginDoesntMatchId_ShouldReturnNull()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjI2NThFNUQyLUQ0NUItNDI5MC05RkRBLTVENzA4MkQzNEI3MyIsImxvZ2luIjoiV3JvbmdMb2dpbiJ9.1sW_jp4VGvCESmyzlXW00tJa8GZPqLZhX1NyybKuD34";

            var configurationSection = Substitute.For<IConfigurationSection>();
            _configuration.GetSection("Secret").Returns(configurationSection);
            configurationSection.Value.Returns("UsersSecret");

            var id = Guid.Parse("2658E5D2-D45B-4290-9FDA-5D7082D34B73");
            _repository.GetById(Arg.Is(id)).Returns(new User(id, "Name", "Login", "Password"));

            //Act
            var result = _service.ValidateToken(token);

            //Assert
            result.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "The informed login doesn't correspond with the user id."));
        }

        [TestMethod]
        public void ValidateToken_WhenLoginMatchesId_ShouldReturnUser()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjI2NThFNUQyLUQ0NUItNDI5MC05RkRBLTVENzA4MkQzNEI3MyIsImxvZ2luIjoiTG9naW4ifQ.TXgEUAMG71JUwvM2EzSkUi38cycRvYzKCunVqMM_Abc";

            var configurationSection = Substitute.For<IConfigurationSection>();
            _configuration.GetSection("Secret").Returns(configurationSection);
            configurationSection.Value.Returns("UsersSecret");

            var id = Guid.Parse("2658E5D2-D45B-4290-9FDA-5D7082D34B73");
            _repository.GetById(Arg.Is(id)).Returns(new User(id, "Name", "Login", "Password"));

            //Act
            var result = _service.ValidateToken(token);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Login.Should().Be("Login");
            result.Name.Should().Be("Name");
        }

        [TestMethod]
        public void ValidateToken_WhenThrowsException_ShouldReturnNull()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjI2NThFNUQyLUQ0NUItNDI5MC05RkRBLTVENzA4MkQzNEI3MyIsImxvZ2luIjoiTG9naW4ifQ.TXgEUAMG71JUwvM2EzSkUi38cycRvYzKCunVqMM_Abc";

            var configurationSection = Substitute.For<IConfigurationSection>();
            _configuration.GetSection("Secret").Returns(configurationSection);
            configurationSection.Value.Returns("UsersSecret");

            //Act
            var result = _service.ValidateToken(token);

            //Assert
            result.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message.Contains("An error has occurred: ")));
        }
    }
}
