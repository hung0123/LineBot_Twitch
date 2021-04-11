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

namespace LineBot_Twitch.Controllers
{
    public class HomeController : LineWebHookControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _UserID;
        private readonly ILogger _logger;

        public HomeController(IConfiguration config, ILogger<HomeController> log)
        {
            _config = config;
            _UserID = _config["Line:UserId"];
            _logger = log;
            this.ChannelAccessToken = _config["Line:ChannelAccessToken"];

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
            catch(Exception ex)
            {

            }
            return Ok();
        }
        [HttpPost]
        public IActionResult LineBotReply()
        {
            try
            {
                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000")
                {
                    return Ok();
                }
                var responseMsg = "";
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "image")
                {
                    var Bytes = this.GetUserUploadedContent(LineEvent.message.id);
                    //var ret = MakeRequest(endpoint, key, Bytes);
                    responseMsg = $"captions :  \n JSON: ";
                }
                else if (LineEvent.type.ToLower() == "message")
                {
                    responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                }
                else
                {
                    responseMsg = $"收到 event : {LineEvent.type} ";
                }
                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();

            }
            catch (Exception ex)
            {

            }
            return Ok();
        }
    }
}
