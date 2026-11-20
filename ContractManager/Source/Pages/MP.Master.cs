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

    private string connectionString;

    readonly SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);

    private bool canAccess(string userType, string targetPermission) {
      string[] allowedPermissions;

      switch (userType) {
        case "Solicitante":
          allowedPermissions = new[] { "Request" };
          break;

        case "Autorizador":
          allowedPermissions = new[] { "Catalog", "Report", "Revision" };
          break;

        case "Administrador":
          allowedPermissions = new[] { "Request", "Catalog", "Report", "Revision" };
          break;

        default:
          allowedPermissions = Array.Empty<string>();
          break;
      }

      return allowedPermissions.Contains(targetPermission);
    }
    protected void Page_Load(object sender, EventArgs e) {
      if (Session["username"] != null) {
        string loggedUser = Session["username"].ToString();

        string server = "(localdb)\\Servercito";

        string database = "ContractManager";

        SqlParameter[] loginParams = new SqlParameter[] {
          new SqlParameter("@P_Username", loggedUser)
        };

        DataAccess data = new DataAccess();

        DataTable profile = data.GetProfile("SP_GetProfile", loginParams);

        lblUser.Text = profile.Rows[0]["name"].ToString();

        string userType = profile.Rows[0]["userType"].ToString();

        pnlRevision.Visible = canAccess(userType, "Revision");
        pnlCatalog.Visible = canAccess(userType, "Catalog");
        pnlReport.Visible = canAccess(userType, "Report");
        pnlRequest.Visible = canAccess(userType, "Request");
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