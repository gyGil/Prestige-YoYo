<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/template.Master" CodeBehind="defectPareto.aspx.cs" Inherits="PrestigeYoYo.defectPareto" %>

<asp:Content ID="cIDIntroContent" ContentPlaceHolderID="IntroContent" runat="server">
      <p> Pareto Diagram for Defect Categories </p>
</asp:Content>

<asp:Content ID="cIDMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Button ID="btnPareto" runat="server" OnClick="btnPareto_Click" Text="Get Data" />
    </div>
    <br /><br /><br />
    <div>
        <br />
        <asp:Chart ID="ctPareto"  Width="1000px" Height="300px" Visible="false" runat="server"  >
            <BorderSkin BackColor="Transparent" PageColor="Transparent" SkinStyle="Emboss"/>
            <Titles>
                <asp:Title Font="Times New Roman, 12pt, style=Bold, Italic" Name="Title1" Text="Defect Categories"/>
            </Titles>
            <Series>
                <asp:Series Name="Each Defect"  XValueMember="Defect" YValueMembers="Numbers"  IsVisibleInLegend="true"  Color="255, 255,128,112" IsValueShownAsLabel="true">
                </asp:Series>
            </Series>

            <ChartAreas>
 
                <asp:ChartArea Name="ChartArea1">

                    <AxisY2 LineColor="64, 64, 64, 64">
 
                        <MajorGrid LineColor="64, 64, 64, 64" />
 
                    </AxisY2>
 
                    <AxisY LineColor="64, 64, 64, 64">
 

 
                        <MajorGrid LineColor="64, 64, 64, 64" />
 
                    </AxisY>
 
                    <AxisX LineColor="64, 64, 64, 64">
 

 
                        <MajorGrid LineColor="64, 64, 64, 64" />
 
                    </AxisX>

                    </asp:ChartArea>
 
              </ChartAreas>

              <Legends>
                    <asp:Legend></asp:Legend>
              </Legends>
  

        </asp:Chart>
    </div>
</asp:Content>
