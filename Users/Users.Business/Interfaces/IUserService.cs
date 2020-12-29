using System;
using System.Collections.Generic;
using Users.Business.Contracts;

namespace Users.Business.Interfaces
{
    public interface IUserService
    {
        UserDto GetById(Guid id);

        List<UserDto> GetAll();

        UserDto Create(UserCreateDto userDto);

        UserDto Update(UserEditDto userDto);

        void Delete(Guid id);
    }
}
