﻿using Microsoft.Extensions.Options;
using Quiz.Exam.Web.Configuration;
using System.Security.Cryptography;

namespace Quiz.Exam.Web.Helper
{
    public class TokenGenerator(
       IOptions<AppConfiguration> appConfiguration)
    {
        public static string GenerateCryptographicallySecureGuid()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[16];
            rng.GetBytes(bytes);
            return new Guid(bytes).ToString("N");
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
