using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Linq;
using System.IO;

namespace block_io_lib
{
    class Helper
    {
        public static string ComputeSha256Hash(byte[] rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(rawData);

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static string Encrypt(string data, string key)
        {
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                byte[] keyArr = Convert.FromBase64String(key);
                byte[] KeyArrBytes32Value = new byte[32];
                Array.Copy(keyArr, KeyArrBytes32Value, 24);
                csp.Key = keyArr;
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform encrypter = csp.CreateEncryptor();
                return Convert.ToBase64String(encrypter.TransformFinalBlock(ASCIIEncoding.UTF8.GetBytes(data), 0, ASCIIEncoding.UTF8.GetBytes(data).Length));
            }
        }

        public static string Decrypt(string data, string key)
        {
            string plaintext = null;
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                byte[] keyArr = Convert.FromBase64String(key);
                byte[] KeyArrBytes32Value = new byte[32];
                Array.Copy(keyArr, KeyArrBytes32Value, 24);
                csp.Key = Convert.FromBase64String(key);
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform decrypter = csp.CreateDecryptor();

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(data)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decrypter, CryptoStreamMode.Read))
                    {

                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return plaintext;
            }
        }

        private static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static string PinToAesKey(string pin)
        {
            byte[] salt = new byte[0]; //empty salt

            string firstHash = ByteArrayToString(KeyDerivation.Pbkdf2(
            password: pin,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1024,
            numBytesRequested: 16));


            byte[] key = KeyDerivation.Pbkdf2(
            password: firstHash.ToLower(),
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1024,
            numBytesRequested: 32);

            return Convert.ToBase64String(key);
        }

        public static Key ExtractKeyFromEncryptedPassphrase(string EncryptedData, string B64Key)
        {
            string Decrypted = Decrypt(EncryptedData, B64Key);
            byte[] Unhexlified = StringToByteArray(Decrypted);
            
            return new Key(Encoding.ASCII.GetBytes(ComputeSha256Hash(Unhexlified)));
        }

        public static dynamic[] SignInputs(Key PrivKey, dynamic[] Inputs)
        {
            var PubKey = PrivKey.PubKey.ToHex();
            foreach (dynamic input in Inputs)
            {
                foreach(dynamic signer in input.signers)
                {
                    if (signer.signer_public_key == PubKey)
                    {
                        signer.signed_data = PrivKey.Sign(input.data_to_sign);
                    }

                }
            }
            return Inputs;
        }
    }
}
