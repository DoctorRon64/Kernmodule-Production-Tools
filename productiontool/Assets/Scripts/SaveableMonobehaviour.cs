using System.Collections.Generic;
using UnityEngine;

public abstract class SaveableMonobehaviour : MonoBehaviour, ISaveable
{


    public abstract void Load();

    public virtual SaveFile Save()
    {
        SaveFile file = new SaveFile();

        System.Reflection.FieldInfo[] fields = this.GetType().GetFields(
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance
            );
        foreach (System.Reflection.FieldInfo field in fields)
        {
            file.saveData.Add(field.GetValue(this));
        }

        return file;
    }
}