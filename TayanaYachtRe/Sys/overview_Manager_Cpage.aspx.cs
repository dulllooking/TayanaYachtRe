using CKFinder;
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
    public partial class overview_Manager_Cpage : System.Web.UI.Page
    {
        //宣告List方便用Add依序添加資料
        private List<RowData> saveRowList = new List<RowData>();
        protected void Page_Load(object sender, EventArgs e)
        {
            //權限關門判斷 (Cookie)
            
            if (!IsPostBack) {
                ckfinderSetPath();
                DListModel.DataBind();
                loadContent();
                loadRowList();
                renderRowList();
            }
        }

        private void loadContent()
        {
            TBoxVideo.Text = "";
            TBoxDLTitle.Text = "";
            TBoxDimImg.Text = "";
            TBoxDLFile.Text = "";
            PDFpreview.Text = "";
            DimensionsImg.ImageUrl = "";
            string selectModelStr = DListModel.SelectedValue;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "SELECT * FROM Yachts WHERE yachtModel = @selectModelStr";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@selectModelStr", selectModelStr);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader(); //指標指在BOF(表格之上)
            string imgPathStr = "";
            string filePathStr = "";
            while (reader.Read()) {
                CKEditorControl1.Text = HttpUtility.HtmlDecode(reader["overviewContentHtml"].ToString());
                imgPathStr = reader["overviewDimensionsImgPath"].ToString();
                filePathStr = reader["overviewDownloadsFilePath"].ToString();
            }
            //資料庫關閉
            connection.Close();
            //畫面渲染
            if (!imgPathStr.Equals("")) {
                string[] fileNameArr = imgPathStr.Split('/');
                TBoxDimImg.Text = fileNameArr[fileNameArr.Length - 1];
            }
            if (!filePathStr.Equals("")) {
                string[] fileNameArr = filePathStr.Split('/');
                TBoxDLFile.Text = fileNameArr[fileNameArr.Length - 1];
                string embed = "<object type='application/pdf' data='{0}' width='250' height='385' class='rounded mx-auto d-block' ></object>";
                PDFpreview.Text = string.Format(embed, ResolveUrl($"~/Tayanahtml/{filePathStr}"));
            }
            DimensionsImg.ImageUrl = $"~/Tayanahtml/{imgPathStr}";
        }

        protected void BtnUploadMainContent_Click(object sender, EventArgs e)
        {
            string yachtModel = DListModel.SelectedValue;
            string mainContentHtmlStr = HttpUtility.HtmlEncode(CKEditorControl1.Text);
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "UPDATE Yachts SET overviewContentHtml = @overviewContentHtml WHERE yachtModel = @yachtModel";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@overviewContentHtml", mainContentHtmlStr);
            command.Parameters.AddWithValue("@yachtModel", yachtModel);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //無回傳值
            //7.資料庫關閉
            connection.Close();

            DateTime nowtime = DateTime.Now;
            LabUploadMainContent.Visible = true;
            LabUploadMainContent.Text = "*Upload Success! - " + nowtime.ToString("G");
        }

        private void ckfinderSetPath()
        {
            FileBrowser fileBrowser = new FileBrowser();
            fileBrowser.BasePath = "/ckfinder";
            fileBrowser.SetupCKEditor(CKEditorControl1);
        }

        protected void DListModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            LabUploadMainContent.Visible = false;
            LabUpdateDimensionsList.Visible = false;
            loadContent();
            loadRowList();
            renderRowList();
        }

        protected void BtnUploadDimImg_Click(object sender, EventArgs e)
        {
            //需填完整路徑，結尾反斜線如果沒加要用Path.Combine()可自動添加
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
            //有選檔案才可上傳
            if (DimImgUpload.HasFile) {
                string fileName = DimImgUpload.FileName;
                string yachtModel = DListModel.SelectedValue;
                //1.連線資料庫
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                //2.sql語法
                string sqlDel = "SELECT overviewDimensionsImgPath FROM Yachts WHERE yachtModel = @yachtModel";
                string sql = "UPDATE Yachts SET overviewDimensionsImgPath = @path WHERE yachtModel = @yachtModel";
                //3.創建command物件
                SqlCommand commandDel = new SqlCommand(sqlDel, connection);
                SqlCommand command = new SqlCommand(sql, connection);
                //4.參數化
                command.Parameters.AddWithValue("@path", $"upload/Images/{fileName}");
                command.Parameters.AddWithValue("@yachtModel", yachtModel);
                commandDel.Parameters.AddWithValue("@yachtModel", yachtModel);

                //刪除舊圖檔
                connection.Open();
                SqlDataReader reader = commandDel.ExecuteReader();
                if (reader.Read()) {
                    string delFileName = reader["overviewDimensionsImgPath"].ToString();
                    if (!delFileName.Equals("")) {
                        string delPath = Server.MapPath("~/Tayanahtml/");
                        delPath += delFileName;
                        File.Delete(delPath);
                    }
                }
                connection.Close();

                //存圖
                savePath += fileName;
                DimImgUpload.SaveAs(savePath);

                //更新資料
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                //畫面渲染
                TBoxDimImg.Text = fileName;
                DimensionsImg.ImageUrl = $"~/Tayanahtml/upload/Images/{fileName}";
            }
        }

        protected void BtnUploadFile_Click(object sender, EventArgs e)
        {
            //需填完整路徑，結尾反斜線如果沒加要用Path.Combine()可自動添加
            string savePath = Server.MapPath("~/Tayanahtml/upload/files/");
            string fileName = FileUpload.FileName;
            string yachtModel = DListModel.SelectedValue;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlDel = "SELECT overviewDownloadsFilePath FROM Yachts WHERE yachtModel = @yachtModel";
            string sql = "UPDATE Yachts SET overviewDownloadsFilePath = @path WHERE yachtModel = @yachtModel";
            //3.創建command物件
            SqlCommand commandDel = new SqlCommand(sqlDel, connection);
            SqlCommand command = new SqlCommand(sql, connection);
            
            command.Parameters.AddWithValue("@yachtModel", yachtModel);
            commandDel.Parameters.AddWithValue("@yachtModel", yachtModel);
            //刪除圖檔
            connection.Open();
            SqlDataReader reader = commandDel.ExecuteReader();
            if (reader.Read()) {
                string delFileName = reader["overviewDownloadsFilePath"].ToString();
                if (!delFileName.Equals("")) {
                    string delPath = Server.MapPath("~/Tayanahtml/");
                    delPath += delFileName;
                    File.Delete(delPath);
                }
            }
            connection.Close();

            //有選檔案才可上傳
            if (FileUpload.HasFile) {
                //檢查專案資料夾內有無同名檔案
                DirectoryInfo directoryInfo = new DirectoryInfo(Server.MapPath("~/Tayanahtml/upload/files/"));
                foreach (var fileItem in directoryInfo.GetFiles()) {
                    if (fileItem.Name.Equals(fileName)) {
                        string[] fileNameArr = fileName.Split('.');
                        fileName = fileNameArr[0] + "(1)." + fileNameArr[1];
                    }
                }
                //存檔
                savePath += fileName;
                FileUpload.SaveAs(savePath);
                
                //4.參數化
                command.Parameters.AddWithValue("@path", $"upload/files/{fileName}");
                //更新資料
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                //畫面渲染
                loadContent();
                loadRowList();
                renderRowList();
            }
            else {
                string sqlDelPath = "UPDATE Yachts SET overviewDownloadsFilePath = @path WHERE yachtModel = @yachtModel";
                SqlCommand commandDelPath = new SqlCommand(sqlDelPath, connection);
                //4.參數化
                commandDelPath.Parameters.AddWithValue("@path", "");
                commandDelPath.Parameters.AddWithValue("@yachtModel", yachtModel);
                //更新資料
                connection.Open();
                commandDelPath.ExecuteNonQuery();
                connection.Close();
                //畫面渲染
                loadContent();
                loadRowList();
                renderRowList();
            }
        }

        private void loadRowList()
        {
            string selectModelStr = DListModel.SelectedValue;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "SELECT * FROM Yachts WHERE yachtModel = @selectModelStr";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@selectModelStr", selectModelStr);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader(); //指標指在BOF(表格之上)
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["overviewDimensionsJSON"].ToString());
                //反序列化JSON格式
                saveRowList = JsonConvert.DeserializeObject<List<RowData>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
        }
        protected void AddRow_Click(object sender, EventArgs e)
        {
            //將Json資料載入List
            loadRowList();
            //增加欄位
            saveRowList.Add(new RowData { SaveItem = "", SaveValue = "" });
            //更新資料庫資料
            uploadRowList();
            //渲染畫面
            renderRowList();
        }

        private void uploadRowList()
        {
            saveRowList[0].SaveValue = TBoxVideo.Text;
            saveRowList[1].SaveValue = TBoxDLTitle.Text;
            //更新資料
            int saveRowListCount = saveRowList.Count;
            for (int i = 2; i < saveRowListCount; i++) {
                saveRowList[i].SaveItem = Request.Form[$"item{i}"];
                saveRowList[i].SaveValue = Request.Form[$"value{i}"];
            }
            string yachtModel = DListModel.SelectedValue;
            //將List資料轉為Json格式字串
            string saveRowListJsonStr = JsonConvert.SerializeObject(saveRowList);
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "UPDATE Yachts SET overviewDimensionsJSON = @overviewDimensionsJSON WHERE yachtModel = @yachtModel";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@overviewDimensionsJSON", saveRowListJsonStr);
            command.Parameters.AddWithValue("@yachtModel", yachtModel);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery();
            //7.資料庫關閉
            connection.Close();
        }

        //JSON資料
        public class RowData
        {
            public string SaveItem { get; set; }
            public string SaveValue { get; set; }
        }

        protected void BtnUpdateDimensionsList_Click(object sender, EventArgs e)
        {
            //將Json資料載入List
            loadRowList();
            //更新資料庫資料
            uploadRowList();
            //渲染畫面
            renderRowList();

            DateTime nowtime = DateTime.Now;
            LabUpdateDimensionsList.Visible = true;
            LabUpdateDimensionsList.Text = "*Upload Success! - " + nowtime.ToString("G");
        }

        protected void DeleteRow_Click(object sender, EventArgs e)
        {
            //將Json資料載入List
            loadRowList();
            //更新資料庫資料
            uploadRowList();
            //刪除末欄
            saveRowList.RemoveAt(saveRowList.Count - 1);
            //更新資料庫資料
            uploadRowList();
            //渲染畫面
            renderRowList();
        }

        private void renderRowList()
        {
            string addRowHtmlStr = "";
            if (saveRowList.Count > 0) {
                int index = 0;
                //從List載入舊有資料
                foreach (var item in saveRowList) {
                    if (index == 0) {
                        TBoxVideo.Text = item.SaveValue;
                    }
                    if (index == 1) {
                        TBoxDLTitle.Text = item.SaveValue;
                    }
                    if (index > 1) {
                        addRowHtmlStr += $"<tr><td><input id='item{index}' name='item{index}' type='text' class='form-control' value='{item.SaveItem}' /></td><td><input id='value{index}' name='value{index}' type='text' class='form-control' value='{item.SaveValue}' /></td></tr>";
                    }
                    index++;
                }
                //渲染畫面
                LitDimensionsHtml.Text = addRowHtmlStr;
            }
        }

    }
}