namespace LiteNinja.DataPersistence.Encryptors
{
  public class  NoEncryptor : IEncryptor
  {
    public string Encrypt(string text)
    {
      return text;
    }
  
    public string Decrypt(string text)
    {
      return text;
    }
  }
}