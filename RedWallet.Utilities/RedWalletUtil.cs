using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Utilities
{
    public static class RedWalletUtil
    {

        // https://stackoverflow.com/a/9995303/237858
        public static byte[] FromHexToByteArray(this string hex)
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
            int val = hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
        public static string FromByteArrayToHex(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        // hashing 
        public static string ToSHA256(this string str)
        {
            return BitConverter.ToString(Encoding.UTF8.GetBytes(str).ToSHA256()).Replace("-", "");
        }

        public static byte[] ToSHA256(this byte[] bytes)
        {
            var hash = new SHA256Managed();
            return hash.ComputeHash(bytes);
        }

        // string encryption
        // https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
        public static string EncryptString(string plainText, string passphrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passphrase, saltStringBytes, 1000))
            {
                var keyBytes = password.GetBytes(256 / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string DecryptString(string cipherText, string passphrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(256 / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(256 / 8).Take(256 / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((256 / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((256 / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passphrase, saltStringBytes, 1000))
            {
                var keyBytes = password.GetBytes(256 / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
        public static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 bytes will be 256 bits
            using (var rng = new RNGCryptoServiceProvider())
            {
                // fill the array with cryptographically secure random bytes
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

    }

    /*public static class OlympExt
    {
        // json
        private static JsonSerializerSettings jsonSettings(bool includeNulls = false, bool ignoreReferences = false)
        {
            var sett = UtilJson.Settings;
            if (includeNulls)
                sett.NullValueHandling = NullValueHandling.Include;
            if (ignoreReferences)
                sett.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return sett;
        }

        public static string ToJsonStr(this object source, bool includeNulls = false, bool ignoreReferences = false)
        {
            var sett = jsonSettings(includeNulls, ignoreReferences);
            return UtilJson.Serialize(source, sett);
        }

        public static string ToJsonPretty(this object source, bool includeNulls = false, bool ignoreReferences = false)
        {
            var sett = jsonSettings(includeNulls, ignoreReferences);
            sett.Formatting = Formatting.Indented;
            return UtilJson.Serialize(source, sett);
        }


        
    }*/
}
