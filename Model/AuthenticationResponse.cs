using MS.Poc.Server.Core;

namespace MS.Poc.Server.Model
{
    public class AuthenticationResponse
    {
        public IUser User { get; private set; }
        public string Token { get; private set; }

        public AuthenticationResponse(IUser user, string token)
        {
            User = user;
            Token = token;
        }
    }
}
