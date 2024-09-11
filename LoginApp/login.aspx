<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="LoginApp.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxLabel ID="Usernamelbl" runat="server" Text="username"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="Usernametxt" runat="server" Width="170px" ></dx:ASPxTextBox>
            <br />
            <dx:ASPxLabel ID="Passwordlbl" runat="server" Text="password"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="Passwordtxt" runat="server" Width="170px" Password="True"></dx:ASPxTextBox>
            <dx:ASPxCheckBox ID="CBbtn" runat="server"></dx:ASPxCheckBox>
            <br />
            <dx:ASPxButton ID="Loginbtn" runat="server" Text="ASPxButton" OnClick="Loginbtn_Click" AutoPostBack="False"></dx:ASPxButton>
        </div>
    </form>
</body>
</html>
