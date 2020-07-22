using Novell.Directory.Ldap;
using System.Collections.Generic;
using System.Linq;

namespace MS.Poc.Server.Extensions
{
    public static class LdapEntryExtension
    {
        public static string Attribute(this LdapEntry entry, string attrName)
        {
            try
            {
                LdapAttribute attr = entry.GetAttribute(attrName);
                if (null == attr) return null;

                return attr.StringValue;
            }
            catch (KeyNotFoundException) { }
            return string.Empty;
        }

        public static IList<string> ArrayAttribute(this LdapEntry entry, string attrName)
        {
            try
            {
                LdapAttribute attr = entry.GetAttribute(attrName);
                if (null == attr) return null;

                return attr.StringValueArray.ToList();
            }
            catch { }
            return new string[0];
        }
    }
}
