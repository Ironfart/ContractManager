<%@ Page Title="" Language="C#" MasterPageFile="~/Source/Pages/MP.Master" AutoEventWireup="true" CodeBehind="revision.aspx.cs" Inherits="ContractManager.Source.Pages.revision" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
  <script type="text/javascript">
    function abrirModalRechazo() {
      var modalEl = document.getElementById('modalRechazo');
      var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
      modal.show();
    }
  </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
  <div id="form1" runat="server">
    <div class="container py-4">
      <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
        <h2 class="h3 mb-0 text-dark">Revisión y Autorización de Contratos</h2>
      </div>

      <!-- filters -->
      <div class="card shadow-sm mb-4">
        <div class="card-body">
          <div class="row g-3 align-items-end">
            <div class="col-md-4">
              <label for="ddlFiltroEstatus" class="form-label fw-semibold">Filtrar por Estatus</label>
              <asp:DropDownList ID="ddlFiltroEstatus" runat="server" CssClass="form-select"
                AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroEstatus_SelectedIndexChanged">
                <asp:ListItem Text="Pendientes" Value="Pendiente" Selected="True" />
                <asp:ListItem Text="Autorizados" Value="Autorizado" />
                <asp:ListItem Text="Rechazados" Value="Rechazado" />
                <asp:ListItem Text="Todos" Value="" />
              </asp:DropDownList>
            </div>

            <div class="col-md-8">
              <asp:Label ID="lblMensaje" runat="server" EnableViewState="false" />
            </div>
          </div>
        </div>
      </div>

      <!-- request list -->
      <div class="card shadow-sm">
        <div class="card-header bg-white">
          <strong>Solicitudes para Revisión</strong>
        </div>
        <div class="card-body p-0">
          <div class="table-responsive">
            <asp:GridView ID="gvRevision" runat="server" AutoGenerateColumns="False"
              CssClass="table table-striped table-hover align-middle mb-0"
              DataKeyNames="id"
              OnRowCommand="gvRevision_RowCommand"
              OnRowDataBound="gvRevision_RowDataBound"
              EmptyDataText="No hay solicitudes de contrato para el estatus seleccionado.">
              <Columns>
                <asp:BoundField DataField="id" HeaderText="Folio" ItemStyle-CssClass="text-nowrap" />
                <asp:BoundField DataField="provider" HeaderText="Proveedor" />
                <asp:BoundField DataField="jobDescription" HeaderText="Descripción del Trabajo" />
                <asp:TemplateField HeaderText="Monto" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <%# Eval("contractCost", "{0:N2}") %> <%# Eval("currency") %>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Vigencia" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <%# Eval("beginDate", "{0:dd/MM/yyyy}") %> - <%# Eval("endDate", "{0:dd/MM/yyyy}") %>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Estatus" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <span class='<%# ObtenerClaseBadge(Eval("Estatus").ToString()) %>'>
                      <%# Eval("Estatus") %>
                    </span>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Motivo de Rechazo">
                  <ItemTemplate>
                    <%# string.IsNullOrWhiteSpace(Eval("rejectReason")?.ToString()) ? "-" : Eval("rejectReason") %>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="PDF" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <a href='<%# ResolveUrl(Eval("filePath").ToString()) %>' target="_blank"
                      class="btn btn-sm btn-outline-primary">Ver PDF
                    </a>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-nowrap">
                  <ItemTemplate>
                    <div class="d-flex gap-1">
                      <asp:Button ID="btnAutorizar" runat="server" Text="Autorizar"
                        CssClass="btn btn-sm btn-success"
                        CommandName="Autorizar"
                        CommandArgument='<%# Eval("id") %>'
                        CausesValidation="false"
                        OnClientClick="return confirm('¿Deseas autorizar este contrato?');" />
                      <asp:Button ID="btnRechazar" runat="server" Text="Rechazar"
                        CssClass="btn btn-sm btn-danger"
                        CommandName="Rechazar"
                        CommandArgument='<%# Eval("id") %>'
                        CausesValidation="false" />
                    </div>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
            </asp:GridView>
          </div>
        </div>
      </div>
    </div>

    <asp:HiddenField ID="hfIdContratoRechazo" runat="server" Value="0" />

    <!-- Modal -->
    <div class="modal fade" id="modalRechazo" tabindex="-1" aria-labelledby="modalRechazoLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-danger text-white">
            <h5 class="modal-title" id="modalRechazoLabel">Rechazar Contrato</h5>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
          </div>
          <div class="modal-body">
            <label for="txtMotivoRechazo" class="form-label fw-semibold">Motivo del Rechazo</label>
            <asp:TextBox ID="txtMotivoRechazo" runat="server" CssClass="form-control" TextMode="MultiLine"
              Rows="4" MaxLength="500" placeholder="Indique el motivo por el cual se rechaza la solicitud..." />
            <asp:RequiredFieldValidator ID="rfvMotivoRechazo" runat="server"
              ControlToValidate="txtMotivoRechazo"
              ErrorMessage="* Debe capturar un motivo de rechazo"
              CssClass="text-danger small d-block mt-1"
              Display="Dynamic" ValidationGroup="VG_Rechazo" />
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
            <asp:Button ID="btnConfirmarRechazo" runat="server" Text="Confirmar Rechazo"
              CssClass="btn btn-danger" ValidationGroup="VG_Rechazo"
              OnClick="btnConfirmarRechazo_Click" />
          </div>
        </div>
      </div>
    </div>
  </div>
</asp:Content>
