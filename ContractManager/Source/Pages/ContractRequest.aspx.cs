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
          cmd.Parameters.Add("@p_contractCost", SqlDbType.Decimal).Value = monto;

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