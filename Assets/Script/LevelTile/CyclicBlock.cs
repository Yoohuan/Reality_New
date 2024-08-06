using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclicBlock : TileBase
{
    [SerializeField] GameObject highlight;
    [SerializeField] Collider2D edit_hited;
    [SerializeField] Collider2D playing_collider;
    [SerializeField] Animator animator;


    void Start()
    {
       
    }

    void Update()
    {

    }



    public override void SetHighLight(bool b)
    {
        highlight.SetActive(b);
    }
}
