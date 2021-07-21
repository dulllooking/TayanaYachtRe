<%@ Page Title="" Language="C#" MasterPageFile="~/Sys/TayanaYacht_Manager_Mpage.Master" AutoEventWireup="true" CodeBehind="company_Manager_Cpage.aspx.cs" Inherits="TayanaYachtRe.Sys.company_Manager_Cpage" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Company Manager</title>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- Company Content Start -->
<!-- About Us Start -->
<div class="page-body">
    <div class="row">
        <div class="col-md-12 col-xl-8">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>About Us</h5>
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
                            Height="400px">
                        </CKEditor:CKEditorControl>
                        <asp:Label ID="UploadAboutUsLab" runat="server" Visible="False" ForeColor="#009933" class="d-flex justify-content-center"></asp:Label>
                        <asp:Button ID="UploadAboutUsBtn" runat="server" Text="Upload About Us Content" class="btn btn-outline-primary btn-block mt-3" OnClick="UploadAboutUsBtn_Click"/>
                    </div>
                </div>
            </div>
        </div>
        <!-- About Us End -->

        <!-- Certificat Text Start -->
        <div ID="Div1" class="col-md-12 col-xl-4" runat="server">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Certificat</h5>
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
                        <h6>Content Text :</h6>
                        <asp:TextBox ID="certificatTbox" runat="server" type="text" class="form-control" placeholder="Enter certificat text" TextMode="MultiLine" Height="200px"></asp:TextBox>
                        <asp:Label ID="uploadCertificatLab" runat="server" Visible="False" ForeColor="#009933" class="d-flex justify-content-center"></asp:Label>
                        <asp:Button ID="uploadCertificatBtn" runat="server" Text="Upload Certificat Text" class="btn btn-outline-primary btn-block mt-3" OnClick="uploadCertificatBtn_Click"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Certificat Text End -->
<hr />
<!-- Certificat ImageV Start -->
<div class="page-body mt-5">
    <div class="row">
        <div class="col-md-12 col-xl-6">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Certificat Vertical Image</h5>
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
                        <h6>Upload Vertical Group Image :</h6>
                        <div class="input-group my-3">
                            <asp:FileUpload ID="imageUploadV" runat="server" class="btn btn-outline-primary btn-block" AllowMultiple="True" />
                            <asp:Button ID="UploadVBtn" runat="server" Text="Upload" class="btn btn-primary" OnClick="UploadVBtn_Click" />
                        </div>
                        <h6>Vertical Image List :</h6>
                        <asp:RadioButtonList ID="RadioButtonListV" runat="server" class="my-3 mx-auto" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonListV_SelectedIndexChanged" RepeatDirection="Horizontal" RepeatColumns="3" CellPadding="10"></asp:RadioButtonList>
                        <asp:Button ID="DelVImageBtn" runat="server" Text="Delete Image" type="button" class="btn btn-danger btn-sm" OnClientClick="return confirm('Are you sure you want to delete？')" Visible="False" OnClick="DelVImageBtn_Click" />
                    </div>
                </div>
            </div>
        </div>
        <!-- Certificat ImageV End -->

        <!-- Certificat ImageH Start -->
        <div ID="Div2" class="col-md-12 col-xl-6" runat="server">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Certificat Horizontal Image</h5>
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
                        <h6>Upload Horizontal Group Image :</h6>
                        <div class="input-group my-3">
                            <asp:FileUpload ID="imageUploadH" runat="server" class="btn btn-outline-primary btn-block" AllowMultiple="True" />
                            <asp:Button ID="UploadHBtn" runat="server" Text="Upload" class="btn btn-primary" OnClick="UploadHBtn_Click" />
                        </div>
                        <h6>Horizontal Image List :</h6>
                        <asp:RadioButtonList ID="RadioButtonListH" runat="server" class="my-3 mx-auto" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonListH_SelectedIndexChanged" CellPadding="10" RepeatColumns="2" RepeatDirection="Horizontal"></asp:RadioButtonList>
                        <asp:Button ID="DelHImageBtn" runat="server" Text="Delete Image" type="button" class="btn btn-danger btn-sm" OnClientClick="return confirm('Are you sure you want to delete？')" Visible="False" OnClick="DelHImageBtn_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Certificat ImageH End -->
<!-- Company Content End -->
</asp:Content>
