<%@ Page Title="" Language="C#" MasterPageFile="~/Sys/TayanaYacht_Manager_Mpage.Master" AutoEventWireup="true" CodeBehind="overview_Manager_Cpage.aspx.cs" Inherits="TayanaYachtRe.Sys.overview_Manager_Cpage" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Overview Manager</title>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.9.359/pdf.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- Overview Content Start -->
<!-- Overview File Start -->
<div class="page-body">
    <div class="row">
        <div class="col-md-12 col-xl-4" runat="server">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Yacht Overview</h5>
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
                        <h6>Yacht Model :</h6>
                        <asp:DropDownList ID="DListModel" runat="server" DataSourceID="SqlDataSource1" DataTextField="yachtModel" DataValueField="yachtModel" AutoPostBack="True" Width="100%" Font-Bold="True" class="btn btn-outline-primary dropdown-toggle" OnSelectedIndexChanged="DListModel_SelectedIndexChanged" ></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT [yachtModel] FROM [Yachts]"></asp:SqlDataSource>
                        <hr />
                        <h6>Dimensions Image :</h6>
                        <asp:Image ID="DimensionsImg" runat="server" alt="Dimensions Image" class="img-thumbnail rounded mx-auto d-block" Width="250px" />
                        <div class="input-group my-3">
                            <asp:FileUpload ID="DimImgUpload" runat="server" class="btn btn-outline-primary btn-block" />
                            <asp:Button ID="BtnUploadDimImg" runat="server" Text="Upload" class="btn btn-primary" OnClick="BtnUploadDimImg_Click" />
                        </div>
                        <hr />
                        <h6>Downloads File :</h6>
                        <asp:Literal ID="PDFpreview" runat="server" ></asp:Literal>
                        <div class="input-group my-3">
                            <asp:FileUpload ID="FileUpload" runat="server" class="btn btn-outline-primary btn-block" />
                            <asp:Button ID="BtnUploadFile" runat="server" Text="Upload" class="btn btn-primary" OnClick="BtnUploadFile_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Overview File End -->

        <!-- Overview Dimensions Text Start -->
        <div class="col-md-12 col-xl-8">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Overview Dimensions Text</h5>
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
                        <h6>Dimensions Text :</h6>
                        <table class="table table-hover">
                            <thead>
                                <tr>
                                    <th>Item<asp:Button ID="AddRow" runat="server" Text="Add Row" class="btn btn-outline-primary btn-sm py-0 px-1 align-top mx-5" OnClick="AddRow_Click" /></th>
                                    <th>Value<asp:Button ID="DeleteRow" runat="server" Text="Delete" class="btn btn-outline-danger btn-sm py-0 px-1 align-top mx-5" OnClick="DeleteRow_Click" /></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Literal ID="LitDimensionsHtml" runat="server"></asp:Literal>
                                <tr>
                                    <td>
                                        <p class="d-inline-block m-r-20">Dimensions Image</p>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TBoxDimImg" runat="server" type="text" class="form-control" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p class="d-inline-block m-r-20">Downloads Title</p>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TBoxDLTitle" runat="server" type="text" class="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p class="d-inline-block m-r-20">Downloads File</p>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TBoxDLFile" runat="server" type="text" class="form-control" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <p class="d-inline-block m-r-20">Video URL</p>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TBoxVideo" runat="server" type="text" class="form-control" TextMode="Url"></asp:TextBox>
                                    </td>
                                </tr>
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td>
                                        <asp:Label ID="LabUpdateTitle" runat="server" Text="Click for Update" class="font-weight-bold"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="BtnUpdateDimensionsList" runat="server" Text="Update Dimensions List" class="btn btn-outline-primary btn-block" OnClick="BtnUpdateDimensionsList_Click" />
                                        <asp:Label ID="LabUpdateDimensionsList" runat="server" Text="*Upload Success!" ForeColor="green" class="d-flex justify-content-center" Visible="False"></asp:Label>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Overview Dimensions Text End -->
<!-- Overview Dimensions End -->
<hr />
<!-- Overview Content Text Start -->
<div class="page-body mt-5">
    <div class="row">
        <div class="col-md-12 col-xl-8">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Overview Content</h5>
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
                        <h6>Main Content :</h6>
                        <CKEditor:CKEditorControl ID="CKEditorControl1" runat="server" BasePath="/Scripts/ckeditor/"
                            Toolbar="Bold|Italic|Underline|Strike|Subscript|Superscript|-|RemoveFormat
                                NumberedList|BulletedList|-|Outdent|Indent|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock|-|BidiLtr|BidiRtl
                                /
                                Styles|Format|Font|FontSize
                                TextColor|BGColor
                                Link|Image"
                            Height="400px">
                        </CKEditor:CKEditorControl>
                        <asp:Label ID="LabUploadMainContent" runat="server" Visible="False" ForeColor="#009933" class="d-flex justify-content-center"></asp:Label>
                        <asp:Button ID="BtnUploadMainContent" runat="server" Text="Upload Overview Content" class="btn btn-outline-primary btn-block mt-3" OnClick="BtnUploadMainContent_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Overview Content Text End -->
<!-- Overview Content End -->
</asp:Content>
