using BTD_Mod_Helper.Api.Display;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Unity.Display;

namespace AbilityChoice.Displays;

public class ZomgTrojan : ModDisplay
{
    public override string BaseDisplay => "0e3f0a7aad0778f41b70496a96589153";

    public override float Scale => 1.2f;

    public override void Register()
    {
        base.Register();
        
        // TODO re-enable after MelonLoader bug fix
        return;
        
        GameData.Instance.bloonOverlays.overlayTypes["BenjaminTrojan"].assets[BloonOverlayClass.Zomg] =
            CreatePrefabReference<ZomgTrojan>();
    }

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var renderer in node.genericRenderers)
        {
            renderer.sortingLayerName = "ZomgMoab";
        }
    }
}