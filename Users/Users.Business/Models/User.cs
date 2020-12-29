using System;

namespace Users.Business.Models
{
    public class User : Entity
    {
        public User(Guid id, string name, string login, string password)
        {
            Id = id;
            Name = name;
            Login = login;
            Password = password;
        }

        public string Name { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }

        public bool ValidatePassword(string password)
        {
            return Hash.Verify(password, Password);
        }

        public void ChangePassword(string newPassword)
        {
            Password = Hash.Generate(newPassword);
        }

        public void ChangeName(string name)
        {
            Name = name;
        }
    }
}
