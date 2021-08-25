<%@ Page Title="" Language="C#" MasterPageFile="~/Sys/TayanaYacht_Manager_Mpage.Master" AutoEventWireup="true" CodeBehind="dealers_Manager_Cpage.aspx.cs" Inherits="TayanaYachtRe.Sys.dealers_Manager_Cpage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Dealers Manager</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- Dealer Start -->
<!-- Add Area Start -->
<div class="page-body">
    <div class="row">
        <div class="col-md-12 col-xl-5">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Country Dealers</h5>
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
                        <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="SqlDataSource2" DataTextField="countrySort" DataValueField="id" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="True" Width="100%" Font-Bold="True" class="btn btn-outline-primary dropdown-toggle" ></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT * FROM [CountrySort]"></asp:SqlDataSource>
                        <asp:RadioButtonList ID="RadioButtonList1" runat="server" class="my-3" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged"></asp:RadioButtonList>
                        <asp:Button ID="BtnDelArea" runat="server" Text="Delete Area" type="button" class="btn btn-danger btn-sm" OnClick="BtnDelArea_Click" OnClientClick="return confirm('Are you sure you want to delete？')" Visible="False" />
                        <hr />
                        <h6>Add Area :</h6>
                        <div class="input-group my-3" style="left: 0px; top: 4px">
                          <asp:TextBox ID="TBoxAddArea" runat="server" type="text" class="form-control" placeholder="Enter area name" ></asp:TextBox>
                          <div class="input-group-append">
                            <asp:Button ID="BtnAddArea" runat="server" Text="Add" class="btn btn-outline-primary btn-pill" OnClick="BtnAddArea_Click"/>
                          </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Add Area End -->

        <!-- Dealer List Start -->
        <div ID="DealerList" class="col-md-12 col-xl-7" runat="server" visible="False">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Area Dealer Info</h5>
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
                        <div class="border rounded" style="padding: 5px; width: 221px; height: 160px; margin-right: auto; margin-left: auto;">
                            <p style="overflow: hidden; width: 209px; height: 148px;">
                                <asp:Literal ID="LiteralImg" runat="server"></asp:Literal>
                            </p>
                        </div>
                        <asp:Label ID="LabUploadImg" runat="server" Text="*Upload Success!" ForeColor="green" class="d-flex justify-content-center" Visible="False"></asp:Label>
                        <div class="input-group my-3">
                            <asp:FileUpload ID="FileUpload1" runat="server" class="btn btn-outline-primary btn-block" />
                            <asp:Button ID="BtnUpload" runat="server" Text="Upload Image" class="btn btn-primary" OnClick="BtnUploadImg_Click"/>
                        </div>

                        <hr />
                        <table class="table table-hover">
                            <thead>
                                <tr>
                                    <th>Item</th>
                                    <th>Value</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <th>
                                        <p class="d-inline-block m-r-20">Country : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxCountry" runat="server" type="text" class="form-control" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <p class="d-inline-block m-r-20">Area : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxArea" runat="server" type="text" class="form-control" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <p class="d-inline-block m-r-20">Image : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxImage" runat="server" type="text" class="form-control" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="table-info">
                                    <th>
                                        <p class="d-inline-block m-r-20">Name : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxName" runat="server" type="text" class="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="table-info">
                                    <th>
                                        <p class="d-inline-block m-r-20">Contact : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxContact" runat="server" type="text" class="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="table-info">
                                    <th>
                                        <p class="d-inline-block m-r-20">Address : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxAddress" runat="server" type="text" class="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="table-info">
                                    <th>
                                        <p class="d-inline-block m-r-20">Tel : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxTel" runat="server" type="text" class="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="table-info">
                                    <th>
                                        <p class="d-inline-block m-r-20">Fax : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxFax" runat="server" type="text" class="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="table-info">
                                    <th>
                                        <p class="d-inline-block m-r-20">Email : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxEmail" runat="server" type="text" class="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="table-info">
                                    <th>
                                        <p class="d-inline-block m-r-20">Link : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxLink" runat="server" type="text" class="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <p class="d-inline-block m-r-20">Creation Date : </p>
                                    </th>
                                    <td>
                                        <asp:TextBox ID="TBoxDate" runat="server" type="text" class="form-control" ReadOnly="True"></asp:TextBox>
                                    </td>
                                </tr>
                            </tbody>
                            <tfoot>
                                <tr>
                                    <th>
                                        <asp:Label ID="LabUpdateDealerList" runat="server" Text="Click for Update : " ></asp:Label>
                                    </th>
                                    <td>
                                        <asp:Button ID="BtnUpdateDealerList" runat="server" Text="Update Dealer List Value" class="btn btn-outline-primary btn-block" OnClick="BtnUpdateDealerList_Click"/>
                                        <asp:Label ID="UpdateDealerListLab" runat="server" Text="*Upload Success!" ForeColor="green" class="d-flex justify-content-center" Visible="False"></asp:Label>
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
<!-- Dealer List End -->
<!-- Dealer End -->
<hr />
<!-- Country Start -->
<!-- Add Country Start -->
<div class="page-body mt-5">
    <div class="row">
        <div class="col-md-12 col-xl-5">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Add Country</h5>
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
                          <asp:TextBox ID="TBoxAddCountry" runat="server" type="text" class="form-control" placeholder="Enter country name" ></asp:TextBox>
                          <div class="input-group-append">
                            <asp:Button ID="BtnAddCountry" runat="server" Text="Add" class="btn btn-outline-primary btn-block" OnClick="BtnAddCountry_Click"/>
                          </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Add Country End -->

        <!-- Country List Start -->
        <div class="col-md-12 col-xl-7">
            <div class="card project-task">
                <div class="card-header">
                    <div class="card-header-left ">
                        <h5>Country List</h5>
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
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="id" DataSourceID="SqlDataSource1" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="100%" OnRowDeleted="DeltedCountry">
                            <Columns>
                                <asp:CommandField ButtonType="Button" CancelText="Cancel" DeleteText="Delete" EditText="Edit" HeaderText="Edit" InsertText="Insert" NewText="New" SelectText="Select" ShowEditButton="True"  ControlStyle-CssClass='btn btn-primary btn-block' ControlStyle-BorderColor="#66CCFF" ControlStyle-BorderStyle="Solid" ControlStyle-BorderWidth="1px" ControlStyle-ForeColor="White" >
                                    <ControlStyle BorderColor="#66CCFF" BorderWidth="1px" BorderStyle="Solid" CssClass="btn btn-primary btn-block" ForeColor="White"></ControlStyle>
                                </asp:CommandField>
                                <asp:BoundField DataField="id" HeaderText="ID Number" InsertVisible="False" ReadOnly="True" SortExpression="id" >
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="countrySort" HeaderText="Country Name" SortExpression="countrySort" />
                                <asp:BoundField DataField="initDate" HeaderText="Creation Date" SortExpression="initDate" ReadOnly="True" InsertVisible="False" />
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
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TayanaYachtConnectionString %>" SelectCommand="SELECT * FROM [CountrySort]" DeleteCommand="DELETE FROM [Dealers] WHERE [country_ID] = @id; DELETE FROM [CountrySort] WHERE [id] = @id" UpdateCommand="UPDATE [CountrySort] SET [countrySort] = @countrySort WHERE [id] = @id">
                            <DeleteParameters>
                                <asp:Parameter Name="id" Type="Int32" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="countrySort" Type="String" />
                                <asp:Parameter Name="id" Type="Int32" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Country List End -->
<!-- Country End -->
</asp:Content>
