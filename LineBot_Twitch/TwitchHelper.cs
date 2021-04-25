using LineBot_Twitch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LineBot_Twitch
{
    public class TwitchHelper
    {
        public static async Task<string> RefreshTokenAsync(string userID)
        {
            var redis = new RedisHelper();
            var twitchToken = redis.GetData(userID);
            var tokenOri = JsonConvert.DeserializeObject<TwitchToken>(twitchToken);

            var http = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Post, "https://id.twitch.tv/oauth2/token");

            // This is the important part:
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", "9vijl1a2mn05drkmhlg7b0bmb5gl3r" },
                    { "client_secret", "hus3zfndvcix8u2mgmps9ajsa4e30b" },
                    { "refresh_token", tokenOri.refresh_token },
                    { "grant_type", "refresh_token" }
                });

            HttpResponseMessage resp = await http.SendAsync(req);

            var response = resp.Content.ReadAsStringAsync().Result;

            var tokenNew = JsonConvert.DeserializeObject<TwitchToken>(response);

            //把登入後的資訊全都存進去，之後要刷新TOKEN比較方便
            redis.SetData(userID, response);

            return tokenNew.access_token;
        }

        public static string model = @"{
                                    ""type"": ""bubble"",
                                    ""hero"": {
                                    ""type"": ""image"",
                                    ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/01_1_cafe.png"",
                                    ""size"": ""full"",
                                    ""aspectRatio"": ""20:13"",
                                    ""aspectMode"": ""cover"",
                                    ""action"": {
                                        ""type"": ""uri"",
                                        ""uri"": ""http://linecorp.com/""
                                    }
                                    },
                                    ""body"": {
                                    ""type"": ""box"",
                                    ""layout"": ""vertical"",
                                    ""contents"": [
                                        {
                                        ""type"": ""text"",
                                        ""text"": ""實況主名稱"",
                                        ""weight"": ""bold"",
                                        ""size"": ""xl""
                                        },
                                        {
                                        ""type"": ""text"",
                                        ""text"": ""實況標題"",
                                        ""size"": ""md""
                                        },
                                        {
                                        ""type"": ""box"",
                                        ""layout"": ""vertical"",
                                        ""margin"": ""lg"",
                                        ""spacing"": ""sm"",
                                        ""contents"": [
                                            {
                                            ""type"": ""box"",
                                            ""layout"": ""baseline"",
                                            ""spacing"": ""sm"",
                                            ""contents"": [
                                                {
                                                ""type"": ""text"",
                                                ""text"": ""分類"",
                                                ""color"": ""#aaaaaa"",
                                                ""size"": ""sm"",
                                                ""flex"": 1
                                                },
                                                {
                                                ""type"": ""text"",
                                                ""text"": ""英雄聯盟"",
                                                ""wrap"": true,
                                                ""color"": ""#666666"",
                                                ""size"": ""sm"",
                                                ""flex"": 5
                                                }
                                            ]
                                            },
                                            {
                                            ""type"": ""box"",
                                            ""layout"": ""baseline"",
                                            ""spacing"": ""sm"",
                                            ""contents"": [
                                                {
                                                ""type"": ""text"",
                                                ""text"": ""人數"",
                                                ""color"": ""#aaaaaa"",
                                                ""size"": ""sm"",
                                                ""flex"": 1
                                                },
                                                {
                                                ""type"": ""text"",
                                                ""text"": ""10000000"",
                                                ""wrap"": true,
                                                ""color"": ""#666666"",
                                                ""size"": ""sm"",
                                                ""flex"": 5
                                                }
                                            ]
                                            }
                                        ]
                                        }
                                    ]
                                    },
                                    ""footer"": {
                                    ""type"": ""box"",
                                    ""layout"": ""vertical"",
                                    ""spacing"": ""sm"",
                                    ""contents"": [
                                        {
                                        ""type"": ""button"",
                                        ""style"": ""link"",
                                        ""height"": ""sm"",
                                        ""action"": {
                                            ""type"": ""uri"",
                                            ""label"": ""前往實況"",
                                            ""uri"": ""https://linecorp.com""
                                        }
                                        }
                                    ],
                                    ""flex"": 0
                                    }
                                }";


        public static string test = @"[{
  ""type"": ""bubble"",
  ""hero"": {
    ""type"": ""image"",
    ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/01_1_cafe.png"",
    ""size"": ""full"",
    ""aspectRatio"": ""20:13"",
    ""aspectMode"": ""cover"",
    ""action"": {
      ""type"": ""uri"",
      ""uri"": ""http://linecorp.com/""
    }
  },
  ""body"": {
    ""type"": ""box"",
    ""layout"": ""vertical"",
    ""contents"": [
      {
        ""type"": ""text"",
        ""text"": ""Brown Cafe"",
        ""weight"": ""bold"",
        ""size"": ""xl""
      },
      {
        ""type"": ""box"",
        ""layout"": ""baseline"",
        ""margin"": ""md"",
        ""contents"": [
          {
            ""type"": ""icon"",
            ""size"": ""sm"",
            ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/review_gold_star_28.png""
          },
          {
            ""type"": ""icon"",
            ""size"": ""sm"",
            ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/review_gold_star_28.png""
          },
          {
            ""type"": ""icon"",
            ""size"": ""sm"",
            ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/review_gold_star_28.png""
          },
          {
            ""type"": ""icon"",
            ""size"": ""sm"",
            ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/review_gold_star_28.png""
          },
          {
            ""type"": ""icon"",
            ""size"": ""sm"",
            ""url"": ""https://scdn.line-apps.com/n/channel_devcenter/img/fx/review_gray_star_28.png""
          },
          {
            ""type"": ""text"",
            ""text"": ""4.0"",
            ""size"": ""sm"",
            ""color"": ""#999999"",
            ""margin"": ""md"",
            ""flex"": 0
          }
        ]
      },
      {
        ""type"": ""box"",
        ""layout"": ""vertical"",
        ""margin"": ""lg"",
        ""spacing"": ""sm"",
        ""contents"": [
          {
            ""type"": ""box"",
            ""layout"": ""baseline"",
            ""spacing"": ""sm"",
            ""contents"": [
              {
                ""type"": ""text"",
                ""text"": ""Place"",
                ""color"": ""#aaaaaa"",
                ""size"": ""sm"",
                ""flex"": 1
              },
              {
                ""type"": ""text"",
                ""text"": ""Miraina Tower, 4-1-6 Shinjuku, Tokyo"",
                ""wrap"": true,
                ""color"": ""#666666"",
                ""size"": ""sm"",
                ""flex"": 5
              }
            ]
          },
          {
            ""type"": ""box"",
            ""layout"": ""baseline"",
            ""spacing"": ""sm"",
            ""contents"": [
              {
                ""type"": ""text"",
                ""text"": ""Time"",
                ""color"": ""#aaaaaa"",
                ""size"": ""sm"",
                ""flex"": 1
              },
              {
                ""type"": ""text"",
                ""text"": ""10:00 - 23:00"",
                ""wrap"": true,
                ""color"": ""#666666"",
                ""size"": ""sm"",
                ""flex"": 5
              }
            ]
          }
        ]
      }
    ]
  },
  ""footer"": {
    ""type"": ""box"",
    ""layout"": ""vertical"",
    ""spacing"": ""sm"",
    ""contents"": [
      {
        ""type"": ""button"",
        ""style"": ""link"",
        ""height"": ""sm"",
        ""action"": {
          ""type"": ""uri"",
          ""label"": ""CALL"",
          ""uri"": ""https://linecorp.com""
        }
      },
      {
        ""type"": ""button"",
        ""style"": ""link"",
        ""height"": ""sm"",
        ""action"": {
          ""type"": ""uri"",
          ""label"": ""WEBSITE"",
          ""uri"": ""https://linecorp.com""
        }
      },
      {
        ""type"": ""spacer"",
        ""size"": ""sm""
      }
    ],
    ""flex"": 0
  }
}]";
    }
}
