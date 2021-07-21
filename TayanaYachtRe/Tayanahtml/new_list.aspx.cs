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
            DateTime nowTime = DateTime.Now;
            string nowDate = nowTime.ToString("yyyy-MM-dd");
            StringBuilder newListHtml = new StringBuilder();
            //1.連線資料庫
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //2.建立判斷網址是否有傳值邏輯 (網址傳值功能已於製作控制項時已完成)
            //分頁用
            int page = 0;
            //判斷網址後有無參數
            //也可用String.IsNullOrWhiteSpace
            if (String.IsNullOrEmpty(Request.QueryString["page"])) {
                page = 1; //page後無值就是第一頁
            }
            else {
                page = Convert.ToInt32(Request.QueryString["page"]);
            }
            //3.設定頁面參數屬性
            //跟我們的控制項說一頁有幾筆
            WebUserControl_Page.limit = 6;
            //跟我們控制項說我們是哪一頁要有分頁
            WebUserControl_Page.targetpage = "new_list.aspx";
            //4.建立計算分頁顯示資料項邏輯 (每一頁是從第幾筆開始到第幾筆結束)
            //計算每個分頁的第幾筆到第幾筆
            var floor = (page - 1) * WebUserControl_Page.limit + 1;
            var ceiling = page * WebUserControl_Page.limit;
            //5.建立計算資料筆數的SQL語法
            //算出我們要秀的資料筆數的總數
            string sql_countTotal = "SELECT COUNT(*) FROM News WHERE dateTitle <= @dateTitle";
            SqlCommand commandForTotal = new SqlCommand(sql_countTotal, connection);
            commandForTotal.Parameters.AddWithValue("@dateTitle", nowDate);
            //6.將取得的資料筆數設定給頁面參數屬性
            connection.Open();
            int count = Convert.ToInt32(commandForTotal.ExecuteScalar()); //count不用全算
            connection.Close();
            //7. 將取得的資料筆數設定給頁面參數屬性
            //算出總數以後，把總數給我們的控制項
            WebUserControl_Page.totalitems = count;
            //8. 使用showPageControls()渲染至網頁 (方法於製作控制項時已完成)
            //渲染控制項出來(分頁頁碼),最後的步驟
            WebUserControl_Page.showPageControls();
            //9. 將原始資料表的SQL語法使用CTE暫存表改寫，並使用ROW_NUMBER()函式製作資料項流水號
            //SQL用CTE暫存表 + ROW_NUMBER 去生出我的流水號 (修改原本的SQL)
            string sql = $"WITH temp AS (SELECT ROW_NUMBER() OVER (ORDER BY dateTitle DESC) AS rowindex, * FROM News WHERE dateTitle <= @dateTitle) SELECT * FROM temp WHERE rowindex >= {floor} AND rowindex <= {ceiling}";
            SqlCommand command = new SqlCommand(sql, connection);
            //4.放入參數化資料
            command.Parameters.AddWithValue("@dateTitle", nowDate);
            //讀出一筆資料寫入控制器 (.Read()一次會跑一筆)
            //.Read()=>指針往下一移並回傳bool，如果要讀全部可用while //最後一筆之後是EOF
            //取得News
            connection.Open();
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
                newListHtml.Append($"<li><div class='list01'><ul><li><div class='news01'><img src='images/new_top01.png' alt='&quot;&quot;' style='display: {displayStr};position: absolute;z-index: 5;'/><div class='news02p1' style='margin: 0px;border-width: 0px;padding: 0px;' ><p>" +
                    $"<img id='ctl00_ContentPlaceHolder1_Repeater1_ctl{idStr}_Image1' src='{thumbnailPathStr}' style='border-width: 0px;position: absolute;z-index: 1;' width='161px' height='121px' />" +
                    $"</p></div></li><li><span>{dateTitleStr}</span><br />" +
                    $"<a href='new_view.aspx?id={guidStr}'>{headlineStr} </a></li><br />" +
                    $"<li>{summaryStr} </li></ul></div></li>");
            }
            connection.Close();
            newList.Text = newListHtml.ToString();
        }
    }
}