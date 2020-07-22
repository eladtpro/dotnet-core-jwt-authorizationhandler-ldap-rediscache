using MS.Poc.Server.Model;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MS.Poc.Server.Extensions;
using MS.Poc.Server.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Security;
using MS.Poc.Server.Core;

namespace MS.Poc.Server.Repositories
{
    class UserRepository : IUserRepository, IDisposable
    {
        static readonly string[] Attributes = new[] { "givenName", "sn", "displayName", "mail", "sAMAccountName", "memberOf" };
        private readonly Lazy<LdapConnection> connection;
        private readonly IConfiguration configuration;
        private readonly IDistributedCache cache;
        private readonly IList<string> roles;
        private readonly TimeSpan slidingExpiration;

        ILdapConnection Connection => connection.Value;

        public UserRepository(IConfiguration configuration, IDistributedCache cache)
        {
            this.cache = cache;
            this.configuration = configuration;

            string admin = configuration["Authorization:Roles:Admin"];
            string user = configuration["Authorization:Roles:User"];

            roles = new[] { admin, user};
            connection = new Lazy<LdapConnection>(() =>
            {
                LdapConnection ldap = new LdapConnection() { SecureSocketLayer = configuration["Ldap:SecureSocketLayer"].Parse<bool>() };
                ldap.Connect(configuration["Ldap:Host"], configuration["Ldap:Port"].Parse<int>());
                ldap.Bind(configuration["Ldap:QueryDN"], configuration["Ldap:QueryPassword"]);
                return ldap;
            });
            slidingExpiration = TimeSpan.Parse(configuration["Jwt:Expiry"]);
        }

        public async Task<IUser> Authenticate(AuthenticationRequest request)
        {
            IUser user = await Find(request.Username);
            if (null == user) throw new SecurityException("invalid credentials");

            request.DistinguishedName = user.DistinguishedName;

            // TODO: make async
            using (LdapConnection ldap = new LdapConnection())
            {
                ldap.SecureSocketLayer = configuration["Ldap:SecureSocketLayer"].Parse<bool>();
                ldap.Connect(configuration["Ldap:Host"], configuration["Ldap:Port"].Parse<int>());
                ldap.Bind(request.DistinguishedName, request.Password);
            }
            return user;
        }

        public async Task<IUser> Find(string username)
        {
            IUser user = await cache.GetAsync<IUser>(username);
            if (null != user) return user;

            string filter = username.SAMAccountNameFilter();
            if (!Connection.Connected)
            {
                Connection.Connect(configuration["Ldap:Host"], configuration["Ldap:Port"].Parse<int>());
                Connection.Bind(configuration["Ldap:QueryDN"], configuration["Ldap:QueryPassword"]);
            }
            IList<LdapEntry> entries = await Search(filter);
            LdapEntry entry = entries.FirstOrDefault();

            if (null == entry) return null;

            user = Convert(entry);
            await cache.SetAsync(username, user, new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration });
            return user;
        }

        async Task<IList<LdapEntry>> Search(string filter)
        {
            return await Task.Run(() =>
            {
                ILdapSearchResults results = Connection.Search(
                configuration["Ldap:Root"],
                LdapConnection.ScopeSub,
                filter,
                Attributes,
                false,
                (LdapSearchConstraints)null); // TODO: adjust LdapSearchConstraints

                List<LdapEntry> entries = results.Where(e => null != e).ToList();
                return entries.AsReadOnly();
            });
        }

        private IUser Convert(LdapEntry entry)
        {
            if (null == entry)
                return null;

            User user = new User
            {
                DistinguishedName = entry.Dn,
                DisplayName = entry.Attribute("displayName"),
                Email = entry.Attribute("mail"),
                FirstName = entry.Attribute("givenName"),
                LastName = entry.Attribute("sn"),
                Username = entry.Attribute("sAMAccountName"),
                Roles = entry.ArrayAttribute("memberOf").Where(r => roles.Contains(r)).ToList()
            };
            return user;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UserRepository()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            Connection.Dispose();
        }
    }
}
