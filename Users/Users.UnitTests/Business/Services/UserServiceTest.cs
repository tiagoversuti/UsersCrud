using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Users.Business;
using Users.Business.Contracts;
using Users.Business.Interfaces;
using Users.Business.Models;
using Users.Business.Notifications;
using Users.Business.Services;

namespace Users.UnitTests.Business.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private readonly UserService _service;

        private readonly IUserRepository _repository;
        private readonly INotifier _notifier;

        public UserServiceTest()
        {
            _repository = Substitute.For<IUserRepository>();
            _notifier = Substitute.For<INotifier>();

            _service = new UserService(_repository, _notifier);
        }

        [TestMethod]
        public void GetById_WhenIdIsInvalid_ShouldReturnNull()
        {
            //Arrange
            var validId = Guid.NewGuid();
            _repository.GetById(Arg.Is(validId)).Returns(new User(validId, "Name", "Login", Hash.Generate("Password")));

            var invalidId = Guid.NewGuid();

            //Act
            var user = _service.GetById(invalidId);

            //Assert
            user.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "User not found."));
        }

        [TestMethod]
        public void GetById_WhenIdIsValid_ShouldReturnUser()
        {
            //Arrange
            var validId = Guid.NewGuid();
            _repository.GetById(Arg.Is(validId)).Returns(new User(validId, "Name", "Login", Hash.Generate("Password")));

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
                new User(Guid.NewGuid(), "Name 1", "Login 1", Hash.Generate("Password 1")),
                new User(Guid.NewGuid(), "Name 2", "Login 2", Hash.Generate("Password 2"))
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
        public void Create_WhenNameOrLoginExists_ShouldNotCreateUser()
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
                new User(Guid.NewGuid(), "Name", "Login", Hash.Generate("Password"))
            });

            //Act
            var user = _service.Create(userDto);

            //Assert
            user.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "There already exists an user with this name or login."));
            _repository.Received(0).Add(Arg.Any<User>());
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
        public void Update_WhenUserDoesntExists_ShouldNotUpdateUser()
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
            var user = _service.Update(userDto);

            //Assert
            user.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "No user found with this id."));
            _repository.Received(0).Add(Arg.Any<User>());
        }

        [TestMethod]
        public void Update_WhenNewPasswordsDontMatch_ShouldNotUpdateUser()
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

            _repository.GetById(Arg.Is(id)).Returns(new User(id, "Name", "Login", Hash.Generate("Password")));

            //Act
            var user = _service.Update(userDto);

            //Assert
            user.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "The new password confirmation is different than the new password."));
            _repository.Received(0).Add(Arg.Any<User>());
        }

        [TestMethod]
        public void Update_WhenCurrentPasswordDoesntMatch_ShouldNotUpdateUsern()
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

            _repository.GetById(Arg.Is(id)).Returns(new User(id, "Name", "Login", Hash.Generate("Password")));

            //Act
            var user = _service.Update(userDto);

            //Assert
            user.Should().BeNull();
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "The old password is wrong."));
            _repository.Received(0).Add(Arg.Any<User>());
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

            _repository.GetById(Arg.Is(id)).Returns(new User(id, "Name", "Login", Hash.Generate("Password")));

            //Act
            var user = _service.Update(userDto);

            //Assert
            _repository.Received(1).Update(Arg.Any<User>());

            user.Name.Should().Be("New Name");
            user.Login.Should().Be("Login");
        }

        [TestMethod]
        public void Delete_WhenIdDoesntExist_ShouldNotDeleteUser()
        {
            //Arrange
            var validId = Guid.NewGuid();
            _repository.GetById(Arg.Is(validId)).Returns(new User(validId, "Name", "Login", Hash.Generate("Password")));

            var invalidId = Guid.NewGuid();

            //Act
            _service.Delete(invalidId);

            //Assert
            _notifier.Received(1).Handle(Arg.Is<Notification>(n => n.Message == "User not found."));
            _repository.Received(0).Delete(Arg.Any<User>());
        }

        [TestMethod]
        public void Delete_WhenIdExists_ShouldDeleteUser()
        {
            //Arrange
            var validId = Guid.NewGuid();
            _repository.GetById(Arg.Is(validId)).Returns(new User(validId, "Name", "Login", Hash.Generate("Password")));

            //Act
            _service.Delete(validId);

            //Assert
            _repository.Received(1).Delete(Arg.Any<User>());
        }
    }
}
