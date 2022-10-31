using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteNinja.DataPersistence.Encryptors;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EncryptorTest
{
  // A Test behaves as an ordinary method
  [Test]
  public void SimpleTextEncryptor()
  {
    const string text =
      "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
    const string key = "1234567890123456";

    var encryptor = new SimpleEncryptor(key);

    //encrypt the text
    var encryptedText = encryptor.Encrypt(text);
    //decrypt the textEncryptor
    var decryptedText = encryptor.Decrypt(encryptedText);

    //check if the decrypted text is equal to the original text
    Assert.AreEqual(text, decryptedText);
  }

  [Test]
  public void SimpleTextEncryptorWithJSON()
  {
    var serializableClass = new SerializableClass
    {
      name = "John Doe",
      age = 25,
      isMale = true,
      height = 1.75f,
      hobbies = new List<string>() { "gaming", "reading", "coding" }
    };

    var json = JsonUtility.ToJson(serializableClass);
    var encryptor = new SimpleEncryptor("liT3n1nj4!");
    var encryptedJson = encryptor.Encrypt(json);
    var decryptedJson = encryptor.Decrypt(encryptedJson);
    var deserializedClass = JsonUtility.FromJson<SerializableClass>(decryptedJson);

    Assert.IsTrue(deserializedClass == serializableClass);
  }


  [Serializable]
  public class SerializableClass
  {
    public string name;
    public int age;
    public float height;
    public bool isMale;
    public List<string> hobbies;

    //Equal operator
    public static bool operator ==(SerializableClass a, SerializableClass b)
    {
      if (a.hobbies.Count != b.hobbies.Count)
        return false;

      //check if the lists are equal
      if (a.hobbies.Where((t, i) => t != b.hobbies[i]).Any())
      {
        return false;
      }

      return a.name == b.name && a.age == b.age && a.height == b.height && a.isMale == b.isMale;
    }

    public static bool operator !=(SerializableClass a, SerializableClass b)
    {
      return !(a == b);
    }
  }
}