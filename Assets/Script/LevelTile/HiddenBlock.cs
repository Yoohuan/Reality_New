using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenBlock : TileBase
{
    [SerializeField]
    private GameObject highlight;

    [SerializeField] Collider2D edit_hited;
    [SerializeField] Collider2D[] playing_colliders;
    [SerializeField] Animator animator;
    [SerializeField] EndField ef;

    void Update()
    {
    }

    public override void SetHighLight(bool b)
    {
        highlight.SetActive(b);
    }

    public override void OnStart()
    {
        print("Onstart");
        edit_hited.enabled = false;
        foreach (Collider2D c in playing_colliders)
        {
            c.enabled = true;
        }
        animator.SetBool("playing", true);
    }


    public override void OnEnd(bool sucess)
    {
        edit_hited.enabled = true;
        foreach(Collider2D c in playing_colliders)
        {
            c.enabled = false;
        }
        animator.SetBool("playing", false);
    }
}
