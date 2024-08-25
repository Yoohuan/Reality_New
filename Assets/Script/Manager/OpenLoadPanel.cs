using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLoadPanel : MonoBehaviour
{
    public GameObject panel;
    private bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void panelControl()
    {
        panel.SetActive(!isOpen);
        isOpen = !isOpen;
    }
}
