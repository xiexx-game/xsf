//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIHelper.cs
// 作者：Xoen Xie
// 时间：2023/06/29
// 描述：UI助手
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public enum UIID
{
    None = 0,

    UITest,
    UIAtlasTest,
    UIMain,
    UILevel,

    Max,
}

public sealed class UIHelper : IUIHelper
{
    public int MaxID { get { return (int)UIID.Max; } }

    public UIBase GetUI(int nID)
    {
        switch((UIID)nID)
        {
            case UIID.UITest:       return new UITest();
            case UIID.UIAtlasTest:  return new UIAtlasTest();
            case UIID.UIMain:       return new UIMain();
            case UIID.UILevel:      return new UILevel();
            default: return null;
        }
    }
}