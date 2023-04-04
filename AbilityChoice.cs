using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using Il2CppNinjaKiwi.Common;
using MelonLoader;
using UnityEngine.UI;

namespace AbilityChoice;

public abstract class AbilityChoice : ModVanillaUpgrade
{
    public static readonly Dictionary<string, AbilityChoice> Cache = new();

    public abstract string Description1 { get; }
    public virtual string Description2 => "";

    public virtual string AbilityName => Regex.Replace(
        Name,
        "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
        " $1",
        RegexOptions.Compiled).Trim();


    protected TowerModel BaseTowerModel { get; private set; }

    protected MelonPreferences_Entry<int> setting;

    protected string DefaultDescription { get; private set; }

    public bool Enabled => setting?.Value is 1 or 2;

    protected bool Mode2 => setting?.Value == 2;

    public override void Register()
    {
        base.Register();
        BaseTowerModel = GetAffected(Game.instance.model).First();
        Cache[UpgradeId] = this;
        setting = AbilityChoiceMod.AbilityChoiceSettings.CreateEntry(Id, 1);
        DefaultDescription = LocalizationManager.Instance.textTable[UpgradeId + " Description"];
        // MelonLogger.Msg($"Registered AbilityChoice {Name} for upgrade {UpgradeId} and value {setting.Value}");
    }


    public string CurrentDescription => Mode2 && !string.IsNullOrEmpty(Description2) ? Description2 : Description1;

    public void Toggle()
    {
        setting.Value++;
        if (setting.Value > 2 || Mode2 && string.IsNullOrEmpty(Description2))
        {
            setting.Value = 0;
        }
    }

    public void SetMode(int i)
    {
        setting.Value = i;
        if (setting.Value == 2 && string.IsNullOrEmpty(Description2))
        {
            setting.Value = 1;
        }
    }

    public sealed override IEnumerable<ModContent> Load()
    {
        yield return this;
        yield return new AbilityChoiceDescription(this);
    }

    public virtual void Apply1(TowerModel model)
    {
    }

    public virtual void Apply2(TowerModel model)
    {
    }

    public virtual void ApplyBoth(TowerModel model)
    {
    }

    public sealed override void Apply(TowerModel towerModel)
    {
        if (Mode2)
        {
            Apply2(towerModel);
        }
        else
        {
            Apply1(towerModel);
        }

        ApplyBoth(towerModel);

        RemoveAbility(towerModel);
    }

    public override IEnumerable<TowerModel> GetAffected(GameModel gameModel)
    {
        return base.GetAffected(gameModel)
            .Where(model => AbilityModel(model) != null)
            .OrderBy(model => model.appliedUpgrades.Length);
    }

    public static void HandleIcon(UpgradeDetails upgradeDetails)
    {
        var abilityObject = upgradeDetails.abilityObject;
        var circle = abilityObject.GetComponent<Image>();
        if (upgradeDetails.AbilityChoice() is AbilityChoice abilityChoice)
        {
            abilityObject.SetActive(true);
            circle.SetSprite(IconForMode(abilityChoice.setting.Value));
        }
        else
        {
            circle.SetSprite(VanillaSprites.NotificationYellow);
        }
    }

    public static string IconForMode(int mode) => mode switch
    {
        1 => VanillaSprites.NotifyRed,
        2 => VanillaSprites.NotifyBlue,
        _ => VanillaSprites.NotificationYellow
    };

    public virtual AbilityModel AbilityModel(TowerModel model)
    {
        return model.GetBehaviors<AbilityModel>()
            .FirstOrDefault(abilityModel => abilityModel.displayName == AbilityName);
    }

    public virtual void RemoveAbility(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        if (abilityModel != null)
        {
            model.RemoveBehavior(abilityModel);
        }
        else
        {
            // MelonLogger.Warning($"Couldn't apply ModAbilityChoice {Name}");
        }
    }


    protected float CalcAvgBonus(float uptime, float dpsMult)
    {
        return uptime * dpsMult + (1 - uptime);
    }

    protected void TechBotify(TowerModel model)
    {
        var ability = AbilityModel(model);

        // ability.enabled = false;
        ability.CooldownSpeedScale = -1;
        var name = $"ActivateAbilityAfterIntervalModel_{AbilityName.Replace(" ", "")}";
        if (model.behaviors.FirstOrDefault(a => a.name == name).Is<ActivateAbilityAfterIntervalModel>(out var m))
        {
            m.abilityModel = ability;
            m.interval = ability.Cooldown;
        }
        else
        {
            model.AddBehavior(new ActivateAbilityAfterIntervalModel(name, ability, ability.Cooldown));
        }
    }
}