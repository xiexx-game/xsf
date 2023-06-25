--------------------------------------------------------------------------
--
-- 文件：Lua-src\UI\View\UITest.lua
-- 作者：Xoen Xie
-- 时间：2023/6/23
-- 描述：
-- 说明：
--
--------------------------------------------------------------------------
local UIBase = require "UI.UIBase"

local UITest = 
{
    Name = "UITest",
}

setmetatable(UITest, UIBase)
UITest.__super = getmetatable(UITest)

function UITest:OnInit()
    xsf_log("UITest:OnInit start")
    --UI_INIT_BEGIN
	-- 
	self.Image = self.RootT:Find("Image"):GetComponent(typeof(CS.UnityEngine.UI.Image))
	-- Button1 click
	self.Button1 = self.RootT:Find("Bottom/Button1").gameObject
	CS.UIUtil.SetClick( self.Button1, self:OnButton1Click())
	-- Btn1Text
	self.Btn1Text = self.RootT:Find("Bottom/Button1/Text"):GetComponent(typeof(CS.UILocalizationTMP))
	-- Button2 click
	self.Button2 = self.RootT:Find("Bottom/Button2").gameObject
	CS.UIUtil.SetClick( self.Button2, self:OnButton2Click())
	-- Button2Text
	self.Button2Text = self.RootT:Find("Bottom/Button2/Text"):GetComponent(typeof(CS.TMPro.TextMeshProUGUI))
	-- Button3 click
	self.Button3 = self.RootT:Find("Bottom/Button3").gameObject
    --UI_INIT_END

    xsf_log("UITest:OnInit end", self.Button2Text.text)
end



-- Button1 click
function UITest:OnButton1Click()
	local function f( go )
        xsf_log("UITest:OnButton1Click")
        self.Btn1Text:SetKey("login")
	end
	return f
end


-- Button2 click
function UITest:OnButton2Click()
	local function f( go )
        local UI = require "UI.UI"
        UI:Show("UIAtlasTest");
	end
	return f
end

--UI_FUNC_REPLACE
--不要删除以上注释，否则自动化代码无法生成

return UITest
