
<%@ Page Title="Órdenes de reparación"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="OrdenesNT.aspx.cs"
    Inherits="CapaPresentacion.OrdenesReparacion.webOrdenesNT" %>

<asp:Content ID="cntOrdenesNT"
    ContentPlaceHolderID="cphContenido"
    runat="server">

    <h2>Órdenes de reparación</h2>

    <p>Consulta de órdenes de reparación abiertas.</p>

    <asp:Label
        ID="lblMensajeNT"
        runat="server"
        EnableViewState="false" />
    
<div class="d-flex gap-2 mb-3">
    <a href="RegistrarOrdenNT.aspx"
       class="btn btn-primary">
        <i class="bi bi-plus-circle me-1"></i>
        Nueva orden
    </a>

    <a href="HistorialOrdenesNT.aspx"
       class="btn btn-outline-primary">
        <i class="bi bi-clock-history me-1"></i>
        Historial por VIN
    </a>
</div>

    <asp:GridView
        ID="grvOrdenesNT"
        runat="server"
        AutoGenerateColumns="False"
        EmptyDataText="No existen órdenes abiertas."
        CssClass="table">

        <Columns>
            
              <asp:HyperLinkField
                 HeaderText="Acciones"
                 Text="Ver detalle"
                 DataNavigateUrlFields="lIntIdOrdenNT"
                 DataNavigateUrlFormatString="~/OrdenesReparacion/DetalleOrdenNT.aspx?id={0}" />

            <asp:BoundField
                DataField="lIntIdOrdenNT"
                HeaderText="Orden" />

            <asp:BoundField
                DataField="lStrVinNT"
                HeaderText="VIN" />

            <asp:BoundField
                DataField="lStrPlacaNT"
                HeaderText="Placa" />

            <asp:BoundField
                DataField="lStrMotivoIngresoNT"
                HeaderText="Motivo de ingreso" />

            <asp:BoundField
                DataField="lStrEstadoNT"
                HeaderText="Estado" />
        </Columns>

    </asp:GridView>

</asp:Content>
