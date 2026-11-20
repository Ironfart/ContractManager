using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ContractManager.Source.Pages {
  public partial class revision : System.Web.UI.Page {
    private readonly string conn = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e) {
      if (!IsPostBack) {
        CargarSolicitudes();
      }
    }
    protected void ddlFiltroEstatus_SelectedIndexChanged(object sender, EventArgs e) {
      lblMensaje.Text = "";
      CargarSolicitudes();
    }

    private void CargarSolicitudes() {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getContractRequestsByStatus", con)) {
        cmd.CommandType = CommandType.StoredProcedure;

        string estatusFiltro = ddlFiltroEstatus.SelectedValue;
        cmd.Parameters.Add("@p_status", SqlDbType.VarChar, 20).Value =
            string.IsNullOrWhiteSpace(estatusFiltro) ? (object)DBNull.Value : estatusFiltro;

        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        gvRevision.DataSource = dt;
        gvRevision.DataBind();
      }
    }

    protected void gvRevision_RowDataBound(object sender, GridViewRowEventArgs e) {
      if (e.Row.RowType == DataControlRowType.DataRow) {
        DataRowView fila = (DataRowView)e.Row.DataItem;
        string estatus = fila["Estatus"].ToString();

        Button btnAutorizar = (Button)e.Row.FindControl("btnAutorizar");
        Button btnRechazar = (Button)e.Row.FindControl("btnRechazar");

        bool esPendiente = string.Equals(estatus, "Pendiente", StringComparison.OrdinalIgnoreCase);

        if (btnAutorizar != null) btnAutorizar.Visible = esPendiente;
        if (btnRechazar != null) btnRechazar.Visible = esPendiente;
      }
    }

    protected void gvRevision_RowCommand(object sender, GridViewCommandEventArgs e) {
      if (e.CommandName == "Autorizar") {
        int idContrato = Convert.ToInt32(e.CommandArgument);
        AutorizarContrato(idContrato);
      } else if (e.CommandName == "Rechazar") {
        int idContrato = Convert.ToInt32(e.CommandArgument);
        hfIdContratoRechazo.Value = idContrato.ToString();
        txtMotivoRechazo.Text = "";

        ScriptManager.RegisterStartupScript(
            this,
            this.GetType(),
            "MostrarModalRechazo",
            "abrirModalRechazo();",
            true
        );
      }
    }

    private void AutorizarContrato(int id) {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_authorizeContractRequest", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@p_id", SqlDbType.Int).Value = id;

        con.Open();
        cmd.ExecuteNonQuery();
      }

      MostrarMensaje("Contrato autorizado correctamente.", esError: false);
      CargarSolicitudes();
    }

    protected void btnConfirmarRechazo_Click(object sender, EventArgs e) {
      if (!Page.IsValid) {
        ScriptManager.RegisterStartupScript(
            this,
            this.GetType(),
            "ReabrirModalRechazo",
            "abrirModalRechazo();",
            true
        );
        return;
      }

      int idContrato = int.Parse(hfIdContratoRechazo.Value);
      string motivo = txtMotivoRechazo.Text.Trim();

      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_rejectContractRequest", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@p_id", SqlDbType.Int).Value = idContrato;
        cmd.Parameters.Add("@p_rejectReason", SqlDbType.VarChar, 500).Value = motivo;

        con.Open();
        cmd.ExecuteNonQuery();
      }

      hfIdContratoRechazo.Value = "0";
      txtMotivoRechazo.Text = "";

      MostrarMensaje("Contrato rechazado con el motivo registrado.", esError: false);
      CargarSolicitudes();
    }

    protected string ObtenerClaseBadge(string estatus) {
      switch (estatus) {
        case "Autorizado":
          return "badge bg-success";
        case "Rechazado":
          return "badge bg-danger";
        default:
          return "badge bg-warning text-dark";
      }
    }

    private void MostrarMensaje(string mensaje, bool esError) {
      lblMensaje.Text = mensaje;
      lblMensaje.CssClass = esError ? "fw-semibold text-danger" : "fw-semibold text-success";
    }
  }
}