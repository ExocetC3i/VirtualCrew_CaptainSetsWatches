using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System;
using System.Reflection;
using System.Linq;

namespace RemoveFirstOfficerRequirement
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(VirtualCrewPluginGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class RemoveFirstOfficerRequirementPlugin : BaseUnityPlugin
    {
        internal const string PluginGuid = "com.sailwind.removefirstofficerrequirement";
        internal const string PluginName = "Remove First Officer Requirement";
        internal const string PluginVersion = "1.0.4";
        internal const string VirtualCrewPluginGuid = "com.zorkinian.virtualcrew";

        private Harmony _harmony;

        private void Awake()
        {
            if (!Chainloader.PluginInfos.ContainsKey(VirtualCrewPluginGuid))
            {
                Logger.LogWarning("SailwindVirtualCrew is not loaded; the HasFirstOfficer patch was not applied.");
                return;
            }

            var managerType = AccessTools.TypeByName("SailwindVirtualCrew.VirtualCrewManager");
            var hasFirstOfficerGetter = managerType == null
                ? null
                : AccessTools.PropertyGetter(managerType, "HasFirstOfficer");
            if (hasFirstOfficerGetter == null || hasFirstOfficerGetter.ReturnType != typeof(bool))
            {
                Logger.LogError("SailwindVirtualCrew does not expose the expected HasFirstOfficer property; patch was not applied.");
                return;
            }

            _harmony = new Harmony(PluginGuid);
            try
            {
                var hasFirstOfficerPostfix = typeof(HasFirstOfficerPatch).GetMethod(
                    nameof(HasFirstOfficerPatch.Postfix),
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (hasFirstOfficerPostfix == null)
                {
                    Logger.LogError("Could not resolve the HasFirstOfficer postfix; shift requirement remains unchanged.");
                    _harmony.UnpatchSelf();
                    _harmony = null;
                    return;
                }

                Patch(hasFirstOfficerGetter, hasFirstOfficerPostfix);
                if (!HasOurPatch(hasFirstOfficerGetter))
                {
                    Logger.LogError("Harmony did not register the HasFirstOfficer postfix; shift requirement remains unchanged.");
                    _harmony.UnpatchSelf();
                    _harmony = null;
                    return;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError($"Could not install the HasFirstOfficer patch: {exception}");
                _harmony.UnpatchSelf();
                _harmony = null;
                return;
            }
        }

        private void Patch(MethodBase target, MethodInfo postfix)
        {
            _harmony.Patch(
                target,
                postfix: new HarmonyMethod(postfix));
        }

        private bool HasOurPatch(MethodBase target)
        {
            var patches = Harmony.GetPatchInfo(target);
            return patches?.Postfixes != null
                && patches.Postfixes.Any(patch => patch.owner == PluginGuid);
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
