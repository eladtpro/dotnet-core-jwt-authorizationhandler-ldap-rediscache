using System.Collections.Generic;

namespace MS.Poc.Server.Core
{

    public interface IUser
    {
        string FirstName { get; }
        string LastName { get; }
        string DisplayName { get; }
        string Email { get; }
        string Username { get; }
        string DistinguishedName { get; }
        IList<string> Roles { get; set; }
    }
}
