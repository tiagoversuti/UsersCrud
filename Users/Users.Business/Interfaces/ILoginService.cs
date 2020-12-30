using Users.Business.Contracts;

namespace Users.Business.Interfaces
{
    public interface ILoginService
    {
        string Authenticate(LoginDto loginDto);

        UserDto ValidateToken(string token);
    }
}
