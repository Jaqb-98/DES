using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace DEScipher
{
    class Program
    {
        static void Main()
        {

            DES DESalg = DES.Create();

            Console.Write("Text: ");
            string sData = Console.ReadLine();
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };


            string keyString;
            byte[] key;
            do
            {
                Console.Write("Key (8 chars): ");
                keyString = Console.ReadLine();
                key = Encoding.Default.GetBytes(keyString);
            } while (keyString.Length != 8);

            byte[] Data = Encrypt(sData, key, IV);

            var encoded = Encoding.Default.GetString(Data);
            Console.WriteLine("Encrypted: " + encoded);


            string Final = Decrypt(Data, key, IV);
            Console.WriteLine("Decrypted: " + Final);

            Console.ReadKey();

        }

        public static byte[] Encrypt(string Data, byte[] Key, byte[] IV)
        {

            MemoryStream mStream = new MemoryStream();
            DES DESalg = DES.Create();

            CryptoStream cStream = new CryptoStream(mStream, DESalg.CreateEncryptor(Key, IV), CryptoStreamMode.Write);

            byte[] toEncrypt = new ASCIIEncoding().GetBytes(Data);


            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();

            byte[] ret = mStream.ToArray();


            cStream.Close();
            mStream.Close();


            return ret;

        }

        public static string Decrypt(byte[] Data, byte[] Key, byte[] IV)
        {

            MemoryStream msDecrypt = new MemoryStream(Data);

            DES DESalg = DES.Create();

            CryptoStream csDecrypt = new CryptoStream(msDecrypt, DESalg.CreateDecryptor(Key, IV), CryptoStreamMode.Read);

            byte[] fromEncrypt = new byte[Data.Length];


            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            return new ASCIIEncoding().GetString(fromEncrypt);

        }
    }
}


