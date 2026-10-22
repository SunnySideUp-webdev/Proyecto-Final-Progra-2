<%@ Page Title="Repuestos de la orden" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RepuestosOrdenSC.aspx.cs" Inherits="CapaPresentacion.OrdenesReparacion.webRepuestosOrdenSC" ClientIDMode="Static" ValidateRequest="false" %>
<%@ Import Namespace="CapaPresentacion.Utilidades" %>
<%-- Repuestos utilizados en una orden (parte de Sharon). Une el módulo de órdenes (Neider)
     con el inventario (Pavel): registrar un repuesto lo descuenta del stock. --%>

<asp:Content ID="cntRutaSC" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item"><a href="OrdenesNT.aspx">Órdenes de reparación</a></li>
            <asp:PlaceHolder ID="phRutaOrdenSC" runat="server" Visible="false">
                <li class="breadcrumb-item"><asp:HyperLink ID="lnkRutaOrdenSC" runat="server" /></li>
            </asp:PlaceHolder>
            <li class="breadcrumb-item active" aria-current="page">Repuestos utilizados</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAccionesSC" ContentPlaceHolderID="cphAcciones" runat="server">
    <asp:HyperLink ID="lnkVolverOrdenSC" runat="server" CssClass="btn btn-light" NavigateUrl="OrdenesNT.aspx">
        <i class="bi bi-arrow-left me-2" aria-hidden="true"></i>Volver a la orden
    </asp:HyperLink>
</asp:Content>

