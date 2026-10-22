<%@ Page Title="Detalle del vehículo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DetalleVehiculo.aspx.cs" Inherits="CapaPresentacion.Vehiculos.webDetalleVehiculoSC" ClientIDMode="Static" ValidateRequest="false" %>

<asp:Content ID="cntRuta" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item"><a href="Vehiculos.aspx">Vehículos</a></li>
            <li class="breadcrumb-item active" aria-current="page">Detalle del vehículo</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAcciones" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-light" href="Vehiculos.aspx">
        <i class="bi bi-arrow-left me-2" aria-hidden="true"></i>Volver al listado
    </a>
</asp:Content>

<asp:Content ID="cntContenido" ContentPlaceHolderID="cphContenido" runat="server">
    <asp:Panel ID="pnlVehiculoSC" runat="server" CssClass="row g-4" Visible="false">
        <div class="col-lg-4">
            <div class="adms-card adms-card-cuerpo adms-perfil">
                <span class="adms-id-tarjeta"><span class="visually-hidden">Vehículo </span>#<asp:Literal ID="litIdVehiculoSC" runat="server" Mode="Encode" /></span>
                <span class="adms-placa adms-placa-lg">
                    <span class="adms-placa-pais" aria-hidden="true">GT</span>
                    <span class="adms-placa-numero"><span class="visually-hidden">Placa </span><asp:Literal ID="litPlacaGrandeSC" runat="server" Mode="Encode" /></span>
                </span>
                <h2 class="adms-perfil-nombre"><asp:Literal ID="litMarcaModeloSC" runat="server" Mode="Encode" /></h2>
                <div class="d-flex flex-wrap justify-content-center gap-2">
                    <span class="adms-chip"><i class="bi bi-calendar3" aria-hidden="true"></i>Registrado el <asp:Literal ID="litFechaRegistroSC" runat="server" Mode="Encode" /></span>
                </div>
            </div>
        </div>

        <div class="col-lg-8">
            <div class="adms-card mb-4">
                <div class="adms-card-encabezado">
                    <h3 class="adms-card-titulo"><i class="bi bi-car-front me-2 text-primary" aria-hidden="true"></i>Datos del vehículo</h3>
                </div>
                <div class="adms-card-cuerpo">
                    <dl class="adms-datos">
                        <div>
                            <dt>Placa</dt>
                            <dd class="adms-numero"><asp:Literal ID="litPlacaSC" runat="server" Mode="Encode" /></dd>
                        </div>
                        <div>
                            <dt>VIN</dt>
                            <dd><span class="adms-vin"><asp:Literal ID="litVinSC" runat="server" Mode="Encode" /></span></dd>
                        </div>
                        <div>
                            <dt>Marca</dt>
                            <dd><asp:Literal ID="litMarcaSC" runat="server" Mode="Encode" /></dd>
                        </div>
                        <div>
                            <dt>Modelo</dt>
                            <dd><asp:Literal ID="litModeloSC" runat="server" Mode="Encode" /></dd>
                        </div>
                        <div>
                            <dt>Año</dt>
                            <dd class="adms-numero"><asp:Literal ID="litAnioSC" runat="server" Mode="Encode" /></dd>
                        </div>
                    </dl>
                </div>
            </div>

            <div class="adms-card">
                <div class="adms-card-encabezado">
                    <h3 class="adms-card-titulo"><i class="bi bi-person me-2 text-primary" aria-hidden="true"></i>Propietario</h3>
                    <asp:HyperLink ID="lnkVerClienteSC" runat="server" CssClass="btn btn-sm btn-outline-primary">Ver cliente</asp:HyperLink>
                </div>
                <div class="adms-card-cuerpo">
                    <div class="adms-persona mb-4">
                        <span class="adms-avatar" aria-hidden="true"><asp:Literal ID="litInicialesPropietarioSC" runat="server" Mode="Encode" /></span>
                        <div class="adms-persona-texto">
                            <asp:HyperLink ID="lnkNombrePropietarioSC" runat="server" CssClass="adms-persona-nombre" />
                        </div>
                    </div>
                    <dl class="adms-datos">
                        <div>
                            <dt>DPI</dt>
                            <dd class="adms-numero"><asp:Literal ID="litDpiPropietarioSC" runat="server" Mode="Encode" /></dd>
                        </div>
                        <div>
                            <dt>Teléfono</dt>
                            <dd class="adms-numero"><asp:Literal ID="litTelefonoPropietarioSC" runat="server" Mode="Encode" /></dd>
                        </div>
                        <div>
                            <dt>Correo electrónico</dt>
                            <dd>
                                <asp:Literal ID="litCorreoPropietarioSC" runat="server" Mode="Encode" />
                                <asp:Label ID="lblCorreoVacioSC" runat="server" CssClass="adms-dato-vacio" Text="No registrado" Visible="false" />
                            </dd>
                        </div>
                    </dl>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlNoEncontradoSC" runat="server" CssClass="adms-card" Visible="false">
        <div class="adms-vacio">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-car-front"></i></span>
            <h2 class="adms-vacio-titulo">No encontramos el vehículo solicitado</h2>
            <p class="adms-vacio-texto">Es posible que el enlace esté incompleto o que el vehículo no exista. Búscalo por su placa o VIN.</p>
            <a class="btn btn-primary" href="Vehiculos.aspx">Ir al listado de vehículos</a>
        </div>
    </asp:Panel>
</asp:Content>
