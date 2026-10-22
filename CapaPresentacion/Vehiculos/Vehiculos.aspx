<%@ Page Title="Vehículos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Vehiculos.aspx.cs" Inherits="CapaPresentacion.Vehiculos.webVehiculosSC" ClientIDMode="Static" ValidateRequest="false" %>
<%@ Import Namespace="CapaPresentacion.Utilidades" %>

<asp:Content ID="cntRuta" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item active" aria-current="page">Vehículos</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAcciones" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-primary" href="RegistrarVehiculo.aspx">
        <i class="bi bi-plus-lg me-2" aria-hidden="true"></i>Registrar vehículo
    </a>
</asp:Content>

<asp:Content ID="cntContenido" ContentPlaceHolderID="cphContenido" runat="server">
    <div class="adms-card mb-4">
        <div class="adms-card-cuerpo">
            <asp:Panel ID="pnlBusquedaSC" runat="server" DefaultButton="btnBuscarSC" CssClass="row g-3 align-items-start">
                <div class="col-lg">
                    <label for="txtBusquedaSC" class="form-label">Buscar vehículo</label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="bi bi-search" aria-hidden="true"></i></span>
                        <asp:TextBox ID="txtBusquedaSC" runat="server" CssClass="form-control" MaxLength="40"
                            placeholder="Placa o VIN" autocomplete="off" aria-describedby="ayudaBusquedaSC" />
                    </div>
                    <div id="ayudaBusquedaSC" class="form-text">Puedes escribir la placa completa o una parte, con o sin guion (P-123ABC), o el VIN.</div>
                    <asp:Label ID="lblErrorBusquedaSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                </div>
                <div class="col-lg-auto d-flex gap-2 adms-busqueda-botones">
                    <asp:Button ID="btnBuscarSC" runat="server" Text="Buscar" CssClass="btn btn-primary px-4"
                        OnClick="btnBuscarSC_Click" data-adms-procesando="Buscando…" />
                    <asp:HyperLink ID="lnkLimpiarSC" runat="server" NavigateUrl="Vehiculos.aspx" CssClass="btn btn-light" Visible="false">Limpiar</asp:HyperLink>
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

        <asp:Repeater ID="rptVehiculosSC" runat="server">
            <HeaderTemplate>
                <div class="table-responsive">
                    <table class="table table-hover adms-tabla">
                        <thead>
                            <tr>
                                <th scope="col">Vehículo</th>
                                <th scope="col" class="d-none d-xl-table-cell">VIN</th>
                                <th scope="col" class="d-none d-md-table-cell">Propietario</th>
                                <th scope="col" class="d-none d-xxl-table-cell">Registro</th>
                                <th scope="col" class="text-end"><span class="visually-hidden">Acciones</span></th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                            <tr>
                                <td>
                                    <div class="adms-vehiculo">
                                        <span class="adms-placa" role="img" aria-label='<%# fncCodificarSC("Placa " + ClsFormatoSC.fncFormatearPlacaSC((string)Eval("lStrPlacaSC"))) %>'>
                                            <span class="adms-placa-pais" aria-hidden="true">GT</span>
                                            <span class="adms-placa-numero" aria-hidden="true"><%# fncCodificarSC(ClsFormatoSC.fncFormatearPlacaSC((string)Eval("lStrPlacaSC"))) %></span>
                                        </span>
                                        <div class="adms-persona-texto">
                                            <a class="adms-vehiculo-titulo" href='DetalleVehiculo.aspx?id=<%# Eval("lIntIdVehiculoSC") %>'><%# fncCodificarSC(Eval("lStrMarcaSC") + " " + Eval("lStrModeloSC")) %></a>
                                            <div class="adms-persona-detalle">Año <%# Eval("lIntAnioSC") %></div>
                                        </div>
                                    </div>
                                </td>
                                <td class="d-none d-xl-table-cell"><%# ClsFormatoSC.fncVinHtmlSC((string)Eval("lStrVinSC")) %></td>
                                <td class="d-none d-md-table-cell adms-col-propietario">
                                    <a class="adms-persona-nombre fw-medium" href='../Clientes/DetalleCliente.aspx?id=<%# Eval("lIntIdClienteSC") %>'><%# fncCodificarSC(Eval("lObjPropietarioSC.lStrNombresSC") + " " + Eval("lObjPropietarioSC.lStrApellidosSC")) %></a>
                                </td>
                                <td class="adms-numero d-none d-xxl-table-cell text-secondary"><%# fncCodificarSC(ClsFormatoSC.fncFormatearFechaSC((DateTime)Eval("lDatFechaRegistroUtcSC"))) %></td>
                                <td class="text-end">
                                    <a class="btn btn-sm btn-outline-primary text-nowrap" href='DetalleVehiculo.aspx?id=<%# Eval("lIntIdVehiculoSC") %>' title="Ver detalle">
                                        <i class="bi bi-eye" aria-hidden="true"></i><span class="visually-hidden d-xl-none">Ver detalle</span><span class="d-none d-xl-inline ms-1">Ver detalle</span>
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
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-car-front"></i></span>
            <p class="adms-vacio-titulo"><asp:Literal ID="litVacioTituloSC" runat="server" Mode="Encode" /></p>
            <p class="adms-vacio-texto"><asp:Literal ID="litVacioDetalleSC" runat="server" Mode="Encode" /></p>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
