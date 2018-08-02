using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStringConvertible
{
    bool ConvertFromString(string value);
    string ConvertToString();
}
