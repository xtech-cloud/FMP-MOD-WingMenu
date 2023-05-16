

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.WingMenu.LIB.Proto;
using XTC.FMP.MOD.WingMenu.LIB.MVCS;
using XTC.FMP.LIB.MVCS;

namespace XTC.FMP.MOD.WingMenu.LIB.Unity
{
    /// <summary>
    /// 实例类
    /// </summary>
    public class MyInstance : MyInstanceBase
    {

        public class UiReference
        {
            public RectTransform templateSectionEdge;
            public RectTransform templateSectionMiddle;
            public RectTransform templateEntryEdge;
            public RectTransform templateEntryMiddle;
        }

        private UiReference uiReference_ = new UiReference();

        public MyInstance(string _uid, string _style, MyConfig _config, MyCatalog _catalog, LibMVCS.Logger _logger, Dictionary<string, LibMVCS.Any> _settings, MyEntryBase _entry, MonoBehaviour _mono, GameObject _rootAttachments)
            : base(_uid, _style, _config, _catalog, _logger, _settings, _entry, _mono, _rootAttachments)
        {
        }

        /// <summary>
        /// 当被创建时
        /// </summary>
        /// <remarks>
        /// 可用于加载主题目录的数据
        /// </remarks>
        public void HandleCreated()
        {
            handleCreated();
        }

        /// <summary>
        /// 当被删除时
        /// </summary>
        public void HandleDeleted()
        {
            handleDeleted();
        }

        /// <summary>
        /// 当被打开时
        /// </summary>
        /// <remarks>
        /// 可用于加载内容目录的数据
        /// </remarks>
        public void HandleOpened(string _source, string _uri)
        {
            rootUI.gameObject.SetActive(true);
            rootWorld.gameObject.SetActive(true);
        }

        /// <summary>
        /// 当被关闭时
        /// </summary>
        public void HandleClosed()
        {
            rootUI.gameObject.SetActive(false);
            rootWorld.gameObject.SetActive(false);
        }

        private void handleCreated()
        {
            uiReference_.templateSectionMiddle = rootUI.transform.Find("Scroll View/Viewport/Content/section-middle").GetComponent<RectTransform>();
            uiReference_.templateSectionMiddle.gameObject.SetActive(false);
            uiReference_.templateSectionEdge = rootUI.transform.Find("Scroll View/Viewport/Content/section-edge").GetComponent<RectTransform>();
            uiReference_.templateSectionEdge.gameObject.SetActive(false);
            uiReference_.templateEntryMiddle = uiReference_.templateSectionMiddle.Find("middle/panelContent/entryContainer/Viewport/Content/templateEntry").GetComponent<RectTransform>();
            uiReference_.templateEntryMiddle.gameObject.SetActive(false);
            uiReference_.templateEntryEdge = uiReference_.templateSectionEdge.Find("right/panelContent/entryContainer/Viewport/Content/templateEntry").GetComponent<RectTransform>();
            uiReference_.templateEntryEdge.gameObject.SetActive(false);

            // 设置样式颜色
            System.Action<Transform, string, string> applyStyleColor = (_section, _path, _color) =>
            {
                var target = _section.Find(_path);
                if (null != target)
                {
                    Color color;
                    if (ColorUtility.TryParseHtmlString(_color, out color))
                        target.GetComponent<Image>().color = color;
                }
            };
            applyStyleColor(uiReference_.templateSectionEdge, "left", style_.edgePanel.color);
            applyStyleColor(uiReference_.templateSectionEdge, "right", style_.edgePanel.color);
            applyStyleColor(uiReference_.templateSectionMiddle, "middle", style_.middlePanel.color);

            // 设置入口样式
            System.Action<RectTransform, string, string> createEntry = (_template, _icon, _text) =>
            {
                var clone = GameObject.Instantiate(_template.gameObject, _template.parent);
                loadTextureFromTheme(_icon, (_texture) =>
                {
                    clone.GetComponent<RawImage>().texture = _texture;
                }, () => { });
                clone.transform.Find("Text").GetComponent<Text>().text = _text;
            };
            foreach (var entry in style_.entryS)
            {
                createEntry(uiReference_.templateEntryEdge, entry.icon, entry.text);
                createEntry(uiReference_.templateEntryMiddle, entry.icon, entry.text);
            }

            // 设置中部贴花
            Transform middleDecal = uiReference_.templateSectionMiddle.Find("middle/panelContent/frame");
            loadTextureFromTheme(style_.middlePanel.decal, (_texture) =>
            {
                middleDecal.GetComponent<RawImage>().texture = _texture;
                middleDecal.gameObject.SetActive(null != _texture);
            }, () => { });

            // 创建根门户
            foreach (var section in catalog_.sectionS)
            {
                var strS = section.path.Split('/');
                if (strS.Length != 2)
                    continue;
                createPortal(section);
            }
        }

        private void handleDeleted()
        {

        }

        private void createPortal(MyCatalog.Section _section)
        {
            logger_.Debug("create portal with path:{0}", _section.path);
            string mode;
            _section.kvS.TryGetValue("mode", out mode);
            if (string.IsNullOrEmpty(mode))
            {
                logger_.Error("mode is null or empty", mode);
                return;
            }
            var template = uiReference_.templateSectionEdge;
            if ("middle" == mode)
            {
                template = uiReference_.templateSectionMiddle;
            }
            var clone = GameObject.Instantiate(template.gameObject, template.parent);
            clone.SetActive(true);
            clone.name = _section.path.Replace("/", "_");

            // 加载banner图片
            string banner;
            _section.kvS.TryGetValue("banner", out banner);
            if (!string.IsNullOrEmpty(banner))
            {
                loadTextureFromTheme(banner, (_texture) =>
                {
                    clone.transform.Find("imgBanner").GetComponent<RawImage>().texture = _texture;
                }, () => { });
            }
        }
    }
}
