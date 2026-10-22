<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistroRepuesto.aspx.cs" Inherits="Inventario.RegistroRepuesto" %>

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
    <form id="ReistroRepuestos" runat="server">
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
        <!--contenido principal-->
        <div class="main-content">
            <!--encabezado-->
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <small class="text-muted">Inicio/Repuestos/Registrar Repuesto</small>
                    <h1 class="h2 fw-bold text-dark m-0" style="font-size: 32px; letter-spacing: -0.5px;">Registrar Repuestos</h1>
                </div>

                <asp:Button ID="btnregistrarrepuesto" Text=" Regresar a Inicio" cssclass="btn btn-primary fw-semibold px-4 shadow-sm" runat="server" OnClick="btnRegresarprincipal_click"/>
            </div>
            <asp:Label id="lblmensaje" runat="server" CssClass="alert alert-info d-block mt-3 p-2 text-center" visible="false"/>
            <!--cuerpo-->
            <div class="card card-custom shadow-sm">
                <div class="card-body p-4">
                    <!--formulario para el ingreso del repuesto-->
                    <h6 class="text-primary fw-bold mb-3">Informacion del Repuesto</h6>
                    <div class="row g-3 mb-4">
                        <div class="col-md-4">
                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Codigo de Respuesto" runat="server" />
                            <asp:TextBox runat="server" id="txtcodigo" cssclass="form-control" placeholder="Ej. 020FRENO234"/>
                        </div>
                        <div class="col-md-8">
                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Nombre de Respuesto" runat="server" />
                            <asp:TextBox runat="server" id="txtnomrepuesto" cssclass="form-control" placeholder="Ej. pastilla de freno"/>
                        </div>
                        <div class="col-md-12">
                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Descripcion de Respuesto" runat="server" />
                            <asp:TextBox runat="server" id="txtdescrepuesto" cssclass="form-control" placeholder="Ej. 020FRENO234"/>
                        </div>
                        <div class="col-md-4">
                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Stock" runat="server" />
                            <asp:TextBox runat="server" id="txtstockrepuesto" cssclass="form-control" placeholder="Ej. 12"/>
                        </div>
                   </div>
                    <!--ingreso para la compativilidad con el automovil-->
                    <h6 class="text-primary fw-bold mb-3">Compatibilidad con el Automovil</h6>
                    <div class="row g-3 mb-4">
                        <div class="col-md-4">
                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Marca Compatible" runat="server" />
                            <asp:TextBox runat="server" id="txtmarcacompatible" cssclass="form-control" placeholder="Ej. Honda"/>
                        </div>
                        <div class="col-md-4">
                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Modelo Compatible" runat="server" />
                            <asp:TextBox runat="server" id="modelocompatible" cssclass="form-control" placeholder="Ej. 12"/>
                        </div>
                        <div class="col-md-4">
                            <asp:Label Class="form-label small text-muted text-uppercase fw-bold mb-1" Text="Año del Carro" runat="server" />
                            <asp:TextBox runat="server" id="txtaniocarro" cssclass="form-control" placeholder="Ej. 2017"/>
                        </div>
                    </div>
                    <!--boton de guardar-->
                    <div class="d-flex justify-content-between border-top pt-3">
                        <asp:Button ID="btnguardarinfo" Text="Guardar" runat="server" cssclass="btn btn-success fw-bold px-4 shadow-sm" OnClick="btnguardarinfo_Click"/>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
