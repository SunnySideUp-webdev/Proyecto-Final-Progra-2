<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inventario.aspx.cs" Inherits="Inventario.Inventario" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AutoManager DMS - Repuestos</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js"></script>
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Arial, sans-serif;
            background-color: #f8fafc;
            color: #334155;
        }
        .sidebar {
            width: 260px;
            height: 100vh;
            position: fixed;
            top: 0;
            left: 0;
            background-color: #1e40af;
            color: white;
            padding: 20px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }
        .main-content {
            margin-left: 260px;
            padding: 30px;
            min-height: 100vh;
        }
        .nav-menu-link {
            color: #bfdbfe;
            text-decoration: none;
            padding: 12px 15px;
            display: block;
            border-radius: 8px;
            margin-bottom: 5px;
            transition: all 0.3s;
        }
        .nav-menu-link.active, .nav-menu-link:hover {
            background-color: #1d4ed8;
            color: white;
            font-weight: bold;
        }
        .card-custom {
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.05);
            background-color: white;
            margin-bottom: 24px;
        }
        .card-custom-header {
            padding: 15px 20px;
            border-bottom: 1px solid #e2e8f0;
            font-weight: bold;
            font-size: 16px;
            color: #0f172a;
        }
        .card-custom-body {
            padding: 20px;
        }
        .avatar-repuesto {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            background-color: #dbeafe;
            color: #2563eb;
            font-size: 28px;
            font-weight: bold;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 15px auto;
        }
    </style>
