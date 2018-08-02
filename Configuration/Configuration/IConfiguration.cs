using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public interface IConfiguration
{
    string FilePath { get; }
    string FileName { get; }
    bool IsUpToDate { get; }

    string GetString(string section, string key);
    string[] GetStringArray(string section, string key);
    float GetFloat(string section, string key, float defaultValue);
    int GetInt(string section, string key, int defaultValue);
    bool GetBool(string section, string key, bool defaultValue);
    Vector3 GetVector3(string section, string key, Vector3 defaultValue);

    string[] GetAllSections();

    T Get<T>(string section, string key, T defaultValue);
    object Get(Type type, string section, string key, object defaultValue);

    void Set(string section, string key, object value);
    void Set(string section, string key, string value);

    void ReadFile();
    void SaveFile();
}