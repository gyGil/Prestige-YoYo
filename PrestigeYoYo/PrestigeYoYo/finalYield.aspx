<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/template.Master" CodeBehind="finalYield.aspx.cs" Inherits="PrestigeYoYo.finalYield" %>

<asp:Content ID="cIDIntroContent" ContentPlaceHolderID="IntroContent" runat="server">
      <p> Final Yield </p>
</asp:Content>

<asp:Content ID="cIDMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Button ID="btnFinalYield" runat="server" OnClick="btnFinalYield_Click" Text="Get Data" />
    </div>
    <div>
        <br />
        <asp:Label ID="lbColour" runat="server" Text="Colour : " Font-Size="Small"></asp:Label>

        <asp:DropDownList id="ddlColor" Visible="true" runat="server" >
            <asp:ListItem Text="Pastel" Value="Pastel"></asp:ListItem>
            <asp:ListItem Text="Mono" Value="Mono"></asp:ListItem>
        </asp:DropDownList>

        <asp:Label ID="lbStation" Visible="false" runat="server" Text="Station : " Font-Size="Small"></asp:Label>

        <asp:DropDownList id="ddlStation" Visible="false" DataTextField="stationName" DataValueField="stationNum" runat="server" >
        </asp:DropDownList>

         <br /><br />
        <asp:Chart ID="ctFinalYield" Width="500px" Visible="false" runat="server" >
            <BorderSkin BackColor="Transparent" PageColor="Transparent" SkinStyle="Emboss"/>
            <Titles>
                <asp:Title Font="Times New Roman, 12pt, style=Bold, Italic" Name="Title1" Text="Reject numbers At Each Station"/>
            </Titles>
            <Series>
                <asp:Series ChartType="Pie" Name="Series1"  XValueMember="x" YValueMembers="y" IsVisibleInLegend="true"  Color="255, 255,128,112" IsValueShownAsLabel="true">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1">
                    <AxisX LineColor="Gray">
                        <MajorGrid LineColor="Red" />
                    </AxisX>
                    <AxisY LineColor="Gray">
                        <MajorGrid LineColor="Gray" />
                    </AxisY>
                    <Area3DStyle Enable3D="true" LightStyle="Realistic" />
                </asp:ChartArea>
            </ChartAreas>
            <Legends>
                <asp:Legend></asp:Legend>
            </Legends>
        </asp:Chart>

        <asp:GridView ID="gvFinalYield" HeaderStyle-HorizontalAlign="Center" Visible="false" runat="server">
            
        </asp:GridView>

        <br />
    </div>
</asp:Content>
