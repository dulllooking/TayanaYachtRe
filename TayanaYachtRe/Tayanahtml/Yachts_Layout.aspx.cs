using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace TayanaYachtRe.Tayanahtml
{
    public partial class Yachts_Layout : System.Web.UI.Page
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
            List<LayoutPath> saveImagPathList = new List<LayoutPath>();
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
            StringBuilder layoutHtmlStr = new StringBuilder();
            if (reader.Read()) {
                string yachtModelStr = reader["yachtModel"].ToString();
                string loadDimJson = HttpUtility.HtmlDecode(reader["overviewDimensionsJSON"].ToString());
                string loadImgJson = HttpUtility.HtmlDecode(reader["layoutDeckPlanImgPathJSON"].ToString());

                topMenuHtmlStr.Append($"<li><a class='menu_yli01' href='Yachts_OverView.aspx?id={guidStr}' >OverView</a></li>");
                topMenuHtmlStr.Append($"<li><a class='menu_yli02' href='Yachts_Layout.aspx?id={guidStr}' >Layout & deck plan</a></li>");
                topMenuHtmlStr.Append($"<li><a class='menu_yli03' href='Yachts_Specification.aspx?id={guidStr}' >Specification</a></li>");

                saveRowList = JsonConvert.DeserializeObject<List<RowData>>(loadDimJson);
                saveImagPathList = JsonConvert.DeserializeObject<List<LayoutPath>>(loadImgJson);

                if (!String.IsNullOrEmpty(saveRowList[0].SaveValue)) {
                    topMenuHtmlStr.Append($"<li><a class='menu_yli04' href='Yachts_Video.aspx?id={guidStr}' >Video</a></li>");
                }
                foreach (var item in saveImagPathList) {
                    layoutHtmlStr.Append($"<li><img src='upload/Images/{item.SavePath}' alt='&quot;&quot;' /></li>");
                }

                LabLink.Text = yachtModelStr;
                LiteralTtitleHtml.Text = $"<span>{yachtModelStr}</span>";
                TopMenuLinkHtml.Text = topMenuHtmlStr.ToString();
                ContentHtml.Text = layoutHtmlStr.ToString();
            }
            connection.Close();

        }

        //JSON資料
        public class LayoutPath
        {
            public string SavePath { get; set; }
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
                leftMenuHtml.Append($"<li><a href='Yachts_Layout.aspx?id={guidStr}'>{yachtModelStr}{isNewStr}</a></li>");
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