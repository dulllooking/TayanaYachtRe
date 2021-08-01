using Konscious.Security.Cryptography;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

namespace TayanaYachtRe.Sys
{
    public partial class User_Manager_Cpage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //權限關門判斷 (Cookie)
            if (!User.Identity.IsAuthenticated) {
                Response.Redirect("Manager_SignIn.aspx"); //導回登入頁
            }
            if (!IsPostBack) {
                GridView1.DataBind();
            }
        }

        protected void BtnAddAccount_Click(object sender, EventArgs e)
        {
            bool haveSameAccount = false;

            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sqlCheck = "SELECT * FROM managerData WHERE account = @account";
            string sqlAdd = "INSERT INTO managerData (account, password, salt) VALUES(@account, @password, @salt)";
            SqlCommand commandCheck = new SqlCommand(sqlCheck, connection);
            SqlCommand commandAdd = new SqlCommand(sqlAdd, connection);

            //檢查有無重複帳號
            commandCheck.Parameters.AddWithValue("@account", TBoxAddAccount.Text);
            connection.Open();
            SqlDataReader readerCountry = commandCheck.ExecuteReader();
            if (readerCountry.Read()) {
                haveSameAccount = true;
                LabelAdd.Visible = true; //帳號重複通知
            }
            connection.Close();

            //無重複帳號才執行加入
            if (!haveSameAccount) {
                //Hash 加鹽加密
                string password = TBoxAddPassword.Text;
                var salt = CreateSalt();
                string saltStr = Convert.ToBase64String(salt); //將 byte 改回字串存回資料表
                var hash = HashPassword(password, salt);
                string hashPassword = Convert.ToBase64String(hash);

                commandAdd.Parameters.AddWithValue("@account", TBoxAddAccount.Text);
                commandAdd.Parameters.AddWithValue("@password", hashPassword);
                commandAdd.Parameters.AddWithValue("@salt", saltStr);

                connection.Open();
                commandAdd.ExecuteNonQuery();
                connection.Close();
                //畫面渲染
                GridView1.DataBind();
                //清空輸入欄位
                TBoxAddAccount.Text = "";
                TBoxAddPassword.Text = "";
                LabelAdd.Visible = false;
            }
                
        }

        // Argon2 加密
        //產生 Salt 功能
        private byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider(); // 強亂數產生器
            rng.GetBytes(buffer);
            return buffer;
        }
        // Hash 處理加鹽的密碼功能
        private byte[] HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            //底下這些數字會影響運算時間，而且驗證時要用一樣的值
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8; // 4 核心就設成 8
            argon2.Iterations = 4; // 迭代運算次數
            argon2.MemorySize = 1024 * 1024; // 1 GB

            return argon2.GetBytes(16);
        }

        protected void OnDataBind(object sender, EventArgs e)
        {
            GridView1.Rows[0].Cells[6].Controls.Clear();
        }
    }
}