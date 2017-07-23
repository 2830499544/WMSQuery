using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP;
using JXCSIMC.WMSQuery.Bll;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.Entities;

namespace JXCSIMC.WMSQuery.UI.Controllers
{
    public class WeiXinController : Controller
    {
        public static readonly string Token = "75F83895DA9CFEC1";//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = "1ClG6WfYCKdG9XhrNZn3QXqo4Azzpex9cTgDvui449b";//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = "wx4df7108464fb86f1";//与微信公众账号后台的AppId设置保持一致，区分大小写。

        //[HttpGet]
        //public ActionResult Menu ()
        //{
        //    return View("Menu");
        //}

        //[HttpPost]
        public void Menu()
        {
            var accessToken = AccessTokenContainer.GetAccessToken(AppId);

            ButtonGroup bg = new ButtonGroup();

            //单击
            bg.button.Add(new SingleClickButton()
            {
                name = "单击测试",
                key = "OneClick",
                type = ButtonType.click.ToString(),//默认已经设为此类型，这里只作为演示
            });

            //二级菜单
            var subButton = new SubButton()
            {
                name = "二级菜单"
            };
            subButton.sub_button.Add(new SingleClickButton()
            {
                key = "SubClickRoot_Text",
                name = "返回文本"
            });
            subButton.sub_button.Add(new SingleClickButton()
            {
                key = "SubClickRoot_News",
                name = "返回图文"
            });
            subButton.sub_button.Add(new SingleClickButton()
            {
                key = "SubClickRoot_Music",
                name = "返回音乐"
            });
            subButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://weixin.senparc.com",
                name = "Url跳转"
            });
            bg.button.Add(subButton);
            var result = CommonApi.CreateMenu(accessToken, bg);
            //return result;
        }



        [HttpGet]
        
        public ActionResult Index(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                Menu();
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + Senparc.Weixin.MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }


        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        public ActionResult Index(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel);//接收消息

            messageHandler.Execute();//执行微信处理过程

            return new FixWeixinBugWeixinResult(messageHandler);//返回结果
        }
    }
}