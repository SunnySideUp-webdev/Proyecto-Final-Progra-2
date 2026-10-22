<%@ Page Title="Reportes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="CapaPresentacion.Reportes.webReportesFP" ClientIDMode="Static" ValidateRequest="false" %>
<%@ Import Namespace="CapaPresentacion.Utilidades" %>
<%-- Módulo 4: Reportes (Fátima). Solo lectura: no registra ni modifica datos. --%>

<asp:Content ID="cntRutaFP" ContentPlaceHolderID="cphRuta" runat="server">
    <nav aria-label="Ruta de navegación">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="../Default.aspx">Inicio</a></li>
            <li class="breadcrumb-item active" aria-current="page">Reportes</li>
        </ol>
    </nav>
</asp:Content>

<asp:Content ID="cntAccionesFP" ContentPlaceHolderID="cphAcciones" runat="server">
    <button type="button" class="btn btn-light" onclick="window.print();">
        <i class="bi bi-printer me-2" aria-hidden="true"></i>Imprimir
    </button>
</asp:Content>

<asp:Content ID="cntContenidoFP" ContentPlaceHolderID="cphContenido" runat="server">
    <%-- Pestañas: cada una es un enlace con ?seccion=, así cada reporte tiene su propia dirección. --%>
    <nav class="adms-pestanas adms-no-imprimir" aria-label="Secciones de reportes">
        <asp:HyperLink ID="lnkResumenFP" runat="server" NavigateUrl="~/Reportes/Reportes.aspx">
            <i class="bi bi-speedometer2" aria-hidden="true"></i>Resumen
        </asp:HyperLink>
        <asp:HyperLink ID="lnkAbiertasFP" runat="server" NavigateUrl="~/Reportes/Reportes.aspx?seccion=abiertas">
            <i class="bi bi-hourglass-split" aria-hidden="true"></i>Órdenes abiertas
        </asp:HyperLink>
        <asp:HyperLink ID="lnkCerradasFP" runat="server" NavigateUrl="~/Reportes/Reportes.aspx?seccion=cerradas">
            <i class="bi bi-check2-circle" aria-hidden="true"></i>Órdenes cerradas
        </asp:HyperLink>
        <asp:HyperLink ID="lnkHistorialFP" runat="server" NavigateUrl="~/Reportes/Reportes.aspx?seccion=historial">
            <i class="bi bi-person-lines-fill" aria-hidden="true"></i>Historial por cliente
        </asp:HyperLink>
        <asp:HyperLink ID="lnkExistenciasFP" runat="server" NavigateUrl="~/Reportes/Reportes.aspx?seccion=existencias">
            <i class="bi bi-box-seam" aria-hidden="true"></i>Existencias
        </asp:HyperLink>
        <asp:HyperLink ID="lnkRepuestosFP" runat="server" NavigateUrl="~/Reportes/Reportes.aspx?seccion=repuestos">
            <i class="bi bi-tools" aria-hidden="true"></i>Repuestos utilizados
        </asp:HyperLink>
    </nav>

    <%-- ===================== Resumen ===================== --%>
    <asp:Panel ID="pnlResumenFP" runat="server" Visible="false">
        <div class="row g-4 mb-4">
            <div class="col-sm-6 col-xxl-3">
                <div class="adms-card adms-estadistica">
                    <span class="adms-estadistica-icono" aria-hidden="true"><i class="bi bi-hourglass-split"></i></span>
                    <div>
                        <div class="adms-estadistica-valor"><asp:Literal ID="litAbiertasFP" runat="server" Mode="Encode" /></div>
                        <div class="adms-estadistica-etiqueta">Órdenes abiertas</div>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xxl-3">
                <div class="adms-card adms-estadistica">
                    <span class="adms-estadistica-icono" aria-hidden="true"><i class="bi bi-check2-circle"></i></span>
                    <div>
                        <div class="adms-estadistica-valor"><asp:Literal ID="litCerradasMesFP" runat="server" Mode="Encode" /></div>
                        <div class="adms-estadistica-etiqueta">Órdenes cerradas este mes</div>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xxl-3">
                <div class="adms-card adms-estadistica">
                    <span class="adms-estadistica-icono" aria-hidden="true"><i class="bi bi-car-front"></i></span>
                    <div>
                        <div class="adms-estadistica-valor"><asp:Literal ID="litVehiculosFP" runat="server" Mode="Encode" /></div>
                        <div class="adms-estadistica-etiqueta">Vehículos atendidos</div>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xxl-3">
                <div class="adms-card adms-estadistica">
                    <span class="adms-estadistica-icono" aria-hidden="true"><i class="bi bi-stopwatch"></i></span>
                    <div>
                        <div class="adms-estadistica-valor"><asp:Literal ID="litPromedioFP" runat="server" Mode="Encode" /></div>
                        <div class="adms-estadistica-etiqueta">Tiempo promedio de reparación</div>
                    </div>
                </div>
            </div>
        </div>

        <div class="adms-card adms-card-cuerpo">
            <h2 class="adms-card-titulo mb-2">Órdenes que requieren atención</h2>
            <p class="mb-3 text-secondary"><asp:Literal ID="litMasAntiguaFP" runat="server" Mode="Encode" /></p>
            <div class="d-flex flex-wrap gap-2 adms-no-imprimir">
                <a class="btn btn-sm btn-outline-primary" href="Reportes.aspx?seccion=abiertas">Ver órdenes abiertas</a>
                <a class="btn btn-sm btn-outline-primary" href="Reportes.aspx?seccion=cerradas">Ver órdenes cerradas del mes</a>
            </div>
        </div>
    </asp:Panel>

    <%-- ===================== Órdenes abiertas ===================== --%>
    <asp:Panel ID="pnlAbiertasFP" runat="server" CssClass="adms-card adms-card-tabla" Visible="false">
        <div class="adms-card-encabezado">
            <div>
                <h2 class="adms-card-titulo">Órdenes abiertas</h2>
                <p class="adms-card-subtitulo">De la más antigua a la más reciente. Las que llevan una semana o más aparecen resaltadas.</p>
            </div>
            <span class="adms-contador"><asp:Literal ID="litTotalAbiertasFP" runat="server" Mode="Encode" /></span>
        </div>

        <asp:Repeater ID="rptAbiertasFP" runat="server">
            <HeaderTemplate>
                <div class="table-responsive">
                    <table class="table table-hover adms-tabla">
                        <thead>
                            <tr>
                                <th scope="col" class="d-none d-sm-table-cell">Orden</th>
                                <th scope="col">Vehículo</th>
                                <th scope="col" class="d-none d-md-table-cell">Cliente</th>
                                <th scope="col" class="d-none d-lg-table-cell">Apertura</th>
                                <th scope="col">Días abierta</th>
                                <th scope="col" class="d-none d-xxl-table-cell">Motivo</th>
                                <th scope="col" class="text-end adms-no-imprimir"><span class="visually-hidden">Acciones</span></th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                            <tr>
                                <td class="d-none d-sm-table-cell adms-numero fw-semibold">#<%# Eval("gIntIdOrdenFP") %></td>
                                <td>
                                    <div class="adms-vehiculo">
                                        <span class="adms-placa">
                                            <span class="adms-placa-pais" aria-hidden="true">GT</span>
                                            <span class="adms-placa-numero"><span class="visually-hidden">Placa </span><%# FncCodificarFP(ClsFormatoSC.FncFormatearPlacaSC((string)Eval("gStrPlacaFP"))) %></span>
                                        </span>
                                        <span class="adms-vehiculo-titulo d-none d-sm-inline"><%# FncCodificarFP(Eval("gStrMarcaFP") + " " + Eval("gStrModeloFP")) %></span>
                                    </div>
                                </td>
                                <td class="d-none d-md-table-cell adms-col-propietario">
                                    <a class="adms-persona-nombre" href='../Clientes/DetalleCliente.aspx?id=<%# Eval("gIntIdClienteFP") %>'><%# FncCodificarFP(Eval("gStrClienteFP")) %></a>
                                </td>
                                <td class="d-none d-lg-table-cell adms-numero text-secondary"><%# FncCodificarFP(ClsFormatoSC.FncFormatearFechaSC((DateTime)Eval("gDatFechaAperturaUtcFP"))) %></td>
                                <td><span class='<%# FncClaseDiasFP((int)Eval("gIntDiasAbiertaFP")) %>'><%# FncCodificarFP(FncTextoDiasFP((int)Eval("gIntDiasAbiertaFP"))) %></span></td>
                                <td class="d-none d-xxl-table-cell"><%# FncCodificarFP(Eval("gStrMotivoIngresoFP")) %></td>
                                <td class="text-end adms-no-imprimir">
                                    <a class="btn btn-sm btn-outline-primary text-nowrap" href='../OrdenesReparacion/DetalleOrdenNT.aspx?id=<%# Eval("gIntIdOrdenFP") %>' title="Ver la orden">
                                        <i class="bi bi-eye" aria-hidden="true"></i><span class="visually-hidden">Ver la orden</span>
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

        <asp:Panel ID="pnlSinAbiertasFP" runat="server" CssClass="adms-vacio" Visible="false">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-emoji-smile"></i></span>
            <p class="adms-vacio-titulo">No hay órdenes abiertas</p>
            <p class="adms-vacio-texto">Todas las órdenes de reparación están cerradas.</p>
        </asp:Panel>
    </asp:Panel>

    <%-- ===================== Filtro de fechas ===================== --%>
    <%-- Lo comparten «Órdenes cerradas» y «Repuestos utilizados»; «Filtrar» vuelve a la sección actual. --%>
    <asp:Panel ID="pnlFiltroFP" runat="server" CssClass="adms-card mb-4 adms-no-imprimir" Visible="false">
        <div class="adms-card-cuerpo">
            <asp:Panel ID="pnlFiltroFechasFP" runat="server" DefaultButton="btnFiltrarFP" CssClass="row g-3 align-items-start">
                <%-- Campos de fecha HTML (type="date"): el navegador muestra un calendario y envía
                     la fecha como año-mes-día. Se usan inputs normales porque el TextBox con
                     TextMode="Date" no se dibuja bien en Mono. Los valores se codifican al mostrarse. --%>
                <div class="col-sm-6 col-lg-4">
                    <label for="txtDesdeFP" class="form-label"><%= FncCodificarFP(gStrEtiquetaDesdeFP) %></label>
                    <input type="date" id="txtDesdeFP" name="desde" class="<%= gStrClaseDesdeFP %>"
                        value="<%= FncCodificarFP(gStrDesdeFP) %>" aria-describedby="errDesdeFP" />
                    <asp:Label ID="errDesdeFP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                </div>
                <div class="col-sm-6 col-lg-4">
                    <label for="txtHastaFP" class="form-label">Hasta</label>
                    <input type="date" id="txtHastaFP" name="hasta" class="<%= gStrClaseHastaFP %>"
                        value="<%= FncCodificarFP(gStrHastaFP) %>" aria-describedby="errHastaFP" />
                    <asp:Label ID="errHastaFP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                </div>
                <div class="col-lg-auto d-flex gap-2 adms-busqueda-botones">
                    <asp:Button ID="btnFiltrarFP" runat="server" Text="Filtrar" CssClass="btn btn-primary px-4"
                        OnClick="btnFiltrarFP_Click" data-adms-procesando="Filtrando…" />
                    <a class="btn btn-light" href="Reportes.aspx?seccion=<%= gStrSeccionFP %>">Mes actual</a>
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>

    <%-- ===================== Órdenes cerradas ===================== --%>
    <asp:Panel ID="pnlCerradasFP" runat="server" Visible="false">
        <asp:Panel ID="pnlResultadoCerradasFP" runat="server" CssClass="adms-card adms-card-tabla">
            <div class="adms-card-encabezado">
                <div>
                    <h2 class="adms-card-titulo">Órdenes cerradas</h2>
                    <p class="adms-card-subtitulo"><asp:Literal ID="litRangoFP" runat="server" Mode="Encode" /></p>
                </div>
                <span class="adms-contador"><asp:Literal ID="litTotalCerradasFP" runat="server" Mode="Encode" /></span>
            </div>

            <asp:Repeater ID="rptCerradasFP" runat="server">
                <HeaderTemplate>
                    <div class="table-responsive">
                        <table class="table table-hover adms-tabla">
                            <thead>
                                <tr>
                                    <th scope="col" class="d-none d-sm-table-cell">Orden</th>
                                    <th scope="col">Vehículo</th>
                                    <th scope="col" class="d-none d-md-table-cell">Cliente</th>
                                    <th scope="col" class="d-none d-xxl-table-cell">Apertura</th>
                                    <th scope="col" class="d-none d-xxl-table-cell">Cierre</th>
                                    <th scope="col">Duración</th>
                                    <th scope="col" class="text-end adms-no-imprimir"><span class="visually-hidden">Acciones</span></th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                                <tr>
                                    <td class="d-none d-sm-table-cell adms-numero fw-semibold">#<%# Eval("gIntIdOrdenFP") %></td>
                                    <td>
                                        <div class="adms-vehiculo">
                                            <span class="adms-placa">
                                                <span class="adms-placa-pais" aria-hidden="true">GT</span>
                                                <span class="adms-placa-numero"><span class="visually-hidden">Placa </span><%# FncCodificarFP(ClsFormatoSC.FncFormatearPlacaSC((string)Eval("gStrPlacaFP"))) %></span>
                                            </span>
                                            <span class="adms-vehiculo-titulo d-none d-sm-inline"><%# FncCodificarFP(Eval("gStrMarcaFP") + " " + Eval("gStrModeloFP")) %></span>
                                        </div>
                                    </td>
                                    <td class="d-none d-md-table-cell adms-col-propietario">
                                        <a class="adms-persona-nombre" href='../Clientes/DetalleCliente.aspx?id=<%# Eval("gIntIdClienteFP") %>'><%# FncCodificarFP(Eval("gStrClienteFP")) %></a>
                                    </td>
                                    <td class="d-none d-xxl-table-cell adms-numero text-secondary"><%# FncCodificarFP(ClsFormatoSC.FncFormatearFechaSC((DateTime)Eval("gDatFechaAperturaUtcFP"))) %></td>
                                    <td class="d-none d-xxl-table-cell adms-numero"><%# FncCodificarFP(FncFechaOpcionalFP(Eval("gDatFechaCierreUtcFP"))) %></td>
                                    <td class="adms-dias">
                                        <%# FncCodificarFP(FncFormatearDuracionFP((int)Eval("gIntMinutosReparacionFP"))) %>
                                        <%-- En pantallas medianas la columna «Cierre» se oculta: la fecha se muestra aquí. --%>
                                        <div class="adms-persona-detalle d-xxl-none">Cerró el <%# FncCodificarFP(FncFechaOpcionalFP(Eval("gDatFechaCierreUtcFP"))) %></div>
                                    </td>
                                    <td class="text-end adms-no-imprimir">
                                        <a class="btn btn-sm btn-outline-primary text-nowrap" href='../OrdenesReparacion/DetalleOrdenNT.aspx?id=<%# Eval("gIntIdOrdenFP") %>' title="Ver la orden">
                                            <i class="bi bi-eye" aria-hidden="true"></i><span class="visually-hidden">Ver la orden</span>
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

            <asp:Panel ID="pnlSinCerradasFP" runat="server" CssClass="adms-vacio" Visible="false">
                <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-calendar-x"></i></span>
                <p class="adms-vacio-titulo">No se cerraron órdenes en estas fechas</p>
                <p class="adms-vacio-texto">Prueba con un rango de fechas más amplio.</p>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>

    <%-- ===================== Repuestos utilizados ===================== --%>
    <asp:Panel ID="pnlRepuestosFP" runat="server" CssClass="adms-card adms-card-tabla" Visible="false">
        <div class="adms-card-encabezado">
            <div>
                <h2 class="adms-card-titulo">Repuestos utilizados</h2>
                <p class="adms-card-subtitulo"><asp:Literal ID="litRangoRepuestosFP" runat="server" Mode="Encode" /></p>
            </div>
            <span class="adms-contador"><asp:Literal ID="litTotalRepuestosFP" runat="server" Mode="Encode" /></span>
        </div>

        <asp:Repeater ID="rptRepuestosFP" runat="server">
            <HeaderTemplate>
                <div class="table-responsive">
                    <table class="table table-hover adms-tabla">
                        <thead>
                            <tr>
                                <th scope="col">Repuesto</th>
                                <th scope="col">Unidades</th>
                                <th scope="col" class="d-none d-sm-table-cell">Órdenes</th>
                                <th scope="col" class="d-none d-lg-table-cell">Último uso</th>
                                <th scope="col" class="d-none d-md-table-cell">Stock actual</th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                            <tr>
                                <td>
                                    <div class="fw-semibold"><%# FncCodificarFP(Eval("gStrNombreFP")) %></div>
                                    <div class="adms-persona-detalle adms-numero"><%# FncCodificarFP(Eval("gStrCodigoFP")) %></div>
                                </td>
                                <td class="adms-numero fw-semibold"><%# Eval("gIntUnidadesFP") %></td>
                                <td class="d-none d-sm-table-cell adms-numero"><%# Eval("gIntOrdenesFP") %></td>
                                <td class="d-none d-lg-table-cell adms-numero text-secondary"><%# FncCodificarFP(ClsFormatoSC.FncFormatearFechaSC((DateTime)Eval("gDatUltimoUsoUtcFP"))) %></td>
                                <td class="d-none d-md-table-cell adms-numero"><%# Eval("gIntStockActualFP") %></td>
                            </tr>
            </ItemTemplate>
            <FooterTemplate>
                        </tbody>
                    </table>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlSinRepuestosFP" runat="server" CssClass="adms-vacio" Visible="false">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-tools"></i></span>
            <p class="adms-vacio-titulo">No se utilizaron repuestos en estas fechas</p>
            <p class="adms-vacio-texto">Prueba con un rango de fechas más amplio.</p>
        </asp:Panel>
    </asp:Panel>

    <%-- ===================== Existencias ===================== --%>
    <asp:Panel ID="pnlExistenciasFP" runat="server" CssClass="adms-card adms-card-tabla" Visible="false">
        <div class="adms-card-encabezado">
            <div>
                <h2 class="adms-card-titulo">Existencias de repuestos</h2>
                <p class="adms-card-subtitulo"><asp:Literal ID="litResumenExistenciasFP" runat="server" Mode="Encode" /></p>
            </div>
            <span class="adms-contador"><asp:Literal ID="litTotalExistenciasFP" runat="server" Mode="Encode" /></span>
        </div>

        <asp:Repeater ID="rptExistenciasFP" runat="server">
            <HeaderTemplate>
                <div class="table-responsive">
                    <table class="table table-hover adms-tabla">
                        <thead>
                            <tr>
                                <th scope="col">Repuesto</th>
                                <th scope="col" class="d-none d-md-table-cell">Vehículo compatible</th>
                                <th scope="col">Stock</th>
                                <th scope="col" class="d-none d-sm-table-cell">Nivel</th>
                                <th scope="col" class="d-none d-lg-table-cell">Usadas en órdenes</th>
                                <th scope="col" class="text-end adms-no-imprimir"><span class="visually-hidden">Acciones</span></th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                            <tr>
                                <td>
                                    <div class="fw-semibold"><%# FncCodificarFP(Eval("gStrNombreFP")) %></div>
                                    <div class="adms-persona-detalle adms-numero"><%# FncCodificarFP(Eval("gStrCodigoFP")) %></div>
                                </td>
                                <td class="d-none d-md-table-cell"><%# FncCodificarFP(Eval("gStrVehiculoFP")) %></td>
                                <td class="adms-numero fw-semibold">
                                    <%# Eval("gIntStockFP") %>
                                    <%-- En teléfonos la columna «Nivel» se oculta: el nivel se muestra aquí. --%>
                                    <div class="d-sm-none"><span class='<%# FncClaseNivelStockFP((string)Eval("gStrNivelStockFP")) %>'><%# FncCodificarFP(Eval("gStrNivelStockFP")) %></span></div>
                                </td>
                                <td class="d-none d-sm-table-cell"><span class='<%# FncClaseNivelStockFP((string)Eval("gStrNivelStockFP")) %>'><%# FncCodificarFP(Eval("gStrNivelStockFP")) %></span></td>
                                <td class="d-none d-lg-table-cell adms-numero"><%# Eval("gIntUnidadesUsadasFP") %></td>
                                <td class="text-end adms-no-imprimir">
                                    <a class="btn btn-sm btn-outline-primary text-nowrap" href='../Inventario/Inventario.aspx?q=<%# FncCodificarFP(HttpUtility.UrlEncode((string)Eval("gStrCodigoFP"))) %>' title="Ver en el inventario">
                                        <i class="bi bi-box-seam" aria-hidden="true"></i><span class="visually-hidden">Ver en el inventario</span>
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

        <asp:Panel ID="pnlSinExistenciasFP" runat="server" CssClass="adms-vacio" Visible="false">
            <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-box-seam"></i></span>
            <p class="adms-vacio-titulo">Todavía no hay repuestos en el inventario</p>
            <p class="adms-vacio-texto">Cuando se registren repuestos en el módulo de inventario, aparecerán aquí.</p>
        </asp:Panel>
    </asp:Panel>

    <%-- ===================== Historial por cliente ===================== --%>
    <asp:Panel ID="pnlHistorialFP" runat="server" Visible="false">
        <div class="adms-card mb-4 adms-no-imprimir">
            <div class="adms-card-cuerpo">
                <asp:Panel ID="pnlBuscarClienteFP" runat="server" DefaultButton="btnBuscarClienteFP" CssClass="row g-3 align-items-start">
                    <div class="col-lg">
                        <label for="txtClienteFP" class="form-label">Buscar cliente</label>
                        <div class="input-group">
                            <span class="input-group-text"><i class="bi bi-search" aria-hidden="true"></i></span>
                            <asp:TextBox ID="txtClienteFP" runat="server" CssClass="form-control" MaxLength="80"
                                placeholder="DPI, nombre, apellido o teléfono" autocomplete="off" aria-describedby="ayudaClienteFP lblErrorClienteFP" />
                        </div>
                        <div id="ayudaClienteFP" class="form-text">Elige al cliente en los resultados para ver todas las reparaciones de sus vehículos.</div>
                        <asp:Label ID="lblErrorClienteFP" runat="server" CssClass="invalid-feedback d-block" Visible="false" />
                    </div>
                    <div class="col-lg-auto d-flex gap-2 adms-busqueda-botones">
                        <asp:Button ID="btnBuscarClienteFP" runat="server" Text="Buscar" CssClass="btn btn-primary px-4"
                            OnClick="btnBuscarClienteFP_Click" data-adms-procesando="Buscando…" />
                    </div>
                </asp:Panel>
            </div>
        </div>

        <%-- Resultados de la búsqueda de clientes (usa la búsqueda del módulo 1). --%>
        <asp:Panel ID="pnlClientesEncontradosFP" runat="server" CssClass="adms-card mb-4" Visible="false">
            <div class="adms-card-encabezado">
                <h2 class="adms-card-titulo">Clientes encontrados</h2>
                <span class="adms-contador"><asp:Literal ID="litTotalClientesFP" runat="server" Mode="Encode" /></span>
            </div>
            <asp:Repeater ID="rptClientesFP" runat="server">
                <HeaderTemplate><ul class="adms-lista"></HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <div class="adms-persona">
                            <span class="adms-avatar" aria-hidden="true"><%# FncCodificarFP(ClsFormatoSC.FncObtenerInicialesSC((string)Eval("gStrNombresSC"), (string)Eval("gStrApellidosSC"))) %></span>
                            <div class="adms-persona-texto">
                                <a class="adms-persona-nombre" href='Reportes.aspx?seccion=historial&amp;idCliente=<%# Eval("gIntIdClienteSC") %>'><%# FncCodificarFP(Eval("gStrNombresSC") + " " + Eval("gStrApellidosSC")) %></a>
                                <div class="adms-persona-detalle">DPI <%# FncCodificarFP(ClsFormatoSC.FncFormatearDpiSC((string)Eval("gStrDpiSC"))) %></div>
                            </div>
                        </div>
                        <a class="btn btn-sm btn-outline-primary" href='Reportes.aspx?seccion=historial&amp;idCliente=<%# Eval("gIntIdClienteSC") %>'>Ver historial</a>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlSinClientesFP" runat="server" CssClass="adms-vacio" Visible="false">
                <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-person-x"></i></span>
                <p class="adms-vacio-titulo">No se encontraron clientes</p>
                <p class="adms-vacio-texto">Revisa el texto o busca por otro dato, como el DPI o el teléfono.</p>
            </asp:Panel>
        </asp:Panel>

        <%-- Historial del cliente elegido (?idCliente=). --%>
        <asp:Panel ID="pnlHistorialClienteFP" runat="server" CssClass="adms-card adms-card-tabla" Visible="false">
            <div class="adms-card-encabezado">
                <div>
                    <h2 class="adms-card-titulo">Historial de <asp:Literal ID="litClienteFP" runat="server" Mode="Encode" /></h2>
                    <p class="adms-card-subtitulo">Todas las reparaciones de sus vehículos, de la más reciente a la más antigua.</p>
                </div>
                <div class="d-flex align-items-center gap-2">
                    <span class="adms-contador"><asp:Literal ID="litTotalHistorialFP" runat="server" Mode="Encode" /></span>
                    <asp:HyperLink ID="lnkVerClienteFP" runat="server" CssClass="btn btn-sm btn-outline-primary adms-no-imprimir">Ver cliente</asp:HyperLink>
                </div>
            </div>

            <asp:Repeater ID="rptHistorialFP" runat="server">
                <HeaderTemplate>
                    <div class="table-responsive">
                        <table class="table table-hover adms-tabla">
                            <thead>
                                <tr>
                                    <th scope="col" class="d-none d-sm-table-cell">Orden</th>
                                    <th scope="col">Vehículo</th>
                                    <th scope="col">Estado</th>
                                    <th scope="col" class="d-none d-lg-table-cell">Apertura</th>
                                    <th scope="col" class="d-none d-md-table-cell">Tiempo</th>
                                    <th scope="col" class="d-none d-xxl-table-cell">Motivo</th>
                                    <th scope="col" class="text-end adms-no-imprimir"><span class="visually-hidden">Acciones</span></th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                                <tr>
                                    <td class="d-none d-sm-table-cell adms-numero fw-semibold">#<%# Eval("gIntIdOrdenFP") %></td>
                                    <td>
                                        <div class="adms-vehiculo">
                                            <span class="adms-placa">
                                                <span class="adms-placa-pais" aria-hidden="true">GT</span>
                                                <span class="adms-placa-numero"><span class="visually-hidden">Placa </span><%# FncCodificarFP(ClsFormatoSC.FncFormatearPlacaSC((string)Eval("gStrPlacaFP"))) %></span>
                                            </span>
                                            <span class="adms-vehiculo-titulo d-none d-sm-inline"><%# FncCodificarFP(Eval("gStrMarcaFP") + " " + Eval("gStrModeloFP")) %></span>
                                        </div>
                                    </td>
                                    <td><span class='<%# FncClaseEstadoFP((string)Eval("gStrEstadoFP")) %>'><%# FncCodificarFP(Eval("gStrEstadoFP")) %></span></td>
                                    <td class="d-none d-lg-table-cell adms-numero text-secondary"><%# FncCodificarFP(ClsFormatoSC.FncFormatearFechaSC((DateTime)Eval("gDatFechaAperturaUtcFP"))) %></td>
                                    <td class="d-none d-md-table-cell adms-dias"><%# FncCodificarFP(FncTiempoOrdenFP(Container.DataItem)) %></td>
                                    <td class="d-none d-xxl-table-cell"><%# FncCodificarFP(Eval("gStrMotivoIngresoFP")) %></td>
                                    <td class="text-end adms-no-imprimir">
                                        <a class="btn btn-sm btn-outline-primary text-nowrap" href='../OrdenesReparacion/DetalleOrdenNT.aspx?id=<%# Eval("gIntIdOrdenFP") %>' title="Ver la orden">
                                            <i class="bi bi-eye" aria-hidden="true"></i><span class="visually-hidden">Ver la orden</span>
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

            <asp:Panel ID="pnlSinHistorialFP" runat="server" CssClass="adms-vacio" Visible="false">
                <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-clipboard"></i></span>
                <p class="adms-vacio-titulo">Este cliente todavía no tiene reparaciones</p>
                <p class="adms-vacio-texto">Cuando se abra una orden para alguno de sus vehículos, aparecerá aquí.</p>
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlClienteNoEncontradoFP" runat="server" CssClass="adms-card" Visible="false">
            <div class="adms-vacio">
                <span class="adms-vacio-icono" aria-hidden="true"><i class="bi bi-person-x"></i></span>
                <p class="adms-vacio-titulo">No encontramos el cliente solicitado</p>
                <p class="adms-vacio-texto">Es posible que el enlace esté incompleto. Búscalo de nuevo con el buscador.</p>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
