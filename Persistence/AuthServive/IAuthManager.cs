using QulixTest.Core.Model;

namespace QulixTest.Persistence.AuthServive
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginUserDTO userDTO);
        Task<string> CreateToken();
    }
}
