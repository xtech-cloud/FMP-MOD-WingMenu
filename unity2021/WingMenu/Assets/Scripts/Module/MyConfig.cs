
using System.Xml.Serialization;

namespace XTC.FMP.MOD.WingMenu.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class Entry
        {
            [XmlAttribute("icon")]
            public string icon { get; set; } = "";
            [XmlAttribute("text")]
            public string text { get; set; } = "";
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

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";
            [XmlElement("MiddlePanel")]
            public MiddlePanel middlePanel { get; set; } = new MiddlePanel();
            [XmlElement("EdgePanel")]
            public EdgePanel edgePanel { get; set; } = new EdgePanel();
            [XmlArray("EntryS"), XmlArrayItem("Entry")]
            public Entry[] entryS { get; set; } = new Entry[0];
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

