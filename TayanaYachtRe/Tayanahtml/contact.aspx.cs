using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class contact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                //1.連線資料庫
                SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
                //2.sql語法
                string sqlCountry = "SELECT * FROM CountrySort";
                string sqlType = "SELECT * FROM Yachts";
                //3.創建command物件
                SqlCommand commandCountry = new SqlCommand(sqlCountry, connection);
                SqlCommand commandType = new SqlCommand(sqlType, connection);

                //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
                //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
                //取得國家分類
                connection.Open();
                SqlDataReader readerCountry = commandCountry.ExecuteReader();
                while (readerCountry.Read()) {
                    string typeStr = readerCountry["countrySort"].ToString();
                    ListItem listItem = new ListItem();
                    listItem.Text = typeStr;
                    listItem.Value = typeStr;
                    Country.Items.Add(listItem);
                }
                connection.Close();

                //取得遊艇型號分類
                connection.Open();
                SqlDataReader readerType = commandType.ExecuteReader();
                while (readerType.Read()) {
                    string typeStr = readerType["yachtModel"].ToString();
                    string isNewDesign = readerType["isNewDesign"].ToString();
                    string isNewBuilding = readerType["isNewBuilding"].ToString();
                    ListItem listItem = new ListItem();
                    if (isNewDesign.Equals("True")) {
                        listItem.Text = $"{typeStr} (New Design)";
                        listItem.Value = $"{typeStr} (New Design)";
                        Yachts.Items.Add(listItem);
                    }
                    else if (isNewBuilding.Equals("True")) {
                        listItem.Text = $"{typeStr} (New Building)";
                        listItem.Value = $"{typeStr} (New Building)";
                        Yachts.Items.Add(listItem);
                    }
                    else {
                        listItem.Text = typeStr;
                        listItem.Value = typeStr;
                        Yachts.Items.Add(listItem);
                    }
                }
                connection.Close();
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(Recaptcha1.Response)) {
                lblMessage.Visible = true;
                lblMessage.Text = "Captcha cannot be empty.";
            }
            else {
                var result = Recaptcha1.Verify();
                if (result.Success) {
                    sendGmail();
                    Response.Write("<script>alert('Thank you for contacting us!');location.href='contact.aspx';</script>");
                }
                else {
                    lblMessage.Text = "Error(s): ";

                    foreach (var err in result.ErrorCodes) {
                        lblMessage.Text = lblMessage.Text + err;
                    }
                }
            }
        }

        public void sendGmail()
        {
            //宣告使用MailMessage
            MailMessage mail = new MailMessage();
            //MailAddress("發信email", "發信人", 編碼方式-預設UTF8)
            mail.From = new MailAddress("dullulysses@gmail.com", "TayanaYacht");
            //收信者email
            mail.To.Add(Email.Text.Trim());//抓取使用者填入的email
            //寄件副本email
            mail.CC.Add("dullulysses@gmail.com");
            //設定優先權
            mail.Priority = MailPriority.Normal;
            //標題
            mail.Subject = "TayanaYacht Auto Email";
            //內容編碼方式
            //mail.BodyEncoding = Encoding.UTF8;
            //夾帶附件
            //mail.Attachments.Add(new Attachment("附件位置");
            //郵件內容
            mail.Body = "<h1>Thank you for contacting us!</h1>" +
                $"<h3>Name : {Name.Text.Trim()}</h3>" +
                $"<h3>Email : {Email.Text.Trim()}</h3>" +
                $"<h3>Phone : {Phone.Text.Trim()}</h3>" +
                $"<h3>Country : {Country.SelectedValue}</h3>" +
                $"<h3>Type : {Yachts.SelectedValue}</h3>" +
                $"<h3>Comments : </h3>" +
                $"<p>{Comments.Text.Trim()}</p>";
            //是否為Html標籤訊息
            mail.IsBodyHtml = true;
            //設定gmail的smtp Server及設定Port
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
            //設定gmail的帳號及16碼應用程式密碼
            MySmtp.Credentials = new NetworkCredential("dullulysses@gmail.com", "mcymfzopqjeigsdo");
            //開啟ssl加密
            MySmtp.EnableSsl = true;
            //發送郵件
            MySmtp.Send(mail);
            //釋放資源
            mail.Dispose();
        }

    }
}