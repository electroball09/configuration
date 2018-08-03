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

    string GetString(string section, string key, int index = 0);
    string[] GetStringArray(string section, string key);
    float GetFloat(string section, string key, float defaultValue, int index = 0);
    int GetInt(string section, string key, int defaultValue, int index = 0);
    bool GetBool(string section, string key, bool defaultValue, int index = 0);
    Vector3 GetVector3(string section, string key, Vector3 defaultValue, int index = 0);

    string[] GetAllSections();

    T Get<T>(string section, string key, T defaultValue, int index = 0);
    object Get(Type type, string section, string key, object defaultValue, int index = 0);

    void Set(string section, string key, object value, int index = 0);
    void Set(string section, string key, string value, int index = 0);

    void ReadFile();
    void SaveFile();
}