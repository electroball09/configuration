using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class StartupConfig
{
    const string CONFIG_FILE_NAME = "startup.cfg";
    const char SEPARATOR = '=';

    static Dictionary<string, string> startupValues = new Dictionary<string, string>();

    static StartupConfig()
    {
        ReadFile();
    }

    public static void ReadFile()
    {
        startupValues = new Dictionary<string, string>();

        if (!File.Exists(CONFIG_FILE_NAME))
            return;

        string[] lines = File.ReadAllLines(CONFIG_FILE_NAME);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(SEPARATOR);
            if (values.Length == 2 && !startupValues.ContainsKey(values[0]))
            {
                startupValues.Add(values[0], values[1]);
            }
        }
    }

    public static string GetValue(string key)
    {
        if (startupValues.ContainsKey(key))
            return startupValues[key];

        return string.Empty;
    }
}