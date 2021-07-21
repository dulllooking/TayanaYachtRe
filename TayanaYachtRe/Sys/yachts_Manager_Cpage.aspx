<%@ Page Title="" Language="C#" MasterPageFile="~/Sys/TayanaYacht_Manager_Mpage.Master" AutoEventWireup="true" CodeBehind="yachts_Manager_Cpage.aspx.cs" Inherits="TayanaYachtRe.Sys.yachts_Manager_Cpage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Yacht Model Manager</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- Yachts Banner Start -->
<div class="page-body">
    <div class="row">
        <div class="col-md-12 col-xl-12">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Yacht Model Banner Image</h5>
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
                        <div class="input-group my-3">
                            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" DataSourceID="SqlDataSource1" DataTextField="yachtModel" DataValueField="yachtModel" Width="50%" Font-Bold="True" class="btn btn-outline-primary dropdown-toggle" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"></asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT [yachtModel] FROM [Yachts]"></asp:SqlDataSource>
                            <asp:FileUpload ID="imageUploadH" runat="server" class="btn btn-outline-primary btn-block" AllowMultiple="True" />
                            <asp:Button ID="UploadHBtn" runat="server" Text="Upload" class="btn btn-primary" OnClick="UploadHBtn_Click" />
                        </div>
                        <hr />
                        <h6>Banner Image List :</h6>
                        <h6><span class="badge badge-pill badge-info text-dark">* The first image will be the home page banner !</span></h6>
                        <h6>Step1. To upload one image to be the home page banner.</h6>
                        <h6>Step2. Then upload other images.</h6>
                        <asp:RadioButtonList ID="RadioButtonListH" runat="server" class="my-3 mx-auto" AutoPostBack="True" CellPadding="10" RepeatColumns="5" RepeatDirection="Horizontal" OnSelectedIndexChanged="RadioButtonListH_SelectedIndexChanged" ></asp:RadioButtonList>
                        <asp:Button ID="DelHImageBtn" runat="server" Text="Delete Image" type="button" class="btn btn-danger btn-sm" OnClientClick="return confirm('Are you sure you want to delete？')" Visible="False" OnClick="DelHImageBtn_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Yachts Banner End -->
<hr />
<!-- Yachts Model Start -->
<!-- Add Model Start -->
<div class="page-body mt-5">
    <div class="row">
        <div class="col-md-12 col-xl-4">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Add Model</h5>
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
                        <asp:CheckBox ID="CBoxNewDesign" runat="server" Text="NewDesign" Width="50%" />
                        <asp:CheckBox ID="CBoxNewBuilding" runat="server" Text="NewBuilding" Width="50%" />
                        <div class="input-group mb-3">
                            <asp:TextBox ID="TBoxAddYachtModel" runat="server" type="text" class="form-control" placeholder="Model" Width="30%" ></asp:TextBox>
                            <asp:TextBox ID="TBoxAddYachtLength" runat="server" type="text" class="form-control" placeholder="Length" ></asp:TextBox>
                            <div class="input-group-append">
                                <asp:Button ID="BtnAddYacht" runat="server" Text="Add" class="btn btn-outline-primary btn-block" OnClick="BtnAddYacht_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Add Model End -->

        <!-- Model List Start -->
        <div class="col-md-12 col-xl-8">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Yacht Model List</h5>
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
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="id" DataSourceID="SqlDataSource2" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnRowDeleted="DeletedModel" OnRowUpdated="UpdatedModel" OnRowDeleting="DeletingModel">
                            <Columns>
                                <asp:CommandField ButtonType="Button" CancelText="Cancel" DeleteText="Delete" EditText="Edit" HeaderText="Edit" InsertText="Insert" NewText="New" SelectText="Select" ShowEditButton="True"  ControlStyle-CssClass='btn btn-primary btn-block' ControlStyle-BorderColor="#66CCFF" ControlStyle-BorderStyle="Solid" ControlStyle-BorderWidth="1px" ControlStyle-ForeColor="White" >
                                <ControlStyle BorderColor="#66CCFF" BorderWidth="1px" BorderStyle="Solid" CssClass="btn btn-primary btn-block" ForeColor="White"></ControlStyle>
                                </asp:CommandField>
                                <asp:BoundField DataField="id" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="id" />
                                <asp:BoundField DataField="yachtModel" HeaderText="Yacht Model" SortExpression="yachtModel" />
                                <asp:CheckBoxField DataField="isNewDesign" HeaderText="New Design" SortExpression="isNewDesign" />
                                <asp:CheckBoxField DataField="isNewBuilding" HeaderText="New Building" SortExpression="isNewBuilding" />
                                <asp:BoundField DataField="initData" HeaderText="Creation Date" SortExpression="initData" InsertVisible="False" ReadOnly="True" />
                                <asp:TemplateField HeaderText="Delete" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="BtnDeleteCountry" runat="server" CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete？')" CausesValidation="False"></asp:LinkButton>
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
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT [id], [yachtModel], [isNewDesign], [isNewBuilding], [initData] FROM [Yachts]" DeleteCommand="DELETE FROM [Specification] WHERE [yachtModel_ID] = @id; DELETE FROM [Yachts] WHERE [id] = @id" UpdateCommand="UPDATE [Yachts] SET [yachtModel] = @yachtModel, [isNewDesign] = @isNewDesign, [isNewBuilding] = @isNewBuilding WHERE [id] = @id">
                            <DeleteParameters>
                                <asp:Parameter Name="id" Type="Int32" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="isNewBuilding" Type="Boolean" />
                                <asp:Parameter Name="isNewDesign" Type="Boolean" />
                                <asp:Parameter Name="yachtModel" Type="String" />
                                <asp:Parameter Name="id" Type="Int32" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Model List End -->
<!-- Yachts Type End -->
</asp:Content>
