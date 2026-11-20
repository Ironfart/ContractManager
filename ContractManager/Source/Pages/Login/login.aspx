<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="ContractManager.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.9/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-yEPZ7jP9S0QKiUQ0m3jjsO4ProIW7wvR9gXkI7JOGa2XVsw53Yke2bsdkBbjmmiX" crossorigin="anonymous">
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.9/dist/js/bootstrap.bundle.min.js" integrity="sha384-WyOxWvTyr1QaMPfiPJSv76iReVthorl5uWlFtZPyK0IEB0hGzo9y5Bp9mYanr8Bu" crossorigin="anonymous"></script>
  <title>ContractManager - Login</title>
</head>
<body>
  <form id="form1" runat="server" class="container d-flex justify-content-center align-items-center">
    <div class="col-5">
      <div class="form-control card card-body align-items-center h3">
        <asp:Label runat="server" Font-Size="Larger">Login</asp:Label>
      </div>
      <br />
      <div class="input-group">
        <asp:TextBox runat="server" CssClass="form-control" placeholder="Usuario" ID="txtUser" />
      </div>

      <br />
      <div class="input-group">
        <asp:TextBox runat="server" CssClass="form-control" placeholder="Contraseña" TextMode="Password" ID="txtPassword" />
      </div>

      <br />
      <div class="input-group">
        <asp:Button runat="server" CssClass="form-control btn btn-dark" Text="Login" OnClick="onLogin"/>
      </div>
      <br />
      <br />
      <div>
        <asp:Label ID="lblMessage" runat="server" class="alert-danger"></asp:Label>
      </div>
    </div>
  </form>
</body>
</html>