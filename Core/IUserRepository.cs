using MS.Poc.Server.Model;
using System.Threading.Tasks;

namespace MS.Poc.Server.Core
{
    public interface IUserRepository
    {
        Task<IUser> Find(string username);

        Task<IUser> Authenticate(AuthenticationRequest request);
    }
}
