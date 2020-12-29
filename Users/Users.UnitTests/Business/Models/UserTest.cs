using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Users.Business.Models;

namespace Users.UnitTests.Business.Models
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void ValidatePassword_WhenPasswordIsCorrect_ShouldReturnTrue()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Name", "Login", "$2a$12$LzA4TO2jjgYypiUUluvGqO/PtDDHlmbm9GJNcm6hTBVx6.FZzHY12");
            var password = "Password";

            //Act
            var result = user.ValidatePassword(password);

            //Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void ValidatePassword_WhenPasswordIsIncorrect_ShouldReturnFalse()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Name", "Login", "$2a$12$LzA4TO2jjgYypiUUluvGqO/PtDDHlmbm9GJNcm6hTBVx6.FZzHY12");
            var password = "WrongPassword";

            //Act
            var result = user.ValidatePassword(password);

            //Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void ChangePassword_ShouldChangePassword()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Name", "Login", "$2a$12$LzA4TO2jjgYypiUUluvGqO/PtDDHlmbm9GJNcm6hTBVx6.FZzHY12");
            var password = "NewPassword";

            //Act
            user.ChangePassword(password);

            //Assert
            user.Password.Should().NotBe("$2a$12$LzA4TO2jjgYypiUUluvGqO/PtDDHlmbm9GJNcm6hTBVx6.FZzHY12");
        }

        [TestMethod]
        public void ChangeName_ShouldChangeName()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Name", "Login", "$2a$12$LzA4TO2jjgYypiUUluvGqO/PtDDHlmbm9GJNcm6hTBVx6.FZzHY12");
            var name = "New Name";

            //Act
            user.ChangeName(name);

            //Assert
            user.Name.Should().Be("New Name");
        }
    }
}
