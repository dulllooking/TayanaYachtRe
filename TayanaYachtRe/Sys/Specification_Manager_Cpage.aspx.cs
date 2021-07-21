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
        //宣告List方便用Add依序添加資料
        private List<ImagePath> savePathList = new List<ImagePath>();
        protected void Page_Load(object sender, EventArgs e)
        {
            //權限關門判斷 (Cookie)
            if (!User.Identity.IsAuthenticated) {
                Response.Redirect("Manager_SignIn.aspx"); //導回登入頁
            }
            if (!IsPostBack) {
                DListModel.DataBind();
                DListDetailTitle.DataBind();
                loadImageList();
                loadDetailList();
            }
        }

        #region Group Image List
        private void loadImageList()
        {
            string selectModelStr = DListModel.SelectedValue;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlLoad = "SELECT layoutDeckPlanImgPathJSON FROM Yachts WHERE yachtModel = @yachtModel";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            //4.參數化
            command.Parameters.AddWithValue("@yachtModel", selectModelStr);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["layoutDeckPlanImgPathJSON"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (savePathList.Count > 0) {
                foreach (var item in savePathList) {
                    ListItem listItem = new ListItem($"<img src='../Tayanahtml/upload/Images/{item.SavePath}' alt='thumbnail' class='img-thumbnail' width='250'/>", item.SavePath);
                    RadioButtonListImg.Items.Add(listItem);
                }
            }
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
                    //將List資料轉為Json格式字串
                    string savePathJsonStr = JsonConvert.SerializeObject(savePathList);
                    string selectModelStr = DListModel.SelectedValue;
                    //1.連線資料庫
                    SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                    //2.sql語法
                    string sql = "UPDATE Yachts SET layoutDeckPlanImgPathJSON = @layoutDeckPlanImgPathJSON WHERE yachtModel = @yachtModel";
                    //3.創建command物件
                    SqlCommand command = new SqlCommand(sql, connection);
                    //4.參數化
                    command.Parameters.AddWithValue("@layoutDeckPlanImgPathJSON", savePathJsonStr);
                    command.Parameters.AddWithValue("@yachtModel", selectModelStr);
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
            string selectModelStr = DListModel.SelectedValue;
            //2.sql語法
            string sql = "UPDATE Yachts SET layoutDeckPlanImgPathJSON = @layoutDeckPlanImgPathJSON WHERE yachtModel = @yachtModel";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@layoutDeckPlanImgPathJSON", savePathJsonStr);
            command.Parameters.AddWithValue("@yachtModel", selectModelStr);
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

        protected void DListModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioButtonListImg.Items.Clear();
            RadioButtonListD.Items.Clear();
            loadImageList();
            loadDetailList();
            DListDetailTitle.DataBind();
        }

        protected void DListDetailTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioButtonListD.Items.Clear();
            loadDetailList();
        }

        private void loadDetailList()
        {
            string selectModelStr = DListModel.SelectedValue;
            string selectModelIDStr = "";
            string selectTitleStr = DListDetailTitle.SelectedValue;
            string selectTitleIDStr = "";
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlModel = "SELECT id FROM Yachts WHERE yachtModel = @yachtModel";
            string sqlTitle = "SELECT id FROM DetailTitleSort WHERE detailTitleSort = @detailTitleSort";
            string sql = "SELECT detail FROM Specification WHERE yachtModel_ID = @yachtModel_ID AND detailTitleSort_ID = @detailTitleSort_ID";
            //3.創建command物件
            SqlCommand commandModel = new SqlCommand(sqlModel, connection);
            SqlCommand commandTitle = new SqlCommand(sqlTitle, connection);
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            commandModel.Parameters.AddWithValue("@yachtModel", selectModelStr);
            commandTitle.Parameters.AddWithValue("@detailTitleSort", selectTitleStr);
            //取得Model代表id
            connection.Open();
            SqlDataReader reader = commandModel.ExecuteReader();
            if (reader.Read()) {
                selectModelIDStr = reader["id"].ToString();
            }
            connection.Close();
            //取得Title代表id
            connection.Open();
            SqlDataReader reader2 = commandTitle.ExecuteReader();
            if (reader2.Read()) {
                selectTitleIDStr = reader2["id"].ToString();
            }
            connection.Close();
            //取得Detail
            command.Parameters.AddWithValue("@yachtModel_ID", selectModelIDStr);
            command.Parameters.AddWithValue("@detailTitleSort_ID", selectTitleIDStr);
            connection.Open();
            SqlDataReader reader3 = command.ExecuteReader();
            while (reader3.Read()) {
                ListItem listItem = new ListItem(HttpUtility.HtmlDecode(reader3["detail"].ToString()), reader3["detail"].ToString());
                RadioButtonListD.Items.Add(listItem);
            }
            connection.Close();
        }

        protected void BtnAddDetail_Click(object sender, EventArgs e)
        {
            string selectModelStr = DListModel.SelectedValue;
            string selectModelIDStr = "";
            string selectTitleStr = DListDetailTitle.SelectedValue;
            string selectTitleIDStr = "";
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlModel = "SELECT id FROM Yachts WHERE yachtModel = @yachtModel";
            string sqlTitle = "SELECT id FROM DetailTitleSort WHERE detailTitleSort = @detailTitleSort";
            string sql = "INSERT INTO Specification (yachtModel_ID, detailTitleSort_ID, detail) VALUES (@yachtModel_ID, @detailTitleSort_ID, @detail)";
            //3.創建command物件
            SqlCommand commandModel = new SqlCommand(sqlModel, connection);
            SqlCommand commandTitle = new SqlCommand(sqlTitle, connection);
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            commandModel.Parameters.AddWithValue("@yachtModel", selectModelStr);
            commandTitle.Parameters.AddWithValue("@detailTitleSort", selectTitleStr);
            //取得Model代表id
            connection.Open();
            SqlDataReader reader = commandModel.ExecuteReader();
            if (reader.Read()) {
                selectModelIDStr = reader["id"].ToString();
            }
            connection.Close();
            //取得Title代表id
            connection.Open();
            SqlDataReader reader2 = commandTitle.ExecuteReader();
            if (reader2.Read()) {
                selectTitleIDStr = reader2["id"].ToString();
            }
            connection.Close();
            //新增Detail
            string detailStr = TboxDetail.Text;
            //轉換換行規則
            detailStr = HttpUtility.HtmlEncode(detailStr.Replace("\r\n", "<br>"));
            command.Parameters.AddWithValue("@yachtModel_ID", selectModelIDStr);
            command.Parameters.AddWithValue("@detailTitleSort_ID", selectTitleIDStr);
            command.Parameters.AddWithValue("@detail", detailStr);
            connection.Open();
            command.ExecuteNonQuery(); 
            connection.Close();
            //畫面渲染
            ListItem listItem = new ListItem(HttpUtility.HtmlDecode(detailStr), detailStr);
            RadioButtonListD.Items.Add(listItem);
            TboxDetail.Text = "";
        }

        protected void RadioButtonListD_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnDelDetail.Visible = true;
        }

        protected void BtnDelDetail_Click(object sender, EventArgs e)
        {
            string selectModelStr = DListModel.SelectedValue;
            string selectModelIDStr = "";
            string selectTitleStr = DListDetailTitle.SelectedValue;
            string selectTitleIDStr = "";
            string selectDetailStr = RadioButtonListD.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlModel = "SELECT id FROM Yachts WHERE yachtModel = @yachtModel";
            string sqlTitle = "SELECT id FROM DetailTitleSort WHERE detailTitleSort = @detailTitleSort";
            string sql = "DELETE FROM Specification WHERE yachtModel_ID = @yachtModel_ID AND detailTitleSort_ID = @detailTitleSort_ID AND detail = @detail";
            //3.創建command物件
            SqlCommand commandModel = new SqlCommand(sqlModel, connection);
            SqlCommand commandTitle = new SqlCommand(sqlTitle, connection);
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            commandModel.Parameters.AddWithValue("@yachtModel", selectModelStr);
            commandTitle.Parameters.AddWithValue("@detailTitleSort", selectTitleStr);
            //取得Model代表id
            connection.Open();
            SqlDataReader reader = commandModel.ExecuteReader();
            if (reader.Read()) {
                selectModelIDStr = reader["id"].ToString();
            }
            connection.Close();
            //取得Title代表id
            connection.Open();
            SqlDataReader reader2 = commandTitle.ExecuteReader();
            if (reader2.Read()) {
                selectTitleIDStr = reader2["id"].ToString();
            }
            connection.Close();
            //刪除Detail
            command.Parameters.AddWithValue("@yachtModel_ID", selectModelIDStr);
            command.Parameters.AddWithValue("@detailTitleSort_ID", selectTitleIDStr);
            command.Parameters.AddWithValue("@detail", selectDetailStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            //畫面渲染
            RadioButtonListD.Items.Clear();
            loadDetailList();
            BtnDelDetail.Visible = false;
        }

        protected void BtnAddNewTitle_Click(object sender, EventArgs e)
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "INSERT INTO DetailTitleSort (detailTitleSort) VALUES(@title)";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@title", TBoxAddNewTitle.Text);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //無回傳值
            //7.資料庫關閉
            connection.Close();
            //畫面渲染
            DListDetailTitle.DataBind();
            GridView1.DataBind();
            //清空輸入欄位
            TBoxAddNewTitle.Text = "";
        }

        protected void DeltedTitle(object sender, GridViewDeletedEventArgs e)
        {
            DListDetailTitle.DataBind();
        }
    }
}