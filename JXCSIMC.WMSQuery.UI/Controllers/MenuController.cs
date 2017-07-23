using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP.AdvancedAPIs.UserTag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace JXCSIMC.WMSQuery.UI.Controllers
{
    public class MenuController : Controller
    {
        public static readonly string Token = "75F83895DA9CFEC1";//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = "1ClG6WfYCKdG9XhrNZn3QXqo4Azzpex9cTgDvui449b";//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = "wx4df7108464fb86f1";//与微信公众账号后台的AppId设置保持一致，区分大小写。


        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuManager()

        {
            AccessTokenContainer.Register("wx4df7108464fb86f1", "46515264f29685ba478ea93d44919ed5");
            var accessToken = AccessTokenContainer.GetAccessToken(AppId);


            var aa = UserTagApi.Get(accessToken);


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
            return View();
        }
    }
}