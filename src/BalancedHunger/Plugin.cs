using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;

namespace BalancedHunger;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    private const int TARGET_PLAYER_COUNT = 4;

    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;

        Harmony harmony = new("NachoToast.BalancedHunger");

        harmony.Patch(
            original: AccessTools.PropertyGetter(
                type: typeof(Ascents),
                name: nameof(Ascents.hungerRateMultiplier)),
            postfix: new HarmonyMethod(
                methodType: typeof(Plugin),
                methodName: nameof(GetHungerRateMultiplier_Postfix)));
    }

    private static void GetHungerRateMultiplier_Postfix(ref float __result)
    {
        int playerCount = PhotonNetwork.PlayerList.Length;

#if DEBUG
        float old = __result;
#endif

        float multiplier = playerCount switch
        {
            5 => (float)TARGET_PLAYER_COUNT / 5f,
            6 => (float)TARGET_PLAYER_COUNT / 6f,
            7 => (float)TARGET_PLAYER_COUNT / 7f,
            8 => (float)TARGET_PLAYER_COUNT / 8f,
            9 => (float)TARGET_PLAYER_COUNT / 9f,
            10 => (float)TARGET_PLAYER_COUNT / 10f,
            11 => (float)TARGET_PLAYER_COUNT / 11f,
            12 => (float)TARGET_PLAYER_COUNT / 12f,
            _ => (float)TARGET_PLAYER_COUNT / PhotonNetwork.PlayerList.Length
        };

        __result *= multiplier;

#if DEBUG
        Log.LogInfo($"[Balanced Hunger] Changed from {old:F3} to {__result:F3}");
#endif
    }
}
