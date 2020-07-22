using System.ComponentModel;

namespace MS.Poc.Server.Extensions
{
    public static class StringExtension
    {
        public static T Parse<T>(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return default;

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertTo(typeof(T)) && converter.CanConvertFrom(typeof(string)))
                return (T)converter.ConvertFromString(source);

            return default;
        }

        public static bool TryParse<T>(this string source, out T value)
        {
            value = default;
            if (string.IsNullOrWhiteSpace(source))
                return false;

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertTo(typeof(T)) && converter.CanConvertFrom(typeof(string)))
            {
                value = (T)converter.ConvertFromString(source);
                return true;
            }

            return false;
        }

        //public static string DnFilter(this string userDN)
        //{
        //    return $"(&(objectCategory=person)(objectClass=user)(sAMAccountType=805306368)(!(userAccountControl:1.2.840.113556.1.4.803:=2))(dn={userDN}))";
        //}

        public static string SAMAccountNameFilter(this string username)
        {
            return $"(&(objectCategory=person)(objectClass=user)(sAMAccountType=805306368)(!(userAccountControl:1.2.840.113556.1.4.803:=2))(sAMAccountName={username}))";
        }
    }
}
