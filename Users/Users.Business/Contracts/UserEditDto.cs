using System;

namespace Users.Business.Contracts
{
    public class UserEditDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}
