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
    private Button templateButton;
    [SerializeField]
    private Button[] template;

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
        templateButton.onClick.AddListener(SaveTemplate);
        template[0].onClick.AddListener(LoadTemplate0);
        template[1].onClick.AddListener(LoadTemplate1);
        template[2].onClick.AddListener(LoadTemplate2);
    }

    private void SaveTemplate()
    {
        if (input.text == "") return;

        DirectoryInfo path = Directory.CreateDirectory(Application.dataPath + "\\TemplateSaves");
        path = Directory.CreateDirectory(Application.dataPath + "\\TemplateSaves\\" + input.text);

        string code = CodeManager.GetString("PlayerScript\\player.lua");

        writeStr(path.ToString() + "\\code.lua", code);
        writeStr(path.ToString() + "\\map.json", LevelManager.Instance.SerializeLevel());
        writeStr(path.ToString() + "\\mapLimit.json", LevelManager.Instance.SerializeLevelLimit());
        MsgBox.Instance.PushMsg("“—±£¥Ê", 1.5f);
    }

    private void LoadTemplate0()
    {
        DirectoryInfo path = Directory.CreateDirectory(Application.dataPath + "\\TemplateSaves");
        if (Directory.Exists(Application.dataPath + "\\TemplateSaves\\" + "\\0\\"))
        {
            print("Load Level");
            path = Directory.CreateDirectory(Application.dataPath + "\\TemplateSaves\\" + "\\0\\");
            string code = getStr(path.ToString() + "\\code.lua");
            writeStr(Application.dataPath + "\\PlayerScript\\player.lua", code);
            LevelManager.Instance.UnserializeLevel(getStr(path.ToString() + "\\map.json"));
            LevelManager.Instance.UnserializeLevelLimit(getStr(path.ToString() + "\\mapLimit.json"));
            MsgBox.Instance.PushMsg("º”‘ÿ¥Êµµ", 1.5f);
        }
        else
        {
            MsgBox.Instance.PushMsg("¥Êµµ≤ª¥Ê‘⁄", 1.5f);
        }
    }

    private void LoadTemplate1()
    {
        DirectoryInfo path = Directory.CreateDirectory(Application.dataPath + "\\TemplateSaves");
        if (Directory.Exists(Application.dataPath + "\\TemplateSaves\\" + "\\1\\"))
        {
            print("Load Level");
            path = Directory.CreateDirectory(Application.dataPath + "\\TemplateSaves\\" + "\\1\\");
            string code = getStr(path.ToString() + "\\code.lua");
            writeStr(Application.dataPath + "\\PlayerScript\\player.lua", code);
            LevelManager.Instance.UnserializeLevel(getStr(path.ToString() + "\\map.json"));
            LevelManager.Instance.UnserializeLevelLimit(getStr(path.ToString() + "\\mapLimit.json"));
            MsgBox.Instance.PushMsg("º”‘ÿ¥Êµµ", 1.5f);
        }
        else
        {
            MsgBox.Instance.PushMsg("¥Êµµ≤ª¥Ê‘⁄", 1.5f);
        }
    }

    private void LoadTemplate2()
    {
        DirectoryInfo path = Directory.CreateDirectory(Application.dataPath + "\\TemplateSaves");
        if (Directory.Exists(Application.dataPath + "\\TemplateSaves\\" + "\\2\\"))
        {
            print("Load Level");
            path = Directory.CreateDirectory(Application.dataPath + "\\TemplateSaves\\" + "\\2\\");
            string code = getStr(path.ToString() + "\\code.lua");
            writeStr(Application.dataPath + "\\PlayerScript\\player.lua", code);
            LevelManager.Instance.UnserializeLevel(getStr(path.ToString() + "\\map.json"));
            LevelManager.Instance.UnserializeLevelLimit(getStr(path.ToString() + "\\mapLimit.json"));
            MsgBox.Instance.PushMsg("º”‘ÿ¥Êµµ", 1.5f);
        }
        else
        {
            MsgBox.Instance.PushMsg("¥Êµµ≤ª¥Ê‘⁄", 1.5f);
        }
    }

    private void Save()
    {
        if (input.text == "") return;

        DirectoryInfo path = Directory.CreateDirectory(Application.dataPath + "\\Saves");
        path = Directory.CreateDirectory(Application.dataPath + "\\Saves\\" + input.text);

        string code = CodeManager.GetString("PlayerScript\\player.lua");

        writeStr(path.ToString() + "\\code.lua", code);
        writeStr(path.ToString() + "\\map.json", LevelManager.Instance.SerializeLevel());
        writeStr(path.ToString() + "\\mapLimit.json", LevelManager.Instance.SerializeLevelLimit());
        MsgBox.Instance.PushMsg("“—±£¥Ê", 1.5f);
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
            LevelManager.Instance.UnserializeLevelLimit(getStr(path.ToString() + "\\mapLimit.json"));
            MsgBox.Instance.PushMsg("º”‘ÿ¥Êµµ", 1.5f);
        }
        else
        {
            MsgBox.Instance.PushMsg("¥Êµµ≤ª¥Ê‘⁄", 1.5f);
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
        if((LevelManager.Instance.CurrMode == LevelManager.OptMode.Start) || (LevelManager.Instance.CurrMode == LevelManager.OptMode.HiddenStart))
        {
            foreach (Button button in template)
            {
                button.interactable = false;
            }
            saveButton.interactable = false;
            loadButton.interactable = false;
        }
        else
        {
            foreach (Button button in template)
            {
                button.interactable = true;
            }
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
    }
}
