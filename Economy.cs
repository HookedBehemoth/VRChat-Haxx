using HarmonyLib;
using Il2CppVRC.Economy;
using Il2CppVRC.SDKBase;

[HarmonyPatch(typeof(Store), nameof(Store.DoesPlayerOwnProduct), typeof(VRCPlayerApi), typeof(IProduct))]
class EconomyPatch
{
	static bool Prefix(ref VRCPlayerApi player, IProduct product, ref bool __result)
	{
		return !(__result = player.isLocal);
	}
}
