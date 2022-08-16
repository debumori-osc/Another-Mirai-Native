using Another_Mirai_Native.Adapter.CQCode;
using Another_Mirai_Native.Enums;
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
        public static string Parse (List<MiraiMessageBase> chainMsg)
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
                        string imgId = image.imageId.Replace("-", "");
                        Directory.CreateDirectory("data/image");
                        File.WriteAllText($"data/image/{imgId}.cqimg", $"[image]\nmd5=0\nsize=0\nurl={image.url}");
                        Result.Append($"[CQ:image,file={imgId}]");
                        break;
                    case MiraiMessageType.FlashImage:
                        var flashImage = (MiraiMessageTypeDetail.FlashImage)item;
                        string flashImgId = flashImage.imageId.Replace("-", "");
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
                        break;
                    case MiraiMessageType.File:
                        var file = (MiraiMessageTypeDetail.File)item;
                        Result.Append($"[CQ:file,]");
                        break;
                    case MiraiMessageType.MiraiCode:
                        break;
                    default:
                        break;
                }
            }
            return Result.ToString();
        }
    }
}
