
<%@ Page Title="Detalle de orden"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="DetalleOrdenNT.aspx.cs"
    Inherits="CapaPresentacion.OrdenesReparacion.webDetalleOrdenNT" %>

<asp:Content ID="cntDetalleOrdenNT"
    ContentPlaceHolderID="cphContenido"
    runat="server">

    <div class="container-fluid">

        <h2>Detalle de orden de reparación</h2>

        <p class="text-muted">
            Consulta los datos de la reparación y registra su cierre.
        </p>

        <asp:Label
            ID="lblMensajeNT"
            runat="server"
            EnableViewState="false"
            CssClass="text-danger" />

        <asp:Panel ID="pnlDetalleNT" runat="server"
            CssClass="card mt-3">

            <div class="card-body">

                <h5>Información de la orden</h5>

                <p>
                    <strong>Número de orden:</strong>
                    <asp:Label ID="lblIdOrdenNT" runat="server" />
                </p>

                <p>
                    <strong>VIN:</strong>
                    <asp:Label ID="lblVinNT" runat="server" />
                </p>

                <p>
                    <strong>Placa:</strong>
                    <asp:Label ID="lblPlacaNT" runat="server" />
                </p>

                <p>
                    <strong>Motivo de ingreso:</strong>
                    <asp:Label ID="lblMotivoNT" runat="server" />
                </p>

                <p>
                    <strong>Diagnóstico:</strong>
                    <asp:Label ID="lblDiagnosticoNT" runat="server" />
                </p>

                <p>
                    <strong>Estado:</strong>
                    <asp:Label ID="lblEstadoNT" runat="server" />
                </p>

            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCerrarNT" runat="server"
            CssClass="card mt-3">

            <div class="card-body">

                <h5>Cerrar orden de reparación</h5>

                <div class="mb-3">
                    <label for="txtObservacionesNT"
                        class="form-label">
                        Observaciones finales
                    </label>

                    <asp:TextBox
                        ID="txtObservacionesNT"
                        runat="server"
                        TextMode="MultiLine"
                        Rows="4"
                        MaxLength="1000"
                        ClientIDMode="Static"
                        CssClass="form-control" />
                </div>

                <asp:Button
                    ID="btnCerrarOrdenNT"
                    runat="server"
                    Text="Cerrar orden"
                    CssClass="btn btn-danger"
                    OnClick="btnCerrarOrdenNT_Click" />

            </div>
        </asp:Panel>

        <div class="mt-3">
            <a href="OrdenesNT.aspx"
                class="btn btn-outline-secondary">
                Volver a órdenes
            </a>
        </div>

    </div>

</asp:Content>
