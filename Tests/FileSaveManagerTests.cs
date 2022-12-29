using System;
using System.Collections;
using System.Collections.Generic;
using LiteNinja.DataPersistence;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FileSaveManagerTests
{
  [Test]
  public void TestSaveAndLoadInt()
  {
    // Arrange
    var saveManager = new FileSaveManager();
    const string resourceId = "int_value";
    const int expectedValue = 123;

    // Act
    saveManager.Save(resourceId, expectedValue);
    var actualValue = saveManager.Load<int>(resourceId);

    // Assert
    Assert.AreEqual(expectedValue, actualValue);
  }

  [Test]
  public void TestSaveAndLoadFloat()
  {
    // Arrange
    var saveManager = new FileSaveManager();
    const string resourceId = "float_value";
    const float expectedValue = 1.23f;

    // Act
    saveManager.Save(resourceId, expectedValue);
    var actualValue = saveManager.Load<float>(resourceId);

    // Assert
    Assert.AreEqual(expectedValue, actualValue);
  }

  [Test]
  public void TestSaveAndLoadBool()
  {
    // Arrange
    var saveManager = new FileSaveManager();
    const string resourceId = "bool_value";
    const bool expectedValue = true;

    // Act
    saveManager.Save(resourceId, expectedValue);
    var actualValue = saveManager.Load<bool>(resourceId);

    // Assert
    Assert.AreEqual(expectedValue, actualValue);
  }

  [Test]
  public void TestSaveAndLoadString()
  {
    // Arrange
    var saveManager = new FileSaveManager();
    const string resourceId = "string_value";
    const string expectedValue = "Hello World!";

    // Act
    saveManager.Save(resourceId, expectedValue);
    var actualValue = saveManager.Load<string>(resourceId);

    // Assert
    Assert.AreEqual(expectedValue, actualValue);
  }

  [Test]
  public void TestSaveAndLoadClass()
  {
    // Arrange
    var saveManager = new FileSaveManager();
    const string resourceId = "class_value";
    var expectedValue = new TestClass
    {
      Id = 123,
      Name = "Test",
      Description = "This is a test class",
      
    };
    expectedValue.PrivateField = 1969;
    
    // Act
    saveManager.Save(resourceId, expectedValue);
    var actualValue = saveManager.Load<TestClass>(resourceId);

    // Assert
    Assert.AreEqual(expectedValue.Id, actualValue.Id);
    Assert.AreEqual(expectedValue.Name, actualValue.Name);
    Assert.AreEqual(expectedValue.Description, actualValue.Description);
    Assert.AreEqual(expectedValue.PrivateField, actualValue.PrivateField);
  }

  [Serializable]
  private class TestClass
  {
    public int Id;
    public string Name;
    public string Description;
    
    [SerializeField]
    private int _privateField;
    
    public int PrivateField
    {
      get => _privateField;
      set => _privateField = value;
    }
  }

}
