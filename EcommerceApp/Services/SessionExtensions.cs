using System;
using Microsoft.AspNetCore.Http;

namespace EcommerceApp.Services
{
    public static class CustomSessionExtensions
    {
        public static void SetCustomString(this ISession session, string key, string value)
        {
            session.Set(key, System.Text.Encoding.UTF8.GetBytes(value));
        }

        public static string? GetCustomString(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return System.Text.Encoding.UTF8.GetString(data);
        }
    }
}
