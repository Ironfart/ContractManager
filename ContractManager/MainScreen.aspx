<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainScreen.aspx.cs" Inherits="ContractManager.MainScreen" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ContractManager - Main</title>
</head>
<body>
  <form id="form1" runat="server">
    <h2>Bienvenido</h2>
    <asp:Label ID="lblMsg" runat="server" />
    <div>
      <asp:Button Text="Cerrar sesión" runat="server" OnClick="onLogout" />
    </div>
  </form>
</body>
</html>
