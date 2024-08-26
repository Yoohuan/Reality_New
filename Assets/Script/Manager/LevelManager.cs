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
    public GameObject currentSelectedModule; // �洢��ǰѡ�е�ģ��
    public GameObject deleteButton;
    public Button deleteBtn;                 //ɾ����ť
    public Button hiddenBtn;                 //����ģʽ��ť
    public int limitOne;                     //����һ��������
    public int limitTwo;                     //���Ͷ���������

    public Transform backgroundSquare;       //�������飨��ȡ��С��
    public static LevelManager Instance;
    public Transform startPoint;            // ���λ��
    public Transform endPoint;              // �յ�λ��

    public TileBase[] registries;         // �ؿ�ע���
    public List<TileBase> tiles;            // ��Ƭ

    public int GetIdInRegistry(string name)
    {
        for(int i = 0; i < registries.Length; i++)
        {
            if (registries[i].tileName == name) return i;
        }
        return -1;
    }


    float grid_space = 1f;                  // ������

    [SerializeField]
    private GameObject tool_move;           // �ƶ�����


    [SerializeField]
    private OptMode mode = OptMode.Select;  // ��ǰ����ģʽ
    public OptMode CurrMode { get { return mode; } }
    Vector3 m_last;                         // ��һ֡������������


    public enum OptMode    // ����ģʽ��ö��
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
    }
    enum Dir        // ����ö��
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
        }
        else
        {
            hiddenBtn.interactable = true;
        }
        Vector3 m_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // �����λ��ת��Ϊ��������ϵ
        Vector3 mouse_delta = m_world - m_last;                                 // ����ƶ�����������
        if (Input.GetMouseButtonDown(0))
        {
            drug_start_pos = m_world;
            //����Ƿ������˱༭������
            RaycastHit2D cast_tool = Physics2D.Raycast(m_world, Vector2.zero, 0f, LayerMask.GetMask("Editor"));
            
            if (cast_tool.collider)                                             // �������˱༭������
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
            else                                                                // ���û��
            {
                m_dir = Dir.None;
                //����Ƿ������˹ؿ�����
                
            }

            if (mode == OptMode.Put)// ����tile
            {
                print("put");
                if (CheckInRange(m_world)) //������Χ������
                {
                    TileBase t = UIManager.Instance.selectedModule;
                    if (t && canPut(t))
                    {
                        print("put2");
                        GameObject obj = GameObject.Instantiate(t.gameObject);
                        obj.transform.position = new Vector3(m_world.x, m_world.y, 0);
                        t = obj.GetComponent<TileBase>();
                        tiles.Add(t);
                    }
                }
            }

            if (mode == OptMode.HiddenPut)// ����tile
            {
                print("put");
                if (CheckInRange(m_world)) //������Χ������
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
            select_dirty = true;                                                // ѡ�������
        }

        if (select_dirty)
        {
            select_dirty = false;                                               // ���ñ�����
            if (selected.Count > 0) UIManager.Instance.DrawGrid();
            else UIManager.Instance.CloseGrid();
            calSelectedCenter();                                                // ����ѡ����������ĵ�
        }
        

        if (Input.GetMouseButton(0))                                            // ������һֱ����ʱ
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
                select_center += mouse_delta;// ����ѡ����������ĵ�λ��
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

        tool_move.SetActive(selected.Count > 0);                                //�����ƶ�����

        tool_move.transform.position = select_center;
        m_last = m_world;
    }



    Vector3 select_center = Vector3.zero;

    public bool CheckInRange(Vector3 pos)
    {
        return Mathf.Abs(pos.x) <= (backgroundSquare.localScale.x / 2 - 0.5) && pos.y >= -1.5f && pos.y <= (backgroundSquare.localScale.y / 2 - 0.5);
    }

    void calSelectedCenter()                                        // ����ѡ����������ĵ�λ��
    {
        select_center = Vector3.zero;
        foreach (TileBase t in selected)
        {
            select_center += t.transform.localPosition / selected.Count;
        }
    }

    private bool select_dirty = false;                              // �Ƿ�ѡ��

    public HashSet<TileBase> selected = new HashSet<TileBase>();    // ѡ������弯��
    void SelectObject(RaycastHit2D cast)                            // ѡ������
    {
        TileBase tile = null;
        if (cast.collider)                                          // ������������
        {
            tile = cast.collider.GetComponent<TileBase>();          // ��ȡ�����TileBase���
            if (tile.isLock)
            {
                return;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))                        //��ѡ
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
        else                                                        //��ѡ
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

        select_dirty = true;                                        //ѡ��״̬
    }

    bool canPut(TileBase selectedModule)
    {
        int countOne = 0;
        int countTwo = 0;
        foreach (TileBase t in tiles)
        {
            if (!t.isLock && !t.isHidden && (GetIdInRegistry(t.tileName) == 0 || GetIdInRegistry(t.tileName) == 2))
            { countOne++; }
            else if (!t.isLock && !t.isHidden && (GetIdInRegistry(t.tileName) == 1 || GetIdInRegistry(t.tileName) == 3))
            { countTwo++; }
        }
        if ((GetIdInRegistry(selectedModule.tileName) == 0 || GetIdInRegistry(selectedModule.tileName) == 2))
        { countOne++; }
        else if ((GetIdInRegistry(selectedModule.tileName) == 1 || GetIdInRegistry(selectedModule.tileName) == 3))
        { countTwo++; }
        if ((countOne <= limitOne) && (countTwo <= limitTwo))
            return true;
        return false;
    }

    void unHighLightAllSelect()                                     // ȡ����������ĸ�����ʾ
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

            if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
            { mode = OptMode.Start; }
            else
            { mode = OptMode.HiddenStart; }
        }
        else
        {
            ScriptManager.Instance.DestroyPlayer();
            CameraContorller.Instance.LerpCam2Zero();

            UIManager.Instance.DrawGrid();
            UIManager.Instance.OnTilemapEditorClick();

            if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
            { mode = OptMode.Select; }
            else
            { mode = OptMode.HiddenSelect; }
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

        MsgBox.Instance.PushMsg(sucess ? "ͨ��":"ʧ��", 0.7f);
        foreach (TileBase t in tiles)
        {
            t.OnEnd(sucess);
        }
    }

    public void SetHiddenMode()
    {
        if (mode == OptMode.Select || mode == OptMode.Put || mode == OptMode.Start)
        { 
            mode = OptMode.HiddenSelect; 
            foreach (TileBase t in tiles)
            {
                t.gameObject.SetActive(true);
            }
        }
        else
        { 
            mode = OptMode.Select;
            foreach (TileBase t in tiles)
            {
                if(t.isHidden)
                {
                    t.gameObject.SetActive(false);
                }
            }
        }
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
        // ���ص�ͼ��λ����Ϣ
        return new Vector3(0, 0, 0); // �����ͼ��λ��Ϊ(0, 0, 0)
    }

    public Vector2Int GetGridSize()
    {
        // ���ص�ͼ�Ĵ�С��Ϣ
        return new Vector2Int((int)backgroundSquare.localScale.x, (int)backgroundSquare.localScale.y); // �����ͼ�Ĵ�СΪ15x10
    }

    public float GetGridCellSize()
    {
        // ��������Ԫ�Ĵ�С
        return 1f; // ����ÿ������Ԫ�Ĵ�СΪ1
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
            TileBase tile = registries[id];  // Դ
            tile = Instantiate(tile);
            tile.transform.position = pos;
            tile.isLock = isLock;
            tile.isHidden = isHidden;
            Debug.Log(tile.transform);

            if(isHidden)
            {
                tile.gameObject.SetActive(false);
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

        limitOne = (int)obj.GetValue("LimitOne");
        limitTwo = (int)obj.GetValue("LimitTwo");
        float x = (float)obj.GetValue("x");
        float y = (float)obj.GetValue("y");
        float z = 1;
        Vector3 scale = new Vector3(x, y, z);
        backgroundSquare.localScale = scale;

        Vector3 startPointPos = new Vector3(-x / 2 - 1, -0.25f, 0);
        Vector3 endPointPos = new Vector3(x / 2 + 1, -0.25f, 0);
        startPoint.parent.position = startPointPos;
        endPoint.parent.position = endPointPos;
    }
}
