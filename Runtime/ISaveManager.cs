namespace LiteNinja.DataPersistence
{
  public interface ISaveManager
  {
    bool Save<T>(string resourceId, T data);
    T Load<T>(string resourceId);
    bool Exists<T>(string resourceId);
    void Delete<T>(string resourceId);
    void DeleteAll();
  }
}