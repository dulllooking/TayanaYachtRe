using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace TayanaYachtRe.Sys
{
    public partial class yachts_Manager_Cpage : System.Web.UI.Page
    {
        //宣告 List 方便用 Add 依序添加圖檔資料
        private List<ImagePath> savePathList = new List<ImagePath>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                DropDownList1.DataBind(); //先綁定，圖片才能取到型號
                loadImageList();
            }
        }

        protected void BtnAddYacht_Click(object sender, EventArgs e)
        {
            //插入空格區隔文字跟數字 (頁面細項標題會用到)
            string yachtModelStr = TBoxAddYachtModel.Text + " " + TBoxAddYachtLength.Text;
            //產生 GUID 隨機碼 + 時間2位秒數 (加強避免重複)
            DateTime nowTime = DateTime.Now;
            string nowSec = nowTime.ToString("ff");
            string guidStr = Guid.NewGuid().ToString().Trim() + nowSec;
            //取得勾選項目
            string isNewDesign = CBoxNewDesign.Checked.ToString();
            string isNewBuilding = CBoxNewBuilding.Checked.ToString();

            //插入遊艇型號基本資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "INSERT INTO Yachts (yachtModel, isNewDesign, isNewBuilding, guid) VALUES (@yachtModelStr, @isNewDesign, @isNewBuilding, @guidStr)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@yachtModelStr", yachtModelStr);
            command.Parameters.AddWithValue("@isNewDesign", isNewDesign);
            command.Parameters.AddWithValue("@isNewBuilding", isNewBuilding);
            command.Parameters.AddWithValue("@guidStr", guidStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //畫面渲染
            DropDownList1.DataBind();
            GridView1.DataBind();
            TBoxAddYachtModel.Text = "";
            TBoxAddYachtLength.Text = "";
            CBoxNewDesign.Checked = false;
            CBoxNewBuilding.Checked = false;
            DropDownList1.SelectedValue = yachtModelStr; //設定下拉選單選取項為新增項
            RadioButtonList.Items.Clear(); //新添加型號還沒有任何圖片，記得要清空畫面
        }

        protected void DeletingModel(object sender, GridViewDeleteEventArgs e)
        {
            //在刪除狀態下先取得刪除項的索引鍵欄位值
            string idStr = "";
            foreach (DictionaryEntry entry in e.Keys) {
                idStr = entry.Value.ToString();
            }

            //取出刪除的遊艇型號的組圖資料
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlBannerImg = "SELECT bannerImgPathJSON FROM Yachts WHERE id = @idStr";
            SqlCommand command = new SqlCommand(sqlBannerImg, connection);
            command.Parameters.AddWithValue("@idStr", idStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["bannerImgPathJSON"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            connection.Close();
            //刪除組圖實際圖檔
            for (int i = 0; i < savePathList.Count; i++) {
                File.Delete(savePath + savePathList[i].SavePath);
            }

            //取出刪除的遊艇型號的 Layout 組圖資料
            string sqlLayoutImg = "SELECT layoutDeckPlanImgPathJSON FROM Yachts WHERE id = @idStr";
            SqlCommand command2 = new SqlCommand(sqlLayoutImg, connection);
            command2.Parameters.AddWithValue("@idStr", idStr);
            connection.Open();
            SqlDataReader reader2 = command2.ExecuteReader();
            if (reader2.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader2["layoutDeckPlanImgPathJSON"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            connection.Close();
            //刪除組圖實際圖檔
            for (int i = 0; i < savePathList.Count; i++) {
                File.Delete(savePath + savePathList[i].SavePath);
            }

            //取出刪除的遊艇型號的 overview 規格圖片資料
            string sqlDimImg = "SELECT overviewDimensionsImgPath FROM Yachts WHERE id = @idStr";
            SqlCommand command3 = new SqlCommand(sqlDimImg, connection);
            command3.Parameters.AddWithValue("@idStr", idStr);
            connection.Open();
            SqlDataReader reader3 = command3.ExecuteReader();
            if (reader3.Read()) {
                string imgPath = reader3["overviewDimensionsImgPath"].ToString();
                //刪除實際圖檔
                if (!String.IsNullOrWhiteSpace(imgPath)) {
                    File.Delete(savePath + imgPath);
                }
            }
            connection.Close();

            //取出刪除的遊艇型號的 overview 的 PDF 檔案資料
            string sqlPDF = "SELECT overviewDownloadsFilePath FROM Yachts WHERE id = @idStr";
            SqlCommand command4 = new SqlCommand(sqlPDF, connection);
            command4.Parameters.AddWithValue("@idStr", idStr);
            connection.Open();
            SqlDataReader reader4 = command4.ExecuteReader();
            if (reader4.Read()) {
                string imgPath = reader4["overviewDownloadsFilePath"].ToString();
                //刪除實際圖檔
                if (!String.IsNullOrWhiteSpace(imgPath)) {
                    File.Delete(savePath + imgPath);
                }
            }
            connection.Close();
        }

        protected void DeletedModel(object sender, GridViewDeletedEventArgs e)
        {
            RadioButtonList.Items.Clear(); //清空圖片選項
            DropDownList1.DataBind(); //刷新下拉選單
            loadImageList(); //取得圖片選項
        }

        #region Group Image List
        private void loadImageList()
        {
            //取得下拉選單選取值
            string selModel_id = DropDownList1.SelectedValue;
            //連線資料庫取得首頁輪播圖資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlLoad = "SELECT bannerImgPathJSON FROM Yachts WHERE id = @selModel_id";
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            command.Parameters.AddWithValue("@selModel_id", selModel_id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["bannerImgPathJSON"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            connection.Close();
            if (savePathList?.Count > 0) {
                //預設第一張上傳的圖片為該遊艇型號首頁圖片
                bool firstCheck = true;
                foreach (var item in savePathList) {
                    if (firstCheck) {
                        //替首張圖片加上醒目色彩邊框
                        ListItem listItem = new ListItem($"<img src='/Tayanahtml/upload/Images/{item.SavePath}' alt='thumbnail' class='img-thumbnail bg-success' width='200px'/>", item.SavePath);
                        RadioButtonList.Items.Add(listItem);
                        firstCheck = false;
                    }
                    else {
                        ListItem listItem = new ListItem($"<img src='/Tayanahtml/upload/Images/{item.SavePath}' alt='thumbnail' class='img-thumbnail' width='200px'/>", item.SavePath);
                        RadioButtonList.Items.Add(listItem);
                    }
                }
            }
            DelImageBtn.Visible = false; //刪除鈕有選擇圖片時才顯示
        }

        protected void UploadBtn_Click(object sender, EventArgs e)
        {
            if (imageUpload.HasFile) {
                //取得上傳檔案大小
                int fileSize = imageUpload.PostedFile.ContentLength;
                if (fileSize < 1024 * 1000 * 10) {
                    loadImageList();
                    string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
                    //添加圖檔資料
                    foreach (HttpPostedFile postedFile in imageUpload.PostedFiles) {
                        //儲存圖片檔案及圖片名稱
                        //檢查專案資料夾內有無同名檔案，有同名就加流水號
                        DirectoryInfo directoryInfo = new DirectoryInfo(savePath);
                        string fileName = postedFile.FileName;
                        string[] fileNameArr = fileName.Split('.');
                        int count = 0;
                        foreach (var fileItem in directoryInfo.GetFiles()) {
                            if (fileItem.Name.Contains(fileNameArr[0])) {
                                count++;
                            }
                        }
                        fileName = fileNameArr[0] + $"({count + 1})." + fileNameArr[1];
                        postedFile.SaveAs(savePath + "temp" + fileName);
                        //新增每筆JSON資料
                        savePathList.Add(new ImagePath { SavePath = fileName });
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
                    string selModelStr = DropDownList1.SelectedValue;
                    //1.連線資料庫
                    SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                    //2.sql語法
                    string sql = "UPDATE Yachts SET bannerImgPathJSON = @savePathJsonStr WHERE id = @selModelStr";
                    //3.創建command物件
                    SqlCommand command = new SqlCommand(sql, connection);
                    //4.參數化
                    command.Parameters.AddWithValue("@savePathJsonStr", savePathJsonStr);
                    command.Parameters.AddWithValue("@selModelStr", selModelStr);
                    //5.資料庫連線開啟
                    connection.Open();
                    //6.執行sql (新增刪除修改)
                    command.ExecuteNonQuery();
                    //7.資料庫關閉
                    connection.Close();
                    RadioButtonList.Items.Clear();
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

        protected void RadioButtonListH_SelectedIndexChanged(object sender, EventArgs e)
        {
            DelImageBtn.Visible = true;
        }

        protected void DelHImageBtn_Click(object sender, EventArgs e)
        {
            string selImageStr = RadioButtonList.SelectedValue;
            loadImageList();
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
            string selModel_id = DropDownList1.SelectedValue;
            //2.sql語法
            string sql = "UPDATE Yachts SET bannerImgPathJSON = @savePathJsonStr WHERE id = @selModel_id";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@savePathJsonStr", savePathJsonStr);
            command.Parameters.AddWithValue("@selModel_id", selModel_id); ;
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery();
            //7.資料庫關閉
            connection.Close();
            //渲染畫面
            RadioButtonList.Items.Clear();
            loadImageList();
        }
        #endregion

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioButtonList.Items.Clear();
            loadImageList();
        }

        protected void UpdatedModel(object sender, GridViewUpdatedEventArgs e)
        {
            DropDownList1.DataBind();
        }

    }
}