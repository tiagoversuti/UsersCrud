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
            var user = new User(Guid.NewGuid(), "Name", "Login", "Password");
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
            var user = new User(Guid.NewGuid(), "Name", "Login", "Password");
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
            var user = new User(Guid.NewGuid(), "Name", "Login", "Password");
            var hashedPassword = user.Password;
            var newPassword = "NewPassword";

            //Act
            user.ChangePassword(newPassword);

            //Assert
            user.Password.Should().NotBe(hashedPassword);
        }

        [TestMethod]
        public void ChangeName_ShouldChangeName()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Name", "Login", "Password");
            var name = "New Name";

            //Act
            user.ChangeName(name);

            //Assert
            user.Name.Should().Be("New Name");
        }
    }
}
