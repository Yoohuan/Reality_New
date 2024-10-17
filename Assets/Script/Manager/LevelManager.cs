using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UIManager;

public class LevelManager : MonoBehaviour, IModuleSelection
{
    public GameObject currentSelectedModule; // 存储当前选中的模块
    public GameObject deleteButton;
    public GameObject endTile;
    public Button deleteBtn;                 //删除按钮
    public Button hiddenBtn;                 //附加模式按钮
    public Button unHiddenBtn;               //普通模式按钮
    public Button showHiddenBtn;             //显示附加物块
    public Text modeShow;
    public int limitOne;                     //类型一限制数量
    public int limitTwo;                     //类型二限制数量
    public Text limitOneText;
    public Text limitTwoText;

    public Transform backgroundSquare;       //背景方块（读取大小）
    public static LevelManager Instance;
    public Transform startPoint;            // 起点位置
    public Transform endPoint;              // 终点位置

    public TileBase[] registries;         // 地块注册表
    public List<TileBase> tiles;            // 瓦片

    public int GetIdInRegistry(string name)
    {
        for(int i = 0; i < registries.Length; i++)
        {
            if (registries[i].tileName == name) return i;
        }
        return -1;
    }


    float grid_space = 1f;                  // 网格间距

    [SerializeField]
    private GameObject tool_move;           // 移动工具


    [SerializeField]
    private OptMode mode = OptMode.Select;  // 当前操作模式
    public OptMode CurrMode { get { return mode; } }
    Vector3 m_last;                         // 上一帧鼠标的世界坐标


    public enum OptMode    // 操作模式的枚举
    {
        Select, Put, Start, HiddenSelect, HiddenPut, HiddenStart
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        deleteBtn.onClick.AddListener(() =>
        {
            Debug.Log("delete");
            foreach (TileBase t in selected)
            {
                tiles.Remove(t);
                GameObject.Destroy(t.gameObject);
            }
            selected.Clear();



            select_dirty = true;
        });
        hiddenBtn.onClick.AddListener(SetHiddenMode);
        unHiddenBtn.onClick.AddListener(SetUnHiddenMode);
    }
    enum Dir        // 方向枚举
    {
        None, XY, X, Y
    }



