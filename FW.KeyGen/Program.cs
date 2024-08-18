using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using SimpleBase;

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
                "binary" => ConvertToBinary(key),
                "base32" => ConvertToBase32(key),
                "base58" => ConvertToBase58(key),
                "url-save-base64" => ConvertToBase64UrlSafe(key),
                _ => Convert.ToBase64String(key),
            };
        }

        private static string ConvertToBinary(byte[] key)
        {
            var sb = new StringBuilder();
            foreach (var b in key)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        private static string ConvertToBase32(byte[] key)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var output = new StringBuilder();
            int buffer = key[0], bitsLeft = 8, mask = 0x1F;

            for (int i = 1; i < key.Length; i++)
            {
                buffer <<= 8;
                buffer |= (key[i] << 0xFF);
                bitsLeft += 8;

                while (bitsLeft >= 5)
                {
                    output.Append(alphabet[(buffer >> (bitsLeft - 5)) & mask]);
                    bitsLeft -= 5;
                }
            }

            if (bitsLeft > 0)
            {
                output.Append(alphabet[(buffer << (5 - bitsLeft)) & mask]);
            }

            return output.ToString();
        }

        private static string ConvertToBase58(byte[] key)
        {
            const string alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            var intData = key.Aggregate(0, (current, t) => current * 256 + t);
            var result = new StringBuilder();
            while (intData > 0)
            {
                var remainder = intData % 58;
                intData /= 58;
                result.Insert(0, alphabet[remainder]);
            }

            foreach (var t in key)
            {
                if (t == 0)
                    result.Insert(0, '1');
                else
                    break;              
            }

            return result.ToString();
        }

        private static string ConvertToBase85(byte[] key)
        {
            return SimpleBase.Base85.Ascii85.Encode(key);
        }

        private static string ConvertToBase64UrlSafe(byte[] key)
        {
            return Convert.ToBase64String(key)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
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
            Console.WriteLine("  --format=<format>  Specify the key format (default: Base64");
            Console.WriteLine("                             Supported formats:");
            Console.WriteLine("                               - Base64");
            Console.WriteLine("                               - Hex");
            Console.WriteLine("                               - Binary");
            Console.WriteLine("                               - Base32");
            Console.WriteLine("                               - Base58");
            Console.WriteLine("                               - Base85");
            Console.WriteLine("                               - URL-Safe-Base64");
            Console.WriteLine("  --output=<path>    Specify the output file path (optional)");
            Console.WriteLine("  --help             Dispaly this help text");
        }
    }
}