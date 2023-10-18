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

    public void ShowSelect(int nType)
    {
        if(nType == 0)
        {
            Anim.Play("Scale");
        }
        else
        {
            Anim.Play("Show");
        }
    }
}