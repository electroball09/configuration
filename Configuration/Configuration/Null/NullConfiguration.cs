using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullConfiguration : IConfiguration
{
    static string[] emptyStringArray = new string[0];

    public string FilePath { get { return null; } }
    public string FileName { get { return null; } }
    public bool IsUpToDate { get { return true; } }

    public string GetString(string section, string key, int index = 0)
    {
        return "";
    }

    public string[] GetStringArray(string section, string key)
    {
        return emptyStringArray;
    }

    public float GetFloat(string section, string key, float defaultValue, int index = 0)
    {
        return defaultValue;
    }

    public int GetInt(string section, string key, int defaultValue, int index = 0)
    {
        return defaultValue;
    }

    public bool GetBool(string section, string key, bool defaultValue, int index = 0)
    {
        return defaultValue;
    }

    public Vector3 GetVector3(string section, string key, Vector3 defaultValue, int index = 0)
    {
        return defaultValue;
    }

    public string[] GetAllSections()
    {
        return emptyStringArray;
    }

    public T Get<T>(string section, string key, T defaultValue, int index = 0)
    {
        return defaultValue;
    }

    public object Get(Type type, string section, string key, object defaultValue, int index = 0)
    {
        return defaultValue;
    }

    public void Set(string section, string key, object value, int index = 0)
    {

    }

    public void Set(string section, string key, string value, int index = 0)
    {

    }

    public void ReadFile()
    {

    }

    public void SaveFile()
    {

    }
}
