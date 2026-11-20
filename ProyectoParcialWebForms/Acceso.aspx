<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Acceso.aspx.cs" Inherits="ProyectoParcialWebForms.Acceso" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Ingresar</title>
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
        .login { max-width: 440px; margin: 26px auto; }
        .field { margin-bottom: 14px; }
        .nota { color: var(--muted); font-size: .8rem; margin-top: 14px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="top">
            <h1>🔐 Ingresar</h1>
            <p>Use su identificación y el correo con el que se registró.</p>
            <%= ProyectoParcialWebForms.Seguridad.Navegacion.Render() %>
        </div>
        <div class="wrap">
            <div class="card login">
                <h2>Acceso de participantes y organizadores</h2>
                <asp:Label ID="lblMensaje" runat="server" CssClass="msg" />
                <div class="field">
                    <label>Número de identificación</label>
                    <asp:TextBox ID="txtIdentificacion" runat="server" CssClass="ctl" MaxLength="12" />
                </div>
                <div class="field">
                    <label>Correo electrónico registrado</label>
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="ctl" MaxLength="120" />
                </div>
                <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" CssClass="btn btn-primary" OnClick="btnIngresar_Click" />
                <p class="nota">¿Aún no se ha registrado? Use «Registro de participantes» en el menú.</p>
            </div>
        </div>
    </form>
</body>
</html>
