onGrund = false     --是否在地面上
-- public codeInput = "" -- 公共变量用于存储玩家输入的代码

function Awake()

end

function Start()

end

function Update()

    v = self:GetVelocity()      -- 获取角色当前的速度
    input = self:GetInput()     -- 获取玩家的输入
    
    --code = InputField.text -- 获取代码玩家输入

    
    if Input.GetKeyDown(KeyCode.Space) and onGrund then 
        -- 如果玩家按下空格键并且角色在地面上
        -- TODO: 在这里填写不能无限踏空跳的代码                       2分
        onGrund = false
        v.y = 12                        -- 将角色的垂直速度设置为12，跳跃
        -- TODO: 在这里填写二段跳的代码
        -- DoubleJump(code);
        -- TODO: 在这里填写跳跃缓存的代码
    end

    self:SetVelocity(input.x * 5, v.y)  -- 根据玩家输入设置角色的水平速度

    if Input.GetKeyDown(KeyCode.LeftShift) then
        -- 获取玩家填写的代码字符串
        local code = "-- 冲刺（加速跑）/闪现（瞬移或短暂快速冲刺，不允许卡墙）/弹射（根据鼠标方位起跳，下落前玩家无法控制方向）"
        -- TODO: 在这里调用SpecialAbility函数，并传递玩家填写的代码字符串
        SpecialAbility(code)
    end
end

function FixedUpdate()

end

function OnDestroy()
    
end

function OnHitGround()
    onGrund = true
end

function DoubleJump(code)
    -- TODO: 3. 二段跳功能的代码逻辑                    2分
    load(code)() -- 将字符串代码加载并执行
end

function SpecialAbility(code)
    -- TODO: 5. 冲刺（加速跑）/闪现（瞬移或短暂快速冲刺，不允许卡墙）/弹射（根据鼠标方位起跳，下落前玩家无法控制方向）    4分
    load(code)() -- 将字符串代码加载并执行
end

function JumpHeight(code)
    -- TODO: 6. 大小跳（根据按键时长影响跳跃高度，仅设置大跳/小跳两档）       4分
    load(code)() -- 将字符串代码加载并执行
end


