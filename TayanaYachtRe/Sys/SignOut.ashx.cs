using System;
using System.Web;
using System.Web.Security;

namespace TayanaYachtRe.Sys
{
    /// <summary>
    /// SignOut 的摘要描述
    /// </summary>
    public class SignOut : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //清除Cache，避免登出後按上一頁還會顯示Cache頁面
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            //清除所有的 Session
            if (context.Session != null) {
                context.Session.Abandon();
                context.Session.RemoveAll();
            }

            //建立一個同名的 Cookie 來覆蓋原本的 Cookie
            HttpCookie authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            authenticationCookie.Expires = DateTime.Now.AddYears(-1);
            context.Response.Cookies.Add(authenticationCookie);

            // 執行登出
            FormsAuthentication.SignOut();

            // 轉向到你登出後要到的頁面
            context.Response.Redirect("Manager_SignIn.aspx", true);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}