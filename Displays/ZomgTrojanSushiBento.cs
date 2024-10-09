using BTD_Mod_Helper.Api.Display;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Unity.Display;

namespace AbilityChoice.Displays;

public class ZomgTrojanSushiBento : ModDisplay
{
    public override string BaseDisplay => "708ed3d201495e847a0fba2b2b1e4aed";

    public override float Scale => 1.2f;

    public override void Register()
    {
        base.Register();
        
        // TODO re-enable after MelonLoader bug fix
        return;
        
        GameData.Instance.bloonOverlays.overlayTypes["BenjaminTrojanSushiBento"].assets[BloonOverlayClass.Zomg] =
            CreatePrefabReference<ZomgTrojanSushiBento>();
    }

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var renderer in node.genericRenderers)
        {
            renderer.sortingLayerName = "ZomgMoab";
        }
    }
}