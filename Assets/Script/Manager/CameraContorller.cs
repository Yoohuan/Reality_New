using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraContorller : MonoBehaviour
{
    public static CameraContorller Instance;
    [SerializeField]
    private Camera view_cam;        //相机对象
    [SerializeField]
    private float cam_lerp_rate = 0.2f;

    public float s = 0.001f;        // 鼠标移动速度的缩放因子

    //[SerializeField]
    //private GameObject tile_list;

    private float target_orthographicSize = 5f;

    //Vector3 _local_pos_tl;
    //Vector3 _local_scale_tl;


    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        //_local_pos_tl = tile_list.transform.localPosition;
        //_local_scale_tl = tile_list.transform.localScale;
    }

    Vector3 last = Vector3.zero;    // 上一帧鼠标位置
    float mouseSpeed = 10;          // 鼠标移动速度


    Vector3 pos_tmp = new Vector3(1, 1, 0);
    void Update()
    {
        // 使用滚轮控制相机的缩放
       

        if (LevelManager.Instance.IsPlaying) // play模式
        {

        }
        else                                 // 编辑模式
        {
            
            target_orthographicSize = Mathf.Clamp(target_orthographicSize - Input.mouseScrollDelta.y * 0.5f, 3, 7);
            view_cam.orthographicSize = Mathf.Lerp(view_cam.orthographicSize, target_orthographicSize, 0.12f);
            DragCamera();
        }

        //HandleUI();
    }

    IEnumerator _LerpCam2Zero()
    {
        while(true)
        {
            pos_tmp = Vector3.Lerp(view_cam.transform.position, Vector3.zero, cam_lerp_rate * 2);
            pos_tmp.z = -10;
            view_cam.transform.position = pos_tmp;
            if (((Vector2)pos_tmp).magnitude <= 0.2f) break;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    Coroutine lerp2zero = null;
    public void LerpCam2Zero()
    {
        if(lerp2zero != null)
        {
            StopCoroutine(lerp2zero);
            lerp2zero = null;
        }
        StartCoroutine(_LerpCam2Zero());
    }

    private void FixedUpdate()
    {
        if (LevelManager.Instance.IsPlaying) // play模式
        {
            view_cam.orthographicSize = Mathf.Lerp(view_cam.orthographicSize, 4.3f, cam_lerp_rate*2);
            if (ScriptManager.Instance.player_instance)
            {
                pos_tmp = Vector3.Lerp(view_cam.transform.position, ScriptManager.Instance.player_instance.transform.position, cam_lerp_rate);
                pos_tmp.z = -10;
                view_cam.transform.position = pos_tmp;
            }
        }
    }

    void DragCamera()
    {
        if (Input.GetMouseButtonDown(1)) last = Input.mousePosition;

        // 当按住鼠标右键时，根据鼠标移动的距离来移动相机位置
        if (Input.GetMouseButton(1))
        {
            Vector3 deltaMousePosition = Input.mousePosition - last;
            deltaMousePosition.Scale(view_cam.cameraToWorldMatrix.lossyScale * (view_cam.orthographicSize * Mathf.Sqrt(s)));

            deltaMousePosition.Scale(new Vector3(1, -1, 1));
            view_cam.transform.position += deltaMousePosition;
        }

        last = Input.mousePosition; // 更新鼠标位置
    }

    float _s = 1;
    void HandleUI() //缩放时处理UI
    {
        _s = (1 + (view_cam.orthographicSize - 5)/5f);

        //tile_list.transform.localPosition = _local_pos_tl * _s;
        //tile_list.transform.localScale = _local_scale_tl * _s;
    }



}
