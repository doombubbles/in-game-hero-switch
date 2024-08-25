using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using InGameHeroSwitch;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(InGameHeroSwitchMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace InGameHeroSwitch;

public class InGameHeroSwitchMod : BloonsTD6Mod
{
    public static readonly ModSettingBool CycleIfPlaced = new(false)
    {
        description = "Whether to still allow cycling to different heroes if one is already placed down.",
        button = true
    };

    public static readonly ModSettingHotkey CycleUp = new(KeyCode.PageUp);

    public static readonly ModSettingHotkey CycleDown = new(KeyCode.PageDown);

    public override void OnUpdate() => InGameHeroSwitchUtility.Update();
}