<%@ Page Title="" Language="C#" MasterPageFile="~/Sys/TayanaYacht_Manager_Mpage.Master" AutoEventWireup="true" CodeBehind="Specification_Manager_Cpage.aspx.cs" Inherits="TayanaYachtRe.Sys.Specification_Manager_Cpage" MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Specification Manager</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- Layout & Specification Start -->
<!-- Layout Start -->
<div class="page-body">
    <div class="row">
        <div class="col-md-12 col-xl-4">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Yacht Layout</h5>
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
                        <asp:DropDownList ID="DListModel" runat="server" DataSourceID="SqlDataSource1" DataTextField="yachtModel" DataValueField="id" AutoPostBack="True" Width="100%" Font-Bold="True" class="btn btn-outline-primary dropdown-toggle" OnSelectedIndexChanged="DListModel_SelectedIndexChanged"></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT [yachtModel], [id] FROM [Yachts]"></asp:SqlDataSource>
                        <hr />
                        <h6>Layout & Deck Plan Image :</h6>
                        <h6><span class="badge badge-pill badge-warning text-dark">* The maximum upload size at once is 10MB !</span></h6>
                        <div class="input-group my-3">
                            <asp:FileUpload ID="imageUpload" runat="server" class="btn btn-outline-primary btn-block" AllowMultiple="True" />
                            <asp:Button ID="UploadImgBtn" runat="server" Text="Upload" class="btn btn-primary" OnClick="UploadImgBtn_Click"/>
                        </div>
                        <hr />
                        <h6>Group Image List :</h6>
                        <asp:RadioButtonList ID="RadioButtonListImg" runat="server" class="my-3 mx-auto" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonListImg_SelectedIndexChanged"></asp:RadioButtonList>
                        <asp:Button ID="DelImageBtn" runat="server" Text="Delete Image" type="button" class="btn btn-danger btn-sm" OnClientClick="return confirm('Are you sure you want to delete？')" Visible="False" OnClick="DelImageBtn_Click"/>
                    </div>
                </div>
            </div>
        </div>
        <!-- Layout End -->

        <!-- Specification Start -->
        <div class="col-md-12 col-xl-8">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Detail Specification</h5>
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
                        <h6>Detail Title :</h6>
                        <asp:DropDownList ID="DListDetailTitle" runat="server" DataSourceID="SqlDataSource2" DataTextField="detailTitleSort" DataValueField="id" AutoPostBack="True" Width="100%" Font-Bold="True" class="btn btn-outline-primary dropdown-toggle" OnSelectedIndexChanged="DListDetailTitle_SelectedIndexChanged"></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT [detailTitleSort], [id] FROM [DetailTitleSort]"></asp:SqlDataSource>
                        <hr />
                        <h6>Add Detail :</h6>
                        <asp:TextBox ID="TboxDetail" runat="server" type="text" class="form-control" placeholder="Enter detail text" TextMode="MultiLine" Height="100px"></asp:TextBox>
                        <asp:Button ID="BtnAddDetail" runat="server" Text="Add Detail" class="btn btn-outline-primary btn-block mt-3" OnClick="BtnAddDetail_Click"/>
                        <hr />
                        <h6>Detail List :</h6>
                        <asp:RadioButtonList ID="RadioButtonListDetail" runat="server" class="my-3 mx-auto" AutoPostBack="True" RepeatDirection="Vertical" OnSelectedIndexChanged="RadioButtonListD_SelectedIndexChanged" Width="100%"></asp:RadioButtonList>
                        <asp:Button ID="BtnDelDetail" runat="server" Text="Delete Detail" type="button" class="btn btn-danger btn-sm" OnClientClick="return confirm('Are you sure you want to delete？')" Visible="False" OnClick="BtnDelDetail_Click"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Specification End -->
<!-- Layout & Specification End -->
<hr />
<!-- Detail Title Start -->
<!-- Add New Detail Title Start -->
<div class="page-body mt-5">
    <div class="row">
        <div class="col-md-12 col-xl-5">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Add New Detail Title</h5>
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
                        <hr />
                        <h6>Add New Title :</h6>
                        <div class="input-group mb-3">
                          <asp:TextBox ID="TBoxAddNewTitle" runat="server" type="text" class="form-control" placeholder="Enter new title" ></asp:TextBox>
                          <div class="input-group-append">
                            <asp:Button ID="BtnAddNewTitle" runat="server" Text="Add" class="btn btn-outline-primary btn-block" OnClick="BtnAddNewTitle_Click" />
                          </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Add New Detail Title End -->

        <!-- Detail Title List Start -->
        <div class="col-md-12 col-xl-7">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Detail Title List</h5>
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
                    <div class="table-responsive text-center">
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="id" DataSourceID="SqlDataSource3" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnRowDeleted="DeltedTitle" OnRowUpdated="UpdatedTitle" >
                            <Columns>
                                <asp:CommandField ButtonType="Button" CancelText="Cancel" DeleteText="Delete" EditText="Edit" HeaderText="Edit" InsertText="Insert" NewText="New" SelectText="Select" ShowEditButton="True"  ControlStyle-CssClass='btn btn-primary btn-block' ControlStyle-BorderColor="#66CCFF" ControlStyle-BorderStyle="Solid" ControlStyle-BorderWidth="1px" ControlStyle-ForeColor="White" >
                                <ControlStyle BorderColor="#66CCFF" BorderWidth="1px" BorderStyle="Solid" CssClass="btn btn-primary btn-block" ForeColor="White"></ControlStyle>
                                </asp:CommandField>
                                <asp:BoundField DataField="id" HeaderText="ID Number" InsertVisible="False" ReadOnly="True" SortExpression="id" >
                                <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="detailTitleSort" HeaderText="Detail Title" SortExpression="detailTitleSort" />
                                <asp:BoundField DataField="initDate" HeaderText="Creation Date" SortExpression="initDate" ReadOnly="True" InsertVisible="False" />
                                <asp:TemplateField HeaderText="Delete" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="BtnDeleteTitle" runat="server" CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete？')" CausesValidation="False"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ControlStyle BorderColor="#66CCFF" BorderStyle="Solid" BorderWidth="1px" CssClass="btn btn-danger btn-block" ForeColor="White" />
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="White" ForeColor="#000066" />
                            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                            <RowStyle ForeColor="#000066" />
                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F1F1F1" />
                            <SortedAscendingHeaderStyle BackColor="#007DBB" />
                            <SortedDescendingCellStyle BackColor="#CAC9C9" />
                            <SortedDescendingHeaderStyle BackColor="#00547E" />
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT * FROM [DetailTitleSort]" DeleteCommand="DELETE FROM [DetailTitleSort] WHERE [id] = @id" UpdateCommand="UPDATE [DetailTitleSort] SET [detailTitleSort] = @detailTitleSort WHERE [id] = @id">
                            <DeleteParameters>
                                <asp:Parameter Name="id" Type="Int32" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="detailTitleSort" Type="String" />
                                <asp:Parameter Name="id" Type="Int32" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Detail Title List End -->
<!-- Detail Title End -->
</asp:Content>
