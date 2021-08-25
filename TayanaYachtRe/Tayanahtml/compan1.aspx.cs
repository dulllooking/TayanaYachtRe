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
            //從資料庫取內文資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlCountry = "SELECT TOP 1 certificatContent FROM Company";
            SqlCommand command = new SqlCommand(sqlCountry, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                //渲染畫面
                ContentText.Text = reader["certificatContent"].ToString();
            }
            connection.Close();
        }

        private void loadContentImgV()
        {
            //會重複添加內容所以用 StringBuilder 效能較好
            StringBuilder ImgVHtml = new StringBuilder();
            //用 List<T> 來存取 JSON 格式圖片檔名
            List<ImagePathV> savePathListV = new List<ImagePathV>();
            //從資料庫取出直式圖檔檔名
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlLoad = "SELECT TOP 1 certificatVerticalImgJSON FROM Company";
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["certificatVerticalImgJSON"].ToString());
                //反序列化JSON格式
                savePathListV = JsonConvert.DeserializeObject<List<ImagePathV>>(loadJson);
            }
            connection.Close();
            // ?. 可用來判斷不是 Null 才執行 Count
            if (savePathListV?.Count > 0) {
                foreach (var item in savePathListV) {
                    ImgVHtml.Append($"<li><p><img src='images/{item.SaveName}' alt='Tayana ' /></p></li>");
                }
            }
            //渲染畫面
            ContentImgV.Text = ImgVHtml.ToString();
        }

        // JSON 資料 V 直式
        public class ImagePathV
        {
            public string SaveName { get; set; }
        }

        private void loadContentImgH()
        {
            //會重複添加內容所以用 StringBuilder 效能較好
            StringBuilder ImgHHtml = new StringBuilder();
            //用 List<T> 來存取 JSON 格式圖片檔名
            List<ImagePathH> savePathListH = new List<ImagePathH>();
            //從資料庫取出橫式圖檔檔名
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlLoad = "SELECT TOP 1 certificatHorizontalImgJSON FROM Company";
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["certificatHorizontalImgJSON"].ToString());
                //反序列化JSON格式
                savePathListH = JsonConvert.DeserializeObject<List<ImagePathH>>(loadJson);
            }
            connection.Close();
            // ?. 可用來判斷不是 Null 才執行 Count
            if (savePathListH?.Count > 0) {
                foreach (var item in savePathListH) {
                    ImgHHtml.Append($"<li><p><img src='images/{item.SaveName}' alt='Tayana ' width='319px' height='234px' /></p></li>");
                }
            }
            ContentImgH.Text = ImgHHtml.ToString();
        }

        // JSON 資料 H 直式
        public class ImagePathH
        {
            public string SaveName { get; set; }
        }

    }
}