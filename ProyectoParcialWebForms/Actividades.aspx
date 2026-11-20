<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Actividades.aspx.cs" Inherits="ProyectoParcialWebForms.Actividades" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Actividades académicas</title>
    <style>
        :root {
            --navy: #14304d; --navy-dark: #0c2036; --gold: #e0a640; --bg: #eef1f6; --text: #263447;
            --muted: #6b7785; --danger: #d64550; --success: #1f9d55; --border: #dde3ea; --radius: 14px;
        }
        * { box-sizing: border-box; }
        body { margin: 0; background: var(--bg); color: var(--text); font-family: 'Segoe UI', Arial, sans-serif; }
        .top { background: linear-gradient(135deg, var(--navy), var(--navy-dark)); color: #fff; padding: 22px 32px; }
        .top h1 { margin: 0 0 4px; font-size: 1.6rem; }
        .top p { margin: 0; opacity: .85; font-size: .92rem; }
        .nav { margin-top: 12px; display: flex; gap: 18px; flex-wrap: wrap; }
        .nav a { color: var(--gold); font-weight: 600; font-size: .86rem; text-decoration: none; }
        .wrap { max-width: 1240px; margin: 22px auto; padding: 0 16px; }
        .grid2 { display: grid; grid-template-columns: 420px 1fr; gap: 20px; align-items: start; }
        @media (max-width: 960px) { .grid2 { grid-template-columns: 1fr; } }
        .card { background: #fff; border-radius: var(--radius); padding: 22px; box-shadow: 0 6px 20px rgba(20,48,77,.08); }
        .card h2 { margin: 0 0 14px; font-size: 1.1rem; color: var(--navy); }
        .row { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
        .field { margin-bottom: 12px; }
        .field.full { grid-column: 1 / -1; }
        label { display: block; font-size: .82rem; font-weight: 600; margin-bottom: 4px; }
        .req { color: var(--danger); }
        .ctl { width: 100%; padding: 9px 10px; border: 1.5px solid var(--border); border-radius: 9px; font: inherit; font-size: .9rem; }
        .ctl:focus { outline: none; border-color: var(--navy); }
        textarea.ctl { resize: vertical; min-height: 78px; }
        .btn { padding: 10px 20px; border-radius: 9px; border: 1.5px solid var(--navy); font: inherit; font-weight: 600; cursor: pointer; }
        .btn-primary { background: var(--navy); color: #fff; }
        .btn-secondary { background: #fff; color: var(--navy); }
        .msg { display: block; margin: 0 0 14px; font-weight: 600; font-size: .9rem; }
        .msg.ok { color: var(--success); } .msg.err { color: var(--danger); }
        .errores { margin: 0 0 14px; padding: 10px 14px 10px 30px; background: #fdecee; border: 1px solid #f3c2c7; border-radius: 9px; color: var(--danger); font-size: .88rem; }
        .filtros { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr 1fr auto auto; gap: 10px; align-items: end; margin-bottom: 14px; }
        @media (max-width: 1100px) { .filtros { grid-template-columns: 1fr 1fr; } }
        .tabla { width: 100%; border-collapse: collapse; font-size: .88rem; }
        .tabla th { background: var(--navy); color: #fff; text-align: left; padding: 9px 10px; }
        .tabla td { border-bottom: 1px solid var(--border); padding: 8px 10px; vertical-align: top; }
        .tabla tr:nth-child(even) td { background: #f7f9fc; }
        .accion { color: var(--navy); font-weight: 600; text-decoration: none; margin-right: 10px; }
        .accion.peligro { color: var(--danger); }
        .total { display: block; color: var(--muted); font-size: .85rem; margin-top: 10px; }
        .scroll { overflow-x: auto; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top">
            <h1>🎓 Gestión de actividades académicas</h1>
            <p>Registra, consulta, actualiza y desactiva las actividades (solo organizadores).</p>
            <%= ProyectoParcialWebForms.Seguridad.Navegacion.Render() %>
        </div>

        <div class="wrap">
            <asp:Label ID="lblMensaje" runat="server" CssClass="msg" />
            <asp:Literal ID="ltErrores" runat="server" />

            <div class="grid2">
                <!-- Formulario -->
                <div class="card">
                    <h2><asp:Label ID="lblModo" runat="server" Text="Nueva actividad" /></h2>
                    <asp:HiddenField ID="hfActividadId" runat="server" />

                    <div class="field">
                        <label>Título de la actividad <span class="req">*</span></label>
                        <asp:TextBox ID="txtTitulo" runat="server" CssClass="ctl" MaxLength="120" placeholder="Ej: Taller de Programación Web" />
                    </div>

                    <div class="field">
                        <label>Tipo de actividad <span class="req">*</span></label>
                        <asp:DropDownList ID="ddlTipo" runat="server" CssClass="ctl" />
                    </div>

                    <div class="field">
                        <label>Descripción <span class="req">*</span></label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="ctl" TextMode="MultiLine" Rows="3" MaxLength="1000" />
                    </div>

                    <div class="row">
                        <div class="field">
                            <label>Fecha de inicio <span class="req">*</span></label>
                            <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="ctl" TextMode="Date" />
                        </div>
                        <div class="field">
                            <label>Fecha de fin <span class="req">*</span></label>
                            <asp:TextBox ID="txtFechaFin" runat="server" CssClass="ctl" TextMode="Date" />
                        </div>
                        <div class="field">
                            <label>Hora de inicio <span class="req">*</span></label>
                            <asp:TextBox ID="txtHoraInicio" runat="server" CssClass="ctl" TextMode="Time" />
                        </div>
                        <div class="field">
                            <label>Hora de fin <span class="req">*</span></label>
                            <asp:TextBox ID="txtHoraFin" runat="server" CssClass="ctl" TextMode="Time" />
                        </div>
                        <div class="field">
                            <label>Modalidad <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlModalidad" runat="server" CssClass="ctl">
                                <asp:ListItem Text="Presencial" Value="Presencial" />
                                <asp:ListItem Text="Virtual" Value="Virtual" />
                                <asp:ListItem Text="Híbrida" Value="Híbrida" />
                            </asp:DropDownList>
                        </div>
                        <div class="field">
                            <label>Cupos disponibles <span class="req">*</span></label>
                            <asp:TextBox ID="txtCupo" runat="server" CssClass="ctl" TextMode="Number" />
                        </div>
                        <div class="field">
                            <label>Lugar <span class="req">*</span></label>
                            <asp:TextBox ID="txtLugar" runat="server" CssClass="ctl" MaxLength="150" placeholder="Ej: Laboratorio 3" />
                        </div>
                        <div class="field">
                            <label>Estado <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="ctl">
                                <asp:ListItem Text="Activa" Value="Activa" />
                                <asp:ListItem Text="Próxima" Value="Próxima" />
                                <asp:ListItem Text="Finalizada" Value="Finalizada" />
                                <asp:ListItem Text="Inactiva" Value="Inactiva" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="btnLimpiar_Click" />
                </div>

                <!-- Listado -->
                <div class="card">
                    <h2>Listado de actividades</h2>

                    <div class="filtros">
                        <div>
                            <label>Buscar por título o lugar</label>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="ctl" placeholder="Buscar actividad..." />
                        </div>
                        <div>
                            <label>Tipo</label>
                            <asp:DropDownList ID="ddlFiltroTipo" runat="server" CssClass="ctl" />
                        </div>
                        <div>
                            <label>Estado</label>
                            <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="ctl">
                                <asp:ListItem Text="Todos (sin inactivas)" Value="" />
                                <asp:ListItem Text="Activa" Value="Activa" />
                                <asp:ListItem Text="Próxima" Value="Próxima" />
                                <asp:ListItem Text="Finalizada" Value="Finalizada" />
                                <asp:ListItem Text="Inactiva" Value="Inactiva" />
                            </asp:DropDownList>
                        </div>
                        <div>
                            <label>Desde</label>
                            <asp:TextBox ID="txtDesde" runat="server" CssClass="ctl" TextMode="Date" />
                        </div>
                        <div>
                            <label>Hasta</label>
                            <asp:TextBox ID="txtHasta" runat="server" CssClass="ctl" TextMode="Date" />
                        </div>
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary" OnClick="btnBuscar_Click" />
                        <asp:Button ID="btnVerTodas" runat="server" Text="Ver todas" CssClass="btn btn-secondary" OnClick="btnVerTodas_Click" />
                    </div>

                    <div class="scroll">
                        <asp:GridView ID="gvActividades" runat="server" AutoGenerateColumns="false" CssClass="tabla"
                            GridLines="None" DataKeyNames="ActividadId" EmptyDataText="No hay actividades para mostrar."
                            OnRowCommand="gvActividades_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="ActividadId" HeaderText="#" />
                                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                                <asp:BoundField DataField="TipoNombre" HeaderText="Tipo" />
                                <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:TemplateField HeaderText="Horario">
                                    <ItemTemplate><%# FormatoHorario(Eval("HoraInicio"), Eval("HoraFin")) %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Modalidad" HeaderText="Modalidad" />
                                <asp:TemplateField HeaderText="Inscritos / Cupo">
                                    <ItemTemplate><%# Eval("Inscritos") %> / <%# Eval("Cupo") %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Estado" HeaderText="Estado" />
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Editar"
                                            CommandArgument='<%# Eval("ActividadId") %>' CssClass="accion" CausesValidation="false">Editar</asp:LinkButton>
                                        <asp:LinkButton ID="lnkDesactivar" runat="server" CommandName="Desactivar"
                                            CommandArgument='<%# Eval("ActividadId") %>' CssClass="accion peligro" CausesValidation="false"
                                            Visible='<%# Eval("Estado").ToString() != "Inactiva" %>'
                                            OnClientClick="return confirm('¿Desactivar esta actividad?');">Desactivar</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:Label ID="lblTotal" runat="server" CssClass="total" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
