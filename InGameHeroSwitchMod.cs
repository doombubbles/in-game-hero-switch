using System.Linq;
using Assets.Scripts.Models.TowerSets;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.InGame.RightMenu;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using InGameHeroSwitch;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(InGameHeroSwitchMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace InGameHeroSwitch;

public class InGameHeroSwitchMod : BloonsTD6Mod
{
    private static readonly ModSettingBool CycleIfPlaced = new(false)
    {
        description = "Whether to still allow cycling to different heroes if one is already placed down.",
        button = true
    };

    private static readonly ModSettingHotkey CycleUp = new(KeyCode.PageUp);

    private static readonly ModSettingHotkey CycleDown = new(KeyCode.PageDown);

    private static string realSelectedHero;

    private static void ChangeHero(int delta)
    {
        var hero = realSelectedHero ?? InGame.instance.SelectedHero;

        var purchaseButton = ShopMenu.instance.GetTowerButtonFromBaseId(hero);
        if (!CycleIfPlaced &&
            purchaseButton != null &&
            purchaseButton.GetLockedState() ==
            TowerPurchaseLockState.TowerInventoryLocked)
        {
            return;
        }

        var towerInventory = InGame.instance.GetTowerInventory();
        var unlockedHeroes = Game.instance.GetPlayerProfile().unlockedHeroes;

        var heroDetailsModels = InGame.instance.GetGameModel().heroSet.Select(tdm => tdm.Cast<HeroDetailsModel>());
        var heroes = heroDetailsModels as HeroDetailsModel[] ?? heroDetailsModels.ToArray();

        var index = heroes.First(hdm => hdm.towerId == hero).towerIndex;
        var newHero = "";
        while (!unlockedHeroes.Contains(newHero))
        {
            index += delta;
            index = (index + heroes.Length) % heroes.Length;
            newHero = heroes.First(hdm => hdm.towerIndex == index).towerId;
        }

        foreach (var unlockedHero in unlockedHeroes)
        {
            towerInventory.towerMaxes[unlockedHero] = 0;
        }

        towerInventory.towerMaxes[newHero] = 1;

        realSelectedHero = newHero;
        ShopMenu.instance.RebuildTowerSet();
        foreach (var button in ShopMenu.instance.activeTowerButtons)
        {
            button.Update();
        }
    }

    public override void OnUpdate()
    {
        if (CycleUp.JustPressed())
        {
            ChangeHero(-1);
        }
        else if (CycleDown.JustPressed())
        {
            ChangeHero(1);
        }
    }
}