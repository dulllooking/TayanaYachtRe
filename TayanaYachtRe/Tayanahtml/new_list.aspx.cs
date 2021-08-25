using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class new_list : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                loadNewsList();
            }
        }

        private void loadNewsList()
        {
            //取得目前的時間，只顯示日期前的新聞
            DateTime nowTime = DateTime.Now;
            string nowDate = nowTime.ToString("yyyy-MM-dd");

            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);

            //2.建立判斷網址是否有傳值邏輯 (網址傳值功能已於製作控制項時已完成)
            int page = 1; //預設為第1頁
            //判斷網址後有無參數
            //也可用String.IsNullOrWhiteSpace
            if (!String.IsNullOrEmpty(Request.QueryString["page"])) {
                page = Convert.ToInt32(Request.QueryString["page"]);
            }

            //3.設定頁面參數屬性
            //設定控制項參數: 一頁幾筆資料
            WebUserControl_Page.limit = 5;
            //設定控制項參數: 作用頁面完整網頁名稱
            WebUserControl_Page.targetPage = "new_list.aspx";

            //4.建立計算分頁資料顯示邏輯 (每一頁是從第幾筆開始到第幾筆結束)
            //計算每個分頁的第幾筆到第幾筆
            var floor = (page - 1) * WebUserControl_Page.limit + 1; //每頁的第一筆
            var ceiling = page * WebUserControl_Page.limit; //每頁的最末筆

            //5.建立計算資料筆數的 SQL 語法
            //算出我們要秀的資料數
            string sql_countTotal = "SELECT COUNT(id) FROM News WHERE dateTitle <= @nowDate";
            SqlCommand commandForTotal = new SqlCommand(sql_countTotal, connection);
            commandForTotal.Parameters.AddWithValue("@nowDate", nowDate); //只秀當天及之前的資料

            //6.將取得的資料數設定給參數 count
            connection.Open();
            //用 ExecuteScalar() 來算數量
            int count = Convert.ToInt32(commandForTotal.ExecuteScalar());
            connection.Close();

            //7.將取得的資料筆數設定給頁面參數屬性
            //設定控制項參數: 總共幾筆資料
            WebUserControl_Page.totalItems = count;

            //8.使用 showPageControls() 渲染至網頁 (方法於製作控制項時已完成)
            //渲染分頁控制項
            WebUserControl_Page.showPageControls();

            //9.將原始資料表的 SQL 語法使用 CTE 暫存表改寫，並使用 ROW_NUMBER() 函式製作資料項流水號 rowindex
            // SQL 用 CTE 暫存表 + ROW_NUMBER 去生出我的流水號 rowindex 後以流水號為條件來查詢暫存表
            // 排序先用 isTop 後用 dateTitle 產生 TOP News 置頂效果
            string sql = $"WITH temp AS (SELECT ROW_NUMBER() OVER (ORDER BY isTop DESC, dateTitle DESC) AS rowindex, * FROM News WHERE dateTitle <= @nowDate) SELECT * FROM temp WHERE rowindex >= {floor} AND rowindex <= {ceiling}";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nowDate", nowDate);

            //10.取得每頁的新聞列表資料製作成 HTML 內容
            connection.Open();
            StringBuilder newListHtml = new StringBuilder();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                string idStr = reader["id"].ToString();
                DateTime dateTimeTitle = DateTime.Parse(reader["dateTitle"].ToString());
                string dateTitleStr = dateTimeTitle.ToString("yyyy/M/d");
                string headlineStr = reader["headline"].ToString();
                string summaryStr = reader["summary"].ToString();
                string thumbnailPathStr = reader["thumbnailPath"].ToString();
                string guidStr = reader["guid"].ToString();
                string isTopStr = reader["isTop"].ToString();
                string displayStr = "none";
                if (isTopStr.Equals("True")) {
                    displayStr = "inline-block";
                }
                newListHtml.Append($"<li><div class='list01'><ul><li><div class='news01'>" +
                    $"<img src='images/new_top01.png' alt='&quot;&quot;' style='display: {displayStr};position: absolute;z-index: 5;'/>" +
                    $"<div class='news02p1' style='margin: 0px;border-width: 0px;padding: 0px;' ><p>" +
                    $"<img id='thumbnail_Image{idStr}' src='upload/Images/{thumbnailPathStr}' style='border-width: 0px;position: absolute;z-index: 1;' width='161px' height='121px' />" +
                    $"</p></div></li><li><span>{dateTitleStr}</span><br />" +
                    $"<a href='new_view.aspx?id={guidStr}'>{headlineStr} </a></li><br />" +
                    $"<li>{summaryStr} </li></ul></div></li>");
            }
            connection.Close();

            //渲染新聞列表
            newList.Text = newListHtml.ToString();
        }
    }
}