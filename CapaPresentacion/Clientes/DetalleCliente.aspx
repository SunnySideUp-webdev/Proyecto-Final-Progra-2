<%@ Page Title="Detalle del cliente" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DetalleCliente.aspx.cs" Inherits="CapaPresentacion.Clientes.webDetalleClienteSC" ClientIDMode="Static" ValidateRequest="false" %>
<%@ Import Namespace="CapaPresentacion.Utilidades" %>

<asp:Content ID="cntRuta" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item"><a href="Clientes.aspx">Clientes</a></li>
            <li class="breadcrumb-item active" aria-current="page">Detalle del cliente</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAcciones" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-light" href="Clientes.aspx">
        <i class="bi bi-arrow-left me-2" aria-hidden="true"></i>Volver al listado
    </a>
</asp:Content>

<asp:Content ID="cntContenido" ContentPlaceHolderID="cphContenido" runat="server">
    <asp:Panel ID="pnlClienteSC" runat="server" CssClass="row g-4" Visible="false">
        <div class="col-lg-4">
            <div class="adms-card adms-card-cuerpo adms-perfil">
                <span class="adms-id-tarjeta"><span class="visually-hidden">Cliente </span>#<asp:Literal ID="litIdClienteSC" runat="server" Mode="Encode" /></span>
                <span class="adms-avatar adms-avatar-lg" aria-hidden="true"><asp:Literal ID="litInicialesSC" runat="server" Mode="Encode" /></span>
                <h2 class="adms-perfil-nombre"><asp:Literal ID="litNombreCompletoSC" runat="server" Mode="Encode" /></h2>
                <div class="d-flex flex-wrap justify-content-center gap-2">
                    <span class="adms-chip"><i class="bi bi-calendar3" aria-hidden="true"></i>Registrado el <asp:Literal ID="litFechaRegistroSC" runat="server" Mode="Encode" /></span>
                </div>
            </div>
        </div>

        <div class="col-lg-8">
            <div class="adms-card mb-4">
                <div class="adms-card-encabezado">
                    <h3 class="adms-card-titulo"><i class="bi bi-person-vcard me-2 text-primary" aria-hidden="true"></i>Datos personales</h3>
                </div>
                <div class="adms-card-cuerpo">
                    <dl class="adms-datos">
                        <div>
                            <dt>DPI</dt>
                            <dd class="adms-numero"><asp:Literal ID="litDpiSC" runat="server" Mode="Encode" /></dd>
                        </div>
                        <div>
                            <dt>Nombres</dt>
                            <dd><asp:Literal ID="litNombresSC" runat="server" Mode="Encode" /></dd>
                        </div>
                        <div>
                            <dt>Apellidos</dt>
                            <dd><asp:Literal ID="litApellidosSC" runat="server" Mode="Encode" /></dd>
                        </div>
                    </dl>
                </div>
            </div>

            <div class="adms-card">
                <div class="adms-card-encabezado">
                    <h3 class="adms-card-titulo"><i class="bi bi-telephone me-2 text-primary" aria-hidden="true"></i>Contacto</h3>
                </div>
                <div class="adms-card-cuerpo">
                    <dl class="adms-datos">
                        <div>
                            <dt>Teléfono</dt>
                            <dd class="adms-numero"><asp:Literal ID="litTelefonoSC" runat="server" Mode="Encode" /></dd>
                        </div>
                        <div>
                            <dt>Correo electrónico</dt>
                            <dd>
                                <asp:Literal ID="litCorreoSC" runat="server" Mode="Encode" />
                                <asp:Label ID="lblCorreoVacioSC" runat="server" CssClass="adms-dato-vacio" Text="No registrado" Visible="false" />
                            </dd>
                        </div>
                        <div>
                            <dt>Dirección</dt>
                            <dd>
                                <asp:Literal ID="litDireccionSC" runat="server" Mode="Encode" />
                                <asp:Label ID="lblDireccionVaciaSC" runat="server" CssClass="adms-dato-vacio" Text="No registrada" Visible="false" />
                            </dd>
                        </div>
                    </dl>
                </div>
            </div>
        </div>

        <div class="col-12">
            <section class="adms-card adms-card-tabla" aria-labelledby="tituloVehiculosSC">
                <div class="adms-card-encabezado">
                    <div>
                        <h3 class="adms-card-titulo" id="tituloVehiculosSC"><i class="bi bi-car-front me-2 text-primary" aria-hidden="true"></i>Vehículos asociados</h3>
                        <p class="adms-card-subtitulo"><asp:Literal ID="litResumenVehiculosSC" runat="server" Mode="Encode" /></p>
                    </div>
                    <asp:HyperLink ID="lnkRegistrarVehiculoSC" runat="server" CssClass="btn btn-sm btn-primary">
                        <i class="bi bi-plus-lg me-1" aria-hidden="true"></i>Registrar vehículo
                    </asp:HyperLink>
                </div>

                <asp:Repeater ID="rptVehiculosSC" runat="server">
                    <HeaderTemplate>
                        <div class="table-responsive">
                            <table class="table table-hover adms-tabla">
                                <thead>
                                    <tr>
                                        <th scope="col">Vehículo</th>
                                        <th scope="col" class="d-none d-md-table-cell">VIN</th>
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
                                                <span class="adms-placa">
                                                    <span class="adms-placa-pais" aria-hidden="true">GT</span>
                                                    <span class="adms-placa-numero"><span class="visually-hidden">Placa </span><%# fncCodificarSC(ClsFormatoSC.fncFormatearPlacaSC((string)Eval("lStrPlacaSC"))) %></span>
                                                </span>
                                                <div class="adms-persona-texto">
                                                    <a class="adms-vehiculo-titulo" href='../Vehiculos/DetalleVehiculo.aspx?id=<%# Eval("lIntIdVehiculoSC") %>'><%# fncCodificarSC(Eval("lStrMarcaSC") + " " + Eval("lStrModeloSC")) %></a>
                                                    <div class="adms-persona-detalle">Año <%# Eval("lIntAnioSC") %></div>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="d-none d-md-table-cell"><%# ClsFormatoSC.fncVinHtmlSC((string)Eval("lStrVinSC")) %></td>
                                        <td class="adms-numero d-none d-xxl-table-cell text-secondary"><%# fncCodificarSC(ClsFormatoSC.fncFormatearFechaSC((DateTime)Eval("lDatFechaRegistroUtcSC"))) %></td>
                                        <td class="text-end">
                                            <a class="btn btn-sm btn-outline-primary text-nowrap" href='../Vehiculos/DetalleVehiculo.aspx?id=<%# Eval("lIntIdVehiculoSC") %>' title="Ver detalle del vehículo">
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

                <asp:Panel ID="pnlSinVehiculosSC" runat="server" CssClass="adms-vacio" Visible="false">
                    <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-car-front"></i></span>
                    <p class="adms-vacio-titulo">Este cliente todavía no tiene vehículos registrados</p>
                    <p class="adms-vacio-texto">Registra su primer vehículo para que aparezca aquí y pueda usarse en las órdenes de reparación.</p>
                    <asp:HyperLink ID="lnkRegistrarPrimerVehiculoSC" runat="server" CssClass="btn btn-primary">
                        <i class="bi bi-plus-lg me-1" aria-hidden="true"></i>Registrar vehículo
                    </asp:HyperLink>
                </asp:Panel>

                <asp:Panel ID="pnlErrorVehiculosSC" runat="server" CssClass="adms-vacio" Visible="false">
                    <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-exclamation-triangle"></i></span>
                    <p class="adms-vacio-titulo">No fue posible cargar los vehículos de este cliente</p>
                    <p class="adms-vacio-texto">Los datos del cliente se muestran correctamente. Intenta recargar la página en unos minutos.</p>
                </asp:Panel>
            </section>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlNoEncontradoSC" runat="server" CssClass="adms-card" Visible="false">
        <div class="adms-vacio">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-person-x"></i></span>
            <h2 class="adms-vacio-titulo">No encontramos el cliente solicitado</h2>
            <p class="adms-vacio-texto">Es posible que el enlace esté incompleto o que el cliente no exista. Búscalo de nuevo en el listado.</p>
            <a class="btn btn-primary" href="Clientes.aspx">Ir al listado de clientes</a>
        </div>
    </asp:Panel>
</asp:Content>
