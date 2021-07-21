<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Yachts_OverView.aspx.cs" Inherits="TayanaYachtRe.Tayanahtml.Yachts_OverView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>
        Tayana | Tayana Yachts Official Website
    </title>
    <script type="text/javascript" src="Scripts/jquery.min.js"></script>
    <!--[if lt IE 7]>
    <script type="text/javascript" src="javascript/iepngfix_tilebg.js"></script>
    <![endif]-->
    <link rel="shortcut icon" href="favicon.ico" />
    <link href="css/homestyle.css" rel="stylesheet" type="text/css" />
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $('.topbuttom').click(function () {
                $('html, body').scrollTop(0);
            });
        });
    </script>

    <link rel="stylesheet" type="text/css" href="css/jquery.ad-gallery.css"/>
    <style type="text/css">
        img,
        div,
        input {
            behavior: url("");
        }
    </style>
    <script type="text/javascript" src="Scripts/jquery.ad-gallery.js"></script>
    <script type="text/javascript">
        $(function () {

            var galleries = $('.ad-gallery').adGallery();
            galleries[0].settings.effect = 'fade';
            if ($('.banner input[type=hidden]').val() == "0") {
                $(".bannermasks").hide();
                $(".banner").hide();
                $("#crumb").css("top", "125px");
            }


        });
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <div class="contain">
            <div class="sub">
                <p>
                    <a href="index.aspx">Home</a>
                </p>
            </div>
            <!--------------------------------選單開始---------------------------------------------------->
            <div id="logol"><a href="index.aspx"><img src="images/logo001.gif" alt="Tayana" /></a></div>
            <div class="menu">
                <ul>
                    <li class="menuli01"><a href="Yachts_OverView.aspx">Yachts</a></li>
                    <li class="menuli02"><a href="new_list.aspx">NEWS</a></li>
                    <li class="menuli03"><a href="compan.aspx">COMPANY</a></li>
                    <li class="menuli04"><a href="dealers.aspx">DEALERS</a></li>
                    <li class="menuli05"><a href="contact.aspx">CONTACT</a></li>
                </ul>
            </div>
            <!--------------------------------選單開始結束---------------------------------------------------->
            <!--遮罩-->
            <div class="bannermasks"><img src="images/banner01_masks.png" alt="&quot;&quot;" /></div>
            <!--遮罩結束-->

            <div class="banner1">
                <input type="hidden" name="ctl00$ContentPlaceHolder1$Gallery1$HiddenField1"
                    id="ctl00_ContentPlaceHolder1_Gallery1_HiddenField1" value="1" />
                <div id="gallery" class="ad-gallery">
                    <div class="ad-image-wrapper">
                    </div>
                    <div class="ad-controls">
                    </div>
                    <div class="ad-nav">
                        <div class="ad-thumbs">
                            <ul class="ad-thumb-list">
                                <asp:Repeater ID="RepeaterImg" runat="server">
                                    <ItemTemplate>
                                        <li>
                                            <a href='<%# Eval("ImageUrl") %>' >
                                                <img src='<%# Eval("ImageUrl") %>' class="image0" alt="" height="59" />
                                            </a>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="conbg">
                <!--------------------------------左邊選單開始---------------------------------------------------->
                <div class="left">
                    <div class="left1">
                        <p><span>YACHTS</span></p>
                        <ul>
                            <asp:Literal ID="LeftMenuHtml" runat="server"></asp:Literal>
                        </ul>
                    </div>
                </div>
                <!--------------------------------左邊選單結束---------------------------------------------------->
                <!--------------------------------右邊選單開始---------------------------------------------------->
                <div id="crumb1">
                    <a href="index.aspx">Home</a> >> <a href="#">Yachts</a> >> <a
                        href="Yachts_OverView.aspx"><span class="on1">
                            <asp:Label ID="LabLink" runat="server" Text="Tayana 37"></asp:Label>
                    </span></a></div>
                <div class="right">
                    <div class="right1">
                        <div class="title"> 
                            <asp:Literal ID="LiteralTtitleHtml" runat="server"></asp:Literal>
                        </div>
                        <!--------------------------------內容開始---------------------------------------------------->
                        <!--次選單-->
                        <div class="menu_y">
                            <ul>
                                <li class="menu_y00">YACHTS</li>
                                <asp:Literal ID="TopMenuLinkHtml" runat="server"></asp:Literal>
                            </ul>
                        </div>
                        <!--次選單-->
                        <div class="box1">
                            <asp:Literal ID="ContentHtml" runat="server"></asp:Literal>
                            &nbsp;
                        </div>
                        <div class="box3">
                            <h4><asp:Label ID="LabNumber" runat="server" Text="37"></asp:Label> DIMENSIONS</h4>
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
                        <asp:Literal ID="DownloadsHtml" runat="server"></asp:Literal>
                        <!--下載結束-->
                        <!--------------------------------內容結束------------------------------------------------------>
                    </div>
                </div>
                <!--------------------------------右邊選單結束---------------------------------------------------->
            </div>
            <!--------------------------------落款開始---------------------------------------------------->
            <div class="footer">
                <div class="footerp00">
                    <a href="http://www.tognews.com/" target="_blank">
                        <p><img src="images/tog.jpg" alt="TOG" /></p>
                    </a>
                    <p class="footerp001">© 1973-2012 Tayana Yachts, Inc. All Rights Reserved</p>
                </div>
                <div class="footer01">
                    <span>No. 60, Hai Chien Road, Chung Men Li, Lin Yuan District, Kaohsiung City, Taiwan, R.O.C.</span><br />
                    <span>TEL：+886(7)641-2721</span> <span>FAX：+886(7)642-3193</span>
                </div>
            </div>
            <!--------------------------------落款結束---------------------------------------------------->
        </div>
    </form>
</body>
</html>
