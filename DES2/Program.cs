using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DES2
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = "0000000011111111000000001111111100000000111111110000000011111111";
            var keyByte = StringToByte(key);
            var des = new SimpleDES(keyByte);
            

            var txt = "0000000000000000000000000000000011111111111111111111111111111111";
            var txtByte = StringToByte(txt);

            Console.WriteLine("Text: " + txt);
            Console.WriteLine("Key: " + key);

            var encoded = des.Encrypt(txtByte);
            var encString = encoded.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray();
            Console.Write("Encoded: ");
            var encodedBinary = "";
            foreach (var item in encString)
            {
                encodedBinary += item;
            }
            Console.WriteLine(encodedBinary);

            var decoded = des.Decrypt(encoded);

            var dstring = decoded.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray();
            Console.Write("Decoded: ");
            var decodedBinary = "";
            foreach (var item in dstring)
            {
                decodedBinary += item;
            }
            Console.WriteLine(decodedBinary);
            Console.ReadKey();
        }

        static byte[] StringToByte(string s)
        {
            string input = s;
            int numOfBytes = input.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }
            return bytes;
        }
    }

    public class SimpleDES
    {
        private readonly byte[] IV = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private byte[] mKey;
        private DESCryptoServiceProvider des;

        public SimpleDES(byte[] aKey)
        {
            if (aKey.Length != 8)
                throw new Exception("Key size must be 8 bytes");
            mKey = aKey;
            des = new DESCryptoServiceProvider();
            des.BlockSize = 64;
            des.KeySize = 64;
            des.Padding = PaddingMode.None;
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data.Length != 8)
                throw new Exception("Data size must be 8 bytes");

            ICryptoTransform encryptor = des.CreateWeakEncryptor(mKey, IV,0);
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        public byte[] Decrypt(byte[] data)
        {
            if (data.Length != 8)
                throw new Exception("Data size must be 8 bytes");
            ICryptoTransform decryptor = des.CreateWeakDecryptor(mKey, IV);
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }
    }

    public static class DESCryptoExtensions
    {
        public static ICryptoTransform CreateWeakEncryptor(this DESCryptoServiceProvider cryptoProvider, byte[] key, byte[] iv,int mode )
        {
            MethodInfo mi = cryptoProvider.GetType().GetMethod("_NewEncryptor", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] Par = { key, cryptoProvider.Mode, iv, cryptoProvider.FeedbackSize, mode };
            ICryptoTransform trans = mi.Invoke(cryptoProvider, Par) as ICryptoTransform;
            return trans;
        }

        public static ICryptoTransform CreateWeakEncryptor(this DESCryptoServiceProvider cryptoProvider)
        {
            return CreateWeakEncryptor(cryptoProvider, cryptoProvider.Key, cryptoProvider.IV,0);
        }

        public static ICryptoTransform CreateWeakDecryptor(this DESCryptoServiceProvider cryptoProvider, byte[] key, byte[] iv)
        {
            return CreateWeakEncryptor(cryptoProvider, key, iv,1);
        }

        public static ICryptoTransform CreateWeakDecryptor(this DESCryptoServiceProvider cryptoProvider)
        {
            return CreateWeakDecryptor(cryptoProvider, cryptoProvider.Key, cryptoProvider.IV);
        }
    }
}