    Vector3 drug_start_pos = Vector3.zero;
    Vector3 drug_start_select_pos = Vector3.zero;
    Vector3 select_pos_last = Vector3.zero;
     Dir m_dir = Dir.None;
    // Update is called once per frame
    void Update()
    {
        if ((mode == OptMode.Start) || (mode == LevelManager.OptMode.HiddenStart))
        {
            hiddenBtn.interactable = false;
            unHiddenBtn.interactable = false;
            showHiddenBtn.interactable = true;
        }
        else
        {
            hiddenBtn.interactable = true;
            unHiddenBtn.interactable = true;
            showHiddenBtn.interactable = false;
        }
        Vector3 m_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // 将鼠标位置转换为世界坐标系
        Vector3 mouse_delta = m_world - m_last;                                 // 鼠标移动的增量向量
        if (Input.GetMouseButtonDown(0))
        {
            drug_start_pos = m_world;
            //检测是否点击到了编辑器工具
            RaycastHit2D cast_tool = Physics2D.Raycast(m_world, Vector2.zero, 0f, LayerMask.GetMask("Editor"));
            
            if (cast_tool.collider)                                             // 如果点击了编辑器工具
            {
                switch (cast_tool.collider.name)
                {
                    case "right":
                        m_dir = Dir.X;
                        break;
                    case "up":
                        m_dir = Dir.Y;
                        break;
                    case "center":
                        m_dir = Dir.XY;
                        break;
                    default:
                        break;
                }
            }
            else                                                                // 如果没有
            {
                m_dir = Dir.None;
                //检测是否点击到了关卡物体
                
            }

            if (mode == OptMode.Put)// 放置tile
            {
                if (CheckInRange(m_world)) //超出范围不给放
                {
                    TileBase t = UIManager.Instance.selectedModule;
                    if (t && canPut(t))
                    {
                        GameObject obj = GameObject.Instantiate(t.gameObject);
                        obj.transform.position = new Vector3(m_world.x, m_world.y, 0);
                        t = obj.GetComponent<TileBase>();
                        tiles.Add(t);
                    }
                }
            }

            if (mode == OptMode.HiddenPut)// 放置tile
            {
                print("put");
                if (CheckInRange(m_world)) //超出范围不给放
                {
                    TileBase t = UIManager.Instance.selectedModule;
                    if (t)
                    {
                        print("put2");
                        GameObject obj = GameObject.Instantiate(t.gameObject);
                        obj.transform.position = new Vector3(m_world.x, m_world.y, 0);
                        t = obj.GetComponent<TileBase>();
                        t.isHidden = true;
                        tiles.Add(t);
                        Color32 color = t.gameObject.GetComponentInChildren<SpriteRenderer>().color;
                        t.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 130);
                    }
                }
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (mode != OptMode.Start && mode != OptMode.HiddenStart)
            {
                RaycastHit2D cast_level = Physics2D.Raycast(m_world, Vector2.zero, 0f, LayerMask.GetMask("Level"));
                SelectObject(cast_level);
            }
            m_dir = Dir.None;
            select_dirty = true;                                                // 选择变更标记
        }

        if (select_dirty)
        {
            select_dirty = false;                                               // 重置变更标记
            if (selected.Count > 0) UIManager.Instance.DrawGrid();
            else UIManager.Instance.CloseGrid();
            calSelectedCenter();                                                // 计算选择物体的中心点
        }
        

        if (Input.GetMouseButton(0))                                            // 鼠标左键一直按下时
        {

            switch (m_dir)
            {
                case Dir.X:
                    mouse_delta.y = 0;
                    break;
                case Dir.Y:
                    mouse_delta.x = 0;
                    break;
                case Dir.XY:
                    break;
                default:
                    mouse_delta = Vector2.zero;
                    break;
            }

            bool inRange = true;
            foreach (TileBase t in selected)
            {
                inRange = CheckInRange(t.transform.position + mouse_delta);
                if (!inRange) break;
            }

            if (inRange)
            {
                foreach (TileBase t in selected)
                {
                    t.transform.position = t.transform.position + mouse_delta;
                }
                select_center += mouse_delta;// 更新选择物体的中心点位置
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            foreach (TileBase t in selected)
            {
                tiles.Remove(t);
                GameObject.Destroy(t.gameObject);
            }
            selected.Clear();

            

            select_dirty = true;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (TileBase t in selected)
            {
                t.isLock = !t.isLock;
            }
            unHighLightAllSelect();
            selected.Clear();
            select_dirty = true;
        }

        if (!start && Input.GetKeyDown(KeyCode.Escape))
        {
            if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
            { mode = OptMode.Select; }
            else
            { mode = OptMode.HiddenSelect; }
        }

        if (selected.Count == 0)
        {
            deleteButton.SetActive(false);
        }
        else
        {
            deleteButton.SetActive(true);
        }

        tool_move.SetActive(selected.Count > 0);                                //激活移动工具

        tool_move.transform.position = select_center;
        m_last = m_world;
        showMode();
        CountTile_Text();
    }



    Vector3 select_center = Vector3.zero;

    public bool CheckInRange(Vector3 pos)
    {
        return Mathf.Abs(pos.x) <= (backgroundSquare.localScale.x / 2 - 0.5) && pos.y >= -1.5f && pos.y <= (backgroundSquare.localScale.y / 2 - 0.5);
    }

    void calSelectedCenter()                                        // 计算选择物体的中心点位置
    {
        select_center = Vector3.zero;
        foreach (TileBase t in selected)
        {
            select_center += t.transform.localPosition / selected.Count;
        }
    }

    private bool select_dirty = false;                              // 是否选择

    public HashSet<TileBase> selected = new HashSet<TileBase>();    // 选择的物体集合
    void SelectObject(RaycastHit2D cast)                            // 选择物体
    {
        TileBase tile = null;
        if (cast.collider)                                          // 如果点击了物体
        {
            Debug.Log(cast.collider);
            tile = cast.collider.GetComponent<TileBase>();          // 获取物体的TileBase组件
            Debug.Log(tile);
            if (tile.isLock)
            {
                return;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))                        //多选
        {
            if (tile)
            {
                if (selected.Contains(tile))
                {
                    tile.SetHighLight(false);
                    selected.Remove(tile);
                }
                else
                {
                    tile.SetHighLight(true);
                    selected.Add(tile);
                }
                if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
                { mode = OptMode.Select; }
                else
                { mode = OptMode.HiddenSelect; }
            }
        }
        else                                                        //单选
        {
            unHighLightAllSelect();
            selected.Clear();
            if (tile)
            {
                tile.SetHighLight(true);
                selected.Add(tile);
                if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
                { mode = OptMode.Select; }
                else
                { mode = OptMode.HiddenSelect; }
            }
        }

        select_dirty = true;                                        //选择状态
    }

    bool canPut(TileBase selectedModule)
    {
        int countOne = 0;
        int countTwo = 0;
        foreach (TileBase t in tiles)
        {
            if (!t.isLock && !t.isHidden && (GetIdInRegistry(t.tileName) == 0 || GetIdInRegistry(t.tileName) == 1))
            { countOne++; }
            else if (!t.isLock && !t.isHidden && (GetIdInRegistry(t.tileName) == 2 || GetIdInRegistry(t.tileName) == 3))
            { countTwo++; }
        }
        if ((GetIdInRegistry(selectedModule.tileName) == 0 || GetIdInRegistry(selectedModule.tileName) == 1))
        { countOne++; }
        else if ((GetIdInRegistry(selectedModule.tileName) == 2 || GetIdInRegistry(selectedModule.tileName) == 3))
        { countTwo++; }
        if ((countOne <= limitOne) && (countTwo <= limitTwo))
            return true;
        return false;
    }

    void unHighLightAllSelect()                                     // 取消所有物体的高亮显示
    {
        foreach (TileBase t in selected)
        {
            t.SetHighLight(false);
        }
    }

    bool start = false;

    public bool IsPlaying { get { return start; } }
    public void Start_End()
    {
        //UIManager.Instance.panel.SetActive(false);
        start = !start;
        UIManager.Instance.SetStartButtonIcon(start);
        if (start)
        {
            unHighLightAllSelect();
            selected.Clear();
            ScriptManager.Instance.CreatePlayer();

            UIManager.Instance.CloseGrid();
            UIManager.Instance.ClosePanel();

            foreach (TileBase t in tiles)
            {
                if (t.isHidden) t.gameObject.SetActive(false); ;
            }
            mode = OptMode.Start;
        }
        else
        {
            ScriptManager.Instance.DestroyPlayer();
            CameraContorller.Instance.LerpCam2Zero();

            UIManager.Instance.DrawGrid();
            UIManager.Instance.OnTilemapEditorClick();

            foreach (TileBase t in tiles)
            {
                if (t.isHidden)
                {
                    t.gameObject.SetActive(true);
                    Color32 color = t.gameObject.GetComponentInChildren<SpriteRenderer>().color;
                    t.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 130);
                }
            }

            mode = OptMode.Select;
        }

        foreach(TileBase t in tiles)
        {
            if (start) t.OnStart();
            else t.OnEnd(false);
        }
    }

