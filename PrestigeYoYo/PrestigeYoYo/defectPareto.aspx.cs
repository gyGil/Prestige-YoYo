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
using System.ComponentModel;
using System.Web.UI.DataVisualization.Charting;

namespace PrestigeYoYo
{
    public partial class defectPareto : System.Web.UI.Page
    {
        SqlConnection conn;

        /// <summary>
        /// create connection string with database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectionStringSettings mySettings = ConfigurationManager.ConnectionStrings["YoYoConn"];

            if (mySettings != null)
                this.conn = new SqlConnection(mySettings.ConnectionString);
        }

        /// <summary>
        /// Create pareto chart by data reading from database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPareto_Click(object sender, EventArgs e)
        {
            this.ctPareto.Visible = true;   // make diagram visible

            DAL dal = new DAL();
            Dictionary<int, int> dic = dal.QueryDefectCategories(this.conn);
            string[] defectCate = {"Inconsistent Thickness","Pitting","Warping",
                                  "Primer Defect","Drip Mark","Final Coat Flaw",
                                  "Broken Shell","Broken Axle","Tangled String" };
            const int NUM_DEFECT = 9;

            DataTable dtBar = CreateDefectTable();
            DataTable dtLine = CreateDefectTable();
            int lineVal = 0;
            int col_minVal = int.MaxValue;

            for (int i = 0; i < NUM_DEFECT; ++i)
            {
                DataRow dr1 = dtBar.NewRow();
                dr1[0] = defectCate[i];
                int val = 0;
                dic.TryGetValue(i, out val);
                col_minVal = (val < col_minVal) ? val : col_minVal;
                dr1[1] = val;
                lineVal += val;
                dr1[2] = lineVal;
                dtBar.Rows.Add(dr1);
            }

            // bind with data
            this.MakeParetoChart("Accumulated Defect", "Defect", "Total");
            this.ctPareto.DataSource = dtBar;
            this.ctPareto.DataBind();
            this.ctPareto.ChartAreas["ChartArea1"].AxisY.Maximum = lineVal;
            this.ctPareto.ChartAreas["ChartArea1"].AxisY.Minimum = col_minVal/2;
        }

        /// <summary>
        /// Create datatable for final yield diagram.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateDefectTable()
        {
            DataTable tempTbl = new DataTable();
            tempTbl.Columns.Add("Defect");
            tempTbl.Columns.Add("Numbers");
            tempTbl.Columns.Add("Total");

            return tempTbl;
        }

        /// <summary>
        /// Add the second series to make pareto chart
        /// refer to http://www.skill-guru.com/blog/2009/11/29/advanced-features-of-net-chart-control/
        /// </summary>
        /// <param name="secondSeriesName"></param>
        /// <param name="xValsecSeriesName"></param>
        /// <param name="yValsecSeriesName"></param>
        private void MakeParetoChart(string secondSeriesName,string xValsecSeriesName, string yValsecSeriesName)
        {
            this.ctPareto.Series.Add(secondSeriesName);

            // add chart type and x, y axis name
            this.ctPareto.Series[secondSeriesName].ChartType = SeriesChartType.Line;    
            this.ctPareto.Series[secondSeriesName].XValueMember = xValsecSeriesName;
            this.ctPareto.Series[secondSeriesName].YValueMembers = yValsecSeriesName;
        }
    }
}