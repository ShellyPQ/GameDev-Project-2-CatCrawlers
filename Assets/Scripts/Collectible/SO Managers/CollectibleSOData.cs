using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleSOData : ScriptableObject
{
    public abstract void Collect(GameObject objectCollected);
}
