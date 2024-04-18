using System.Security.Cryptography;

namespace Services.Implementation
{
    public static class EncryptDecrypt
    {
        private static readonly string EncryptionKey = GenerateRandomKey(256);
        private static readonly char[] Digits = "0123456789".ToCharArray();
        private static readonly char[] Alphabets = "ABCDEFGHIJ".ToCharArray();

        public static string Encrypt(string plainText)
        {
            return new string(plainText.Select(ch => EncryptCharacter(ch)).ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            return new string(cipherText.Select(ch => DecryptCharacter(ch)).ToArray());
        }

        private static char EncryptCharacter(char ch)
        {
            int index = Array.IndexOf(Digits, ch);
            return index >= 0 ? Alphabets[index] : ch;
        }

        private static char DecryptCharacter(char ch)
        {
            int index = Array.IndexOf(Alphabets, ch);
            return index >= 0 ? Digits[index] : ch;
        }

       

        private static byte[] GenerateRandomIV()
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateIV();
                return aesAlg.IV;
            }
        }

        private static string GenerateRandomKey(int keySizeInBits)
        {
            // Convert the key size to bytes
            int keySizeInBytes = keySizeInBits / 8;

            // Create a byte array to hold the random key
            byte[] keyBytes = new byte[keySizeInBytes];

            // Use a cryptographic random number generator to fill the byte array
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }

            // Convert the byte array to a base64-encoded string for storage
            return Convert.ToBase64String(keyBytes);
        }

    }
}