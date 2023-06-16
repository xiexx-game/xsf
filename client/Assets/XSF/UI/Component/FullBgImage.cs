//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\UI\Component\FullBgImage.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：全屏锁比例图片控件
// 说明：这个是之前在网上找的一个实现方法，好像是在csdn上找的，忘记出处了，后面找到了再把相文档关链接补上。
//
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullBgImage : MonoBehaviour
{
    [Header("图片原始宽度")]
    public float width;

    [Header("图片原始高度")]
    public float height;

    void Start()
    {
        UpdateFit();
    }

    public void UpdateFit()
    {
        Rect canvasSize = gameObject.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;

        //当前画布尺寸长宽比
        float screenxyRate = canvasSize.width / canvasSize.height;

        Vector2 bgSize = new Vector2(width, height);

        //图片尺寸长宽比
        float texturexyRate = bgSize.x / bgSize.y;

        RectTransform rt = (RectTransform)transform;
        //图片x偏长,需要适配y
        if (texturexyRate > screenxyRate)
        {
            int newSizeY = Mathf.CeilToInt(canvasSize.height);
            int newSizeX = Mathf.CeilToInt((float)newSizeY / bgSize.y * bgSize.x);
            rt.sizeDelta = new Vector2(newSizeX, newSizeY);
        }
        else
        {
            int newVideoSizeX = Mathf.CeilToInt(canvasSize.width);
            int newVideoSizeY = Mathf.CeilToInt((float)newVideoSizeX / bgSize.x * bgSize.y);
            rt.sizeDelta = new Vector2(newVideoSizeX, newVideoSizeY);
        }
    }
}
