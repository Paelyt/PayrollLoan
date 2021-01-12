using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
namespace DataAccessLayerT.Classes
{
   
  public  class ManagedAES
    {
       
        //private byte[] Encrypt(byte[] input)
        //{
        //    PasswordDeriveBytes pdb = new PasswordDeriveBytes("hjiweykaksd", new byte[] { 0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84 });
        //    MemoryStream ms = new MemoryStream();
        //    Aes aes = new AesManaged();
        //    aes.Key = pdb.GetBytes(aes.KeySize / 8);
        //    aes.IV = pdb.GetBytes(aes.BlockSize / 8);
        //    CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        //    cs.Write(input, 0, input.Length);
        //    cs.Close();
        //    return ms.ToArray();
        //}
       //public  static dynamic EncryptAesManaged(string raw,byte[] encryptVal, bool encrypt)
       // {
       //     try
       //     {
                
       //         // Create Aes that generates a new key and initialization vector (IV).    
       //         // Same key must be used in encryption and decryption    
       //         using ( AesManaged aes = new AesManaged())
       //         {
       //             if (encrypt)
       //             {
       //                 // Encrypt string    
       //                 byte[] encrypted = Encrypt(raw, aes.Key, aes.IV);
       //             // Print encrypted string    
       //             // Console.WriteLine($"Encrypted data: {System.Text.Encoding.UTF8.GetString(encrypted)}");
       //             return encrypted;
       //             }else
       //             {                   
       //             // Decrypt the bytes to a string.    
       //             string decrypted = Decrypt(encryptVal, aes.Key, aes.IV);
       //             // Print decrypted string. It should be same as raw data    
       //             // Console.WriteLine($"Decrypted data: {decrypted}");
       //             return decrypted;
       //             }
       //         }
       //     }
       //     catch (Exception exp)
       //     {
       //         return "error:" + exp.Message;
       //     }
          
       // }
      public  static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }
      public  static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}
