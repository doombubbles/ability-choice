﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Bloons;
using Assets.Scripts.Unity.Display;
using Assets.Scripts.Utils;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using NinjaKiwi.Common;
using UnityEngine;

namespace AbilityChoice.Displays;

public class ElectricShock : ModDisplay
{
    /// <summary>
    /// The overlay class that this is for
    /// </summary>
    protected readonly BloonOverlayClass overlayClass;

    /// <summary>
    /// Quick getter for all overlays
    /// </summary>
    private static SerializableDictionary<string, BloonOverlayScriptable> OverlayTypes =>
        GameData.Instance.bloonOverlays.overlayTypes;

    /// <summary>
    /// What we're copying from, same as the "overlayType" field in behaviors
    /// </summary>
    private const string BaseOverlay = "LaserShock";


    /// <summary>
    /// Unique name for each overlay
    /// </summary>
    public override string Name => base.Name + "-" + overlayClass;

    /// <summary>
    /// Get the corresponding base overlay
    /// </summary>
    public override PrefabReference BaseDisplayReference => OverlayTypes[BaseOverlay].assets[overlayClass];

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
    /// Load an instance for each overlay class
    /// </summary>
    public override IEnumerable<ModContent> Load() => Enum.GetValues(typeof(BloonOverlayClass))
        .Cast<BloonOverlayClass>()
        .Select(bc => new ElectricShock(bc));

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
            electricShock.assets = new SerializableDictionary<BloonOverlayClass, PrefabReference>();
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