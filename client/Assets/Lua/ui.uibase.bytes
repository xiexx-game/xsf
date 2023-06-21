--------------------------------------------------------------------------
--
-- 文件：Lua\UI\UIBase.lua
-- 作者：Xoen Xie
-- 时间：2023/06/21
-- 描述：UI基类
-- 说明：
--
--------------------------------------------------------------------------
local LuaUtil = CS.LuaUtil
local AASUtil = CS.AASUtil
local UI = require "UI.UI"

local MAX_REFRESH = 5

local UIStatus = {
    None = 0,
    Loading = 1, -- 正在加载中
    BeforeShow = 2,
    Show = 3, -- 已显示
    Hide = 4, -- 隐藏，资源未销毁
    Close = 5 -- 已关闭
}

local UIBase = {
    -- 不可被重写的属性
    -- prop that can not be overridden
    RootT = nil,
    RootG = nil,
    Canvas = nil,
    m_Status = UIStatus.None,   -- 当前UI状态
    m_RefreshQueue = nil,       -- UI刷新队列

    -- 可被重写的属性
    -- prop that can be overridden
    Name = "UIBase",
    ShowEventObjID = nil,           -- 事件ID
    SortingLayerName = nil,     -- UI打开的层级
}

UIBase.__index = UIBase

-------------------------------------------------------------------------------
--
-- functions that can be overridden
--
-------------------------------------------------------------------------------
-- UI初始化函数
function UIBase:OnInit()
    xsf_log("UIBase:OnInit ", self.Name)
end

-- 在隐藏前，或关闭前调用Show
function UIBase:OnHide()
    xsf_log("UIBase:OnHide ", self.Name)
end

-- 在显示前调用
function UIBase:OnShow()
    xsf_log("UIBase:OnShow ", self.Name)
end

-- 在关闭前调用，会在OnHide后调用
function UIBase:OnClose()
    xsf_log("UIBase:OnClose ", self.Name)
end

-- 刷新回调函数
function UIBase:OnRefresh(id, data)
    xsf_log("UIBase:OnRefresh ", self.Name, id, data)
end

function UIBase:OnUpdate()
    
end


-------------------------------------------------------------------------------
--
-- functions that can not be overridden
--
-------------------------------------------------------------------------------
function UIBase:OnUILoadDone()
    local function f(go)
        xsf_log("UIBase:OnUILoadDone start name=", self.Name)

        -- UI状态不正确
        if not self:IsLoading() then
            xsf_error("UIBase:OnUILoadDone status error", self.m_Status)
            return true
        end

        self.RootG = go
        self.RootT = self.RootG.transform

        self.RootT:SetParent(UI.RootT, true)

        self.RootT.localRotation = Quaternion.identity
        self.RootT.localScale = Vector3.one
        self.RootT.localPosition = Vector3.zero

        local rt = self.RootT:GetComponent(typeof(CS.UnityEngine.RectTransform))
        rt.anchoredPosition = Vector2.zero
        rt.sizeDelta = Vector2.zero

        self.RootT:SetAsLastSibling()

        self.Canvas = self.RootG:GetComponent(typeof(CS.UnityEngine.Canvas))

        if self.SortingLayerName ~= nil then
            self.Canvas.overrideSorting = true
            self.Canvas.sortingLayerName = self.SortingLayerName
        end

        self:OnInit()
        self.m_Status = UIStatus.BeforeShow
        UI:AddUI(self)

        return false
    end

    return f
end

-- 显示UI
function UIBase:Show()
    xsf_log("UIBase:Show ", self.Name, self.m_Status)
    if self.m_Status == UIStatus.None or self.m_Status == UIStatus.Close then
        AASUtil.LoadUI(self.Name, self:OnUILoadDone())
        self.m_Status = UIStatus.Loading
    elseif self.m_Status == UIStatus.StartHide or self.m_Status == UIStatus.Hide then
        self.m_Status = UIStatus.BeforeShow
        UI:AddUI(self)

        self.RootG:SetActive(true)
        self.RootT:SetAsLastSibling()
    end
end

function UIBase:IsLoading()
    return self.m_Status == UIStatus.Loading
end

function UIBase:IsShow()
    return self.m_Status == UIStatus.Show
end

function UIBase:Fresh(id, data)
    if self.m_Status == UIStatus.None or self.m_Status == UIStatus.Hide or self.m_Status == UIStatus.Close then
        xsf_error("UIBase:Fresh can not fresh ui, please call Show() first ", self.Name)
        return
    end

    xsf_log("UIBase:Fresh ", self.Name, id)
    local rfData = {}
    rfData.id = id
    rfData.data = data
    if self.m_RefreshQueue == nil then
        self.m_RefreshQueue = {}
    end
    table.insert(self.m_RefreshQueue, rfData)
end

-- 隐藏UI，资源未释放
function UIBase:Hide()
    if self.m_Status == UIStatus.Hide or self.m_Status == UIStatus.Close then
        xsf_warn("UIBase:Hide UI do not need hide ", self.Name, self.m_Status)
        return
    end

    self:OnHide()

    xsf_log("UIBase:Hide ", self.Name)
    self.m_RefreshQueue = {}
    self.m_Status = UIStatus.Hide

    -- 隐藏UI
    self.RootG:SetActive(false)
end

-- 关闭UI，UI资源会被释放
function UIBase:Close()
    if self.m_Status == UIStatus.Hide or self.m_Status == UIStatus.Close then
        xsf_warn("UIBase:Close UI do not need hide ", self.Name, self.m_Status)
        return
    end

    self:OnHide()
    self:OnClose()

    UI:DelUI(self)

    xsf_log("UIBase:Close ", self.Name)
    self.m_RefreshQueue = {}
    self.m_Status = UIStatus.Close

    -- 释放UI资源
    AASUtil.ReleaseInstance(self.RootG)
    self.RootG = nil
    self.RootT = nil
    self.Canvas = nil
end

-- UI内部更新函数
function UIBase:Update()
    if self.m_Status == UIStatus.BeforeShow then
        self:OnShow()
        self.m_Status = UIStatus.Show

        if self.EventObjID ~= nil then
            -- 发送事件
            LuaUtil.FireEvent(XSF.EventID.UIShow, self.ShowEventObjID, nil, false)
        end
    elseif self.m_Status == UIStatus.Show then
        if self.m_RefreshQueue ~= nil then
            local count = #(self.m_RefreshQueue)
            if count > MAX_REFRESH then
                count = MAX_REFRESH
            end
            while (count > 0) do
                count = count - 1
                local rfData = self.m_RefreshQueue[1]
                self:OnRefresh(rfData.id, rfData.data)
                table.remove(self.m_RefreshQueue, 1)
            end
        end

        self:OnUpdate()
    end
end

return UIBase