using ContractManager.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ContractManager {
  public partial class login : System.Web.UI.Page {

    readonly SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e) {

    }

    protected void onLogin(object sender, EventArgs e) {
      string username, password;
      lblMessage.Text = "";

      System.Diagnostics.Debug.WriteLine("ASDASD");

      if (string.IsNullOrEmpty(txtUser.Text)) {
        lblMessage.Text = "Ingresa un usuario";
        return;
      }

      if (string.IsNullOrEmpty(txtPassword.Text)) {
        lblMessage.Text = "Ingresa una contraseña";
        return;
      }

      
      //System.Diagnostics.Debug.WriteLine("data " + dataReader.Read());
      
      /*
      using (this.conn) {
        using (SqlCommand cmd = new SqlCommand("SP_UserAuth", this.conn)) {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add("@P_Username", SqlDbType.VarChar).Value = txtUser.Text;
          cmd.Parameters.Add("@P_Password", SqlDbType.VarChar).Value = txtPassword.Text;

          conn.Open();
          SqlDataReader dataReader = cmd.ExecuteReader();

          if (dataReader.Read()) {
            Session["username"] = dataReader["username"].ToString();

            Response.Redirect("/Source/Pages/index.aspx");
          } else {
            lblMessage.Text = "Usuario o contraseña incorrectos"; 
          }
          conn.Close();
        }
      }*/

      
      username = txtUser.Text;
      password = txtPassword.Text;

      SqlParameter[] loginParams = new SqlParameter[] {
        new SqlParameter("@P_Username", username),
        new SqlParameter("@P_Password", password)
      };

      DataAccess data = new DataAccess();

      (int resultLogin, string msgLogin) = data.ExecuteScalar("SP_UserAuth", loginParams);

      if (resultLogin == 1) {
        Session["username"] = username;

        Response.Redirect("/Source/Pages/index.aspx");
      } else {
        lblMessage.Text = "Usuario o contraseña incorrectos " + msgLogin;
      }
    }
  }
}