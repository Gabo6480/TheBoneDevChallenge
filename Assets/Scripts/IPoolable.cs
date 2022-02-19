using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    public string getName();

    public void OnSpawned();
    //public void OnDespawned();
}
