using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Users.Business.Models;

namespace Users.Business.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);

        void Update(User user);

        User GetById(Guid id);

        User GetByLogin(string login);

        List<User> GetAll();

        IEnumerable<User> Search(Expression<Func<User, bool>> predicate);

        void Delete(User user);
    }
}
