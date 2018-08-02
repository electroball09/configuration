using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class IniConfiguration : IConfiguration
{
    public const string FILE_EXTENSION = ".ini";
    const char SECTION_START = '[';
    const char SECTION_END = ']';
    const char KEYVALUE_SEPARATOR = '=';
    const char ARRAY_SEPARATOR = ',';
    const string ARRAY_SEPARATOR_STR = ",";

    private class IniSection
    {
        private string m_name;
        public string Name { get { return m_name; } }

        private Dictionary<string, string> m_keys = new Dictionary<string, string>();

        public IniSection(string _name)
        {
            m_name = _name;
        }

        public string Get(string key)
        {
            if (m_keys.ContainsKey(key))
                return m_keys[key];

            return null;
        }

        public void Set(string key, string value)
        {
            if (m_keys.ContainsKey(key))
            {
                m_keys[key] = value;
                return;
            }

            m_keys.Add(key, value);
        }

        public List<string> GetKeyStrings()
        {
            List<string> ret = new List<string>();
            foreach (KeyValuePair<string, string> kvp in m_keys)
            {
                if (kvp.Value.Contains(ARRAY_SEPARATOR_STR))
                {
                    string[] split = kvp.Value.Split(ARRAY_SEPARATOR);
                    foreach (string str in split)
                        ret.Add(kvp.Key + KEYVALUE_SEPARATOR + str);
                }
                else
                    ret.Add(kvp.Key + KEYVALUE_SEPARATOR + kvp.Value);
            }

            return ret;
        }

        public bool Contains(string key)
        {
            return m_keys.ContainsKey(key);
        }
    }

    private string m_filePath = "";
    public string FilePath { get { return m_filePath; } }

    private string m_fileName = "";
    public string FileName { get { return m_fileName; } }

    private bool m_isUpToDate = false;
    public bool IsUpToDate { get { return m_isUpToDate; } }

    private List<IniSection> m_sections = new List<IniSection>();

    public IniConfiguration(string filePath, string fileName)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            m_filePath = Environment.CurrentDirectory + "\\";
        }
        else
        {
            m_filePath = new DirectoryInfo(filePath).FullName;
        }
        m_fileName = fileName;
    }

    private IniSection getSection(string sectionName)
    {
        foreach (IniSection section in m_sections)
        {
            if (section.Name == sectionName)
                return section;
        }

        IniSection newSection = new IniSection(sectionName);
        m_sections.Add(newSection);
        return newSection;
    }

    private bool verifyFileExists()
    {
        if (!Directory.Exists(m_filePath))
        {
            Directory.CreateDirectory(m_filePath);
        }

        if (!File.Exists(m_filePath + m_fileName + FILE_EXTENSION))
        {
            File.Create(m_filePath + m_fileName + FILE_EXTENSION);
            return false;
        }

        return true;
    }

    public void ReadFile()
    {
        m_sections.Clear();

        if (!verifyFileExists())
            return;

        using (StreamReader sr = new StreamReader(m_filePath + m_fileName + FILE_EXTENSION))
        {
            string line = "";
            IniSection currentSection = null;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                //determine if we're on a section header or not
                if (line[0] == SECTION_START)
                {
                    if (line[line.Length - 1] == SECTION_END) //if section end char is at end of section name, remove it and section start
                        line = line.Substring(1, line.Length - 2);

                    currentSection = getSection(line);
                    continue;
                }

                if (currentSection == null) //discard any data before first section
                    continue;
                
                //parse value
                string[] values = line.Split(KEYVALUE_SEPARATOR);
                if (values.Length == 2)
                {
                    //if we already have a key of the same name in the current section, add the value as a string array
                    //  i.e. if we already have a key called "MyKey" with value "MyValue1", and we have read a value of "MyValue2" with the same key later, the string value of "MyKey" becomes "MyValue1,MyValue2"
                    if (currentSection.Contains(values[0]))
                    {
                        string value = currentSection.Get(values[0]);
                        value += ARRAY_SEPARATOR + values[1];
                        currentSection.Set(values[0], value);
                    }
                    else
                        currentSection.Set(values[0], values[1]);
                }
            }

            sr.Close();
        }

        m_isUpToDate = true;
    }

    public void SaveFile()
    {
        verifyFileExists();
        File.WriteAllLines(m_filePath + m_fileName + FILE_EXTENSION, GetFileStrings());

        m_isUpToDate = true;
    }

    public string[] GetFileStrings()
    {
        List<string> lines = new List<string>();
        foreach (IniSection section in m_sections)
        {
            lines.Add(SECTION_START + section.Name + SECTION_END);
            lines.AddRange(section.GetKeyStrings());
            lines.Add("");
        }
        return lines.ToArray();
    }

    public string[] GetAllSections()
    {
        string[] sections = new string[m_sections.Count];
        for (int i = 0; i < sections.Length; i++)
        {
            sections[i] = m_sections[i].Name;
        }
        return sections;
    }

    public void Set(string section, string key, object value)
    {
        Set(section, key, ConvertUtil.ConvertToString(value.GetType(), value));
    }

    public void Set(string section, string key, string value)
    {
        getSection(section).Set(key, value);
        m_isUpToDate = false;
    }

    public T Get<T>(string section, string key, T defaultValue)
    {
        T newValue = default(T);
        try
        {
            newValue = ConvertUtil.ConvertFromStr<T>(getSection(section).Get(key));
        }
        catch
        {
            newValue = defaultValue;
        }
        return newValue;
    }

    public object Get(Type type, string section, string key, object defaultValue)
    {
        object newValue = null;
        try
        {
            newValue = ConvertUtil.ConvertFromStr(type, getSection(section).Get(key));
        }
        catch
        {
            newValue = defaultValue;
        }
        return newValue;
    }

    public string GetString(string section, string key)
    {
        return getSection(section).Get(key);
    }

    public string[] GetStringArray(string section, string key)
    {
        string data = getSection(section).Get(key);
        return data.Split(ARRAY_SEPARATOR);
    }

    public float GetFloat(string section, string key, float defaultValue)
    {
        string data = getSection(section).Get(key);
        float ret = defaultValue;
        if (float.TryParse(data, out ret))
            return ret;
        return defaultValue;
    }

    public int GetInt(string section, string key, int defaultValue)
    {
        string data = getSection(section).Get(key);
        if (String.IsNullOrEmpty(data))
            return defaultValue;
        int ret = defaultValue;
        if (int.TryParse(data, out ret))
            return ret;
        return defaultValue;
    }

    public bool GetBool(string section, string key, bool defaultValue)
    {
        string data = getSection(section).Get(key);
        if (String.IsNullOrEmpty(data))
            return defaultValue;
        bool ret = defaultValue;
        if (bool.TryParse(data, out ret))
            return ret;
        return defaultValue;
    }

    public Vector3 GetVector3(string section, string key, Vector3 defaultValue)
    {
        string data = getSection(section).Get(key);
        if (String.IsNullOrEmpty(data))
            return defaultValue;
        return (Vector3)ConvertUtil.ConvertVector3FromStr(data);
    }
}