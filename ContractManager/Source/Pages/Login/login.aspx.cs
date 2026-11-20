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