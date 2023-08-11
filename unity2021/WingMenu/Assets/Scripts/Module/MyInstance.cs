

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.WingMenu.LIB.Proto;
using XTC.FMP.MOD.WingMenu.LIB.MVCS;
using XTC.FMP.LIB.MVCS;
using System.Collections;
using Newtonsoft.Json;
using System;

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
        }

        public class PortalClone
        {
            public GameObject root;
            public Transform bundleContainer;
            public Transform contentContainer;
            public Transform filterContainer;
            public Transform entryContainer;
            public Button btnNav;
            public List<GameObject> menuS = new List<GameObject>();
            public List<GameObject> contentS = new List<GameObject>();
            public List<GameObject> entryS = new List<GameObject>();
            public Transform panelContentDetail;
            public RawImage imgDetailIcon;
            public Text textDetailTitle;
            public Text textDetailLabel;
            public Text textDetailCaption;
            public Text textDetailTopic;
            public Text textDetailDescription;
            public bool contentVisibleCreated;
            public ContentMetaSchema activeContentMetaSchema;
        }

        private UiReference uiReference_ = new UiReference();
        private AssetReader assetReader_ = null;

        // 创建的门户实例列表，key 是 section.path
        private Dictionary<string, PortalClone> portalCloneS = new Dictionary<string, PortalClone>();
        // 页码的索引， key is instanceID of portal
        private Dictionary<int, List<GameObject>> pageMapS = new Dictionary<int, List<GameObject>>();
        // bundle的meta结构索引, key is bundleUuid
        private Dictionary<string, BundleMetaSchema> bundleSchemaS_ = new Dictionary<string, BundleMetaSchema>();
        private Dictionary<string, Texture2D> portalCoverS_ = new Dictionary<string, Texture2D>();

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
            handleOpended();
        }

        /// <summary>
        /// 当被关闭时
        /// </summary>
        public void HandleClosed()
        {
            handleClosed();
        }

        private void handleCreated()
        {
            uiReference_.templateSectionMiddle = rootUI.transform.Find("Scroll View/Viewport/Content/section-middle").GetComponent<RectTransform>();
            uiReference_.templateSectionEdge = rootUI.transform.Find("Scroll View/Viewport/Content/section-edge").GetComponent<RectTransform>();
            var templateEntryMiddle = uiReference_.templateSectionMiddle.Find("middle/panelContent/entryContainer/Viewport/Content/templateEntry").GetComponent<RectTransform>();
            var templateEntryEdge = uiReference_.templateSectionEdge.Find("right/panelContent/entryContainer/Viewport/Content/templateEntry").GetComponent<RectTransform>();
            // 隐藏模板
            uiReference_.templateSectionMiddle.gameObject.SetActive(false);
            uiReference_.templateSectionEdge.gameObject.SetActive(false);
            uiReference_.templateSectionEdge.Find("left/filter/templateTab").gameObject.SetActive(false);
            uiReference_.templateSectionMiddle.Find("middle/filter/templateTab").gameObject.SetActive(false);
            uiReference_.templateSectionEdge.Find("left/Scroll View/Viewport/Content/templateBundle").gameObject.SetActive(false);
            uiReference_.templateSectionEdge.Find("left/Scroll View/Viewport/Content/templateContent").gameObject.SetActive(false);
            uiReference_.templateSectionMiddle.Find("middle/layout/templateContent").gameObject.SetActive(false);
            templateEntryEdge.gameObject.SetActive(false);
            templateEntryMiddle.gameObject.SetActive(false);
            // 设置字体
            Font mainFont = settings_["font.main"].AsObject() as Font;
            Color bundleCellFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.bundleCell.fontColor, out bundleCellFontColor))
                bundleCellFontColor = Color.black;
            Color contentCellFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.contentCell.fontColor, out contentCellFontColor))
                contentCellFontColor = Color.black;
            Color entryCellFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.entryCell.fontColor, out entryCellFontColor))
                entryCellFontColor = Color.black;
            Color filterFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.filter.fontColor, out filterFontColor))
                filterFontColor = Color.black;
            Color navigationFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.navigation.fontColor, out navigationFontColor))
                navigationFontColor = Color.black;
            Color titleFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.contentDetail.title.fontColor, out titleFontColor))
                titleFontColor = Color.black;
            Color labelFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.contentDetail.label.fontColor, out labelFontColor))
                labelFontColor = Color.black;
            Color captionFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.contentDetail.caption.fontColor, out captionFontColor))
                captionFontColor = Color.black;
            Color topicFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.contentDetail.topic.fontColor, out topicFontColor))
                topicFontColor = Color.black;
            Color descriptionFontColor;
            if (!ColorUtility.TryParseHtmlString(style_.contentDetail.description.fontColor, out descriptionFontColor))
                descriptionFontColor = Color.black;
            System.Action<Text, Font, int, Color> applyFontStyle = (_text, _font, _fontSize, _fontColor) =>
            {
                _text.font = mainFont;
                _text.fontSize = _fontSize;
                _text.color = _fontColor;
            };
            applyFontStyle(uiReference_.templateSectionEdge.Find("left/Scroll View/Viewport/Content/templateBundle/txtName").GetComponent<Text>(), mainFont, style_.bundleCell.fontSize, bundleCellFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("left/Scroll View/Viewport/Content/templateContent/txtName").GetComponent<Text>(), mainFont, style_.bundleCell.fontSize, bundleCellFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("left/filter/templateTab/Label").GetComponent<Text>(), mainFont, style_.filter.fontSize, filterFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("left/filter/all/Label").GetComponent<Text>(), mainFont, style_.filter.fontSize, filterFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("left/nav/txtName").GetComponent<Text>(), mainFont, style_.navigation.fontSize, navigationFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("right/panelContent/txtTitle").GetComponent<Text>(), mainFont, style_.contentDetail.title.fontSize, titleFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("right/panelContent/txtLabel").GetComponent<Text>(), mainFont, style_.contentDetail.label.fontSize, labelFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("right/panelContent/txtCaption").GetComponent<Text>(), mainFont, style_.contentDetail.caption.fontSize, captionFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("right/panelContent/txtTopic").GetComponent<Text>(), mainFont, style_.contentDetail.topic.fontSize, topicFontColor);
            applyFontStyle(uiReference_.templateSectionEdge.Find("right/panelContent/svDescription/Viewport/Content/txtDescription").GetComponent<Text>(), mainFont, style_.contentDetail.description.fontSize, descriptionFontColor);
            applyFontStyle(uiReference_.templateSectionMiddle.Find("middle/filter/templateTab/Label").GetComponent<Text>(), mainFont, style_.filter.fontSize, filterFontColor);
            applyFontStyle(uiReference_.templateSectionMiddle.Find("middle/filter/all/Label").GetComponent<Text>(), mainFont, style_.filter.fontSize, filterFontColor);
            applyFontStyle(uiReference_.templateSectionMiddle.Find("middle/nav/txtName").GetComponent<Text>(), mainFont, style_.navigation.fontSize, navigationFontColor);
            applyFontStyle(uiReference_.templateSectionMiddle.Find("middle/panelContent/txtTitle").GetComponent<Text>(), mainFont, style_.contentDetail.title.fontSize, titleFontColor);
            applyFontStyle(uiReference_.templateSectionMiddle.Find("middle/panelContent/txtLabel").GetComponent<Text>(), mainFont, style_.contentDetail.label.fontSize, labelFontColor);
            applyFontStyle(uiReference_.templateSectionMiddle.Find("middle/panelContent/txtCaption").GetComponent<Text>(), mainFont, style_.contentDetail.caption.fontSize, captionFontColor);
            applyFontStyle(uiReference_.templateSectionMiddle.Find("middle/panelContent/txtTopic").GetComponent<Text>(), mainFont, style_.contentDetail.topic.fontSize, topicFontColor);
            applyFontStyle(uiReference_.templateSectionMiddle.Find("middle/panelContent/svDescription/Viewport/Content/txtDescription").GetComponent<Text>(), mainFont, style_.contentDetail.description.fontSize, descriptionFontColor);
            applyFontStyle(templateEntryMiddle.Find("Text").GetComponent<Text>(), mainFont, style_.entryCell.fontSize, entryCellFontColor);
            applyFontStyle(templateEntryEdge.Find("Text").GetComponent<Text>(), mainFont, style_.entryCell.fontSize, entryCellFontColor);

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
            System.Action<RectTransform, string, string, string> createEntry = (_template, _icon, _text, _metaKey) =>
            {
                var clone = GameObject.Instantiate(_template.gameObject, _template.parent);
                clone.name = _metaKey;
                loadTextureFromTheme(_icon, (_texture) =>
                {
                    clone.GetComponent<RawImage>().texture = _texture;
                }, () => { });
                clone.transform.Find("Text").GetComponent<Text>().text = _text;
            };
            foreach (var entry in style_.entryS)
            {
                createEntry(templateEntryEdge, entry.icon, entry.text, entry.kvKey);
                createEntry(templateEntryMiddle, entry.icon, entry.text, entry.kvKey);
            }

            // 设置中部贴花
            Transform middleDecal = uiReference_.templateSectionMiddle.Find("middle/panelContent/frame");
            loadTextureFromTheme(style_.middlePanel.decal, (_texture) =>
            {
                middleDecal.GetComponent<RawImage>().texture = _texture;
                middleDecal.gameObject.SetActive(null != _texture);
            }, () => { });

        }

        private void handleOpended()
        {
            assetReader_ = new AssetReader(assetObjectsPool);
            assetReader_.AssetRootPath = settings_["path.assets"].AsString();
            rootUI.gameObject.SetActive(true);
            rootWorld.gameObject.SetActive(true);

            // 创建根门户
            foreach (var section in catalog_.sectionS)
            {
                var strS = section.path.Split('/');
                if (strS.Length != 2)
                    continue;
                createPortal(section);
            }
            // 创建目录
            foreach (var section in catalog_.sectionS)
            {
                var strS = section.path.Split('/');
                if (strS.Length <= 2)
                    continue;
                createMenu(section);
                foreach (var content in section.contentS)
                {
                    var bundleUuid = content.Split("/")[0];
                    bundleSchemaS_[bundleUuid] = null;
                }
            }

            CounterSequence sequence = new CounterSequence(bundleSchemaS_.Count);
            sequence.OnFinish = () =>
            {
                logger_.Info("all bundles are parsed.");
                List<string> contentUriS = new List<string>();
                foreach (var section in catalog_.sectionS)
                {
                    var strS = section.path.Split('/');
                    if (strS.Length <= 2)
                        continue;

                    contentUriS.Clear();
                    // 获取一个section下的所有content
                    foreach (var content in section.contentS)
                    {
                        if (content.EndsWith("/+"))
                        {
                            string bundleUuid = content.Split("/")[0];
                            BundleMetaSchema meta = null;
                            bundleSchemaS_.TryGetValue(bundleUuid, out meta);
                            if (null != meta)
                            {
                                foreach (var contentUuid in meta.foreign_content_uuidS)
                                {
                                    string contentUri = string.Format("{0}/{1}", meta.Uuid, contentUuid);
                                    if (!contentUriS.Contains(contentUri))
                                        contentUriS.Add(contentUri);
                                }
                            }
                            else
                            {
                                logger_.Error("meta of bundle:{0} is null", bundleUuid);
                            }
                        }
                        else
                        {
                            if (!contentUriS.Contains(content))
                                contentUriS.Add(content);
                        }
                    }
                    foreach (var contentUri in contentUriS)
                    {
                        createContent(section, contentUri);
                    }
                }
            };

            // 加载需要的bundle的meta.json
            foreach (var pair in bundleSchemaS_)
            {
                string uuid = pair.Key;
                assetReader_.LoadText(uuid + "/meta.json", (_bytes) =>
                {
                    BundleMetaSchema meta = null;
                    try
                    {
                        string json = System.Text.Encoding.UTF8.GetString(_bytes);
                        meta = JsonConvert.DeserializeObject<BundleMetaSchema>(json);
                    }
                    catch (System.Exception ex)
                    {
                        logger_.Error("parse meta of bundle:{0} happened error", uuid);
                        logger_.Exception(ex);
                    }
                    bundleSchemaS_[uuid] = meta;
                    sequence.Tick();
                }, () => { });
            }
        }

        private void handleClosed()
        {
            assetReader_ = null;
            rootUI.gameObject.SetActive(false);
            rootWorld.gameObject.SetActive(false);
        }

        private void handleDeleted()
        {

        }

        /// <summary>
        /// 创建门户
        /// </summary>
        /// <param name="_section"></param>
        private void createPortal(MyCatalog.Section _section)
        {
            logger_.Debug("create portal with path:{0}", _section.path);

            string shape;
            _section.kvS.TryGetValue("shape", out shape);
            if (string.IsNullOrEmpty(shape))
            {
                logger_.Error("shape is not found in kvS");
                return;
            }
            string pagination;
            _section.kvS.TryGetValue("pagination", out pagination);
            if (string.IsNullOrEmpty(pagination))
            {
                logger_.Error("pagination not found in kvS");
                return;
            }

            string cover;
            _section.kvS.TryGetValue("cover", out cover);
            if (!string.IsNullOrEmpty(cover))
            {
                loadTextureFromTheme(cover, (_texture) =>
                {
                    portalCoverS_[_section.path] = _texture;
                }, () => { });
            }

            RectTransform template = null;
            if ("middle" == shape)
            {
                template = uiReference_.templateSectionMiddle;
            }
            else if ("edge" == shape)
            {
                template = uiReference_.templateSectionEdge;
            }
            else
            {
                logger_.Error("shape:{0} not supported", shape);
                return;
            }

            var clone = GameObject.Instantiate(template.gameObject, template.parent);
            clone.SetActive(true);
            clone.name = _section.path.Replace("/", "_");
            var portalClone = new PortalClone();
            portalClone.root = clone;
            if (shape == "edge")
            {
                portalClone.entryContainer = clone.transform.Find("right/panelContent/entryContainer/Viewport/Content");
                portalClone.bundleContainer = clone.transform.Find("left/Scroll View/Viewport/Content");
                portalClone.contentContainer = clone.transform.Find("left/Scroll View/Viewport/Content");
                portalClone.filterContainer = clone.transform.Find("left/filter");
                portalClone.btnNav = clone.transform.Find("left/nav").GetComponent<Button>();
                portalClone.panelContentDetail = clone.transform.Find("right/panelContent");
                portalClone.imgDetailIcon = clone.transform.Find("right/panelContent/imgIcon").GetComponent<RawImage>();
                portalClone.textDetailTitle = clone.transform.Find("right/panelContent/txtTitle").GetComponent<Text>();
                portalClone.textDetailLabel = clone.transform.Find("right/panelContent/txtLabel").GetComponent<Text>();
                portalClone.textDetailCaption = clone.transform.Find("right/panelContent/txtCaption").GetComponent<Text>();
                portalClone.textDetailTopic = clone.transform.Find("right/panelContent/txtTopic").GetComponent<Text>();
                portalClone.textDetailDescription = clone.transform.Find("right/panelContent/svDescription/Viewport/Content/txtDescription").GetComponent<Text>();
            }
            else if (shape == "middle")
            {
                portalClone.entryContainer = clone.transform.Find("middle/panelContent/entryContainer/Viewport/Content");
                portalClone.bundleContainer = null;
                portalClone.contentContainer = clone.transform.Find("middle/layout");
                portalClone.filterContainer = clone.transform.Find("middle/filter");
                portalClone.btnNav = clone.transform.Find("middle/nav").GetComponent<Button>();
                portalClone.panelContentDetail = clone.transform.Find("middle/panelContent");
                portalClone.imgDetailIcon = clone.transform.Find("middle/panelContent/imgIcon").GetComponent<RawImage>();
                portalClone.textDetailTitle = clone.transform.Find("middle/panelContent/txtTitle").GetComponent<Text>();
                portalClone.textDetailLabel = clone.transform.Find("middle/panelContent/txtLabel").GetComponent<Text>();
                portalClone.textDetailCaption = clone.transform.Find("middle/panelContent/txtCaption").GetComponent<Text>();
                portalClone.textDetailTopic = clone.transform.Find("middle/panelContent/txtTopic").GetComponent<Text>();
                portalClone.textDetailDescription = clone.transform.Find("middle/panelContent/svDescription/Viewport/Content/txtDescription").GetComponent<Text>();
            }
            portalClone.contentVisibleCreated = pagination == "filter";
            foreach (var entry in style_.entryS)
            {
                if (string.IsNullOrEmpty(entry.kvKey))
                    continue;
                var tEntry = portalClone.entryContainer.Find(entry.kvKey);
                if (null == tEntry)
                    continue;
                portalClone.entryS.Add(tEntry.gameObject);
                tEntry.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (null == portalClone.activeContentMetaSchema)
                        return;
                    string kvValue;
                    portalClone.activeContentMetaSchema.kvS.TryGetValue(entry.kvKey, out kvValue);
                    if (string.IsNullOrEmpty(kvValue))
                        return;
                    string resource_uri = string.Format("{0}/_resources/{1}", portalClone.activeContentMetaSchema.foreign_bundle_uuid, kvValue);
                    string content_uri = string.Format("{0}/{1}", portalClone.activeContentMetaSchema.foreign_bundle_uuid, portalClone.activeContentMetaSchema.Uuid);
                    Dictionary<string, object> variableS = new Dictionary<string, object>();
                    foreach (var subject in entry.subjectS)
                    {
                        foreach (var parameter in subject.parameters)
                        {
                            if (parameter.type == "_")
                            {
                                variableS[parameter.key] = parameter.key.Replace("{{resource_uri}}", resource_uri).Replace("{{content_uri}}", content_uri);
                            }
                        }
                    }
                    publishSubjects(entry.subjectS, variableS);
                });
            }
            portalCloneS[_section.path] = portalClone;

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

            // 导览栏
            var navTxt = portalClone.btnNav.transform.Find("txtName").GetComponent<Text>();
            navTxt.text = "";
            portalClone.btnNav.gameObject.SetActive(false);
            // 过滤栏
            portalClone.filterContainer.gameObject.SetActive(pagination == "filter");
            var filterAll = portalClone.filterContainer.Find("all").GetComponent<Toggle>();

            if ("edge" == shape)
            {
                // 隐藏内容信息
                var panelContent = clone.transform.Find("right/panelContent");
                panelContent.gameObject.SetActive(false);

                // 隐藏封面
                var imgCover = clone.transform.Find("right/imgCover");
                imgCover.gameObject.SetActive(false);
            }

            // banner 事件
            var imgBanner = clone.transform.Find("imgBanner");
            imgBanner.GetComponent<Toggle>().onValueChanged.AddListener((_toggled) =>
            {
                var animator = clone.GetComponent<Animator>();
                if (_toggled)
                {
                    animator.SetTrigger("expand");
                    if ("filter" == pagination)
                    {
                        mono_.StartCoroutine(delay(0.1f, () =>
                        {
                            filterAll.isOn = true;
                        }));
                    }
                    else if ("navigation" == pagination)
                    {
                        foreach (var obj in portalClone.menuS)
                        {
                            obj.SetActive(true);
                        }
                        foreach (var obj in portalClone.contentS)
                        {
                            obj.SetActive(false);
                        }
                        portalClone.btnNav.gameObject.SetActive(false);
                        showPortalCover(_section.path, clone);
                    }
                    portalClone.panelContentDetail.gameObject.SetActive(false);
                }
                else
                {
                    animator.SetTrigger("fold");
                    if ("middle" == shape)
                    {
                        clone.transform.Find("middle/layout").GetComponent<GridLayoutGroup>().enabled = true;
                        clone.transform.Find("middle/panelContent").gameObject.SetActive(false);
                    }
                }
            });

            if ("middle" == shape)
            {
                var imgBannerLeft = clone.transform.Find("imgBannerLeft");
                var imgBannerRight = clone.transform.Find("imgBannerRight");
                imgBannerLeft.GetComponent<Button>().onClick.AddListener(() =>
                {
                    imgBanner.GetComponent<Toggle>().isOn = false;
                });
                imgBannerRight.GetComponent<Button>().onClick.AddListener(() =>
                {
                    imgBanner.GetComponent<Toggle>().isOn = false;
                });
                System.Action<int> onPageButtonClick = (_offset) =>
                {
                    List<GameObject> pageObjs;
                    if (pageMapS.TryGetValue(clone.GetInstanceID(), out pageObjs))
                    {
                        for (int i = 0; i < pageObjs.Count; i++)
                        {
                            Toggle toggle = pageObjs[i].GetComponent<Toggle>();
                            if (toggle.isOn)
                            {
                                int index = i + _offset;
                                if (index >= 0 && index < pageObjs.Count)
                                    pageObjs[index].GetComponent<Toggle>().isOn = true;
                                break;
                            }
                        }
                    }
                };
                clone.transform.Find("middle/page/anchor/btnPrev").GetComponent<Button>().onClick.AddListener(() => { onPageButtonClick(-1); });
                clone.transform.Find("middle/page/anchor/btnNext").GetComponent<Button>().onClick.AddListener(() => { onPageButtonClick(1); });
            }
            // 导览栏事件
            portalClone.btnNav.onClick.AddListener(() =>
            {
                // 隐藏全部内容
                foreach (var obj in portalClone.contentS)
                {
                    obj.SetActive(false);
                }
                //  显示全部菜单
                foreach (var obj in portalClone.menuS)
                {
                    obj.SetActive(true);
                    showPortalCover(_section.path, clone);
                }
                portalClone.btnNav.gameObject.SetActive(false);
                portalClone.panelContentDetail.gameObject.SetActive(false);
            });
            // 过滤栏事件
            filterAll.onValueChanged.AddListener((_toggled) =>
            {
                if (!_toggled)
                    return;
                foreach (var obj in portalClone.contentS)
                {
                    obj.SetActive(true);
                }
                portalClone.panelContentDetail.gameObject.SetActive(false);
            });
        }

        private void showPortalCover(string _sectionPath, GameObject _portalClone)
        {
            var imgCover = _portalClone.transform.Find("right/imgCover");
            if (portalCoverS_.ContainsKey(_sectionPath))
            {
                imgCover.transform.GetComponent<RawImage>().texture = portalCoverS_[_sectionPath];
                imgCover.gameObject.SetActive(true);
            }
            else
            {
                imgCover.gameObject.SetActive(false);
            }
        }

        private void createMenu(MyCatalog.Section _section)
        {
            logger_.Debug("create menu, path:{0}", _section.path);
            // navigation形态需要创建bundle的entry
            System.Action<PortalClone, MyCatalog.Section> createNavigation = (_portal, _section) =>
            {
                var template = _portal.bundleContainer.Find("templateBundle");
                if (null == template)
                {
                    logger_.Error("templateBundle not found");
                    return;
                }
                var clone = GameObject.Instantiate(template.gameObject, template.parent);
                clone.name = _section.path;
                clone.transform.Find("txtName").GetComponent<Text>().text = _section.name;
                clone.SetActive(true);
                _portal.menuS.Add(clone);

                string iconSource;
                if (!_section.kvS.TryGetValue("icon.source", out iconSource))
                    iconSource = "";
                string iconUri;
                if (!_section.kvS.TryGetValue("icon.uri", out iconUri))
                    iconUri = "";
                string cover;
                _section.kvS.TryGetValue("cover", out cover);
                if (!string.IsNullOrEmpty(cover))
                {
                    loadTextureFromTheme(cover, (_texture) =>
                    {
                        portalCoverS_[_section.path] = _texture;
                    }, () => { });
                }

                if (iconSource == "theme://")
                {
                    loadTextureFromTheme(iconUri, (_texture) =>
                    {
                        clone.transform.Find("imgIcon").GetComponent<RawImage>().texture = _texture;
                    }, () => { });
                }
                else if (iconSource == "assloud://")
                {
                    assetReader_.LoadTexture(iconUri, (_texture) =>
                    {
                        clone.transform.Find("imgIcon").GetComponent<RawImage>().texture = _texture;
                    }, () => { });
                }
                else
                {
                    logger_.Error("icon.source:{0} not supported");
                }

                // 监听事件
                clone.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _portal.panelContentDetail.gameObject.SetActive(false);
                    refreshContents(clone.name);
                    showPortalCover(_section.path, _portal.root);
                });
            };

            // filter形态只创建底部的过滤工具栏，entry全为content，且没有导航栏
            System.Action<PortalClone, MyCatalog.Section> createFilter = (_portal, _section) =>
            {
                var template = _portal.filterContainer.Find("templateTab");
                if (null == template)
                {
                    logger_.Error("templateFilter not found");
                    return;
                }
                var clone = GameObject.Instantiate(template.gameObject, template.parent);
                clone.name = _section.path;
                clone.transform.Find("Label").GetComponent<Text>().text = _section.name;
                clone.SetActive(true);
                // 监听事件
                clone.GetComponent<Toggle>().onValueChanged.AddListener((_toggled) =>
                {
                    if (!_toggled)
                        return;
                    _portal.panelContentDetail.gameObject.SetActive(false);
                    refreshContents(clone.name);
                });
            };

            // 查找对应的Portal的Section
            string[] strS = _section.path.Split('/');
            string portalPath = string.Format("/{0}", strS[1]);
            var portalSection = findSectionWithPath(portalPath);
            if (null == portalSection)
            {
                logger_.Error("section:{0} not found in catalog", portalPath);
                return;
            }

            PortalClone portalClone = null;
            portalCloneS.TryGetValue(portalPath, out portalClone);
            if (null == portalClone)
            {
                logger_.Error("clone of section:{0} not found", portalPath);
                return;
            }

            string pagination;
            if (!portalSection.kvS.TryGetValue("pagination", out pagination))
                pagination = "";
            if ("filter" == pagination)
            {
                createFilter(portalClone, _section);
            }
            else if ("navigation" == pagination)
            {
                createNavigation(portalClone, _section);
            }
            else
            {
                logger_.Error("pagination:{0} not supported", pagination);
                return;
            }
        }

        private void createContent(MyCatalog.Section _section, string _contentUri)
        {
            assetReader_.LoadText(_contentUri + "/meta.json", (_bytes) =>
            {
                ContentMetaSchema meta = null;
                try
                {
                    string json = System.Text.Encoding.UTF8.GetString(_bytes);
                    meta = JsonConvert.DeserializeObject<ContentMetaSchema>(json);
                }
                catch (System.Exception ex)
                {
                    logger_.Error("parse meta.json of uri:{0} happend error", _contentUri);
                    logger_.Exception(ex);
                    return;
                }
                // 查找对应的Portal的Section
                string[] strS = _section.path.Split('/');
                string portalPath = string.Format("/{0}", strS[1]);
                var portalSection = findSectionWithPath(portalPath);
                if (null == portalSection)
                {
                    logger_.Error("section:{0} not found in catalog", portalPath);
                    return;
                }

                PortalClone portalClone = null;
                portalCloneS.TryGetValue(portalPath, out portalClone);
                if (null == portalClone)
                {
                    logger_.Error("clone of section:{0} not found", portalPath);
                    return;
                }

                string shape;
                if (!portalSection.kvS.TryGetValue("shape", out shape))
                    shape = "";

                Transform template = portalClone.contentContainer.Find("templateContent");
                // 如果已经创建过
                if (null != portalClone.contentContainer.Find(meta.Uuid))
                    return;

                string uri = string.Format("{0}/{1}", meta.foreign_bundle_uuid, meta.Uuid);
                var clone = GameObject.Instantiate(template.gameObject, template.parent);
                clone.name = uri;
                clone.transform.Find("txtName").GetComponent<Text>().text = meta.name;
                clone.SetActive(portalClone.contentVisibleCreated);
                portalClone.contentS.Add(clone);
                assetReader_.LoadTexture(uri + "/icon.png", (_texture) =>
                {
                    clone.transform.Find("imgIcon").GetComponent<RawImage>().texture = _texture;
                }, () => { });
                clone.GetComponent<Button>().onClick.AddListener(() =>
                {
                    refreshContentDetail(portalClone, uri, clone.transform.Find("imgIcon").GetComponent<RawImage>().texture);
                });
            }, () => { });
        }

        private void refreshContents(string _sectionPath)
        {
            // 查找对应的Portal的Section
            string[] strS = _sectionPath.Split('/');
            string portalPath = string.Format("/{0}", strS[1]);
            var portalSection = findSectionWithPath(portalPath);
            if (null == portalSection)
            {
                logger_.Error("section:{0} not found in catalog", portalPath);
                return;
            }

            PortalClone portalClone = null;
            portalCloneS.TryGetValue(portalPath, out portalClone);
            if (null == portalClone)
            {
                logger_.Error("clone of section:{0} not found", portalPath);
                return;
            }

            // 隐藏全部菜单
            foreach (var obj in portalClone.menuS)
            {
                obj.SetActive(false);
            }
            // 隐藏全部内容
            foreach (var obj in portalClone.contentS)
            {
                obj.SetActive(false);
            }

            var menuSection = findSectionWithPath(_sectionPath);
            if (null == menuSection)
            {
                logger_.Error("section:{0} not found in catalog", menuSection);
                return;
            }

            Action<string> showContent = (_contentUri) =>
            {
                var found = portalClone.contentS.Find((_item) => { return _item.name == _contentUri; });
                if (null != found)
                    found.SetActive(true);
            };
            // 显示对应的content
            foreach (var content in menuSection.contentS)
            {
                if (content.EndsWith("/+"))
                {
                    string bundleUuid = content.Split("/")[0];
                    BundleMetaSchema meta = null;
                    bundleSchemaS_.TryGetValue(bundleUuid, out meta);
                    if (null != meta)
                    {
                        foreach (var contentUuid in meta.foreign_content_uuidS)
                        {
                            string contentUri = string.Format("{0}/{1}", meta.Uuid, contentUuid);
                            showContent(contentUri);
                        }
                    }
                    else
                    {
                        logger_.Error("meta of bundle:{0} is null", bundleUuid);
                    }
                }
                else
                {
                    showContent(content);
                }
            }

            // 仅当没显示过滤栏时显示导览栏
            portalClone.btnNav.transform.Find("txtName").GetComponent<Text>().text = menuSection.name;
            portalClone.btnNav.gameObject.SetActive(!portalClone.filterContainer.gameObject.activeSelf);
        }

        private void refreshContentDetail(PortalClone _portalClone, string _uri, Texture _icon)
        {
            _portalClone.activeContentMetaSchema = null;
            _portalClone.panelContentDetail.gameObject.SetActive(false);
            foreach (var obj in _portalClone.entryS)
            {
                obj.SetActive(false);
            }

            assetReader_.LoadText(_uri + "/meta.json", (_bytes) =>
            {
                ContentMetaSchema meta = null;
                try
                {
                    string json = System.Text.Encoding.UTF8.GetString(_bytes);
                    meta = JsonConvert.DeserializeObject<ContentMetaSchema>(json);
                }
                catch (Exception e)
                {
                    logger_.Error("parse meta.json of content:{0} happened error", _uri);
                    logger_.Exception(e);
                    return;
                }
                _portalClone.activeContentMetaSchema = meta;
                _portalClone.imgDetailIcon.texture = _icon;
                _portalClone.textDetailTitle.text = meta.title;
                _portalClone.textDetailLabel.text = meta.label;
                _portalClone.textDetailCaption.text = meta.caption;
                _portalClone.textDetailTopic.text = meta.topic;
                _portalClone.textDetailDescription.text = meta.description;
                _portalClone.panelContentDetail.gameObject.SetActive(true);
                // 显示有效的资源入口
                foreach (var entry in style_.entryS)
                {
                    string kvValue;
                    meta.kvS.TryGetValue(entry.kvKey, out kvValue);
                    if (string.IsNullOrEmpty(kvValue))
                        continue;
                    var found = _portalClone.entryS.Find((_item) =>
                    {
                        return _item.name == entry.kvKey;
                    });
                    if (!found)
                        return;
                    found.gameObject.SetActive(true);
                }
            }, () => { });
        }

        private MyCatalog.Section findSectionWithPath(string _path)
        {
            MyCatalog.Section foundSection = null;
            foreach (var section in catalog_.sectionS)
            {
                if (section.path == _path)
                {
                    foundSection = section;
                    break;
                }
            }
            return foundSection;
        }

        private IEnumerator delay(float _seconds, System.Action _action)
        {
            yield return new WaitForSeconds(_seconds);
            _action();
        }

    }
}
