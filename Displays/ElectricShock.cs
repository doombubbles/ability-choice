using System.Collections.Generic;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Bloons;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Utils;
using Il2CppNinjaKiwi.Common;
using UnityEngine;

namespace AbilityChoice.Displays;

public class ElectricShock : ModDisplay
{
    /// <summary>
    /// What we're copying from, same as the "overlayType" field in behaviors
    /// </summary>
    private const string BaseOverlay = "LaserShock";

    public static readonly string OverlayType = "LaserShock" /*nameof(ElectricShock)*/;

    /// <summary>
    /// The overlay class that this is for
    /// </summary>
    protected readonly BloonOverlayClass overlayClass;

    /// <summary>
    /// Still need a 0 argument constructor
    /// </summary>
    public ElectricShock()
    {
    }

    /// <summary>
    /// Creating the instance for the specific overlay class
    /// </summary>
    public ElectricShock(BloonOverlayClass overlayClass)
    {
        this.overlayClass = overlayClass;
    }

    /// <summary>
    /// Quick getter for all overlays
    /// </summary>
    private static SerializableDictionary<string, BloonOverlayScriptable> OverlayTypes =>
        GameData.Instance.bloonOverlays.overlayTypes;


    /// <summary>
    /// Unique name for each overlay
    /// </summary>
    public override string Name => base.Name + "-" + overlayClass;

    /// <summary>
    /// Get the corresponding base overlay
    /// </summary>
    public override PrefabReference BaseDisplayReference => OverlayTypes[BaseOverlay].assets[overlayClass];

    /// <summary>
    /// Load an instance for each overlay class
    /// </summary>
    public override IEnumerable<ModContent> Load() /* => Enum.GetValues(typeof(BloonOverlayClass))
        .Cast<BloonOverlayClass>()
        .Select(bc => new ElectricShock(bc));*/ => Enumerable.Empty<ModContent>();

    /// <summary>
    /// Setup the BloonOverlayScriptable over the course of each instance registering
    /// </summary>
    public override void Register()
    {
        base.Register();
        BloonOverlayScriptable electricShock;
        // attempting to use TryGetValue here throws an error lol
        if (!OverlayTypes.ContainsKey(nameof(ElectricShock)))
        {
            electricShock = OverlayTypes[nameof(ElectricShock)] =
                ScriptableObject.CreateInstance<BloonOverlayScriptable>();
            // electricShock.assets = new SerializableDictionary<BloonOverlayClass, PrefabReference>();
            electricShock.displayLayer = OverlayTypes[BaseOverlay].displayLayer;
        }
        else
        {
            electricShock = OverlayTypes[nameof(ElectricShock)];
        }

        electricShock.assets[overlayClass] = CreatePrefabReference(Id);
    }

    /// <summary>
    /// Make the visuals yellow
    /// </summary>
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