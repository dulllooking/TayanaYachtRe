using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class Yachts_Video : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                loadContent();
            }
        }

        private void loadContent()
        {
            List<RowData> saveRowList = new List<RowData>();
            //取得 Session 共用 Guid，Session 物件需轉回字串
            string guidStr = Session["guid"].ToString();
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //依 Guid 取得型號資料
            string sql = "SELECT overviewDimensionsJSON FROM Yachts WHERE guid = @guid";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@guid", guidStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["overviewDimensionsJSON"].ToString());
                saveRowList = JsonConvert.DeserializeObject<List<RowData>>(loadJson);
                // List<T> 第一筆資料是放影片連結
                string youtubeUrlStr = saveRowList[0].SaveValue;
                //如果沒有影片連結就導回 OverView 分頁
                if (String.IsNullOrEmpty(youtubeUrlStr)) {
                    Response.Redirect($"Yachts_OverView.aspx?id={guidStr}");
                }
                else {
                    //將取出的 YouTube 連結字串分離出 "影片 ID 字串"
                    //使用者如果是用分享功能複製連結時處理方式
                    string[] youtubeUrlArr = youtubeUrlStr.Split('/');
                    //使用者如果是直接從網址複製連結時處理方式
                    string[] vedioIDArr = youtubeUrlArr[youtubeUrlArr.Length - 1].Split('=');
                    //將 "影片 ID 字串" 組合成嵌入狀態的 YouTube 連結
                    string strNewUrl = "https:/" + "/youtube.com/" + "embed/" + vedioIDArr[vedioIDArr.Length - 1];
                    //更新 <iframe> src 連結
                    video.Attributes.Add("src", strNewUrl);
                }
            }
            connection.Close();
        }

        // JSON 資料
        public class RowData
        {
            public string SaveItem { get; set; }
            public string SaveValue { get; set; }
        }
    }
}