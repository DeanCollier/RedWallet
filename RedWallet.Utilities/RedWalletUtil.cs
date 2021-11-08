using System;
using System.Collections.Generic;
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
