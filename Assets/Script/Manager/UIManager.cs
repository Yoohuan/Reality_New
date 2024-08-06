using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    public Button TilemapEditor;
    //public Button Select;
    //public Button Put;
    //public Button Back;
    //public Button Obstacle1;
    //public Button Obstacle2;
    //public Button Obstacle3;
    public TileBase selectedModule;


    public GameObject panel;        //选择地图块面板
    public LevelManager levelManager;

    [SerializeField]
    private Image start_end_button;
    [SerializeField]
    private Sprite start_sprite;
    [SerializeField]
    private Sprite end_sprite;

    private bool showGrid = false;

    [SerializeField]
    private Image imgShow;
    [SerializeField]
    private Text name_discript;

    [SerializeField]
    private LineRenderer lineRenderer;

    private void Awake()
    {
        Instance = this;
    }

    public void SetStartButtonIcon(bool start)
    {
         start_end_button.sprite = !start ? start_sprite : end_sprite;
    }


    private void Start()
    {
        levelManager = LevelManager.Instance;
        TilemapEditor.onClick.AddListener(OnTilemapEditorClick);
        //Select.onClick.AddListener(OnSelectClick);
        //Put.onClick.AddListener(OnPutClick);
        //Back.onClick.AddListener(OnBackClick);
        // 为每个按钮添加OnClick事件
        //Obstacle1.onClick.AddListener(() => OnObstacleClick(Obstacle1.gameObject));
        //Obstacle2.onClick.AddListener(() => OnObstacleClick(Obstacle2.gameObject));
        //Obstacle3.onClick.AddListener(() => OnObstacleClick(Obstacle3.gameObject));


        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;
        lineRenderer.startColor = Color.gray;
        lineRenderer.endColor = Color.gray;
    }

    private void Update()
    {
        if(LevelManager.Instance.CurrMode == LevelManager.OptMode.Put && selectedModule != null)
        {
            imgShow.sprite = selectedModule.imgOnGui;
            imgShow.color = Color.white;
            name_discript.text = selectedModule.detail;
        }
        else
        {
            imgShow.sprite = null;
            imgShow.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            name_discript.text = "None";
        }


    }
    public interface IModuleSelection
    {
        GameObject GetCurrentSelectedModule();
    }


    public void OnTilemapEditorClick()
    {
        panel.SetActive(true);

    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    private void OnSelectClick()
    {
        levelManager.SetSelectMode();

        // 设置按钮为激活状态
        //Obstacle1.gameObject.SetActive(true);
        //Obstacle2.gameObject.SetActive(true);
        //Obstacle3.gameObject.SetActive(true);
        //Back.gameObject.SetActive(true);

        // Interactable 属性设置为 false
        //Put.interactable = false;

        //Select.interactable = false;
    }

    public void OnPutClick()
    {
        levelManager.SetPutMode();
        showGrid = true;
        if (showGrid)
        {
            DrawGrid();
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
         ClosePanel();
         OnBackClick();
        //OnSelectClick();
        //Put.interactable = false;
    }

    private void OnBackClick()
    {

        //Obstacle1.gameObject.SetActive(false);
        //Obstacle2.gameObject.SetActive(false);
        //Obstacle3.gameObject.SetActive(false);
        //Back.gameObject.SetActive(false);


        //Put.interactable = true;

        //Select.interactable = true;
    }

    public void OnObstacleClick(TileBase obstacle)
    {
        //Put.interactable = true;
        // 设置当前选中的模块

        levelManager.SetPutMode();
        showGrid = true;
        if (showGrid)
        {
            DrawGrid();
        }
        else
        {
            lineRenderer.positionCount = 0;
        }

        selectedModule = obstacle;
        //levelManager.currentSelectedModule = selectedModule;
    }

    


    public void CloseGrid()
    {
        showGrid = false;
        lineRenderer.positionCount = 0;
    }

    public void DrawGrid()
    {
        if (lineRenderer.positionCount > 0) return; // 避免重复绘制
        // 获取网格的大小和位置信息
        Vector3 gridPosition = levelManager.GetGridPosition();
        Vector2Int gridSize = levelManager.GetGridSize();
        float gridCellSize = levelManager.GetGridCellSize();

        // 定义不可编辑的行数
        int editableRows = 3;

        //计算网格的偏移量，使中心点位于0,0
        float xOffset = -gridSize.x * gridCellSize / 2;
        float yOffset = -gridSize.y * gridCellSize / 2;

        // 绘制网格
        List<Vector3> gridLines = new List<Vector3>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                
                if (y >= editableRows)
                {
                    Vector3 cellPosition = gridPosition + new Vector3(x * gridCellSize + xOffset, y * gridCellSize + yOffset, 0);
                    // 添加每个网格单元的四条边
                    gridLines.Add(cellPosition);
                    gridLines.Add(cellPosition + new Vector3(gridCellSize, 0, 0));

                    gridLines.Add(cellPosition + new Vector3(gridCellSize, 0, 0));
                    gridLines.Add(cellPosition + new Vector3(gridCellSize, gridCellSize, 0));

                    gridLines.Add(cellPosition + new Vector3(gridCellSize, gridCellSize, 0));
                    gridLines.Add(cellPosition + new Vector3(0, gridCellSize, 0));

                    gridLines.Add(cellPosition + new Vector3(0, gridCellSize, 0));
                    gridLines.Add(cellPosition);

                    // 如果不是最后一列，则添加右侧的连线
                    if (x < gridSize.x - 1)
                    {
                        gridLines.Add(cellPosition + new Vector3(gridCellSize, 0, 0));
                        gridLines.Add(cellPosition + new Vector3(gridCellSize, gridCellSize, 0));
                    }
                }
            }
        }
        lineRenderer.positionCount = gridLines.Count;
        lineRenderer.SetPositions(gridLines.ToArray());
    }
}
