using UnityEngine;

public class TestClass : MonoBehaviour, ISaveable
{
    [SerializeField] private int ivar = 0;
    public void Load()
    {
        this.ivar = DataManager.Instance.ivar;
    }
    public void Save()
    {
        DataManager.Instance.ivar = this.ivar;
    }
}
