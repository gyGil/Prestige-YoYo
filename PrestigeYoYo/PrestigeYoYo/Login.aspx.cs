using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;
using System.Text;
using DALlib;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;


namespace PrestigeYoYo
{
    public partial class pageLogin : System.Web.UI.Page
    {
        SqlConnection conn;
        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectionStringSettings mySettings = ConfigurationManager.ConnectionStrings["YoYoConn"];
            if (mySettings != null)
            {
                this.conn = new SqlConnection(mySettings.ConnectionString);
            }

            this.lbErrorMsg.Text = "";

            // if it is still logged in, sign out and delete Authentication and session cookies
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                Session.Abandon();

                // clear authentication cookie
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie1);

                //clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
                HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
                cookie2.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie2);

                // Page.User.Identity.IsAuthenticated gets its value from Page.
                // User (obviously) which is unfortunately read-only and is not updated 
                // when you call FormsAuthentication.SignOut().
                HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            }      
    
            
        }

        /// <summary>
        /// Validate the user from database and give the right to access the pages(admin/user)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            DAL dal = new DAL();

            int result = dal.AuthenticateUser(this.tbID.Text, this.tbPw.Text, this.conn);

            if (result == 0)    // user
            {
                this.lbErrorMsg.Text = "user logged in.";
                // Redirects an authenticated user back to the originally requested URL 
                // or the default URL
                FormsAuthentication.RedirectFromLoginPage(this.tbID.Text, true);
            }
            else if (result == 1)    // admin
            {
                this.lbErrorMsg.Text = "admin logged in.";
                FormsAuthentication.RedirectFromLoginPage("admin", true);
            }
            else if (result == -1)
                this.lbErrorMsg.Text = "ID or Password was incorrect.";
            else
                this.lbErrorMsg.Text = "Connection error.";
        }
    }
}