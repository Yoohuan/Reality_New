using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using XLuaTest;

// 标记这个类可以被Lua调用
[LuaCallCSharp]
public class PlayerLuaScript : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] Collider2D ground_checker;

    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second

    private Action luaStart;
    private Action luaUpdate;
    private Action luaFUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;
    private Action luaOnHitGround;



    public static string luaDefault = @"onGrund = false

function Awake()

end

function Start()

end

function Update()

    v = self:GetVelocity()
    input = self:GetInput()

    if Input.GetKeyDown(KeyCode.Space) and onGrund then
        onGrund = false
        v.y = 12
    end

    self:SetVelocity(input.x * 5, v.y)
end

function FixedUpdate()

end

function OnDestroy()
    
end

function OnHitGround()
    onGrund = true
end";



    void Awake()
    {
        filter.useLayerMask = true;
        filter.SetLayerMask(LayerMask.GetMask("Level"));



        scriptEnv = luaEnv.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);

        //scriptEnv.Set()

        // 引入Unity相关的API
        string init_api = @"
UnityEngine = CS.UnityEngine
Vector3 = UnityEngine.Vector3
Vector2 = UnityEngine.Vector2
GameObject = UnityEngine.GameObject
Input = UnityEngine.Input
Time = UnityEngine.Time
KeyCode = UnityEngine.KeyCode
        ";

        luaEnv.DoString(init_api, "init_api", scriptEnv);
        luaEnv.DoString(CodeManager.GetString("PlayerScript\\player.lua"), "player_control", scriptEnv);
        //luaEnv.DoString(luaDefault, "player_control", scriptEnv);
        Action luaAwake = scriptEnv.Get<Action>("Awake");
        scriptEnv.Get("Start", out luaStart);
        scriptEnv.Get("Update", out luaUpdate);
        scriptEnv.Get("FixedUpdate", out luaFUpdate);
        scriptEnv.Get("OnDestroy", out luaOnDestroy);
        //scriptEnv.Get("OnHitGround", out luaOnHitGround);

        if (luaAwake != null)
        {
            luaAwake();

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ScriptManager.Instance.SetPlayer(this.gameObject);
        if (luaStart != null)
        {
            luaStart();
        }
        rigidbody.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (luaUpdate != null)
        {
            HandleInput();
            luaUpdate();
        }
    }

    // 在FixedUpdate方法中执行Lua脚本的FixedUpdate方法，并进行垃圾回收
    private void FixedUpdate()
    {
        if (luaFUpdate != null)
        {
            luaFUpdate();
        }
        if (Time.fixedTime - LuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaBehaviour.lastGCTime = Time.fixedTime;
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
    }

    public void SetVelocity(Vector2 v)
    {
        rigidbody.velocity = v;
    }

    Vector2 velocity_;
    public void SetVelocity(float x, float y)
    {
        velocity_ = rigidbody.velocity;
        velocity_.Set(x, y);
        rigidbody.velocity = velocity_;
    }

    public void SetVelocity(Vector3 v)
    {
        rigidbody.velocity = v;
    }

    public Vector2 GetVelocity()
    {
        return rigidbody.velocity;
    }

    public bool IsOnGround()
    {
        int count = ground_checker.Cast(Vector2.down, filter, rch2ds, 0f);
        return count > 0;
    }

    void HandleInput()
    {
        Input.GetKeyDown(KeyCode.Space);
        input.x = Input.GetAxisRaw("x");
        input.y = Input.GetAxisRaw("y");
    }

    Vector2 input = new Vector2();
    public Vector2 GetInput(bool isRaw)
    {
        return input;
    }

    RaycastHit2D[] rch2ds = new RaycastHit2D[6];
    ContactFilter2D filter = new ContactFilter2D();
}
