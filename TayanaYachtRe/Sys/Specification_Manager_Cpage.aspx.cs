using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace TayanaYachtRe.Sys
{
    public partial class Specification_Manager_Cpage : System.Web.UI.Page
    {
        //宣告 List<T> 方便用 Add 依序添加資料
        private List<ImagePath> savePathList = new List<ImagePath>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                DListModel.DataBind(); //先取得型號預設選取值
                DListDetailTitle.DataBind(); //先取得細節標題預設選取值
                loadImageList(); //取得 Layout 組圖
                loadDetailList(); //取得標題細節

                int i = 0;
                
                for (; i < 5;) {

                    i++;
                }
            }
        }

        #region Group Image List
        private void loadImageList()
        {
            //依型號取得組圖圖片資料
            string selectModel_id = DListModel.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlLoad = "SELECT layoutDeckPlanImgPathJSON FROM Yachts WHERE id = @selectModel_id";
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                //將特殊符號解碼
                string loadJson = HttpUtility.HtmlDecode(reader["layoutDeckPlanImgPathJSON"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            connection.Close();

            //渲染圖片選項
            if (savePathList?.Count > 0) {
                foreach (var item in savePathList) {
                    ListItem listItem = new ListItem($"<img src='/Tayanahtml/upload/Images/{item.SavePath}' alt='thumbnail' class='img-thumbnail' width='250px'/>", item.SavePath);
                    RadioButtonListImg.Items.Add(listItem);
                }
            }

            DelImageBtn.Visible = false; //刪除鈕有選擇圖片時才顯示
        }

        protected void UploadImgBtn_Click(object sender, EventArgs e)
        {
            //有選擇檔案才執行
            if (imageUpload.HasFile) {
                //取得上傳檔案大小 (限制 10MB)
                int fileSize = imageUpload.PostedFile.ContentLength;
                if (fileSize < 1024 * 1000 * 10) {
                    //先讀取資料庫原有資料
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
                        savePathList.Add(new ImagePath { SavePath = fileName });

                        //壓縮圖檔
                        var image = NetVips.Image.NewFromFile(savePath + "temp" + fileName);
                        if (image.Width > 672 * 2) {
                            var newImg = image.Resize(0.5);
                            while (newImg.Width > 672 * 2) {
                                newImg = newImg.Resize(0.5);
                            }
                            newImg.WriteToFile(savePath + fileName);
                        }
                        else {
                            postedFile.SaveAs(savePath + fileName);
                        }
                        File.Delete(savePath + "temp" + fileName);
                    }

                    //依遊艇型號更新資料
                    string selectModel_id = DListModel.SelectedValue;
                    //將 List<T> 資料轉為 JSON 格式字串
                    string savePathJsonStr = JsonConvert.SerializeObject(savePathList);
                    SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                    string sql = "UPDATE Yachts SET layoutDeckPlanImgPathJSON = @layoutDeckPlanImgPathJSON WHERE id = @selectModel_id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@layoutDeckPlanImgPathJSON", savePathJsonStr);
                    command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    
                    //渲染畫面
                    RadioButtonListImg.Items.Clear();
                    loadImageList();
                }
                else {
                    Response.Write("<script>alert('*The maximum upload size is 10MB!');</script>");
                }
                
            }
        }

        // Layout 圖片 JSON 資料
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
            //依選取項目刪除 List<T> 資料
            loadImageList(); //先取得 List<T> 資料
            string selImageStr = RadioButtonListImg.SelectedValue;
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
            File.Delete(savePath + selImageStr);
            for (int i = 0; i < savePathList.Count; i++) {
                if (savePathList[i].SavePath.Equals(selImageStr)) {
                    savePathList.RemoveAt(i);
                }
            }

            //將 List<T> 資料轉為 JSON 格式字串
            string savePathJsonStr = JsonConvert.SerializeObject(savePathList);
            string selectModel_id = DListModel.SelectedValue;

            //依選取型號更新圖檔資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "UPDATE Yachts SET layoutDeckPlanImgPathJSON = @layoutDeckPlanImgPathJSON WHERE id = @selectModel_id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@layoutDeckPlanImgPathJSON", savePathJsonStr);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染畫面
            RadioButtonListImg.Items.Clear();
            loadImageList();
        }
        #endregion

        protected void DListModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //刷新全部選項
            RadioButtonListImg.Items.Clear();
            RadioButtonListDetail.Items.Clear();
            loadImageList();
            loadDetailList();
        }

        protected void DListDetailTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            //刷新細節選項
            RadioButtonListDetail.Items.Clear();
            loadDetailList();
        }

        protected void RadioButtonListD_SelectedIndexChanged(object sender, EventArgs e)
        {
            //顯示細節項目刪除按鈕
            BtnDelDetail.Visible = true;
        }

        private void loadDetailList()
        {
            //取得 Model 代表 id
            string selectModel_id = DListModel.SelectedValue;
            //取得 Title 代表 id
            string selectTitle_id = DListDetailTitle.SelectedValue;

            //依遊艇型號 id 及標題 id 取得 Detail
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT detail FROM Specification WHERE yachtModel_ID = @selectModel_id AND detailTitleSort_ID = @selectTitle_id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            command.Parameters.AddWithValue("@selectTitle_id", selectTitle_id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                string detail = reader["detail"].ToString();
                //將轉成字元實體的編碼轉回 HTML 標籤語法渲染
                ListItem listItem = new ListItem(HttpUtility.HtmlDecode(detail), detail);
                RadioButtonListDetail.Items.Add(listItem);
            }
            connection.Close();

            BtnDelDetail.Visible = false; //刪除鈕有選擇項目時才顯示
        }

        protected void BtnAddDetail_Click(object sender, EventArgs e)
        {
            //取得新增 Detail
            string newDetailStr = TboxDetail.Text;
            //將換行跳脫字元改成 HTML 換行標籤
            newDetailStr = newDetailStr.Replace("\r\n", "<br>");

            //依取得下拉選項的值 (id) 存入 Detail 資料
            string selectModel_id = DListModel.SelectedValue;
            string selectTitle_id = DListDetailTitle.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "INSERT INTO Specification (yachtModel_ID, detailTitleSort_ID, detail) VALUES (@selectModel_id, @selectTitle_id, @detail)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            command.Parameters.AddWithValue("@selectTitle_id", selectTitle_id);
            //特殊符號要轉成字元實體才能正常存進資料庫
            command.Parameters.AddWithValue("@detail", HttpUtility.HtmlEncode(newDetailStr));
            connection.Open();
            command.ExecuteNonQuery(); 
            connection.Close();

            //將改成 HTML 換行標籤資料加入選項渲染畫面
            ListItem listItem = new ListItem(newDetailStr, newDetailStr);
            RadioButtonListDetail.Items.Add(listItem);
            TboxDetail.Text = "";
        }


        protected void BtnDelDetail_Click(object sender, EventArgs e)
        {
            //依選取資料刪除 Detail 資料
            string selectModel_id = DListModel.SelectedValue;
            string selectTitle_id = DListDetailTitle.SelectedValue;
            string selectDetailStr = RadioButtonListDetail.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "DELETE FROM Specification WHERE yachtModel_ID = @selectModel_id AND detailTitleSort_ID = @selectTitle_id AND detail = @selectDetailStr";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            command.Parameters.AddWithValue("@selectTitle_id", selectTitle_id);
            command.Parameters.AddWithValue("@selectDetailStr", selectDetailStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染畫面
            RadioButtonListDetail.Items.Clear();
            loadDetailList();
        }

        protected void BtnAddNewTitle_Click(object sender, EventArgs e)
        {
            //取得輸入標題字串
            string newTitleStr = TBoxAddNewTitle.Text;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "INSERT INTO DetailTitleSort (detailTitleSort) VALUES(@newTitleStr)";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@newTitleStr", newTitleStr);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //單純執行無回傳值
            //7.資料庫關閉
            connection.Close();
            //畫面渲染
            GridView1.DataBind();
            DListDetailTitle.DataBind();
            //下拉選單改為選取最新項
            DListDetailTitle.SelectedIndex = DListDetailTitle.Items.Count - 1;
            //清空輸入欄位
            TBoxAddNewTitle.Text = "";
            //清空細節項目
            RadioButtonListDetail.Items.Clear();
        }

        protected void DeltedTitle(object sender, GridViewDeletedEventArgs e)
        {
            //刷新下拉選單
            DListDetailTitle.DataBind();
            //刷新細節項目
            RadioButtonListDetail.Items.Clear();
            RadioButtonListDetail.DataBind();
            loadDetailList();
        }

        protected void UpdatedTitle(object sender, GridViewUpdatedEventArgs e)
        {
            //刷新下拉選單
            DListDetailTitle.DataBind();
            //刷新細節項目
            RadioButtonListDetail.Items.Clear();
            RadioButtonListDetail.DataBind();
            loadDetailList();
        }
    }
}