
<%@ Page Title="Historial de reparaciones"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="HistorialOrdenesNT.aspx.cs"
    Inherits="CapaPresentacion.OrdenesReparacion.webHistorialOrdenesNT" %>

<asp:Content ID="cntHistorialNT"
    ContentPlaceHolderID="cphContenido"
    runat="server">

    <div class="container-fluid">

        <h2>Historial de reparaciones</h2>

        <p class="text-muted">
            Consulta las reparaciones realizadas a un vehículo
            mediante su número VIN.
        </p>

        <div class="card mb-3">
            <div class="card-body">

                <div class="mb-3">
                    <label for="txtVinNT" class="form-label">
                        VIN del vehículo
                    </label>

                    <asp:TextBox
                        ID="txtVinNT"
                        runat="server"
                        ClientIDMode="Static"
                        MaxLength="17"
                        CssClass="form-control"
                        placeholder="Ingrese el VIN de 17 caracteres" />
                </div>

                <asp:Button
                    ID="btnBuscarNT"
                    runat="server"
                    Text="Buscar historial"
                    CssClass="btn btn-primary"
                    OnClick="btnBuscarNT_Click" />

            </div>
        </div>

        <asp:Label
            ID="lblMensajeNT"
            runat="server"
            EnableViewState="false"
            CssClass="text-danger" />

        <div class="table-responsive mt-3">

            <asp:GridView
                ID="grvHistorialNT"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-striped table-hover"
                EmptyDataText="No se encontraron reparaciones."
                GridLines="None">

                <Columns>

                    <asp:BoundField
                        DataField="lIntIdOrdenNT"
                        HeaderText="Orden" />

                    <asp:BoundField
                        DataField="lDatFechaAperturaNT"
                        HeaderText="Fecha de apertura"
                        DataFormatString="{0:dd/MM/yyyy HH:mm}" />

                    <asp:BoundField
                        DataField="lStrPlacaNT"
                        HeaderText="Placa" />

                    <asp:BoundField
                        DataField="lStrMotivoIngresoNT"
                        HeaderText="Motivo de ingreso" />

                    <asp:BoundField
                        DataField="lStrEstadoNT"
                        HeaderText="Estado" />

                    <asp:HyperLinkField
                        HeaderText="Acciones"
                        Text="Ver detalle"
                        DataNavigateUrlFields="lIntIdOrdenNT"
                        DataNavigateUrlFormatString="~/OrdenesReparacion/DetalleOrdenNT.aspx?id={0}" />

                </Columns>

            </asp:GridView>

        </div>

        <a href="OrdenesNT.aspx"
            class="btn btn-outline-secondary mt-3">
            Volver a órdenes
        </a>

    </div>

</asp:Content>