</head>
<body>
    <form id="InicioRepuestos" runat="server">
        <!--creacion del sidebar lateral con las opciones-->
        <div class="sidebar shadow">
            <div class="d-flex flex-column">
                <div class="d-flex align-items-center mb-4">
                    <div class="rounded-3 d-flex align-items-center justify-content-center me-3" 
                         style="width: 44px; height: 44px; background-color: rgba(255, 255, 255, 0.15); border: 1px solid rgba(255, 255, 255, 0.25);">
                        <span class="fs-4" style="opacity: 0.85;">🔧</span>
                    </div>
                    <div class="d-flex flex-column justify-content-center">
                        <span class="fs-4 fw-bold">AutoManager</span>
                        <span class="text-white-50 text-uppercase fw-bold" style="font-size: 11px; letter-spacing: 0.5px;">DMS</span>
                    </div>
                </div>

                <p class="text-uppercase text-white-50 small fw-bold mb-2">Modulos</p>
                <nav class="nav flex-column">
                    <a class="nav-menu-link" href="#"><span class="me-2">📊</span>Inicio</a>
                    <a class="nav-menu-link" href="#"><span class="me-2">👥</span>Clientes</a>
                    <a class="nav-menu-link active" href="#"><span class="me-2">⚙️</span>Repuestos</a>
                </nav>
            </div>
            <!--creacion del footer del sidebar con datos-->
            <div class="text-white-50 small border-top pt-3">
                <p class="mb-0 fw-semibold">Sistema de Gestion de taller mecanico</p>
                <p class="mb-0 text-muted">Proyecto Final - Programación II</p>
            </div>
        </div>

        <!--creacion del registro de repuestos-->
        <div class="main-content">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <small class="text-muted">Inicio/Repuestos</small>
                    <h1 class="h2 fw-bold text-dark m-0" style="font-size: 32px; letter-spacing: -0.5px;">Repuestos</h1>
                </div>
            
                <asp:Button ID="btnregistrarrepuesto" Text="⚙️+ Registrar Respuesto" cssclass="btn btn-primary fw-semibold px-4 shadow-sm" runat="server" OnClick="btnRegistrarcliente_click"/>
                <asp:Label id="lblmensaje" runat="server" CssClass="alert alert-info d-block mt-3 p-2 text-center" visible="false"/>
            </div>

            <div class="row g-4">
                <div class="col-12">
                    <div class="card card-custom">
                        <div class="card-custom-header">🔍 Buscar Repuesto</div>
                        <div class="card-custom-body">
                            <div class="d-flex gap-2 mb-3">
                                <asp:TextBox id="txtbuscar" runat="server" cssclass="form-control" placeholder="nombre o codigo..." />
                                <asp:Button id="btnbuscar" Text="Buscar" runat="server" cssclass="btn btn-primary" OnClick="btnbuscar_Click"/>
                            </div>
                        </div>
                    </div>

                    <div class="card card-custom mt-4">
                        <div class="card-custom-header py-3 px-4 bg-white border-buttom d-flex justify-content-between align-items-center">
                            <div>
                                <h5 class="mb-0 fw-bold text-dark fs-5">Respuestos Registrados Recientemente</h5>
                                <small class="text-muted">Ordenados del mas reciente al mas antiguo</small>
                            </div>
                            <span class="badge bg-primary-subtle text-primary border border-primary-subtle px-3 py-2 rounded-pill fw-semibold">
                                <asp:Label ID="lbltotalregistro" Text="0 repuestos registrados" runat="server" />
                            </span>
                        </div>

                        <div class="card-custom-body p-0">
                            <div class="table-responsive">
                                <table class="table table-hover align-middle m-0" style="border-collapse: collapse; min-width: 800px;">
                                    <thead class="bg-light border-bottom text-uppercase text-muted" style="font-size: 12px; font-weight: bold; letter-spacing: 0.5px;">
                                        <tr>
                                            <th class="ps-4 py-3" style="width: 40%;">Repuesto</th>
                                            <th class="py-3" style="width: 15%;">Código</th>
                                            <th class="py-3" style="width: 15%;">descripcion</th>
                                            <th class="py-3" style="width: 15%;">stock</th>
                                            <th class="pe-4 py-3 text-end" style="width: 15%;">Acción</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater runat="server" ID="rptrepuesto">
                                            <ItemTemplate>
                                                <tr class="border-bottom">
                                                     <td class="ps-4 py-3">
                                                        <div class="d-flex align-items-center">
                                                            <!-- Círculo con Iniciales dinámicas extraídas del DataTable -->
                                                            <div class="rounded-circle bg-primary bg-opacity-10 text-primary fw-bold d-flex align-items-center justify-content-center me-3" 
                                                                 style="width: 40px; height: 40px; font-size: 14px; min-width: 40px;">
                                                                <%# Eval("nombre").ToString().Substring(0, Math.Min(2, Eval("nombre").ToString().Length)).ToUpper() %>
                                                            </div>
                                                            <div>
                                                                <div class="fw-bold text-dark"><%# Eval("nombre") %></div>
                                                                <small class="text-muted d-block"><%# Eval("marcacarro") %> <%# Eval("modelocarro") %> (<%# Eval("aniocarro") %>)</small>
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <!-- Columna 2: Código -->
                                                    <td class="py-3 fw-semibold text-dark"><%# Eval("codigo") %></td>
                                                    <!-- Columna 3: Descripción -->
                                                    <td class="py-3 text-secondary" style="max-width: 200px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">
                                                        <%# Eval("descripcion") %>
                                                    </td>
                                                    <!-- Columna 4: Stock -->
                                                    <td class="py-3">
                                                        <div class="text-dark fw-medium"><%# Eval("stock") %> uds.</div>
                                                    </td>
                                                    <td class="pe-4 py-3 text-end">
                                                        <button type="button" 
                                                             class="btn btn-outline-secondary btn-sm rounded-3 fw-semibold px-3 py-1 border-secondary-subtle text-dark btn-abrir-modal" 
                                                             style="font-size: 13px;"
                                                             data-bs-toggle="modal" 
                                                             data-bs-target="#modalStock"
                                                             data-codigo='<%# Eval("codigo") %>'
                                                             data-nombre='<%# Eval("nombre") %>'>
                                                             Actualizar Stock
                                                        </button>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <!--ventana para actualizar el stock-->
                        <div class="modal fade" id="modalStock" data-bs-backdrop="static" tabindex="-1" aria-labelledby="modalStockLabel" aria-hidden="true">
                            <div class="modal-dialog modal-dialog-centered">
                                <div class="modal-content border-0 shadow-lg" style="border-radius: 12px;">
                                    <!--encabezado de la ventana-->
                                    <div class="modal-header bg-warning text-dark" style="border-top-left-radius: 12px; border-top-right-radius: 12px;">
                                        <h5 class="modal-title fw-bold" id="modalStockLabel">Actualizar Stock</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <!--cuerpo de la ventana-->
                                    <div class="modal-body p-4">
                                        <p class="text-muted small mb-3">Modifica las unidades que hay disponible en bodega</p>
                                        <asp:hiddenfield ID="hndcodigorepuesto" runat="server"></asp:hiddenfield>
                                        <div class="mb-3">
                                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Repuesto Seleccionado" runat="server" />
                                            <asp:TextBox id="txtnombrerepuesto" runat="server" CssClass="form-control bg-light text-dark fw-semibold"/>
                                        </div>
                                        <div class="mb-3">
                                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Nuevo Stock" runat="server" />
                                            <asp:TextBox id="txtnuevostock" runat="server" CssClass="form-control bg-light text-dark fw-semibold" />
                                        </div>
                                    </div>
                                    <!--footer de la ventana-->
                                    <div class="modal-footer bg-light border-top p-3" style="border-bottom-left-radius: 12px; border-bottom-right-radius: 12px;">
                                        <button type="button" class="btn btn-outline-secondary fw-semibold px-3" data-bs-dismiss="modal">Cancelar</button>
                                        <asp:Button ID="btnconfirmarstock" Text="Guardar Cambios" CssClass="btn btn-warning fw-bold px-4 text-dark shadow-sm" runat="server" OnClick="btnconfirmarstock_Click"/>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            // Buscamos todos los botones que tengan la clase que creamos
            const botones = document.querySelectorAll('.btn-abrir-modal');

            botones.forEach(boton => {
                boton.addEventListener('click', function () {
                    // Extraemos los datos del repuesto directamente del botón HTML
                    const codigo = this.getAttribute('data-codigo');
                    const nombre = this.getAttribute('data-nombre');

                    // Los inyectamos en los controles del modal usando sus ClientID nativos
                    // NOTA: Usamos ID terminado en 'hndcodigorepuesto' para evitar problemas de nombres de ASP.NET
                    document.querySelector('[id$="hndcodigorepuesto"]').value = codigo;
                    document.querySelector('[id$="txtnombrerepuesto"]').value = nombre;
                    document.querySelector('[id$="txtnuevostock"]').value = "";
                });
            });
        });
    </script>
</body>
</html>
