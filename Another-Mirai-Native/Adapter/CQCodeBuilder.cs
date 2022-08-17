using Another_Mirai_Native.Adapter.CQCode;
using Another_Mirai_Native.Adapter.CQCode.Expand;
using Another_Mirai_Native.Adapter.CQCode.Model;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter
{
    public static class CQCodeBuilder
    {
        /// <summary>
        /// 消息链转CQ码
        /// </summary>
        /// <param name="chainMsg"></param>
        /// <returns></returns>
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
        /// <param name="message"></param>
        /// <returns></returns>
        public static List<MiraiMessageBase> BuildMessageChains(string message)
        {
            List<MiraiMessageBase> result = new();
            var list = CQCodeModel.Parse(message);
            foreach (var item in list)
            {
                message = message.Replace(item.ToString(), "<!cqcode!>[$$]");
            }
            List<string> p = new();
            string tmp = "";
            for (int i = 0; i < message.Length; i++)
            {
                tmp += message[i];
                if (tmp.EndsWith("[$$]"))
                {
                    p.Add(tmp[..^4]);
                    tmp = "";
                }
            }
            p.Add(tmp);
            int cqcode_index = 0;
            for (int i = 0; i < p.Count; i++)
            {
                MiraiMessageBase messageBase;
                if (p[i] == "<!cqcode!>")
                {
                    messageBase = ParseCQCode2MiraiMessageBase(list[cqcode_index]);
                    cqcode_index++;
                }
                else
                {
                    messageBase = new MiraiMessageTypeDetail.Plain { text = p[i] };
                }
                result.Add(messageBase);
            }
            return result;
        }

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
                    picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", picPath);
                    if (!File.Exists(picPath))
                    {
                        string picTmp = File.ReadAllText(picPath + ".cqimg");
                        picTmp = picTmp.Split('\n').Last().Replace("url=", "");
                        if (cqcode.Items.ContainsKey("flash"))
                        {
                            return new MiraiMessageTypeDetail.FlashImage { url = picTmp };
                        }
                        return new MiraiMessageTypeDetail.Image { url = picTmp };
                    }
                    if (cqcode.Items.ContainsKey("flash"))
                    {
                        return new MiraiMessageTypeDetail.FlashImage { path = picPath };
                    }
                    return new MiraiMessageTypeDetail.Image { path = picPath };
                case CQCode.Enum.CQFunction.Record:
                    string recordPath = cqcode.Items["file"];
                    recordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, recordPath);
                    return new MiraiMessageTypeDetail.Voice { path = recordPath };
                case CQCode.Enum.CQFunction.At:
                    return new MiraiMessageTypeDetail.At { target = Convert.ToInt64(cqcode.Items["qq"]) };
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
