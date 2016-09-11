<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/template.Master" CodeBehind="addSchedule.aspx.cs" Inherits="PrestigeYoYo.addSchedule" %>

<asp:Content ID="cIDIntroContent" ContentPlaceHolderID="IntroContent" runat="server">
      <p> Only Admin is allowed to add schedule </p>
</asp:Content>

<asp:Content ID="cIDMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        
        <asp:Table ID="scheduleTable" Visible="false" runat="server">
        </asp:Table>
        
    </div>
    <br /><br /><br />
    <div>
       <h2> Add Schedules </h2>
        <br /><br />
        <label> (date example: 3/29/2016 10:05:04 AM) </label>
        <br /><br />
        <label> Start date: </label> <asp:TextBox runat="server" ID="tbStartDate"></asp:TextBox>
        <br /><br />
        <label> End date: </label> <asp:TextBox runat="server" ID="tbEndDate"></asp:TextBox>
        <br /><br />
        <label> Model: </label> 
        <asp:DropDownList id="ddlModel"  runat="server">
        </asp:DropDownList>
        <br /><br />
        <asp:Label ID="lbMsg" runat="server" ForeColor="Red" Font-Size="Medium"></asp:Label>
        <br /><br /><br />
        <br /><br />
        <asp:Button ID="btnEnter" runat="server" Text="Enter" Height="28px" Width="48px" OnClick="btnEnter_Click" />
        <br />

    </div>
</asp:Content>