using AbilityChoice.Displays;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace AbilityChoice.AbilityChoices.Primary;

public class MoabEliminator : AssassinateMOAB
{
    public override string UpgradeId => UpgradeType.MOABEliminator;
    public override string Description1 => "Frequently shoots out mini Moab Eliminator missiles at the strongest Moab on screen.";
    public override string Description2 => "Does extremely further increased MOAB damage.";

    protected override void ApplyDisplay(ProjectileModel projectileModel)
    {
        projectileModel.ApplyDisplay<MiniMoabEliminator>();
    }
}