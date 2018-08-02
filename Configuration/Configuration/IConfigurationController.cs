using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfigurationController
{
    string FileExtension { get; }
    IConfiguration GetConfiguration(string filePath, string fileName);
    void Destroy();
}