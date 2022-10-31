namespace LiteNinja.DataPersistence.Encryptors
{
  public interface IEncryptor
  {
    string Encrypt(string text);
    string Decrypt(string text);
  }
}