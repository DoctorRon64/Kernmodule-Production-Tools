using System.Collections.Generic;
using UnityEngine;

public class Testclass : MonoBehaviour, ISaveable
{
    public int hey = 10;
    public string hallo = "apdfas ralph hallo";
    public float kipje = 10.23f;

    public void Load()
    {

    }

    public object Save()
    {
        return this;
    }
}