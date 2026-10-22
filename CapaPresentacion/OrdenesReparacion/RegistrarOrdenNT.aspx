
<%@ Page Title="Registrar orden de reparación"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="RegistrarOrdenNT.aspx.cs"
    Inherits="CapaPresentacion.OrdenesReparacion.webRegistrarOrdenNT" %>

<asp:Content ID="cntRegistrarOrdenNT"
    ContentPlaceHolderID="cphContenido"
    runat="server">

    <div class="container-fluid">

        <h2>Nueva orden de reparación</h2>
        <p class="text-muted">
            Registra el ingreso de un vehículo al taller.
        </p>

        <asp:Label
            ID="lblMensajeNT"
            runat="server"
            EnableViewState="false"
            CssClass="text-danger" />

        <div class="card mt-3">
            <div class="card-body">

                <h5 class="card-title">Datos del vehículo</h5>

                <div class="mb-3">
                    <label for="ddlVehiculoNT" class="form-label">
                        Seleccionar vehículo
                    </label>

                    <asp:DropDownList
                        ID="ddlVehiculoNT"
                        runat="server"
                        CssClass="form-select"
                        ClientIDMode="Static">
                    </asp:DropDownList>
                </div>

                <h5 class="card-title mt-4">
                    Información de la reparación
                </h5>

                <div class="mb-3">
                    <label for="txtMotivoIngresoNT"
                        class="form-label">
                        Motivo de ingreso
                    </label>

                    <asp:TextBox
                        ID="txtMotivoIngresoNT"
                        runat="server"
                        CssClass="form-control"
                        TextMode="MultiLine"
                        Rows="3"
                        MaxLength="500"
                        ClientIDMode="Static">
                    </asp:TextBox>
                </div>

                <div class="mb-3">
                    <label for="txtDiagnosticoNT"
                        class="form-label">
                        Diagnóstico inicial
                    </label>

                    <asp:TextBox
                        ID="txtDiagnosticoNT"
                        runat="server"
                        CssClass="form-control"
                        TextMode="MultiLine"
                        Rows="3"
                        MaxLength="1000"
                        ClientIDMode="Static">
                    </asp:TextBox>
                </div>

                <div class="d-flex gap-2">
                    <asp:Button
                        ID="btnGuardarOrdenNT"
                        runat="server"
                        Text="Guardar orden"
                        CssClass="btn btn-primary"
                        OnClick="btnGuardarOrdenNT_Click" />

                    <a href="OrdenesNT.aspx"
                       class="btn btn-outline-secondary">
                        Cancelar
                    </a>
                </div>

            </div>
        </div>
    </div>

</asp:Content>
