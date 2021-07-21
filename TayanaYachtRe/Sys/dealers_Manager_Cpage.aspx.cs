using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace TayanaYachtRe.Sys
{
    public partial class dealers_Manager_Cpage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //權限關門判斷 (Cookie)
            if (!User.Identity.IsAuthenticated) {
                Response.Redirect("Manager_SignIn.aspx"); //導回登入頁
            }
            if (!IsPostBack) {
                DealerList.Visible = false;
                showDealerList();
            }
        }

        protected void BtnAddCountry_Click(object sender, EventArgs e)
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "INSERT INTO CountrySort (countrySort) VALUES(@name)";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@name", TBoxAddCountry.Text);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //無回傳值
            //7.資料庫關閉
            connection.Close();
            //畫面渲染
            GridView1.DataBind();
            DropDownList1.DataBind();
            //清空輸入欄位
            TBoxAddCountry.Text = "";
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioButtonList1.Items.Clear();
            BtnDelArea.Visible = false;
            DealerList.Visible = false;
            LabUploadImg.Visible = false;
            UpdateDealerListLab.Visible = false;
            showDealerList();
        }

        private void showDealerList()
        {
            string selCountryStr = DropDownList1.SelectedValue;
            if (selCountryStr.Equals("")) {
                selCountryStr = "1";
            }
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = $"SELECT * FROM Dealers WHERE country_ID = {selCountryStr}";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得地區分類
            connection.Open();
            SqlDataReader readerCountry = command.ExecuteReader(); //指標指在BOF(表格之上)
            while (readerCountry.Read()) {
                string typeStr = readerCountry["area"].ToString();
                ListItem listItem = new ListItem();
                listItem.Text = typeStr;
                listItem.Value = typeStr;
                RadioButtonList1.Items.Add(listItem);
            }
            connection.Close();
        }

        protected void BtnAddArea_Click(object sender, EventArgs e)
        {
            RadioButtonList1.Items.Clear();
            string selCountryStr = DropDownList1.SelectedValue;
            string areaStr = TBoxAddArea.Text;
            bool isRepeat = false;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = $"SELECT * FROM Dealers WHERE country_ID = {selCountryStr}";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得地區分類
            connection.Open();
            SqlDataReader readerCountry = command.ExecuteReader(); //指標指在BOF(表格之上)
            while (readerCountry.Read()) {
                string typeStr = readerCountry["area"].ToString();
                if (areaStr.Equals(typeStr)) {
                    isRepeat = true;
                    TBoxAddArea.ForeColor = Color.Red;
                    TBoxAddArea.Text = "The area name is repeated!";
                }
            }
            connection.Close();
            if (!isRepeat) {
                TBoxAddArea.ForeColor = Color.Black;
                //2.sql語法
                string sql2 = "INSERT INTO Dealers (country_ID, area) VALUES(@countryID, @area)";
                //3.創建command物件
                SqlCommand command2 = new SqlCommand(sql2, connection);
                //4.參數化
                command2.Parameters.AddWithValue("@countryID", selCountryStr);
                command2.Parameters.AddWithValue("@area", areaStr);
                //5.資料庫連線開啟
                connection.Open();
                //6.執行sql (新增刪除修改)
                command2.ExecuteNonQuery(); //無回傳值
                //7.資料庫關閉
                connection.Close();
                //畫面渲染
                RadioButtonList1.Items.Clear();
                BtnDelArea.Visible = false;
                DealerList.Visible = false;
                LabUploadImg.Visible = false;
                UpdateDealerListLab.Visible = false;
                showDealerList();
                //清空輸入欄位
                TBoxAddArea.Text = "";
            }
        }

        protected void BtnDelArea_Click(object sender, EventArgs e)
        {
            string selCountryStr = RadioButtonList1.SelectedValue;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "SELECT dealerImgPath FROM Dealers WHERE area = @name";
            string sqlDel = "DELETE FROM Dealers WHERE area = @name";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            SqlCommand commandDel = new SqlCommand(sqlDel, connection);
            //4.參數化
            command.Parameters.AddWithValue("@name", selCountryStr);
            commandDel.Parameters.AddWithValue("@name", selCountryStr);

            //刪除圖檔
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                string imgPath = reader["dealerImgPath"].ToString();
                if (!imgPath.Equals("")) {
                    string savePath = Server.MapPath("~/Tayanahtml/");
                    savePath += imgPath;
                    File.Delete(savePath);
                }
            }
            connection.Close();

            //刪除資料
            connection.Open();
            commandDel.ExecuteNonQuery();
            connection.Close();

            //畫面渲染
            RadioButtonList1.Items.Clear();
            BtnDelArea.Visible = false;
            DealerList.Visible = false;
            LabUploadImg.Visible = false;
            UpdateDealerListLab.Visible = false;
            showDealerList();
            TBoxAddArea.ForeColor = Color.Black;
            TBoxAddArea.Text = "";
        }

        protected void BtnUploadImg_Click(object sender, EventArgs e)
        {
            //需填完整路徑，結尾反斜線如果沒加要用Path.Combine()可自動添加
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");
            //有選檔案才可上傳
            if (FileUpload1.HasFile) {
                string fileName = FileUpload1.FileName;
                string selAreaStr = RadioButtonList1.SelectedValue;
                //1.連線資料庫
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                //2.sql語法
                string sqlDel = "SELECT dealerImgPath FROM Dealers WHERE area = @area";
                string sql = "UPDATE Dealers SET dealerImgPath = @path WHERE area = @area";
                //3.創建command物件
                SqlCommand commandDel = new SqlCommand(sqlDel, connection);
                SqlCommand command = new SqlCommand(sql, connection);
                //4.參數化
                command.Parameters.AddWithValue("@path", $"upload/Images/{fileName}");
                command.Parameters.AddWithValue("@area", selAreaStr);
                commandDel.Parameters.AddWithValue("@area", selAreaStr);

                //刪除舊圖檔
                connection.Open();
                SqlDataReader reader = commandDel.ExecuteReader();
                if (reader.Read()) {
                    string delFileName = reader["dealerImgPath"].ToString();
                    if (!delFileName.Equals("")) {
                        string delPath = Server.MapPath("~/Tayanahtml/");
                        delPath += delFileName;
                        File.Delete(delPath);
                    }
                }
                connection.Close();

                //存圖
                savePath += fileName;
                FileUpload1.SaveAs(savePath);
                LabUploadImg.Visible = true;
                LabUploadImg.ForeColor = Color.Green;
                LabUploadImg.Text = "*Upload Success!";

                //更新資料
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                //畫面渲染
                showDealerListTable();
            }
            else {
                LabUploadImg.Visible = true;
                LabUploadImg.ForeColor = Color.Red;
                LabUploadImg.Text = "*Need Choose File!";
            }
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnDelArea.Visible = true;
            LabUploadImg.Visible = false;
            UpdateDealerListLab.Visible = false;
            showDealerListTable();
        }

        private void showDealerListTable()
        {
            DealerList.Visible = true;
            TBoxCountry.Text = DropDownList1.SelectedItem.Text;
            TBoxArea.Text = RadioButtonList1.SelectedValue;
            string areaStr = RadioButtonList1.SelectedValue;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = $"SELECT * FROM Dealers WHERE area = '{areaStr}'";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得地區分類
            connection.Open();
            SqlDataReader reader = command.ExecuteReader(); //指標指在BOF(表格之上)
            while (reader.Read()) {
                string savePath = "~/Tayanahtml/" + reader["dealerImgPath"].ToString();
                Thumbnail.ImageUrl = savePath;
                string[] imgPathArr = reader["dealerImgPath"].ToString().Split('/');
                TBoxImage.Text = imgPathArr[imgPathArr.Length - 1];
                TBoxName.Text = reader["name"].ToString();
                TBoxContact.Text = reader["contact"].ToString();
                TBoxAddress.Text = reader["address"].ToString();
                TBoxTel.Text = reader["tel"].ToString();
                TBoxFax.Text = reader["fax"].ToString();
                TBoxEmail.Text = reader["email"].ToString();
                TBoxLink.Text = reader["link"].ToString();
                TBoxDate.Text = reader["initDate"].ToString();
            }
            connection.Close();
        }

        protected void BtnUpdateDealerList_Click(object sender, EventArgs e)
        {
            string selCountryStr = RadioButtonList1.SelectedValue;
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = $"UPDATE Dealers SET name=@name, contact=@ccontact, address=@address, tel=@tel, fax=@fax, email=@email, link=@link WHERE area = @area";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@name", TBoxName.Text);
            command.Parameters.AddWithValue("@ccontact", TBoxContact.Text);
            command.Parameters.AddWithValue("@address", TBoxAddress.Text);
            command.Parameters.AddWithValue("@tel", TBoxTel.Text);
            command.Parameters.AddWithValue("@fax", TBoxFax.Text);
            command.Parameters.AddWithValue("@email", TBoxEmail.Text);
            command.Parameters.AddWithValue("@link", TBoxLink.Text);
            command.Parameters.AddWithValue("@area", selCountryStr);
            //5.資料庫連線開啟
            connection.Open();
            //6.執行sql (新增刪除修改)
            command.ExecuteNonQuery(); //無回傳值
            //7.資料庫關閉
            connection.Close();
            //畫面渲染
            UpdateDealerListLab.Visible = true;
            showDealerListTable();
        }

        protected void DeltedCountry(object sender, EventArgs e)
        {
            DropDownList1.DataBind();
        }

    }
}