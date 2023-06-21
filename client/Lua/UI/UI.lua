--------------------------------------------------------------------------
--
-- 文件：UI\UI.lua
-- 作者：Xoen Xie
-- 时间：2023/06/21
-- 描述：UI模块
-- 说明：
--
--------------------------------------------------------------------------

local UI = {
    RootG = nil,
    RootT = nil,

    UpdateID = XSF.UpdateID.UI,
    m_UpdateList = {},
}

function UI:Init()
    -- GameObject of UIRoot
    self.RootG = CS.LuaUtil.UIRoot

    -- 注意，这个root不是根节点UIRoot，而是UIRoot/Root
    self.RootT = self.RootG.transform:Find("Root")

    local XSFUpdate = require "Base.XSFUpdate"
    XSFUpdate:Add(self)
end

function UI:AddUI( ui )
    self.m_UpdateList[ui.Name] = ui
end

function UI:DelUI( ui )
    self.m_UpdateList[ui.Name] = nil
end

function UI:IsUpdateWorking()
    return true
end

function UI:OnUpdate()
    for _, ui in pairs(self.m_UpdateList) do
        ui:Update()
    end
end

function UI:GetByName( name )
    return require("UI.View." .. name)
end

function UI:Show(name)
    local ui = self:GetByName(name)
    ui:Show()

    return ui
end


return UI