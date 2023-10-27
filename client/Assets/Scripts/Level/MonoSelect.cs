//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/MonoSelect.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：选择特效
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class MonoSelect : MonoBehaviour
{
    public SpriteRenderer[] Sprite;

    public Animator Anim;

    private void SetColor(Color32 color)
    {
        for(int i = 0; i < Sprite.Length; i ++)
        {
            var curColor = (Color32)Sprite[i].color;
            color.a = curColor.a;
            Sprite[i].color = color;
        }
    }

    public void SetOK()
    {
        SetColor(new Color32(85, 243, 0, 255));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowSelect(int nType)
    {
        gameObject.SetActive(true);
        if(nType == 0)
        {
            Anim.Play("Scale");
            SetColor(new Color32(2, 154, 255, 255));
        }
        else
        {
            
            SetColor(new Color32(220, 251, 209, 255));
            Anim.Play("Show");
        }
    }
}