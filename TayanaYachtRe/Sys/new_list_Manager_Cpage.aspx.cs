﻿using CKFinder;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;

namespace TayanaYachtRe.Sys
{
    public partial class new_list_Manager_Cpage : System.Web.UI.Page
    {
        //宣告 List 方便用 Add 依序添加資料
        private List<ImagePath> savePathList = new List<ImagePath>();
        protected void Page_Load(object sender, EventArgs e)
        {
            //權限關門判斷 (Cookie)
            if (!User.Identity.IsAuthenticated) {
                Response.Redirect("Manager_SignIn.aspx"); //導回登入頁
            }
            if (!IsPostBack) {
                ckfinderSetPath();
                CoverList.Visible = false; //隱藏: 新聞列表縮圖 + 新聞簡介
                NewsContent.Visible = false; //隱藏: 新聞上方主要內容 + 新聞下方組圖
                Calendar1.SelectedDate = Calendar1.TodaysDate; //預設選取當日日期
                loadDayNewsHeadline(); //讀取新聞標題
                //如果選取當日有新聞
                if (headlineRadioBtnList.Items.Count > 0) {
                    CoverList.Visible = true; //顯示: 新聞列表縮圖 + 新聞簡介
                    NewsContent.Visible = true; //顯示: 新聞上方主要內容 + 新聞下方組圖
                    loadThumbnail(); //讀取-新聞列表縮圖
                    loadSummary(); //讀取-新聞簡介
                    loadNewsContent(); //讀取-新聞上方主要內容
                    loadImageList(); //讀取-新聞下方組圖
                }
            }
        }

        private void loadThumbnail()
        {
            string selHeadlineStr = headlineRadioBtnList.SelectedValue;
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "SELECT thumbnailPath FROM News WHERE dateTitle = @dateTitle AND headline = @headline";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", selHeadlineStr);
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得地區分類
            connection.Open();
            SqlDataReader reader = command.ExecuteReader(); //指標指在BOF(表格之上)
            if (reader.Read()) {
                string thumbnailPathStr = reader["thumbnailPath"].ToString();
                Thumbnail.ImageUrl = "~/Tayanahtml/" + thumbnailPathStr;
            }
            else {
                Thumbnail.ImageUrl = "";
            }
            connection.Close();
            //渲染畫面
            LabUploadThumbnail.Visible = false;
        }

        private void loadSummary()
        {
            //取得新聞標題
            string selHeadlineStr = headlineRadioBtnList.SelectedValue;
            //取得新聞日期
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            //取得新聞簡介內容
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT summary FROM News WHERE dateTitle = @dateTitle AND headline = @headline";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", selHeadlineStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                summaryTbox.Text = reader["summary"].ToString();
            }
            else {
                summaryTbox.Text = "";
            }
            connection.Close();
            //渲染畫面
            LabUploadSummary.Visible = false;
        }

