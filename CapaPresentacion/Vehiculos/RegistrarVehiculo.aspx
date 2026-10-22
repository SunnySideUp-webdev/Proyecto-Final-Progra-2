<%@ Page Title="Registrar vehículo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistrarVehiculo.aspx.cs" Inherits="CapaPresentacion.Vehiculos.webRegistrarVehiculoSC" ClientIDMode="Static" ValidateRequest="false" %>
<%@ Import Namespace="CapaPresentacion.Utilidades" %>

<asp:Content ID="cntRuta" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item"><a href="Vehiculos.aspx">Vehículos</a></li>
            <li class="breadcrumb-item active" aria-current="page">Registrar vehículo</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAcciones" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-light" href="Vehiculos.aspx">
        <i class="bi bi-arrow-left me-2" aria-hidden="true"></i>Volver al listado
    </a>
</asp:Content>

<asp:Content ID="cntContenido" ContentPlaceHolderID="cphContenido" runat="server">
    <div class="row justify-content-center">
        <div class="col-xl-10 col-xxl-9">
            <div class="adms-card">
                <div class="adms-card-encabezado">
                    <div>
                        <h2 class="adms-card-titulo">Datos del nuevo vehículo</h2>
                        <p class="adms-card-subtitulo">Los campos marcados con <span class="adms-requerido">*</span> son obligatorios.</p>
                    </div>
                </div>

                <div class="adms-card-cuerpo">
                    <section class="adms-seccion-form" aria-labelledby="tituloPropietarioSC">
                        <h3 class="adms-seccion-titulo" id="tituloPropietarioSC">
                            <i class="bi bi-person" aria-hidden="true"></i>Propietario <span class="adms-requerido" aria-hidden="true">*</span>
                        </h3>

                        <asp:Panel ID="pnlPropietarioSeleccionadoSC" runat="server" CssClass="adms-seleccion" Visible="false">
                            <span class="adms-avatar" aria-hidden="true"><asp:Literal ID="litInicialesPropietarioSC" runat="server" Mode="Encode" /></span>
                            <div class="adms-persona-texto flex-grow-1">
                                <div class="fw-semibold"><asp:Literal ID="litNombrePropietarioSC" runat="server" Mode="Encode" /></div>
                                <div class="adms-persona-detalle adms-numero">DPI <asp:Literal ID="litDpiPropietarioSC" runat="server" Mode="Encode" /> · Tel. <asp:Literal ID="litTelefonoPropietarioSC" runat="server" Mode="Encode" /></div>
                            </div>
                            <asp:LinkButton ID="lnkCambiarPropietarioSC" runat="server" CssClass="btn btn-sm btn-light" OnClick="lnkCambiarPropietarioSC_Click" CausesValidation="false">
                                <i class="bi bi-arrow-repeat me-1" aria-hidden="true"></i>Cambiar
                            </asp:LinkButton>
                        </asp:Panel>

                        <asp:Panel ID="pnlBuscarPropietarioSC" runat="server" DefaultButton="btnBuscarPropietarioSC">
                            <label for="txtBuscarPropietarioSC" class="form-label">Buscar cliente</label>
                            <div class="d-flex flex-wrap gap-2">
                                <div class="input-group flex-grow-1 w-auto">
                                    <span class="input-group-text"><i class="bi bi-search" aria-hidden="true"></i></span>
                                    <asp:TextBox ID="txtBuscarPropietarioSC" runat="server" CssClass="form-control" MaxLength="80"
                                        placeholder="DPI, nombre, apellido o teléfono" autocomplete="off" aria-describedby="ayudaPropietarioSC errPropietarioSC" />
                                </div>
                                <asp:Button ID="btnBuscarPropietarioSC" runat="server" Text="Buscar" CssClass="btn btn-outline-primary px-4"
                                    OnClick="btnBuscarPropietarioSC_Click" formnovalidate="formnovalidate" data-adms-procesando="Buscando…" />
                            </div>
                            <div id="ayudaPropietarioSC" class="form-text">
                                Busca al cliente y pulsa «Seleccionar». ¿Es un cliente nuevo? <a href="../Clientes/RegistrarCliente.aspx">Regístralo primero</a>.
                            </div>

                            <asp:Panel ID="pnlResultadosPropietarioSC" runat="server" CssClass="adms-resultados" Visible="false">
                                <asp:Repeater ID="rptPropietariosSC" runat="server" OnItemCommand="rptPropietariosSC_ItemCommand" ClientIDMode="Predictable">
                                    <HeaderTemplate><ul class="adms-lista"></HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <span class="adms-avatar" aria-hidden="true"><%# fncCodificarSC(ClsFormatoSC.fncObtenerInicialesSC((string)Eval("lStrNombresSC"), (string)Eval("lStrApellidosSC"))) %></span>
                                            <div class="adms-persona-texto flex-grow-1">
                                                <div class="fw-semibold"><%# fncCodificarSC(Eval("lStrNombresSC") + " " + Eval("lStrApellidosSC")) %></div>
                                                <div class="adms-persona-detalle adms-numero">DPI <%# fncCodificarSC(ClsFormatoSC.fncFormatearDpiSC((string)Eval("lStrDpiSC"))) %></div>
                                            </div>
                                            <asp:LinkButton ID="lnkSeleccionarSC" runat="server" CssClass="btn btn-sm btn-primary" CommandName="Seleccionar"
                                                CommandArgument='<%# Eval("lIntIdClienteSC") %>' CausesValidation="false">Seleccionar</asp:LinkButton>
                                        </li>
                                    </ItemTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                </asp:Repeater>
                                <asp:Panel ID="pnlSinPropietariosSC" runat="server" CssClass="p-3 text-secondary" Visible="false">
                                    No se encontraron clientes con ese dato. Revisa lo escrito o
                                    <a href="../Clientes/RegistrarCliente.aspx">registra al cliente</a>.
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>

                        <asp:Label ID="errPropietarioSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                    </section>

                    <section class="adms-seccion-form" aria-labelledby="tituloIdentificacionVehiculoSC">
                        <h3 class="adms-seccion-titulo" id="tituloIdentificacionVehiculoSC">
                            <i class="bi bi-upc-scan" aria-hidden="true"></i>Identificación del vehículo
                        </h3>
                        <div class="row g-3">
                            <div class="col-md-5">
                                <label for="txtPlacaSC" class="form-label">Placa <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtPlacaSC" runat="server" CssClass="form-control text-uppercase" MaxLength="14"
                                    autocomplete="off" required="required" aria-describedby="ayudaPlacaSC errPlacaSC" />
                                <div id="ayudaPlacaSC" class="form-text">5 a 10 letras o números, por ejemplo P-123ABC.</div>
                                <asp:Label ID="errPlacaSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-7">
                                <label for="txtVinSC" class="form-label">VIN <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtVinSC" runat="server" CssClass="form-control text-uppercase adms-vin" MaxLength="20"
                                    autocomplete="off" required="required" aria-describedby="ayudaVinSC errVinSC" />
                                <div id="ayudaVinSC" class="form-text">17 caracteres, sin las letras I, O ni Q. Está en la tarjeta de circulación.</div>
                                <asp:Label ID="errVinSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                        </div>
                    </section>

                    <section class="adms-seccion-form" aria-labelledby="tituloDatosVehiculoSC">
                        <h3 class="adms-seccion-titulo" id="tituloDatosVehiculoSC">
                            <i class="bi bi-car-front" aria-hidden="true"></i>Datos del vehículo
                        </h3>
                        <div class="row g-3">
                            <div class="col-md-5">
                                <label for="txtMarcaSC" class="form-label">Marca <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtMarcaSC" runat="server" CssClass="form-control" MaxLength="50"
                                    autocomplete="off" required="required" placeholder="Por ejemplo, Toyota" aria-describedby="errMarcaSC" />
                                <asp:Label ID="errMarcaSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-4">
                                <label for="txtModeloSC" class="form-label">Modelo <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtModeloSC" runat="server" CssClass="form-control" MaxLength="50"
                                    autocomplete="off" required="required" placeholder="Por ejemplo, Corolla" aria-describedby="errModeloSC" />
                                <asp:Label ID="errModeloSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-3">
                                <label for="txtAnioSC" class="form-label">Año <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtAnioSC" runat="server" CssClass="form-control adms-numero" MaxLength="4"
                                    inputmode="numeric" autocomplete="off" required="required" aria-describedby="ayudaAnioSC errAnioSC" />
                                <div id="ayudaAnioSC" class="form-text"><asp:Literal ID="litAyudaAnioSC" runat="server" Mode="Encode" /></div>
                                <asp:Label ID="errAnioSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                        </div>
                    </section>
                </div>

                <div class="adms-card-pie">
                    <a class="btn btn-light" href="Vehiculos.aspx">Cancelar</a>
                    <asp:Button ID="btnGuardarSC" runat="server" Text="Guardar vehículo" CssClass="btn btn-primary px-4"
                        OnClick="btnGuardarSC_Click" data-adms-procesando="Guardando…" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
