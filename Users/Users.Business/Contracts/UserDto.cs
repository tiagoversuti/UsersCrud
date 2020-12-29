using System;

namespace Users.Business.Contracts
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
    }
}
