using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SavesManager : MonoBehaviour
{
    public static SavesManager Instance;

    [SerializeField]
    private Button saveButton;
    [SerializeField]
    private Button loadButton;

    [SerializeField]
    private InputField input;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
    }

    private void Save()
    {
        if (input.text == "") return;

        DirectoryInfo path = Directory.CreateDirectory(Application.dataPath + "\\Saves");
        path = Directory.CreateDirectory(Application.dataPath + "\\Saves\\" + input.text);

        string code = CodeManager.GetString("PlayerScript\\player.lua");

        writeStr(path.ToString() + "\\code.lua", code);
        writeStr(path.ToString() + "\\map.json", LevelManager.Instance.SerializeLevel());
        MsgBox.Instance.PushMsg("已保存", 1.5f);
    }

    private void Load()
    {
        DirectoryInfo path = Directory.CreateDirectory(Application.dataPath + "\\Saves");
        if(Directory.Exists(Application.dataPath + "\\Saves\\" + input.text))
        {
            print("Load Level");
            path = Directory.CreateDirectory(Application.dataPath + "\\Saves\\" + input.text);
            string code = getStr(path.ToString() + "\\code.lua");
            writeStr(Application.dataPath + "\\PlayerScript\\player.lua", code);
            LevelManager.Instance.UnserializeLevel(getStr(path.ToString() + "\\map.json"));
            MsgBox.Instance.PushMsg("加载存档", 1.5f);
        }
        else
        {
            MsgBox.Instance.PushMsg("存档不存在", 1.5f);
        }
    }

    private string getStr(string filename)
    {
        string s = "";
        StreamReader sr = new StreamReader(filename);
        s = sr.ReadToEnd();
        sr.Close();
        return s;
    }

    private void writeStr(string filename, string str)
    {
        StreamWriter sr = new StreamWriter(filename);
        sr.Write(str);
        sr.Close();
    }


    // Update is called once per frame
    void Update()
    {
        if(LevelManager.Instance.CurrMode == LevelManager.OptMode.Start)
        {
            saveButton.interactable = false;
            loadButton.interactable = false;
        }
        else
        {
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
    }
}
