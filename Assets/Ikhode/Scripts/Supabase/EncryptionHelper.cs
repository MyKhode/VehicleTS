using System.Security.Cryptography;
using System.Text;
using System.IO;  // For StreamWriter and StreamReader
using System;     // For Convert
using System.Collections.Generic; // For Dictionary<>


public static class EncryptionHelper
{
    private static readonly string encryptionKey = "YourSuperSecretKey123!"; // Replace with a secure key

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;
        var key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 16));
        using (var aesAlg = Aes.Create())
        {
            using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
            {
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, 16);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return string.Empty;
        var fullCipher = Convert.FromBase64String(cipherText);
        var key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 16));
        using (var aesAlg = Aes.Create())
        {
            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            using (var decryptor = aesAlg.CreateDecryptor(key, iv))
            {
                using (var msDecrypt = new System.IO.MemoryStream(fullCipher, 16, fullCipher.Length - 16))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
