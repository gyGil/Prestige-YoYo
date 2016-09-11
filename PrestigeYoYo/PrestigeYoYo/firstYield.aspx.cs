using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DALlib;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace PrestigeYoYo
{
    public partial class firstYield : System.Web.UI.Page
    {
        SqlConnection conn;

        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectionStringSettings mySettings = ConfigurationManager.ConnectionStrings["YoYoConn"];
            
            if (mySettings != null)
                this.conn = new SqlConnection(mySettings.ConnectionString);

            if(!Page.IsPostBack)
            {
                this.ddlStation.DataSource = this.PopulateFilterDDLs();
                this.ddlStation.DataBind();
                this.ddlStation.Visible = true;
                this.lbStation.Visible = true;
                this.SetColor("Pastel");
            }           
        }

        /// <summary>
        /// It get the data from database and display with charts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFirstYield_Click(object sender, EventArgs e)
        {
            // Create Pie Chart/ table
            this.ctFirstYield.Visible = true;   
             this.gvFirstYield.Visible = true;

            DAL dal = new DAL();
            Dictionary<string, int> dic = dal.QueryFirstYieldTotal(this.conn);

            DataTable dtDefect = this.CreatePieChartTable();
            DataTable dtTotal = this.CreateTotalTable();


            int defectNumForStation = 0;
            int passNumForStation = 0;
            for(int i = 1; i <= 3; ++i)
            {
                // Add rows for Pie chart
                DataRow drDefect = dtDefect.NewRow();
                drDefect[0] = "Station " + i;
                int defectNum = -1;
                dic.TryGetValue("Station " + i, out defectNum);
                drDefect[1] = defectNum;
                dtDefect.Rows.Add(drDefect);

                // Add rows for Table
                DataRow drTotal = dtTotal.NewRow();
                drTotal[0] = "Station " + i;
                int total = -1;
                dic.TryGetValue("Total " + i, out total);
                float yieldRate = (float)(total - defectNum) / total * 100;
                drTotal[1] = yieldRate.ToString() + " %";
                dtTotal.Rows.Add(drTotal);

                if(this.ddlStation.Text == i.ToString())
                {
                    defectNumForStation = defectNum;
                    passNumForStation = total - defectNum;
                }
            }

            // Bind with chart depends on dropdown text
            if(this.ddlStation.Text == "4")
            {
                // data bind with chart
                this.ctFirstYield.DataSource = dtDefect;
                this.ctFirstYield.DataBind();
            }
            else if (this.ddlStation.Text == "1" ||
                     this.ddlStation.Text == "2" ||
                     this.ddlStation.Text == "3")
            {
                DataTable dtEachStation = this.CreatePieChartTable();
                DataRow drEachStation = dtEachStation.NewRow();
                drEachStation[0] = "Pass numbers";
                drEachStation[1] = passNumForStation;
                dtEachStation.Rows.Add(drEachStation);
                drEachStation = dtEachStation.NewRow();
                drEachStation[0] = "Reject/Rework numbers";
                drEachStation[1] = defectNumForStation;
                dtEachStation.Rows.Add(drEachStation);

                this.ctFirstYield.DataSource = dtEachStation;
                this.ctFirstYield.DataBind();
            }            

            this.gvFirstYield.DataSource = dtTotal;
            this.gvFirstYield.DataBind();

            if (this.ddlColor.Text == "Mono")
                this.SetColor("Mono");
            else
                this.SetColor("Pastel");
        }

        /// <summary>
        /// Populate dropdownlist for station type
        /// </summary>
        /// <returns></returns>
        private DataTable PopulateFilterDDLs()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("stationNum");
            dt.Columns.Add("stationName");

            for (int i = 1; i <= 4; ++i)
            {
                DataRow dr = dt.NewRow();
                dr[0] = i;
                if(i < 4)
                    dr[1] = "Station " + i;
                else
                    dr[1] = "Total";

                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// Create datatable for Pie chart
        /// </summary>
        /// <returns></returns>
        private DataTable CreatePieChartTable()
        {
            DataTable tempTbl = new DataTable();
            tempTbl.Columns.Add("x");
            tempTbl.Columns.Add("y");
            return tempTbl;
        }

        /// <summary>
        /// Create datatable for Table.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateTotalTable()
        {
            DataTable tempTbl = new DataTable();
            tempTbl.Columns.Add("Station");
            tempTbl.Columns.Add("Yield Rate");
            return tempTbl;
        }

        /// <summary>
        /// set the color for Pie chart
        /// </summary>
        /// <param name="type"></param>
        private void SetColor(string type)
        {
            Color[] myPalette = new Color[3];
            if(type == "Mono")
            {
                myPalette[0] = Color.FromKnownColor(KnownColor.DarkGray);
                myPalette[1] = Color.FromKnownColor(KnownColor.WhiteSmoke);
                myPalette[2] = Color.FromKnownColor(KnownColor.Gray);
            }
            else
            {
                myPalette[0] = Color.FromKnownColor(KnownColor.Yellow);
                myPalette[1] = Color.FromKnownColor(KnownColor.SkyBlue);
                myPalette[2] = Color.FromKnownColor(KnownColor.OrangeRed);
            }

            this.ctFirstYield.Palette = ChartColorPalette.None;
            this.ctFirstYield.PaletteCustomColors = myPalette;
        }
    }
}