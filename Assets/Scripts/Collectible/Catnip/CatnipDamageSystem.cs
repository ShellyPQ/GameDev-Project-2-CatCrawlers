using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CatnipDamageSystem 
{
    public static bool frenzyActive = false;

    public static void EnableFrenzyMode() => frenzyActive = true;
    public static void DisableFrenzyMode() => frenzyActive = false;
}
