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
        //宣告List方便用Add依序添加資料
        private List<ImagePath> savePathList = new List<ImagePath>();
        protected void Page_Load(object sender, EventArgs e)
        {
            //權限關門判斷 (Cookie)
            if (!User.Identity.IsAuthenticated) {
                Response.Redirect("Manager_SignIn.aspx"); //導回登入頁
            }
            if (!IsPostBack) {
                DropDownList1.DataBind();
                loadImageList();
            }
        }

        protected void BtnAddYacht_Click(object sender, EventArgs e)
        {
            string yachtModelStr = TBoxAddYachtModel.Text + " " + TBoxAddYachtLength.Text;
            string guidStr = Guid.NewGuid().ToString().Trim();
            string isNewDesign = CBoxNewDesign.Checked.ToString();
            string isNewBuilding = CBoxNewBuilding.Checked.ToString();

            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "INSERT INTO Yachts (yachtModel, isNewDesign, isNewBuilding, guid) VALUES (@yachtModel, @isNewDesign, @isNewBuilding, @guid)";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@yachtModel", yachtModelStr);
            command.Parameters.AddWithValue("@isNewDesign", isNewDesign);
            command.Parameters.AddWithValue("@isNewBuilding", isNewBuilding);
            command.Parameters.AddWithValue("@guid", guidStr);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //無回傳值
            //7.資料庫關閉
            connection.Close();
            //畫面渲染
            DropDownList1.DataBind();
            GridView1.DataBind();
            TBoxAddYachtModel.Text = "";
            TBoxAddYachtLength.Text = "";
            CBoxNewDesign.Checked = false;
            CBoxNewBuilding.Checked = false;
            DropDownList1.SelectedValue = yachtModelStr;
            RadioButtonListH.Items.Clear();
        }

        protected void DeletingModel(object sender, GridViewDeleteEventArgs e)
        {
            string idStr = "";
            foreach (DictionaryEntry entry in e.Keys) {
                idStr = entry.Value.ToString();
            }
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlLoad = "SELECT bannerImgPathJSON FROM Yachts WHERE id = @id";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            //4.參數化
            command.Parameters.AddWithValue("@id", idStr);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["bannerImgPathJSON"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
            for (int i = 0; i < savePathList.Count; i++) {
                File.Delete(savePath + savePathList[i].SavePath);
            }
            
        }

        protected void DeletedModel(object sender, GridViewDeletedEventArgs e)
        {
            RadioButtonListH.Items.Clear();
            DropDownList1.DataBind();
            loadImageList();
        }

        #region Group Image List
        private void loadImageList()
        {
            string selModelStr = DropDownList1.SelectedValue;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlLoad = "SELECT bannerImgPathJSON FROM Yachts WHERE yachtModel = @yachtModel";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            //4.參數化
            command.Parameters.AddWithValue("@yachtModel", selModelStr);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["bannerImgPathJSON"].ToString());
                //反序列化JSON格式
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (savePathList.Count > 0) {
                bool firstCheck = true;
                foreach (var item in savePathList) {
                    if (firstCheck) {
                        ListItem listItem = new ListItem($"<img src='../Tayanahtml/upload/Images/{item.SavePath}' alt='thumbnail' class='img-thumbnail bg-info' width='200'/>", item.SavePath);
                        RadioButtonListH.Items.Add(listItem);
                        firstCheck = false;
                    }
                    else {
                        ListItem listItem = new ListItem($"<img src='../Tayanahtml/upload/Images/{item.SavePath}' alt='thumbnail' class='img-thumbnail' width='200'/>", item.SavePath);
                        RadioButtonListH.Items.Add(listItem);
                    }
                }
            }
        }

        protected void UploadHBtn_Click(object sender, EventArgs e)
        {
            if (imageUploadH.HasFile) {
                //取得上傳檔案大小
                int fileSize = imageUploadH.PostedFile.ContentLength;
                if (fileSize < 1024*10*1000) {
                    loadImageList();
                    DelHImageBtn.Visible = false;
                    //添加圖檔資料
                    foreach (HttpPostedFile postedFile in imageUploadH.PostedFiles) {
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
                    string selModelStr = DropDownList1.SelectedValue;
                    //1.連線資料庫
                    SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                    //2.sql語法
                    string sql = "UPDATE Yachts SET bannerImgPathJSON = @bannerImgPathJSON WHERE yachtModel = @yachtModel";
                    //3.創建command物件
                    SqlCommand command = new SqlCommand(sql, connection);
                    //4.參數化
                    command.Parameters.AddWithValue("@bannerImgPathJSON", savePathJsonStr);
                    command.Parameters.AddWithValue("@yachtModel", selModelStr);
                    //5.資料庫連線開啟
                    connection.Open();
                    //6.執行sql (新增刪除修改)
                    command.ExecuteNonQuery();
                    //7.資料庫關閉
                    connection.Close();
                    RadioButtonListH.Items.Clear();
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
            DelHImageBtn.Visible = true;
        }

        protected void DelHImageBtn_Click(object sender, EventArgs e)
        {
            string selImageStr = RadioButtonListH.SelectedValue;
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
            string selModelStr = DropDownList1.SelectedValue;
            //2.sql語法
            string sql = "UPDATE Yachts SET bannerImgPathJSON = @bannerImgPathJSON WHERE yachtModel = @yachtModel";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@bannerImgPathJSON", savePathJsonStr);
            command.Parameters.AddWithValue("@yachtModel", selModelStr); ;
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery();
            //7.資料庫關閉
            connection.Close();
            //渲染畫面
            RadioButtonListH.Items.Clear();
            loadImageList();
            DelHImageBtn.Visible = false;
        }
        #endregion

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioButtonListH.Items.Clear();
            loadImageList();
        }

        protected void UpdatedModel(object sender, GridViewUpdatedEventArgs e)
        {
            DropDownList1.DataBind();
        }

    }
}