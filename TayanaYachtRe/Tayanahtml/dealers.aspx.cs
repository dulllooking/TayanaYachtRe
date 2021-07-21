using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class dealers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                loadLeftMenu();
                loadDealerList();
            }
        }

        private void loadDealerList()
        {
            string urlIDStr = Request.QueryString["id"];
            if (string.IsNullOrEmpty(urlIDStr)) {
                urlIDStr = "1";
            }
            StringBuilder dealerListHtml = new StringBuilder();
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlCountry = "SELECT * FROM Dealers";
            //3.創建command物件
            SqlCommand commandCountry = new SqlCommand(sqlCountry, connection);
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得國家分類
            connection.Open();
            SqlDataReader readerCountry = commandCountry.ExecuteReader(); //指標指在BOF(表格之上)
            while (readerCountry.Read()) {
                string ctyidStr = readerCountry["country_ID"].ToString();
                if (urlIDStr.Equals(ctyidStr)) {
                    string idStr = readerCountry["id"].ToString();
                    string areaStr = readerCountry["area"].ToString();
                    string imgPathStr = readerCountry["dealerImgPath"].ToString();
                    string nameStr = readerCountry["name"].ToString();
                    string contactStr = readerCountry["contact"].ToString();
                    string addressStr = readerCountry["address"].ToString();
                    string telStr = readerCountry["tel"].ToString();
                    string faxStr = readerCountry["fax"].ToString();
                    string emailStr = readerCountry["email"].ToString();
                    string linkStr = readerCountry["link"].ToString();
                    dealerListHtml.Append("<li><div class='list02'><ul><li class='list02li'><div>"+
                $"<p><img id ='ctl00_ContentPlaceHolder1_Repeater1_ctl0{idStr}_Image{idStr}' src='{imgPathStr}' style='border-width:0px;' /> </p></div></li>" +
                $"<li class='list02li02'> <span>{areaStr}</span><br />");
                    if (!string.IsNullOrEmpty(nameStr)) {
                        dealerListHtml.Append($"{nameStr}<br />");
                    }
                    if (!string.IsNullOrEmpty(contactStr)) {
                        dealerListHtml.Append($"Contact：{contactStr}<br />");
                    }
                    if (!string.IsNullOrEmpty(addressStr)) {
                        dealerListHtml.Append($"Address：{addressStr}<br />");
                    }
                    if (!string.IsNullOrEmpty(telStr)) {
                        dealerListHtml.Append($"TEL：{telStr}<br />");
                    }
                    if (!string.IsNullOrEmpty(faxStr)) {
                        dealerListHtml.Append($"FAX：{faxStr}<br />");
                    }
                    if (!string.IsNullOrEmpty(emailStr)) {
                        dealerListHtml.Append($"E-Mail：{emailStr}<br />");
                    }
                    if (!string.IsNullOrEmpty(linkStr)) {
                        dealerListHtml.Append($"<a href='{linkStr}' target='_blank'>{linkStr}</a>");
                    }
                    dealerListHtml.Append("</li></ul></div></li>");
                }
            }
            connection.Close();
            DealerList.Text = dealerListHtml.ToString();
        }

        private void loadLeftMenu()
        {
            StringBuilder leftMenuHtml = new StringBuilder();
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlCountry = "SELECT * FROM CountrySort";
            //3.創建command物件
            SqlCommand commandCountry = new SqlCommand(sqlCountry, connection);
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得國家分類
            connection.Open();
            SqlDataReader readerCountry = commandCountry.ExecuteReader(); //指標指在BOF(表格之上)
            while (readerCountry.Read()) {
                string idStr = readerCountry["id"].ToString();
                string countryStr = readerCountry["countrySort"].ToString();
                leftMenuHtml.Append($"<li><a href='dealers.aspx?id={idStr}'> {countryStr} </a></li>");
            }
            connection.Close();
            LeftMenu.Text = leftMenuHtml.ToString();
        }
    }
}