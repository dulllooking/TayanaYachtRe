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
        //宣告List方便用Add依序添加資料
        private List<ImagePathV> savePathListV = new List<ImagePathV>();
        private List<ImagePathH> savePathListH = new List<ImagePathH>();
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

        private void loadCertificatContent()
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "SELECT certificatContent FROM Company WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader(); //指標指在BOF(表格之上)
            if (reader.Read()) {
                certificatTbox.Text = reader["certificatContent"].ToString();
            }
            //資料庫關閉
            connection.Close();
        }

        private void loadCkeditorContent()
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "SELECT aboutUsHtml FROM Company WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader(); //指標指在BOF(表格之上)
            if (reader.Read()) {
                CKEditorControl1.Text = HttpUtility.HtmlDecode(reader["aboutUsHtml"].ToString());
            }
            //資料庫關閉
            connection.Close();
        }

        private void ckfinderSetPath()
        {
            FileBrowser fileBrowser = new FileBrowser();
            fileBrowser.BasePath = "/ckfinder";
            fileBrowser.SetupCKEditor(CKEditorControl1);
        }

        protected void UploadAboutUsBtn_Click(object sender, EventArgs e)
        {
            string aboutUsHtmlStr = HttpUtility.HtmlEncode(CKEditorControl1.Text);
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "UPDATE Company SET aboutUsHtml = @aboutUsHtml WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@aboutUsHtml", aboutUsHtmlStr);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //無回傳值
            //7.資料庫關閉
            connection.Close();

            DateTime nowtime = DateTime.Now;
            UploadAboutUsLab.Visible = true;
            UploadAboutUsLab.Text = "*Upload Success! - " + nowtime.ToString("G");
        }

        protected void uploadCertificatBtn_Click(object sender, EventArgs e)
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "UPDATE Company SET certificatContent = @certificatContent WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@certificatContent", certificatTbox.Text);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //無回傳值
            //7.資料庫關閉
            connection.Close();

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
                savePathListV = JsonConvert.DeserializeObject<List<ImagePathV>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (savePathListV.Count > 0) {
                foreach (var item in savePathListV) {
                    ListItem listItem = new ListItem($"<img src='../Tayanahtml/images/{item.SavePath}' alt='thumbnail' class='img-thumbnail' width='150'/>", item.SavePath);
                    RadioButtonListV.Items.Add(listItem);
                }
            }
        }

        protected void UploadVBtn_Click(object sender, EventArgs e)
        {
            if (imageUploadV.HasFile) {
                loadImageVList();
                DelVImageBtn.Visible = false;
                //添加圖檔資料
                foreach (HttpPostedFile postedFile in imageUploadV.PostedFiles) {
                    //需填完整路徑，結尾反斜線如果沒加要用Path.Combine()可自動添加
                    string savePathStr = Server.MapPath("~/Tayanahtml/images/");
                    string fileName = postedFile.FileName;
                    if (savePathListV.Count > 0) {
                        foreach (var item in savePathListV) {
                            if (item.SavePath.Equals(fileName)) {
                                string[] fileNameArr = fileName.Split('.');
                                fileName = fileNameArr[0] + "(1)." + fileNameArr[1];
                            }
                        }
                        postedFile.SaveAs(savePathStr + "temp" + fileName);
                        //新增每筆JSON資料
                        savePathListV.Add(new ImagePathV { SavePath = fileName });
                    }
                    else {
                        postedFile.SaveAs(savePathStr + "temp" + fileName);
                        //新增每筆JSON資料
                        savePathListV.Add(new ImagePathV { SavePath = fileName });
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
                string savePathJsonStr = JsonConvert.SerializeObject(savePathListV);
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
                RadioButtonListV.Items.Clear();
                loadImageVList();
            }
        }

        //JSON資料V
        public class ImagePathV
        {
            public string SavePath { get; set; }
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
            for (int i = 0; i < savePathListV.Count; i++) {
                if (savePathListV[i].SavePath.Equals(selVImageStr)) {
                    savePathListV.RemoveAt(i);
                }
            }
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //將List資料轉為Json格式字串
            string savePathJsonStr = JsonConvert.SerializeObject(savePathListV);
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
        private void loadImageHList()
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sqlLoad = "SELECT certificatHorizontalImgJSON FROM Company WHERE id = 1";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sqlLoad, connection);
            //取得資料
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["certificatHorizontalImgJSON"].ToString());
                //反序列化JSON格式
                savePathListH = JsonConvert.DeserializeObject<List<ImagePathH>>(loadJson);
            }
            //資料庫關閉
            connection.Close();
            //?.可用來判斷不是Null才執行Count
            if (savePathListH.Count > 0) {
                foreach (var item in savePathListH) {
                    ListItem listItem = new ListItem($"<img src='../Tayanahtml/images/{item.SavePath}' alt='thumbnail' class='img-thumbnail' width='230'/>", item.SavePath);
                    RadioButtonListH.Items.Add(listItem);
                }
            }
        }

        protected void UploadHBtn_Click(object sender, EventArgs e)
        {
            if (imageUploadH.HasFile) {
                loadImageHList();
                DelHImageBtn.Visible = false;
                //添加圖檔資料
                foreach (HttpPostedFile postedFile in imageUploadH.PostedFiles) {
                    //需填完整路徑，結尾反斜線如果沒加要用Path.Combine()可自動添加
                    string savePath = Server.MapPath("~/Tayanahtml/images/");
                    string fileName = postedFile.FileName;
                    if (savePathListH.Count > 0) {
                        foreach (var item in savePathListH) {
                            if ((item.SavePath.ToString()).Equals(fileName)) {
                                string[] fileNameArr = fileName.Split('.');
                                fileName = fileNameArr[0] + "(1)." + fileNameArr[1];
                            }
                        }
                        postedFile.SaveAs(savePath + "temp" + fileName);
                        //新增每筆JSON資料
                        savePathListH.Add(new ImagePathH { SavePath = fileName });
                    }
                    else {
                        postedFile.SaveAs(savePath + "temp" + fileName);
                        //新增每筆JSON資料
                        savePathListH.Add(new ImagePathH { SavePath = fileName });
                    }
                    //壓縮圖檔
                    var image = NetVips.Image.NewFromFile(savePath + "temp" + fileName);
                    if (image.Width > 214 * 2) {
                        var newImg = image.Resize(0.5);
                        while (newImg.Width > 214 * 2) {
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
                string savePathJsonStr = JsonConvert.SerializeObject(savePathListH);
                //1.連線資料庫
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                //2.sql語法
                string sql = "UPDATE Company SET certificatHorizontalImgJSON = @path WHERE id = 1";
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
                RadioButtonListH.Items.Clear();
                loadImageHList();
            }
        }

        //JSON資料H
        public class ImagePathH
        {
            public string SavePath { get; set; }
        }

        protected void RadioButtonListH_SelectedIndexChanged(object sender, EventArgs e)
        {
            DelHImageBtn.Visible = true;
        }

        protected void DelHImageBtn_Click(object sender, EventArgs e)
        {
            loadImageHList();
            string selHImageStr = RadioButtonListH.SelectedValue;
            string savePath = Server.MapPath("~/Tayanahtml/images/");
            savePath += selHImageStr;
            File.Delete(savePath);
            for (int i = 0; i < savePathListH.Count; i++) {
                if (savePathListH[i].SavePath.Equals(selHImageStr)) {
                    savePathListH.RemoveAt(i);
                }
            }
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //將List資料轉為Json格式字串
            string savePathJsonStr = JsonConvert.SerializeObject(savePathListH);
            //2.sql語法
            string sql = "UPDATE Company SET certificatHorizontalImgJSON = @path WHERE id = 1";
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
            RadioButtonListH.Items.Clear();
            loadImageHList();
            DelHImageBtn.Visible = false;
        }
        #endregion

    }
}