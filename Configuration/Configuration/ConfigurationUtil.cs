using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ConfigurationUtil
{
    public static void SetClassFieldsFromConfiguration<T>(this T obj, IConfiguration cfg, string sectionName, string[] fieldNames)
    {
        FieldInfo currInfo = null;
        for (int i = 0; i < fieldNames.Length; i++)
        {
            currInfo = obj.GetType().GetField(fieldNames[i]);
            if (currInfo != null)
            {
                object originalValue = currInfo.GetValue(obj);
                object newValue = cfg.Get(currInfo.FieldType, sectionName, currInfo.Name, originalValue);
                if (newValue != null)
                {
                    currInfo.SetValue(obj, newValue);
                }

                if (!originalValue.Equals(newValue))
                    cfg.Set(sectionName, currInfo.Name, newValue);
            }
        }

        cfg.SaveFile();
    }
}
