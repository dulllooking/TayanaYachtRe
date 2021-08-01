using Microsoft.AspNet.FriendlyUrls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace TayanaYachtRe
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // 設定不顯示副檔名 (如果只想隱藏副檔名做到此區塊就好)
            var routes = RouteTable.Routes;
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            //routes.EnableFriendlyUrls(settings);

            //修改避免 CKCKFinder 上傳功能錯誤
            routes.EnableFriendlyUrls(settings, new Microsoft.AspNet.FriendlyUrls.Resolvers.IFriendlyUrlResolver[] { new MyWebFormsFriendlyUrlResolver() });

            // 執行短網址路由方法
            RegisterRouters(RouteTable.Routes);
        }

        public class MyWebFormsFriendlyUrlResolver : Microsoft.AspNet.FriendlyUrls.Resolvers.WebFormsFriendlyUrlResolver
        {
            public override string ConvertToFriendlyUrl(string path)
            {
                //字串為 ckfinder 固定內容
                if (!string.IsNullOrEmpty(path) && path.ToLower().Contains("/ckfinder/core/connector/aspx/connector.aspx")) {
                    return path;
                }
                return base.ConvertToFriendlyUrl(path);
            }
        }

        private void RegisterRouters(RouteCollection routes)
        {
            //引數含義:
            //第一個引數：路由名稱--隨便自己起
            //第二個引數：路由規則
            //第三個引數：該路由規則交給哪一個頁面來處理
            routes.MapPageRoute("shortUrlRoute", "ShowList/{shortUrl}", "~/Tayanahtml/dealers.aspx");
            //...當然，您還可以新增更多路由規則
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}