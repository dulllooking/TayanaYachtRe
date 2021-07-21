using System.Web;
using System.Web.Security;


namespace TayanaYachtRe.Sys
{
    /// <summary>
    /// CheckAccount 的摘要描述
    /// </summary>
    public class CheckAccount : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //ashx裡的Request/Response都要加上context
            string ticketUserData = ((FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.UserData;
            string[] ticketUserDataArr = ticketUserData.Split(';');
            bool haveRight = HttpContext.Current.User.Identity.IsAuthenticated;
            //依管理權限導頁
            if (haveRight) {
                if (ticketUserDataArr[0].Equals("True")) {
                    //以驗證票夾帶資料作為限制
                    context.Response.Redirect("User_Manager_Cpage.aspx"); //最高管理員-跳至管理員審核頁面
                }
                else {
                    context.Response.Redirect("new_list_Manager_Cpage.aspx");
                }
            }
            else {
                context.Response.Redirect("Manager_SignIn.aspx"); //導回登入頁
            }
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