<%@ Page Title="" Language="C#" MasterPageFile="~/Source/Pages/MP.Master" AutoEventWireup="true" CodeBehind="providerCatalog.aspx.cs" Inherits="ContractManager.Source.Pages.providerCatalog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
  <div id="form1" runat="server">
    <div class="container py-5">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="h3 mb-0 text-dark">Catálogo de Proveedores</h2>
      </div>

      <!-- Campo oculto para saber si estamos insertando (0) o editando (>0) -->
      <asp:HiddenField ID="hfIdProveedor" runat="server" Value="0" />

      <!-- Tarjeta del formulario -->
      <div class="card shadow-sm mb-4">
        <div class="card-header bg-primary text-white">
          <strong>Datos del Proveedor</strong>
        </div>
        <div class="card-body">
          <div class="row g-3">
            <div class="col-md-6">
              <label for="txtRazonSocial" class="form-label fw-semibold">Razón Social</label>
              <asp:TextBox ID="txtRazonSocial" runat="server" CssClass="form-control" MaxLength="150" placeholder="Ej. Comercializadora del Norte S.A. de C.V." />
              <asp:RequiredFieldValidator ID="rfvRazon" runat="server" ControlToValidate="txtRazonSocial"
                ErrorMessage="* La razón social es requerida" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Proveedor" />
            </div>

            <div class="col-md-6">
              <label for="txtRFC" class="form-label fw-semibold">RFC</label>
              <asp:TextBox ID="txtRFC" runat="server" CssClass="form-control text-uppercase" MaxLength="13" placeholder="Ej. XAXX010101000" />
              <asp:RequiredFieldValidator ID="rfvRFC" runat="server" ControlToValidate="txtRFC"
                ErrorMessage="* El RFC es requerido" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Proveedor" />
            </div>

            <div class="col-md-6">
              <label for="txtTelefono" class="form-label fw-semibold">Teléfono</label>
              <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" MaxLength="20" placeholder="Ej. 6141234567" />
              <asp:RequiredFieldValidator ID="rfvTel" runat="server" ControlToValidate="txtTelefono"
                ErrorMessage="* El teléfono es requerido" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Proveedor" />
            </div>

            <div class="col-md-6">
              <label for="txtCorreo" class="form-label fw-semibold">Correo Electrónico</label>
              <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" TextMode="Email" MaxLength="100" placeholder="Ej. contacto@proveedor.com" />
              <asp:RequiredFieldValidator ID="rfvCorreo" runat="server" ControlToValidate="txtCorreo"
                ErrorMessage="* El correo electrónico es requerido" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Proveedor" />
            </div>
          </div>

          <div class="mt-4 d-flex gap-2">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary"
              OnClick="btnGuardar_Click" ValidationGroup="VG_Proveedor" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar / Limpiar" CssClass="btn btn-outline-secondary"
              OnClick="btnCancelar_Click" CausesValidation="false" />
          </div>

          <div class="mt-3">
            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-semibold text-success" EnableViewState="false" />
          </div>
        </div>
      </div>

      <!-- Tarjeta de la tabla -->
      <div class="card shadow-sm">
        <div class="card-header bg-white">
          <strong>Proveedores Registrados</strong>
        </div>
        <div class="card-body p-0">
          <div class="table-responsive">
            <asp:GridView ID="gvProveedores" runat="server" AutoGenerateColumns="False"
              CssClass="table table-striped table-hover align-middle mb-0"
              DataKeyNames="id" OnRowCommand="gvProveedores_RowCommand"
              EmptyDataText="No hay proveedores registrados todavía.">
              <Columns>
                <asp:BoundField DataField="business_name" HeaderText="Razón Social" />
                <asp:BoundField DataField="RFC" HeaderText="RFC" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="phone" HeaderText="Teléfono" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="email" HeaderText="Correo Electrónico" />
                <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <div class="d-flex gap-1">
                      <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btn btn-sm btn-warning"
                        CommandName="Editar" CommandArgument='<%# Eval("id") %>' CausesValidation="false" />
                      <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CssClass="btn btn-sm btn-danger"
                        CommandName="Eliminar" CommandArgument='<%# Eval("id") %>' CausesValidation="false"
                        OnClientClick="return confirm('¿Estás seguro de eliminar este proveedor?');" />
                    </div>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
            </asp:GridView>
          </div>
        </div>
      </div>
    </div>
  </div>
</asp:Content>
