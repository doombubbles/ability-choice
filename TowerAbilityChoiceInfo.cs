using System;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.UI;

namespace AbilityChoice;

[RegisterTypeInIl2Cpp(false)]
public class TowerAbilityChoiceInfo : MonoBehaviour
{
    public UpgradeDetails upgradeDetails;

    public TowerAbilityChoice abilityChoice;

    /// <inheritdoc />
    public TowerAbilityChoiceInfo(IntPtr ptr) : base(ptr)
    {
    }

    public void UpdateIcon()
    {
        var abilityObject = upgradeDetails.abilityObject;
        var circle = abilityObject.GetComponent<Image>();

        if (abilityChoice != null)
        {
            abilityObject.SetActive(true);
            circle.SetSprite(TowerAbilityChoice.IconForMode(abilityChoice.Mode));
        }
        else
        {
            circle.SetSprite(TowerAbilityChoice.IconForMode(0));
        }
    }

    public static void Setup(Il2CppReferenceArray<UpgradeDetails> pathUpgrades)
    {
        foreach (var upgradeDetails in pathUpgrades)
        {
            if (!upgradeDetails.isActiveAndEnabled) return;
            
            var info = upgradeDetails.GetButton().gameObject.GetComponent<TowerAbilityChoiceInfo>();

            if (info != null)
            {
                info.abilityChoice = null;
            }

            var name = upgradeDetails.upgrade?.name ?? "";

            if (TowerAbilityChoice.Cache.TryGetValue(name, out var abilityChoice))
            {
                if (info == null)
                {
                    info = upgradeDetails.GetButton().gameObject.AddComponent<TowerAbilityChoiceInfo>();
                }

                info.abilityChoice = abilityChoice;
                info.upgradeDetails = upgradeDetails;
            }

            if (info != null)
            {
                info.UpdateIcon();
            }
        }
    }
}