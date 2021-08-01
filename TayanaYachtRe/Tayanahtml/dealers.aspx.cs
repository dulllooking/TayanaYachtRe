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
            //取得網址傳值的 id 內容
            string urlIDStr = Request.QueryString["id"];
            //如果是直接連到這頁時的預設 id (用公司主力 USA 的 id)
            if (string.IsNullOrEmpty(urlIDStr)) {
                urlIDStr = "1";
            }

            //如果是用短網址連入則用短網址 shortUrl 參數內容來判斷 ID
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            if (Page.RouteData.Values.Count > 0) {
                string urlStr = Page.RouteData.Values["shortUrl"].ToString();
                string sqlID = "SELECT * FROM CountrySort WHERE countrySort = @countrySort";
                SqlCommand commandID = new SqlCommand(sqlID, connection);
                commandID.Parameters.AddWithValue("@countrySort", urlStr);
                connection.Open();
                SqlDataReader readerID = commandID.ExecuteReader();
                if (readerID.Read()) {
                    urlIDStr = readerID["id"].ToString();
                }
                connection.Close();
            }

            //更新右側上方標題跟連結文字
            string sql = "SELECT countrySort FROM CountrySort WHERE id = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", urlIDStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string labStr = reader["countrySort"].ToString();
                LabLink.Text = labStr;
                LitTiTle.Text = $"<span>{labStr}</span>";
            }
            connection.Close();

            //讀取內容
            StringBuilder dealerListHtml = new StringBuilder();
            string sqlArea = "SELECT * FROM Dealers WHERE country_ID = @country_ID";
            SqlCommand commandArea = new SqlCommand(sqlArea, connection);
            commandArea.Parameters.AddWithValue("@country_ID", urlIDStr);
            connection.Open();
            SqlDataReader readerArea = commandArea.ExecuteReader();
            while (readerArea.Read()) {
                string idStr = readerArea["id"].ToString();
                string areaStr = readerArea["area"].ToString();
                string imgPathStr = readerArea["dealerImgPath"].ToString();
                string nameStr = readerArea["name"].ToString();
                string contactStr = readerArea["contact"].ToString();
                string addressStr = readerArea["address"].ToString();
                string telStr = readerArea["tel"].ToString();
                string faxStr = readerArea["fax"].ToString();
                string emailStr = readerArea["email"].ToString();
                string linkStr = readerArea["link"].ToString();
                dealerListHtml.Append("<li><div class='list02'><ul><li class='list02li'><div>" +
            $"<p><img id='ctl0{idStr}_Image{idStr}' src='../Tayanahtml/{imgPathStr}' style='border-width:0px;' /> </p></div></li>" +
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
            connection.Close();

            //渲染畫面
            DealerList.Text = dealerListHtml.ToString();
        }

        private void loadLeftMenu()
        {
            //反覆變更字串的值建議用 StringBuilder 效能較好
            StringBuilder leftMenuHtml = new StringBuilder();

            //取得國家分類
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlCountry = "SELECT * FROM CountrySort";
            SqlCommand commandCountry = new SqlCommand(sqlCountry, connection);
            connection.Open();
            SqlDataReader readerCountry = commandCountry.ExecuteReader();
            while (readerCountry.Read()) {
                string idStr = readerCountry["id"].ToString();
                string countryStr = readerCountry["countrySort"].ToString();
                // StringBuilder 用 Append 加入字串內容
                leftMenuHtml.Append($"<li><a href='dealers.aspx?id={idStr}'> {countryStr} </a></li>");
            }
            connection.Close();

            //渲染畫面
            LeftMenu.Text = leftMenuHtml.ToString();
        }
    }
}