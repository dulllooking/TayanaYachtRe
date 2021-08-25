using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class Yachts_Layout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                loadContent();
            }
        }

        private void loadContent()
        {
            //取得 Session 共用 Guid，Session 物件需轉回字串
            string guidStr = Session["guid"].ToString();
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //依 Guid 取得型號資料
            string sql = "SELECT layoutDeckPlanImgPathJSON FROM Yachts WHERE guid = @guidStr";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@guidStr", guidStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            StringBuilder layoutHtmlStr = new StringBuilder();
            List<LayoutPath> saveImagPathList = new List<LayoutPath>();
            if (reader.Read()) {
                string loadImgJson = HttpUtility.HtmlDecode(reader["layoutDeckPlanImgPathJSON"].ToString());
                //加入頁面組圖 HTML
                saveImagPathList = JsonConvert.DeserializeObject<List<LayoutPath>>(loadImgJson);
                foreach (var item in saveImagPathList) {
                    //加入每張圖片
                    layoutHtmlStr.Append($"<li><img src='upload/Images/{item.SavePath}' alt='layout' Width='670px' /></li>");
                }
                //渲染畫面
                ContentHtml.Text = layoutHtmlStr.ToString();
            }
            connection.Close();

        }

        //頁面組圖 JSON 資料
        public class LayoutPath
        {
            public string SavePath { get; set; }
        }
    }
}