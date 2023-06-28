//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\UI\Component\UIEventClick.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：UI点击事件
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void OnClickCallback(GameObject go);

public class UIEventClick : MonoBehaviour, IPointerClickHandler
{
    private OnClickCallback onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(gameObject);
        }
            
    }

    public static void Set(GameObject obj, OnClickCallback callback)
    {
        UIEventClick mono = obj.GetComponent<UIEventClick>();
        if (mono == null)
        {
            mono = obj.AddComponent<UIEventClick>();
        }
        else
        {
            mono.onClick = null;
        }

        mono.onClick = callback;
    }

    void OnDestroy()
    {
        onClick = null;
    }
}
