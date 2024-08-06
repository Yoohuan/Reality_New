using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : TileBase
{
    [SerializeField]
    private GameObject highlight;

    [SerializeField] Collider2D edit_hited;
    [SerializeField] Collider2D playing_collider;
    [SerializeField] Animator animator;

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
        playing_collider.enabled = true;
        animator.SetBool("playing", true);
    }

    public override void OnEnd(bool sucess)
    {
        edit_hited.enabled = true;
        playing_collider.enabled = false;
        animator.SetBool("playing", false);
    }
}
