//using UnityEngine;
//using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;


public class SJ_Ase 
{

	public static  string AESEncrypt256(string _text , string Password )
    {
        //string Password = "jasmintime"; // 인증키
        RijndaelManaged RijndaelCipher = new RijndaelManaged();
        byte[] PlainText = Encoding.Unicode.GetBytes(_text);
        byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
        PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
        ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
        MemoryStream memoryStream = new MemoryStream();
 
        CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(PlainText, 0, PlainText.Length);
        cryptoStream.FlushFinalBlock();
 
        byte[] CipherBytes = memoryStream.ToArray();
        memoryStream.Close();
        cryptoStream.Close();
 
        return Convert.ToBase64String(CipherBytes);
    }
 
    public static  string AESDecrypte256(string _text , string Password)
    {
        //string Password = "jasmintime"; // 인증키
        RijndaelManaged RijndaelCipher = new RijndaelManaged();
 
        byte[] EncryptedData = Convert.FromBase64String(_text);
        byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
        PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
        ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
        MemoryStream memoryStream = new MemoryStream(EncryptedData);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
 
        byte[] PlainText = new byte[EncryptedData.Length];
        int DecrypedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
        memoryStream.Close();
        cryptoStream.Close();
 
        return Encoding.Unicode.GetString(PlainText, 0, DecrypedCount);
    }


	//출처: http://jasmintime.com/136 [Jasmin Time]


	public static string Decrypt_128_WithJava(string textToDecrypt, string key)
	{
		RijndaelManaged rijndaelCipher = new RijndaelManaged();
		rijndaelCipher.Mode = CipherMode.CBC;
		rijndaelCipher.Padding = PaddingMode.PKCS7;
 
		rijndaelCipher.KeySize = 128;
		rijndaelCipher.BlockSize = 128;
		byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
		byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
		byte[] keyBytes = new byte[16];
		int len = pwdBytes.Length;
		if (len > keyBytes.Length)
		{
			len = keyBytes.Length;
		}
		Array.Copy(pwdBytes, keyBytes, len);
		rijndaelCipher.Key = keyBytes;
		rijndaelCipher.IV = keyBytes;
		byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
		return Encoding.UTF8.GetString(plainText);
	}
 
	public static string Encrypt_128_WithJava(string textToEncrypt, string key )
	{
		RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;
 
        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;
        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;
        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }
        Array.Copy(pwdBytes, keyBytes, len);
        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;
        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
	}


		static	public RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }
 
        static	public byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }
 
		static	public byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
 
        /// <summary>
        /// Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
        /// </summary>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="key">Secret key</param>
        /// <returns>Base64 encoded string</returns>
        static	public String Encrypt(String plainText, String key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)));
        }
 
        /// <summary>
        /// Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
        /// </summary>
        /// <param name="encryptedText">Base64 Encoded String</param>
        /// <param name="key">Secret Key</param>
        /// <returns>Decrypted String</returns>
        static	public String Decrypt(String encryptedText, String key)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)));
        }


}
