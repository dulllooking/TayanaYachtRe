<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="contact.aspx.cs" Inherits="TayanaYachtRe.Tayanahtml.contact" %>

<%@ Register Assembly="Recaptcha.Web" Namespace="Recaptcha.Web.UI.Controls" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Tayana | Tayana Yachts Official Website
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
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyDMMhz3zbCdt7YNP2AQB9dXbKESYEGfbfg&callback=initMap" type="text/javascript"></script>
    <script type="text/javascript" src='https://www.google.com/recaptcha/api.js?hl=en'></script>
</head>
<body>
    <form runat="server" method="post" name="aspnetForm" id="aspnetForm">
        <div class="contain">
            <div class="sub">
                <p>
                    <a href="index.aspx">Home</a>
                </p>
            </div>
            <!--------------------------------選單開始---------------------------------------------------->
            <div id="logol"><a href="index.aspx">
                <img src="images/logo001.gif" alt="Tayana" /></a></div>
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
            <div class="bannermasks">
                <img src="images/contact.jpg" alt="&quot;&quot;" width="967" height="371" /></div>
            <!--遮罩結束-->
            <!--<div id="buttom01"><a href="#"><img src="images/buttom01.gif" alt="next" /></a></div>-->
            <!--小圖開始-->
            <!--<div class="bannerimg">
            <ul>
            <li> <a href="#"><div class="on"><p class="bannerimg_p"><img  src="images/pit003.jpg" alt="&quot;&quot;" /></p></div></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" width="300" /></p>
            </a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            </ul>
            <ul>
            <li> <a class="on" href="#"><p class="bannerimg_p"><img  src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <p class="bannerimg_p"><a href="#"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            <li> <a href="#"><p class="bannerimg_p"><img src="images/pit003.jpg" alt="&quot;&quot;" /></p></a></li>
            </ul>
            </div>-->
            <!--小圖結束-->
            <!--<div id="buttom02"> <a href="#"><img src="images/buttom02.gif" alt="next" /></a></div>-->
            <!--------------------------------換圖開始---------------------------------------------------->
            <div class="banner">
                <ul>
                    <li>
                        <img src="images/newbanner.jpg" alt="Tayana Yachts" /></li>
                </ul>
            </div>
            <!--------------------------------換圖結束---------------------------------------------------->
            <div class="conbg">
                <!--------------------------------左邊選單開始---------------------------------------------------->
                <div class="left">
                    <div class="left1">
                        <p><span>CONTACT</span></p>
                        <ul>
                            <li><a href="#">contacts</a></li>
                        </ul>
                    </div>
                </div>
                <!--------------------------------左邊選單結束---------------------------------------------------->
                <!--------------------------------右邊選單開始---------------------------------------------------->
                <div id="crumb"><a href="index.aspx">Home</a> >> <a href="#"><span class="on1">Contact</span></a></div>
                <div class="right">
                    <div class="right1">
                        <div class="title"><span>Contact</span></div>
                        <!--------------------------------內容開始---------------------------------------------------->
                        <!--表單-->
                        <div class="from01">
                            <p>
                                Please Enter your contact information<span class="span01">*Required</span>
                            </p>
                            <br />
                            <table>
                                <tr>
                                    <td class="from01td01">Name :</td>
                                    <td><span>*</span><asp:TextBox runat="server" name="Name" type="text" ID="Name" class="{validate:{required:true, messages:{required:'Required'}}}" Style="width: 250px;" required="" aria-required="true" oninput="setCustomValidity('');" oninvalid="setCustomValidity('Required!')" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="from01td01">Email :</td>
                                    <td><span>*</span><asp:TextBox runat="server" name="Email" type="text" ID="Email" class="{validate:{required:true, email:true, messages:{required:'Required', email:'Please check the E-mail format is correct'}}}" Style="width: 250px;" required="" aria-required="true" oninput="setCustomValidity('');" oninvalid="setCustomValidity('Required!')" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="from01td01">Phone :</td>
                                    <td><span>*</span><asp:TextBox runat="server" name="Phone" type="text" ID="Phone" class="{validate:{required:true, messages:{required:'Required'}}}" Style="width: 250px;" required="" aria-required="true" oninput="setCustomValidity('');" oninvalid="setCustomValidity('Required!')" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="from01td01">Country :</td>
                                    <td><span>*</span>
                                        <asp:DropDownList name="Country" id="Country" runat="server" DataTextField="country" DataValueField="country"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2"><span>*</span>Brochure of interest *Which Brochure would you like to view?</td>
                                </tr>
                                <tr>
                                    <td class="from01td01">&nbsp;</td>
                                    <td>
                                        <asp:DropDownList name="Yachts" id="Yachts" runat="server" DataTextField="type" DataValueField="type"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="from01td01">Comments:</td>
                                    <td>
                                        <asp:TextBox runat="server" TextMode="MultiLine" name="Comments" Rows="2" cols="20" ID="Comments" Style="height: 150px; width: 330px;" MaxLength="500"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="from01td01">&nbsp;</td>
                                    <td class="f_right">
                                        <!-- Render recaptcha API script -->
                                        <cc1:RecaptchaApiScript ID="RecaptchaApiScript1" runat="server" />
                                        <!-- Render recaptcha widget -->
                                        <cc1:RecaptchaWidget ID="Recaptcha1" runat="server" />
                                        <asp:Label ID="lblMessage" runat="server" Visible="False" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="from01td01">&nbsp;</td>
                                    <td class="f_right">
                                        <asp:ImageButton runat="server" type="image" name="ImageButton1" id="ImageButton1" src="images/buttom03.gif" style="border-width: 0px;" Height="25px" OnClick="ImageButton1_Click"/>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <!--表單-->
                        <div class="box1">
                            <span class="span02">Contact with us</span><br />
                            Thanks for your enjoying our web site as an introduction to the Tayana world and our range of yachts.
                    As all the designs in our range are semi-custom built, we are glad to offer a personal service to all our
                    potential customers.
                    If you have any questions about our yachts or would like to take your interest a stage further, please
                    feel free to contact us.
                        </div>
                        <div class="list03">
                            <p>
                                <span>TAYANA HEAD OFFICE</span><br />
                                NO.60 Haichien Rd. Chungmen Village Linyuan Kaohsiung Hsien 832 Taiwan R.O.C<br />
                                tel. +886(7)641 2422<br />
                                fax. +886(7)642 3193<br />
                            </p>
                        </div>
                        <div class="box4">
                            <h4>Location</h4>
                            <p>
                                <iframe width="695" height="518" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="https://www.google.com/maps/d/u/0/embed?mid=19I5ZUTOO3BR2rHiKwEgRQptBRd_0CkfX"></iframe>
                            </p>
                        </div>
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
