<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Manager_SignIn.aspx.cs" Inherits="TayanaYachtRe.Sys.Manager_SignIn" ViewStateEncryptionMode="Always" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Manager Sign In</title>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0, minimal-ui"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="description" content="CodedThemes"/>
    <meta name="keywords" content=" Admin , Responsive, Landing, Bootstrap, App, Template, Mobile, iOS, Android, apple, creative app"/>
    <meta name="author" content="CodedThemes"/>
    <!-- Favicon icon -->
    <link rel="icon" href="assets/images/logo.ico" type="image/x-icon"/>
    <!-- Google font-->
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:400,600,800" rel="stylesheet"/>
    <!-- Required Fremwork -->
    <link rel="stylesheet" type="text/css" href="assets/css/bootstrap/css/bootstrap.min.css"/>
    <!-- themify-icons line icon -->
    <link rel="stylesheet" type="text/css" href="assets/icon/themify-icons/themify-icons.css"/>
    <!-- ico font -->
    <link rel="stylesheet" type="text/css" href="assets/icon/icofont/css/icofont.css"/>
    <!-- Style.css -->
    <link rel="stylesheet" type="text/css" href="assets/css/style.css"/>
    <style type="text/css">
        @keyframes blink {
            0% {
              opacity: .2;
            }
            20% {
              opacity: 1;
            }
            100% {
              opacity: .2;
            }
        }

        .saving span {
            animation-name: blink;
            animation-duration: 1.4s;
            animation-iteration-count: infinite;
            animation-fill-mode: both;
        }

        .saving span:nth-child(2) {
            animation-delay: .2s;
        }

        .saving span:nth-child(3) {
            animation-delay: .4s;
        }
    </style>

</head>
<body class="fix-menu">
    <form id="form1" runat="server" method="post">
        <!-- Pre-loader end -->
        <section class="login p-fixed d-flex text-center bg-primary common-img-bg">
            <!-- Container-fluid starts -->
            <div class="container">
                <div class="row">
                    <div class="col-sm-12">
                        <!-- Authentication card start -->
                        <div class="login-card card-block auth-body mr-auto ml-auto">
                            <div class="md-float-material">
                                <div class="text-center">
                                    <img id="logo" src="assets/images/auth/logo-dark.png" alt="logo.png" width="50%" />
                               </div>
                                <div id="sighin" class="auth-box">
                                    <div class="row m-b-20">
                                        <div class="col-md-12">
                                            <h3 class="text-left text-primary">Sign In To Backend</h3>
                                        </div>
                                    </div>
                                    <hr/>
                                    <div class="input-group">
                                        <asp:TextBox ID="TextBox1" runat="server" AutoCompleteType="Disabled" required="" aria-required="true" oninput="setCustomValidity('');" oninvalid="setCustomValidity('The account can't be blank!')" class="form-control" placeholder="Your Account"></asp:TextBox>
                                        <span class="md-line"></span>
                                    </div>
                                    <div class="input-group">
                                        <asp:TextBox ID="TextBox2" runat="server" AutoCompleteType="Disabled" TextMode="Password" required="" aria-required="true" oninput="setCustomValidity('');" oninvalid="setCustomValidity('The password can't be blank!')" class="form-control" placeholder="Password"></asp:TextBox>
                                        <span class="md-line"></span>
                                    </div>
                                    <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="Red" Visible="False"></asp:Label>
                                    <div class="row m-t-30">
                                        <div class="col-md-12">
                                            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Sign in" Font-Bold="True" class="btn btn-primary btn-md btn-block waves-effect text-center m-b-20" OnClientClick="ShowProgressBar();"/>
                                        </div>
                                        <div class="col-md-12">
                                            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Back to index" Font-Bold="True" class="btn btn btn-info btn-md btn-block waves-effect text-center m-b-20" UseSubmitBehavior="False"/>
                                        </div>
                                    </div>
                                    <hr/>
                                    <div class="row">
                                        <div class="col-md-10">
                                            <p class="text-inverse text-left m-b-0">Thank you and enjoy our website.</p>
                                            <p class="text-inverse text-left"><b>Your Authentication Team!</b></p>
                                        </div>
                                        <div class="col-md-2">
                                            <img src="assets/images/auth/Logo-small-bottom.png" alt="small-logo.png"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- end of form -->
                        </div>
                        <!-- Authentication card end -->
                    </div>
                    <!-- end of col-sm-12 -->
                </div>
                <!-- end of row -->
            </div>
            <!-- end of container-fluid -->
        </section>
        <!-- Required Jquery -->
        <script type="text/javascript" src="assets/js/jquery/jquery.min.js"></script>
        <script type="text/javascript" src="assets/js/jquery-ui/jquery-ui.min.js"></script>
        <script type="text/javascript" src="assets/js/popper.js/popper.min.js"></script>
        <script type="text/javascript" src="assets/js/bootstrap/js/bootstrap.min.js"></script>
        <!-- jquery slimscroll js -->
        <script type="text/javascript" src="assets/js/jquery-slimscroll/jquery.slimscroll.js"></script>
        <!-- modernizr js -->
        <script type="text/javascript" src="assets/js/modernizr/modernizr.js"></script>
        <script type="text/javascript" src="assets/js/modernizr/css-scrollbars.js"></script>
        <script type="text/javascript" src="assets/js/common-pages.js"></script>

        <div id="divProgress" style="text-align:center; display: none; position: fixed; top: 50%;  left: 50%;" >
            <asp:Image ID="imgLoading" runat="server" ImageUrl="assets/images/godz.gif" />
            <br />
            <h5 color="#1B3563" size="2px" class="saving">Verifying  Your  Account <span> . </span><span> . </span><span> . </span></h5>
        </div>
        <div id="divMaskFrame" style="background-color: #F2F4F7; display: none; left: 0px; position: absolute; top: 0px;">
        </div>

        <script type="text/javascript">
            // 顯示讀取遮罩
            function ShowProgressBar() {
                displayProgress();
                displayMaskFrame();
            }
            // 隱藏讀取遮罩
            function HideProgressBar() {
                var progress = $('#divProgress');
                var maskFrame = $("#divMaskFrame");
                progress.hide();
                maskFrame.hide();
            }
            // 顯示讀取畫面
            function displayProgress() {
                var w = $(document).width();
                var h = $(window).height();
                var progress = $('#divProgress');
                progress.css({ "z-index": 999999, "top": (h / 2) - (progress.height() / 2), "left": (w / 2) - (progress.width() / 2) });
                progress.show();
            }
            // 顯示遮罩畫面
            function displayMaskFrame() {
                var w = $(window).width();
                var h = $(document).height();
                var maskFrame = $("#divMaskFrame");
                maskFrame.css({ "z-index": 999998, "opacity": 0.7, "width": w, "height": h });
                maskFrame.show();
            }
        </script>

    </form>
</body>
</html>
