
using System.Xml.Serialization;

namespace XTC.FMP.MOD.WingMenu.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class FontOptions
        {
            [XmlAttribute("fontColor")]
            public string fontColor { get; set; } = "#FFFFFFFF";
            [XmlAttribute("fontSize")]
            public int fontSize { get; set; } = 16;
        }

        public class Entry
        {
            [XmlAttribute("icon")]
            public string icon { get; set; } = "";
            [XmlAttribute("text")]
            public string text { get; set; } = "";
            [XmlAttribute("kvKey")]
            public string kvKey { get; set; } = "";
            [XmlArray("SubjectS"), XmlArrayItem("Subject")]
            public Subject[] subjectS = new Subject[0];
        }

        public class MiddlePanel
        {
            [XmlAttribute("color")]
            public string color { get; set; } = "#33333333";
            [XmlAttribute("decal")]
            public string decal { get; set; } = "";
        }

        public class EdgePanel
        {
            [XmlAttribute("color")]
            public string color { get; set; } = "#33333333";
        }

        public class ContentDetail
        {
            [XmlElement("Title")]
            public FontOptions title { get; set; } = new FontOptions();
            [XmlElement("Label")]
            public FontOptions label { get; set; } = new FontOptions();
            [XmlElement("Caption")]
            public FontOptions caption { get; set; } = new FontOptions();
            [XmlElement("Topic")]
            public FontOptions topic { get; set; } = new FontOptions();
            [XmlElement("Description")]
            public FontOptions description { get; set; } = new FontOptions();
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";
            [XmlElement("BundleCell")]
            public FontOptions bundleCell { get; set; } = new FontOptions();
            [XmlElement("ContentCell")]
            public FontOptions contentCell { get; set; } = new FontOptions();
            [XmlElement("EntryCell")]
            public FontOptions entryCell { get; set; } = new FontOptions();
            [XmlElement("Navigation")]
            public FontOptions navigation { get; set; } = new FontOptions();
            [XmlElement("Filter")]
            public FontOptions filter { get; set; } = new FontOptions();
            [XmlElement("MiddlePanel")]
            public MiddlePanel middlePanel { get; set; } = new MiddlePanel();
            [XmlElement("EdgePanel")]
            public EdgePanel edgePanel { get; set; } = new EdgePanel();
            [XmlElement("ContentDetail")]
            public ContentDetail contentDetail { get; set; } = new ContentDetail();
            [XmlArray("EntryS"), XmlArrayItem("Entry")]
            public Entry[] entryS { get; set; } = new Entry[0];
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

