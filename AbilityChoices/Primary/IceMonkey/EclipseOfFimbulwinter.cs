using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace AbilityChoice.AbilityChoices.Primary.IceMonkey;

public class EclipseOfFimbulwinter : TowerAbilityChoice
{
    public override string UpgradeId => UpgradeType.IcemonkeyParagon;

    public override string AbilityName => "Eclipse of Fimbulwinter";

    public override string Description1 =>
        "As the eternal frost descends and cold reigns supreme, even the baddest of Bloons are helpless against its icy grasp. " +
        "Periodically freezes and slows all Bloons, even BADs and Bosses somewhat, and the first Boss skull each tier is nullified.";

    public override string Description2 =>
        "As the eternal frost descends and cold reigns supreme, even the baddest of Bloons are helpless against its icy grasp. " +
        "All Bloons are permanently slowed, even BADs and Bosses somewhat.";

    public const int Factor = 5;

    public override void Apply1(TowerModel model)
    {
        var ability = AbilityModel(model);

        ability.maxActivationsPerRound = -1;
        ability.RemoveBehavior<AbilityTriggerEclipseModel>();
        ability.RemoveBehavior<CreateEffectOnAbilityModel>();
        ability.RemoveBehavior<CreateSoundOnAbilityModel>();

        var createProj = ability.GetBehavior<CreateProjectileOnAbilityActivateModel>();
        var realProj = createProj.projectile.GetDescendant<ProjectileModel>();
        createProj.RemoveChildDependant(createProj.projectile);
        createProj.AddChildDependant(realProj);
        createProj.projectile = realProj;

        realProj.RemoveBehavior<AddHeatToBloonModel>();

        ability.GetDescendants<ActivateSlowZoneOnAbilityModel>().ForEach(slow =>
        {
            slow.lifetime /= Factor;
        });
        ability.GetDescendants<FreezeModel>().ForEach(freeze =>
        {
            freeze.Lifespan /= Factor;
        });
        ability.GetDescendants<SlowModel>().ForEach(slow =>
        {
            slow.Lifespan /= Factor;
        });

        ability.Cooldown /= Factor;

        TechBotify(model);

        model.GetDescendants<AddHeatToBloonModel>().ForEach(heat =>
        {
            heat.heatAmount *= 2;
        });
    }

    public override void Apply2(TowerModel model)
    {
        var ability = AbilityModel(model);

        foreach (var activateSlow in ability.GetBehaviors<ActivateSlowZoneOnAbilityModel>())
        {
            model.AddBehavior(SlowBloonsZoneModel.Create(new()
            {
                name = activateSlow.name.Replace(nameof(ActivateSlowZoneOnAbilityModel), nameof(SlowBloonsZoneModel)),
                zoneRadius = activateSlow.zoneRadius,
                mutationId = activateSlow.mutationId,
                isUnique = activateSlow.isUnique,
                filters = activateSlow.filters,
                speedScale = activateSlow.speedScale,
                speedChange = activateSlow.speedChange,
                bindRadiusToTowerRange = activateSlow.bindRadiusToTowerRange,
                radiusOffset = activateSlow.radiusOffset,
                bloonTag = activateSlow.bloonTag,
                bloonTags = activateSlow.bloonTags,
                inclusive = activateSlow.inclusive,
            }));
        }
    }


}