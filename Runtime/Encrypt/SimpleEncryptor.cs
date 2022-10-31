using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace LiteNinja.DataPersistence.Encryptors
{
  public class SimpleEncryptor : IEncryptor
  {
    private readonly string _encryptionKey;

    public SimpleEncryptor(string encryptionKey)
    {
      _encryptionKey = encryptionKey;
    }

    public string Encrypt(string text)
    {
      var clearBytes = Encoding.Unicode.GetBytes(text);
      var output = "";
      using var encryptor = Aes.Create();
      var pdb = new Rfc2898DeriveBytes(_encryptionKey,
        new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
      encryptor.Key = pdb.GetBytes(32);
      encryptor.IV = pdb.GetBytes(16);
      using var memoryStream = new MemoryStream();
      using (var cryptoStream = new CryptoStream(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
      {
        cryptoStream.Write(clearBytes, 0, clearBytes.Length);
        cryptoStream.Close();
      }

      output = Convert.ToBase64String(memoryStream.ToArray());
      return output;
    }

    public string Decrypt(string text)
    {
      text = text.Replace(" ", "+");
      var output = "";
      var cipherBytes = Convert.FromBase64String(text);
      using var encryptor = Aes.Create();
      var pdb = new Rfc2898DeriveBytes(_encryptionKey,
        new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
      encryptor.Key = pdb.GetBytes(32);
      encryptor.IV = pdb.GetBytes(16);
      using var memoryStream = new MemoryStream();
      using (var cryptoStream = new CryptoStream(memoryStream, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
      {
        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
        cryptoStream.Close();
      }

      output = Encoding.Unicode.GetString(memoryStream.ToArray());

      return output;
    }
  }
}