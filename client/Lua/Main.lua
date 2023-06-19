--------------------------------------------------------------------------
--
-- 文件：Lua\Main.lua
-- 作者：Xoen Xie
-- 时间：2023/06/19
-- 描述：lua入口
-- 说明：
--
--------------------------------------------------------------------------

local pb = require "pb"

-- lua main 函数
local function main()
    xsf_log("lua main start ...")
    
end


function OnUpdate()
    xsf_log("Main.OnUpdate", CS.UnityEngine.Time.deltaTime)
end


main()
