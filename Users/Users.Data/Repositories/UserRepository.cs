using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Users.Business.Interfaces;
using Users.Business.Models;
using Users.Data.Context;

namespace Users.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected readonly UsersContext _context;

        public UserRepository(UsersContext context)
        {
            _context = context;
        }

        public void Add(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public User GetById(Guid id)
        {
            return _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetAll()
        {
            return _context.Users.AsNoTracking().ToList();
        }

        public IEnumerable<User> Search(Expression<Func<User, bool>> predicate)
        {
            return _context.Users.AsNoTracking().Where(predicate).ToList();
        }

        public void Delete(User user)
        {
            _context.Remove(user);
            _context.SaveChanges();
        }
    }
}
