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
using YooAsset;
using System.Collections;

namespace XSF
{
    public class UIImageSwitcher : MonoBehaviour
    {
        private Image image;
        private AssetHandle handle;

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

            if (handle != null)
            {
                // 如果已有异步加载操作在进行中，则先取消该操作
                handle.Release();
                handle = null;
            }

            StopAllCoroutines();

            StartCoroutine(LoadSprite());
        }

        IEnumerator LoadSprite()
        {
            var package = YooAssets.GetPackage(XSFConfig.Instance.YooAssetPackage);
            handle = package.LoadAssetAsync<SpriteAtlas>("Atlas_" + m_sAtlasLabel);
            yield return handle;
            var atlas = handle.AssetObject as SpriteAtlas;
            var sprite = atlas.GetSprite(m_sSpriteName);
            if(sprite == null)
            {
                Debug.LogError("UIImageSwitcher LoadSprite sprite not exist, name=" + m_sSpriteName);
            }
            else
            {
                image.sprite = sprite;
                Debug.Log($"UIImageSwitcher LoadSprite Sprite name is {sprite.name}");
            }
        }

        void OnDestroy()
        {
            if (handle != null)
            {
                handle.Release();
                handle = null;
            }
        }
    }
}