using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class new_view : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                loadNews();
            }
        }

        private void loadNews()
        {
            List<ImagePath> savePathList = new List<ImagePath>();
            string guidStr = Request.QueryString["id"];
            //如果沒有網址傳值就導回新聞列表頁
            if (String.IsNullOrEmpty(guidStr)) {
                Response.Redirect("~/Tayanahtml/new_list.aspx");
            }
            //依取得 guid 連線資料庫取得新聞資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT * FROM News WHERE guid = @guid";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@guid", guidStr.Trim());
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                //渲染新聞標題
                newsTitle.InnerText = reader["headline"].ToString();
                //渲染新聞主文
                newsContent.Text = HttpUtility.HtmlDecode(reader["newsContentHtml"].ToString());
                string loadJson = HttpUtility.HtmlDecode(reader["newsImageJson"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            connection.Close();
            //渲染新聞組圖
            if (savePathList?.Count > 0) {
                string imgHtmlStr = "";
                foreach (var item in savePathList) {
                    imgHtmlStr += $"<p><img alt='Image' src='upload/Images/{item.SavePath}' style='width: 700px;' /></p>";
                }
                groupImg.Text = HttpUtility.HtmlDecode(imgHtmlStr);
            }
        }

        //用來接收組圖JSON資料
        public class ImagePath
        {
            public string SavePath { get; set; }
        }

    }
}