<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PruebaConexion.aspx.cs" Inherits="PruebaConexionSQL.PruebaConexion" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Prueba de conexión SQL</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Prueba de conexión a SQL Server</h1>
        <p>Estado: <asp:Label ID="lblEstado" runat="server" /></p>
        <ul>
            <li>Servidor: <asp:Label ID="lblServidor" runat="server" /></li>
            <li>Base de datos conectada: <asp:Label ID="lblBaseDatos" runat="server" /></li>
            <li>Usuario: <asp:Label ID="lblUsuario" runat="server" /></li>
            <li>Versión: <asp:Label ID="lblVersion" runat="server" /></li>
            <li>Edición: <asp:Label ID="lblEdicion" runat="server" /></li>
            <li>¿Existe TallerMecanicoDB?: <asp:Label ID="lblExisteBase" runat="server" /></li>
            <li>Consulta con parámetros (2 + 3): <asp:Label ID="lblConsulta" runat="server" /></li>
        </ul>
        <p><asp:Label ID="lblError" runat="server" /></p>
    </form>
</body>
</html>
