using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using NBitcoin;

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

        public static string Encrypt(string PlainText, string KeyStr)
        {
            AesManaged aes = new AesManaged();
            aes.KeySize = 256;
 
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            byte[] keyArr = Convert.FromBase64String(KeyStr);
            byte[] KeyArrBytes32Value = new byte[32];
            Array.Copy(keyArr, KeyArrBytes32Value, 24);

            // Initialization vector.   
            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, 16);

            aes.Key = KeyArrBytes32Value;
            aes.IV = IVBytes16Value;

            ICryptoTransform encrypto = aes.CreateEncryptor();

            byte[] plainTextByte = ASCIIEncoding.UTF8.GetBytes(PlainText);
            byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
            return Convert.ToBase64String(CipherText);
        }

        public static string Decrypt(string CipherText, string KeyStr)
        {
            AesManaged aes = new AesManaged();
            aes.KeySize = 256;

            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            byte[] keyArr = Convert.FromBase64String(KeyStr);
            byte[] KeyArrBytes32Value = new byte[32];
            Array.Copy(keyArr, KeyArrBytes32Value, 24);

            // Initialization vector.   
            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, 16);

            aes.Key = KeyArrBytes32Value;
            aes.IV = IVBytes16Value;

            ICryptoTransform decrypto = aes.CreateDecryptor();

            byte[] encryptedBytes = Convert.FromBase64CharArray(CipherText.ToCharArray(), 0, CipherText.Length);
            byte[] decryptedData = decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return ASCIIEncoding.UTF8.GetString(decryptedData);
        }

        public static string PinToKey(string Pin, string Salt = "", int Iterations = 2048)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] salt;
            if (Salt != "")
            {
                salt = Encoding.ASCII.GetBytes(Salt);
                provider.GetBytes(salt);
            }
            else
            {
                int SALT_SIZE = 24;
                salt = new byte[SALT_SIZE];
                provider.GetBytes(salt);
            }

            // Generate the hash
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(Pin), salt, (int) Iterations / 2, HashAlgorithmName.SHA256);

            //BitConverter and Replace used to convert to hex string
            pbkdf2 = new Rfc2898DeriveBytes(BitConverter.ToString(pbkdf2.GetBytes(16)).Replace("-", ""), salt, (int)Iterations / 2, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(pbkdf2.GetBytes(32));
        }

        public static Key ExtractKeyFromEncryptedPassphrase(string EncryptedData, string B64Key)
        {
            string Decrypted = Decrypt(EncryptedData, B64Key);
            byte[] Unhexlified = StringToByteArray(Decrypted);
            
            return new Key(Encoding.ASCII.GetBytes(ComputeSha256Hash(Unhexlified)));
        }

        public static List<SigInput> SignInputs(Key PrivKey, List <SigInput> Inputs)
        {
            var PubKey = PrivKey.PubKey.ToHex();
            foreach (SigInput input in Inputs)
            {
                foreach(Signer signer in input.signers)
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
