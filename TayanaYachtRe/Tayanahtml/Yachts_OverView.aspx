<%@ Page Title="" Language="C#" MasterPageFile="~/Tayanahtml/Yachts.Master" AutoEventWireup="true" CodeBehind="Yachts_OverView.aspx.cs" Inherits="TayanaYachtRe.Tayanahtml.Yachts_OverView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="box1">
    <asp:Literal ID="ContentHtml" runat="server"></asp:Literal>
    &nbsp;
</div>
<div class="box3" id="dimensionTable" runat="server">
    <h4 id="dimensionTitle" runat="server"></h4>
    <table class="table02">
        <tbody>
            <tr>
                <td class="table02td01">
                    <table>
                        <tbody>
                            <asp:Literal ID="DimensionsTableHtml" runat="server"></asp:Literal>
                        </tbody>
                    </table>
                </td>
                <asp:Literal ID="DimensionsImgHtml" runat="server"></asp:Literal>
            </tr>
        </tbody>
    </table>
</div>
<p class="topbuttom"><img src="images/top.gif" alt="top" /></p>
<!--下載開始-->
<div id="divDownload" class="downloads" runat="server">
    <p><img src="images/downloads.gif" alt="&quot;&quot;" /></p>
    <ul>
        <li>
            <asp:Literal ID="DownloadsHtml" runat="server"></asp:Literal>
        </li>
    </ul>
</div>
<!--下載結束-->
</asp:Content>
