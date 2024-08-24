function Awake()

end

function Start()

end

function Update()

    v = self:GetVelocity()      -- 获取角色当前的速度
    input = self:GetInput()     -- 获取玩家的输入
    
    --code = InputField.text -- 获取代码玩家输入

    if Input.GetKeyDown(KeyCode.Space) and self:IsOnGround() then 
        v.y = 12
    end

    self:SetVelocity(input.x * 5, v.y)  -- 根据玩家输入设置角色的水平速度

    if Input.GetKeyDown(KeyCode.LeftShift) then
        -- TODO
        -- 冲刺（加速跑）/闪现（瞬移或短暂快速冲刺，不允许卡墙）/弹射（根据鼠标方位起跳，下落前玩家无法控制方向
    end
end

function FixedUpdate()

end

function OnDestroy()
    
end

