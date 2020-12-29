using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Users.Business.Contracts;
using Users.Business.Interfaces;
using Users.Business.Models;
using Users.Business.Services;

namespace Users.UnitTests.Business.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private readonly UserService _service;

        private readonly IUserRepository _repository;

        public UserServiceTest()
        {
            _repository = Substitute.For<IUserRepository>();

            _service = new UserService(_repository);
        }

        [TestMethod]
        public void GetById_WhenIdIsInvalid_ShouldThrowException()
        {
            //Arrange
            var validId = Guid.NewGuid();
            _repository.GetById(Arg.Is(validId)).Returns(new User(validId, "Name", "Login", "Password"));

            var invalidId = Guid.NewGuid();

            //Act
            Action action = () => _service.GetById(invalidId);

            //Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("User not found.");
        }

        [TestMethod]
        public void GetById_WhenIdIsValid_ShouldReturnUser()
        {
            //Arrange
            var validId = Guid.NewGuid();
            _repository.GetById(Arg.Is(validId)).Returns(new User(validId, "Name", "Login", "Password"));

            //Act
            var user = _service.GetById(validId);

            //Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(validId);
            user.Name.Should().Be("Name");
            user.Login.Should().Be("Login");
        }

        [TestMethod]
        public void GetAll_ShouldReturnUsers()
        {
            //Arrange
            _repository.GetAll().Returns(new List<User>
            {
                new User(Guid.NewGuid(), "Name 1", "Login 1", "Password 1"),
                new User(Guid.NewGuid(), "Name 2", "Login 2", "Password 2")
            });

            //Act
            var users = _service.GetAll();

            //Assert
            users.Should().HaveCount(2);
            users.ElementAt(0).Name.Should().Be("Name 1");
            users.ElementAt(0).Login.Should().Be("Login 1");
            users.ElementAt(1).Name.Should().Be("Name 2");
            users.ElementAt(1).Login.Should().Be("Login 2");
        }

        [TestMethod]
        public void Create_WhenNameOrLoginExists_ShouldThrowException()
        {
            //Arrange
            var userDto = new UserCreateDto
            {
                Name = "Name",
                Login = "Login",
                Password = "Password"
            };

            _repository.Search(Arg.Any<Expression<Func<User, bool>>>()).Returns(new List<User>
            {
                new User(Guid.NewGuid(), "Name", "Login", "Password")
            });

            //Act
            Action action = () => _service.Create(userDto);

            //Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("There already exists an user with this name or login.");
        }

        [TestMethod]
        public void Create_WhenNameAndLoginDontExist_ShouldCreateUser()
        {
            //Arrange
            var userDto = new UserCreateDto
            {
                Name = "Name",
                Login = "Login",
                Password = "Password"
            };

            _repository.Search(Arg.Any<Expression<Func<User, bool>>>()).Returns(new List<User>());

            //Act
            var user = _service.Create(userDto);

            //Assert
            _repository.Received(1).Add(Arg.Any<User>());

            user.Name.Should().Be("Name");
            user.Login.Should().Be("Login");
        }

        [TestMethod]
        public void Update_WhenUserDoesntExists_ShouldThrowException()
        {
            //Arrange
            var id = Guid.NewGuid();

            var userDto = new UserEditDto
            {
                Id = id,
                Name = "Name",
                OldPassword = "OldPassword",
                NewPassword = "NewPassword",
                NewPasswordConfirm = "NewPassword"
            };

            _repository.GetById(Arg.Is(id)).Returns(null as User);

            //Act
            Action action = () => _service.Update(userDto);

            //Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("No user found with this id.");
        }

        [TestMethod]
        public void Update_WhenNewPasswordsDontMatch_ShouldThrowException()
        {
            //Arrange
            var id = Guid.NewGuid();

            var userDto = new UserEditDto
            {
                Id = id,
                Name = "Name",
                OldPassword = "Password",
                NewPassword = "NewPassword",
                NewPasswordConfirm = "NewPasswordWrong"
            };

            _repository.GetById(Arg.Is(id)).Returns(new User(id, "Name", "Login", "$2a$12$LzA4TO2jjgYypiUUluvGqO/PtDDHlmbm9GJNcm6hTBVx6.FZzHY12"));

            //Act
            Action action = () => _service.Update(userDto);

            //Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("The new password confirmation is different than the new password");
        }

        [TestMethod]
        public void Update_WhenCurrentPasswordDoesntMatch_ShouldThrowException()
        {
            //Arrange
            var id = Guid.NewGuid();

            var userDto = new UserEditDto
            {
                Id = id,
                Name = "Name",
                OldPassword = "WrongPassword",
                NewPassword = "NewPassword",
                NewPasswordConfirm = "NewPassword"
            };

            _repository.GetById(Arg.Is(id)).Returns(new User(id, "Name", "Login", "$2a$12$LzA4TO2jjgYypiUUluvGqO/PtDDHlmbm9GJNcm6hTBVx6.FZzHY12"));

            //Act
            Action action = () => _service.Update(userDto);

            //Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("The old password is wrong.");
        }

        [TestMethod]
        public void Update_WhenAllfieldsMatch_ShouldUpdateUser()
        {
            //Arrange
            var id = Guid.NewGuid();

            var userDto = new UserEditDto
            {
                Id = id,
                Name = "New Name",
                OldPassword = "Password",
                NewPassword = "NewPassword",
                NewPasswordConfirm = "NewPassword"
            };

            _repository.GetById(Arg.Is(id)).Returns(new User(id, "Name", "Login", "$2a$12$LzA4TO2jjgYypiUUluvGqO/PtDDHlmbm9GJNcm6hTBVx6.FZzHY12"));

            //Act
            var user = _service.Update(userDto);

            //Assert
            _repository.Received(1).Update(Arg.Any<User>());

            user.Name.Should().Be("New Name");
            user.Login.Should().Be("Login");
        }

        [TestMethod]
        public void Delete_WhenIdDoesntExist_ShouldThrowException()
        {
            //Arrange
            var validId = Guid.NewGuid();
            _repository.GetById(Arg.Is(validId)).Returns(new User(validId, "Name", "Login", "Password"));

            var invalidId = Guid.NewGuid();

            //Act
            Action action = () => _service.Delete(invalidId);

            //Assert
            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("User not found.");
        }

        [TestMethod]
        public void Delete_WhenIdExists_ShouldDeleteUser()
        {
            //Arrange
            var validId = Guid.NewGuid();
            _repository.GetById(Arg.Is(validId)).Returns(new User(validId, "Name", "Login", "Password"));

            //Act
            _service.Delete(validId);

            //Assert
            _repository.Received(1).Delete(Arg.Any<User>());
        }
    }
}
