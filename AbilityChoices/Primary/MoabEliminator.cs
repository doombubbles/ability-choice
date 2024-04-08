using BTD_Mod_Helper.Api.Enums;

namespace AbilityChoice.AbilityChoices.Primary;

public class MoabEliminator : AssassinateMOAB
{
    public override string UpgradeId => UpgradeType.MOABEliminator;

    public override string Description1 =>
        "Frequently shoots out mini Moab Eliminator missiles at the strongest Moab on screen.";

    public override string Description2 => "Does extremely further increased MOAB damage.";
}