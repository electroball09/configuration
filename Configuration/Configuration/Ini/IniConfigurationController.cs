using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IniConfigurationController : IConfigurationController
{
    public string FileExtension { get { return IniConfiguration.FILE_EXTENSION; } }

    static List<IniConfiguration> configurations = new List<IniConfiguration>();

    public IConfiguration GetConfiguration(string filePath, string fileName)
    {
        if (configurations == null)
            configurations = new List<IniConfiguration>();
        
        foreach (IniConfiguration cfg in configurations)
        {
            if (cfg.FilePath == filePath && cfg.FileName == fileName)
                return cfg;
        }

        IniConfiguration newCfg = new IniConfiguration(filePath, fileName);
        configurations.Add(newCfg);
        return newCfg;
    }

    public void Destroy()
    {
        configurations.Clear();
        configurations = null;
    }
}