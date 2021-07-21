<%@ Page Title="" Language="C#" MasterPageFile="~/Sys/TayanaYacht_Manager_Mpage.Master" AutoEventWireup="true" CodeBehind="new_list_Manager_Cpage.aspx.cs" Inherits="TayanaYachtRe.Sys.new_list_Manager_Cpage" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>News Manager</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- News Cover Start -->
<!-- Add News Start -->
<div class="page-body">
    <div class="row">
        <div class="col-md-12 col-xl-8">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Add News</h5>
                    </div>
                    <div class="card-header-right">
                        <ul class="list-unstyled card-option">
                            <li><i class="icofont icofont-simple-left "></i></li>
                            <li><i class="icofont icofont-maximize full-card"></i></li>
                            <li><i class="icofont icofont-minus minimize-card"></i></li>
                            <li><i class="icofont icofont-refresh reload-card"></i></li>
                            <li><i class="icofont icofont-error close-card"></i></li>
                        </ul>
                    </div>
                </div>
                <div class="card-block p-b-10">
                    <div class="table-responsive">
                        <h6>Date :</h6>
                        <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="White" BorderWidth="1px" Font-Names="Verdana" Font-Size="9pt" ForeColor="Black" Height="190px" NextPrevFormat="FullMonth" Width="100%" OnSelectionChanged="Calendar1_SelectionChanged" OnDayRender="Calendar1_DayRender">
                            <DayHeaderStyle Font-Bold="True" Font-Size="8pt" />
                            <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" />
                            <OtherMonthDayStyle ForeColor="#999999" />
                            <SelectedDayStyle BackColor="#3399FF" ForeColor="White" Font-Bold="True" />
                            <TitleStyle BackColor="White" BorderColor="#3399FF" BorderWidth="3px" Font-Bold="True" Font-Size="12pt" ForeColor="#3399FF" />
                            <TodayDayStyle BackColor="#CCCCCC" />
                        </asp:Calendar>
                        <hr />
                        <h6>Headline : <asp:Label ID="LabIsTop" runat="server" Text="* Select item is top news !" ForeColor="Red" Visible="False" class="badge badge-pill badge-warning text-dark"></asp:Label></h6>
                        <asp:RadioButtonList ID="headlineRadioBtnList" runat="server" class="my-3" AutoPostBack="True" OnSelectedIndexChanged="headlineRadioBtnList_SelectedIndexChanged"></asp:RadioButtonList>
                        <asp:Button ID="deleteNewsBtn" runat="server" Text="Delete News" type="button" class="btn btn-danger btn-sm" OnClientClick="return confirm('Are you sure you want to delete？')" Visible="False" OnClick="deleteNewsBtn_Click" />
                        <hr />
                        <h6>Add Headline :</h6>
                        <asp:CheckBox ID="CBoxIsTop" runat="server" Text="Top Tag" Width="100%" />
                        <asp:TextBox ID="headlineTbox" runat="server" type="text" class="form-control" placeholder="Enter headline text" MaxLength="75"></asp:TextBox>
                        <asp:Button ID="AddHeadlineBtn" runat="server" Text="Add Headline" class="btn btn-outline-primary btn-block mt-3" OnClick="AddHeadlineBtn_Click"/>
                    </div>
                </div>
            </div>
        </div>
        <!-- Add News End -->

        <!-- Cover Content Start -->
        <div ID="CoverList" class="col-md-12 col-xl-4" runat="server">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Cover Content</h5>
                    </div>
                    <div class="card-header-right">
                        <ul class="list-unstyled card-option">
                            <li><i class="icofont icofont-simple-left "></i></li>
                            <li><i class="icofont icofont-maximize full-card"></i></li>
                            <li><i class="icofont icofont-minus minimize-card"></i></li>
                            <li><i class="icofont icofont-refresh reload-card"></i></li>
                            <li><i class="icofont icofont-error close-card"></i></li>
                        </ul>
                    </div>
                </div>
                <div class="card-block p-b-10">
                    <div class="table-responsive">
                        <h6>Thumbnail :</h6>
                        <asp:Image ID="Thumbnail" runat="server" alt="Thumbnail Image" class="img-thumbnail rounded mx-auto d-block" Width="161px" Height="121px" />
                        <asp:Label ID="LabUploadThumbnail" runat="server" Text="*Upload Success!" ForeColor="green" class="d-flex justify-content-center" Visible="False"></asp:Label>
                        <div class="input-group my-3">
                            <asp:FileUpload ID="thumbnailUpload" runat="server" class="btn btn-outline-primary btn-block" />
                            <asp:Button ID="uploadThumbnailBtn" runat="server" Text="Upload Image" class="btn btn-primary" OnClick="UploadThumbnailBtn_Click" />
                        </div>
                        <hr />
                        <h6>Summary :</h6>
                        <asp:TextBox ID="summaryTbox" runat="server" type="text" class="form-control" placeholder="Enter summary text" TextMode="MultiLine" Height="170px" MaxLength="325"></asp:TextBox>
                        <asp:Label ID="LabUploadSummary" runat="server" Text="*Upload Success!" ForeColor="green" class="d-flex justify-content-center" Visible="False"></asp:Label>
                        <asp:Button ID="uploadSummaryBtn" runat="server" Text="Upload Summary" class="btn btn-outline-primary btn-block mt-3" OnClick="UploadSummaryBtn_Click"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Cover Content End -->
