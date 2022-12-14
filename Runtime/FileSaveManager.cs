using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteNinja.DataPersistence.Encryptors;
using UnityEngine;

namespace LiteNinja.DataPersistence
{
  public class FileSaveManager : ISaveManager
  {
    private readonly string _baseLocation;
    private readonly IEncryptor _encryptor;

    private readonly Dictionary<Type, IFileData> _data;
    private readonly Dictionary<Type, string> _fileNames;

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
      _data = new Dictionary<Type, IFileData>();
      _fileNames = new Dictionary<Type, string>();
    }

    public bool Save<T>(string resourceId, T data)
    {
      if (IsPrimitive<T>())
      {
        return PrimitiveSave<T>(resourceId, data);
      }

      var text = JsonUtility.ToJson(data);
      File.WriteAllText(GetFileName(resourceId), _encryptor.Encrypt(text));
      return true;
    }



    public T Load<T>(string resourceId)
    {
      if (IsPrimitive<T>())
      {
        return PrimitiveLoad<T>(resourceId);
      }

      var fileName = GetFileName(resourceId);
      if (!File.Exists(fileName))
      {
        return default;
      }

      var text = File.ReadAllText(fileName);
      return JsonUtility.FromJson<T>(_encryptor.Decrypt(text));
    }
    
    public bool Exists<T>(string resourceId)
    {
      return IsPrimitive<T>() ? PrimitiveExists<T>(resourceId) : File.Exists(GetFileName(resourceId));
    }

    public void Delete<T>(string resourceId)
    {
      if (IsPrimitive<T>())
      {
        PrimitiveDelete<T>(resourceId);
      }
      else
      {
        File.Delete(GetFileName(resourceId));
      }
    }

    private void PrimitiveDelete<T>(string resourceId)
    {
      Preload<T>();
      var fileData = (FileData<T>)_data[typeof(T)];
      fileData.Remove(resourceId);
      var text = JsonUtility.ToJson(fileData);
      File.WriteAllText(GetFileName<T>(), _encryptor.Encrypt(text));
    }

    public void DeleteAll()
    {
      var files = Directory.GetFiles(_baseLocation);
      foreach (var file in files)
      {
        File.Delete(file);
      }

      _data.Clear();
    }

    private void Preload<T>()
    {
      if (_data.ContainsKey(typeof(T))) return;
      _data.Add(typeof(T), new FileData<T>());
      var path = GetFileName<T>();
      if (!File.Exists(path)) return;
      var text = _encryptor.Decrypt(File.ReadAllText(path));
      var fileData = JsonUtility.FromJson<FileData<T>>(text);
      _data[typeof(T)] = fileData;
    }

    
    private bool IsPrimitive<T>()
    {
      return typeof(T).IsPrimitive || typeof(T) == typeof(string);
    }
    

    private string GetFileName<T>()
    {
      if (_fileNames.ContainsKey(typeof(T))) return _fileNames[typeof(T)];

      var fileName = typeof(T).Name;
      //replace forbidden characters in file name
      fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) =>
        current.Replace(c, '_'));

      var path = _baseLocation + Path.DirectorySeparatorChar + fileName;
      _fileNames.Add(typeof(T), path);
      return path;
    }

    private string GetFileName(string resourceId)
    {
      var fileName = resourceId;
      //replace forbidden characters in file name
      fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) =>
        current.Replace(c, '_'));

      var path = _baseLocation + Path.DirectorySeparatorChar + fileName;
      return path;
    }
    
    private bool PrimitiveSave<T>(string resourceId, T data)
    {
      Preload<T>();
      var fileData = (FileData<T>)_data[typeof(T)];
      fileData.Add(resourceId, data);
      var text = JsonUtility.ToJson(fileData);
      File.WriteAllText(GetFileName<T>(), _encryptor.Encrypt(text));
      return true;
    }
    
    private T PrimitiveLoad<T>(string resourceId)
    {
      Preload<T>();
      var fileData = (FileData<T>)_data[typeof(T)];
      var result = fileData.Get(resourceId, out var value);
      return result ? value : default;
    }
    
    private bool PrimitiveExists<T>(string resourceId)
    {
      Preload<T>();
      var fileData = (FileData<T>)_data[typeof(T)];
      return fileData.Get(resourceId, out _);
    }

    #region Wrapper for Primitive Types
    private interface IFileData
    {
    }

    [Serializable]
    private class FileData<T> : IFileData
    {
      [SerializeField] private List<string> _keys;
      [SerializeField] private List<T> _values;

      public FileData()
      {
        _keys = new List<string>();
        _values = new List<T>();
      }

      public void Add(string key, T value)
      {
        //if key already exists, replace value
        if (Get(key, out var _))
        {
          var index = _keys.IndexOf(key);
          _values[index] = value;
          return;
        }

        _keys.Add(key);
        _values.Add(value);
      }

      public bool Get(string key, out T value)
      {
        var index = _keys.IndexOf(key);
        if (index == -1)
        {
          value = default;
          return false;
        }

        value = _values[index];
        return true;
      }

      public bool Remove(string key)
      {
        var index = _keys.IndexOf(key);
        if (index == -1)
        {
          return false;
        }

        _keys.RemoveAt(index);
        _values.RemoveAt(index);
        return true;
      }

      public void Clear()
      {
        _keys.Clear();
        _values.Clear();
      }

      public int Count => _keys.Count;
      public List<string> Keys => _keys;
    }
    #endregion
  }
}