<%@ Page Title="Registro Encuentro Académico" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProyectoParcialWebForms._Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Registro al Encuentro Académico</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <style>
        :root {
            --navy: #14304d;
            --navy-dark: #0c2036;
            --gold: #e0a640;
            --bg: #eef1f6;
            --card-bg: #ffffff;
            --text: #263447;
            --muted: #6b7785;
            --danger: #d64550;
            --success: #1f9d55;
            --success-bg: #e8f8ee;
            --border: #dde3ea;
            --radius: 14px;
        }

        * { box-sizing: border-box; }

        body {
            margin: 0;
            font-family: 'Poppins', 'Segoe UI', sans-serif;
            background: var(--bg);
            color: var(--text);
            padding: 32px 16px 60px;
        }

        .page-header {
            max-width: 900px;
            margin: 0 auto 20px;
            text-align: center;
            background: linear-gradient(135deg, var(--navy) 0%, var(--navy-dark) 100%);
            color: #fff;
            padding: 34px 24px;
            border-radius: var(--radius);
            box-shadow: 0 10px 30px rgba(15, 40, 70, 0.25);
        }

        .page-header h1 { margin: 0 0 6px; font-size: 1.7rem; letter-spacing: .5px; }
        .page-header p { margin: 0; opacity: .85; font-size: .92rem; }
        .page-header .badge {
            display: inline-block; margin-top: 14px; background: rgba(255,255,255,.15);
            border: 1px solid rgba(255,255,255,.35); padding: 4px 14px; border-radius: 20px;
            font-size: .72rem; letter-spacing: 1.5px; text-transform: uppercase;
        }

        .card {
            max-width: 900px; margin: 0 auto 20px; background: var(--card-bg);
            border-radius: var(--radius); box-shadow: 0 6px 20px rgba(20,40,70,0.08);
            padding: 30px 32px;
        }

        .required-note { font-size: .82rem; color: var(--muted); margin-bottom: 20px; }
        .required-note span { color: var(--danger); font-weight: 600; }

        .validation-summary {
            background: #fff6e5; border: 1px solid #f1c778; color: #7a5210;
            border-radius: 10px; padding: 14px 18px; margin-bottom: 22px; font-size: .87rem;
        }
        .validation-summary ul { margin: 4px 0 0 18px; padding: 0; }

        .section { margin-bottom: 30px; }
        .section:last-of-type { margin-bottom: 6px; }

        .section-title {
            display: flex; align-items: center; gap: 10px; font-size: 1.02rem;
            font-weight: 600; color: var(--navy); border-left: 4px solid var(--gold);
            padding-left: 10px; margin-bottom: 16px;
        }
        .section-title .icon { font-size: 1.2rem; }

        .field-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 18px; }
        @media (max-width: 640px) { .field-grid { grid-template-columns: 1fr; } }

        .field label { display: block; font-size: .85rem; font-weight: 600; margin-bottom: 6px; color: #3a4a5e; }
        .field label .req { color: var(--danger); }

        .form-control {
            width: 100%; padding: 10px 12px; border: 1.5px solid var(--border);
            border-radius: 9px; font-family: inherit; font-size: .92rem;
            background: #fbfcfe; transition: border-color .15s, box-shadow .15s;
        }
        .form-control:focus {
            outline: none; border-color: var(--navy);
            box-shadow: 0 0 0 3px rgba(20,48,77,0.12); background: #fff;
        }
        textarea.form-control { resize: vertical; min-height: 84px; }

        .radio-pills, .checkbox-grid { display: flex; flex-wrap: wrap; gap: 10px; }
        .radio-pills span, .checkbox-grid span { position: relative; }
        .radio-pills input, .checkbox-grid input { position: absolute; opacity: 0; }
        .radio-pills label, .checkbox-grid label {
            display: inline-block; cursor: pointer; padding: 9px 18px; border: 1.5px solid var(--border);
            border-radius: 20px; font-size: .85rem; background: #fbfcfe; transition: .15s;
        }
        .radio-pills input:checked + label, .checkbox-grid input:checked + label {
            background: var(--navy); border-color: var(--navy); color: #fff;
            box-shadow: 0 4px 10px rgba(20,48,77,.25);
        }
        .radio-pills input:focus + label, .checkbox-grid input:focus + label {
            outline: 2px solid var(--gold); outline-offset: 2px;
        }

        .error-text { color: var(--danger); font-size: .78rem; display: block; margin-top: 5px; }

        .header-link { display: inline-block; margin-top: 12px; color: var(--gold); font-size: .85rem; font-weight: 600; text-decoration: none; }
        .lookup-box {
            background: #f7f9fc; border: 1px dashed var(--border); border-radius: 10px;
            padding: 18px; margin-bottom: 28px;
        }
        .lookup-box .section-title { margin-bottom: 8px; }
        .lookup-box .hint { color: var(--muted); font-size: .82rem; margin: 0 0 14px; }
        .lookup-row { display: flex; gap: 12px; align-items: flex-end; flex-wrap: wrap; }
        .lookup-row .field { flex: 1; min-width: 220px; }
        .info-text { display: block; font-size: .85rem; margin-top: 10px; font-weight: 600; }
        .ok-text { color: var(--success); }
        .accept-row {
            display: flex; align-items: flex-start; gap: 10px; background: #f7f9fc;
            border: 1px solid var(--border); border-radius: 10px; padding: 14px 16px;
        }
        .accept-row label { font-size: .87rem; color: #3a4a5e; }

        .actions {
            display: flex; gap: 12px; flex-wrap: wrap; margin-top: 6px;
            padding-top: 20px; border-top: 1px dashed var(--border);
        }

        .btn {
            border: none; border-radius: 9px; padding: 11px 26px; font-size: .9rem;
            font-weight: 600; cursor: pointer; font-family: inherit;
            transition: transform .1s, box-shadow .15s;
        }
        .btn:active { transform: translateY(1px); }
        .btn-primary {
            background: linear-gradient(135deg, var(--navy), var(--navy-dark)); color: #fff;
            box-shadow: 0 6px 16px rgba(20,48,77,.3);
        }
        .btn-secondary { background: #fff; color: var(--navy); border: 1.5px solid var(--navy); }
        .btn-ghost { background: transparent; color: var(--danger); }

        .result-card {
            max-width: 900px; margin: 0 auto; background: var(--success-bg);
            border: 1px solid #b7e4c7; border-radius: var(--radius);
            padding: 28px 30px; text-align: center;
        }
        .result-card .check { font-size: 2rem; }
        .result-card h2 { color: var(--success); margin: 6px 0 10px; font-size: 1.3rem; }
        .result-card p { margin: 0; font-size: .95rem; }
        .result-code {
            display: inline-block; background: #fff; border: 1px dashed var(--success);
            padding: 6px 16px; border-radius: 20px; font-weight: 600; color: var(--navy);
            letter-spacing: 1px; margin-top: 12px; font-size: .85rem;
        }
        .result-summary { color: var(--muted); font-size: .82rem; margin-top: 12px; line-height: 1.5; }

        .footer-note { text-align: center; color: var(--muted); font-size: .78rem; margin-top: 26px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="page-header">
            <h1>🎓 Registro al Encuentro Académico</h1>
            <p>Formulario ASP.NET Web Forms — Proyecto de Parcial</p>
            <span class="badge">Sistema de registro de asistentes</span>
            <%= ProyectoParcialWebForms.Seguridad.Navegacion.Render() %>
        </div>

        <div class="card">

            <p class="required-note">Los campos marcados con <span>*</span> son obligatorios.</p>

            <asp:ValidationSummary ID="vsErrores" runat="server" CssClass="validation-summary"
                HeaderText="⚠️ Revisa los siguientes campos antes de continuar:" DisplayMode="BulletList" />

            <!-- Consultar / editar registro -->
            <div class="lookup-box">
                <div class="section-title"><span class="icon">🔎</span> ¿Ya te registraste? Consulta y edita tu registro</div>
                <p class="hint">Escribe tu número de identificación y el correo con el que te registraste para cargar tus respuestas, modificarlas y actualizarlas.</p>
                <div class="lookup-row">
                    <div class="field">
                        <label>Número de identificación</label>
                        <asp:TextBox ID="txtBuscarId" runat="server" CssClass="form-control" placeholder="Ej: 1234567890" MaxLength="12" />
                    </div>
                    <div class="field">
                        <label>Correo registrado</label>
                        <asp:TextBox ID="txtBuscarCorreo" runat="server" CssClass="form-control" placeholder="correo@ejemplo.com" MaxLength="120" />
                    </div>
                    <asp:Button ID="btnBuscar" runat="server" Text="BUSCAR MI REGISTRO" CssClass="btn btn-secondary"
                        CausesValidation="false" OnClick="btnBuscar_Click" />
                </div>
                <asp:Label ID="lblBusqueda" runat="server" CssClass="info-text" />
            </div>

            <!-- Datos personales -->
            <div class="section">
                <div class="section-title"><span class="icon">👤</span> Datos personales</div>
                <div class="field-grid">
                    <div class="field">
                        <label>Nombres <span class="req">*</span></label>
                        <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" placeholder="Escriba sus nombres" MaxLength="80" />
                        <asp:RequiredFieldValidator ID="rfvNombres" runat="server" ControlToValidate="txtNombres"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="El nombre es obligatorio." />
                    </div>
                    <div class="field">
                        <label>Apellidos <span class="req">*</span></label>
                        <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control" placeholder="Escriba sus apellidos" MaxLength="80" />
                        <asp:RequiredFieldValidator ID="rfvApellidos" runat="server" ControlToValidate="txtApellidos"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="El apellido es obligatorio." />
                    </div>
                    <div class="field">
                        <label>Número de identificación <span class="req">*</span></label>
                        <asp:TextBox ID="txtIdentificacion" runat="server" CssClass="form-control" placeholder="Sin puntos ni espacios" MaxLength="12" />
                        <asp:RequiredFieldValidator ID="rfvIdentificacion" runat="server" ControlToValidate="txtIdentificacion"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="El número de identificación es obligatorio." />
                        <asp:RegularExpressionValidator ID="revIdentificacion" runat="server" ControlToValidate="txtIdentificacion"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="La identificación debe tener entre 5 y 12 dígitos numéricos."
                            ValidationExpression="^[0-9]{5,12}$" />
                    </div>
                </div>
            </div>

            <!-- Contacto -->
            <div class="section">
                <div class="section-title"><span class="icon">✉️</span> Contacto</div>
                <div class="field-grid">
                    <div class="field">
                        <label>Correo electrónico <span class="req">*</span></label>
                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" placeholder="ejemplo@correo.com" MaxLength="120" />
                        <asp:RequiredFieldValidator ID="rfvCorreo" runat="server" ControlToValidate="txtCorreo"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="El correo es obligatorio." />
                        <asp:RegularExpressionValidator ID="revCorreo" runat="server" ControlToValidate="txtCorreo"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="El correo no tiene un formato válido."
                            ValidationExpression="^[\w\.-]+@[\w\.-]+\.\w{2,}$" />
                    </div>
                    <div class="field">
                        <label>Teléfono <span class="req">*</span></label>
                        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" placeholder="300 000 0000" MaxLength="10" />
                        <asp:RequiredFieldValidator ID="rfvTelefono" runat="server" ControlToValidate="txtTelefono"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="El teléfono es obligatorio." />
                        <asp:RegularExpressionValidator ID="revTelefono" runat="server" ControlToValidate="txtTelefono"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="Debe tener entre 7 y 10 dígitos numéricos."
                            ValidationExpression="^[0-9]{7,10}$" />
                    </div>
                </div>
            </div>

            <!-- Información académica -->
            <div class="section">
                <div class="section-title"><span class="icon">🏛️</span> Información académica</div>
                <div class="field-grid">
                    <div class="field">
                        <label>Programa académico <span class="req">*</span></label>
                        <asp:DropDownList ID="ddlPrograma" runat="server" CssClass="form-control">
                            <asp:ListItem Text="-- Seleccione un programa --" Value="" />
                            <asp:ListItem Text="Ingeniería de Sistemas" Value="Ingeniería de Sistemas" />
                            <asp:ListItem Text="Ingeniería Industrial" Value="Ingeniería Industrial" />
                            <asp:ListItem Text="Administración de Empresas" Value="Administración de Empresas" />
                            <asp:ListItem Text="Derecho" Value="Derecho" />
                            <asp:ListItem Text="Psicología" Value="Psicología" />
                            <asp:ListItem Text="Comunicación Social" Value="Comunicación Social" />
                            <asp:ListItem Text="Otro" Value="Otro" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvPrograma" runat="server" ControlToValidate="ddlPrograma"
                            CssClass="error-text" Display="Dynamic" InitialValue="" ErrorMessage="Seleccione un programa." />
                    </div>
                    <div class="field">
                        <label>Semestre <span class="req">*</span></label>
                        <asp:DropDownList ID="ddlSemestre" runat="server" CssClass="form-control">
                            <asp:ListItem Text="-- Seleccione --" Value="" />
                            <asp:ListItem Text="1" /><asp:ListItem Text="2" /><asp:ListItem Text="3" />
                            <asp:ListItem Text="4" /><asp:ListItem Text="5" /><asp:ListItem Text="6" />
                            <asp:ListItem Text="7" /><asp:ListItem Text="8" /><asp:ListItem Text="9" />
                            <asp:ListItem Text="10" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvSemestre" runat="server" ControlToValidate="ddlSemestre"
                            CssClass="error-text" Display="Dynamic" InitialValue="" ErrorMessage="Seleccione un semestre." />
                    </div>
                </div>
            </div>

            <!-- Participación -->
            <div class="section">
                <div class="section-title"><span class="icon">🗳️</span> Participación</div>
                <div class="field-grid">
                    <div class="field">
                        <label>Tipo de participación <span class="req">*</span></label>
                        <asp:RadioButtonList ID="rblParticipacion" runat="server" CssClass="radio-pills"
                            RepeatLayout="Flow" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Ponente" Value="Ponente" />
                            <asp:ListItem Text="Asistente" Value="Asistente" />
                            <asp:ListItem Text="Organizador" Value="Organizador" />
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator ID="rfvParticipacion" runat="server" ControlToValidate="rblParticipacion"
                            CssClass="error-text" Display="Dynamic" ErrorMessage="Seleccione un tipo de participación." />
                    </div>
                    <div class="field">
                        <label>Jornada preferida <span class="req">*</span></label>
                        <asp:DropDownList ID="ddlJornada" runat="server" CssClass="form-control">
                            <asp:ListItem Text="-- Seleccione --" Value="" />
                            <asp:ListItem Text="Mañana" Value="Mañana" />
                            <asp:ListItem Text="Tarde" Value="Tarde" />
                            <asp:ListItem Text="Noche" Value="Noche" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvJornada" runat="server" ControlToValidate="ddlJornada"
                            CssClass="error-text" Display="Dynamic" InitialValue="" ErrorMessage="Seleccione una jornada." />
                    </div>
                </div>
            </div>

            <!-- Temas de interés -->
            <div class="section">
                <div class="section-title"><span class="icon">💡</span> Temas de interés</div>
                <div class="field">
                    <asp:CheckBoxList ID="cblTemas" runat="server" CssClass="checkbox-grid"
                        RepeatLayout="Flow" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Inteligencia Artificial" Value="Inteligencia Artificial" />
                        <asp:ListItem Text="Desarrollo Web" Value="Desarrollo Web" />
                        <asp:ListItem Text="Ciberseguridad" Value="Ciberseguridad" />
                        <asp:ListItem Text="Bases de Datos" Value="Bases de Datos" />
                        <asp:ListItem Text="Redes y Telecomunicaciones" Value="Redes y Telecomunicaciones" />
                        <asp:ListItem Text="Cloud Computing" Value="Cloud Computing" />
                        <asp:ListItem Text="Innovación Educativa" Value="Innovación Educativa" />
                    </asp:CheckBoxList>
                    <asp:CustomValidator ID="cvTemas" runat="server" CssClass="error-text" Display="Dynamic"
                        ErrorMessage="Seleccione al menos un tema de interés." OnServerValidate="cvTemas_ServerValidate" />
                </div>
            </div>

            <!-- Observaciones -->
            <div class="section">
                <div class="section-title"><span class="icon">📝</span> Observaciones</div>
                <div class="field">
                    <label>Comentarios <span style="font-weight:400;color:var(--muted);">(opcional, máx. 400 caracteres)</span></label>
                    <asp:TextBox ID="txtComentarios" runat="server" CssClass="form-control" TextMode="MultiLine"
                        Rows="4" MaxLength="400" placeholder="Escriba una observación o expectativa" />
                </div>
            </div>

            <!-- Autorización -->
            <div class="section">
                <div class="section-title"><span class="icon">✅</span> Autorización</div>
                <div class="accept-row">
                    <asp:CheckBox ID="chkAcepta" runat="server" />
                    <label for="chkAcepta">Acepto el tratamiento de mis datos para fines académicos. <span class="req">*</span></label>
                </div>
                <asp:CustomValidator ID="cvAcepta" runat="server" CssClass="error-text" Display="Dynamic"
                    ErrorMessage="Debe aceptar el tratamiento de datos para continuar." OnServerValidate="cvAcepta_ServerValidate" />
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="error-text" EnableViewState="false" />

            <!-- Acciones -->
            <div class="actions">
                <asp:Button ID="btnRegistrar" runat="server" Text="REGISTRAR" CssClass="btn btn-primary" OnClick="btnRegistrar_Click" />
                <asp:Button ID="btnLimpiar" runat="server" Text="LIMPIAR" CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnLimpiar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="CANCELAR" CssClass="btn btn-ghost" CausesValidation="false" OnClick="btnCancelar_Click" />
            </div>

        </div>

        <!-- Resultado -->
        <asp:Panel ID="pnlResultado" runat="server" CssClass="result-card" Visible="false">
            <div class="check">✅</div>
            <h2>Registro completado</h2>
            <asp:Label ID="lblMensaje" runat="server" />
            <br />
            <asp:Label ID="lblCodigo" runat="server" CssClass="result-code" />
            <br />
            <asp:Label ID="lblResumen" runat="server" CssClass="result-summary" />
        </asp:Panel>

        <p class="footer-note">Proyecto de Parcial · ASP.NET Web Forms + C#</p>

    </form>
</body>
</html>
