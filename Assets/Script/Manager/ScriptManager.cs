using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager Instance;
    [SerializeField]
    private GameObject player;

    public GameObject player_instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer(GameObject p)
    {
        player_instance = p;
    }

    public void CreatePlayer()
    {
        if (player) DestroyPlayer();
        player_instance = GameObject.Instantiate(player);
        player_instance.transform.position = LevelManager.Instance.startPoint.position;
    }

    public void DestroyPlayer()
    {
        GameObject.Destroy(player_instance, 0f);
    }
}
