using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class Yachts_OverView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //會先跑 Content 頁的 Page_Load 才跑 Master 頁的 Page_Load
            if (!IsPostBack) {
                loadContent();
            }
        }

        private void loadContent()
        {
            //取得 Session 共用 GUID，Session 物件需轉回字串
            string guidStr = Session["guid"].ToString();
            //依 GUID 取得遊艇資料
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT * FROM Yachts WHERE guid = @guidStr";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@guidStr", guidStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            StringBuilder dimensionsTableHtmlStr = new StringBuilder();
            List<RowData> saveRowList = new List<RowData>();
            if (reader.Read()) {
                string yachtModelStr = reader["yachtModel"].ToString();
                string contentHtmlStr = HttpUtility.HtmlDecode(reader["overviewContentHtml"].ToString());
                string loadJson = HttpUtility.HtmlDecode(reader["overviewDimensionsJSON"].ToString());
                string dimensionsImgPathStr = reader["overviewDimensionsImgPath"].ToString();
                string downloadsFilePathStr = reader["overviewDownloadsFilePath"].ToString();
                saveRowList = JsonConvert.DeserializeObject<List<RowData>>(loadJson);

                //渲染型號主要內容
                ContentHtml.Text = contentHtmlStr;

                //渲染 DIMENSIONS 尺寸資料區塊 (第 3 筆開始才是尺寸資料)
                if (saveRowList?.Count > 2) {
                    //渲染尺寸表型號標題
                    string[] yachtModelArr = yachtModelStr.Split(' ');
                    dimensionTitle.InnerText = yachtModelArr[1] + " DIMENSIONS";

                    //加入渲染 DIMENSIONS 尺寸資料
                    int count = 1;
                    foreach (var item in saveRowList) {
                        //第1筆是 Video 網址，第2筆是 Download 檔名，從第3筆開始取
                        if (count > 2) {
                            dimensionsTableHtmlStr.Append($"<tr><th>{item.SaveItem}</th><td>{item.SaveValue}</td></tr>");
                        }
                        count++;
                    }
                    //渲染尺寸表格文字內容
                    DimensionsTableHtml.Text = dimensionsTableHtmlStr.ToString();

                    //渲染尺寸表格圖片內容，無圖片時不執行
                    if (!String.IsNullOrEmpty(dimensionsImgPathStr)) {
                        DimensionsImgHtml.Text = $"<td><img alt='{yachtModelStr}' src='upload/Images/{dimensionsImgPathStr}' Width='278px' /></td>";
                    }
                }
                else {
                    //無尺寸資料則隱藏整個區塊
                    dimensionTable.Visible = false;
                }


                //渲染下方 Downloads 區塊
                if (!String.IsNullOrEmpty(downloadsFilePathStr)) {
                    string downloadsTitle = saveRowList[1].SaveValue;
                    if (String.IsNullOrEmpty(downloadsTitle)) {
                        //如果沒設定 PDF 標題文字，則顯示文字改為 PDF 檔名
                        downloadsTitle = downloadsFilePathStr;
                    }
                    //渲染下載連結
                    DownloadsHtml.Text = $"<a id='HyperLink1' href='upload/files/{downloadsFilePathStr}' target='blank' >{downloadsTitle}</a>";
                }
                else {
                    //無下載連結則隱藏整個區塊
                    divDownload.Visible = false;
                }
            }
            connection.Close();
        }

        //表格欄位 JSON 資料
        public class RowData
        {
            public string SaveItem { get; set; }
            public string SaveValue { get; set; }
        }

    }
}