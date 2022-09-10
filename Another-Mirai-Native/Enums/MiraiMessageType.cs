using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Enums
{
    public interface MiraiMessageBase
    {
        string type { get; set; }
        MiraiMessageType messageType { get; set; }
    }
    public enum MiraiMessageType
    {
        Source,
        Quote,
        At,
        AtAll,
        Face,
        Plain,
        Image,
        FlashImage,
        Voice,
        Xml,
        Json,
        App,
        Poke,
        Dice,
        MarketFace,
        MusicShare,
        Forward,
        File,
        MiraiCode
    }
    public class MiraiMessageTypeDetail
    {
        public class Source : MiraiMessageBase
        {
            public string type { get; set; } = "Source";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Source;
            public long id { get; set; }
            public long time { get; set; }
        }

        public class Quote : MiraiMessageBase
        {
            public string type { get; set; } = "Quote";
            public long id { get; set; }
            public long groupId { get; set; }
            public long senderId { get; set; }
            public long targetId { get; set; }
            public Origin[] origin { get; set; }
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Quote;

            public class Origin
            {
                public string type { get; set; }
                public string text { get; set; }
            }
        }

        public class At : MiraiMessageBase
        {
            public string type { get; set; } = "At";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.At;
            public long target { get; set; }
            public string display { get; set; }
        }

        public class AtAll : MiraiMessageBase
        {
            public string type { get; set; } = "AtAll";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.AtAll;
        }

        public class Face : MiraiMessageBase
        {
            public string type { get; set; } = "Face";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Face;
            public int faceId { get; set; }
            public string name { get; set; }
        }

        public class Plain : MiraiMessageBase
        {
            public string type { get; set; } = "Plain";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Plain;
            public string text { get; set; }
        }

        public class Image : MiraiMessageBase
        {
            public string type { get; set; } = "Image";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Image;
            public string imageId { get; set; }
            public string url { get; set; }
            public string path { get; set; }
            public string base64 { get; set; }
        }

        public class FlashImage : MiraiMessageBase
        {
            public string type { get; set; } = "FlashImage";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.FlashImage;
            public string imageId { get; set; }
            public string url { get; set; }
            public string path { get; set; }
            public string base64 { get; set; }
        }

        public class Voice : MiraiMessageBase
        {
            public string type { get; set; } = "Voice";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Voice;
            public string voiceId { get; set; }
            public string url { get; set; }
            public string path { get; set; }
            public string base64 { get; set; }
            public long length { get; set; }
        }

        public class Xml : MiraiMessageBase
        {
            public string type { get; set; } = "Xml";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Xml;
            public string xml { get; set; }
        }

        public class Json : MiraiMessageBase
        {
            public string type { get; set; } = "Json";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Json;
            public string json { get; set; }
        }

        public class App : MiraiMessageBase
        {
            public string type { get; set; } = "App";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.App;
            public string content { get; set; }
        }

        public class Poke : MiraiMessageBase
        {
            public string type { get; set; } = "Poke";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Poke;
            public string name { get; set; }
        }

        public class Dice : MiraiMessageBase
        {
            public string type { get; set; } = "Dice";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Dice;
            public long value { get; set; }
        }

        public class MarketFace : MiraiMessageBase
        {
            public string type { get; set; } = "MarketFace";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.MarketFace;
            public long id { get; set; }
            public string name { get; set; }
        }

        public class MusicShare : MiraiMessageBase
        {
            public string type { get; set; } = "MusicShare";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.MusicShare;
            public string kind { get; set; }
            public string title { get; set; }
            public string summary { get; set; }
            public string jumpUrl { get; set; }
            public string pictureUrl { get; set; }
            public string musicUrl { get; set; }
            public string brief { get; set; }
        }

        public class Forward : MiraiMessageBase
        {
            public string type { get; set; } = "Forward";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.Forward;
            public Nodelist[] nodeList { get; set; }
            public class Nodelist
            {
                public long senderId { get; set; }
                public long time { get; set; }
                public string senderName { get; set; }
                public object[] messageChain { get; set; }
                public long messageId { get; set; }
            }
        }

        public class File : MiraiMessageBase
        {
            public string type { get; set; } = "File";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.File;
            public string id { get; set; }
            public string name { get; set; }
            public long size { get; set; }
        }

        public class MiraiCode : MiraiMessageBase
        {
            public string type { get; set; } = "MiraiCode";
            public MiraiMessageType messageType { get; set; } = MiraiMessageType.MiraiCode;
            public string code { get; set; }
        }
    }
}
