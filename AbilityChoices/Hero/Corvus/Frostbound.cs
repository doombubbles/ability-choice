using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace AbilityChoice.AbilityChoices.Hero.Corvus;

public class Frostbound : CorvusAbilityChoice
{
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

        var clear = spell.GetDescendant<ClearHitBloonsModel>();
        clear.interval /= factor;
        clear.intervalFrames = (int) (clear.intervalFrames / factor);

        var refresh = spell.GetDescendant<RefreshPierceModel>();
        refresh.interval /= factor;
        refresh.intervalFrames = (int) (refresh.intervalFrames / factor);
    }
}