using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ContractManager.Source.Pages {
  public partial class contractReport : System.Web.UI.Page {
    private readonly string conn = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e) {
      if (!IsPostBack) {
        CargarClientes();
        InicializarRangoFechas();
        GenerarReporte();
      }

    }

    private void CargarClientes() {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getProvidersSummary", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        con.Open();

        using (SqlDataReader reader = cmd.ExecuteReader()) {
          ddlCliente.Items.Clear();
          ddlCliente.Items.Add(new ListItem("Todos", "0"));
          ddlCliente.DataSource = reader;
          ddlCliente.DataTextField = "TextoMostrar";
          ddlCliente.DataValueField = "id";
          ddlCliente.DataBind();
        }
      }
    }

    private void InicializarRangoFechas() {
      DateTime fechaMinima = DateTime.Today;

      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getContractRequestsBeginingDate", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        con.Open();
        object resultado = cmd.ExecuteScalar();

        if (resultado != null && resultado != DBNull.Value) {
          fechaMinima = Convert.ToDateTime(resultado);
        }
      }

      txtFechaDesde.Text = fechaMinima.ToString("yyyy-MM-dd");
      txtFechaHasta.Text = DateTime.Today.ToString("yyyy-MM-dd");
    }

    private DataTable ObtenerDatosReporte() {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_contractRequestsReport", con)) {
        cmd.CommandType = CommandType.StoredProcedure;

        string estatus = ddlEstatus.SelectedValue;
        int idCliente = int.Parse(ddlCliente.SelectedValue);

        cmd.Parameters.Add("@p_Estatus", SqlDbType.VarChar, 20).Value =
            string.IsNullOrWhiteSpace(estatus) ? (object)DBNull.Value : estatus;

        cmd.Parameters.Add("@p_idProvider", SqlDbType.Int).Value =
            idCliente <= 0 ? (object)DBNull.Value : idCliente;

        DateTime fechaDesde;
        if (DateTime.TryParse(txtFechaDesde.Text, out fechaDesde))
          cmd.Parameters.Add("@p_sinceDate", SqlDbType.Date).Value = fechaDesde;
        else
          cmd.Parameters.Add("@p_sinceDate", SqlDbType.Date).Value = DBNull.Value;

        DateTime fechaHasta;
        if (DateTime.TryParse(txtFechaHasta.Text, out fechaHasta))
          cmd.Parameters.Add("@p_untilDate", SqlDbType.Date).Value = fechaHasta;
        else
          cmd.Parameters.Add("@p_untilDate", SqlDbType.Date).Value = DBNull.Value;

        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
      }
    }

    private void GenerarReporte() {
      DateTime desde, hasta;
      if (DateTime.TryParse(txtFechaDesde.Text, out desde) &&
          DateTime.TryParse(txtFechaHasta.Text, out hasta) &&
          hasta < desde) {
        MostrarMensaje("La fecha 'Hasta' no puede ser menor que la fecha 'Desde'.", esError: true);
        return;
      }

      DataTable dt = ObtenerDatosReporte();
      gvReporte.DataSource = dt;
      gvReporte.DataBind();

      lblTotalContratos.Text = dt.Rows.Count.ToString();

      decimal totalMXN = dt.AsEnumerable()
          .Where(r => string.Equals(r.Field<string>("currency"), "MXN", StringComparison.OrdinalIgnoreCase))
          .Sum(r => r.Field<decimal>("contract_cost"));

      decimal totalUSD = dt.AsEnumerable()
          .Where(r => string.Equals(r.Field<string>("currency"), "USD", StringComparison.OrdinalIgnoreCase))
          .Sum(r => r.Field<decimal>("contract_cost"));

      lblTotalMXN.Text = totalMXN.ToString("C2");
      lblTotalUSD.Text = totalUSD.ToString("C2");
    }

    protected void btnFiltrar_Click(object sender, EventArgs e) {
      lblMensaje.Text = "";
      GenerarReporte();
    }

    protected void btnLimpiar_Click(object sender, EventArgs e) {
      ddlEstatus.SelectedIndex = 0;
      ddlCliente.SelectedIndex = 0;
      InicializarRangoFechas();
      lblMensaje.Text = "";
      GenerarReporte();
    }

    protected void btnExportarExcel_Click(object sender, EventArgs e) {
      DataTable dt = ObtenerDatosReporte();

      if (dt.Rows.Count == 0) {
        MostrarMensaje("No hay datos para exportar con los filtros actuales.", esError: true);
        return;
      }

      string nombreArchivo = $"Reporte_Contratos_{DateTime.Now:yyyyMMdd_HHmm}.xls";

      Response.Clear();
      Response.Buffer = true;
      Response.AddHeader("content-disposition", $"attachment;filename={nombreArchivo}");
      Response.Charset = "UTF-8";
      Response.ContentEncoding = Encoding.UTF8;
      Response.BinaryWrite(Encoding.UTF8.GetPreamble());
      Response.ContentType = "application/vnd.ms-excel";

      using (StringWriter sw = new StringWriter())
      using (HtmlTextWriter hw = new HtmlTextWriter(sw)) {
        GridView gvExport = new GridView();
        gvExport.AutoGenerateColumns = false;
        gvExport.DataSource = dt;

        gvExport.Columns.Add(new BoundField { DataField = "Folio", HeaderText = "Folio" });
        gvExport.Columns.Add(new BoundField { DataField = "ClienteProveedor", HeaderText = "Cliente / Proveedor" });
        gvExport.Columns.Add(new BoundField { DataField = "RFC", HeaderText = "RFC" });
        gvExport.Columns.Add(new BoundField { DataField = "job_description", HeaderText = "Descripción del Trabajo" });
        gvExport.Columns.Add(new BoundField { DataField = "contract_cost", HeaderText = "Monto", DataFormatString = "{0:N2}" });
        gvExport.Columns.Add(new BoundField { DataField = "currency", HeaderText = "Moneda" });
        gvExport.Columns.Add(new BoundField { DataField = "begin_date", HeaderText = "Fecha Inicio", DataFormatString = "{0:dd/MM/yyyy}" });
        gvExport.Columns.Add(new BoundField { DataField = "end_date", HeaderText = "Fecha Fin", DataFormatString = "{0:dd/MM/yyyy}" });
        gvExport.Columns.Add(new BoundField { DataField = "Estatus", HeaderText = "Estatus" });
        gvExport.Columns.Add(new BoundField { DataField = "MotivoRechazo", HeaderText = "Motivo de Rechazo" });

        gvExport.DataBind();

        // Estilo limpio para encabezados en Excel
        gvExport.HeaderRow.Style.Add("background-color", "#0d6efd");
        gvExport.HeaderRow.Style.Add("color", "#ffffff");
        gvExport.HeaderRow.Style.Add("font-weight", "bold");

        gvExport.RenderControl(hw);
        Response.Write(sw.ToString());
        Response.Flush();
        Response.End();
      }
    }

    public override void VerifyRenderingInServerForm(Control control) {
    }

    private void MostrarMensaje(string mensaje, bool esError) {
      lblMensaje.Text = mensaje;
      lblMensaje.CssClass = esError ? "fw-semibold text-danger" : "fw-semibold text-success";
    }
  }
}