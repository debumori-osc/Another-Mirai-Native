using Another_Mirai_Native.Adapter.CQCode;
using Another_Mirai_Native.Adapter.CQCode.Model;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Another_Mirai_Native.Adapter
{
    /// <summary>
    /// 辅助CQ码构建与解析的类
    /// </summary>
    public static class CQCodeBuilder
    {
        /// <summary>
        /// 消息链转CQ码
        /// </summary>
        /// <param name="chainMsg">从Mirai发送来的消息链</param>
        /// <returns>转换CQ码后的结果</returns>
        public static string Parse(List<MiraiMessageBase> chainMsg)
        {
            StringBuilder Result = new();
            foreach (var item in chainMsg)
            {
                switch (item.messageType)
                {
                    case MiraiMessageType.Source:
                        break;
                    case MiraiMessageType.Quote:
                        break;
                    case MiraiMessageType.At:
                        var at = (MiraiMessageTypeDetail.At)item;
                        Result.Append(CQApi.CQCode_At(at.target));
                        break;
                    case MiraiMessageType.AtAll:
                        Result.Append(CQApi.CQCode_AtAll());
                        break;
                    case MiraiMessageType.Face:
                        var face = (MiraiMessageTypeDetail.Face)item;
                        Result.Append(CQApi.CQCode_Face(face.faceId));
                        break;
                    case MiraiMessageType.Plain:
                        var plain = (MiraiMessageTypeDetail.Plain)item;
                        Result.Append(plain.text);
                        break;
                    case MiraiMessageType.Image:
                        var image = (MiraiMessageTypeDetail.Image)item;
                        string imgId = image.imageId.Replace("-", "").Replace("{", "").Replace("}", "").Split('.').First();
                        Directory.CreateDirectory("data/image");
                        File.WriteAllText($"data/image/{imgId}.cqimg", $"[image]\nmd5=0\nsize=0\nurl={image.url}");
                        Result.Append($"[CQ:image,file={imgId}]");
                        break;
                    case MiraiMessageType.FlashImage:
                        var flashImage = (MiraiMessageTypeDetail.FlashImage)item;
                        string flashImgId = flashImage.imageId.Replace("-", "").Replace("{", "").Replace("}", "").Split('.').First();
                        Directory.CreateDirectory("data/image");
                        File.WriteAllText($"data/image/{flashImgId}.cqimg", $"[image]\nmd5=0\nsize=0\nurl={flashImage.url}");
                        Result.Append($"[CQ:image,file={flashImgId},flash=true]");
                        break;
                    case MiraiMessageType.Voice:
                        var voice = (MiraiMessageTypeDetail.Voice)item;
                        string voiceId = voice.voiceId.Replace(".amr", "");
                        Directory.CreateDirectory("data/record");
                        File.WriteAllText($"data/image/{voiceId}.cqrecord", $"[record]\nurl={voice.url}");
                        Result.Append($"[CQ:record,file={voice.voiceId}]");
                        break;
                    case MiraiMessageType.Xml:
                        var xml = (MiraiMessageTypeDetail.Xml)item;
                        Result.Append($"[CQ:rich,type=xml,content={xml.xml}]");
                        break;
                    case MiraiMessageType.Json:
                        var json = (MiraiMessageTypeDetail.Json)item;
                        Result.Append($"[CQ:rich,type=json,content={json.json}]");
                        break;
                    case MiraiMessageType.App:
                        var app = (MiraiMessageTypeDetail.App)item;
                        Result.Append($"[CQ:rich,type=app,content={app.content}]");
                        break;
                    case MiraiMessageType.Poke:
                        var poke = (MiraiMessageTypeDetail.Poke)item;
                        Result.Append($"[CQ:poke,name={poke.name},]");
                        break;
                    case MiraiMessageType.Dice:
                        var dice = (MiraiMessageTypeDetail.Dice)item;
                        Result.Append($"[CQ:dice,point={dice.value}]");
                        break;
                    case MiraiMessageType.MarketFace:
                        var marketFace = (MiraiMessageTypeDetail.MarketFace)item;
                        Result.Append($"[CQ:bigface,id={marketFace.id}]");
                        break;
                    case MiraiMessageType.MusicShare:
                        var musicShare = (MiraiMessageTypeDetail.MusicShare)item;
                        Result.Append(CQApi.CQCode_DIYMusic(musicShare.jumpUrl, musicShare.musicUrl, musicShare.title, musicShare.brief, musicShare.pictureUrl));
                        break;
                    case MiraiMessageType.Forward:
                    case MiraiMessageType.File:
                    case MiraiMessageType.MiraiCode:
                    default:
                        break;
                }
            }
            return Result.ToString();
        }
        /// <summary>
        /// CQ码转消息链
        /// </summary>
        /// <param name="message">CQ码文本</param>
        /// <returns>转换后的消息链数组</returns>
        public static List<MiraiMessageBase> BuildMessageChains(string message)
        {
            List<MiraiMessageBase> result = new();
            var list = CQCodeModel.Parse(message);// 通过工具函数提取所有的CQ码
            foreach (var item in list)
            {
                message = message.Replace(item.ToString(), "<!cqcode!>");// 将CQ码的位置使用占空文本替换
            }
            List<string> p = new();// 记录下文本与CQ码的位置关系
            string tmp = "";
            for (int i = 0; i < message.Length; i++)// 将消息中的CQ码与文本分离开
            {
                tmp += message[i];// 文本
                if(tmp == "<!cqcode!>")// 此消息中没有其他文本, 只有CQ码
                {
                    p.Add("<!cqcode!>");
                    tmp = "";
                }
                else if (tmp.EndsWith("<!cqcode!>"))// 消息以CQ码结尾
                {
                    p.Add(tmp[..^10]);// 记录文本位置
                    p.Add("<!cqcode!>");// 记录CQ码位置
                    tmp = "";
                }
            }
            if(tmp != "")// 文本中没有CQ码, 或不以CQ码结尾
                p.Add(tmp);
            int cqcode_index = 0;
            for (int i = 0; i < p.Count; i++)
            {
                MiraiMessageBase messageBase;
                if (p[i] == "<!cqcode!>")
                {
                    messageBase = ParseCQCode2MiraiMessageBase(list[cqcode_index]);// 将CQ码转换为消息链对象
                    cqcode_index++;
                    if (messageBase == null) continue;
                }
                else
                {
                    messageBase = new MiraiMessageTypeDetail.Plain { text = p[i] };// 将文本转换为消息链对象
                }
                result.Add(messageBase);
            }
            return result;
        }
        /// <summary>
        /// 将CQ码转换为消息链对象
        /// </summary>
        /// <param name="cqcode">需要转换的CQ码对象</param>
        private static MiraiMessageBase ParseCQCode2MiraiMessageBase(CQCodeModel cqcode)
        {
            switch (cqcode.Function)
            {
                case CQCode.Enum.CQFunction.Face:
                    return new MiraiMessageTypeDetail.Face { faceId = Convert.ToInt32(cqcode.Items["id"]) };
                case CQCode.Enum.CQFunction.Bface:
                    return new MiraiMessageTypeDetail.MarketFace { id = Convert.ToInt32(cqcode.Items["id"]) };
                case CQCode.Enum.CQFunction.Image:
                    string picPath = cqcode.Items["file"];
                    // 以下为两个纠错路径, 防止拼接路径时出现以下两种情况
                    // basePath + "\foo.jpg"
                    // basePath + "foo.jpg"
                    string picPathA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image") + picPath;
                    string picPathB = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", picPath);
                    if (File.Exists(picPathA))
                    {
                        picPath = picPathA;
                    }
                    else if (File.Exists(picPathB))
                    {
                        picPath = picPathB;
                    }
                    else
                    {
                        // 若以上两个路径均不存在, 判断对应的cqimg文件是否存在
                        if(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", picPath + ".cqimg")))
                        {
                            LogHelper.WriteLog(LogLevel.Warning, "发送图片", "文件不存在", "");
                            return null;
                        }
                        string picTmp = File.ReadAllText(picPath + ".cqimg");      
                        // 分离cqimg文件中的url
                        picTmp = picTmp.Split('\n').Last().Replace("url=", "");
                        if (cqcode.Items.ContainsKey("flash"))
                        {
                            return new MiraiMessageTypeDetail.FlashImage { url = picTmp };
                        }
                        return new MiraiMessageTypeDetail.Image { url = picTmp };
                    }
                    // 将图片转换为base64
                    string picBase64 = Helper.ParsePic2Base64(picPath);
                    if(string.IsNullOrEmpty(picBase64))
                    {
                        return null;
                    }
                    if (cqcode.Items.ContainsKey("flash"))
                    {
                        return new MiraiMessageTypeDetail.FlashImage { base64 = picBase64 };
                    }
                    return new MiraiMessageTypeDetail.Image { base64 = picBase64 };
                case CQCode.Enum.CQFunction.Record:
                    // TODO: 音频支持
                    string recordPath = cqcode.Items["file"];
                    recordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, recordPath);
                    return new MiraiMessageTypeDetail.Voice { path = recordPath };
                case CQCode.Enum.CQFunction.At:
                    return new MiraiMessageTypeDetail.At { target = Convert.ToInt64(cqcode.Items["qq"]), display = "" };
                case CQCode.Enum.CQFunction.Dice:
                    return new MiraiMessageTypeDetail.Dice { value = Convert.ToInt32(cqcode.Items["point"]) };
                case CQCode.Enum.CQFunction.Music:
                    return new MiraiMessageTypeDetail.MusicShare { brief = cqcode.Items["content"], jumpUrl = cqcode.Items["url"], musicUrl = cqcode.Items["audio"], pictureUrl = cqcode.Items["imageUrl"], title = cqcode.Items["title"] };
                case CQCode.Enum.CQFunction.Rich:
                    return (object)cqcode.Items["type"] switch
                    {
                        "xml" => new MiraiMessageTypeDetail.Xml { xml = cqcode.Items["content"] },
                        "json" => new MiraiMessageTypeDetail.Json { json = cqcode.Items["content"] },
                        "app" => new MiraiMessageTypeDetail.App { content = cqcode.Items["content"] },
                        _ => null,
                    };
                default:
                    return null;
            }
        }
        /// <summary>
        /// 将Mirai发送的json数组, 转换为消息链对象
        /// </summary>
        /// <param name="json">json数组</param>
        public static List<MiraiMessageBase> ParseJArray2MiraiMessageBaseList(JArray json)
        {
            List<MiraiMessageBase> chainMsg = new();
            foreach (var item in json)
            {
                MiraiMessageType msgType = Helper.String2Enum<MiraiMessageType>(item["type"].ToString());
                switch (msgType)
                {
                    case MiraiMessageType.Source:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Source>());
                        break;
                    case MiraiMessageType.Quote:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Quote>());
                        break;
                    case MiraiMessageType.At:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.At>());
                        break;
                    case MiraiMessageType.AtAll:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.AtAll>());
                        break;
                    case MiraiMessageType.Face:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Face>());
                        break;
                    case MiraiMessageType.Plain:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Plain>());
                        break;
                    case MiraiMessageType.Image:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Image>());
                        break;
                    case MiraiMessageType.FlashImage:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.FlashImage>());
                        break;
                    case MiraiMessageType.Voice:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Voice>());
                        break;
                    case MiraiMessageType.Xml:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Xml>());
                        break;
                    case MiraiMessageType.Json:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Json>());
                        break;
                    case MiraiMessageType.App:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.App>());
                        break;
                    case MiraiMessageType.Poke:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Poke>());
                        break;
                    case MiraiMessageType.Dice:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Dice>());
                        break;
                    case MiraiMessageType.MarketFace:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MarketFace>());
                        break;
                    case MiraiMessageType.MusicShare:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MusicShare>());
                        break;
                    case MiraiMessageType.Forward:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Forward>());
                        break;
                    case MiraiMessageType.File:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.File>());
                        break;
                    case MiraiMessageType.MiraiCode:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MiraiCode>());
                        break;
                    default:
                        break;
                }
            }
            return chainMsg;
        }
    }
}
