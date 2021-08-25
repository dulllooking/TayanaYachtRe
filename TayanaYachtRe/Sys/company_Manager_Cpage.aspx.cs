using CKFinder;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json;
using System.Web.UI.WebControls;

namespace TayanaYachtRe.Sys
{
    public partial class company_Manager_Cpage : System.Web.UI.Page
    {
        //宣告全域 List<T> 可用 Add 依序添加資料
        private List<ImageNameV> saveNameListV = new List<ImageNameV>();
        private List<ImageNameH> saveNameListH = new List<ImageNameH>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                ckfinderSetPath();
                loadCkeditorContent();
                loadCertificatContent();
                loadImageVList();
                loadImageHList();
            }
        }

        private void ckfinderSetPath()
        {
            FileBrowser fileBrowser = new FileBrowser();
            fileBrowser.BasePath = "/ckfinder";
            fileBrowser.SetupCKEditor(CKEditorControl1);
        }

        private void loadCkeditorContent()
        {
            //取得 About Us 頁面 HTML 資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT aboutUsHtml FROM Company WHERE id = 1";
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                //渲染畫面
                CKEditorControl1.Text = HttpUtility.HtmlDecode(reader["aboutUsHtml"].ToString());
            }
            connection.Close();
        }

        protected void UploadAboutUsBtn_Click(object sender, EventArgs e)
        {
            //取得 CKEditorControl 的 HTML 內容
            string aboutUsHtmlStr = HttpUtility.HtmlEncode(CKEditorControl1.Text);
            //更新 About Us 頁面 HTML 資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "UPDATE Company SET aboutUsHtml = @aboutUsHtmlStr WHERE id = 1";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@aboutUsHtmlStr", aboutUsHtmlStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染畫面提示
            DateTime nowtime = DateTime.Now;
            UploadAboutUsLab.Visible = true;
            UploadAboutUsLab.Text = "*Upload Success! - " + nowtime.ToString("G");
        }

        private void loadCertificatContent()
        {
            //取得 Certificat 頁文字說明資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT certificatContent FROM Company WHERE id = 1";
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                //渲染畫面
                certificatTbox.Text = reader["certificatContent"].ToString();
            }
            connection.Close();
        }

        protected void uploadCertificatBtn_Click(object sender, EventArgs e)
        {
            //更新 Certificat 頁文字說明資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "UPDATE Company SET certificatContent = @certificatContent WHERE id = 1";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@certificatContent", certificatTbox.Text);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染畫面提示
            DateTime nowtime = DateTime.Now;
            uploadCertificatLab.Visible = true;
            uploadCertificatLab.Text = "*Upload Success! - " + nowtime.ToString("G");
        }

        #region Vertical Image List

        // JSON 資料 Vertical Image
        public class ImageNameV
        {
            public string SaveName { get; set; }
        }

        private void loadImageVList()
        {
            //連線資料庫取出資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlLoad = "SELECT certificatVerticalImgJSON FROM Company WHERE id = 1";
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = reader["certificatVerticalImgJSON"].ToString();
                //反序列化JSON格式
                saveNameListV = JsonConvert.DeserializeObject<List<ImageNameV>>(loadJson);
            }
            connection.Close();
            //可以改成用 ?.Count 來判斷不是 Null 後才執行 .Count 避免錯誤
            if (saveNameListV?.Count > 0) {
                //逐一取出 JSON 的每筆資料
                foreach (var item in saveNameListV) {
                    //將 RadioButtonList 選項內容改為圖片格式，值設為檔案名稱
                    ListItem listItem = new ListItem($"<img src='/Tayanahtml/images/{item.SaveName}' alt='thumbnail' class='img-thumbnail' width='150px' />", item.SaveName);
                    //加入圖片選項
                    RadioButtonListV.Items.Add(listItem);
                }
            }
            DelVImageBtn.Visible = false; //刪除鈕有選擇圖片時才顯示
        }

        protected void UploadVBtn_Click(object sender, EventArgs e)
        {
            //有選擇檔案才執行
            if (imageUploadV.HasFile) {
                //先讀取資料庫原有資料
                loadImageVList();
                string savePath = Server.MapPath("~/Tayanahtml/images/");

                //添加圖檔資料
                //逐一讀取選擇的圖片檔案
                foreach (HttpPostedFile postedFile in imageUploadV.PostedFiles) {
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
                    //在圖片名稱前加入 temp 標示並儲存圖片檔案
                    postedFile.SaveAs(savePath + "temp" + fileName);
                    //新增 JSON 資料
                    saveNameListV.Add(new ImageNameV { SaveName = fileName });

                    //使用 NetVips 套件進行壓縮圖檔
                    //判斷儲存的原始圖片寬度是否大於設定寬度的 2 倍
                    var img = NetVips.Image.NewFromFile(savePath + "temp" + fileName);
                    if (img.Width > 214 * 2) {
                        //產生原使圖片一半大小的新圖片
                        var newImg = img.Resize(0.5);
                        //如果新圖片寬度還是大於原始圖片設定寬度的 2 倍就持續縮減
                        while (newImg.Width > 214 * 2) {
                            newImg = newImg.Resize(0.5);
                        }
                        //儲存正式名稱的新圖片
                        newImg.WriteToFile(savePath + fileName);
                    }
                    else {
                        postedFile.SaveAs(savePath + fileName);
                    }
                    //刪除原始圖片
                    File.Delete(savePath + "temp" + fileName);
                }

                //更新新增後的圖片名稱 JSON 存入資料庫
                string fileNameJsonStr = JsonConvert.SerializeObject(saveNameListV);
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                string sql = "UPDATE Company SET certificatVerticalImgJSON = @fileNameJsonStr WHERE id = 1";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fileNameJsonStr", fileNameJsonStr);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                //渲染畫面
                RadioButtonListV.Items.Clear();
                loadImageVList();
            }
        }

        protected void RadioButtonListV_SelectedIndexChanged(object sender, EventArgs e)
        {
            //顯示刪除按鈕
            DelVImageBtn.Visible = true;
        }

        protected void DelVImageBtn_Click(object sender, EventArgs e)
        {
            //先讀取資料庫原有資料
            loadImageVList();
            //取得選取項目的值
            string selVImageStr = RadioButtonListV.SelectedValue;

            //刪除圖片檔案
            string savePath = Server.MapPath("~/Tayanahtml/images/");
            File.Delete(savePath + selVImageStr);

            //逐一比對原始資料 List<saveNameListV> 中的檔案名稱
            for (int i = 0; i < saveNameListV.Count; i++) {
                //與刪除的選項相同名稱
                if (saveNameListV[i].SaveName.Equals(selVImageStr)) {
                    //移除 List 中同名的資料
                    saveNameListV.RemoveAt(i);
                }
            }

            //更新刪除後的圖片名稱 JSON 存入資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string saveNameJsonStr = JsonConvert.SerializeObject(saveNameListV);
            string sql = "UPDATE Company SET certificatVerticalImgJSON = @saveNameJsonStr WHERE id = 1";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@saveNameJsonStr", saveNameJsonStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染畫面
            RadioButtonListV.Items.Clear();
            loadImageVList();
        }
        #endregion

        #region Horizontal Image List

        // JSON 資料 Horizontal Image
        public class ImageNameH
        {
            public string SaveName { get; set; }
        }

        private void loadImageHList()
        {
            //連線資料庫取出資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlLoad = "SELECT certificatHorizontalImgJSON FROM Company WHERE id = 1";
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = reader["certificatHorizontalImgJSON"].ToString();
                //反序列化JSON格式
                saveNameListH = JsonConvert.DeserializeObject<List<ImageNameH>>(loadJson);
            }
            connection.Close();
            //可以改成用 ?.Count 來判斷不是 Null 後才執行 .Count 避免錯誤
            if (saveNameListH?.Count > 0) {
                //逐一取出 JSON 的每筆資料
                foreach (var item in saveNameListH) {
                    //將 RadioButtonList 選項內容改為圖片格式，值設為檔案名稱
                    ListItem listItem = new ListItem($"<img src='/Tayanahtml/images/{item.SaveName}' alt='thumbnail' class='img-thumbnail' width='230px'/>", item.SaveName);
                    //加入圖片選項
                    RadioButtonListH.Items.Add(listItem);
                }
            }
            DelHImageBtn.Visible = false; //刪除鈕有選擇圖片時才顯示
        }

        protected void UploadHBtn_Click(object sender, EventArgs e)
        {
            //有選擇檔案才執行
            if (imageUploadH.HasFile) {
                //先讀取資料庫原有資料
                loadImageHList();
                string savePath = Server.MapPath("~/Tayanahtml/images/");

                //添加圖檔資料
                //逐一讀取選擇的圖片檔案
                foreach (HttpPostedFile postedFile in imageUploadH.PostedFiles) {
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
                    //在圖片名稱前加入 temp 標示並儲存圖片檔案
                    postedFile.SaveAs(savePath + "temp" + fileName);
                    //新增 JSON 資料
                    saveNameListH.Add(new ImageNameH { SaveName = fileName });

                    //使用 NetVips 套件進行壓縮圖檔
                    //判斷儲存的原始圖片寬度是否大於設定寬度的 2 倍
                    var img = NetVips.Image.NewFromFile(savePath + "temp" + fileName);
                    if (img.Width > 214 * 2) {
                        //產生原使圖片一半大小的新圖片
                        var newImg = img.Resize(0.5);
                        //如果新圖片寬度還是大於原始圖片設定寬度的 2 倍就持續縮減
                        while (newImg.Width > 214 * 2) {
                            newImg = newImg.Resize(0.5);
                        }
                        //儲存正式名稱的新圖片
                        newImg.WriteToFile(savePath + fileName);
                    }
                    else {
                        postedFile.SaveAs(savePath + fileName);
                    }
                    //刪除原始圖片
                    File.Delete(savePath + "temp" + fileName);
                }

                //更新新增後的圖片名稱 JSON 存入資料庫
                string fileNameJsonStr = JsonConvert.SerializeObject(saveNameListH);
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                string sql = "UPDATE Company SET certificatHorizontalImgJSON = @fileNameJsonStr WHERE id = 1";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@fileNameJsonStr", fileNameJsonStr);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                //渲染畫面
                RadioButtonListH.Items.Clear();
                loadImageHList();
            }
        }

        protected void RadioButtonListH_SelectedIndexChanged(object sender, EventArgs e)
        {
            //顯示刪除按鈕
            DelHImageBtn.Visible = true;
        }

        protected void DelHImageBtn_Click(object sender, EventArgs e)
        {
            //先讀取資料庫原有資料
            loadImageHList();
            //取得選取項目的值
            string selHImageStr = RadioButtonListH.SelectedValue;

            //刪除圖片檔案
            string savePath = Server.MapPath("~/Tayanahtml/images/");
            File.Delete(savePath + selHImageStr);

            //逐一比對原始資料 List<saveNameListH> 中的檔案名稱
            for (int i = 0; i < saveNameListH.Count; i++) {
                //與刪除的選項相同名稱
                if (saveNameListH[i].SaveName.Equals(selHImageStr)) {
                    //移除 List 中同名的資料
                    saveNameListH.RemoveAt(i);
                }
            }

            //更新刪除後的圖片名稱 JSON 存入資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string saveNameJsonStr = JsonConvert.SerializeObject(saveNameListH);
            string sql = "UPDATE Company SET certificatHorizontalImgJSON = @saveNameJsonStr WHERE id = 1";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@saveNameJsonStr", saveNameJsonStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染畫面
            RadioButtonListH.Items.Clear();
            loadImageHList();
        }
        #endregion

    }
}