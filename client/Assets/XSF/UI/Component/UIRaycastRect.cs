//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\Edtor\UI\UIRaycastRect.cs
// 作者：Xoen Xie
// 时间：2023/06/20
// 描述：显示UI里有Raycast的区域
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace XSF
{
    public class UIRaycastRect : MonoBehaviour
    {
        static Vector3[] fourCorners = new Vector3[4];

        void OnDrawGizmos()
        {
            foreach (Graphic g in GameObject.FindObjectsOfType<Graphic>())
            {
                if (g.raycastTarget)
                {
                    RectTransform rect = g.transform as RectTransform;
                    rect.GetWorldCorners(fourCorners);
                    Gizmos.color = Color.green;
                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
                    }
                }
            }
        }
    }
}