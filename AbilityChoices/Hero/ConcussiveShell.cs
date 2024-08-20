using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Hero;

public class ConcussiveShell : HeroAbilityChoice
{
    public override string HeroId => TowerType.StrikerJones;

    public override Dictionary<int, string> Descriptions1 => new()
    {
        { 3, "Occasionally shoots a shell at the strongest Bloon on screen, stunning and damaging it." },
        { 15, "Concussive Shells are fired more frequently." }
    };

    public override Dictionary<int, string> Descriptions2 => new()
    {
        { 3, "Normal attacks will briefly stuns Bloons." },
        { 9, "Increased attack speed and attack stun duration." },
        { 14, "Increased blast radius and stun duration." },
        { 15, "Increased pierce." }
    };

    private const int Factor = 2;
    private const int Factor2 = 10;


    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);
        var attack = ability.GetDescendant<AttackModel>().Duplicate("ConcussiveShell");
        var weapon = attack.weapons[0];

        weapon.GetDescendants<DamageModel>().ForEach(damageModel => damageModel.damage /= Factor);
        weapon.GetDescendants<SlowModel>().ForEach(slowModel =>
        {
            slowModel.Lifespan /= Factor;
            slowModel.dontRefreshDuration = true;
        });
        weapon.GetDescendants<SlowModifierForTagModel>().ForEach(slowModel => slowModel.lifespanOverride /= Factor);
        weapon.Rate = ability.Cooldown / Factor;

        model.AddBehavior(attack);
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        var slow = ability.GetDescendant<SlowModel>().Duplicate();
        var slowModifiers = ability.GetDescendants<SlowModifierForTagModel>().ToList()
            .Select(modifier => modifier.Duplicate());
        var projectile = model.GetAttackModel().GetDescendants<ProjectileModel>().ToList()
            .First(projectileModel => projectileModel.GetDamageModel() != null);

        slow.Lifespan /= Factor2;
        slow.dontRefreshDuration = true;
        projectile.AddBehavior(slow);

        foreach (var slowModifier in slowModifiers)
        {
            slowModifier.lifespanOverride /= Factor2;
            projectile.AddBehavior(slowModifier);
        }

        projectile.UpdateCollisionPassList();

        if (model.tier >= 15)
        {
            projectile.GetDamageModel().damage++;
            projectile.pierce += 12;
        }
    }
}