using System;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;

public class ConvertUtil
{
    private static Dictionary<Type, Func<string, object>> convertersFromString = new Dictionary<Type, Func<string, object>>
    {
        { typeof(float), TypeDescriptor.GetConverter(typeof(float)).ConvertFromString },
        { typeof(bool), TypeDescriptor.GetConverter(typeof(bool)).ConvertFromString },
        { typeof(double), TypeDescriptor.GetConverter(typeof(double)).ConvertFromString },
        { typeof(long), TypeDescriptor.GetConverter(typeof(long)).ConvertFromString },
        { typeof(string), EZString },
        { typeof(int), TypeDescriptor.GetConverter(typeof(int)).ConvertFromString },
        { typeof(KeyCode), TypeDescriptor.GetConverter(typeof(KeyCode)).ConvertFromString },
        { typeof(Vector3), ConvertVector3FromStr },
        { typeof(Type), Type.GetType },
    };

    static readonly string stringConvertibleTypeName = typeof(IStringConvertible).Name;

    private static Dictionary<Type, Func<object, string>> convertersToString = new Dictionary<Type, Func<object, string>>
    {
        { typeof(Vector3), ConvertVector3ToStr },
    };

    public static Func<string, object> GetConverter(Type type, bool extraConverters = false)
    {
        if (convertersFromString.ContainsKey(type))
        {
            return convertersFromString[type];
        }

        if (extraConverters)
        {
            return TypeDescriptor.GetConverter(type).ConvertFromString;
        }

        return default;
    }

    public static Func<string, object> GetConverter<T>(bool strict = false)
    {
        return GetConverter(typeof(T), strict);
    }

    public static object ConvertFromStr(Type type, string str)
    {
        if (convertersFromString.ContainsKey(type))
        {
            return convertersFromString[type].Invoke(str);
        }

        if (type.GetInterface(stringConvertibleTypeName) != null)
        {
            IStringConvertible convertible = (IStringConvertible)Activator.CreateInstance(type);
            bool success = convertible.ConvertFromString(str);
            if (success)
                return convertible;
        }

        Debug.LogWarning("there's no converter built in for type " +  type.Name + " and type is not IValueConvertible or conversion failed!");

        return TypeDescriptor.GetConverter(type).ConvertFromString(str);
    }

	public static T ConvertFromStr<T>(string str)
    {
        return (T)ConvertFromStr(typeof(T), str);
    }

    public static string ConvertToString(Type type, object obj)
    {
        if (convertersToString.ContainsKey(type))
        {
            return convertersToString[type].Invoke(obj);
        }

        if (type.GetInterface(stringConvertibleTypeName) != null)
        {
            return (obj as IStringConvertible).ConvertToString();
        }

        return obj.ToString();
    }

    public static string ConvertToString<T>(T obj)
    {
        return ConvertToString(typeof(T), obj);
    }

    public static object ConvertVector3FromStr(string str)
    {
        // Remove the parentheses
        if (str.StartsWith("(") && str.EndsWith(")"))
        {
            str = str.Substring(1, str.Length - 2);
        }

        // split the items
        string[] sArray = str.Split(':');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    public static string EZString(string str)
    {
        return str;
    }

    public static string ConvertVector3ToStr(object obj)
    {
        Vector3 vec = (Vector3)obj;

        string str = string.Format("({0}:{1}:{2})", vec.x, vec.y, vec.z);

        return str;
    }
}