//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/Snake/MonoSnakeFood.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：单个方块脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class MonoSnakeFood : MonoBehaviour
{
    public SpriteRenderer Image;

    public Color32[] Colors;

    public void Show()
    {
        int nIndex = UnityEngine.Random.Range(0, Colors.Length);
        Image.color = Colors[nIndex];
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}