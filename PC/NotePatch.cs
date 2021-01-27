using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace Weather
{
    /// <summary>
    /// This patches ClassToPatch.MethodToPatch(Parameter1Type arg1, Parameter2Type arg2)
    /// </summary>
    [HarmonyPatch(typeof(NoteController), "Init")]
    public class NotePatch
    {
        static void Postfix(NoteController __instance)
        {
            GameObject obj = __instance.gameObject;
            MeshRenderer[] mrs = obj.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < BundleLoader.effects.Count; i++)
            {
                for (int x = 0; x < mrs.Length; x++)
                {
                    BundleLoader.effects[i].TrySetNoteMateral(mrs[x]);
                }
            }
        }
    }
}