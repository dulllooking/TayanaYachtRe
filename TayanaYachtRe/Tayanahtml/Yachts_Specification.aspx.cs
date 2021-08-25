using System;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class Yachts_Specification : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                loadContent();
            }
        }

        private void loadContent()
        {
            string guidStr = Session["guid"].ToString();
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT Yachts.guid, Specification.detail, DetailTitleSort.detailTitleSort"
                        + " FROM DetailTitleSort INNER JOIN"
                            + " Specification ON DetailTitleSort.id = Specification.detailTitleSort_ID INNER JOIN"
                            + " Yachts ON Specification.yachtModel_ID = Yachts.id WHERE Yachts.guid = @guidStr";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@guidStr", guidStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            StringBuilder detailHtmlStr = new StringBuilder();
            //用於檢查 Title 是否相同
            string checkTitle = "";
            while (reader.Read()) {
                string detailTitle = reader["detailTitleSort"].ToString();
                //需使用 HtmlDecode ，因為存入時有使用 HtmlEncode 轉換換行用標籤 <br>
                string detailStr = HttpUtility.HtmlDecode(reader["detail"].ToString());
                // 加入第一個標題，並更新檢查用變數
                if (String.IsNullOrEmpty(checkTitle)) {
                    detailHtmlStr.Append($"<p>{detailTitle}</p><ul>");
                    checkTitle = detailTitle;
                }
                // Title 不相同時就更新確認用變數並加入 Title 的 HTML 語法
                else if (!checkTitle.Equals(detailTitle)) {
                    checkTitle = detailTitle;
                    detailHtmlStr.Append($"</ul><p>{detailTitle}</p><ul>");
                }
                detailHtmlStr.Append($"<li>{detailStr}</li>");
            }
            connection.Close();
            //結束 HTML 字串並渲染畫面
            detailHtmlStr.Append($"</ul>");
            ContentHtml.Text = detailHtmlStr.ToString();
        }
    }
}