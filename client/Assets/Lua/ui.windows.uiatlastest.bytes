--------------------------------------------------------------------------
--
-- 文件：Lua-src\UI\View\UIAtlasTest.lua
-- 作者：Xoen Xie
-- 时间：2023/6/24
-- 描述：
-- 说明：
--
--------------------------------------------------------------------------
local UIBase = require "UI.UIBase"

local UIAtlasTest = 
{
    Name = "UIAtlasTest",
}

setmetatable(UIAtlasTest, UIBase)
UIAtlasTest.__super = getmetatable(UIAtlasTest)

function UIAtlasTest:OnInit()
    --UI_INIT_BEGIN
	-- Icon
	self.Icon = self.RootT:Find("Icon"):GetComponent(typeof(CS.UIImageSwitcher))
	-- Button1
	self.Button1 = self.RootT:Find("Button1").gameObject
	CS.UIUtil.SetClick( self.Button1, self:OnButton1Click())
	-- Button2
	self.Button2 = self.RootT:Find("Button2").gameObject
	CS.UIUtil.SetClick( self.Button2, self:OnButton2Click())
	-- Button2
	self.Button2 = self.RootT:Find("Close").gameObject
	CS.UIUtil.SetClick( self.Button2, self:OnButton2Click())
	-- Close
	self.BtnClose = self.RootT:Find("Close").gameObject
	CS.UIUtil.SetClick( self.BtnClose, self:OnBtnCloseClick())
    --UI_INIT_END
end



-- Button1
function UIAtlasTest:OnButton1Click()
	local function f( go )
        self.Icon:SetImage("Fruits", "Carrot")
        --self.Icon:SetImage("Fruits", "Banana")
	end
	return f
end

-- Button2
function UIAtlasTest:OnButton2Click()
	local function f( go )
        self.Icon:SetImage("Egg", "Egg")
	end
	return f
end

-- Close
function UIAtlasTest:OnBtnCloseClick()
	local function f( go )
        self:Close()
	end
	return f
end

--UI_FUNC_REPLACE
--不要删除以上注释，否则自动化代码无法生成

return UIAtlasTest
