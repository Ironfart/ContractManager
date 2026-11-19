using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ContractManager {
  public partial class MainScreen : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
      if (Session["username"] != null) {
        lblMsg.Text = "Bienvenido " + Session["username"];
      } else {
        Response.Redirect("Login.aspx");
      }
    }

    protected void onLogout(object sender, EventArgs e) {
      Session["username"] = null;

      Response.Redirect("Login.aspx");
    }
  }
}