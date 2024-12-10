using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;

namespace AbilityChoice.Displays;

public class ElectricShock : ModBloonOverlay
{
    public override string BaseOverlay => "LaserShock";

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var renderer in node.genericRenderers)
        {
            if (renderer.Is<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.color = Color.yellow;
            }
            else if (renderer.Is<MeshRenderer>(out var meshRenderer))
            {
                meshRenderer.SetMainTexture(GetTexture(nameof(ElectricShock)));
            }
        }
    }
}