using System.Collections.Generic;
using System.Text.RegularExpressions;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;

namespace AbilityChoice;

public abstract class AbilityChoice : NamedModContent
{
    protected MelonPreferences_Entry<int> setting;

    public virtual string AbilityName => Regex.Replace(
        Name,
        "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
        " $1",
        RegexOptions.Compiled).Trim();


    public bool Enabled => setting?.Value is 1 or 2;

    protected bool Mode2 => setting?.Value == 2;

    public int Mode
    {
        get => setting?.Value ?? 0;
        set
        {
            setting.Value = value;
            if (setting.Value == 2 && !HasMode2)
            {
                setting.Value = 1;
            }
        }
    }

    protected abstract bool HasMode2 { get; }

    public override void Register()
    {
        setting = AbilityChoiceMod.AbilityChoiceSettings.CreateEntry(Id, 1);
        // MelonLogger.Msg($"Registered AbilityChoice {Name} for upgrade {UpgradeId} and value {setting.Value}");
    }

    public void Toggle()
    {
        setting.Value++;
        if (setting.Value > 2 || Mode2 && !HasMode2)
        {
            setting.Value = 0;
        }
    }

    protected virtual void Apply1(TowerModel model)
    {
    }

    protected virtual void Apply2(TowerModel model)
    {
    }

    protected virtual void ApplyBoth(TowerModel model)
    {
    }

    public void Apply(TowerModel towerModel)
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

    protected virtual AbilityModel AbilityModel(TowerModel model) =>
        model.GetBehaviors<AbilityModel>()
            .FirstOrDefault(abilityModel => abilityModel.displayName == AbilityName);

    protected virtual void RemoveAbility(TowerModel model)
    {
        var abilityModel = AbilityModel(model);
        if (abilityModel != null)
        {
            model.RemoveBehavior(abilityModel);
        }
        // MelonLogger.Warning($"Couldn't apply ModAbilityChoice {Name}");
    }

    public virtual bool AppliesTo(TowerModel towerModel) => AbilityModel(towerModel) != null;

    public virtual void Apply(GameModel gameModel)
    {
        foreach (var towerModel in GetAffected(gameModel))
        {
            Apply(towerModel);
        }
    }

    public abstract IEnumerable<TowerModel> GetAffected(GameModel gameModel);

    protected float CalcAvgBonus(float uptime, float dpsMult) => uptime * dpsMult + (1 - uptime);

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