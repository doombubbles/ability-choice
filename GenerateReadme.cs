#if DEBUG
using System.IO;
using HarmonyLib;

namespace AbilityChoice;

internal static class GenerateReadme
{
    private const string ReadMe = "README.md";

    private static readonly string[] TowerSets = { "Primary", "Military", "Magic", "Support" };

    private static string FilePath =>
        Path.Combine(ModHelper.ModSourcesDirectory, nameof(TowerAbilityChoice), "AbilityChoices");

    public static void Generate()
    {
        var total = "# All Ability Choices";

        foreach (var towerSet in TowerSets)
        {
            var text = GenerateCategory(towerSet);
            SaveMd(text, Path.Combine(towerSet, ReadMe));
            total += text;
        }

        SaveMd(total, ReadMe);
    }

    private static void SaveMd(string text, string path) => File.WriteAllText(Path.Combine(FilePath, path), text);

    private static string GenerateCategory(string category) =>
        $@"""
        <h2>{category}</h2>

        <table>{ModContent.GetContent<TowerAbilityChoice>().Where(abilityChoice => abilityChoice.GetType().Namespace!.Contains(category)).Select(GenerateEntry).Join(delimiter: "")}
        </table>
        """;

    private static string GenerateEntry(TowerAbilityChoice abilityChoice) =>
        $@"""
            <tr>
                <td align='center'>
                    <h2>{abilityChoice.AbilityName}</h2>
                </td>
                <td>
                    {abilityChoice.Description1}
                </td>{(string.IsNullOrWhiteSpace(abilityChoice.Description2) ? "" : $@"
                <td>
                    {abilityChoice.Description2}
                </td>")}
            </tr>
        """;
}

#endif