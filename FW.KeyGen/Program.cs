using System;
using System.Reflection;
using System.Security.Cryptography;

namespace FW.KeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown Company";
            var product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Unknown Product";
            var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "Unknown Description";
            var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "Unknown Copyright";
            Console.WriteLine($"{product} v{version} by {company}, {copyright}");
            Console.WriteLine($"Description: {description}");
            Console.WriteLine();
            string jwtKey = GenerateSecureKey(32);
            Console.WriteLine($"Your JWT Key:   {jwtKey}");
            Console.WriteLine();
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