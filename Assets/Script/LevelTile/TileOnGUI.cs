using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOnGUI : MonoBehaviour
{
    private int registryId = -1;   //注册表里面的Index



    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetId(int i)
    {
        registryId = i;
    }

    public TileBase GetTile()
    {
        if (registryId < LevelManager.Instance.registries.Length && registryId != -1)
            return LevelManager.Instance.registries[registryId];
        return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
