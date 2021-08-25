using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                loadBanner();
                loadNews();
            }
        }

        private void loadBanner()
        {
            //用 List<T> 接收 Json 格式圖片資料
            List<ImagePath> savePathList = new List<ImagePath>();
            //連線資料庫取出圖片資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlLoad = "SELECT * FROM Yachts ORDER BY id DESC";
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            StringBuilder bannerHtml = new StringBuilder();
            while (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["bannerImgPathJSON"].ToString());
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
                //每個型號只取出第一張圖
                string imgNameStr = "";
                if (savePathList.Count > 0) {
                    imgNameStr = savePathList[0].SavePath;
                }
                //遊艇型號字串用空格切割區分文字及數字
                string[] modelArr = reader["yachtModel"].ToString().Split(' ');
                //依新設計或新建造來切換顯示標籤
                string isNewDesignStr = reader["isNewDesign"].ToString();
                string isNewBuildingStr = reader["isNewBuilding"].ToString();
                string newTagStr = ""; //標籤檔名用
                // value 預設為 0 不顯示標籤
                string displayNewStr = "0";
                //判斷是否顯示對應標籤
                if (isNewDesignStr.Equals("True")) {
                    displayNewStr = "1";
                    newTagStr = "images/new02.png";
                }
                else if (isNewBuildingStr.Equals("True")) {
                    displayNewStr = "1";
                    newTagStr = "images/new01.png";
                }
                //加入遊艇型號輪播圖 HTML
                bannerHtml.Append($"<li class='info' style='border-radius: 5px;height: 424px;width: 978px;'><a href='' target='_blank'><img src='upload/Images/{imgNameStr}' style='width: 978px;height: 424px;border-radius: 5px;'/></a><div class='wordtitle'>{modelArr[0]} <span>{modelArr[1]}</span><br /><p>SPECIFICATION SHEET</p></div><div class='new' style='display: none;overflow: hidden;border-radius:10px;'><img src='{newTagStr}' alt='new' /></div><input type='hidden' value='{displayNewStr}' /></li>");
            }
            connection.Close();
            //渲染畫面
            LitBanner.Text = bannerHtml.ToString();
            LitBannerNum.Text = bannerHtml.ToString(); //不顯示但影響輪播圖片數量計算
        }

        //輪播圖 JSON 資料
        public class ImagePath
        {
            public string SavePath { get; set; }
        }

        private void loadNews()
        {
            //設定首頁 3 筆新聞的時間範圍
            DateTime nowTime = DateTime.Now;
            string nowDate = nowTime.ToString("yyyy-MM-dd");
            int startDate = -1;
            DateTime limitTime = nowTime.AddMonths(startDate);
            string limitDate = limitTime.ToString("yyyy-MM-dd");

            //計算設定的時間範圍內新聞數量
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT COUNT(id) FROM News WHERE dateTitle >= @limitDate AND dateTitle <= @nowDate";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nowDate", nowDate);
            command.Parameters.AddWithValue("@limitDate", limitDate);
            connection.Open();
            //用 ExecuteScalar() 來算數量
            int newsNum = Convert.ToInt32(command.ExecuteScalar());
            //時間範圍設定持續往前 1 個月，直到取出新聞數量超過 3 筆
            while (newsNum < 3) {
                startDate--;
                limitTime = nowTime.AddDays(startDate);
                limitDate = limitTime.ToString("yyyy-MM-dd");
                SqlCommand command2 = new SqlCommand(sql, connection);
                command2.Parameters.AddWithValue("@nowDate", nowDate);
                command2.Parameters.AddWithValue("@limitDate", limitDate);
                newsNum = Convert.ToInt32(command2.ExecuteScalar());
            }
            connection.Close();

            //取出時間範圍內首 3 筆新聞，且 TOP 會放在取出項的最前面
            connection.Open();
            string sql2 = "SELECT TOP 3 * FROM News WHERE dateTitle >= @limitDate AND dateTitle <= @nowDate ORDER BY isTop DESC, dateTitle DESC";
            SqlCommand command3 = new SqlCommand(sql2, connection);
            command3.Parameters.AddWithValue("@nowDate", nowDate);
            command3.Parameters.AddWithValue("@limitDate", limitDate);
            SqlDataReader reader = command3.ExecuteReader();
            int count = 1; //第幾筆新聞
            while (reader.Read()) {
                string isTopStr = reader["isTop"].ToString();
                string guidStr = reader["guid"].ToString();
                if (count == 1) {
                    //渲染第1筆新聞圖卡
                    string newsImg = reader["thumbnailPath"].ToString();
                    LiteralNewsImg1.Text = $"<img id='thumbnail_Image1' src='upload/Images/{newsImg}' style='border-width: 0px;' />";
                    DateTime dateTimeTitle = DateTime.Parse(reader["dateTitle"].ToString());
                    LabNewsDate1.Text = dateTimeTitle.ToString("yyyy/M/d");
                    HLinkNews1.Text = reader["headline"].ToString();
                    HLinkNews1.NavigateUrl = $"new_view.aspx?id={guidStr}";
                    if (isTopStr.Equals("True")) {
                        ImgIsTop1.Visible = true;
                    }
                }
                else if (count == 2) {
                    //渲染第2筆新聞圖卡
                    string newsImg = reader["thumbnailPath"].ToString();
                    LiteralNewsImg2.Text = $"<img id='thumbnail_Image2' src='upload/Images/{newsImg}' style='border-width: 0px;' />";
                    DateTime dateTimeTitle = DateTime.Parse(reader["dateTitle"].ToString());
                    LabNewsDate2.Text = dateTimeTitle.ToString("yyyy/M/d");
                    HLinkNews2.Text = reader["headline"].ToString();
                    HLinkNews2.NavigateUrl = $"new_view.aspx?id={guidStr}";
                    if (isTopStr.Equals("True")) {
                        ImgIsTop2.Visible = true;
                    }
                }
                else if (count == 3) {
                    //渲染第3筆新聞圖卡
                    string newsImg = reader["thumbnailPath"].ToString();
                    LiteralNewsImg3.Text = $"<img id='thumbnail_Image3' src='upload/Images/{newsImg}' style='border-width: 0px;' />";
                    DateTime dateTimeTitle = DateTime.Parse(reader["dateTitle"].ToString());
                    LabNewsDate3.Text = dateTimeTitle.ToString("yyyy/M/d");
                    HLinkNews3.Text = reader["headline"].ToString();
                    HLinkNews3.NavigateUrl = $"new_view.aspx?id={guidStr}";
                    if (isTopStr.Equals("True")) {
                        ImgIsTop3.Visible = true;
                    }
                }
                else break; //超過3筆後停止
                count++;
            }
            connection.Close();
        }
    }
}