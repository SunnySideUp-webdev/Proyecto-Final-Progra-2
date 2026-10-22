<%@ Page Title="Registrar repuesto" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistroRepuesto.aspx.cs" Inherits="CapaPresentacion.Inventario.webRegistroRepuestoPP" ClientIDMode="Static" ValidateRequest="false" %>
<%@ Import Namespace="CapaLogica" %>
<%-- Módulo 3: Inventario de repuestos (Pavel). Formulario de registro de un repuesto nuevo. --%>

<asp:Content ID="cntRutaPP" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item"><a href="Inventario.aspx">Inventario</a></li>
            <li class="breadcrumb-item active" aria-current="page">Registrar repuesto</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAccionesPP" ContentPlaceHolderID="cphAcciones" runat="server">
    <a class="btn btn-light" href="Inventario.aspx">
        <i class="bi bi-arrow-left me-2" aria-hidden="true"></i>Volver al inventario
    </a>
</asp:Content>

<asp:Content ID="cntContenidoPP" ContentPlaceHolderID="cphContenido" runat="server">
    <div class="row justify-content-center">
        <div class="col-xl-10 col-xxl-9">
            <div class="adms-card">
                <div class="adms-card-encabezado">
                    <div>
                        <h2 class="adms-card-titulo">Datos del nuevo repuesto</h2>
                        <p class="adms-card-subtitulo">Los campos marcados con <span class="adms-requerido">*</span> son obligatorios.</p>
                    </div>
                </div>

                <div class="adms-card-cuerpo">
                    <section class="adms-seccion-form" aria-labelledby="tituloInformacionPP">
                        <h3 class="adms-seccion-titulo" id="tituloInformacionPP">
                            <i class="bi bi-box-seam" aria-hidden="true"></i>Información del repuesto
                        </h3>
                        <div class="row g-3">
                            <div class="col-md-4">
                                <label for="txtCodigoPP" class="form-label">Código <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtCodigoPP" runat="server" CssClass="form-control text-uppercase adms-numero" MaxLength="20"
                                    autocomplete="off" required="required" aria-describedby="ayudaCodigoPP errCodigoPP" />
                                <div id="ayudaCodigoPP" class="form-text">Hasta 15 letras, números y guiones, por ejemplo FIL-ACE-001.</div>
                                <asp:Label ID="errCodigoPP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-8">
                                <label for="txtNombrePP" class="form-label">Nombre <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtNombrePP" runat="server" CssClass="form-control" MaxLength="50"
                                    autocomplete="off" required="required" placeholder="Por ejemplo, Pastillas de freno delanteras" aria-describedby="errNombrePP" />
                                <asp:Label ID="errNombrePP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-8">
                                <label for="txtDescripcionPP" class="form-label">Descripción <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtDescripcionPP" runat="server" CssClass="form-control" MaxLength="100"
                                    autocomplete="off" required="required" placeholder="Por ejemplo, Juego de cuatro pastillas cerámicas" aria-describedby="errDescripcionPP" />
                                <asp:Label ID="errDescripcionPP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-4">
                                <label for="txtStockPP" class="form-label">Stock inicial</label>
                                <asp:TextBox ID="txtStockPP" runat="server" CssClass="form-control adms-numero" MaxLength="5"
                                    inputmode="numeric" autocomplete="off" placeholder="0" aria-describedby="ayudaStockPP errStockPP" />
                                <div id="ayudaStockPP" class="form-text">Unidades en bodega. Si lo dejas vacío, se registra con 0.</div>
                                <asp:Label ID="errStockPP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                        </div>
                    </section>

                    <section class="adms-seccion-form" aria-labelledby="tituloCompatibilidadPP">
                        <h3 class="adms-seccion-titulo" id="tituloCompatibilidadPP">
                            <i class="bi bi-car-front" aria-hidden="true"></i>Compatibilidad con el automóvil
                        </h3>
                        <div class="row g-3">
                            <div class="col-md-5">
                                <label for="txtMarcaPP" class="form-label">Marca <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtMarcaPP" runat="server" CssClass="form-control" MaxLength="50"
                                    autocomplete="off" required="required" placeholder="Por ejemplo, Honda" aria-describedby="errMarcaPP" />
                                <asp:Label ID="errMarcaPP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-4">
                                <label for="txtModeloPP" class="form-label">Modelo <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtModeloPP" runat="server" CssClass="form-control" MaxLength="50"
                                    autocomplete="off" required="required" placeholder="Por ejemplo, Civic" aria-describedby="errModeloPP" />
                                <asp:Label ID="errModeloPP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                            <div class="col-md-3">
                                <label for="txtAnioPP" class="form-label">Año <span class="adms-requerido" aria-hidden="true">*</span></label>
                                <asp:TextBox ID="txtAnioPP" runat="server" CssClass="form-control adms-numero" MaxLength="4"
                                    inputmode="numeric" autocomplete="off" required="required" aria-describedby="ayudaAnioPP errAnioPP" />
                                <div id="ayudaAnioPP" class="form-text"><asp:Literal ID="litAyudaAnioPP" runat="server" Mode="Encode" /></div>
                                <asp:Label ID="errAnioPP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                            </div>
                        </div>
                    </section>
                </div>

                <div class="adms-card-pie">
                    <a class="btn btn-light" href="Inventario.aspx">Cancelar</a>
                    <asp:Button ID="btnGuardarPP" runat="server" Text="Guardar repuesto" CssClass="btn btn-primary px-4"
                        OnClick="btnGuardarPP_Click" data-adms-procesando="Guardando…" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
