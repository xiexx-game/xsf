//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\UI\Component\EmptyImage.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：空图
// 说明：有一些时候，只需要点击，不需要图片显示，可以放一张空图
//
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace XSF
{
    public class EmptyImage : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }

}