<asp:Content ID="cntContenidoSC" ContentPlaceHolderID="cphContenido" runat="server">
    <%-- Datos de la orden. --%>
    <asp:Panel ID="pnlOrdenSC" runat="server" CssClass="adms-card mb-4" Visible="false">
        <div class="adms-card-cuerpo d-flex flex-wrap align-items-center gap-3">
            <span class="adms-placa" role="img" aria-label='<%= FncCodificarSC("Placa " + gStrPlacaSC) %>'>
                <span class="adms-placa-pais" aria-hidden="true">GT</span>
                <span class="adms-placa-numero" aria-hidden="true"><%= FncCodificarSC(gStrPlacaSC) %></span>
            </span>
            <div class="flex-grow-1">
                <div class="fw-semibold">
                    Orden #<asp:Literal ID="litNumeroOrdenSC" runat="server" Mode="Encode" /> ·
                    <asp:Literal ID="litVehiculoSC" runat="server" Mode="Encode" />
                    <span class='<%= gStrClaseEstadoSC %>'><asp:Literal ID="litEstadoSC" runat="server" Mode="Encode" /></span>
                </div>
                <div class="adms-persona-detalle">Motivo: <asp:Literal ID="litMotivoSC" runat="server" Mode="Encode" /></div>
                <asp:PlaceHolder ID="phClienteSC" runat="server" Visible="false">
                    <div class="adms-persona-detalle">Cliente: <asp:HyperLink ID="lnkClienteSC" runat="server" CssClass="adms-persona-nombre" /></div>
                </asp:PlaceHolder>
            </div>
        </div>
    </asp:Panel>

    <%-- Formulario para agregar un repuesto: solo mientras la orden está abierta. --%>
    <asp:Panel ID="pnlAgregarSC" runat="server" CssClass="adms-card mb-4" Visible="false">
        <div class="adms-card-encabezado">
            <div>
                <h2 class="adms-card-titulo">Agregar repuesto utilizado</h2>
                <p class="adms-card-subtitulo">Al agregarlo, las unidades se descuentan del inventario.</p>
            </div>
        </div>
        <div class="adms-card-cuerpo">
            <asp:Panel ID="pnlFormularioSC" runat="server" DefaultButton="btnAgregarSC" CssClass="row g-3 align-items-start">
                <div class="col-lg">
                    <label for="ddlRepuestoSC" class="form-label">Repuesto <span class="adms-requerido" aria-hidden="true">*</span></label>
                    <asp:DropDownList ID="ddlRepuestoSC" runat="server" CssClass="form-select" aria-describedby="ayudaRepuestoSC errRepuestoSC" />
                    <div id="ayudaRepuestoSC" class="form-text">Solo aparecen los repuestos con unidades en bodega.</div>
                    <asp:Label ID="errRepuestoSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                </div>
                <div class="col-sm-4 col-lg-2">
                    <label for="txtCantidadSC" class="form-label">Cantidad <span class="adms-requerido" aria-hidden="true">*</span></label>
                    <asp:TextBox ID="txtCantidadSC" runat="server" CssClass="form-control adms-numero" MaxLength="3"
                        inputmode="numeric" autocomplete="off" required="required" aria-describedby="errCantidadSC" />
                    <asp:Label ID="errCantidadSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                </div>
                <div class="col-sm-auto adms-busqueda-botones">
                    <asp:Button ID="btnAgregarSC" runat="server" Text="Agregar" CssClass="btn btn-primary px-4"
                        OnClick="btnAgregarSC_Click" data-adms-procesando="Agregando…" />
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlSinDisponiblesSC" runat="server" CssClass="text-secondary" Visible="false">
                No hay repuestos con unidades en bodega. Registra una entrada en el
                <a href="../Inventario/Inventario.aspx">inventario</a> y vuelve a esta página.
            </asp:Panel>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlCerradaSC" runat="server" CssClass="alert alert-info adms-alerta" Visible="false">
        <i class="bi bi-lock flex-shrink-0" aria-hidden="true"></i>
        <div>La orden está cerrada: su lista de repuestos es de solo lectura.</div>
    </asp:Panel>

    <%-- Repuestos ya registrados en la orden. --%>
    <asp:Panel ID="pnlUsadosSC" runat="server" CssClass="adms-card adms-card-tabla" Visible="false">
        <div class="adms-card-encabezado">
            <div>
                <h2 class="adms-card-titulo">Repuestos utilizados</h2>
                <p class="adms-card-subtitulo">En el orden en que se registraron.</p>
            </div>
            <span class="adms-contador"><asp:Literal ID="litTotalSC" runat="server" Mode="Encode" /></span>
        </div>

        <asp:Repeater ID="rptUsadosSC" runat="server" OnItemCommand="rptUsadosSC_ItemCommand" ClientIDMode="Predictable">
            <HeaderTemplate>
                <div class="table-responsive">
                    <table class="table table-hover adms-tabla">
                        <thead>
                            <tr>
                                <th scope="col">Repuesto</th>
                                <th scope="col">Cantidad</th>
                                <th scope="col" class="d-none d-md-table-cell">Registrado</th>
                                <th scope="col" class="text-end"><span class="visually-hidden">Acciones</span></th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                            <tr>
                                <td>
                                    <div class="fw-semibold"><%# FncCodificarSC(Eval("gStrNombreRepuestoSC")) %></div>
                                    <div class="adms-persona-detalle adms-numero"><%# FncCodificarSC(Eval("gStrCodigoSC")) %></div>
                                </td>
                                <td class="adms-numero"><%# Eval("gIntCantidadSC") %></td>
                                <td class="d-none d-md-table-cell adms-numero text-secondary"><%# FncCodificarSC(ClsFormatoSC.FncFormatearFechaSC((DateTime)Eval("gDatFechaRegistroUtcSC"))) %></td>
                                <td class="text-end">
                                    <asp:LinkButton ID="lnkQuitarSC" runat="server" CssClass="btn btn-sm btn-outline-danger text-nowrap"
                                        CommandName="Quitar" CommandArgument='<%# Eval("gIntIdRepuestoOrdenSC") %>' Visible='<%# gBlnOrdenAbiertaSC %>'
                                        OnClientClick="return confirm('¿Quitar este repuesto de la orden? Sus unidades vuelven al inventario.');"
                                        ToolTip="Quitar de la orden">
                                        <i class="bi bi-x-lg" aria-hidden="true"></i><span class="ms-1">Quitar</span>
                                    </asp:LinkButton>
                                </td>
                            </tr>
            </ItemTemplate>
            <FooterTemplate>
                        </tbody>
                    </table>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlSinUsadosSC" runat="server" CssClass="adms-vacio" Visible="false">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-box-seam"></i></span>
            <p class="adms-vacio-titulo">Todavía no se registran repuestos en esta orden</p>
            <p class="adms-vacio-texto"><asp:Literal ID="litSinUsadosSC" runat="server" Mode="Encode" /></p>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="pnlNoEncontradaSC" runat="server" CssClass="adms-card" Visible="false">
        <div class="adms-vacio">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-clipboard-x"></i></span>
            <p class="adms-vacio-titulo">No encontramos la orden solicitada</p>
            <p class="adms-vacio-texto">Es posible que el enlace esté incompleto. Búscala en el listado de órdenes.</p>
            <a class="btn btn-primary" href="OrdenesNT.aspx">Ir a órdenes de reparación</a>
        </div>
    </asp:Panel>
</asp:Content>
