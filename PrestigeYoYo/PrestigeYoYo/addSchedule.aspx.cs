using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using DALlib;
using System.Configuration;
using System.Data;


namespace PrestigeYoYo
{
    public partial class addSchedule : System.Web.UI.Page
    {
        SqlConnection conn;
        protected void Page_Load(object sender, EventArgs e)
        {            
            ConnectionStringSettings mySettings = ConfigurationManager.ConnectionStrings["YoYoConn"];

            if (mySettings != null)
                this.conn = new SqlConnection(mySettings.ConnectionString);

            if(!Page.IsPostBack)
            {
                DAL dal = new DAL();
                DataTable dt = dal.GetModelList(conn);

                // data bind with model number
                this.ddlModel.DataSource = dt;
                this.ddlModel.DataTextField = "model number";
                this.ddlModel.DataValueField = "model";
                this.ddlModel.DataBind();
            }    
        }

        /// <summary>
        /// It add the schedule with validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnter_Click(object sender, EventArgs e)
        {
            DAL dal = new DAL();
            Dictionary<int, string> dic = dal.AddSchedule(this.conn,this.tbStartDate.Text, this.tbEndDate.Text, this.ddlModel.SelectedItem.Text);

            if(dic.Count < 1)
            {
                this.lbMsg.ForeColor = System.Drawing.Color.Red;
                this.lbMsg.Text = "No response.";
            }
            else if(dic.ContainsKey(-1))
            {
                this.lbMsg.ForeColor = System.Drawing.Color.Red;
                this.lbMsg.Text = dic[-1];
            }
            else if (dic.ContainsKey(1))
            {
                this.lbMsg.ForeColor = System.Drawing.Color.Green;
                this.lbMsg.Text = dic[1];
            }
        }
    }
}