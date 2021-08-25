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
        //宣告 List 方便用 Add 依序添加欄位資料
        private List<RowData> saveRowList = new List<RowData>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                ckfinderSetPath();
                DListModel.DataBind(); //先綁定取得選取值
                loadContent(); //取得尺寸欄位區外的主要內容
                loadRowList(); //取得尺寸欄位內容
                renderRowList(); //渲染尺寸欄位內容
            }
        }

        private void loadContent()
        {
            //清空畫面資料
            TBoxVideo.Text = "";
            TBoxDLTitle.Text = "";
            TBoxDimImg.Text = "";
            TBoxDLFile.Text = "";
            PDFpreview.Text = "";
            LiteralDimImg.Text = "";

            //依下拉選單選取型號取出資料
            string selectModel_id = DListModel.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT * FROM Yachts WHERE id = @selectModel_id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                //渲染畫面
                CKEditorControl1.Text = HttpUtility.HtmlDecode(reader["overviewContentHtml"].ToString());
                string imgPathStr = reader["overviewDimensionsImgPath"].ToString();
                string filePathStr = reader["overviewDownloadsFilePath"].ToString();
                TBoxDimImg.Text = imgPathStr;
                LiteralDimImg.Text = $"<img alt='Dimensions Image' class='img-thumbnail rounded mx-auto d-block' src='/Tayanahtml/upload/Images/{imgPathStr}' Width='250px'/>";
                TBoxDLFile.Text = filePathStr;
                PDFpreview.Text = $"<object type='application/pdf' data='/Tayanahtml/upload/files/{filePathStr}' width='250px' height='385px' class='rounded mx-auto d-block' ></object>";
            }
            connection.Close();
        }

        protected void BtnUploadMainContent_Click(object sender, EventArgs e)
        {
            //將文字編輯器 HTML 內容轉為 HTML 字元編碼
            string mainContentHtmlStr = HttpUtility.HtmlEncode(CKEditorControl1.Text);
            //依下拉選單選取型號存入型號介紹主要圖文內容
            string selectModel_id = DListModel.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "UPDATE Yachts SET overviewContentHtml = @mainContentHtmlStr WHERE id = @selectModel_id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@mainContentHtmlStr", mainContentHtmlStr);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染上傳成功提示
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
            //隱藏上傳成功提示
            LabUploadMainContent.Visible = false;
            LabUpdateDimensionsList.Visible = false;
            //渲染畫面
            loadContent();
            loadRowList();
            renderRowList();
        }

        protected void BtnUploadDimImg_Click(object sender, EventArgs e)
        {
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
            string fileName = DimImgUpload.FileName;
            string selectModel_id = DListModel.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //有選檔案才可上傳，沒選檔案則執行清空
            if (DimImgUpload.HasFile) {
                string sqlDel = "SELECT overviewDimensionsImgPath FROM Yachts WHERE id = @selectModel_id";
                SqlCommand commandDel = new SqlCommand(sqlDel, connection);
                commandDel.Parameters.AddWithValue("@selectModel_id", selectModel_id);

                //刪除舊圖檔
                connection.Open();
                SqlDataReader reader = commandDel.ExecuteReader();
                if (reader.Read()) {
                    string delFileName = reader["overviewDimensionsImgPath"].ToString();
                    if (!String.IsNullOrEmpty(delFileName)) {
                        File.Delete(savePath + delFileName);
                    }
                }
                connection.Close();

                //儲存圖片檔案及圖片名稱
                //檢查專案資料夾內有無同名檔案，有同名就加流水號
                DirectoryInfo directoryInfo = new DirectoryInfo(savePath);
                string[] fileNameArr = fileName.Split('.');
                int count = 0;
                foreach (var fileItem in directoryInfo.GetFiles()) {
                    if (fileItem.Name.Contains(fileNameArr[0])) {
                        count++;
                    }
                }
                fileName = fileNameArr[0] + $"({count + 1})." + fileNameArr[1];
                DimImgUpload.SaveAs(savePath + fileName);

                //更新資料
                string sql = "UPDATE Yachts SET overviewDimensionsImgPath = @fileName WHERE id = @selectModel_id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fileName", fileName);
                command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                //渲染畫面
                loadContent();
                loadRowList();
                renderRowList();
            }
            else {
                //沒選檔案點上傳則清空
                string sql = "UPDATE Yachts SET overviewDimensionsImgPath = @imgPath WHERE id = @selectModel_id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@imgPath", "");
                command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                //渲染畫面
                loadContent();
                loadRowList();
                renderRowList();
            }
        }

        protected void BtnUploadFile_Click(object sender, EventArgs e)
        {
            string savePath = Server.MapPath("~/Tayanahtml/upload/files/");
            string fileName = FileUpload.FileName;
            string selectModel_id = DListModel.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //有選檔案才可上傳
            if (FileUpload.HasFile) {
                //先刪除舊檔
                string sqlDel = "SELECT overviewDownloadsFilePath FROM Yachts WHERE id = @selectModel_id";
                SqlCommand commandDel = new SqlCommand(sqlDel, connection);
                commandDel.Parameters.AddWithValue("@selectModel_id", selectModel_id);
                connection.Open();
                SqlDataReader reader = commandDel.ExecuteReader();
                if (reader.Read()) {
                    string delFileName = reader["overviewDownloadsFilePath"].ToString();
                    if (!String.IsNullOrEmpty(delFileName)) {
                        File.Delete(savePath + delFileName);
                    }
                }
                connection.Close();

                //儲存上傳檔案
                //檢查專案資料夾內有無同名檔案，有同名就加流水號
                DirectoryInfo directoryInfo = new DirectoryInfo(savePath);
                int count = 0;
                foreach (var fileItem in directoryInfo.GetFiles()) {
                    if (fileItem.Name.Contains(fileName)) {
                        count++;
                    }
                }
                string[] fileNameArr = fileName.Split('.');
                fileName = fileNameArr[0] + $"({count + 1})." + fileNameArr[1];
                FileUpload.SaveAs(savePath + fileName);
                
                //更新資料庫資料
                string sql = "UPDATE Yachts SET overviewDownloadsFilePath = @fileName WHERE id = @selectModel_id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fileName", fileName);
                command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                //渲染畫面
                loadContent();
                loadRowList();
                renderRowList();
            }
            else {
                //沒選檔案則清空資料
                string sqlDelPath = "UPDATE Yachts SET overviewDownloadsFilePath = @fileName WHERE id = @selectModel_id";
                SqlCommand commandDelPath = new SqlCommand(sqlDelPath, connection);
                commandDelPath.Parameters.AddWithValue("@fileName", "");
                commandDelPath.Parameters.AddWithValue("@selectModel_id", selectModel_id);
                connection.Open();
                commandDelPath.ExecuteNonQuery();
                connection.Close();

                //渲染畫面
                loadContent();
                loadRowList();
                renderRowList();
            }
        }

        private void loadRowList()
        {
            //依選取型號取得欄位資料更新 List<T>
            string selectModel_id = DListModel.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT * FROM Yachts WHERE id = @selectModel_id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["overviewDimensionsJSON"].ToString());
                saveRowList = JsonConvert.DeserializeObject<List<RowData>>(loadJson);
            }
            connection.Close();
        }

        protected void AddRow_Click(object sender, EventArgs e)
        {
            //將 JSON 資料載入 List<T>
            loadRowList();
            //增加新欄位
            saveRowList.Add(new RowData { SaveItem = "", SaveValue = "" });
            //更新資料庫資料
            uploadRowList();
            //渲染畫面
            renderRowList();
            //將畫面移至出現上傳按鈕處
            Page.SetFocus(BtnUpdateDimensionsList);
        }

        private void uploadRowList()
        {
            //先更新 List<T> 前兩個資料 Video 及 Download File
            saveRowList[0].SaveValue = TBoxVideo.Text;
            saveRowList[1].SaveValue = TBoxDLTitle.Text;
            //更新 List<T> 資料，欄位內容用 Request.Form 
            for (int i = 2; i < saveRowList.Count; i++) {
                saveRowList[i].SaveItem = Request.Form[$"item{i}"];
                saveRowList[i].SaveValue = Request.Form[$"value{i}"];
            }

            //依選取型號更新資料庫資料
            string selectModel_id = DListModel.SelectedValue;
            //將 List<T> 資料轉為 JSON 格式字串
            string saveRowListJsonStr = JsonConvert.SerializeObject(saveRowList);
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "UPDATE Yachts SET overviewDimensionsJSON = @saveRowListJsonStr WHERE id = @selectModel_id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@saveRowListJsonStr", saveRowListJsonStr);
            command.Parameters.AddWithValue("@selectModel_id", selectModel_id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        //欄位表 JSON 資料
        public class RowData
        {
            public string SaveItem { get; set; }
            public string SaveValue { get; set; }
        }

        protected void BtnUpdateDimensionsList_Click(object sender, EventArgs e)
        {
            //將 JSON 資料載入 List<T>
            loadRowList();
            //更新資料庫資料
            uploadRowList();
            //渲染表格畫面
            renderRowList();
            //渲染上傳成功提示
            DateTime nowtime = DateTime.Now;
            LabUpdateDimensionsList.Visible = true;
            LabUpdateDimensionsList.Text = "*Upload Success! - " + nowtime.ToString("G");
        }

        protected void DeleteRow_Click(object sender, EventArgs e)
        {
            //將 JSON 資料載入 List<T>
            loadRowList();
            //更新資料庫資料
            uploadRowList();
            //刪除尺寸欄位最末欄
            saveRowList.RemoveAt(saveRowList.Count - 1);
            //更新資料庫資料
            uploadRowList();
            //渲染表格畫面
            renderRowList();
            //將畫面移至出現上傳按鈕處
            Page.SetFocus(BtnUpdateDimensionsList);
        }

        private void renderRowList()
        {
            if (saveRowList?.Count > 0) {
                string addRowHtmlStr = "";
                int index = 0;
                //從 List<T> 載入資料
                foreach (var item in saveRowList) {
                    if (index == 0) {
                        TBoxVideo.Text = item.SaveValue;
                    }
                    if (index == 1) {
                        TBoxDLTitle.Text = item.SaveValue;
                    }
                    if (index > 1) {
                        addRowHtmlStr += $"<tr class='table-info'><th><input id='item{index}' name='item{index}' type='text' class='form-control font-weight-bold' value='{item.SaveItem}' /></th><td><input id='value{index}' name='value{index}' type='text' class='form-control' value='{item.SaveValue}' /></td></tr>";
                    }
                    index++;
                }
                //渲染畫面
                LitDimensionsHtml.Text = addRowHtmlStr;
            }
        }

    }
}