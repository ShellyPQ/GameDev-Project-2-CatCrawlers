using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleSOData : ScriptableObject
{
    //TO DO: Add effect manager (sfx, particles, ect)
    public abstract void Collect(GameObject objectCollected);
}
