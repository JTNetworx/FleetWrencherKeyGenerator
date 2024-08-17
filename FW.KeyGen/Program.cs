using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace FW.KeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--help")
            {
                ShowHelp();
                return;
            }

            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown Company";
            var product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Unknown Product";
            var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "Unknown Description";
            var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "Unknown Copyright";


            Console.WriteLine($"{product} v{version} by {company}, {copyright}");
            Console.WriteLine($"Description: {description}");

            Console.WriteLine();

            int length = 32;
            string format = "Base64";
            string outputFile = null;

            // Parse command-line args
            if (args.Length == 0)
            {
                Console.WriteLine("Enter key length (default 32): ");
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    length = result;
                }

                Console.Write("Enter format (Base64/Hex): ");
                var inputFormat = Console.ReadLine()?.ToLower();
                if (!string.IsNullOrEmpty(inputFormat))
                {
                    format = inputFormat;
                }

                Console.WriteLine("Enter output file path (optional): ");
                outputFile = Console.ReadLine();
            }
            else
            {
                // Parse command-line arguments
                foreach (var arg in args)
                {
                    if (arg.StartsWith("--length="))
                    {
                        length = int.Parse(arg.Substring("--length=".Length));
                    }
                    else if (arg.StartsWith("--format="))
                    {
                        format = arg.Substring("--format=".Length).ToLower();
                    }
                    else if (arg.StartsWith("--output"))
                    {
                        outputFile = arg.Substring("--output".Length);
                    }
                }
            }



            string jwtKey;
            do
            {
                jwtKey = GenerateSecureKey(length, format);
                if (!IsKeySecure(jwtKey))
                {
                    Console.WriteLine("Generated key did not meet the security criteria. Generating a new key...");
                }
            }while (!IsKeySecure(jwtKey));

            Console.WriteLine($"Your JWT Key:   {jwtKey}");
            Console.WriteLine();
            Console.WriteLine($"Security Level: {EvaluateSecurityLevel(jwtKey)}");
            Console.WriteLine();

            if (!string.IsNullOrEmpty(outputFile))
            {
                File.WriteAllText(outputFile, jwtKey);
                Console.WriteLine($"Key saved to {outputFile}");
            }

            Console.WriteLine();

            Console.WriteLine($"Press any key to exit...");

            Console.ReadLine();
        }

        public static string GenerateSecureKey(int length, string format)
        {
            var key = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            return format switch
            {
                "hex" => BitConverter.ToString(key).Replace("-", "").ToLower(),
                _ => Convert.ToBase64String(key),
            };
        }

        public static bool IsKeySecure(string key)
        {
            return key.Length >= 24 && HasUpperCase(key) && HasLowerCase(key) && HasDigit(key);
        }

        public static string EvaluateSecurityLevel(string key)
        {
            int score = 0;

            if (key.Length >= 24) score++;
            if (HasUpperCase(key)) score++;
            if (HasLowerCase(key)) score++;
            if (HasDigit(key)) score++;
            if (key.Any(c => "!@#$%^&*()".Contains(c))) score++;

            return score switch
            {
                5 => "Very High",
                4 => "High",
                3 => "Medium",
                2 => "Low",
                1 => "Very Low",
            };
        }

        private static bool HasUpperCase(string input) => input.Any(char.IsUpper);
        private static bool HasLowerCase(string input) => input.Any(char.IsLower);
        private static bool HasDigit(string input) => input.Any(char.IsNumber);

        private static void ShowHelp()
        {
            Console.WriteLine("FlletRencher Key Generator");
            Console.WriteLine("Usage:");
            Console.WriteLine("  --length=<number>  Specify the key length (default: 32)");
            Console.WriteLine("  --format=<format>  Specify the key format (Base64 or Hex, default: Base64");
            Console.WriteLine("  --output=<path>    Specify the output file path (optional)");
            Console.WriteLine("  --help             Dispaly this help text");
        }
    }
}