<%@ Page Title="Inventario de repuestos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inventario.aspx.cs" Inherits="CapaPresentacion.Inventario.webInventarioPP" ClientIDMode="Static" ValidateRequest="false" %>
<%@ Import Namespace="CapaLogica" %>
<%-- Módulo 3: Inventario de repuestos (Pavel). Consulta de repuestos y entradas a bodega. --%>

<asp:Content ID="cntRutaPP" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item active" aria-current="page">Inventario</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAccionesPP" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-primary" href="RegistroRepuesto.aspx">
        <i class="bi bi-plus-lg me-2" aria-hidden="true"></i>Registrar repuesto
    </a>
</asp:Content>

<asp:Content ID="cntContenidoPP" ContentPlaceHolderID="cphContenido" runat="server">
    <div class="adms-card mb-4">
        <div class="adms-card-cuerpo">
            <asp:Panel ID="pnlBusquedaPP" runat="server" DefaultButton="btnBuscarPP" CssClass="row g-3 align-items-start">
                <div class="col-lg">
                    <label for="txtBuscarPP" class="form-label">Buscar repuesto</label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="bi bi-search" aria-hidden="true"></i></span>
                        <asp:TextBox ID="txtBuscarPP" runat="server" CssClass="form-control" MaxLength="50"
                            placeholder="Nombre o código" autocomplete="off" aria-describedby="ayudaBuscarPP lblErrorBusquedaPP" />
                    </div>
                    <div id="ayudaBuscarPP" class="form-text">Escribe parte del nombre (sin importar tildes) o el código completo, por ejemplo FIL-ACE-COR15.</div>
                    <asp:Label ID="lblErrorBusquedaPP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                </div>
                <div class="col-lg-auto d-flex gap-2 adms-busqueda-botones">
                    <asp:Button ID="btnBuscarPP" runat="server" Text="Buscar" CssClass="btn btn-primary px-4"
                        OnClick="btnBuscarPP_Click" data-adms-procesando="Buscando…" />
                    <asp:HyperLink ID="lnkLimpiarPP" runat="server" NavigateUrl="Inventario.aspx" CssClass="btn btn-light" Visible="false">Limpiar</asp:HyperLink>
                </div>
            </asp:Panel>
        </div>
    </div>

    <asp:Panel ID="pnlResultadosPP" runat="server" CssClass="adms-card adms-card-tabla">
        <div class="adms-card-encabezado">
            <div>
                <h2 class="adms-card-titulo"><asp:Literal ID="litResumenPP" runat="server" Mode="Encode" /></h2>
                <p class="adms-card-subtitulo"><asp:Literal ID="litDetallePP" runat="server" Mode="Encode" /></p>
            </div>
            <span class="adms-contador"><asp:Literal ID="litTotalPP" runat="server" Mode="Encode" /></span>
        </div>

        <asp:Repeater ID="rptRepuestosPP" runat="server">
            <HeaderTemplate>
                <div class="table-responsive">
                    <table class="table table-hover adms-tabla">
                        <thead>
                            <tr>
                                <th scope="col">Repuesto</th>
                                <th scope="col" class="d-none d-md-table-cell">Código</th>
                                <th scope="col" class="d-none d-xl-table-cell">Descripción</th>
                                <th scope="col">Stock</th>
                                <th scope="col" class="text-end"><span class="visually-hidden">Acciones</span></th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                            <tr>
                                <td>
                                    <div class="adms-persona">
                                        <span class="adms-avatar d-none d-sm-inline-flex" aria-hidden="true"><%# FncCodificarPP(FncInicialesPP((string)Eval("lStrNombrePP"))) %></span>
                                        <div class="adms-persona-texto">
                                            <div class="fw-semibold"><%# FncCodificarPP(Eval("lStrNombrePP")) %></div>
                                            <div class="adms-persona-detalle"><%# FncCodificarPP(Eval("lStrMarcaCarroPP") + " " + Eval("lStrModeloCarroPP") + " (" + Eval("lIntAnioCarroPP") + ")") %></div>
                                            <%-- En pantallas pequeñas la columna «Código» se oculta: el código se muestra aquí. --%>
                                            <div class="adms-persona-detalle adms-numero d-md-none"><%# FncCodificarPP(Eval("lStrCodigoPP")) %></div>
                                        </div>
                                    </div>
                                </td>
                                <td class="d-none d-md-table-cell adms-numero fw-semibold"><%# FncCodificarPP(Eval("lStrCodigoPP")) %></td>
                                <td class="d-none d-xl-table-cell text-secondary"><%# FncCodificarPP(Eval("lStrDescripcionPP")) %></td>
                                <td><span class='<%# FncClaseStockPP((int)Eval("lIntStockPP")) %>'><%# FncCodificarPP(FncTextoStockPP((int)Eval("lIntStockPP"))) %></span></td>
                                <td class="text-end">
                                    <a class="btn btn-sm btn-outline-primary text-nowrap" href='<%# FncCodificarPP(FncUrlEntradaPP((int)Eval("lIntIdRepuestoPP"))) %>' title="Registrar entrada">
                                        <i class="bi bi-box-arrow-in-down" aria-hidden="true"></i><span class="visually-hidden d-xl-none">Registrar entrada</span><span class="d-none d-xl-inline ms-1">Registrar entrada</span>
                                    </a>
                                </td>
                            </tr>
            </ItemTemplate>
            <FooterTemplate>
                        </tbody>
                    </table>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlVacioPP" runat="server" CssClass="adms-vacio" Visible="false">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-box-seam"></i></span>
            <p class="adms-vacio-titulo"><asp:Literal ID="litVacioTituloPP" runat="server" Mode="Encode" /></p>
            <p class="adms-vacio-texto"><asp:Literal ID="litVacioDetallePP" runat="server" Mode="Encode" /></p>
        </asp:Panel>
    </asp:Panel>

    <%-- Ventana para registrar una entrada a bodega. La abre el servidor cuando la dirección
         trae ?entrada=<id del repuesto>; «Cancelar» vuelve al listado sin cambiar nada. --%>
    <asp:Panel ID="pnlEntradaPP" runat="server" CssClass="adms-no-imprimir" Visible="false">
        <div class="modal d-block adms-modal-servidor" tabindex="-1" role="dialog" aria-modal="true" aria-labelledby="tituloEntradaPP">
            <div class="modal-dialog modal-dialog-centered">
                <asp:Panel ID="pnlFormularioEntradaPP" runat="server" CssClass="modal-content" DefaultButton="btnGuardarEntradaPP">
                    <div class="modal-header">
                        <h2 class="modal-title fs-5" id="tituloEntradaPP">Registrar entrada a bodega</h2>
                        <a class="btn-close" href="<%= FncCodificarPP(gStrUrlVolverPP) %>" aria-label="Cerrar"></a>
                    </div>
                    <div class="modal-body">
                        <p class="text-secondary small mb-3">Las unidades que escribas se suman al stock actual del repuesto.</p>
                        <div class="adms-seleccion mb-3">
                            <span class="adms-avatar" aria-hidden="true"><asp:Literal ID="litInicialesEntradaPP" runat="server" Mode="Encode" /></span>
                            <div class="adms-persona-texto">
                                <div class="fw-semibold"><asp:Literal ID="litNombreEntradaPP" runat="server" Mode="Encode" /></div>
                                <div class="adms-persona-detalle adms-numero">Código <asp:Literal ID="litCodigoEntradaPP" runat="server" Mode="Encode" /> · Stock actual: <asp:Literal ID="litStockEntradaPP" runat="server" Mode="Encode" /></div>
                            </div>
                        </div>
                        <label for="txtCantidadPP" class="form-label">Unidades que ingresan <span class="adms-requerido" aria-hidden="true">*</span></label>
                        <asp:TextBox ID="txtCantidadPP" runat="server" CssClass="form-control adms-numero" MaxLength="4"
                            inputmode="numeric" autocomplete="off" autofocus="autofocus" required="required" aria-describedby="ayudaCantidadPP errCantidadPP" />
                        <div id="ayudaCantidadPP" class="form-text">Número entero entre 1 y <%= ClsRepuestoLNPP.lIntEntradaMaximaPP.ToString("N0") %>.</div>
                        <asp:Label ID="errCantidadPP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                    </div>
                    <div class="modal-footer">
                        <a class="btn btn-light" href="<%= FncCodificarPP(gStrUrlVolverPP) %>">Cancelar</a>
                        <asp:Button ID="btnGuardarEntradaPP" runat="server" Text="Guardar entrada" CssClass="btn btn-primary px-4"
                            OnClick="btnGuardarEntradaPP_Click" data-adms-procesando="Guardando…" />
                    </div>
                </asp:Panel>
            </div>
        </div>
        <div class="modal-backdrop show"></div>
    </asp:Panel>
</asp:Content>
