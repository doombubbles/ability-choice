using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;

namespace AbilityChoice.AbilityChoices.Primary;

public class SuperMaelstrom : BladeMaelstrom
{
    public override string UpgradeId => UpgradeType.SuperMaelstrom;

    public override string Description1 => "Shoots out a faster and larger swirl of global, high pierce blades.";
    public override string Description2 => "Shoots out more homing blades with increased range and pierce.";

    protected override int Pierce => 9;
    protected override float Lifespan => 16;

    protected override void Apply1(TowerModel model)
    {
        base.Apply1(model);

        if (Mode2)
        {
            model.GetDescendant<ArcEmissionModel>().count *= 2;
            model.range += 11;
            model.GetAttackModel().range += 11;
        }
    }
}