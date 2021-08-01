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
                DropDownList1.DataBind(); //取值
                showDealerList();
            }
        }

        protected void BtnAddCountry_Click(object sender, EventArgs e)
        {
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法
            string sql = "INSERT INTO CountrySort (countrySort) VALUES(@countryName)";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.參數化
            command.Parameters.AddWithValue("@countryName", TBoxAddCountry.Text);
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
            //當選擇國家改變時刷新畫面資料
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
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = $"SELECT * FROM Dealers WHERE country_ID = {selCountryStr}";
            SqlCommand command = new SqlCommand(sql, connection);
            //取得地區分類
            connection.Open();
            SqlDataReader readerCountry = command.ExecuteReader();
            while (readerCountry.Read()) {
                string typeStr = readerCountry["area"].ToString();
                // RadioButtonList 增加方式
                ListItem listItem = new ListItem();
                listItem.Text = typeStr;
                listItem.Value = typeStr;
                RadioButtonList1.Items.Add(listItem);
            }
            connection.Close();
        }

        protected void BtnAddArea_Click(object sender, EventArgs e)
        {
            //取得下拉選單國家的值 (id)
            string selCountryStr = DropDownList1.SelectedValue;
            //取得輸入欄內的文字
            string areaStr = TBoxAddArea.Text;
            //判斷是否重複用
            bool isRepeat = false;

            //取得地區分類
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = $"SELECT * FROM Dealers WHERE country_ID = {selCountryStr}";
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader readerCountry = command.ExecuteReader();
            while (readerCountry.Read()) {
                string typeStr = readerCountry["area"].ToString();
                //判斷有無重複名稱
                if (areaStr.Equals(typeStr)) {
                    isRepeat = true;
                    //重複警告
                    TBoxAddArea.ForeColor = Color.Red;
                    TBoxAddArea.Text = "The area name is repeated!";
                }
            }
            connection.Close();

            //輸入的區域名稱不重複才執行
            if (!isRepeat) {
                TBoxAddArea.ForeColor = Color.Black;
                //新增區域
                string sql2 = "INSERT INTO Dealers (country_ID, area) VALUES(@countryID, @area)";
                SqlCommand command2 = new SqlCommand(sql2, connection);
                command2.Parameters.AddWithValue("@countryID", selCountryStr);
                command2.Parameters.AddWithValue("@area", areaStr);
                connection.Open();
                command2.ExecuteNonQuery();
                connection.Close();

                //畫面渲染
                RadioButtonList1.Items.Clear(); //清掉舊的
                BtnDelArea.Visible = false;
                DealerList.Visible = false;
                LabUploadImg.Visible = false;
                UpdateDealerListLab.Visible = false;
                showDealerList(); //讀取新的

                //清空輸入欄位
                TBoxAddArea.Text = "";
            }
        }

        protected void BtnDelArea_Click(object sender, EventArgs e)
        {
            //取得選取資料的值
            string selCountryStr = RadioButtonList1.SelectedValue;
            
            //刪除實際圖檔檔案
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT dealerImgPath FROM Dealers WHERE area = @name";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", selCountryStr);
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

            //刪除資料庫該筆資料
            string sqlDel = "DELETE FROM Dealers WHERE area = @name";
            SqlCommand commandDel = new SqlCommand(sqlDel, connection);
            commandDel.Parameters.AddWithValue("@name", selCountryStr);
            connection.Open();
            commandDel.ExecuteNonQuery();
            connection.Close();

            //畫面渲染
            RadioButtonList1.Items.Clear(); //清掉舊的
            BtnDelArea.Visible = false;
            DealerList.Visible = false;
            LabUploadImg.Visible = false;
            UpdateDealerListLab.Visible = false;
            showDealerList(); //讀取新的
            TBoxAddArea.ForeColor = Color.Black;
            TBoxAddArea.Text = "";
        }

        protected void BtnUploadImg_Click(object sender, EventArgs e)
        {
            //設定存檔路徑，需填完整路徑，結尾反斜線如果沒加要用 Path.Combine() 可自動添加
            string savePath = Server.MapPath("~/Tayanahtml/upload/Images/");

            //判斷有選檔案才可上傳
            if (FileUpload1.HasFile) {
                //取得選擇區域名稱
                string selAreaStr = RadioButtonList1.SelectedValue;
                //取得選取檔案名稱
                string fileName = FileUpload1.FileName;

                //先執行刪除舊圖檔
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                string sqlDel = "SELECT dealerImgPath FROM Dealers WHERE area = @area";
                SqlCommand commandDel = new SqlCommand(sqlDel, connection);
                commandDel.Parameters.AddWithValue("@area", selAreaStr);
                connection.Open();
                SqlDataReader reader = commandDel.ExecuteReader();
                if (reader.Read()) {
                    string delFileName = reader["dealerImgPath"].ToString();
                    //有舊圖才執行刪除
                    if (!delFileName.Equals("")) {
                        string delPath = Server.MapPath("~/Tayanahtml/");
                        delPath += delFileName;
                        File.Delete(delPath);
                    }
                }
                connection.Close();

                //存圖檔
                savePath += fileName;
                FileUpload1.SaveAs(savePath);
                LabUploadImg.Visible = true;
                LabUploadImg.ForeColor = Color.Green;
                LabUploadImg.Text = "*Upload Success!";

                //更新資料庫資料
                string sql = "UPDATE Dealers SET dealerImgPath = @path WHERE area = @area";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@path", $"upload/Images/{fileName}");
                command.Parameters.AddWithValue("@area", selAreaStr);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                //畫面渲染
                showDealerListTable();
            }
            else {
                //警告沒有選取檔案
                LabUploadImg.Visible = true;
                LabUploadImg.ForeColor = Color.Red;
                LabUploadImg.Text = "*Need Choose File!";
            }
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //區域預設不選取，但當選取切換時顯示刪除按鈕
            BtnDelArea.Visible = true;
            LabUploadImg.Visible = false;
            UpdateDealerListLab.Visible = false;
            //顯示區域代理商資料表
            showDealerListTable();
            TBoxAddArea.ForeColor = Color.Black;
            TBoxAddArea.Text = "";
        }

        private void showDealerListTable()
        {
            //當區域選取時才顯示代理商資料表
            DealerList.Visible = true;
            //放入國家
            TBoxCountry.Text = DropDownList1.SelectedItem.Text;
            //放入區域
            TBoxArea.Text = RadioButtonList1.SelectedValue;

            //依選取區域的值連接資料庫取得資料
            string areaStr = RadioButtonList1.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = $"SELECT * FROM Dealers WHERE area = @area";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@area", areaStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            //放入個別資料
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
            //依國家及選取區域的值連接資料庫取得資料
            string selAreaStr = RadioButtonList1.SelectedValue;
            string selCountryStr = DropDownList1.SelectedValue;
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = $"UPDATE Dealers SET name=@name, contact=@ccontact, address=@address, tel=@tel, fax=@fax, email=@email, link=@link WHERE country_ID=@country_ID AND area = @area";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", TBoxName.Text);
            command.Parameters.AddWithValue("@ccontact", TBoxContact.Text);
            command.Parameters.AddWithValue("@address", TBoxAddress.Text);
            command.Parameters.AddWithValue("@tel", TBoxTel.Text);
            command.Parameters.AddWithValue("@fax", TBoxFax.Text);
            command.Parameters.AddWithValue("@email", TBoxEmail.Text);
            command.Parameters.AddWithValue("@link", TBoxLink.Text);
            command.Parameters.AddWithValue("@country_ID", selCountryStr);
            command.Parameters.AddWithValue("@area", selAreaStr);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            //上傳成功提示
            UpdateDealerListLab.Visible = true;
        }

        protected void DeltedCountry(object sender, EventArgs e)
        {
            DropDownList1.DataBind(); //刷新國家下拉列表資料
        }

    }
}