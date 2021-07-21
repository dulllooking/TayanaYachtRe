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
            if (String.IsNullOrEmpty(guidStr)) {
                Response.Redirect("~/Tayanahtml/new_list.aspx");
            }
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "SELECT * FROM News WHERE guid = @guid";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.放入參數化資料
            command.Parameters.AddWithValue("@guid", guidStr.Trim());
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得News
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                ctl00_ContentPlaceHolder1_title.InnerText = reader["headline"].ToString();
                newsContent.Text = HttpUtility.HtmlDecode(reader["newsContentHtml"].ToString());
                string loadJson = HttpUtility.HtmlDecode(reader["newsImageJson"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (savePathList.Count > 0) {
                string imgHtmlStr = "";
                foreach (var item in savePathList) {
                    imgHtmlStr += $"<p><img alt='Image' src='upload/Images/{item.SavePath}' style='width: 700px;' /></p>";
                }
                groupImg.Text = HttpUtility.HtmlDecode(imgHtmlStr);
            }
        }

        //JSON資料
        public class ImagePath
        {
            public string SavePath { get; set; }
        }

    }
}