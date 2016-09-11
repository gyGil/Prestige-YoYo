/// \file default.aspx.cs
///
/// \mainpage PROG3070 - Advanced SQL
///
/// \section intro Program Introduction
/// - This seivece the webpage which can retreive the yoyo information from database.
/// - It supports the authentication, and visualized the charts to provides information.
/// - For admin, we provides the service that admin can add new schedules
///
/// Major <b>default.aspx.cs</b>
/// \section version Current version of this Program
/// <ul>
/// <li>\author         Marcus Rankin, GeunYoung Gil</li>
/// <li>\references     N/A
/// <li>\version        1.00.00</li>
/// <li>\date           2016.04.14</li>
/// <li>\pre            N/A
/// <li>\warning        N/A
/// <li>\copyright      Marcus Rankin, GeunYoung Gil</li>
/// <ul>
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PrestigeYoYo
{
    public partial class _default : System.Web.UI.Page
    {
        SqlConnection conn;
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}