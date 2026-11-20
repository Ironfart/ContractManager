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
  public partial class providerCatalog : System.Web.UI.Page {
    private readonly string conn = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e) {
      if (!IsPostBack) {
        CargarProveedores();
      }

    }
    private void CargarProveedores() {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getProviders", con)) {
        cmd.CommandType = CommandType.StoredProcedure;

        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        gvProveedores.DataSource = dt;
        gvProveedores.DataBind();
      }
    }

    protected void btnGuardar_Click(object sender, EventArgs e) {
      if (!Page.IsValid) return;

      int idProveedor = int.Parse(hfIdProveedor.Value);
      string spNombre = (idProveedor == 0) ? "sp_createProvider" : "sp_updateProvider";

      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand(spNombre, con)) {
        cmd.CommandType = CommandType.StoredProcedure;

        if (idProveedor > 0) {
          cmd.Parameters.Add("@p_id", SqlDbType.Int).Value = idProveedor;
        }

        cmd.Parameters.Add("@p_businessName", SqlDbType.VarChar, 150).Value = txtRazonSocial.Text.Trim();
        cmd.Parameters.Add("@p_RFC", SqlDbType.VarChar, 13).Value = txtRFC.Text.Trim().ToUpper();
        cmd.Parameters.Add("@p_phone", SqlDbType.VarChar, 20).Value = txtTelefono.Text.Trim();
        cmd.Parameters.Add("@p_email", SqlDbType.VarChar, 100).Value = txtCorreo.Text.Trim();

        con.Open();
        cmd.ExecuteNonQuery();
      }

      lblMensaje.Text = (idProveedor == 0) ? "Proveedor guardado correctamente." : "Proveedor actualizado correctamente.";
      LimpiarFormulario();
      CargarProveedores();
    }

    protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e) {
      int idProveedor = Convert.ToInt32(e.CommandArgument);

      if (e.CommandName == "Editar") {
        CargarDatosParaEditar(idProveedor);
      } else if (e.CommandName == "Eliminar") {
        EliminarProveedor(idProveedor);
      }
    }

    private void CargarDatosParaEditar(int id) {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_getProviderById", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@p_id", SqlDbType.Int).Value = id;

        con.Open();
        using (SqlDataReader reader = cmd.ExecuteReader()) {
          if (reader.Read()) {
            hfIdProveedor.Value = reader["id"].ToString();
            txtRazonSocial.Text = reader["business_Name"].ToString();
            txtRFC.Text = reader["RFC"].ToString();
            txtTelefono.Text = reader["phone"].ToString();
            txtCorreo.Text = reader["email"].ToString();

            btnGuardar.Text = "Actualizar";
            lblMensaje.Text = "";
          }
        }
      }
    }

    private void EliminarProveedor(int id) {
      using (SqlConnection con = new SqlConnection(conn))
      using (SqlCommand cmd = new SqlCommand("sp_Proveedores_Eliminar", con)) {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@p_id", SqlDbType.Int).Value = id;

        con.Open();
        cmd.ExecuteNonQuery();
      }

      lblMensaje.Text = "Proveedor eliminado.";
      LimpiarFormulario();
      CargarProveedores();
    }

    protected void btnCancelar_Click(object sender, EventArgs e) {
      LimpiarFormulario();
      lblMensaje.Text = "";
    }

    private void LimpiarFormulario() {
      hfIdProveedor.Value = "0";
      txtRazonSocial.Text = "";
      txtRFC.Text = "";
      txtTelefono.Text = "";
      txtCorreo.Text = "";
      btnGuardar.Text = "Guardar";
    }
  }
}