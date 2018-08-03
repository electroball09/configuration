using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config
{
    public const string STARTUP_CONFIG_CONTROLLER_TYPE_KEY = "ConfigurationController";
    public static readonly Type DEFAULT_CONTROLLER_TYPE = typeof(NullConfigurationController);
    public static readonly string LOCAL_CONFIG_PATH;
    public const string DEFAULT_CONFIG_NAME = "Game";
    public static readonly string PERSISTENT_CONFIG_FOLDER_NAME = Application.companyName + "/";
    public static readonly string PERSISTENT_CONFIG_PATH;

    private static IConfiguration localConfig;
    public static IConfiguration Local { get { return localConfig; } }
    private static IConfiguration persistentConfig;
    public static IConfiguration Persistent { get { return persistentConfig; } }

    private static IConfigurationController cfgController;

    private static List<IConfigurationController> initializedControllers = new List<IConfigurationController>();

    static Config()
    {
        LOCAL_CONFIG_PATH = Environment.CurrentDirectory + "\\Config\\";
        PERSISTENT_CONFIG_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\" + PERSISTENT_CONFIG_FOLDER_NAME + @"\";

        if (Application.isPlaying)
            Initialize();
    }
    
    static void Initialize()
    {
        string requestedControllerType = StartupConfig.GetValue(STARTUP_CONFIG_CONTROLLER_TYPE_KEY);
        if (!string.IsNullOrEmpty(requestedControllerType))
        {
            Type cfgControllerType = Type.GetType(requestedControllerType);
            cfgController = InitializeSpecifiedControllerType(cfgControllerType);
        }
        else
        {
            Debug.Log("No requested controller type specified, initializing with default (" + DEFAULT_CONTROLLER_TYPE.Name + ")");
            cfgController = InitializeSpecifiedControllerType(DEFAULT_CONTROLLER_TYPE);
        }

        localConfig = GetController<IniConfigurationController>().GetConfiguration(LOCAL_CONFIG_PATH, DEFAULT_CONFIG_NAME);
        localConfig.ReadFile();
        persistentConfig = GetController<IniConfigurationController>().GetConfiguration(PERSISTENT_CONFIG_PATH, DEFAULT_CONFIG_NAME);
        persistentConfig.ReadFile();
    }

    public static IConfigurationController InitializeSpecifiedControllerType(Type controllerType)
    {
        Type actualType = DEFAULT_CONTROLLER_TYPE;

        //make sure requested controller type is not null and that it implements the IConfigurationController interface
        if (controllerType != null && controllerType.GetInterface("IConfigurationController") != null)
        {
            actualType = controllerType;
        }
        else
        {
            return cfgController;
        }
        
        return (IConfigurationController)Activator.CreateInstance(actualType);
    }

    public static IConfigurationController GetController<T>() where T : IConfigurationController
    {
        if (cfgController is T)
            return cfgController;

        for (int i = 0; i < initializedControllers.Count; i++)
            if (initializedControllers[i] is T)
                return initializedControllers[i];

        IConfigurationController newController = InitializeSpecifiedControllerType(typeof(T));

        initializedControllers.Add(newController);

        return newController;
    }

    public static void UninitializeController<T>() where T : IConfigurationController
    {
        IConfigurationController controller = GetController<T>();
        controller.Destroy();
        initializedControllers.Remove(controller);
    }
    
    public static IConfiguration GetConfiguration(string filePath, string fileName)
    {
        return cfgController.GetConfiguration(filePath, fileName);
    }

    public static IConfiguration GetConfiguration<T>(string filePath, string fileName) where T : IConfigurationController
    {
        return GetController<T>().GetConfiguration(filePath, fileName);
    }

    public static IConfiguration GetLocalConfiguration(string fileName, string localPath = "Client/")
    {
        return cfgController.GetConfiguration(LOCAL_CONFIG_PATH + localPath, fileName);
    }

    public static IConfiguration GetLocalConfiguration<T>(string fileName, string localPath = "Client/") where T : IConfigurationController
    {
        return GetController<T>().GetConfiguration(LOCAL_CONFIG_PATH + localPath, fileName);
    }

    public static IConfiguration GetPersistentConfiguration(string fileName, string localPath = "")
    {
        return cfgController.GetConfiguration(PERSISTENT_CONFIG_PATH + localPath, fileName);
    }

    public static IConfiguration GetPersistentConfiguration<T>(string fileName, string localPath = "") where T : IConfigurationController
    {
        return GetController<T>().GetConfiguration(PERSISTENT_CONFIG_PATH + localPath, fileName);
    }
}