using System.Collections.Generic;
using UnityEngine;

public class Testclass : MonoBehaviour, ISaveable
{
    public int hey = 10;
    public string hallo = "apdfas ralph hallo";
    public float kipje = 10.23f;

    public Dictionary<string, object> SaveData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("hey", hey);
        data.Add("hallo", hallo);
        data.Add("kipje", kipje);

        return data;
    }
}