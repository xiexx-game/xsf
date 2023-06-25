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
local pb = require "pb"

-- lua main 函数
local function main()
    xsf_log("lua main start ...")
    
    local UI = require "UI.UI"
    UI:Init()
    
end


function Start()
    local UI = require "UI.UI"
    UI:Show("UITest")
end


function OnUpdate()

end


function OnProtoLoaded(name, data)
    xsf_log("OnProtoLoaded name:", name)

    local ret, info = pb.load(data)
    if not ret then
        xsf_error("pb load error:" .. info)
    end
end


main()
