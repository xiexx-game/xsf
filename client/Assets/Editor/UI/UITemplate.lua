--------------------------------------------------------------------------
--
-- 文件：Lua-src\UI\View\_UI_NAME_.lua
-- 作者：Xoen Xie
-- 时间：_LUA_DATE_
-- 描述：
-- 说明：
--
--------------------------------------------------------------------------
local UIBase = require "UI.UIBase"

local _UI_NAME_ = 
{
    Name = "_UI_NAME_",
}

setmetatable(_UI_NAME_, UIBase)
_UI_NAME_.__super = getmetatable(_UI_NAME_)

function _UI_NAME_:OnInit()
    --UI_INIT_BEGIN

    --UI_INIT_END
end


--UI_FUNC_REPLACE
--不要删除以上注释，否则自动化代码无法生成

return _UI_NAME_