<%@ Page Title="Registrar cliente" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistrarCliente.aspx.cs" Inherits="CapaPresentacion.Clientes.webRegistrarClienteSC" ClientIDMode="Static" ValidateRequest="false" %>

<asp:Content ID="cntRuta" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item"><a href="Clientes.aspx">Clientes</a></li>
            <li class="breadcrumb-item active" aria-current="page">Registrar cliente</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAcciones" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-light" href="Clientes.aspx">
        <i class="bi bi-arrow-left me-2" aria-hidden="true"></i>Volver al listado
    </a>
</asp:Content>

<asp:Content ID="cntContenido" ContentPlaceHolderID="cphContenido" runat="server">
    <div class="row justify-content-center">
        <div class="col-xl-10 col-xxl-9">
            <div class="adms-card">
                <div class="adms-card-encabezado">
                    <div>
                        <h2 class="adms-card-titulo">Datos del nuevo cliente</h2>
                        <p class="adms-card-subtitulo">Los campos marcados con <span class="adms-requerido">*</span> son obligatorios.</p>
                    </div>
                </div>

                <div class="adms-card-cuerpo">
                    <section class="adms-seccion-form" aria-labelledby="tituloIdentificacionSC">
                        <h3 class="adms-seccion-titulo" id="tituloIdentificacionSC">
                            <i class="bi bi-person-vcard" aria-hidden="true"></i>Identificación
                        </h3>
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label for="txtDpiSC" class="form-label">DPI <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtDpiSC" runat="server" CssClass="form-control" MaxLength="20"
                                    inputmode="numeric" autocomplete="off" required="required" aria-describedby="ayudaDpiSC errDpiSC" />
                                <div id="ayudaDpiSC" class="form-text">13 dígitos. Puedes escribirlo con espacios o guiones.</div>
                                <asp:Label ID="errDpiSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                        </div>
                    </section>

                    <section class="adms-seccion-form" aria-labelledby="tituloPersonalesSC">
                        <h3 class="adms-seccion-titulo" id="tituloPersonalesSC">
                            <i class="bi bi-person" aria-hidden="true"></i>Datos personales
                        </h3>
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label for="txtNombresSC" class="form-label">Nombres <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtNombresSC" runat="server" CssClass="form-control" MaxLength="80"
                                    autocomplete="given-name" required="required" aria-describedby="errNombresSC" />
                                <asp:Label ID="errNombresSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-6">
                                <label for="txtApellidosSC" class="form-label">Apellidos <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtApellidosSC" runat="server" CssClass="form-control" MaxLength="80"
                                    autocomplete="family-name" required="required" aria-describedby="errApellidosSC" />
                                <asp:Label ID="errApellidosSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                        </div>
                    </section>

                    <section class="adms-seccion-form" aria-labelledby="tituloContactoSC">
                        <h3 class="adms-seccion-titulo" id="tituloContactoSC">
                            <i class="bi bi-telephone" aria-hidden="true"></i>Contacto
                        </h3>
                        <div class="row g-3">
                            <div class="col-md-5">
                                <label for="txtTelefonoSC" class="form-label">Teléfono <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtTelefonoSC" runat="server" CssClass="form-control" MaxLength="20"
                                    inputmode="tel" autocomplete="tel" required="required" aria-describedby="ayudaTelefonoSC errTelefonoSC" />
                                <div id="ayudaTelefonoSC" class="form-text">Entre 8 y 15 dígitos, por ejemplo 5555-1234.</div>
                                <asp:Label ID="errTelefonoSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-7">
                                <label for="txtCorreoSC" class="form-label">Correo electrónico <span class="text-secondary fw-normal">(opcional)</span></label>
                                <asp:TextBox ID="txtCorreoSC" runat="server" CssClass="form-control" MaxLength="254"
                                    inputmode="email" autocomplete="email" aria-describedby="errCorreoSC" />
                                <asp:Label ID="errCorreoSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-12">
                                <label for="txtDireccionSC" class="form-label">Dirección <span class="text-secondary fw-normal">(opcional)</span></label>
                                <asp:TextBox ID="txtDireccionSC" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"
                                    maxlength="200" autocomplete="street-address" aria-describedby="errDireccionSC" />
                                <asp:Label ID="errDireccionSC" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                        </div>
                    </section>
                </div>

                <div class="adms-card-pie">
                    <a class="btn btn-light" href="Clientes.aspx">Cancelar</a>
                    <asp:Button ID="btnGuardarSC" runat="server" Text="Guardar cliente" CssClass="btn btn-primary px-4"
                        OnClick="btnGuardarSC_Click" data-adms-procesando="Guardando…" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
