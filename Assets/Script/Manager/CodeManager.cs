using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CodeManager : MonoBehaviour
{
    public static CodeManager Instance;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string GetString(string filename)  //获取Lua文本
    {
        string s = "";
        try
        {
            StreamReader sr = new StreamReader(Application.dataPath + "\\" + filename);
            s = sr.ReadToEnd();
            sr.Close();
        }
        catch(Exception e)
        {

        }
        return s;
    }


}
