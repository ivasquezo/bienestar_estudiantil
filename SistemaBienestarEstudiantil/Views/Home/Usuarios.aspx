<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="SistemaBienestarEstudiantil.Class" %>
<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Usuarios
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    <script type="text/javascript" src="<%=Utils.APP_CONTEXT%>/Scripts/Controllers/usuarios.js?nocache=<%=RandomNumber%>"></script>
    <h2>Usuarios</h2>
    <div id="messages"></div>
    <div ng-controller="UsuariosController as Main">
        <div style="position:fixed; top:0px; left:50%; margin-left:-85px;">
            <div cg-busy="{promise:promise, message:message, backdrop:backdrop, delay:delay, minDuration:minDuration}"></div>
        </div>
        <button ng-click="addNewUserDialog()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary"
            role="button" title="Agregar usuario">
            <span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span>
            <span class="ui-button-text">Nuevo</span>
        </button>
        <div ui-grid="gridOptions"></div>
        <script type="text/ng-template" id="actionsUsers.html">
              <div class="ui-grid-cell-contents" style="text-align:center">
                <button type="button" ng-click="grid.appScope.Main.editUser(COL_FIELD)" title="Editar usuario"><span class="ui-icon ui-icon-pencil"></span></button>
              </div>
        </script>
        <style type="text/css">
            .dialogClassTable, table.dialogClassTable tr, table.dialogClassTable td
            {
                border: none !important;
            }
        </style>
        <script type="text/ng-template" id="newUser.html">
            <fieldset>
                <legend>Nuevo usuario</legend>
                <form id="newUserForm" name="newUserForm" ng-submit="addNewUserDB()">
                    <table class="dialogClassTable">
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="nombreusuario">Nombre de usuario</label>  
                                    <div class="col-md-4">
                                        <input valid-user-name required ng-model="userCopy.NOMBREUSUARIO" id="nombreusuario" name="nombreusuario" type="text" placeholder="Nombre usuario" class="form-control input-md" style="text-transform:lowercase;">
                                        <span class="help-block">Nombre de inicio de sesi&oacute;n</span>
                                        <span ng-messages="newUserForm.nombreusuario.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese un nombre de usuario</span>
                                            <span ng-message="userNameExist" class="help-block ng-message">Existe un usuario con este nombre</span>
                                            <span ng-message="userNameValidator" class="help-block ng-message">Debe ingresar un nombre de usuario v&aacute;lido</span>
                                            <span ng-message="userNameChecking" class="help-block ng-message">Chequeando la base de datos...</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="nombrecompleto">Nombre completo</label>  
                                    <div class="col-md-4">
                                        <input required ng-model="userCopy.NOMBRECOMPLETO" id="nombrecompleto" name="nombrecompleto" type="text" placeholder="Nombre" class="form-control input-md" style="text-transform:uppercase;">
                                        <span class="help-block">Nombre completo</span>
                                        <span ng-messages="newUserForm.nombrecompleto.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese un nombre completo</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="cedulausuario">C&eacute;dula:</label>  
                                    <div class="col-md-4">
                                        <input valid-identification required ng-model="userCopy.CEDULA" id="cedulausuario" name="cedulausuario" type="text" placeholder="C&eacute;dula" class="form-control input-md">
                                        <span class="help-block">N&uacute;mero de identificaci&oacute;n</span>
                                        <span ng-messages="newUserForm.cedulausuario.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese una identificaci&oacute;n</span>
                                            <span ng-message="cedulaValidator" class="help-block ng-message">Debe ingresar un n&uacute;mero de c&eacute;dula v&aacute;lido</span>
                                            <span ng-message="cedulaExist" class="help-block ng-message">Existe un usuario con este n&uacute;mero de c&eacute;dula</span>
                                            <span ng-message="cedulaChecking" class="help-block ng-message">Chequeando la base de datos...</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>                               
                                <div>
                                    <label class="col-md-4 control-label" for="correousuario">Correo:</label>  
                                    <div class="col-md-4">
                                        <input required ng-model="userCopy.CORREO" name="correousuario" id="correousuario" type="email" placeholder="Correo del usuario" class="form-control input-md">
                                        <span class="help-block">Correo electr&oacute;nico</span>
                                        <span ng-messages="newUserForm.correousuario.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese una direcci&oacute;n de correo</span>
                                            <span ng-message="email" class="help-block ng-message">Ingrese correctamente la direcci&oacute;n de correo</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="activeUserBox">Estado</label>
                                    <div class="col-md-4">
                                        <select ng-model="userCopy.ESTADO" id="activeUserBox" name="activeUserBox" class="form-control"
                                            ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: false }, { n: 'Activo', v: true }]">
                                        </select>
                                        <span class="help-block">Estado del usuario</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="activeUserBox">Rol</label>
                                    <div class="col-md-4">
                                        <select name="rol" ng-model="userCopy.CODIGOROL" id="activeUserBox" name="activeUserBox" class="form-control"
                                            ng-options="o.CODIGO as o.NOMBRE for o in Rols" required>
                                        </select>
                                        <span class="help-block">Rol al que pertenece el usuario</span>
                                        <span ng-messages="newUserForm.rol.$error">
                                            <span ng-message="required" class="help-block ng-message">Seleccione un rol para el usuario</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8">
                            <button type="submit" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="editUser.html">
            <fieldset>
                <legend>Editar usuario</legend>
                <form name="userForm">
                    <table class="dialogClassTable">
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="nombreusuario">Nombre de usuario</label>  
                                    <div class="col-md-4">
                                        <input valid-user-name required ng-model="userCopy.NOMBREUSUARIO" id="nombreusuario" name="nombreusuario" type="text" placeholder="Nombre usuario" class="form-control input-md" style="text-transform:lowercase;">
                                        <span class="help-block">Nombre de inicio de sesi&oacute;n</span>
                                        <span ng-messages="userForm.nombreusuario.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese nombre de usuario</span>
                                            <span ng-message="userNameExist" class="help-block ng-message">Existe un usuario con este nombre</span>
                                            <span ng-message="userNameValidator" class="help-block ng-message">Debe ingresar un nombre de usuario v&aacute;lido</span>
                                            <span ng-message="userNameChecking" class="help-block ng-message">Chequeando la base de datos...</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="nombrecompleto">Nombre completo</label>  
                                    <div class="col-md-4">
                                        <input required ng-model="userCopy.NOMBRECOMPLETO" id="nombrecompleto" name="nombrecompleto" type="text" placeholder="Nombre" class="form-control input-md" style="text-transform:uppercase;">
                                        <span class="help-block">Nombre completo</span>
                                        <span ng-messages="userForm.nombrecompleto.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese nombre completo</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="cedulausuario">C&eacute;dula:</label>  
                                    <div class="col-md-4">
                                        <input valid-identification required ng-model="userCopy.CEDULA" id="cedulausuario" name="cedulausuario" type="text" placeholder="C&eacute;dula" class="form-control input-md">
                                        <span class="help-block">N&uacute;mero de identificaci&oacute;n</span>
                                        <span ng-messages="userForm.cedulausuario.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese una identificaci&oacute;n</span>
                                            <span ng-message="cedulaValidator" class="help-block ng-message">Debe ingresar un n&uacute;mero de c&eacute;dula v&aacute;lido</span>
                                            <span ng-message="cedulaExist" class="help-block ng-message">Existe un usuario con este n&uacute;mero de c&eacute;dula</span>
                                            <span ng-message="cedulaChecking" class="help-block ng-message">Chequeando la base de datos...</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="correousuario">Correo:</label>  
                                    <div class="col-md-4">
                                        <input ng-model="userCopy.CORREO" name="correousuario" id="correousuario" placeholder="Correo del usuario" class="form-control input-md" required type="email">
                                        <span class="help-block">Correo electr&oacute;nico</span>
                                        <span ng-messages="userForm.correousuario.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese una direcci&oacute;n de correo</span>
                                            <span ng-message="email" class="help-block ng-message">Ingrese correctamente la direcci&oacute;n de correo</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="activeUserBox">Estado</label>
                                    <div class="col-md-4">
                                        <select ng-model="userCopy.ESTADO" id="activeUserBox" name="activeUserBox" class="form-control"
                                            ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: false }, { n: 'Activo', v: true }]">
                                        </select>
                                        <span class="help-block">Estado del usuario</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="activeUserBox">Rol</label>
                                    <div class="col-md-4">
                                        <select ng-model="userCopy.CODIGOROL" id="activeUserBox" name="activeUserBox" class="form-control"
                                            ng-options="o.CODIGO as o.NOMBRE for o in Rols">
                                        </select>
                                        <span class="help-block">Rol al que pertenece el usuario</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="activeUserBox">Reiniciar</label>
                                    <div class="col-md-4">
                                        <select ng-model="password.reset" id="activeUserBox" name="activeUserBox" class="form-control"
                                            ng-options="o.v as o.n for o in [{ n: 'No', v: false }, { n: 'Si', v: true }]">
                                        </select>
                                        <span class="help-block">Reiniciar contrase&ntilde;a por defecto</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8">
                            <button ng-click="saveEditedUser()" id="buttonsave1" name="buttonsave1" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>
    </div>
</asp:Content>
