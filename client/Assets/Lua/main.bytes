--------------------------------------------------------------------------
--
-- 文件：Lua\Main.lua
-- 作者：Xoen Xie
-- 时间：2023/06/19
-- 描述：lua入口
-- 说明：
--
--------------------------------------------------------------------------
XSF = {}

require "Common.GlobalDef"
require "Common.Class"
local XSFUpdate = require "Base.XSFUpdate"

local IsShow = false
local time = 0

-- lua main 函数
local function main()
    xsf_log("lua main start ...")
    
    local UI = require "UI.UI"
    UI:Init()
    
end


function OnUpdate()
    XSFUpdate:OnUpdate()

    if not IsShow then
        time = time + CS.UnityEngine.Time.deltaTime
        if time > 3 then
            IsShow = true
            local UI = require "UI.UI"
            UI:Show("UITest")
        end
    end
end


main()
