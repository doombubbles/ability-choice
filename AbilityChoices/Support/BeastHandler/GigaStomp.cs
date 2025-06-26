using BTD_Mod_Helper.Api.Enums;
namespace AbilityChoice.AbilityChoices.Support.BeastHandler;

public class GigaStomp : TRexStomp
{
    public override string AbilityName => "Giga STOMP ";

    public override string UpgradeId => UpgradeType.Giganotosaurus;

    public override string Description1 =>
        "The biggest and most fierce dinosaur of them all, Giganotosaurus can shred almost any Bloon instantly and with ease. " +
        "Stuns from stomping last longer. " +
        "Requires 3 additional Tyrannosaurus Handlers to control.";

    public override string Description2 =>
        "The biggest and most fierce dinosaur of them all, Giganotosaurus can shred almost any Bloon instantly and with ease. " +
        "Stuns last longer. " +
        "Requires 3 additional Tyrannosaurus Handlers to control.";
}