<!-- News Cover End -->
<hr />
<!-- News Content Start -->
<!-- Content Text Start -->
<div ID="NewsContent" class="page-body mt-5" runat="server">
    <div class="row">
        <div class="col-md-12 col-xl-8">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Main Content</h5>
                    </div>
                    <div class="card-header-right">
                        <ul class="list-unstyled card-option">
                            <li><i class="icofont icofont-simple-left "></i></li>
                            <li><i class="icofont icofont-maximize full-card"></i></li>
                            <li><i class="icofont icofont-minus minimize-card"></i></li>
                            <li><i class="icofont icofont-refresh reload-card"></i></li>
                            <li><i class="icofont icofont-error close-card"></i></li>
                        </ul>
                    </div>
                </div>
                <div class="card-block p-b-10">
                    <div class="table-responsive">
                        <h6>Content :</h6>
                        <CKEditor:CKEditorControl ID="CKEditorControl1" runat="server" BasePath="/Scripts/ckeditor/"
                            Toolbar="Bold|Italic|Underline|Strike|Subscript|Superscript|-|RemoveFormat
                                NumberedList|BulletedList|-|Outdent|Indent|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock|-|BidiLtr|BidiRtl
                                /
                                Styles|Format|Font|FontSize
                                TextColor|BGColor
                                Link|Image"
                            Height="300px">
                        </CKEditor:CKEditorControl>
                        <asp:Label ID="UploadContentLab" runat="server" Text="*Upload Success!" ForeColor="green" class="d-flex justify-content-center" Visible="False"></asp:Label>
                        <asp:Button ID="UploadContentBtn" runat="server" Text="Upload News Content" class="btn btn-outline-primary btn-block mt-3" OnClick="UploadContentBtn_Click"/>
                    </div>
                </div>
            </div>
        </div>
        <!-- Content Text End -->

        <!-- Group Image Start -->
        <div ID="Div1" class="col-md-12 col-xl-4" runat="server">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Group Image</h5>
                        <h6><span class="badge badge-pill badge-warning text-dark">* The maximum upload size at once is 10MB !</span></h6>
                    </div>
                    <div class="card-header-right">
                        <ul class="list-unstyled card-option">
                            <li><i class="icofont icofont-simple-left "></i></li>
                            <li><i class="icofont icofont-maximize full-card"></i></li>
                            <li><i class="icofont icofont-minus minimize-card"></i></li>
                            <li><i class="icofont icofont-refresh reload-card"></i></li>
                            <li><i class="icofont icofont-error close-card"></i></li>
                        </ul>
                    </div>
                </div>
                <div class="card-block p-b-10">
                    <div class="table-responsive">
                        <h6>Upload Group Image :</h6>
                        <div class="input-group my-3">
                            <asp:FileUpload ID="imageUpload" runat="server" class="btn btn-outline-primary btn-block" AllowMultiple="True" />
                            <asp:Button ID="UploadImgBtn" runat="server" Text="Upload" class="btn btn-primary" OnClick="UploadImgBtn_Click" />
                        </div>
                        <hr />
                        <h6>Group Image List :</h6>
                        <asp:RadioButtonList ID="RadioButtonListImg" runat="server" class="my-3 mx-auto" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonListImg_SelectedIndexChanged"></asp:RadioButtonList>
                        <asp:Button ID="DelImageBtn" runat="server" Text="Delete Image" type="button" class="btn btn-danger btn-sm" OnClientClick="return confirm('Are you sure you want to delete？')" Visible="False" OnClick="DelImageBtn_Click"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Group Image End -->
<!-- News Content End -->
</asp:Content>
