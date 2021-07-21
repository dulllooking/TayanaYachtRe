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
            List<ImagePath> savePathList = new List<ImagePath>();
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlLoad = "SELECT * FROM Yachts ORDER BY id DESC";
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            StringBuilder bannerHtml = new StringBuilder();
            while (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["bannerImgPathJSON"].ToString());
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
                string imgNameStr = "";
                if (savePathList.Count > 0) {
                    imgNameStr = savePathList[0].SavePath;
                }
                string[] modelArr = reader["yachtModel"].ToString().Split(' ');
                string isNewDesignStr = reader["isNewDesign"].ToString();
                string isNewBuildingStr = reader["isNewBuilding"].ToString();
                string displayNewStr = "0";
                string newTagStr = "";
                if (isNewDesignStr.Equals("True")) {
                    displayNewStr = "1";
                    newTagStr = "new02.png";
                }
                else if (isNewBuildingStr.Equals("True")) {
                    displayNewStr = "1";
                    newTagStr = "new01.png";
                }
                bannerHtml.Append($"<li class='info' style='border-radius: 5px;height: 424px;width: 978px;'><a href='' target='_blank'><img src='upload/Images/{imgNameStr}' style='width: 978px;height: 424px;border-radius: 5px;'/></a><div class='wordtitle'>{modelArr[0]} <span>{modelArr[1]}</span><br /><p>SPECIFICATION SHEET</p></div><div class='new' style='display: none;overflow: hidden;border-radius:10px;'><img src='images/{newTagStr}' alt='new' /></div><input type='hidden' value='{displayNewStr}' /></li>");
            }
            connection.Close();
            LitBanner.Text = bannerHtml.ToString();
            LitBannerNum.Text = bannerHtml.ToString(); //不顯示但影響輪播圖片數量計算
        }

        //JSON資料
        public class ImagePath
        {
            public string SavePath { get; set; }
        }

        private void loadNews()
        {
            DateTime nowTime = DateTime.Now;
            string nowDate = nowTime.ToString("yyyy-MM-dd");
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT * FROM News WHERE dateTitle <= @dateTitle ORDER BY dateTitle DESC";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@dateTitle", nowDate);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            int count = 1;
            while (reader.Read()) {
                string isTopStr = reader["isTop"].ToString();
                string guidStr = reader["guid"].ToString();
                if (count == 1) {
                    ImgNews1.ImageUrl = reader["thumbnailPath"].ToString();
                    DateTime dateTimeTitle = DateTime.Parse(reader["dateTitle"].ToString());
                    LabNewsDate1.Text = dateTimeTitle.ToString("yyyy/M/d");
                    HLinkNews1.Text = reader["headline"].ToString();
                    HLinkNews1.NavigateUrl = $"new_view.aspx?id={guidStr}";
                    if (isTopStr.Equals("True")) {
                        ImgIsTop1.Visible = true;
                    }
                }
                else if (count == 2) {
                    ImgNews2.ImageUrl = reader["thumbnailPath"].ToString();
                    DateTime dateTimeTitle = DateTime.Parse(reader["dateTitle"].ToString());
                    LabNewsDate2.Text = dateTimeTitle.ToString("yyyy/M/d");
                    HLinkNews2.Text = reader["headline"].ToString();
                    HLinkNews2.NavigateUrl = $"new_view.aspx?id={guidStr}";
                    if (isTopStr.Equals("True")) {
                        ImgIsTop2.Visible = true;
                    }
                }
                else if (count == 3) {
                    ImgNews3.ImageUrl = reader["thumbnailPath"].ToString();
                    DateTime dateTimeTitle = DateTime.Parse(reader["dateTitle"].ToString());
                    LabNewsDate3.Text = dateTimeTitle.ToString("yyyy/M/d");
                    HLinkNews3.Text = reader["headline"].ToString();
                    HLinkNews3.NavigateUrl = $"new_view.aspx?id={guidStr}";
                    if (isTopStr.Equals("True")) {
                        ImgIsTop3.Visible = true;
                    }
                }
                else break;
                count++;
            }
            connection.Close();
        }
    }
}