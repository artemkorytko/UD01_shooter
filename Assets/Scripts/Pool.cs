using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T: MonoBehaviour
{
    [SerializeField] protected GameObject _prefab = null;
    [SerializeField] protected int _startCounnt = 1;
    [SerializeField] protected int _resizeCount = 1;
    protected Stack<T> _free = new Stack<T>();
    protected int _objectsCount = 0;

    public virtual void Init()
    {
        for (int i = 0; i < _startCounnt; i++)
        {
            _free.Push(GenerateObject());
        }
    }

    protected T GenerateObject()
    {
        GameObject newObject = Instantiate(_prefab, transform);
        newObject.name = newObject.name + "(PoolObject)";
        newObject.SetActive(false);
        _objectsCount++;

        return newObject.GetComponent<T>();
    }

    public T GetObject()
    {
        if (_free.Count == 0)
        {
            for (int i = 0; i < _resizeCount; i++)
            {
                _free.Push(GenerateObject());
            }
        }

        return _free.Pop();
    }

    public void Return(T obj)
    {
        _free.Push(obj);
    }
}
