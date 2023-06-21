--------------------------------------------------------------------------
--
-- 文件：Base\XSFUpdate.lua
-- 作者：Xoen Xie
-- 时间：2023/06/21
-- 描述：Lua逐帧更新模块
-- 说明：
--
--------------------------------------------------------------------------

local UpdateID = {
    None = 0,

    UI = 1,
}

XSF.UpdateID = UpdateID

local XSFUpdate = {
    m_UpdateList = {}
}

function XSFUpdate:Add(node)
    if node.UpdateID == nil then
        xsf_error("XSFUpdate:Add can not add update node, UpdateID is nil")
        return
    end

    if type(node.IsUpdateWorking) ~= "function" then
        xsf_error("XSFUpdate:Add can not add update node, function IsUpdateWorking invalid")
        return
    end

    if type(node.OnUpdate) ~= "function" then
        xsf_error("XSFUpdate:Add can not add update node, function OnUpdate invalid")
        return
    end

    if self.m_UpdateList[node.UpdateID] ~= nil then
        xsf_error("XSFUpdate:Add can not add update node, node.UpdateID Exist", node.UpdateID)
        return
    end

    self.m_UpdateList[node.UpdateID] = node
    xsf_log("UpdateModule:OnUpdate id:", node.UpdateID)
end


function XSFUpdate:OnUpdate()
    for UpdateID, node in pairs(self.m_UpdateList) do
        if node:IsUpdateWorking() then
            node:OnUpdate()
        else
            self.m_UpdateList[UpdateID] = nil
        end
    end
end


return XSFUpdate