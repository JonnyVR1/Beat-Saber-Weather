using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace Weather.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteController), "Init")]
    public class NotePatch
    {
        [UsedImplicitly]
        static void Postfix(NoteController __instance)
        {
            var obj = __instance.gameObject;
            var mrs = obj.GetComponentsInChildren<MeshRenderer>();
            foreach (var t in BundleLoader.Effects)
            {
                foreach (var t1 in mrs)
                {
                    t.TrySetNoteMaterial(t1);
                }
            }
        }
    }
}