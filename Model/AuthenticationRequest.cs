using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MS.Poc.Server.Model
{
    public class AuthenticationRequest
    {
        [Required]
        public string Username { get; set; }

        [JsonIgnore]
        public string DistinguishedName { get; set; } // TODO: create mapping repository - Username => DistinguishedName

        [Required]
        public string Password { get; set; }
    }
}
