<%@ Page Title="Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="CapaPresentacion.Clientes.webClientesSC" ClientIDMode="Static" ValidateRequest="false" %>

<asp:Content ID="cntRuta" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item active" aria-current="page">Clientes</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAcciones" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-primary" href="RegistrarCliente.aspx">
        <i class="bi bi-person-plus me-2" aria-hidden="true"></i>Registrar cliente
    </a>
</asp:Content>

<asp:Content ID="cntContenido" ContentPlaceHolderID="cphContenido" runat="server">
    <div class="adms-card mb-4">
        <div class="adms-card-cuerpo">
            <asp:Panel ID="pnlBusquedaSC" runat="server" DefaultButton="btnBuscarSC" CssClass="row g-3 align-items-start">
                <div class="col-lg">
                    <label for="txtBusquedaSC" class="form-label">Buscar cliente</label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="bi bi-search" aria-hidden="true"></i></span>
                        <asp:TextBox ID="txtBusquedaSC" runat="server" CssClass="form-control" MaxLength="80"
                            placeholder="DPI, nombre, apellido o teléfono" autocomplete="off" aria-describedby="ayudaBusquedaSC" />
                    </div>
                    <div id="ayudaBusquedaSC" class="form-text">Puedes combinar palabras, por ejemplo «Pérez Ana», o escribir el DPI con espacios.</div>
                    <asp:Label ID="lblErrorBusquedaSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                </div>
                <div class="col-lg-auto d-flex gap-2 adms-busqueda-botones">
                    <asp:Button ID="btnBuscarSC" runat="server" Text="Buscar" CssClass="btn btn-primary px-4"
                        OnClick="btnBuscarSC_Click" data-adms-procesando="Buscando…" />
                    <asp:HyperLink ID="lnkLimpiarSC" runat="server" NavigateUrl="Clientes.aspx" CssClass="btn btn-light" Visible="false">Limpiar</asp:HyperLink>
                </div>
            </asp:Panel>
        </div>
    </div>

    <asp:Panel ID="pnlResultadosSC" runat="server" CssClass="adms-card adms-card-tabla">
        <div class="adms-card-encabezado">
            <div>
                <h2 class="adms-card-titulo"><asp:Literal ID="litResumenSC" runat="server" Mode="Encode" /></h2>
                <p class="adms-card-subtitulo"><asp:Literal ID="litLimiteSC" runat="server" Mode="Encode" /></p>
            </div>
            <span class="adms-contador"><asp:Literal ID="litTotalSC" runat="server" Mode="Encode" /></span>
        </div>

        <asp:Repeater ID="rptClientesSC" runat="server">
            <HeaderTemplate>
                <div class="table-responsive">
                    <table class="table table-hover adms-tabla">
                        <thead>
                            <tr>
                                <th scope="col">Cliente</th>
                                <th scope="col" class="d-none d-sm-table-cell">DPI</th>
                                <th scope="col" class="d-none d-md-table-cell">Teléfono</th>
                                <th scope="col" class="d-none d-xl-table-cell">Registro</th>
                                <th scope="col" class="text-end"><span class="visually-hidden">Acciones</span></th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                            <tr>
                                <td>
                                    <div class="adms-persona">
                                        <span class="adms-avatar" aria-hidden="true"><%# fncCodificarSC(CapaPresentacion.Utilidades.ClsFormatoSC.fncObtenerInicialesSC((string)Eval("lStrNombresSC"), (string)Eval("lStrApellidosSC"))) %></span>
                                        <div class="adms-persona-texto">
                                            <a class="adms-persona-nombre" href='DetalleCliente.aspx?id=<%# Eval("lIntIdClienteSC") %>'><%# fncCodificarSC(Eval("lStrNombresSC") + " " + Eval("lStrApellidosSC")) %></a>
                                            <div class="adms-persona-detalle"><%# fncCodificarSC(CapaPresentacion.Utilidades.ClsFormatoSC.fncTextoOpcionalSC((string)Eval("lStrCorreoSC"), "Sin correo registrado")) %></div>
                                        </div>
                                    </div>
                                </td>
                                <td class="adms-numero d-none d-sm-table-cell"><%# fncCodificarSC(CapaPresentacion.Utilidades.ClsFormatoSC.fncFormatearDpiSC((string)Eval("lStrDpiSC"))) %></td>
                                <td class="adms-numero d-none d-md-table-cell"><%# fncCodificarSC(CapaPresentacion.Utilidades.ClsFormatoSC.fncFormatearTelefonoSC((string)Eval("lStrTelefonoSC"))) %></td>
                                <td class="adms-numero d-none d-xl-table-cell text-secondary"><%# fncCodificarSC(CapaPresentacion.Utilidades.ClsFormatoSC.fncFormatearFechaSC((DateTime)Eval("lDatFechaRegistroUtcSC"))) %></td>
                                <td class="text-end">
                                    <a class="btn btn-sm btn-outline-primary text-nowrap" href='DetalleCliente.aspx?id=<%# Eval("lIntIdClienteSC") %>' title="Ver detalle">
                                        <i class="bi bi-eye" aria-hidden="true"></i><span class="d-none d-md-inline ms-1">Ver detalle</span>
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

        <asp:Panel ID="pnlVacioSC" runat="server" CssClass="adms-vacio" Visible="false">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-people"></i></span>
            <p class="adms-vacio-titulo"><asp:Literal ID="litVacioTituloSC" runat="server" Mode="Encode" /></p>
            <p class="adms-vacio-texto"><asp:Literal ID="litVacioDetalleSC" runat="server" Mode="Encode" /></p>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
