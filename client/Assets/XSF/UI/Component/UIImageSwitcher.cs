//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\UI\Component\UIImageSwitcher.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：图片切换器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIImageSwitcher : MonoBehaviour
{
    private Image image;
    private AsyncOperationHandle<SpriteAtlas> handle;

    private string m_sAtlasLabel;
    private string m_sSpriteName;

    void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    public void SetImage(string sAtlasLabel, string sSpriteName)
    {
        m_sAtlasLabel = sAtlasLabel;
        m_sSpriteName = sSpriteName;

        if (handle.IsValid())
        {
            // 如果已有异步加载操作在进行中，则先取消该操作
            Addressables.Release(handle);
        }

        handle = Addressables.LoadAssetAsync<SpriteAtlas>(sAtlasLabel);
        handle.Completed += OnAtlasLoaded;
    }

    void OnAtlasLoaded(AsyncOperationHandle<SpriteAtlas> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            SpriteAtlas atlas = handle.Result;

            // 在图集中查找对应名称的Sprite
            Sprite sprite = atlas.GetSprite(m_sSpriteName);
            if (sprite != null)
            {
                image.sprite = sprite;
            }
            else
            {
                Debug.LogError("UIImageSwitcher.OnAtlasLoaded sprite not found, name=" + m_sSpriteName);
            }
        }
    }

    void OnDestroy()
    {
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }
    }
}