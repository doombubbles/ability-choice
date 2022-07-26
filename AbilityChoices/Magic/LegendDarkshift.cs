﻿using Assets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Magic;

public class LegendDarkshift : AbilityChoice
{
    public override string UpgradeId => UpgradeType.LegendOfTheNight;
    public override string AbilityName => nameof(ChampionDarkshift);

    public override string Description1 => "We turn to him, when all hope is lost... and he's got a bit more range";
        
    public override void Apply1(TowerModel model)
    {
        model.IncreaseRange(30);
    }
}