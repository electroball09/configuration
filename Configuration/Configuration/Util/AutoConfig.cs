using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class AutoConfigAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AutoConfigClassAttribute : Attribute
{
    public ConfigLocation Location
    {
        get;
        private set;
    }

    public string Subdirectory
    {
        get;
        private set;
    }

    public AutoConfigClassAttribute(ConfigLocation location = ConfigLocation.Local, string subdir = "")
    {
        Location = location;
        if (!subdir.EndsWith("\\") && !subdir.EndsWith("/"))
        {
            subdir += "\\";
        }
        Subdirectory = subdir;
    }
}

public static class AutoConfigUtil
{
    public const string AUTO_CONFIG_NAME = "AutoConfig";

    struct ConfigClassStruct
    {
        public Type Type;
        public AutoConfigClassAttribute Attribute;
    }

    static IEnumerable<ConfigClassStruct> configClasses;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadAutoConfig()
    {
        Profiler.BeginSample("AUTO_CONFIG_LOAD");

        Debug.Log("AUTO CONFIG LOAD!");

        if (configClasses == null)
        {
            List<ConfigClassStruct> classes = new List<ConfigClassStruct>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in a.GetTypes())
                    {
                        object attribute = t.GetCustomAttribute(typeof(AutoConfigClassAttribute), true);
                        if (attribute != null)
                            classes.Add(new ConfigClassStruct() { Type = t, Attribute = attribute as AutoConfigClassAttribute });
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogWarning(ex);
                    Debug.LogWarning(a.FullName);
                }
            }

            configClasses = classes;
        }

        foreach (ConfigClassStruct str in configClasses)
        {
            LoadConfigForClass(str);
        }

        Profiler.EndSample();
    }

    static void LoadConfigForClass(ConfigClassStruct str)
    {
        Debug.LogFormat("Loading config for class {0}", str.Type.AssemblyQualifiedName);

        FieldInfo[] fields = str.Type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        var fieldsWithAttribute =
            from f in fields
            where f.FieldType.IsClass
            let attributes = f.GetCustomAttributes(typeof(AutoConfigAttribute), true)
            where attributes != null && attributes.Length > 0
            select new { Field = f, Attribute = attributes.Cast<AutoConfigAttribute>().ElementAt(0) };

        IConfiguration config;
        if (str.Attribute.Location == ConfigLocation.Local)
            config = Config.GetLocalConfiguration(AUTO_CONFIG_NAME);
        else
            config = Config.GetPersistentConfiguration(AUTO_CONFIG_NAME);

        config.ReadFile();

        string configSectionName = str.Type.Name;
        string configKeyPrefix;

        foreach (var f in fieldsWithAttribute)
        {
            configKeyPrefix = f.Field.Name + ".";
            FieldInfo[] classFields = f.Field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            object inst = Activator.CreateInstance(f.Field.FieldType);
            foreach (FieldInfo fInfo in classFields)
            {
                object val = config.Get(fInfo.FieldType, configSectionName, configKeyPrefix + fInfo.Name, fInfo.GetValue(inst));
                fInfo.SetValue(inst, val);
                config.Set(configSectionName, configKeyPrefix + fInfo.Name, val);
            }
            f.Field.SetValue(null, inst);
        }

        config.SaveFile();
    }
}