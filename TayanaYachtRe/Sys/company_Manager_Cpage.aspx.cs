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
            //權限關門判斷 (Cookie)
            if (!User.Identity.IsAuthenticated) {
                Response.Redirect("Manager_SignIn.aspx"); //導回登入頁
            }
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
            string sql = "UPDATE Company SET aboutUsHtml = @aboutUsHtml WHERE id = 1";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@aboutUsHtml", aboutUsHtmlStr);
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
        private void loadImageVList()
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlLoad = "SELECT certificatVerticalImgJSON FROM Company WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["certificatVerticalImgJSON"].ToString());
                //反序列化JSON格式
                saveNameListV = JsonConvert.DeserializeObject<List<ImageNameV>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (saveNameListV.Count > 0) {
                foreach (var item in saveNameListV) {
                    ListItem listItem = new ListItem($"<img src='../Tayanahtml/images/{item.SaveName}' alt='thumbnail' class='img-thumbnail' width='150'/>", item.SaveName);
                    RadioButtonListV.Items.Add(listItem);
                }
            }
        }

        protected void UploadVBtn_Click(object sender, EventArgs e)
        {
            if (imageUploadV.HasFile) {
                loadImageVList();
                
                //添加圖檔資料
                foreach (HttpPostedFile postedFile in imageUploadV.PostedFiles) {
                    //檢查專案資料夾內有無同名檔案
                    DirectoryInfo directoryInfo = new DirectoryInfo(Server.MapPath("~/Tayanahtml/images/"));
                    string savePathStr = Server.MapPath("~/Tayanahtml/images/");
                    string fileName = postedFile.FileName;
                    if (saveNameListV.Count > 0) {
                        foreach (var fileItem in directoryInfo.GetFiles()) {
                            if (fileItem.Name.Equals(fileName)) {
                                string[] fileNameArr = fileName.Split('.');
                                fileName = fileNameArr[0] + "(1)." + fileNameArr[1];
                            }
                        }
                        postedFile.SaveAs(savePathStr + "temp" + fileName);
                        //新增每筆JSON資料
                        saveNameListV.Add(new ImageNameV { SaveName = fileName });
                    }
                    else {
                        postedFile.SaveAs(savePathStr + "temp" + fileName);
                        //新增每筆JSON資料
                        saveNameListV.Add(new ImageNameV { SaveName = fileName });
                    }
                    //壓縮圖檔
                    var image = NetVips.Image.NewFromFile(savePathStr + "temp" + fileName);
                    if (image.Width > 214*2) {
                        var newImg = image.Resize(0.5);
                        while (newImg.Width > 214*2) {
                            newImg = newImg.Resize(0.5);
                        }
                        newImg.WriteToFile(savePathStr + fileName);
                    }
                    else {
                        postedFile.SaveAs(savePathStr + fileName);
                    }
                    File.Delete(savePathStr + "temp" + fileName);
                }
                //將List資料轉為Json格式字串
                string savePathJsonStr = JsonConvert.SerializeObject(saveNameListV);
                //1.連線資料庫
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                //2.sql語法
                string sql = "UPDATE Company SET certificatVerticalImgJSON = @path WHERE id = 1";
                //3.創建command物件
                SqlCommand command = new SqlCommand(sql, connection);
                //4.參數化
                command.Parameters.AddWithValue("@path", savePathJsonStr);
                //5.資料庫連線開啟
                connection.Open();
                //6.執行sql (新增刪除修改)
                command.ExecuteNonQuery();
                //7.資料庫關閉
                connection.Close();

                DelVImageBtn.Visible = false;
                RadioButtonListV.Items.Clear();
                loadImageVList();
            }
        }

        //JSON資料V
        public class ImageNameV
        {
            public string SaveName { get; set; }
        }

        protected void RadioButtonListV_SelectedIndexChanged(object sender, EventArgs e)
        {
            DelVImageBtn.Visible = true;
        }

        protected void DelVImageBtn_Click(object sender, EventArgs e)
        {
            loadImageVList();
            string selVImageStr = RadioButtonListV.SelectedValue;
            string savePath = Server.MapPath("~/Tayanahtml/images/");
            savePath += selVImageStr;
            File.Delete(savePath);
            for (int i = 0; i < saveNameListV.Count; i++) {
                if (saveNameListV[i].SaveName.Equals(selVImageStr)) {
                    saveNameListV.RemoveAt(i);
                }
            }
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //將List資料轉為Json格式字串
            string savePathJsonStr = JsonConvert.SerializeObject(saveNameListV);
            //2.sql語法
            string sql = "UPDATE Company SET certificatVerticalImgJSON = @path WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@path", savePathJsonStr);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery();
            //7.資料庫關閉
            connection.Close();
            //渲染畫面
            RadioButtonListV.Items.Clear();
            loadImageVList();
            DelVImageBtn.Visible = false;
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
                    ListItem listItem = new ListItem($"<img src='../Tayanahtml/images/{item.SaveName}' alt='thumbnail' class='img-thumbnail' width='230'/>", item.SaveName);
                    //加入圖片選項
                    RadioButtonListH.Items.Add(listItem);
                }
            }
        }

        protected void UploadHBtn_Click(object sender, EventArgs e)
        {
            //有選擇檔案才執行
            if (imageUploadH.HasFile) {
                //先讀取資料庫原有資料
                loadImageHList();

                //添加圖檔資料
                //逐一讀取選擇的圖片檔案
                foreach (HttpPostedFile postedFile in imageUploadH.PostedFiles) {

                    //儲存圖片檔案及圖片名稱
                    //檢查專案資料夾內有無同名檔案
                    DirectoryInfo directoryInfo = new DirectoryInfo(Server.MapPath("~/Tayanahtml/images/"));
                    string savePath = Server.MapPath("~/Tayanahtml/images/");
                    //取得選取的檔案名稱
                    string fileName = postedFile.FileName;
                    //與原有資料比對，如果有相同名稱則加入流水號
                    if (saveNameListH.Count > 0) {
                        foreach (var fileItem in directoryInfo.GetFiles()) {
                            if (fileItem.Name.Equals(fileName)) {
                                string[] fileNameArr = fileName.Split('.');
                                fileName = fileNameArr[0] + "(1)." + fileNameArr[1];
                            }
                        }
                        //在圖片名稱前加入 temp 標示並儲存圖片檔案
                        postedFile.SaveAs(savePath + "temp" + fileName);
                        //新增 JSON 資料
                        saveNameListH.Add(new ImageNameH { SaveName = fileName });
                    }
                    else {
                        postedFile.SaveAs(savePath + "temp" + fileName);
                        saveNameListH.Add(new ImageNameH { SaveName = fileName });
                    }

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
                string sql = "UPDATE Company SET certificatHorizontalImgJSON = @imgName WHERE id = 1";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@imgName", fileNameJsonStr);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                //渲染畫面
                DelHImageBtn.Visible = false; //刪除鈕有選擇圖片時才顯示
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
            savePath += selHImageStr;
            File.Delete(savePath);

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
            string sql = "UPDATE Company SET certificatHorizontalImgJSON = @imgName WHERE id = 1";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@imgName", saveNameJsonStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //渲染畫面
            DelHImageBtn.Visible = false;
            RadioButtonListH.Items.Clear();
            loadImageHList();
            
        }

        #endregion

    }
}