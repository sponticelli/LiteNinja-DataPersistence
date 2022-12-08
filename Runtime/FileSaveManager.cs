using System;
using System.IO;
using LiteNinja.DataPersistence.Encryptors;
using UnityEngine;

namespace LiteNinja.DataPersistence
{
  public class FileSaveManager : ISaveManager
  {
    private readonly string _baseLocation;
    private readonly IEncryptor _encryptor;

    public FileSaveManager(string baseLocation = null, IEncryptor encryptor = null)
    {
      _baseLocation = string.IsNullOrEmpty(baseLocation)
        ? Application.persistentDataPath
        : Application.persistentDataPath + Path.DirectorySeparatorChar + baseLocation;
      if (!Directory.Exists(_baseLocation))
      {
        Directory.CreateDirectory(_baseLocation);
      }

      _encryptor = encryptor ?? new NoEncryptor();
    }

    public bool Save(string resourceId, object data)
    {
      var text = JsonUtility.ToJson(data);
      var path = _baseLocation + Path.DirectorySeparatorChar + resourceId;
      File.WriteAllText(path, _encryptor.Encrypt(text));
      return true;
    }

    public T Load<T>(string resourceId)
    {
      var path = _baseLocation + Path.DirectorySeparatorChar + resourceId;
      if (!File.Exists(path)) return default;

      var text = _encryptor.Decrypt(File.ReadAllText(path));
      return JsonUtility.FromJson<T>(text);
    }

    public bool Exists(string resourceId)
    {
      var path = _baseLocation + Path.DirectorySeparatorChar + resourceId;
      return File.Exists(path);
    }

    public void Delete(string resourceId)
    {
      var path = _baseLocation + Path.DirectorySeparatorChar + resourceId;
      if (File.Exists(path))
      {
        File.Delete(path);
      }
    }

    public void DeleteAll()
    {
      var files = Directory.GetFiles(_baseLocation);
      foreach (var file in files)
      {
        File.Delete(file);
      }
    }

    public string[] GetResourceIds()
    {
      return Directory.GetFiles(_baseLocation);
    }
  }
}