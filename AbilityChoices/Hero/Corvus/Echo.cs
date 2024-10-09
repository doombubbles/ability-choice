using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Echo : CorvusAbilityChoice
{
    public override string Description2 =>
        "The Spirit splits itself in two. Commands sent to one are sent to both. The Echo always targets first, but has reduced attack speed.";

    protected override int Order => 0;

    public override void Apply1(TowerModel model)
    {
        var spell = InstantSpell(model);

        spell.initialManaCost = (int) (spell.initialManaCost / spell.duration);

        spell.duration = 1;
        spell.cooldown = 0;
    }

    public override void Apply2(TowerModel model)
    {
        var spell = InstantSpell(model);

        var factor = spell.duration / (spell.duration + spell.cooldown);

        spell.initialManaCost = (int) (factor * (spell.initialManaCost / spell.duration));

        spell.duration = 1;
        spell.cooldown = 0;

        var tower = spell.GetDescendant<TowerModel>();

        tower.GetDescendants<WeaponModel>().ForEach(weaponModel => weaponModel.Rate /= factor);
    }
}