    public void End(bool sucess)
    {
        if (!start) return;
        start = false;
        UIManager.Instance.SetStartButtonIcon(false);

        ScriptManager.Instance.DestroyPlayer();
        CameraContorller.Instance.LerpCam2Zero();

        UIManager.Instance.DrawGrid();
        UIManager.Instance.OnTilemapEditorClick();

        if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
        { mode = OptMode.Select; }
        else
        { mode = OptMode.HiddenSelect; }

        MsgBox.Instance.PushMsg(sucess ? "通关":"失败", 0.7f);
        foreach (TileBase t in tiles)
        {
            t.OnEnd(sucess);
        }
    }

    public void SetHiddenMode()
    {
        mode = OptMode.HiddenSelect;
        foreach (TileBase t in tiles)
        {
            if (t.isHidden)
            {
                t.gameObject.SetActive(true);
                Color32 color = t.gameObject.GetComponentInChildren<SpriteRenderer>().color;
                t.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 130);
            }
        }

    }

    public void SetUnHiddenMode()
    {
        mode = OptMode.Select;
        //foreach (TileBase t in tiles)
        //{
        //    if (t.isHidden)
        //    {
        //        t.gameObject.SetActive(false);
        //    }
        //}

    }

    public void showHidden()
    {
        if (!start)
        {
            foreach (TileBase t in tiles)
            {
                if (t.isHidden)
                {
                    t.gameObject.SetActive(true);
                    Color32 color = t.gameObject.GetComponentInChildren<SpriteRenderer>().color;
                    t.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 130);
                }
            }
        }
        else
        {
            mode = OptMode.HiddenStart;
            foreach (TileBase t in tiles)
            {
                if (t.isHidden)
                {
                    t.gameObject.SetActive(true);
                    Color32 color = t.gameObject.GetComponentInChildren<SpriteRenderer>().color;
                    t.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 255);
                    t.OnStart();
                }
            }
        }
    }

    public void showMode()
    {
        if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
        { modeShow.text = "普通模式"; }
        else
        { modeShow.text = "附加模式"; }
    }

    public void SetSelectMode()
    {
        if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
        { mode = OptMode.Select; }
        else
        { mode = OptMode.HiddenSelect; }
    }

    public void SetPutMode()
    {
        if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
        { mode = OptMode.Put; }
        else
        { mode = OptMode.HiddenPut; }
    }

    public OptMode GetMode()
    {
        return mode;
    }
    public Vector3 GetGridPosition()
    {
        // 返回地图的位置信息
        return new Vector3(0, 0, 0); // 假设地图的位置为(0, 0, 0)
    }

    public Vector2Int GetGridSize()
    {
        // 返回地图的大小信息
        return new Vector2Int((int)backgroundSquare.localScale.x, (int)backgroundSquare.localScale.y); // 假设地图的大小为15x10
    }

    public float GetGridCellSize()
    {
        // 返回网格单元的大小
        return 1f; // 假设每个网格单元的大小为1
    }

    public GameObject GetCurrentSelectedModule()
    {
        return currentSelectedModule;
    }

    
    private JObject Tile2Json(TileBase tile)
    {
        JObject obj = new JObject();

        Vector3 pos = tile.transform.position;
        obj.Add("x", pos.x);
        obj.Add("y", pos.y);
        //obj.Add("z", pos.z);

        obj.Add("id", GetIdInRegistry(tile.tileName));
        obj.Add("isLock", tile.isLock);
        obj.Add("isHidden", tile.isHidden);

        return obj;
    }

    public string SerializeLevel()
    {
        JArray tiles = new JArray();

        foreach(TileBase t in this.tiles)
        {
            tiles.Add(Tile2Json(t));
        }

        return tiles.ToString();
    }

    public string SerializeLevelLimit() 
    {
        JObject obj = new JObject();

        obj.Add("LimitOne", limitOne);
        obj.Add("LimitTwo", limitTwo);
        obj.Add("x", backgroundSquare.localScale.x);
        obj.Add("y", backgroundSquare.localScale.y);
        obj.Add("cameraMaxSize", CameraContorller.Instance.maxSize);
        obj.Add("cameraMinSize", CameraContorller.Instance.minSize);
        

        JArray jArray = new JArray();
        jArray.Add(obj);
        return jArray.ToString();
    }


    private void Json2Tile(JObject obj)
    {
        //JObject obj = new JObject(str);

        Vector3 pos = new Vector3((float)obj.GetValue("x"), (float)obj.GetValue("y"), 0);


        int id = (int)obj.GetValue("id");
        bool isLock = (bool)obj.GetValue("isLock");
        bool isHidden = (bool)obj.GetValue("isHidden");


        if(id < 0 || id >= registries.Length)
        {
            throw new Exception("Unknown Tile: "+id);
        }
        else
        {
            Debug.Log("isput");
            TileBase tile = registries[id];  // 源
            tile = Instantiate(tile);
            tile.transform.position = pos;
            tile.isLock = isLock;
            tile.isHidden = isHidden;
            Debug.Log(tile.transform);

            if(isHidden)
            {
                Color32 color = tile.gameObject.GetComponentInChildren<SpriteRenderer>().color;
                tile.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color32(color.r, color.g, color.b, 130);
            }
            tiles.Add(tile);
        }
    }


    public void ClearMap()
    {
        foreach(TileBase t in tiles)
        {
            Destroy(t.gameObject, 0f);
        }
        tiles.Clear();
    }


    public void UnserializeLevel(string str)
    {
        ClearMap();
        JArray ts = (JArray)JsonConvert.DeserializeObject(str);

        

        for (int i = 0; i < ts.Count; i++)
        {
            Json2Tile((JObject)ts[i]);
        }
    }

    public void UnserializeLevelLimit(string str)
    {
        JArray ts = (JArray)JsonConvert.DeserializeObject(str);


        for (int i = 0; i < ts.Count; i++)
        {
            Json2Limit((JObject)ts[i]);
        }
    }

    private void Json2Limit(JObject obj)
    {
        //JObject obj = new JObject(str);

        CameraContorller.Instance.maxSize = (int)obj.GetValue("cameraMaxSize");
        CameraContorller.Instance.minSize = (int)obj.GetValue("cameraMinSize");
        limitOne = (int)obj.GetValue("LimitOne");
        limitTwo = (int)obj.GetValue("LimitTwo");
        limitOneText.text = "平台数量：     /  " + limitOne.ToString() + " ";
        limitTwoText.text = "障碍数量：     /  " + limitTwo.ToString() + " ";
        float x = (float)obj.GetValue("x");
        float y = (float)obj.GetValue("y");
        float z = 1;
        Vector3 scale = new Vector3(x, y, z);
        backgroundSquare.localScale = scale;

        Vector3 startPointPos = new Vector3(-x / 2 - 1, -0.25f, 0);
        Vector3 endPointPos = new Vector3(x / 2 + 1, -0.25f, 0);
        startPoint.parent.position = startPointPos;
        endPoint.parent.position = endPointPos;
        endTile.transform.position = endPointPos + new Vector3( 0, 1, 0);
    }

    private void CountTile_Text()
    {
        int countOne = 0;
        int countTwo = 0;
        foreach (TileBase t in tiles)
        {
            if (!t.isLock && !t.isHidden && (GetIdInRegistry(t.tileName) == 0 || GetIdInRegistry(t.tileName) == 1))
            { countOne++; }
            else if (!t.isLock && !t.isHidden && (GetIdInRegistry(t.tileName) == 2 || GetIdInRegistry(t.tileName) == 3))
            { countTwo++; }
        }
        limitOneText.text = "平台数量： " + countOne.ToString() + "  /  " + limitOne.ToString() + " ";
        limitTwoText.text = "障碍数量： " + countTwo.ToString() + "  /  " + limitTwo.ToString() + " ";
    }
}
