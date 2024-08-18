using System.Collections.Generic;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;

namespace AbilityChoice;

public abstract class HeroAbilityChoice : AbilityChoice
{
    public static readonly Dictionary<string, AbilityChoice> Cache = new();

    public abstract string HeroId { get; }

    public abstract Dictionary<int, string> Descriptions1 { get; }

    public virtual Dictionary<int, string> Descriptions2 { get; }

    protected override bool HasMode2 => Descriptions2 != null;

    public override IEnumerable<ModContent> Load()
    {
        yield return this;

        foreach (var level in Descriptions1.Keys)
        {
            yield return new HeroAbilityChoiceDescription(this, level, 1, $"[{Id} Description1 {level}]");
        }

        if (HasMode2)
        {
            foreach (var level in Descriptions2.Keys)
            {
                yield return new HeroAbilityChoiceDescription(this, level, 2, $"[{Id} Description2 {level}]");
            }
        }
    }

    public override void Register()
    {
        base.Register();
        Cache[Id] = this;
    }

    public override IEnumerable<TowerModel> GetAffected(GameModel gameModel) =>
        gameModel.GetTowersWithBaseId(HeroId)
            .Where(AppliesTo)
            .OrderBy(model => model.tier);

    public static string IconForMode(int mode) => mode switch
    {
        1 => VanillaSprites.RedBtnSquare,
        2 => VanillaSprites.BlueBtnSquare,
        _ => VanillaSprites.YellowBtnSquare
    };

    public override void RegisterText(Il2CppSystem.Collections.Generic.Dictionary<string, string> textTable)
    {
        textTable[Id] = DisplayName;

        foreach (var (level, description) in Descriptions1)
        {
            textTable[$"{Id} Description1 {level}"] = description;
        }

        if (HasMode2)
        {
            foreach (var (level, description) in Descriptions2)
            {
                textTable[$"{Id} Description2 {level}"] = description;
            }
        }
    }
}