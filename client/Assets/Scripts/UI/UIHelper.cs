//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIHelper.cs
// 作者：Xoen Xie
// 时间：2023/06/29
// 描述：UI助手
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XSF;

namespace XsfUI
{
public enum UIObjectID
{
    UITestShow = 10001,
    UISelectShow,
    UILoadingShow,
}

public enum UIID
{
    None = 0,

    Test,
    AtlasTest,

	Test2,
    //UIID_APPEND

    Max,
}

public sealed class UIHelper : IUIHelper
{
    public int MaxID { get { return (int)UIID.Max; } }

    public UIBase GetUI(int nID)
    {
        switch((UIID)nID)
        {
            case UIID.Test:       return new UITest();
            case UIID.AtlasTest:  return new UIAtlasTest();

			case UIID.Test2:		return new UITest2();
            //GET_UI_APPEND
            default: return null;
        }
    }
}

}
