<%@ Page Title="" Language="C#" MasterPageFile="~/Tayanahtml/Yachts.Master" AutoEventWireup="true" CodeBehind="Yachts_Layout.aspx.cs" Inherits="TayanaYachtRe.Tayanahtml.Yachts_Layout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="box6">
    <p> Layout & deck plan</p>
    <ul>
      <asp:Literal ID="ContentHtml" runat="server"></asp:Literal>
    </ul>
  </div>
  <div class="clear">
  </div>
  <p class="topbuttom"><img src="images/top.gif" alt="top" /></p>
  <!--下載開始-->
  
  <!--下載結束-->
</asp:Content>
