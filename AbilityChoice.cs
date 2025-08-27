﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Unity;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace AbilityChoice;

public abstract class AbilityChoice : NamedModContent
{
    protected MelonPreferences_Entry<int> setting;

    public virtual string AbilityName => Regex.Replace(
        Name,
        "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
        " $1",
        RegexOptions.Compiled).Trim();

    public virtual string BackUpAbilityName => null;

    public override string DisplayName => $"[{AbilityName} Ability]";

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

    protected readonly HashSet<string> affectedIds = [];
    protected readonly List<string> affectedOrder = [];

    protected override float RegistrationPriority => 25f;

    public override void Register()
    {
        setting = AbilityChoiceMod.AbilityChoiceSettings.CreateEntry(Id, 1);
        // MelonLogger.Msg($"Registered AbilityChoice {Name} for upgrade {UpgradeId} and value {setting.Value}");

        foreach (var id in GetAffected(Game.instance.model).Select(model => model.name))
        {
            if (affectedIds.Add(id))
            {
                affectedOrder.Add(id);
            }
        }
    }

    public void Toggle()
    {
        setting.Value++;
        if (setting.Value > 2 || Mode2 && !HasMode2)
        {
            setting.Value = 0;
        }
    }

    public virtual void Apply1(TowerModel model)
    {
    }

    public virtual void Apply2(TowerModel model)
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
            .FirstOrDefault(abilityModel =>
                abilityModel.displayName == AbilityName || abilityModel.displayName == BackUpAbilityName);

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
        foreach (var towerModel in GetAffectedCached(gameModel))
        {
            Apply(towerModel);
        }
    }

    public IEnumerable<TowerModel> GetAffectedCached(GameModel gameModel)
    {
        var towers = gameModel.towers
            .Where(model => affectedIds.Contains(model.name))
            .ToDictionary(model => model.name);
        return affectedOrder.Select(s => towers[s]).Where(AppliesTo);
    }

    public abstract IEnumerable<TowerModel> GetAffected(GameModel gameModel);

    public static float CalcAvgBonus(float uptime, float dpsMult) => uptime * dpsMult + (1 - uptime);

    internal void TechBotify(TowerModel model, AbilityModel ability = null)
    {
        ability ??= AbilityModel(model);

        // ability.enabled = false;
        if (!ability.displayName.Contains(AbilityChoiceMod.DontShowAbilityKeyword))
        {
            ability.displayName += AbilityChoiceMod.DontShowAbilityKeyword;
        }

        var name = $"{nameof(TechBotify)}_{ability.displayName.Replace(" ", "")}";
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

    /// <summary>
    /// If the icon needs to be manually set
    /// </summary>
    public virtual SpriteReference Icon => null;
}