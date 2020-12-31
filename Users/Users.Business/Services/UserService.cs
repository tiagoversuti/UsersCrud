using System;
using System.Collections.Generic;
using System.Linq;
using Users.Business.Contracts;
using Users.Business.Interfaces;
using Users.Business.Models;

namespace Users.Business.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository, INotifier notifier) : base(notifier)
        {
            _repository = repository;
        }

        public UserDto GetById(Guid id)
        {
            var user = _repository.GetById(id);

            if (user is null)
            {
                Notify("User not found.");
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name
            };
        }

        public List<UserDto> GetAll()
        {
            var users = _repository.GetAll();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Login = u.Login,
                Name = u.Name
            }).ToList();
        }

        public UserDto Create(UserCreateDto userDto)
        {
            if (_repository.Search(u => u.Name == userDto.Name || u.Login == userDto.Login)
                .Any())
            {
                Notify("There already exists an user with this name or login.");
                return null;
            }

            var user = new User(Guid.NewGuid(), userDto.Name, userDto.Login, Hash.Generate(userDto.Password));
            _repository.Add(user);

            return new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name
            };
        }

        public UserDto Update(UserEditDto userDto)
        {
            var user = _repository.GetById(userDto.Id);

            if (user is null)
            {
                Notify("No user found with this id.");
                return null;
            }

            if (userDto.NewPassword != userDto.NewPasswordConfirm)
            {
                Notify("The new password confirmation is different than the new password.");
                return null;
            }

            if (!user.ValidatePassword(userDto.OldPassword))
            {
                Notify("The old password is wrong.");
                return null;
            }

            user.ChangeName(userDto.Name);
            user.ChangePassword(userDto.NewPassword);

            _repository.Update(user);

            return new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name
            };
        }

        public void Delete(Guid id)
        {
            var user = _repository.GetById(id);

            if (user is null)
            {
                Notify("User not found.");
            }
            else
                _repository.Delete(user);
        }
    }
}
