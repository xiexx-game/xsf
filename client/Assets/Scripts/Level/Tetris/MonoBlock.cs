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

public enum BlockColor
{
    Yellow = 0,
    Green,
    Blue,
    Cyan,
    Red,
    Max,
}

public interface IBlockDisappearEvent
{
    void OnBlockDisappearEvent(int row, int col);
    void OnBlockDisappearDone(int row, int col);
}

public class MonoBlock : MonoBehaviour
{
    public Color Yellow;
    public Color Green;
    public Color Blue;
    public Color Cyan;
    public Color Red;

    public Color PreShow;

    public SpriteRenderer Image;

    private static Color[] m_Colors;

    public static int MAX_COLOR_COUNT = 5;

    private Color m_CurColor;
    public Color ShimmerColor;

    public float ShimmerSpeed;
    private float m_ShimmerValue;

    public float DisappearSpeed;
    public float DisappearEvent;
    private float m_fDisappearScale;
    private IBlockDisappearEvent m_EventHandler;
    private int m_DisappearRow;
    private int m_DisappearCol;

    public int ColorIndex;

    enum BlockStatus
    {
        None = 0,
        ShimmerUp,
        ShimmerDown,
        Disappear,
    }

    private BlockStatus m_nStatus;

    void Awake()
    {
        if(m_Colors == null)
        {
            m_Colors = new Color[MAX_COLOR_COUNT];
            m_Colors[0] = Yellow;
            m_Colors[1] = Green;
            m_Colors[2] = Blue;
            m_Colors[3] = Cyan;
            m_Colors[4] = Red;
        }
    }

    public void ShowWithColor(int color)
    {
        SetColor(color);
        gameObject.transform.localScale = Vector3.one;
    }

    public void SetColor(int color)
    {
        ColorIndex = color;
        Image.color = m_Colors[color];
    }

    public void SetPreColor()
    {
        Image.color = PreShow;
    }

    void Update()
    {
        switch(m_nStatus)
        {
        case BlockStatus.ShimmerUp:
            {
                Color c = Image.color;
                c.r = Mathf.Lerp(m_CurColor.r, ShimmerColor.r, m_ShimmerValue);
                c.g = Mathf.Lerp(m_CurColor.g, ShimmerColor.g, m_ShimmerValue);
                c.b = Mathf.Lerp(m_CurColor.b, ShimmerColor.b, m_ShimmerValue);
                m_ShimmerValue += Time.deltaTime * ShimmerSpeed;
                if(m_ShimmerValue >= 1.0f)
                {
                    m_nStatus = BlockStatus.ShimmerDown;
                }

                Image.color = c;
            }
            break;
        
        case BlockStatus.ShimmerDown:
            {
                Color c = Image.color;
                c.r = Mathf.Lerp(m_CurColor.r, ShimmerColor.r, m_ShimmerValue);
                c.g = Mathf.Lerp(m_CurColor.g, ShimmerColor.g, m_ShimmerValue);
                c.b = Mathf.Lerp(m_CurColor.b, ShimmerColor.b, m_ShimmerValue);
                m_ShimmerValue -= Time.deltaTime * ShimmerSpeed;
                Image.color = c;

                if(m_ShimmerValue <= 0.0f)
                {
                    Image.color = m_CurColor;
                    m_nStatus = BlockStatus.None;
                }
            }
            break;

        case BlockStatus.Disappear:
            {
                var curScale = gameObject.transform.localScale;
                float step = Time.deltaTime * DisappearSpeed;
                curScale.x -= step;
                curScale.y -= step;
                curScale.z -= step;
                //Debug.Log($"curScale={curScale}");
                gameObject.transform.localScale = curScale;

                if(curScale.x <= 0)
                {
                    if(m_EventHandler != null)
                    {
                        //Debug.LogError($"curScale={curScale} OnBlockDisappearDone");
                        m_EventHandler.OnBlockDisappearDone(m_DisappearRow, m_DisappearCol);
                    }
                        
                    m_nStatus = BlockStatus.None;
                }
                else if(curScale.x <= DisappearEvent)
                {
                    if(m_fDisappearScale <= 0.001f)
                    {
                        m_fDisappearScale = DisappearEvent;
                        m_EventHandler.OnBlockDisappearEvent(m_DisappearRow, m_DisappearCol);
                    }
                }
            }
            break;
        }
    }

    public void DoShimmer()
    {
        m_CurColor = Image.color;
        m_nStatus = BlockStatus.ShimmerUp;
        m_ShimmerValue = 0;
    }

    public void DoDisappear(int row, int col, IBlockDisappearEvent Handler)
    {
        if(m_nStatus == BlockStatus.ShimmerUp || m_nStatus == BlockStatus.ShimmerDown)
        {
            Image.color = m_CurColor;
        }

        m_nStatus = BlockStatus.Disappear;
        m_DisappearRow = row;
        m_DisappearCol = col;
        m_fDisappearScale = 0;
        m_EventHandler = Handler;
        //Debug.LogError($"DoDisappear row={row}, col={col}");
    }
}