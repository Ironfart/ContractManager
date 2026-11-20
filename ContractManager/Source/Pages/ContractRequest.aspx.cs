using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ContractManager.Source.Pages {
  public partial class ContractRequest : System.Web.UI.Page {

    private readonly string conn = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
    private const string CarpetaPdf = "~/Uploads/Contratos/";
    private const int TamanoMaximoBytes = 10 * 1024 * 1024; // 10 MB
    protected void Page_Load(object sender, EventArgs e) {
      if (!IsPostBack) {
        CargarProveedores();
        CargarSolicitudes();
      }
    }

    private void CargarProveedores() {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getProvidersSummary", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        con.Open();

        using (SqlDataReader reader = cmd.ExecuteReader()) {
          ddlProveedor.Items.Clear();
          ddlProveedor.Items.Add(new ListItem("-- Seleccione un proveedor --", ""));
          ddlProveedor.DataSource = reader;
          ddlProveedor.DataTextField = "TextoMostrar";
          ddlProveedor.DataValueField = "id";
          ddlProveedor.DataBind();
        }
      }
    }
    private void CargarSolicitudes() {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getContractRequests", con)) {
        cmd.CommandType = CommandType.StoredProcedure;

        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        gvContratos.DataSource = dt;
        gvContratos.DataBind();
      }
    }

    protected void btnGuardar_Click(object sender, EventArgs e) {
      if (!Page.IsValid) return;

      if (!fuArchivoPdf.HasFile) {
        MostrarMensaje("Debe seleccionar un archivo PDF.", esError: true);
        return;
      }

      string extension = Path.GetExtension(fuArchivoPdf.FileName).ToLowerInvariant();
      if (extension != ".pdf") {
        MostrarMensaje("Solo se admiten archivos con extensión .pdf.", esError: true);
        return;
      }

      if (fuArchivoPdf.PostedFile.ContentLength > TamanoMaximoBytes) {
        MostrarMensaje("El archivo PDF no debe superar los 10 MB.", esError: true);
        return;
      }

      int idProveedor = int.Parse(ddlProveedor.SelectedValue);
      string descripcion = txtDescripcion.Text.Trim();
      decimal monto = decimal.Parse(txtMonto.Text.Trim(), CultureInfo.InvariantCulture);
      string moneda = ddlMoneda.SelectedValue;
      DateTime fechaInicio = DateTime.Parse(txtFechaInicio.Text.Trim());
      DateTime fechaFin = DateTime.Parse(txtFechaFin.Text.Trim());

      if (fechaFin < fechaInicio) {
        MostrarMensaje("La fecha de fin no puede ser menor que la fecha de inicio.", esError: true);
        return;
      }

      int idContrato = int.Parse(hfIdContrato.Value);

      if (idContrato > 0) {
        string estatusActual = ObtenerEstatusActual(idContrato);
        if (!string.Equals(estatusActual, "Pendiente", StringComparison.OrdinalIgnoreCase)) {
          LimpiarFormulario();
          CargarSolicitudes();
          MostrarMensaje($"La solicitud ya fue '{estatusActual}' y los cambios no están permitidos.", esError: true);
          return;
        }
      }

      string nombreOriginal = Path.GetFileName(fuArchivoPdf.FileName);
      string nombreSeguro = Guid.NewGuid().ToString("N") + ".pdf";
      string rutaFisicaCarpeta = Server.MapPath(CarpetaPdf);

      if (!Directory.Exists(rutaFisicaCarpeta)) {
        Directory.CreateDirectory(rutaFisicaCarpeta);
      }

      string rutaFisicaCompleta = Path.Combine(rutaFisicaCarpeta, nombreSeguro);
      string rutaRelativa = CarpetaPdf + nombreSeguro;

      fuArchivoPdf.SaveAs(rutaFisicaCompleta);

      try {
        using (SqlConnection con = new SqlConnection(conn))
        using (SqlCommand cmd = new SqlCommand("sp_createContractRequest", con)) {
          cmd.CommandType = CommandType.StoredProcedure;

          cmd.Parameters.Add("@p_idProvider", SqlDbType.Int).Value = idProveedor;
          cmd.Parameters.Add("@p_jobDescription", SqlDbType.VarChar, 500).Value = descripcion;

          var paramMonto = cmd.Parameters.Add("@p_contractCost", SqlDbType.Decimal);
          paramMonto.Precision = 18;
          paramMonto.Scale = 2;
          paramMonto.Value = monto;

          cmd.Parameters.Add("@p_currency", SqlDbType.VarChar, 10).Value = moneda;
          cmd.Parameters.Add("@p_beginDate", SqlDbType.Date).Value = fechaInicio;
          cmd.Parameters.Add("@p_endDate", SqlDbType.Date).Value = fechaFin;
          cmd.Parameters.Add("@p_originalFilename", SqlDbType.VarChar, 255).Value = nombreOriginal;
          cmd.Parameters.Add("@p_filePath", SqlDbType.VarChar, 500).Value = rutaRelativa;

          con.Open();
          cmd.ExecuteNonQuery();
        }

        MostrarMensaje("Solicitud de contrato registrada correctamente.", esError: false);
        LimpiarFormulario();
        CargarSolicitudes();
      } catch (Exception ex) {
        // Si falla la BD, eliminamos el archivo guardado para no dejar huérfanos
        if (File.Exists(rutaFisicaCompleta)) {
          File.Delete(rutaFisicaCompleta);
        }
        MostrarMensaje("Error al guardar la solicitud: " + ex.Message, esError: true);
      }
    }

    protected void gvContratos_RowDataBound(object sender, GridViewRowEventArgs e) {
      if (e.Row.RowType == DataControlRowType.DataRow) {
        DataRowView fila = (DataRowView)e.Row.DataItem;
        string estatus = fila["Estatus"].ToString();

        Button btnEditar = (Button)e.Row.FindControl("btnEditar");
        Label lblBloqueado = (Label)e.Row.FindControl("lblBloqueado");

        // Solo los contratos Pendientes pueden editarse; Autorizado y Rechazado quedan bloqueados
        bool esEditable = string.Equals(estatus, "Pendiente", StringComparison.OrdinalIgnoreCase);

        if (btnEditar != null) btnEditar.Visible = esEditable;
        if (lblBloqueado != null) lblBloqueado.Visible = !esEditable;
      }
    }

    protected void gvContratos_RowCommand(object sender, GridViewCommandEventArgs e) {
      if (e.CommandName == "Editar") {
        int idContrato = Convert.ToInt32(e.CommandArgument);
        CargarContratoParaEditar(idContrato);
      }
    }

    private void CargarContratoParaEditar(int idContrato) {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getRequestContractById", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@p_id", SqlDbType.Int).Value = idContrato;
        con.Open();

        using (SqlDataReader reader = cmd.ExecuteReader()) {
          if (reader.Read()) {
            string estatus = reader["Estatus"].ToString();

            // Doble validación de seguridad: si ya no está Pendiente, bloqueamos el acceso
            if (!string.Equals(estatus, "Pendiente", StringComparison.OrdinalIgnoreCase)) {
              LimpiarFormulario();
              MostrarMensaje($"Este contrato está '{estatus}' y no puede modificarse.", esError: true);
              return;
            }

            hfIdContrato.Value = reader["id"].ToString();
            ddlProveedor.SelectedValue = reader["id_provider"].ToString();
            txtDescripcion.Text = reader["job_description"].ToString();
            txtMonto.Text = Convert.ToDecimal(reader["contract_cost"]).ToString("0.##", CultureInfo.InvariantCulture);
            ddlMoneda.SelectedValue = reader["currency"].ToString();
            txtFechaInicio.Text = Convert.ToDateTime(reader["begin_date"]).ToString("yyyy-MM-dd");
            txtFechaFin.Text = Convert.ToDateTime(reader["end_date"]).ToString("yyyy-MM-dd");

            btnGuardar.Text = "Actualizar Solicitud";
            lblMensaje.Text = "";
          }
        }
      }
    }

    private string ObtenerEstatusActual(int idContrato) {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getContractRequestStatus", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@p_id", SqlDbType.Int).Value = idContrato;
        con.Open();
        object resultado = cmd.ExecuteScalar();
        return resultado == null || resultado == DBNull.Value ? string.Empty : resultado.ToString();
      }
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

    protected void btnLimpiar_Click(object sender, EventArgs e) {
      LimpiarFormulario();
      lblMensaje.Text = "";
    }

    private void LimpiarFormulario() {
      ddlProveedor.SelectedIndex = 0;
      txtDescripcion.Text = "";
      txtMonto.Text = "";
      ddlMoneda.SelectedValue = "MXN";
      txtFechaInicio.Text = "";
      txtFechaFin.Text = "";
    }

    private void MostrarMensaje(string mensaje, bool esError) {
      lblMensaje.Text = mensaje;
      lblMensaje.CssClass = esError ? "fw-semibold text-danger" : "fw-semibold text-success";
    }
  }
}