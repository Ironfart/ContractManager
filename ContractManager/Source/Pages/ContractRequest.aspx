<%@ Page Title="" Language="C#" MasterPageFile="~/Source/Pages/MP.Master" AutoEventWireup="true" CodeBehind="ContractRequest.aspx.cs" Inherits="ContractManager.Source.Pages.ContractRequest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
  <div id="form1" runat="server">
    <asp:HiddenField ID="hfIdContrato" runat="server" Value="0" />
    <div class="container py-4">

      <div class="card shadow-sm mb-4">
        <div class="card-header bg-primary text-white">
          <strong>Datos del Contrato</strong>
        </div>
        <div class="card-body">
          <div class="row g-3">

            <!-- provider -->
            <div class="col-md-6">
              <label for="ddlProveedor" class="form-label fw-semibold">Proveedor</label>
              <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                <asp:ListItem Text="-- Seleccione un proveedor --" Value="" />
              </asp:DropDownList>
              <asp:RequiredFieldValidator ID="rfvProveedor" runat="server" ControlToValidate="ddlProveedor"
                InitialValue="" ErrorMessage="* Debe seleccionar un proveedor"
                CssClass="text-danger small d-block mt-1" Display="Dynamic" ValidationGroup="VG_Contrato" />
            </div>

            <!-- amount -->
            <div class="col-md-4">
              <label for="txtMonto" class="form-label fw-semibold">Monto del Contrato</label>
              <asp:TextBox ID="txtMonto" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0.01" placeholder="0.00" />
              <asp:RequiredFieldValidator ID="rfvMonto" runat="server" ControlToValidate="txtMonto"
                ErrorMessage="* El monto es requerido" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Contrato" />
              <asp:CompareValidator ID="cvMonto" runat="server" ControlToValidate="txtMonto"
                Operator="GreaterThan" ValueToCompare="0" Type="Double"
                ErrorMessage="* El monto debe ser mayor a 0" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Contrato" />
            </div>

            <!-- currency -->
            <div class="col-md-2">
              <label for="ddlMoneda" class="form-label fw-semibold">Moneda</label>
              <asp:DropDownList ID="ddlMoneda" runat="server" CssClass="form-select">
                <asp:ListItem Text="MXN" Value="MXN" Selected="True" />
                <asp:ListItem Text="USD" Value="USD" />
              </asp:DropDownList>
            </div>

            <!-- job description -->
            <div class="col-12">
              <label for="txtDescripcion" class="form-label fw-semibold">Descripción del Trabajo</label>
              <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" TextMode="MultiLine"
                Rows="3" MaxLength="500" placeholder="Describa el alcance del servicio o suministro a contratar..." />
              <asp:RequiredFieldValidator ID="rfvDescripcion" runat="server" ControlToValidate="txtDescripcion"
                ErrorMessage="* La descripción del trabajo es requerida" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Contrato" />
            </div>

            <!-- begin date -->
            <div class="col-md-4">
              <label for="txtFechaInicio" class="form-label fw-semibold">Fecha de Inicio</label>
              <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date" />
              <asp:RequiredFieldValidator ID="rfvFechaInicio" runat="server" ControlToValidate="txtFechaInicio"
                ErrorMessage="* La fecha de inicio es requerida" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Contrato" />
            </div>

            <!-- end date -->
            <div class="col-md-4">
              <label for="txtFechaFin" class="form-label fw-semibold">Fecha de Fin</label>
              <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-control" TextMode="Date" />
              <asp:RequiredFieldValidator ID="rfvFechaFin" runat="server" ControlToValidate="txtFechaFin"
                ErrorMessage="* La fecha de fin es requerida" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Contrato" />
              <asp:CompareValidator ID="cvFechas" runat="server" ControlToValidate="txtFechaFin"
                ControlToCompare="txtFechaInicio" Operator="GreaterThanEqual" Type="Date"
                ErrorMessage="* La fecha de fin debe ser igual o posterior a la fecha de inicio"
                CssClass="text-danger small d-block mt-1" Display="Dynamic" ValidationGroup="VG_Contrato" />
            </div>

            <!-- pdf -->
            <div class="col-md-4">
              <label for="fuArchivoPdf" class="form-label fw-semibold">Archivo del Contrato (PDF)</label>
              <asp:FileUpload ID="fuArchivoPdf" runat="server" CssClass="form-control" accept=".pdf,application/pdf" />
              <asp:RequiredFieldValidator ID="rfvPdf" runat="server" ControlToValidate="fuArchivoPdf"
                ErrorMessage="* Debe adjuntar un archivo PDF" CssClass="text-danger small d-block mt-1"
                Display="Dynamic" ValidationGroup="VG_Contrato" />
              <asp:RegularExpressionValidator ID="revPdf" runat="server" ControlToValidate="fuArchivoPdf"
                ValidationExpression="^.*\.(pdf|PDF)$" ErrorMessage="* Solo se permiten archivos .pdf"
                CssClass="text-danger small d-block mt-1" Display="Dynamic" ValidationGroup="VG_Contrato" />
            </div>
          </div>

          <div class="mt-4 d-flex gap-2">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar Solicitud" CssClass="btn btn-primary"
              OnClick="btnGuardar_Click" ValidationGroup="VG_Contrato" />
            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary"
              OnClick="btnLimpiar_Click" CausesValidation="false" />
          </div>

          <div class="mt-3">
            <asp:Label ID="lblMensaje" runat="server" EnableViewState="false" />
          </div>
        </div>
      </div>

      <!-- request list -->
      <div class="card shadow-sm">
        <div class="card-header bg-white">
          <strong>Solicitudes Registradas</strong>
        </div>
        <div class="card-body p-0">
          <div class="table-responsive">
            <asp:GridView ID="gvContratos" runat="server" AutoGenerateColumns="False"
              CssClass="table table-striped table-hover align-middle mb-0"
              EmptyDataText="No hay solicitudes de contrato registradas.">
              <Columns>
                <asp:BoundField DataField="id" HeaderText="Folio" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" />
                <asp:BoundField DataField="job_description" HeaderText="Descripción del Trabajo" />
                <asp:TemplateField HeaderText="Monto" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <%# Eval("contract_cost", "{0:N2}") %> <%# Eval("currency") %>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="begin_date" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="end_date" HeaderText="Fin" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="text-nowrap" />
                <asp:TemplateField HeaderText="Estatus" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <span class='<%# ObtenerClaseBadge(Eval("Estatus").ToString()) %>'>
                      <%# Eval("Estatus") %>
                    </span>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Motivo Rechazo">
                  <ItemTemplate>
                    <%# string.IsNullOrWhiteSpace(Eval("reject_reason")?.ToString()) ? "-" : Eval("reject_reason") %>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <asp:Button ID="btnEditar" runat="server" Text="Editar"
                      CssClass="btn btn-sm btn-warning"
                      CommandName="Editar"
                      CommandArgument='<%# Eval("id") %>'
                      CausesValidation="false" />
                    <asp:Label ID="lblBloqueado" runat="server" Text="Bloqueado"
                      CssClass="badge bg-secondary" Visible="false" />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="PDF" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <a href='<%# ResolveUrl(Eval("file_path").ToString()) %>' target="_blank"
                      class="btn btn-sm btn-outline-danger">Ver PDF
                    </a>
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
