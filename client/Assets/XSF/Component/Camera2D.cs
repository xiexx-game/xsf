//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets/XSF/Component/Camera2D.cs
// 作者：Xoen Xie
// 时间：2023/08/07
// 描述：2D游戏 相机自动分辨率脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class Camera2D : MonoBehaviour
{
    // 设计分辨率
    public float targetResolutionWidth = 1920;
    public float targetResolutionHeight = 1080;

    public float orthographicSize;

    public float cameraSizeHight;
    public float cameraSizeWidth;
    public float aspectRatio;


    // Use this for initialization
    void Start()
    {
        aspectRatio = targetResolutionWidth / targetResolutionHeight;

        orthographicSize = (targetResolutionHeight / 100) / 2;

        cameraSizeHight = orthographicSize * 2.0f;
        cameraSizeWidth = cameraSizeHight * aspectRatio;


        float fCurAspectRatio = (float)Screen.width / (float)Screen.height;

        XSF.Log($"width:{Screen.width}, height:{Screen.height}, fCurAspectRatio:{fCurAspectRatio}");

        if (fCurAspectRatio < aspectRatio)
        {
            float height = cameraSizeWidth / fCurAspectRatio;
            orthographicSize = height / 2;
        }

        XSFMain.Instance.MainCamera.orthographicSize = orthographicSize;
    }
}
