//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/MonoBlock.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：单个方块脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;


public class MonoBlock : MonoBehaviour
{
    public Color32 Yellow;
    public Color32 Green;
    public Color32 Blue;
    public Color32 Cyan;
    public Color32 Red;
    public Color32 Hide;
    public Color32 PreShow;

    public SpriteRenderer Image;

    private Color32[] m_Colors;

    public static int MAX_COLOR_COUNT = 5;

    void Awake()
    {
        m_Colors = new Color32[MAX_COLOR_COUNT];
        m_Colors[0] = Yellow;
        m_Colors[1] = Green;
        m_Colors[2] = Blue;
        m_Colors[3] = Cyan;
        m_Colors[4] = Red;
    }

    public void DoHide()
    {
        Image.color = Hide;
    }

    public void SetColor(int nIndex)
    {
        Image.color = m_Colors[nIndex];
    }
}