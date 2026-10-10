using HarmonyLib;

namespace RemoveFirstOfficerRequirement
{
    internal static class HasFirstOfficerPatch
    {
        internal static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
}
