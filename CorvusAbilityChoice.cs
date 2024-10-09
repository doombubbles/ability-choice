using System;
using System.Collections.Generic;
using AbilityChoice.Descriptions;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.CorvusSpells;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Corvus.TowerManager;
using Il2CppAssets.Scripts.Unity;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace AbilityChoice;

public abstract class CorvusAbilityChoice : HeroAbilityChoice
{
    public sealed override string HeroId => TowerType.Corvus;

    protected override float RegistrationPriority => base.RegistrationPriority + 1;

    public sealed override Dictionary<int, string> Descriptions1 => new();
    public sealed override Dictionary<int, string> Descriptions2 => new();

    public virtual string Description1 => null;
    public virtual string Description2 => null;

    public virtual string Description1Lvl20 => null;
    public virtual string Description2Lvl20 => null;

    public CorvusSpellModel Spell(TowerModel model) => model
        .GetBehaviors<CorvusSpellModel>()
        .FirstOrDefault(spellModel => spellModel.locsId == Name);

    public CorvusContinuousSpellModel ContinuousSpell(TowerModel model) => model
        .GetBehaviors<CorvusContinuousSpellModel>()
        .FirstOrDefault(spellModel => spellModel.locsId == Name);

    public CorvusInstantSpellModel InstantSpell(TowerModel model) => model
        .GetBehaviors<CorvusInstantSpellModel>()
        .FirstOrDefault(spellModel => spellModel.locsId == Name);

    public override bool AppliesTo(TowerModel towerModel) => Spell(towerModel) != null;

    public override SpriteReference Icon => Spell(Game.instance.model.GetTowerWithName("Corvus 20")).defaultIcon;

    public CorvusSpellType SpellType => Enum.Parse<CorvusSpellType>(Name);

    protected const int ContinuousFactor = 20;

    public static bool EnabledForSpell(CorvusSpellType spellType) => GetContent<CorvusAbilityChoice>()
        .Any(choice => choice.SpellType == spellType && choice.Enabled);

    public override void Apply(GameModel gameModel)
    {
        foreach (var towerModel in GetAffected(gameModel))
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
        }
    }


    public override IEnumerable<ModContent> Load()
    {
        yield return this;

        if (Description1 != null)
            yield return new CorvusAbilityChoiceDescription(this, 1, Description1);
        if (Description2 != null)
            yield return new CorvusAbilityChoiceDescription(this, 2, Description2);
        if (Description1Lvl20 != null)
            yield return new CorvusAbilityChoiceDescription(this, 1, Description1Lvl20, true);
        if (Description2Lvl20 != null)
            yield return new CorvusAbilityChoiceDescription(this, 2, Description2Lvl20, true);
    }
}