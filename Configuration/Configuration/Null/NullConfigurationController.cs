using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullConfigurationController : IConfigurationController
{
    public string FileExtension { get { return ""; } }

    NullConfiguration nullConfiguration = new NullConfiguration();

    public IConfiguration GetConfiguration(string filePath, string fileName)
    {
        return nullConfiguration;
    }

    public void Destroy()
    {
        nullConfiguration = null;
    }
}
