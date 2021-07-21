using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class Yachts_OverView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                getGuid();
                loadGallery();
                loadLeftMenu();
                loadContent();
            }
        }

        private void getGuid()
        {
            //取得guid
            string guidStr = Request.QueryString["id"];
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT * FROM Yachts";
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                if (String.IsNullOrEmpty(guidStr)) {
                    guidStr = reader["guid"].ToString().Trim();
                }
            }
            Session["guid"] = guidStr;
        }

        private void loadContent()
        {
            List<RowData> saveRowList = new List<RowData>();
            //取得guid
            string guidStr = Session["guid"].ToString();
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //取資料
            string sql = "SELECT * FROM Yachts WHERE guid = @guid";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@guid", guidStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            StringBuilder topMenuHtmlStr = new StringBuilder();
            StringBuilder dimensionsTableHtmlStr = new StringBuilder();
            if (reader.Read()) {
                string yachtModelStr = reader["yachtModel"].ToString();
                string[] yachtModelArr = yachtModelStr.Split(' ');
                string contentHtmlStr = HttpUtility.HtmlDecode(reader["overviewContentHtml"].ToString());
                string loadJson = HttpUtility.HtmlDecode(reader["overviewDimensionsJSON"].ToString());
                string dimensionsImgPathStr = reader["overviewDimensionsImgPath"].ToString();
                string downloadsFilePathStr = reader["overviewDownloadsFilePath"].ToString();

                topMenuHtmlStr.Append($"<li><a class='menu_yli01' href='Yachts_OverView.aspx?id={guidStr}' >OverView</a></li>");
                topMenuHtmlStr.Append($"<li><a class='menu_yli02' href='Yachts_Layout.aspx?id={guidStr}' >Layout & deck plan</a></li>");
                topMenuHtmlStr.Append($"<li><a class='menu_yli03' href='Yachts_Specification.aspx?id={guidStr}' >Specification</a></li>");

                saveRowList = JsonConvert.DeserializeObject<List<RowData>>(loadJson);
                if (!String.IsNullOrEmpty(saveRowList[0].SaveValue)) {
                    topMenuHtmlStr.Append($"<li><a class='menu_yli04' href='Yachts_Video.aspx?id={guidStr}' >Video</a></li>");
                }
                int count = 0;
                foreach (var item in saveRowList) {
                    if (count > 1) {
                        dimensionsTableHtmlStr.Append($"<tr><th>{item.SaveItem}</th><td>{item.SaveValue}</td></tr>");
                    }
                    count++;
                }

                LabLink.Text = yachtModelStr;
                LiteralTtitleHtml.Text = $"<span>{yachtModelStr}</span>";
                TopMenuLinkHtml.Text = topMenuHtmlStr.ToString();
                ContentHtml.Text = contentHtmlStr;
                LabNumber.Text = yachtModelArr[1];
                DimensionsTableHtml.Text = dimensionsTableHtmlStr.ToString();
                if (!String.IsNullOrEmpty(dimensionsImgPathStr)) {
                    DimensionsImgHtml.Text = $"<td><img alt='{yachtModelStr}' src='{dimensionsImgPathStr}' /></td>";
                }
                if (!String.IsNullOrEmpty(dimensionsImgPathStr)) {
                    DownloadsHtml.Text = "<div id='divDownload' class='downloads'><p><img src='images/downloads.gif' alt='&quot;&quot;' /></p>" +
                        $"<ul><li><a id='HyperLink1' href='./{downloadsFilePathStr}' target='blank' >{saveRowList[1].SaveValue}</a></li></ul></div>";
                }
            }
            connection.Close();
            
        }

        //JSON資料
        public class RowData
        {
            public string SaveItem { get; set; }
            public string SaveValue { get; set; }
        }

        private void loadLeftMenu()
        {
            StringBuilder leftMenuHtml = new StringBuilder();
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            string sql = "SELECT * FROM Yachts";
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                string yachtModelStr = reader["yachtModel"].ToString();
                string isNewDesignStr = reader["isNewDesign"].ToString();
                string isNewBuildingStr = reader["isNewBuilding"].ToString();
                string guidStr = reader["guid"].ToString();
                string isNewStr = "";
                if (isNewDesignStr.Equals("True")) {
                    isNewStr = " (New Design)";
                }
                else if (isNewBuildingStr.Equals("True")) {
                    isNewStr = " (New Building)";
                }
                leftMenuHtml.Append($"<li><a href='Yachts_OverView.aspx?id={guidStr}'>{yachtModelStr}{isNewStr}</a></li>");
            }
            connection.Close();
            LeftMenuHtml.Text = leftMenuHtml.ToString();
        }

        private void loadGallery()
        {
            //圖片必須用Repeater送不然JS抓不到html標籤會失敗
            //建立資料表存資料
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[1] { new DataColumn("ImageUrl") });
            List<ImagePath> savePathList = new List<ImagePath>();
            //取得guid
            string guidStr = Session["guid"].ToString();
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TayanaYachtConnectionString"].ConnectionString);
            //取資料
            string sql = "SELECT * FROM Yachts WHERE guid = @guid";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@guid", guidStr);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            StringBuilder galleryImgHtmlStr = new StringBuilder();
            if (reader.Read()) {
                string loadJson = HttpUtility.HtmlDecode(reader["bannerImgPathJSON"].ToString());
                savePathList = JsonConvert.DeserializeObject<List<ImagePath>>(loadJson);
                foreach (var item in savePathList) {
                    dataTable.Rows.Add($"upload/Images/{item.SavePath}");
                }
            }
            connection.Close();
            RepeaterImg.DataSource = dataTable;
            RepeaterImg.DataBind();
        }

        //JSON資料
        public class ImagePath
        {
            public string SavePath { get; set; }
        }

    }
}