using System;
using System.Text;

namespace TayanaYachtRe
{
    public partial class WebUserControl_Page : System.Web.UI.UserControl
    {
        //設定自製控制項屬性接收值的類型
        public int totalItems { get; set; } //總共幾筆資料
        public int limit { get; set; } //一頁幾筆資料
        public string targetPage { get; set; } //作用頁面完整網頁名稱

        public void showPageControls()
        {
            litPage.Text = ""; //清空分頁控制項
            int page = 1;
            //如果網址有傳值
            if (!string.IsNullOrEmpty(Request["page"])) {
                //傳值為數字
                if (IsNumber(Request["page"])) {
                    page = Convert.ToInt16(Request["page"]); //修改當前頁碼
                }
            }
            if (totalItems == 0) {
                return;
            }
            if (limit == 0) {
                return;
            }
            //取得當前頁面檔案名稱
            targetPage = targetPage ?? System.IO.Path.GetFileName(Request.PhysicalPath);
            //渲染分頁控制項 //adjacents 參數不建議設太大，可能導致換行
            litPage.Text = getPaginationString(page, totalItems, limit, 2, targetPage);
        }


        #region "用正規式判斷是否為數字"
        /// <summary>
        /// 判斷是否為數字
        /// </summary>
        /// <param name="inputData">輸入字串</param>
        /// <returns>bool</returns>
        bool IsNumber(string inputData)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(inputData, "^[0-9]+$");
        }
        #endregion


        #region "產生分頁控制項"
        /// <summary>
        /// 產生分頁控制項
        /// </summary>
        /// <param name="page">目前第幾頁</param>
        /// <param name="totalItems">共有幾筆</param>
        /// <param name="limit">一頁幾筆資料</param>
        /// <param name="adjacents">隱藏部分頁碼，只顯示當前頁左右各幾頁</param>
        /// <param name="targetPage">當前頁面檔案名稱，例:index.aspx</param>
        /// <returns>回傳HTML標籤字串</returns>
        public static string getPaginationString(int page, int totalItems, int limit, int adjacents, string targetPage)
        {
            //有無帶有傳值用?如果有就在後面加&無則補加? //預設幫檔名加問號
            targetPage = targetPage.IndexOf('?') != -1 ? targetPage + "&" : targetPage + "?";
            //前一頁 = 目前頁面 - 1
            int prev = page - 1;
            //下一頁 = 目前頁面 + 1
            int nextPage = page + 1;
            //總頁數數值 = 總資料筆數/每頁幾筆
            Double value = Convert.ToDouble((decimal)totalItems / limit);
            //最末頁(總頁數) = 總頁數數值無條件進位成整數
            int lastpage = Convert.ToInt16(Math.Ceiling(value));
            //倒數第二頁 = 最末頁 - 1
            int lpm1 = lastpage - 1;
            //建立分頁HTML字串邏輯
            StringBuilder paginationBuilder = new StringBuilder();
            if (lastpage > 1) {
                //共計幾筆資料HTML
                paginationBuilder.Append("<div class=\"pagination\">Total <span style=\"color:red\" >" + totalItems + "</span> data.");
                //上一頁HTML，目前頁面大於1則啟用連結，否則就禁用
                paginationBuilder.Append(page > 1 ? string.Format("<a href=\"{0}page={1}\"> <<< </a>", targetPage, prev) : "<span class=\"disabled\"> <<< </span>");
                //頁碼選項HTML邏輯判斷
                //總頁數小於(7頁+顯示鄰近頁數*2)，就不執行隱藏部分頁碼
                if (lastpage < 7+(adjacents*2)) {
                    for (int counter = 1; counter <= lastpage; counter++) {
                        //counter等於當前頁則不加入連結，否則就加入連結
                        paginationBuilder.Append(counter == page ? string.Format("<span class=\"current\">{0}</span>", counter) : string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, counter));
                    }
                }
                //執行隱藏部分頁碼
                else {
                    //當前頁小於(1頁+顯示鄰近頁數*3)，只隱藏單側頁碼，大數字區頁碼-右側
                    if (page < 1+(adjacents*3)) {
                        for (int counter = 1; counter <= (3+(adjacents*2)); counter++) {
                            //小於等於(3頁+顯示鄰近頁數*2)的頁碼正常添加HTML頁碼 (當前頁不加連結)
                            paginationBuilder.Append(counter == page ? string.Format("<span class=\"current\">{0}</span>", counter) : string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, counter));
                        }
                        //之後的頁碼用...省略
                        paginationBuilder.Append("...");
                        //加入倒數第2頁
                        paginationBuilder.Append(string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, lpm1));
                        //加入最末頁
                        paginationBuilder.Append(string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, lastpage));
                    }
                    //當前頁大於(顯示鄰近頁數*2) && 當前頁小於(最末頁-顯示鄰近頁數*2)，就隱藏兩側頁碼
                    else if (page > (adjacents*2) && page < (lastpage-(adjacents*2))) {
                        //加入第一頁+第二頁及...省略頁碼
                        paginationBuilder.Append(string.Format("<a href=\"{0}page=1\">1</a>", targetPage));
                        paginationBuilder.Append(string.Format("<a href=\"{0}page=2\">2</a>", targetPage));
                        paginationBuilder.Append("...");
                        for (int counter = (page-adjacents); counter <= (page+adjacents); counter++) {
                            //從當前頁的左側鄰近頁到右側鄰近頁正常添加頁碼 (當前頁不加連結)
                            paginationBuilder.Append(counter == page ? string.Format("<span class=\"current\">{0}</span>", counter) : string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, counter));
                        }
                        //之後的頁碼用...省略，加入倒數第二頁及最末頁
                        paginationBuilder.Append("...");
                        paginationBuilder.Append(string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, lpm1));
                        paginationBuilder.Append(string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, lastpage));
                    }
                    //當前頁大於等於(最末頁-顯示鄰近頁數*2)，只隱藏單側頁碼，數字區頁碼-左側
                    else {
                        //加入第一頁+第二頁及...省略頁碼
                        paginationBuilder.Append(string.Format("<a href=\"{0}page=1\">1</a>", targetPage));
                        paginationBuilder.Append(string.Format("<a href=\"{0}page=2\">2</a>", targetPage));
                        paginationBuilder.Append("...");
                        for (int counter = (lastpage-(3+(adjacents*2))); counter <= lastpage; counter++) {
                            //小於等於(最末頁-((3頁+顯示鄰近頁數*2))的頁碼正常添加HTML頁碼 (當前頁不加連結)
                            paginationBuilder.Append(counter == page ? string.Format("<span class=\"current\">{0}</span>", counter) : string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, counter));
                        }
                    }
                }
                //下一頁HTML，目前頁面小於最末頁則啟用連結，否則就禁用
                paginationBuilder.Append(page < lastpage ? string.Format("<a href=\"{0}page={1}\">>>></a>", targetPage, nextPage) : "<span class=\"disabled\"> >>> </span>");
                paginationBuilder.Append("</div>\r\n");
            }
            return paginationBuilder.ToString();
        }
        #endregion
    }
}