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
    public partial class Yachts_Specification : System.Web.UI.Page
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
            List<TitleList> titleTextList = new List<TitleList>();
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
            
            string modelID = "";
            if (reader.Read()) {
                string yachtModelStr = reader["yachtModel"].ToString();
                string loadDimJson = HttpUtility.HtmlDecode(reader["overviewDimensionsJSON"].ToString());
                modelID = reader["id"].ToString();

                topMenuHtmlStr.Append($"<li><a class='menu_yli01' href='Yachts_OverView.aspx?id={guidStr}' >OverView</a></li>");
                topMenuHtmlStr.Append($"<li><a class='menu_yli02' href='Yachts_Layout.aspx?id={guidStr}' >Layout & deck plan</a></li>");
                topMenuHtmlStr.Append($"<li><a class='menu_yli03' href='Yachts_Specification.aspx?id={guidStr}' >Specification</a></li>");

                saveRowList = JsonConvert.DeserializeObject<List<RowData>>(loadDimJson);

                if (!String.IsNullOrEmpty(saveRowList[0].SaveValue)) {
                    topMenuHtmlStr.Append($"<li><a class='menu_yli04' href='Yachts_Video.aspx?id={guidStr}' >Video</a></li>");
                }
                LabLink.Text = yachtModelStr;
                LiteralTtitleHtml.Text = $"<span>{yachtModelStr}</span>";
                TopMenuLinkHtml.Text = topMenuHtmlStr.ToString();
            }
            connection.Close();

            //取Title
            string sql2 = "SELECT * FROM DetailTitleSort";
            SqlCommand command2 = new SqlCommand(sql2, connection);
            connection.Open();
            SqlDataReader reader2 = command2.ExecuteReader();
            while (reader2.Read()) {
                string titleID = reader2["id"].ToString();
                string titleText = reader2["detailTitleSort"].ToString();
                //新增每筆JSON資料
                titleTextList.Add(new TitleList { TitleID = titleID, TitleText = titleText });
            }
            connection.Close();

            //取Detail
            string sql3 = "SELECT * FROM Specification WHERE yachtModel_ID = @yachtModel_ID";
            SqlCommand command3 = new SqlCommand(sql3, connection);
            command3.Parameters.AddWithValue("@yachtModel_ID", modelID);
            connection.Open();
            SqlDataReader reader3 = command3.ExecuteReader();
            StringBuilder detailHtmlStr = new StringBuilder();
            string checkNum = "";
            bool checkEnd = false;
            while (reader3.Read()) {
                string detailTitleID = reader3["detailTitleSort_ID"].ToString();
                string detailStr = HttpUtility.HtmlDecode(reader3["detail"].ToString());
                if (!checkNum.Equals(detailTitleID)) {
                    if (checkEnd) {
                        detailHtmlStr.Append("</ul>");
                    }
                    checkNum = detailTitleID;
                    foreach (var item in titleTextList) {
                        if (item.TitleID.Equals(checkNum)) {
                            detailHtmlStr.Append($"<p>{item.TitleText}</p><ul>");
                            checkEnd = true;
                        }
                    }
                }
                detailHtmlStr.Append($"<li>{detailStr}</li>");
            }
            connection.Close();
            ContentHtml.Text = detailHtmlStr.ToString();
        }

        //JSON資料
        public class TitleList
        {
            public string TitleID { get; set; }
            public string TitleText { get; set; }
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
                leftMenuHtml.Append($"<li><a href='Yachts_Specification.aspx?id={guidStr}'>{yachtModelStr}{isNewStr}</a></li>");
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