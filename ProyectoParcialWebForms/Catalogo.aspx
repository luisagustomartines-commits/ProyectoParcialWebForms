<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Catalogo.aspx.cs" Inherits="ProyectoParcialWebForms.Catalogo" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Actividades disponibles</title>
    <style>
        :root { --navy: #14304d; --navy-dark: #0c2036; --gold: #e0a640; --bg: #eef1f6; --text: #263447; --muted: #6b7785; --danger: #d64550; --success: #1f9d55; --border: #dde3ea; --radius: 14px; }
        * { box-sizing: border-box; }
        body { margin: 0; background: var(--bg); color: var(--text); font-family: 'Segoe UI', Arial, sans-serif; }
        .top { background: linear-gradient(135deg, var(--navy), var(--navy-dark)); color: #fff; padding: 22px 32px; }
        .top h1 { margin: 0 0 4px; font-size: 1.6rem; }
        .top p { margin: 0; opacity: .85; font-size: .92rem; }
        .wrap { max-width: 1240px; margin: 22px auto; padding: 0 16px; }
        .card { background: #fff; border-radius: var(--radius); padding: 22px; box-shadow: 0 6px 20px rgba(20,48,77,.08); margin-bottom: 20px; }
        .card h2 { margin: 0 0 14px; font-size: 1.1rem; color: var(--navy); }
        .card h3 { margin: 18px 0 8px; font-size: .98rem; color: var(--navy); }
        label { display: block; font-size: .82rem; font-weight: 600; margin-bottom: 4px; }
        .ctl { width: 100%; padding: 9px 10px; border: 1.5px solid var(--border); border-radius: 9px; font: inherit; font-size: .9rem; }
        .ctl:focus { outline: none; border-color: var(--navy); }
        .btn { padding: 10px 20px; border-radius: 9px; border: 1.5px solid var(--navy); font: inherit; font-weight: 600; cursor: pointer; }
        .btn-primary { background: var(--navy); color: #fff; }
        .btn-secondary { background: #fff; color: var(--navy); }
        .btn-danger { background: #fff; color: var(--danger); border-color: var(--danger); }
        .msg { display: block; margin: 0 0 14px; font-weight: 600; font-size: .9rem; }
        .msg.ok { color: var(--success); } .msg.err { color: var(--danger); }
        .errores { margin: 0 0 14px; padding: 10px 14px 10px 30px; background: #fdecee; border: 1px solid #f3c2c7; border-radius: 9px; color: var(--danger); font-size: .88rem; }
        .tabla { width: 100%; border-collapse: collapse; font-size: .88rem; }
        .tabla th { background: var(--navy); color: #fff; text-align: left; padding: 9px 10px; }
        .tabla td { border-bottom: 1px solid var(--border); padding: 8px 10px; vertical-align: top; }
        .tabla tr:nth-child(even) td { background: #f7f9fc; }
        .accion { color: var(--navy); font-weight: 600; text-decoration: none; margin-right: 10px; }
        .accion.peligro { color: var(--danger); }
        .scroll { overflow-x: auto; }
        .total { display: block; color: var(--muted); font-size: .85rem; margin-top: 10px; }
        .filtros { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr 1fr auto auto; gap: 10px; align-items: end; }
        @media (max-width: 1100px) { .filtros { grid-template-columns: 1fr 1fr; } }
        .grid2 { display: grid; grid-template-columns: 1.2fr 1fr; gap: 20px; align-items: start; }
        @media (max-width: 980px) { .grid2 { grid-template-columns: 1fr; } }
        .chip { display: inline-block; padding: 3px 10px; border-radius: 12px; background: #e8edf3; color: var(--navy); font-size: .78rem; font-weight: 600; margin-right: 6px; }
        .datos { list-style: none; padding: 0; margin: 10px 0 14px; font-size: .9rem; }
        .datos li { margin-bottom: 5px; }
        .aviso { display: block; color: var(--muted); font-size: .85rem; margin: 6px 0 12px; }
        .estrellas { color: var(--gold); letter-spacing: 1px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top">
            <h1>🎓 Actividades disponibles</h1>
            <p>Consulte el detalle, inscríbase y valore las actividades en las que participó.</p>
            <%= ProyectoParcialWebForms.Seguridad.Navegacion.Render() %>
        </div>

        <div class="wrap">
            <asp:Label ID="lblMensaje" runat="server" CssClass="msg" />
            <asp:Literal ID="ltErrores" runat="server" />

            <!-- Filtros -->
            <div class="card">
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
            </div>

            <div class="grid2">
                <!-- Listado -->
                <div class="card">
                    <h2>Listado de actividades</h2>
                    <div class="scroll">
                        <asp:GridView ID="gvCatalogo" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                            DataKeyNames="ActividadId" EmptyDataText="No hay actividades para mostrar."
                            OnRowCommand="gvCatalogo_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                                <asp:BoundField DataField="TipoNombre" HeaderText="Tipo" />
                                <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="Modalidad" HeaderText="Modalidad" />
                                <asp:TemplateField HeaderText="Cupos libres">
                                    <ItemTemplate><%# Eval("CuposDisponibles") %> / <%# Eval("Cupo") %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Estado" HeaderText="Estado" />
                                <asp:TemplateField HeaderText="Valoración">
                                    <ItemTemplate><%# FormatoPromedio(Eval("PromedioCalificacion"), Eval("TotalResenas")) %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkDetalle" runat="server" CommandName="Detalle"
                                            CommandArgument='<%# Eval("ActividadId") %>' CssClass="accion" CausesValidation="false">Ver detalle</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:Label ID="lblTotal" runat="server" CssClass="total" />
                </div>

                <!-- Detalle -->
                <asp:Panel ID="pnlDetalle" runat="server" CssClass="card" Visible="false">
                    <h2><asp:Label ID="lblDetTitulo" runat="server" /></h2>
                    <div>
                        <asp:Label ID="lblDetTipo" runat="server" CssClass="chip" />
                        <asp:Label ID="lblDetEstado" runat="server" CssClass="chip" />
                        <asp:Label ID="lblDetModalidad" runat="server" CssClass="chip" />
                    </div>
                    <p><asp:Label ID="lblDetDescripcion" runat="server" /></p>
                    <ul class="datos">
                        <li><strong>Fecha:</strong> <asp:Label ID="lblDetFecha" runat="server" /></li>
                        <li><strong>Horario:</strong> <asp:Label ID="lblDetHorario" runat="server" /></li>
                        <li><strong>Lugar:</strong> <asp:Label ID="lblDetLugar" runat="server" /></li>
                        <li><strong>Cupos:</strong> <asp:Label ID="lblDetCupos" runat="server" /></li>
                        <li><strong>Valoración:</strong> <asp:Label ID="lblDetValoracion" runat="server" /></li>
                    </ul>

                    <asp:Label ID="lblDetAviso" runat="server" CssClass="aviso" />
                    <asp:Button ID="btnInscribirme" runat="server" Text="Inscribirme" CssClass="btn btn-primary" OnClick="btnInscribirme_Click" />
                    <asp:Button ID="btnCancelarInscripcion" runat="server" Text="Cancelar mi inscripción" CssClass="btn btn-danger"
                        OnClick="btnCancelarInscripcion_Click" OnClientClick="return confirm('¿Cancelar su inscripción?');" />

                    <h3>Reseñas</h3>
                    <asp:GridView ID="gvResenas" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                        EmptyDataText="Esta actividad todavía no tiene reseñas.">
                        <Columns>
                            <asp:BoundField DataField="NombreParticipante" HeaderText="Participante" />
                            <asp:TemplateField HeaderText="Calificación">
                                <ItemTemplate><span class="estrellas"><%# Estrellas(Eval("Calificacion")) %></span></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Comentario" HeaderText="Comentario" />
                            <asp:BoundField DataField="Fecha" HeaderText="Fecha" HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                        </Columns>
                    </asp:GridView>

                    <asp:Panel ID="pnlNuevaResena" runat="server" Visible="false">
                        <h3>Publicar mi reseña</h3>
                        <label>Calificación</label>
                        <asp:DropDownList ID="ddlCalificacion" runat="server" CssClass="ctl">
                            <asp:ListItem Text="★★★★★ (5)" Value="5" />
                            <asp:ListItem Text="★★★★ (4)" Value="4" />
                            <asp:ListItem Text="★★★ (3)" Value="3" />
                            <asp:ListItem Text="★★ (2)" Value="2" />
                            <asp:ListItem Text="★ (1)" Value="1" />
                        </asp:DropDownList>
                        <label style="margin-top:10px">Comentario (opcional)</label>
                        <asp:TextBox ID="txtComentarioResena" runat="server" CssClass="ctl" TextMode="MultiLine" Rows="3" MaxLength="1000" />
                        <br />
                        <asp:Button ID="btnPublicarResena" runat="server" Text="Publicar reseña" CssClass="btn btn-primary" OnClick="btnPublicarResena_Click" />
                    </asp:Panel>

                    <asp:Panel ID="pnlInscritos" runat="server" Visible="false">
                        <h3>Participantes inscritos (solo organizadores)</h3>
                        <asp:GridView ID="gvInscritos" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                            EmptyDataText="Aún no hay inscritos.">
                            <Columns>
                                <asp:BoundField DataField="NombreParticipante" HeaderText="Nombre" />
                                <asp:BoundField DataField="Programa" HeaderText="Programa" />
                                <asp:BoundField DataField="FechaInscripcion" HeaderText="Inscripción" HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </asp:Panel>
            </div>

            <!-- Mis inscripciones -->
            <div class="card">
                <h2>Mis inscripciones</h2>
                <div class="scroll">
                    <asp:GridView ID="gvMisInscripciones" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                        EmptyDataText="Todavía no se ha inscrito en ninguna actividad." OnRowCommand="gvMisInscripciones_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Titulo" HeaderText="Actividad" />
                            <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="EstadoActividad" HeaderText="Estado de la actividad" />
                            <asp:BoundField DataField="Estado" HeaderText="Mi inscripción" />
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkCancelar" runat="server" CommandName="CancelarInscripcion"
                                        CommandArgument='<%# Eval("ActividadId") %>' CssClass="accion peligro" CausesValidation="false"
                                        Visible='<%# Eval("Estado").ToString() == "Activa" %>'
                                        OnClientClick="return confirm('¿Cancelar su inscripción?');">Cancelar</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
