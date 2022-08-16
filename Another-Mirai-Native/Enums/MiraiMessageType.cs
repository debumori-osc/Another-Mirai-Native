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
            public string type { get; set; }
            public int id { get; set; }
            public int time { get; set; }
        }

        public class Quote : MiraiMessageBase
        {
            public string type { get; set; }
            public int id { get; set; }
            public int groupId { get; set; }
            public int senderId { get; set; }
            public long targetId { get; set; }
            public Origin[] origin { get; set; }
            public class Origin
            {
                public string type { get; set; }
                public string text { get; set; }
            }
        }

        public class At : MiraiMessageBase
        {
            public string type { get; set; }
            public int target { get; set; }
            public string display { get; set; }
        }

        public class AtAll : MiraiMessageBase
        {
            public string type { get; set; }
        }

        public class Face : MiraiMessageBase
        {
            public string type { get; set; }
            public int faceId { get; set; }
            public string name { get; set; }
        }

        public class Plain : MiraiMessageBase
        {
            public string type { get; set; }
            public string text { get; set; }
        }

        public class Image : MiraiMessageBase
        {
            public string type { get; set; }
            public string imageId { get; set; }
            public string url { get; set; }
            public object path { get; set; }
            public object base64 { get; set; }
        }

        public class FlashImage : MiraiMessageBase
        {
            public string type { get; set; }
            public string imageId { get; set; }
            public string url { get; set; }
            public object path { get; set; }
            public object base64 { get; set; }
        }

        public class Voice : MiraiMessageBase
        {
            public string type { get; set; }
            public string voiceId { get; set; }
            public string url { get; set; }
            public object path { get; set; }
            public object base64 { get; set; }
            public int length { get; set; }
        }

        public class Xml : MiraiMessageBase
        {
            public string type { get; set; }
            public string xml { get; set; }
        }

        public class Json : MiraiMessageBase
        {
            public string type { get; set; }
            public string json { get; set; }
        }

        public class App : MiraiMessageBase
        {
            public string type { get; set; }
            public string content { get; set; }
        }

        public class Poke : MiraiMessageBase
        {
            public string type { get; set; }
            public string name { get; set; }
        }

        public class Dice : MiraiMessageBase
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class MarketFace : MiraiMessageBase
        {
            public string type { get; set; }
            public int id { get; set; }
            public string name { get; set; }
        }

        public class MusicShare : MiraiMessageBase
        {
            public string type { get; set; }
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
            public string type { get; set; }
            public Nodelist[] nodeList { get; set; }
            public class Nodelist
            {
                public int senderId { get; set; }
                public int time { get; set; }
                public string senderName { get; set; }
                public object[] messageChain { get; set; }
                public int messageId { get; set; }
            }
        }

        public class File : MiraiMessageBase
        {
            public string type { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public int size { get; set; }
        }

        public class MiraiCode : MiraiMessageBase
        {
            public string type { get; set; }
            public string code { get; set; }
        }
    }
}
