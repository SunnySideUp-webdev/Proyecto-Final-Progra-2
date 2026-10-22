<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CapaPresentacion.webInicioSC" ClientIDMode="Static" %>

<asp:Content ID="cntAcciones" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-primary" href="Clientes/RegistrarCliente.aspx">
        <i class="bi bi-person-plus me-2" aria-hidden="true"></i>Registrar cliente
    </a>
</asp:Content>

<asp:Content ID="cntContenido" ContentPlaceHolderID="cphContenido" runat="server">
    <section class="adms-bienvenida mb-4" aria-labelledby="tituloBienvenidaSC">
        <span class="adms-bienvenida-fecha"><i class="bi bi-calendar3" aria-hidden="true"></i><asp:Literal ID="litFechaHoySC" runat="server" Mode="Encode" /></span>
        <h2 id="tituloBienvenidaSC">Bienvenido a AutoManager DMS</h2>
        <p>Administra la información del taller desde un solo lugar. Usa el menú lateral para entrar a los módulos disponibles.</p>
    </section>

    <asp:Panel ID="pnlResumenSC" runat="server">
        <div class="row g-4 mb-4">
            <div class="col-md-4">
                <div class="adms-card adms-estadistica">
                    <span class="adms-estadistica-icono" aria-hidden="true"><i class="bi bi-people"></i></span>
                    <div>
                        <div class="adms-estadistica-valor"><asp:Literal ID="litTotalClientesSC" runat="server" Mode="Encode" /></div>
                        <div class="adms-estadistica-etiqueta">Clientes registrados</div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="adms-card adms-estadistica">
                    <span class="adms-estadistica-icono" aria-hidden="true"><i class="bi bi-person-plus"></i></span>
                    <div>
                        <div class="adms-estadistica-valor"><asp:Literal ID="litClientesMesSC" runat="server" Mode="Encode" /></div>
                        <div class="adms-estadistica-etiqueta">Clientes nuevos este mes</div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="adms-card adms-estadistica">
                    <span class="adms-estadistica-icono" aria-hidden="true"><i class="bi bi-car-front"></i></span>
                    <div>
                        <div class="adms-estadistica-valor"><asp:Literal ID="litTotalVehiculosSC" runat="server" Mode="Encode" /></div>
                        <div class="adms-estadistica-etiqueta">Vehículos registrados</div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-4">
            <div class="col-xl-8">
                <div class="adms-card h-100">
                    <div class="adms-card-encabezado">
                        <div>
                            <h2 class="adms-card-titulo">Clientes recientes</h2>
                            <p class="adms-card-subtitulo">Los últimos registrados en el sistema.</p>
                        </div>
                        <a class="btn btn-sm btn-outline-primary" href="Clientes/Clientes.aspx">Ver todos</a>
                    </div>

                    <asp:Repeater ID="rptRecientesSC" runat="server">
                        <HeaderTemplate><ul class="adms-lista"></HeaderTemplate>
                        <ItemTemplate>
                            <li>
                                <span class="adms-avatar" aria-hidden="true"><%# fncCodificarSC(CapaPresentacion.Utilidades.ClsFormatoSC.fncObtenerInicialesSC((string)Eval("lStrNombresSC"), (string)Eval("lStrApellidosSC"))) %></span>
                                <div class="adms-persona-texto flex-grow-1">
                                    <a class="adms-persona-nombre" href='Clientes/DetalleCliente.aspx?id=<%# Eval("lIntIdClienteSC") %>'><%# fncCodificarSC(Eval("lStrNombresSC") + " " + Eval("lStrApellidosSC")) %></a>
                                    <div class="adms-persona-detalle adms-numero">DPI <%# fncCodificarSC(CapaPresentacion.Utilidades.ClsFormatoSC.fncFormatearDpiSC((string)Eval("lStrDpiSC"))) %></div>
                                </div>
                                <span class="adms-chip d-none d-sm-inline-flex adms-numero"><%# fncCodificarSC(CapaPresentacion.Utilidades.ClsFormatoSC.fncFormatearFechaSC((DateTime)Eval("lDatFechaRegistroUtcSC"))) %></span>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate></ul></FooterTemplate>
                    </asp:Repeater>

                    <asp:Panel ID="pnlSinClientesSC" runat="server" CssClass="adms-vacio" Visible="false">
                        <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-people"></i></span>
                        <p class="adms-vacio-titulo">Todavía no hay clientes registrados</p>
                        <p class="adms-vacio-texto">Cuando registres el primero aparecerá aquí.</p>
                    </asp:Panel>
                </div>
            </div>

            <div class="col-xl-4">
                <div class="adms-card h-100">
                    <div class="adms-card-encabezado">
                        <h2 class="adms-card-titulo">Accesos directos</h2>
                    </div>
                    <div class="adms-card-cuerpo">
                        <a class="adms-acceso" href="Clientes/RegistrarCliente.aspx">
                            <span class="adms-acceso-icono" aria-hidden="true"><i class="bi bi-person-plus"></i></span>
                            <span>
                                <span class="adms-acceso-titulo">Registrar cliente</span>
                                <span class="adms-acceso-descripcion">Agregar un cliente nuevo</span>
                            </span>
                            <i class="bi bi-chevron-right adms-acceso-flecha" aria-hidden="true"></i>
                        </a>
                        <a class="adms-acceso" href="Vehiculos/RegistrarVehiculo.aspx">
                            <span class="adms-acceso-icono" aria-hidden="true"><i class="bi bi-plus-lg"></i></span>
                            <span>
                                <span class="adms-acceso-titulo">Registrar vehículo</span>
                                <span class="adms-acceso-descripcion">Asociar un vehículo a un cliente</span>
                            </span>
                            <i class="bi bi-chevron-right adms-acceso-flecha" aria-hidden="true"></i>
                        </a>
                        <asp:Repeater ID="rptAccesosSC" runat="server">
                            <ItemTemplate>
                                <a class="adms-acceso" href='<%# ResolveUrl((string)Eval("lStrUrl")) %>'>
                                    <span class="adms-acceso-icono" aria-hidden="true"><i class='<%# "bi " + Eval("lStrIcono") %>'></i></span>
                                    <span>
                                        <span class="adms-acceso-titulo"><%# fncCodificarSC(Eval("lStrTexto")) %></span>
                                        <span class="adms-acceso-descripcion"><%# fncCodificarSC(Eval("lStrDescripcion")) %></span>
                                    </span>
                                    <i class="bi bi-chevron-right adms-acceso-flecha" aria-hidden="true"></i>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
