namespace LiteNinja.DataPersistence
{
  public interface ISaveManager
  {
    bool Save(string resourceId, object data);
    T Load<T>(string resourceId);
    bool Exists(string resourceId);
    void Delete(string resourceId);
    void DeleteAll();
    string[] GetResourceIds();
  }
}