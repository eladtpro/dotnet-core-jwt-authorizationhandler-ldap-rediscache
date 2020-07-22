using MS.Poc.Server.Core;
using System;
using System.Collections.Generic;

namespace MS.Poc.Server.Model
{
    [Serializable]
    public class User : IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string DistinguishedName { get; set; }
        public IList<string> Roles { get; set; }
    }
}
