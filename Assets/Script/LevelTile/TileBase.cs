using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBase : MonoBehaviour
{
    public string tileName;        // 地图块的名称
    public string detail;

    private bool isHighlighted;    // 地图块是否被高亮显示

    public Sprite imgOnGui;        //放到地图UI里面


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetHighLight(bool b)
    {
        this.isHighlighted = b;

        // 根据需要设置地图块的高亮显示效果
        if (isHighlighted)
        {
            // 设置高亮的颜色、材质等等
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            // 恢复正常的颜色、材质等等
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public virtual void OnStart()
    {

    }

    public virtual void OnEnd(bool sucess)
    {

    }
}
