using Konscious.Security.Cryptography;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace TayanaYachtRe.Sys
{
    public partial class Manager_SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        //Argon2加密驗證

        private byte[] HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8; // four cores
            argon2.Iterations = 4; 
            argon2.MemorySize = 1024 * 1024; // 1 GB

            return argon2.GetBytes(16);
        }

        private bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session["account"] = TextBox1.Text;
            string password = TextBox2.Text;

            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.sql語法 (@參數化避免隱碼攻擊) reader判斷條件設在SQL在資料庫找不到直接報錯
            string sql = $"SELECT * FROM managerData WHERE account = @account";
            //3.創建command物件
            SqlCommand command = new SqlCommand(sql, connection);
            //4.放入參數化資料
            command.Parameters.AddWithValue("@account", TextBox1.Text);
            //5.資料庫用Adapter執行指令
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            //6.建立一個空的Table
            DataTable dataTable = new DataTable();
            //7.將資料放入Table
            dataAdapter.Fill(dataTable);
            //登入流程管理 (Session 改 Cookie)
            if (dataTable.Rows.Count > 0) {
                //SQL有找到資料時執行

                //將字串轉回byte
                byte[] hash = Convert.FromBase64String(dataTable.Rows[0]["password"].ToString());
                byte[] salt = Convert.FromBase64String(dataTable.Rows[0]["salt"].ToString());
                //驗證密碼
                var success = VerifyHash(password, salt, hash);

                if (success) {
                    //宣告驗證票要夾帶的資料
                    string userData = dataTable.Rows[0]["maxPower"].ToString() + ";" + dataTable.Rows[0]["account"].ToString() + ";" + dataTable.Rows[0]["name"].ToString() + ";" + dataTable.Rows[0]["email"].ToString();
                    //設定驗證票(夾帶資料，cookie命名) //需額外引入using System.Web.Configuration;
                    SetAuthenTicket(userData, TextBox1.Text);
                    //導頁至權限分流頁
                    Response.Redirect("CheckAccount.ashx");
                }
                else {
                    //資料庫裡找不到相同資料時，表示密碼有誤!
                    Label4.Text = "password error, login failed!";
                    Label4.Visible = true;
                    connection.Close();
                    //終止程式
                    //Response.End(); //會清空頁面
                    return;
                }
            }
            else {
                //資料庫裡找不到相同資料時，表示帳號有誤!
                Label4.Text = "Account error, login failed!";
                Label4.Visible = true;
                connection.Close();
                //終止程式
                //Response.End(); //會清空頁面
                return;
            }
        }

        //驗證函式
        private void SetAuthenTicket(string userData, string userId)
        {
            //宣告一個驗證票 //需額外引入using System.Web.Security;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userId, DateTime.Now, DateTime.Now.AddHours(3), false, userData);
            //加密驗證票
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            //建立Cookie
            HttpCookie authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            //將Cookie寫入回應
            Response.Cookies.Add(authenticationCookie);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Tayanahtml/index.aspx");
        }
    }
}