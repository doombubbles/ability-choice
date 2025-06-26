using System;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;

namespace AbilityChoice.Displays;

public class ParagonBomber : ModDisplay
{
    public override string BaseDisplay => "6fb19edd78320c34caffdad31ce623cb";

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        var animator = node.GetComponentInChildren<Animator>();

        var obj = animator.gameObject;
        obj.transform.localPosition = new Vector3(0, 0, -25);

        obj.AddComponent<AnimationPauser>();
    }
}

[RegisterTypeInIl2Cpp(false)]
public class AnimationPauser : MonoBehaviour
{
    public float time = 0.2f;
    public string animation = "";

    public Animator animator;

    /// <inheritdoc />
    public AnimationPauser(IntPtr ptr) : base(ptr)
    {
    }

    private void Update()
    {
        animator ??= GetComponent<Animator>();

        animator.Play(animation, 0, time);
        animator.speed = 0;
    }
}