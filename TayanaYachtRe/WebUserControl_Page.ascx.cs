using System;
using System.Text;

namespace TayanaYachtRe
{
    public partial class WebUserControl_Page : System.Web.UI.UserControl
    {
        public int totalItems { get; set; } //總共幾筆資料
        public int limit { get; set; } //一頁幾筆
        public string targetPage { get; set; } //作用頁面網址

        public void showPageControls()
        {
            litPage.Text = "";//清空
            int page = 1;
            if (!string.IsNullOrEmpty(Request["page"])) {
                if (IsNumber(Request["page"])) {
                    page = Convert.ToInt16(Request["page"]);
                }
            }
            if (totalItems == 0) {
                return;
            }
            if (limit == 0) {
                return;
            }
            targetPage = targetPage ?? System.IO.Path.GetFileName(Request.PhysicalPath);
            litPage.Text = getPaginationString(page, totalItems, limit, 2, targetPage);
        }

        #region "判斷是否為數字"
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
        /// <param name="limit">一頁幾筆</param>
        /// <param name="adjacents">不知道，傳2~5都OK</param>
        /// <param name="targetPage">連結文字，例:pagination.aspx?foo=bar</param>
        /// <returns></returns>
        public static string getPaginationString(int page, int totalItems, int limit, int adjacents, string targetPage)
        {
            //defaults
            targetPage = targetPage.IndexOf('?') != -1 ? targetPage + "&" : targetPage + "?";
            string margin = "";
            string padding = "";
            //other vars
            int prev = page - 1;
            //previous page is page - 1
            int nextPage = page + 1;
            //nextPage page is page + 1
            Double value = Convert.ToDouble((decimal)totalItems / limit);
            int lastpage = Convert.ToInt16(Math.Ceiling(value));
            //lastpage is = total items / items per page, rounded up.
            int lpm1 = lastpage - 1;
            //last page minus 1
            int counter = 0;
            // Now we apply our rules and draw the pagination object.
            // We're actually saving the code to a variable in case we want to draw it more than once.
            StringBuilder paginationBuilder = new StringBuilder();
            if (lastpage > 1) {
                paginationBuilder.Append("<div class=\"pagination\"");
                if (!string.IsNullOrEmpty(margin) | !string.IsNullOrEmpty(padding)) {
                    paginationBuilder.Append(" style=\"");
                    if (!string.IsNullOrEmpty(margin)) {
                        paginationBuilder.Append("margin: margin");
                    }
                    if (!string.IsNullOrEmpty(padding)) {
                        paginationBuilder.Append("padding: padding");
                    }
                    paginationBuilder.Append("\"");
                }
                paginationBuilder.Append(">共<span style=\"color:red\" >" + totalItems + "</span>筆資料");
                //previous button
                paginationBuilder.Append(page > 1 ? string.Format("<a href=\"{0}page={1}\">上一頁</a>", targetPage, prev) : "<span class=\"disabled\">上一頁</span>");
                //pages
                if (lastpage < 7 + (adjacents * 2)) {
                    //not enough pages to bother breaking it up
                    for (counter = 1; counter <= lastpage; counter++) {
                        paginationBuilder.Append(counter == page ? string.Format("<span class=\"current\">{0}</span>", counter) : string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, counter));
                    }
                }
                else if (lastpage >= 7 + (adjacents * 2)) {
                    //enough pages to hide some
                    //close to beginning only hide later pages
                    if (page < 1 + (adjacents * 3)) {
                        for (counter = 1; counter <= (4 + (adjacents * 2)) - 1; counter++) {
                            paginationBuilder.Append(counter == page ? string.Format("<span class=\"current\">{0}</span>", counter) : string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, counter));
                        }
                        paginationBuilder.Append("...");
                        paginationBuilder.Append(string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, lpm1));
                        paginationBuilder.Append(string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, lastpage));
                    }
                    //in middle hide some front and some back
                    else if (lastpage - (adjacents * 2) > page & page > (adjacents * 2)) {
                        paginationBuilder.Append(string.Format("<a href=\"{0}page=1\">1</a>", targetPage));
                        paginationBuilder.Append(string.Format("<a href=\"{0}page=2\">2</a>", targetPage));
                        paginationBuilder.Append("...");
                        for (counter = (page - adjacents); counter <= (page + adjacents); counter++) {
                            paginationBuilder.Append(counter == page ? string.Format("<span class=\"current\">{0}</span>", counter) : string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, counter));
                        }
                        paginationBuilder.Append("...");
                        paginationBuilder.Append(string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, lpm1));
                        paginationBuilder.Append(string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, lastpage));
                    }
                    else {
                        //close to end only hide early pages
                        paginationBuilder.Append(string.Format("<a href=\"{0}page=1\">1</a>", targetPage));
                        paginationBuilder.Append(string.Format("<a href=\"{0}page=2\">2</a>", targetPage));
                        paginationBuilder.Append("...");
                        for (counter = (lastpage - (1 + (adjacents * 3))); counter <= lastpage; counter++) {
                            paginationBuilder.Append(counter == page ? string.Format("<span class=\"current\">{0}</span>", counter) : string.Format("<a href=\"{0}page={1}\">{1}</a>", targetPage, counter));
                        }
                    }
                }
                //nextPage button
                paginationBuilder.Append(page < counter - 1 ? string.Format("<a href=\"{0}page={1}\">下一頁</a>", targetPage, nextPage) : "<span class=\"disabled\">下一頁</span>");
                paginationBuilder.Append("</div>\r\n");
            }
            return paginationBuilder.ToString();
        }
        #endregion
    }
}