        private void loadNewsContent()
        {
            string selHeadlineStr = headlineRadioBtnList.SelectedValue;
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "SELECT newsContentHtml FROM News WHERE dateTitle = @dateTitle AND headline = @headline";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", selHeadlineStr);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                CKEditorControl1.Text = HttpUtility.HtmlDecode(reader["newsContentHtml"].ToString());
            }
            else {
                CKEditorControl1.Text = "";
            }
            //資料庫關閉
            connection.Close();
            //渲染畫面
            UploadContentLab.Visible = false;
        }

        #region Group Image List
        private void loadImageList()
        {
            string selHeadlineStr = headlineRadioBtnList.SelectedValue;
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlLoad = "SELECT newsImageJson FROM News WHERE dateTitle = @dateTitle AND headline = @headline";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            //4.參數化
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", selHeadlineStr);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["newsImageJson"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (savePathList.Count > 0) {
                foreach (var item in savePathList) {
                    ListItem listItem = new ListItem($"<img src='../Tayanahtml/upload/Images/{item.SavePath}' alt='thumbnail' class='img-thumbnail' width='200'/>", item.SavePath);
                    RadioButtonListImg.Items.Add(listItem);
                }
            }
            DelImageBtn.Visible = false;
        }

        protected void UploadImgBtn_Click(object sender, EventArgs e)
        {
            if (imageUpload.HasFile) {
                //取得上傳檔案大小
                int fileSize = imageUpload.PostedFile.ContentLength;
                if (fileSize < 1024 * 10 * 1000) {
                    loadImageList();
                    DelImageBtn.Visible = false;
                    //添加圖檔資料
                    foreach (HttpPostedFile postedFile in imageUpload.PostedFiles) {
                        //需填完整路徑，結尾反斜線如果沒加要用Path.Combine()可自動添加
                        string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
                        string fileName = postedFile.FileName;
                        if (savePathList.Count > 0) {
                            foreach (var item in savePathList) {
                                if (item.SavePath.Equals(fileName)) {
                                    string[] fileNameArr = fileName.Split('.');
                                    fileName = fileNameArr[0] + "(1)." + fileNameArr[1];
                                }
                            }
                            postedFile.SaveAs(savePath + "temp" + fileName);
                            //新增每筆JSON資料
                            savePathList.Add(new ImagePath { SavePath = fileName });
                        }
                        else {
                            postedFile.SaveAs(savePath + "temp" + fileName);
                            //新增每筆JSON資料
                            savePathList.Add(new ImagePath { SavePath = fileName });
                        }
                        //壓縮圖檔
                        var image = NetVips.Image.NewFromFile(savePath + "temp" + fileName);
                        if (image.Width > 967 * 2) {
                            var newImg = image.Resize(0.5);
                            while (newImg.Width > 967 * 2) {
                                newImg = newImg.Resize(0.5);
                            }
                            newImg.WriteToFile(savePath + fileName);
                        }
                        else {
                            postedFile.SaveAs(savePath + fileName);
                        }
                        File.Delete(savePath + "temp" + fileName);
                    }
                    //將List資料轉為Json格式字串
                    string savePathJsonStr = JsonConvert.SerializeObject(savePathList);
                    string selHeadlineStr = headlineRadioBtnList.SelectedValue;
                    string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
                    //1.連線資料庫
                    SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                    //2.sql語法
                    string sql = "UPDATE News SET newsImageJson = @newsImageJson WHERE dateTitle = @dateTitle AND headline = @headline";
                    //3.創建command物件
                    SqlCommand command = new SqlCommand(sql, connection);
                    //4.參數化
                    command.Parameters.AddWithValue("@newsImageJson", savePathJsonStr);
                    command.Parameters.AddWithValue("@dateTitle", selNewsDate);
                    command.Parameters.AddWithValue("@headline", selHeadlineStr);
                    //5.資料庫連線開啟
                    connection.Open();
                    //6.執行sql (新增刪除修改)
                    command.ExecuteNonQuery();
                    //7.資料庫關閉
                    connection.Close();
                    RadioButtonListImg.Items.Clear();
                    loadImageList();
                }
                else {
                    Response.Write("<script>alert('*The maximum upload size is 10MB!');</script>");
                }
            }
        }

        //JSON資料
        public class ImagePath
        {
            public string SavePath { get; set; }
        }

        protected void RadioButtonListImg_SelectedIndexChanged(object sender, EventArgs e)
        {
            DelImageBtn.Visible = true;
        }

        protected void DelImageBtn_Click(object sender, EventArgs e)
        {
            loadImageList();
            string selImageStr = RadioButtonListImg.SelectedValue;
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
            savePath += selImageStr;
            File.Delete(savePath);
            for (int i = 0; i < savePathList.Count; i++) {
                if (savePathList[i].SavePath.Equals(selImageStr)) {
                    savePathList.RemoveAt(i);
                }
            }
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //將List資料轉為Json格式字串
            string savePathJsonStr = JsonConvert.SerializeObject(savePathList);
            string selHeadlineStr = headlineRadioBtnList.SelectedValue;
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            //2.sql語法
            string sql = "UPDATE News SET newsImageJson = @newsImageJson WHERE dateTitle = @dateTitle AND headline = @headline";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@newsImageJson", savePathJsonStr);
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", selHeadlineStr);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery();
            //7.資料庫關閉
            connection.Close();
            //渲染畫面
            RadioButtonListImg.Items.Clear();
            loadImageList();
            DelImageBtn.Visible = false;
        }
        #endregion



        private void loadDayNewsHeadline()
        {
            //依選取日期取得資料庫新聞內容
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT * FROM News WHERE dateTitle = @dateTitle ORDER BY id ASC";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@dateTitle", Calendar1.SelectedDate.ToString("yyyy-M-dd"));
            connection.Open();
            SqlDataReader reader= command.ExecuteReader();
            while (reader.Read()) {
                string headlineStr = reader["headline"].ToString();
                string isTopStr = reader["isTop"].ToString();
                //渲染畫面
                LabIsTop.Visible = false;
                if (isTopStr.Equals("True")) {
                    LabIsTop.Visible = true;
                }
                ListItem listItem = new ListItem();
                listItem.Text = headlineStr;
                listItem.Value = headlineStr;
                headlineRadioBtnList.Items.Add(listItem);
            }
            connection.Close();

            //預設選取新增新聞項目
            int RadioBtnCount = headlineRadioBtnList.Items.Count;
            if (RadioBtnCount > 0) {
                headlineRadioBtnList.Items[RadioBtnCount - 1].Selected = true;
                deleteNewsBtn.Visible = true;
            }

        }


        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            //取得當月第一天跟最後一天
            DateTime firstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
            DateTime lastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);
            //取得當月第一天往前10週+當月最後一天往後10週
            string firstDayLastWeek = firstDay.AddDays(-70).ToString("yyyyMMdd");
            string lastDayNextWeek = lastDay.AddDays(70).ToString("yyyyMMdd");
            //取得新聞日期
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = $"SELECT dateTitle FROM News WHERE dateTitle BETWEEN '{firstDayLastWeek}' AND '{lastDayNextWeek}'";
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                //轉換為 DateTime 型別
                DateTime newsTime = DateTime.Parse(reader["dateTitle"].ToString());
                //有新聞的日期 且 此日期不是選中的日期時 就修改日期外觀
                if (e.Day.Date.Date == newsTime && e.Day.Date.Date != Calendar1.SelectedDate) {
                    //渲染畫面
                    //e.Cell.BorderWidth = Unit.Pixel(1); //外框線粗細
                    //e.Cell.BorderColor = Color.BlueViolet; //外框線顏色
                    e.Cell.Font.Underline = true; //有無下地線
                    e.Cell.Font.Bold = true; //是否為粗體
                    e.Cell.ForeColor = Color.DodgerBlue; //外觀色彩
                }
            }
            connection.Close();
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            deleteNewsBtn.Visible = false;
            DelImageBtn.Visible = false;
            LabIsTop.Visible = false;
            headlineRadioBtnList.Items.Clear();
            RadioButtonListImg.Items.Clear();
            loadDayNewsHeadline();
            if (headlineRadioBtnList.Items.Count > 0) {
                CoverList.Visible = true;
                NewsContent.Visible = true;
                loadThumbnail();
                loadSummary();
                loadNewsContent();
                loadImageList();
            }
            else {
                CoverList.Visible = false;
                NewsContent.Visible = false;
            }
        }

        protected void headlineRadioBtnList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //依日期的新聞標題選取項目判斷是不是焦點新聞
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT isTop FROM News WHERE dateTitle = @dateTitle AND headline = @headline";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@dateTitle", Calendar1.SelectedDate.ToString("yyyy-M-dd"));
            command.Parameters.AddWithValue("@headline", headlineRadioBtnList.SelectedValue);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string isTopStr = reader["isTop"].ToString();
                //渲染畫面
                LabIsTop.Visible = false;
                if (isTopStr.Equals("True")) {
                    LabIsTop.Visible = true;
                }
            }
            connection.Close();

            //渲染畫面
            RadioButtonListImg.Items.Clear();
            loadThumbnail();
            loadSummary();
            loadNewsContent();
            loadImageList();
        }

        protected void deleteNewsBtn_Click(object sender, EventArgs e)
        {
            //隱藏刪除鈕
            deleteNewsBtn.Visible = false;
            //取得選取項目內容
            string selHeadlineStr = headlineRadioBtnList.SelectedValue;
            //取得日曆選取日期
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            //連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
 
            //刪除圖檔(縮圖)
            string sql = "SELECT thumbnailPath FROM News WHERE dateTitle = @dateTitle AND headline = @headline";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", selHeadlineStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string savePathT = reader["thumbnailPath"].ToString();
                if (!savePathT.Equals("")) {
                    string savePath = Server.MapPath("~/Tayanahtml/");
                    savePath += savePathT;
                    File.Delete(savePath);
                }
            }
            connection.Close();

            //刪除資料庫該筆資料
            string sqlDel = "DELETE FROM News WHERE dateTitle = @dateTitle AND headline = @headline";
            SqlCommand commandDel = new SqlCommand(sqlDel, connection);
            commandDel.Parameters.AddWithValue("@dateTitle", selNewsDate);
            commandDel.Parameters.AddWithValue("@headline", selHeadlineStr);
            connection.Open();
            commandDel.ExecuteNonQuery();
            connection.Close();

            //渲染畫面
            deleteNewsBtn.Visible = false;
            DelImageBtn.Visible = false;
            CBoxIsTop.Checked = false;
            headlineRadioBtnList.Items.Clear();
            RadioButtonListImg.Items.Clear();
            loadDayNewsHeadline();
            if (headlineRadioBtnList.Items.Count > 0) {
                CoverList.Visible = true;
                NewsContent.Visible = true;
                loadThumbnail();
                loadSummary();
                loadNewsContent();
                loadImageList();
            }
            else {
                CoverList.Visible = false;
                NewsContent.Visible = false;
            }
        }

        protected void AddHeadlineBtn_Click(object sender, EventArgs e)
        {
            //產生 GUID 隨機碼
            string guid = Guid.NewGuid().ToString().Trim();
            //取得日曆選取日期
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            //取得是否勾選
            string isTop = CBoxIsTop.Checked.ToString(); //得到 "True" or "False"
            //將資料存入資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "INSERT INTO News (dateTitle, headline, guid, isTop) VALUES (@dateTitle, @headline, @guid, @top)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", headlineTbox.Text);
            command.Parameters.AddWithValue("@guid", guid);
            command.Parameters.AddWithValue("@top", isTop); //存入資料庫會轉為 0 or 1
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //畫面渲染
            deleteNewsBtn.Visible = false;
            DelImageBtn.Visible = false;
            CBoxIsTop.Checked = false;
            headlineRadioBtnList.Items.Clear();
            RadioButtonListImg.Items.Clear();
            loadDayNewsHeadline();
            if (headlineRadioBtnList.Items.Count > 0) {
                CoverList.Visible = true;
                NewsContent.Visible = true;
                loadThumbnail();
                loadSummary();
                loadNewsContent();
                loadImageList();
            }
            else {
                CoverList.Visible = false;
                NewsContent.Visible = false;
            }
            //清空輸入欄位
            headlineTbox.Text = "";
        }


        protected void UploadThumbnailBtn_Click(object sender, EventArgs e)
        {
            //需填完整路徑，結尾反斜線如果沒加要用Path.Combine()可自動添加
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
            //有選檔案才可上傳
            if (thumbnailUpload.HasFile) {
                string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
                string fileName = thumbnailUpload.FileName;
                string selHeadlineStr = headlineRadioBtnList.SelectedValue;
                //1.連線資料庫
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                //2.sql語法
                string sqlDel = "SELECT thumbnailPath FROM News WHERE dateTitle = @dateTitle AND headline = @headline";
                string sql = "UPDATE News SET thumbnailPath = @path WHERE dateTitle = @dateTitle AND headline = @headline";
                //3.創建command物件
                SqlCommand commandDel = new SqlCommand(sqlDel, connection);
                SqlCommand command = new SqlCommand(sql, connection);
                //4.參數化
                commandDel.Parameters.AddWithValue("@dateTitle", selNewsDate);
                commandDel.Parameters.AddWithValue("@headline", selHeadlineStr);
                command.Parameters.AddWithValue("@path", $"upload/Images/{fileName}");
                command.Parameters.AddWithValue("@dateTitle", selNewsDate);
                command.Parameters.AddWithValue("@headline", selHeadlineStr);

                //刪除舊圖檔
                connection.Open();
                SqlDataReader reader = commandDel.ExecuteReader();
                if (reader.Read()) {
                    string delFileName = reader["thumbnailPath"].ToString();
                    if (!delFileName.Equals("")) {
                        string delPath = Server.MapPath("~/Tayanahtml/");
                        delPath += delFileName;
                        File.Delete(delPath);
                    }
                }
                connection.Close();

                //存圖
                DateTime nowtime = DateTime.Now;
                thumbnailUpload.SaveAs(savePath + "temp" + fileName);
                LabUploadThumbnail.Visible = true;
                LabUploadThumbnail.ForeColor = Color.Green;
                LabUploadThumbnail.Text = "*Upload Success! - " + nowtime.ToString("G");

                //壓縮圖檔
                var image = NetVips.Image.NewFromFile(savePath + "temp" + fileName);
                if (image.Width > 161 * 2) {
                    var newImg = image.Resize(0.5);
                    while (newImg.Width > 161 * 2) {
                        newImg = newImg.Resize(0.5);
                    }
                    newImg.WriteToFile(savePath + fileName);
                }
                else {
                    thumbnailUpload.SaveAs(savePath + fileName);
                }
                File.Delete(savePath + "temp" + fileName);

                //更新資料
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                //畫面渲染
                Thumbnail.ImageUrl = "~/Tayanahtml/upload/Images/" + fileName;
            }
            else {
                LabUploadThumbnail.Visible = true;
                LabUploadThumbnail.ForeColor = Color.Red;
                LabUploadThumbnail.Text = "*Need Choose File!";
            }
        }

        protected void UploadSummaryBtn_Click(object sender, EventArgs e)
        {
            string selHeadlineStr = headlineRadioBtnList.SelectedValue;
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            //更新新聞簡介內容
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "UPDATE News SET summary = @summary WHERE dateTitle = @dateTitle AND headline = @headline";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@summary", summaryTbox.Text);
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", selHeadlineStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染畫面
            DateTime nowtime = DateTime.Now;
            LabUploadSummary.Visible = true;
            LabUploadSummary.Text = "*Upload Success! - " + nowtime.ToString("G");
        }


        private void ckfinderSetPath()
        {
            FileBrowser fileBrowser = new FileBrowser();
            fileBrowser.BasePath = "/ckfinder";
            fileBrowser.SetupCKEditor(CKEditorControl1);
        }

        protected void UploadContentBtn_Click(object sender, EventArgs e)
        {
            string selHeadlineStr = headlineRadioBtnList.SelectedValue;
            string selNewsDate = Calendar1.SelectedDate.ToString("yyyy-M-dd");
            string newsContentHtmlStr = HttpUtility.HtmlEncode(CKEditorControl1.Text);
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "UPDATE News SET newsContentHtml = @newsContentHtml WHERE dateTitle = @dateTitle AND headline = @headline";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@newsContentHtml", newsContentHtmlStr);
            command.Parameters.AddWithValue("@dateTitle", selNewsDate);
            command.Parameters.AddWithValue("@headline", selHeadlineStr);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //無回傳值
            //7.資料庫關閉
            connection.Close();

            DateTime nowtime = DateTime.Now;
            UploadContentLab.Visible = true;
            UploadContentLab.Text = "*Upload Success! - " + nowtime.ToString("G");
        }



    }
}