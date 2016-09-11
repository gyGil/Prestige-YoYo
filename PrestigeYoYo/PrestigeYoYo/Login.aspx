<%@ Page  Language="C#" MasterPageFile="~/template.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PrestigeYoYo.pageLogin" %>

<asp:Content ID="cIDContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>LOG IN</h1>
    <br /><br />
    <div>
        
        <asp:Label ID="lbID" runat="server" Font-Size="Medium" Text="ID: "></asp:Label>
        <asp:TextBox ID="tbID" runat="server"></asp:TextBox>
        <br />
        <asp:RequiredFieldValidator ID="rfvID" runat="server" ControlToValidate="tbID" ErrorMessage="ID is required." ForeColor="#3399FF" Font-Size="Small"></asp:RequiredFieldValidator>
        <br /><br />
        <asp:Label ID="lbPw" runat="server" Font-Size="Medium" Text="PASSWORD: "></asp:Label>
        <asp:TextBox ID="tbPw" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:RequiredFieldValidator ID="rfvPw" runat="server" ControlToValidate="tbPw" ErrorMessage="Password is required." ForeColor="#3399FF" Font-Size="Small"></asp:RequiredFieldValidator>
        <br /><br />
        <asp:Label ID="lbErrorMsg" runat="server" ForeColor="Red" Font-Size="Medium"></asp:Label>
        <br /><br /><br />
        <asp:Button ID="btnSignIn" runat="server" Font-Bold="True" Text="Sign In" Height="30px" Width="80px" OnClick="btnSignIn_Click" />
        <br /> <br /><br /><br /><br /><br /><br />
    </div>
</asp:Content>
