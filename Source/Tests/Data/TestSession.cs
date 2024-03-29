﻿using System;
using System.IO;
using JetBrains.Annotations;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Data; 

public class TestSession : SessionNoServer {
  public const string DatabaseParentFolderPath = "C:\\Simon\\Databases";

  /// <summary>
  ///   TODO Replace hard coded VelocityDbLicenceFilePath value to support multiple developers.
  /// </summary>
  [PublicAPI]
  public const string VelocityDbLicenceFilePath =
    @"D:\Simon\OneDrive\Documents\My Installers\VelocityDB\License Database\4.odb";

  public TestSession() : base(CreateDatabaseFolder()) {
    BeginUpdate();
    new Schema().RegisterPersistableTypes(this);
    Commit();
  }

  public TestSession(string databaseFolderPath) : base(databaseFolderPath) { }

  public static void CopyLicenceToDatabaseFolder(string databaseFolderPath) {
    File.Copy(
      VelocityDbLicenceFilePath, 
      Path.Combine(databaseFolderPath, "4.odb"));
    // File.Copy(
    //   VelocityDbLicenceFilePath,
    //   databaseFolderPath + @"\4.odb");
  }

  public static string CreateDatabaseFolder() {
    string databaseFolderPath = GenerateDatabaseFolderPath();
    Directory.CreateDirectory(databaseFolderPath);
    CopyLicenceToDatabaseFolder(databaseFolderPath);
    return databaseFolderPath;
  }

  public static void DeleteFolderIfExists(string folderPath) {
    if (Directory.Exists(folderPath)) {
      Directory.Delete(folderPath, true);
    }
  }

  public static string GenerateDatabaseFolderPath() {
    return Path.Combine(DatabaseParentFolderPath, $"Database{DateTime.Now.Ticks}");
  }

  public void DeleteDatabaseFolderIfExists() {
    DeleteFolderIfExists(SystemDirectory);
  }
}