using System;
using System.Collections.Generic;
using System.IO;
using BTD_Mod_Helper.Api.Commands;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.CorvusSpells;
using Il2CppAssets.Scripts.Unity;

namespace AbilityChoice;

internal class GenerateReadme : ModCommand<GenerateCommand>
{
    private const string ReadMe = "README.md";

    private const string Title =
        """
        # All Ability Choices

        Left column is the new description for the Red Choice, right column is the new description for the Blue Choice (if there is one)

        """;

    private static readonly string[] TowerSets = ["Primary", "Military", "Magic", "Support", "Hero"];

    private static string FilePath =>
        Path.Combine(ModHelper.ModSourcesDirectory, nameof(AbilityChoice), "AbilityChoices");


    public override bool Execute(ref string resultText)
    {
        try
        {
            Generate();
            return true;
        }
        catch (Exception e)
        {
            resultText = e.Message;
            ModHelper.Error<AbilityChoiceMod>(e);
        }

        return false;
    }

    public override string Command => "abilitychoicemds";

    public override string Help => "Generate ability choice readmes";

    public static void Generate()
    {
        var total = Title;

        foreach (var towerSet in TowerSets)
        {
            var text = $"## {towerSet}\n{Comment(towerSet)}";

            var abilityChoices = GetContent<AbilityChoice>()
                .Where(abilityChoice => !abilityChoice.GetType().IsAbstract &&
                                        abilityChoice.GetType().Namespace!.Contains(towerSet))
                .GroupBy(abilityChoice => abilityChoice.GetType().Namespace!.Split(".").Last());

            foreach (var choices in abilityChoices.OrderBy(SortIndex))
            {
                var result = GenerateGroup(choices.Key, choices.OrderBy(SortIndex));
                SaveMd(result, Path.Combine(towerSet, choices.Key, ReadMe));
                text += "\n\n" + result;
            }

            SaveMd(text, Path.Combine(towerSet, ReadMe));

            total += "\n\n" + text;
        }

        SaveMd(total, ReadMe);
    }

    private static void SaveMd(string text, string path) => File.WriteAllText(Path.Combine(FilePath, path), text);

    private static int SortIndex(IGrouping<string, AbilityChoice> choices) =>
        Enumerable.Concat(Game.instance.model.towerSet, Game.instance.model.heroSet)
            .ToList()
            .FindIndex(model => model.towerId == choices.Key);

    private static int SortIndex(AbilityChoice abilityChoice) => abilityChoice switch
    {
        TowerAbilityChoice t => Game.instance.model.GetUpgrade(t.UpgradeId).tier,
        CorvusAbilityChoice c => Game.instance.model.GetTowerWithName("Corvus 20").GetBehaviors<CorvusSpellModel>()
            .First(spell => spell.locsId == c.Name).unlocksAtLevel,
        HeroAbilityChoice h => Enumerable.Min([
            ..h.Descriptions1?.Keys.ToArray() ?? [], ..h.Descriptions2?.Keys.ToArray() ?? []
        ]),
        _ => 0
    };

    private static string GenerateGroup(string groupName, IEnumerable<AbilityChoice> abilityChoices) =>
        $"""
         ### {groupName.Localize()}
         {Comment(groupName)}

         <table>
            {abilityChoices.Select(Generate).Join(delimiter: "")}
         </table>
         """;

    private static string Generate(AbilityChoice abilityChoice) => abilityChoice switch
    {
        TowerAbilityChoice towerAbilityChoice => GenerateEntry(towerAbilityChoice),
        /*GeraldoAbilityChioce geraldoAbilityChoice => GenerateEntry(geraldoAbilityChoice),*/
        CorvusAbilityChoice corvusAbilityChoice => GenerateEntry(corvusAbilityChoice),
        HeroAbilityChoice heroAbilityChoice => GenerateEntry(heroAbilityChoice),
        _ => throw new ArgumentException("")
    };


    private static string GenerateEntry(TowerAbilityChoice abilityChoice) =>
        $"""
         <tr>
             <td align='center'>
                <h4>{abilityChoice.UpgradeId.Localize()}</h4>
             </td>
             <td>
                {(string.IsNullOrEmpty(abilityChoice.Description1) ? "-" : abilityChoice.Description1)}
             </td>
             <td>
                {(string.IsNullOrEmpty(abilityChoice.Description2) ? "-" : abilityChoice.Description2)}
             </td>
         </tr>
         """;

    private static string GenerateEntry(HeroAbilityChoice abilityChoice) =>
        $"""
         <tr>
             <td align='center'>
                 <h4>{abilityChoice.AbilityName.Localize()}</h4>
             </td>
             <td>
                {(abilityChoice.Descriptions1 is {Count: > 0} ? abilityChoice.Descriptions1.Select(pair => $"Lvl {pair.Key}: {pair.Value}").Join(delimiter: "<br/>") : "-")}
             </td>
             <td>
                {(abilityChoice.Descriptions2 is {Count: > 0} ? abilityChoice.Descriptions2.Select(pair => $"Lvl {pair.Key}: {pair.Value}").Join(delimiter: "<br/>") : "-")}
             </td>
         </tr>
         """;

    private static string GenerateEntry(CorvusAbilityChoice abilityChoice) =>
        $"""
         <tr>
             <td align='center'>
                 <h4>{abilityChoice.AbilityName.Localize()}</h4>
             </td>
             <td>
                {abilityChoice.Description1 ?? "-"}
             </td>
             <td>
                {abilityChoice.Description2 ?? "-"}
             </td>
         </tr>
         """;

    private static string Comment(string group) => group switch
    {
        "Corvus" =>
            """
            - Continuous Spells (Spear, Aggression, Malevolence, Storm)
              - No longer have an upfront mana cost
              - When disabled due to not enough mana, will automatically reenable when enough mana is regained
              - Red Choice - Full power of effect, but increased recurring mana cost to compensate for no upfront cost
              - Blue Choice - Same recurring mana cost, but reduced power of effect to compensate for no upfront cost 
            - Instant Spells (Repel, Echo, Haste, Trample, Frostbound, Ember, Ancestral Might, Overload, Nourishment, Vision)
              - Now behave like Continuous Spells (also auto re-enable and have no upfront cost)
              - Red Choice - Full power of effect, increased recurring mana cost to compensate for 100% uptime potential
              - Blue Choice - Same recurring mana cost, but reduce power of effect to compensate for 100% uptime potential
            - Special Behaviors
              - Trample summons horses that follow the spirit rather than sending them and the spirit along the track
              - Overload no longer makes the spirit disappear, but makes less powerful explosions to compensate (red choice does medium explosions every 10 seconds, blue does small explosions every second)
              - Nourishment just has a Red choice for passively sacrificing 13 mana per second
              - Vision's blue choice does not strip camo but still gives sight
              - Soul Barrier functions normally, but it's one red choice will make it automatically be used if a leak happens
            """,
        "Geraldo" =>
            """
            Geraldo ability choices all have only a Red Ability Choice option. 
            The left column is the changed upgrade descriptions for each hero level, 
            and the right column is the changed item shop descriptions for each hero level. 
            """,
        "Hero" => "Hero Ability Choices list the new upgrade description for each level where there is a change.",
        _ => ""
    };
}