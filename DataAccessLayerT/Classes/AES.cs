using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NCiphers.Ciphers;

namespace DataAccessLayer.Classes
{

    public class AES
    {
        public static void Main(string[] args)
        {
            AES aes = new AES();
            // 16 bytes long key for AES-128 bit encryption
            byte[] key = { 50, 199, 10, 159, 132, 55, 236, 189, 51, 243, 244, 91, 17, 136, 39, 230 };
            // optional 16 byte long initialization vector
            byte[] IV = { 150, 9, 112, 39, 32, 5, 136, 289, 251, 43, 44, 191, 217, 236, 3, 106 };

            string message = "Hello World";

            // encrypt with a key only
            string encryptedMessage = aes.EncryptString(message, key);
            // encrypt with a key and IV
            string encryptedMessageWithIV = aes.EncryptString(message, key, IV);
            // encrypt with a password
            string encryptedMessageWithPassword = aes.EncryptString(message, "my password");
            // encrypt with a password and IV
            string encryptedMessageWithPasswordAndIV = aes.EncryptString(message, "my password", IV);

            message = aes.DecryptString(encryptedMessage, key);
            message = aes.DecryptString(encryptedMessageWithIV, key, IV);
            message = aes.DecryptString(encryptedMessageWithPassword, "my password");
            message = aes.DecryptString(encryptedMessageWithPasswordAndIV, "my password", IV);
        }
    }
}
