using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LineBot_Twitch.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using isRock.LineBot;
using System.IO;
using TwitchLib.Api;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using TwitchLib.Api.Core.Exceptions;

namespace LineBot_Twitch.Controllers
{
    public class HomeController : LineWebHookControllerBase
    {

        private readonly IConfiguration _config;
        private readonly string _UserID;
        private readonly string _redisConnectionstring;
        private readonly ILogger _logger;

        public HomeController(IConfiguration config, ILogger<HomeController> log)
        {
            _config = config;
            _UserID = _config["Line:UserId"];
            _logger = log;
            this.ChannelAccessToken = _config["Line:ChannelAccessToken"];
            _redisConnectionstring = _config["Redis:Connectionstring"];
            RedisHelper.Init(_redisConnectionstring);
        }
        [HttpPost]
        public IActionResult LineBotTest()
        {
            try
            {
                var bot = new isRock.LineBot.Bot(ChannelAccessToken);
                //push text
                bot.PushMessage(_UserID, "Hello World");

            }
            catch (Exception ex)
            {

            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> LineBotReply()
        {
            //取得Line Event
            var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
            var responseMsg = "";
            try
            {
                var userID = LineEvent.source.userId;
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000")
                {
                    return Ok();
                }
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message")
                {
                    //判斷是不是設定的指令
                    if (LineEvent.message.text == "登入")
                    {
                        string twitchRedirect = _config["Twitch:AuthRedirect"];
                        var twitchLogin = twitchRedirect + $"&state={userID}";
                        responseMsg = twitchLogin;
                        this.ReplyMessage(LineEvent.replyToken, responseMsg);
                    }

                    if (LineEvent.message.text == "追隨")
                    {
                        string twitchToken = await TwitchHelper.RefreshTokenAsync(userID);
                        TwitchAPI api;

                        api = new TwitchAPI();
                        api.Settings.ClientId = _config["Twitch:ClientID"];
                        api.Settings.AccessToken = twitchToken;


                        var user = await api.Helix.Users.GetUsersAsync();
                        var userid = user.Users.First().Id;

                        var followers = await api.V5.Users.GetUserFollowsAsync(userid, 100, 0, "desc", "last_broadcast");

                        var followerOnStream = await api.V5.Streams.GetLiveStreamsAsync(followers.Follows.Select(item => item.Channel.Id).ToList());
                        //建立CarouselTemplate
                        var CarouselTemplate = new CarouselTemplate();
                        var count = 0;//建立CarouselTemplate只能有10個column;




                        var flexModels = new List<LineFlexModel>();
                        var carouselContent = new List<CarouselContent>();
                        foreach (var follower in followerOnStream.Streams)
                        {
                            if (count == 10)
                            {
                                //重置
                                carouselContent = new List<CarouselContent>();
                                count = 0;
                            }
                            //開新的
                            if (count == 10 || count == 0)
                            {
                                flexModels.Add(new LineFlexModel()
                                {
                                    altText = "123",
                                    contents = new FlexContent()
                                    {
                                        type = "carousel",
                                        contents = carouselContent
                                    },
                                    type = "flex",
                                });
                            }

                            carouselContent.Add(new CarouselContent()
                            {
                                type = "bubble",
                                hero = new Hero()
                                {
                                    action = new LineAction()
                                    {
                                        type = "uri",
                                        uri = follower.Channel.Url
                                    },
                                    aspectMode = "cover",
                                    aspectRatio = "20:13",
                                    size = "full",
                                    type = "image",
                                    url = follower.Preview.Large
                                },
                                body = new Body()
                                {
                                    contents = new List<BodyContent>()
                                    {
                                        //實況主名稱
                                        new BodyContentText()
                                        {
                                            size = "xl",
                                            text = follower.Channel.Name,
                                            type = "text",
                                            weight = "bold",
                                            color = "#000000",
                                            flex = 0
                                        },
                                        //實況標題
                                        new BodyContentText()
                                        {
                                            size = "md",
                                            //text = follower.Channel.Status.Length < 60 ? follower.Channel.Status : follower.Channel.Status.Substring(0, 60),
                                            text = follower.Channel.Status,
                                            type = "text",
                                            weight = "regular",
                                            color = "#000000",
                                            flex = 0
                                        },
                                        //sub
                                        new BodyContentBox()
                                        {
                                            type = "box",
                                            layout = "vertical",
                                            margin = "lg",
                                            spacing = "sm",
                                            contents = new List<BodyContent>()
                                            {
                                                new BodyContentBox()
                                                {
                                                    type = "box",
                                                    layout = "baseline",
                                                    spacing = "sm",
                                                    margin = "none",
                                                    contents = new List<BodyContent>()
                                                    {
                                                        new BodyContentText()
                                                        {
                                                            type = "text",
                                                            color = "#aaaaaa",
                                                            flex = 1,
                                                            size = "sm",
                                                            text = "分類",
                                                            weight = "regular"
                                                        },
                                                        new BodyContentText()
                                                        {
                                                            type= "text",
                                                            flex = 5,
                                                            color = "#666666",
                                                            size = "sm",
                                                            text = follower.Channel.Game,
                                                            weight = "regular"
                                                        }
                                                    }
                                                },
                                                new BodyContentBox()
                                                {
                                                    type = "box",
                                                    layout = "baseline",
                                                    spacing = "sm",
                                                    margin = "none",
                                                    contents = new List<BodyContent>()
                                                    {
                                                        new BodyContentText()
                                                        {
                                                            type = "text",
                                                            color = "#aaaaaa",
                                                            flex = 1,
                                                            size = "sm",
                                                            text = "人數",
                                                            weight = "regular"
                                                        },
                                                        new BodyContentText()
                                                        {
                                                            type= "text",
                                                            flex = 5,
                                                            color = "#666666",
                                                            size = "sm",
                                                            text = follower.Viewers.ToString(),
                                                            weight = "regular"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    layout = "vertical",
                                    type = "box"
                                }
                            });
                            count++;
                        }
                        string jsonReslt = JsonConvert.SerializeObject(flexModels);

                        this.ReplyMessageWithJSON(LineEvent.replyToken, jsonReslt);
                    }
                }
                else
                {
                    responseMsg = $"收到 event : {LineEvent.type} ";
                }
            }
            catch (TokenExpiredException ex)
            {

            }
            catch (InvalidCredentialException ex)
            {

            }
            catch (Exception ex)
            {
                responseMsg = ex.Message;
            }
            //this.ReplyMessage(LineEvent.replyToken, responseMsg);
            return Ok();
        }

        //授權網址:https://id.twitch.tv/oauth2/authorize?response_type=code&client_id=9vijl1a2mn05drkmhlg7b0bmb5gl3r&redirect_uri=https://localhost:44342/Home/TwitchRedirect&scope=viewing_activity_read
        //要注意https、用local的話port也要對
        /// <summary>
        /// Twitch登入
        /// </summary>
        /// <param name="code">Twitch授權</param>
        /// <param name="state">Line-OAuth</param>
        /// <returns></returns>
        public async Task<IActionResult> TwitchRedirect(string code, string state)
        {
            try
            {
                var user = new
                {
                    client_id = "9vijl1a2mn05drkmhlg7b0bmb5gl3r",
                    client_secret = "hus3zfndvcix8u2mgmps9ajsa4e30b",
                    code = code,
                    grant_type = "authorization_code",
                    redirect_uri = state
                };

                var http = new HttpClient();

                var req = new HttpRequestMessage(HttpMethod.Post, "https://id.twitch.tv/oauth2/token");

                // This is the important part:
                req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", "9vijl1a2mn05drkmhlg7b0bmb5gl3r" },
                    { "client_secret", "hus3zfndvcix8u2mgmps9ajsa4e30b" },
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri","https://linebottwitch20210410094254.azurewebsites.net/Home/TwitchRedirect" }
                });

                HttpResponseMessage resp = await http.SendAsync(req);

                var response = resp.Content.ReadAsStringAsync().Result;

                var token = JsonConvert.DeserializeObject<TwitchToken>(response);

                var redis = new RedisHelper();

                //把登入後的資訊全都存進去，之後要刷新TOKEN比較方便
                redis.SetData(state, response);
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }
    }
}
