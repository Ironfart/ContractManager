<%@ Page Title="" Language="C#" MasterPageFile="~/Source/Pages/MP.Master" AutoEventWireup="true" CodeBehind="contractReport.aspx.cs" Inherits="ContractManager.Source.Pages.contractReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
  <div id="form1" runat="server">
    <div class="container py-4">
      <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
        <h2 class="h3 mb-0 text-dark">Reporte de Contratos</h2>
        <div class="d-flex gap-2">
          <a href="RevisionContratos.aspx" class="btn btn-outline-secondary btn-sm">Revisión</a>
          <a href="SolicitudContrato.aspx" class="btn btn-outline-secondary btn-sm">Solicitudes</a>
          <a href="Proveedores.aspx" class="btn btn-outline-secondary btn-sm">Proveedores</a>
        </div>
      </div>

      <!-- Tarjeta de Filtros -->
      <div class="card shadow-sm mb-4">
        <div class="card-header bg-primary text-white">
          <strong>Filtros de Búsqueda</strong>
        </div>
        <div class="card-body">
          <div class="row g-3">
            <!-- Estatus -->
            <div class="col-md-3">
              <label for="ddlEstatus" class="form-label fw-semibold">Estatus</label>
              <asp:DropDownList ID="ddlEstatus" runat="server" CssClass="form-select">
                <asp:ListItem Text="Todos" Value="" Selected="True" />
                <asp:ListItem Text="Pendiente" Value="Pendiente" />
                <asp:ListItem Text="Autorizado" Value="Autorizado" />
                <asp:ListItem Text="Rechazado" Value="Rechazado" />
              </asp:DropDownList>
            </div>

            <!-- Cliente / Proveedor -->
            <div class="col-md-3">
              <label for="ddlCliente" class="form-label fw-semibold">Cliente / Proveedor</label>
              <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select">
                <asp:ListItem Text="Todos" Value="0" />
              </asp:DropDownList>
            </div>

            <!-- Fecha Inicio Desde (precargada con la más antigua) -->
            <div class="col-md-3">
              <label for="txtFechaDesde" class="form-label fw-semibold">Inicio Trabajos (Desde)</label>
              <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <!-- Fecha Inicio Hasta -->
            <div class="col-md-3">
              <label for="txtFechaHasta" class="form-label fw-semibold">Inicio Trabajos (Hasta)</label>
              <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
          </div>

          <div class="mt-4 d-flex flex-wrap gap-2">
            <asp:Button ID="btnFiltrar" runat="server" Text="Generar Reporte" CssClass="btn btn-primary"
              OnClick="btnFiltrar_Click" />
            <asp:Button ID="btnLimpiar" runat="server" Text="Restablecer Filtros" CssClass="btn btn-outline-secondary"
              OnClick="btnLimpiar_Click" />
            <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel" CssClass="btn btn-success ms-auto"
              OnClick="btnExportarExcel_Click" />
          </div>

          <div class="mt-3">
            <asp:Label ID="lblMensaje" runat="server" EnableViewState="false" />
          </div>
        </div>
      </div>

      <!-- Resumen rápido -->
      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <div class="card shadow-sm border-start border-4 border-primary">
            <div class="card-body py-2">
              <div class="small text-muted">Total de Contratos</div>
              <div class="h5 mb-0 fw-bold">
                <asp:Label ID="lblTotalContratos" runat="server" Text="0" />
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card shadow-sm border-start border-4 border-success">
            <div class="card-body py-2">
              <div class="small text-muted">Monto Total MXN</div>
              <div class="h5 mb-0 fw-bold">
                <asp:Label ID="lblTotalMXN" runat="server" Text="$0.00" />
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card shadow-sm border-start border-4 border-info">
            <div class="card-body py-2">
              <div class="small text-muted">Monto Total USD</div>
              <div class="h5 mb-0 fw-bold">
                <asp:Label ID="lblTotalUSD" runat="server" Text="$0.00" />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Tabla del Reporte -->
      <div class="card shadow-sm">
        <div class="card-header bg-white">
          <strong>Resultados del Reporte</strong>
        </div>
        <div class="card-body p-0">
          <div class="table-responsive">
            <asp:GridView ID="gvReporte" runat="server" AutoGenerateColumns="False"
              CssClass="table table-striped table-hover align-middle mb-0"
              EmptyDataText="No se encontraron contratos con los filtros seleccionados.">
              <Columns>
                <asp:BoundField DataField="Folio" HeaderText="Folio" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="ClienteProveedor" HeaderText="Cliente / Proveedor" />
                <asp:BoundField DataField="RFC" HeaderText="RFC" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="job_description" HeaderText="Descripción del Trabajo" />
                <asp:BoundField DataField="contract_cost" HeaderText="Monto" DataFormatString="{0:N2}" ItemStyle-CssClass="text-end text-nowrap" />
                <asp:BoundField DataField="currency" HeaderText="Moneda" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="begin_date" HeaderText="Fecha Inicio" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="end_date" HeaderText="Fecha Fin" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="Estatus" HeaderText="Estatus" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="MotivoRechazo" HeaderText="Motivo de Rechazo" />
              </Columns>
            </asp:GridView>
          </div>
        </div>
      </div>
    </div>
  </div>
</asp:Content>
