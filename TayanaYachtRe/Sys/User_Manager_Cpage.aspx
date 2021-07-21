<%@ Page Title="" Language="C#" MasterPageFile="~/Sys/TayanaYacht_Manager_Mpage.Master" AutoEventWireup="true" CodeBehind="User_Manager_Cpage.aspx.cs" Inherits="TayanaYachtRe.Sys.User_Manager_Cpage" %>

<asp:Content ID="ManagerHeadContent" ContentPlaceHolderID="ManagerHead" runat="server">
    <title>User Manager</title>
</asp:Content>

<asp:Content ID="ManagerMenuContent" ContentPlaceHolderID="ManagerMenuContentPlaceHolder" runat="server">
<li>
    <a href="User_Manager_Cpage.aspx">
        <span class="pcoded-micon"><i class="ti-layers"></i><b>FC</b></span>
        <span class="pcoded-mtext" data-i18n="nav.form-components.main">User &amp; Password</span>
        <span class="pcoded-mcaret"></span>
    </a>
</li>
</asp:Content>

<asp:Content ID="ManagerMainContent" ContentPlaceHolderID="ManagerMainContentPlaceHolder" runat="server">
<!-- User Start -->
<!-- Add User Start -->
<div class="page-body">
    <div class="row">
        <div class="col-md-12 col-xl-4">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Add User</h5>
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
                        <div class="input-group mb-3">
                            <asp:TextBox ID="TBoxAddAccount" runat="server" type="text" class="form-control" placeholder="Account" ></asp:TextBox>
                            <asp:TextBox ID="TBoxAddPassword" runat="server" type="text" class="form-control" placeholder="Password" TextMode="Password" ></asp:TextBox>
                            <div class="input-group-append">
                                <asp:Button ID="BtnAddAccount" runat="server" Text="Add" class="btn btn-outline-primary btn-block" OnClick="BtnAddAccount_Click" />
                            </div>
                        </div>
                        <asp:Label ID="LabelAdd" runat="server" Visible="False"><span class="badge badge-pill badge-warning text-dark">* Account Name has repeated !</span></asp:Label>
                    </div>
                </div>
            </div>
        </div>
        <!-- Add User End -->

        <!-- User List Start -->
        <div class="col-md-12 col-xl-8">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>User List</h5>
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
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="id" DataSourceID="SqlDataSource1" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnDataBound="OnDataBind" >
                            <Columns>
                                <asp:CommandField ButtonType="Button" CancelText="Cancel" DeleteText="Delete" EditText="Edit" HeaderText="Edit" InsertText="Insert" NewText="New" SelectText="Select" ShowEditButton="True"  ControlStyle-CssClass='btn btn-primary btn-block' ControlStyle-BorderColor="#66CCFF" ControlStyle-BorderStyle="Solid" ControlStyle-BorderWidth="1px" ControlStyle-ForeColor="White" >
                                <ControlStyle BorderColor="#66CCFF" BorderWidth="1px" BorderStyle="Solid" CssClass="btn btn-primary btn-block" ForeColor="White"></ControlStyle>
                                </asp:CommandField>
                                <asp:BoundField DataField="id" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="id" >
                                <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="account" HeaderText="Account" SortExpression="account" />
                                <asp:BoundField DataField="email" HeaderText="Email" SortExpression="email" />
                                <asp:BoundField DataField="name" HeaderText="Name" SortExpression="name" />
                                <asp:BoundField DataField="initDate" HeaderText="Creation Date" SortExpression="initDate" ReadOnly="True" InsertVisible="False" />
                                <asp:TemplateField HeaderText="Delete" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="BtnDelete" runat="server" CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete？')" CausesValidation="False"></asp:LinkButton>
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
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT [id], [account], [email], [name], [initDate] FROM [managerData]" DeleteCommand="DELETE FROM [managerData] WHERE [id] = @id" UpdateCommand="UPDATE [managerData] SET [account] = @account, [email] = @email, [name] = @name WHERE [id] = @id">
                            <DeleteParameters>
                                <asp:Parameter Name="id" Type="Int32" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="account" Type="String" />
                                <asp:Parameter Name="email" Type="String" />
                                <asp:Parameter Name="name" Type="String" />
                                <asp:Parameter Name="id" Type="Int32" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- User List End -->
<!-- User End -->
</asp:Content>


