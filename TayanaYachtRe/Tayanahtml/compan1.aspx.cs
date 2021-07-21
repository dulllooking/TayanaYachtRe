using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class compan1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                loadContentText();
                loadContentImgV();
                loadContentImgH();
            }
        }

        private void loadContentText()
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlCountry = "SELECT certificatContent FROM Company";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlCountry, connection);
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                ContentText.Text = HttpUtility.HtmlDecode(reader["certificatContent"].ToString());
            }
            connection.Close();
        }

        private void loadContentImgV()
        {
            StringBuilder ImgVHtml = new StringBuilder();
            List<ImagePathV> savePathListV = new List<ImagePathV>();
            //取出圖檔資料
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlLoad = "SELECT certificatVerticalImgJSON FROM Company WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["certificatVerticalImgJSON"].ToString());
                //反序列化JSON格式
                savePathListV = JsonConvert.DeserializeObject<List<ImagePathV>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (savePathListV?.Count > 0) {
                foreach (var item in savePathListV) {
                    ImgVHtml.Append($"<li><p><img src='images/{item.SavePath}' alt='Tayana ' /></p></li>");
                }
            }
            ContentImgV.Text = ImgVHtml.ToString();
        }

        //JSON資料V
        public class ImagePathV
        {
            public string SavePath { get; set; }
        }

        private void loadContentImgH()
        {
            StringBuilder ImgHHtml = new StringBuilder();
            List<ImagePathH> savePathListH = new List<ImagePathH>();
            //取出圖檔資料
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlLoad = "SELECT certificatHorizontalImgJSON FROM Company WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["certificatHorizontalImgJSON"].ToString());
                //反序列化JSON格式
                savePathListH = JsonConvert.DeserializeObject<List<ImagePathH>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (savePathListH?.Count > 0) {
                foreach (var item in savePathListH) {
                    ImgHHtml.Append($"<li><p><img src='images/{item.SavePath}' alt='Tayana ' width='319' height='234' /></p></li>");
                }
            }
            ContentImgH.Text = ImgHHtml.ToString();
        }

        //JSON資料H
        public class ImagePathH
        {
            public string SavePath { get; set; }
        }

    }
}