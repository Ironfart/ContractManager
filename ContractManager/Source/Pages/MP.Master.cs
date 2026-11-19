using ContractManager.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ContractManager.Source.Pages {
  public partial class MP : System.Web.UI.MasterPage {

    readonly SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e) {
      if (Session["username"] != null) {
        string loggedUser = Session["username"].ToString();
  

        SqlParameter[] loginParams = new SqlParameter[] {
          new SqlParameter("@P_Username", loggedUser)
        };

        DataAccess data = new DataAccess();

        (User profile, string msg) = data.GetProfile("SP_GetProfile", loginParams);

        /*
        using (conn) {
          using (SqlCommand cmd = new SqlCommand("SP_GetProfile", conn)) {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@P_Username", SqlDbType.VarChar).Value = loggedUser;

            try {
              conn.Open();
              SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
              dataReader.Read();
              lblUser.Text = dataReader["name"].ToString();
            } catch (Exception ex) {
              System.Diagnostics.Debug.WriteLine("err " + ex.Message);
            }
          }
        }
        */
      } else {
        Response.Redirect("/Source/Pages/Login/login.aspx");
      }
    }

    protected void logout(object sender, EventArgs e) {
      Session.Remove("username");
      Response.Redirect("/Source/Pages/Login/login.aspx");
    }
  }
}