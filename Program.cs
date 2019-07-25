using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Console app to generate security key manually");
            while (true)
            {
                Console.Write("Enter the key length (12 - SecurityKey, 16 - RenewalKey): ");
                var input = Console.ReadLine();
                int length = 0;
                int.TryParse(input, out length);
                if (length == 0)
                    break;
                var key = GenerateSecurityKey(length);
                Console.WriteLine("Key(Length = {0}): {1}", length, key);
            }
        }

        private static string GenerateSecurityKey(int length)
        {
            string securityID = "";

            for (int i = 0; i < length; ++i)
            {
                Random random = RealRandomSeed.GetRandom();

                switch (random.Next(1, 2 + 1))
                {
                    case 1:
                        // Number
                        securityID += (char)(random.Next(48, 57 + 1));
                        break;
                    case 2:
                        // lowercase letter (a-f)
                        // Letters are limited to a-f to avoid random undesirable spelling
                        // combinations
                        securityID += (char)(random.Next(97, 102 + 1));
                        break;
                        // No uppercase letters because SQL comparisons are generally case-insensitive;
                        // Also we want to simplify this for customers in case they type it in manually
                }

                if (i > 0 && securityID[i] == securityID[i - 1])
                {
                    // It's a dupe of the last character - remove and try again
                    securityID = securityID.Substring(0, securityID.Length - 1);
                    --i;
                }
            }

            return securityID;
        }

    }

    public static class RealRandomSeed
    {
        public static Random GetRandom()
        {
            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (randomBytes[0] & 0x7f) << 24 |
                        randomBytes[1] << 16 |
                        randomBytes[2] << 8 |
                        randomBytes[3];

            return new Random(seed);
        }

        //creates a random hex string of length * 2
        public static string GenerateRandomAlphaNumericString(int length)
        {
            string result = "";

            for (int i = 0; i < length; ++i)
            {
                var random = GetRandom();
                var number = random.Next(0, 257);
                result += number.ToString("X2").ToLower();
            }

            return result;
        }


    }
}

