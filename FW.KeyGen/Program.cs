using System;
using System.Reflection;
using System.Security.Cryptography;

namespace FW.KeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"FleetWrencher Key Gen v{version}");

            string jwtKey = GenerateSecureKey(32);
            Console.WriteLine($"Your JWT Key: {jwtKey}");
            Console.WriteLine($"Press any key to exit...");
            Console.ReadLine();
        }

        public static string GenerateSecureKey(int length = 32)
        {
            var key = new byte[length];
            RandomNumberGenerator.Fill(key);
            return Convert.ToBase64String(key);
        }
    }
}