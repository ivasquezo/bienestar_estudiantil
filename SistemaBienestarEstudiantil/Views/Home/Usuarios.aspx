<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Usuarios
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>

    <script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/usuarios.js?nocache=<%=RandomNumber%>"></script>
    
    <h2>Usuarios</h2>

    <div id="messages"></div>

    <div ng-controller="UsuariosController as Main">
        <div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
        <button ng-click="addNewUserDialog()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
            <span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span>
            <span class="ui-button-text">Nuevo</span>
        </button><br/>

        <div ui-grid="gridOptions"></div>

        <script type="text/ng-template" id="actionsUsers.html">
              <div class="ui-grid-cell-contents">
                <button type="button" ng-click="grid.appScope.Main.removeUser(COL_FIELD)"><span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editUser(COL_FIELD)"><span class="ui-icon ui-icon-pencil"></span></button>
              </div>
        </script>

        <script type="text/ng-template" id="editUser.html">
            <fieldset>
                <legend>Editar usuario</legend>
                <form name="userForm">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombreusuario">Nombre de usuario</label>  
                        <div class="col-md-4">
                            <input required ng-model="userCopy.NOMBREUSUARIO" id="nombreusuario" name="nombreusuario" type="text" placeholder="Nombre usuario" class="form-control input-md">
                            <br/><span class="help-block">Nombre de inicio de sesión</span>  
                            <span ng-messages="userForm.nombreusuario.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese nombre de usuario</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombrecompleto">Nombre completo</label>  
                        <div class="col-md-4">
                            <input required ng-model="userCopy.NOMBRECOMPLETO" id="nombrecompleto" name="nombrecompleto" type="text" placeholder="Nombre" class="form-control input-md">
                            <br/><span class="help-block">Nombre completo</span>  
                            <span ng-messages="userForm.nombrecompleto.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese nombre completo</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="cedulausuario">Cédula:</label>  
                        <div class="col-md-4">
                            <input ng-model="userCopy.CEDULA" type="text" id="cedulausuario" name="cedulausuario" placeholder="Cédula" required class="form-control input-md">
                            <br/><span class="help-block">Ingrese la cédula del usuario</span>  
                            <span ng-messages="userForm.cedulausuario.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese la identificación</span>
                                <span ng-message="available" class="help-block ng-message">Ya existe un usuario ingresado con esta identificación</span>
                            </span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="correousuario">Correo:</label>  
                        <div class="col-md-4">
                            <input ng-model="userCopy.CORREO" name="correousuario" id="correousuario" placeholder="Correo del usuario" class="form-control input-md" required type="email">
                            <br/><span class="help-block">Ingrese el correo del usuario</span>
                            <span ng-messages="userForm.correousuario.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese una dirección de correo</span>
                                <span ng-message="email" class="help-block ng-message">Ingrese correctamente la dirección de correo</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="activeUserBox">Estado</label>
                        <div class="col-md-4">
                            <select ng-model="userCopy.ESTADO" id="activeUserBox" name="activeUserBox" class="form-control"
                                ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: false }, { n: 'Activo', v: true }]">
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="activeUserBox">Rol</label>
                        <div class="col-md-4">
                            <select ng-model="userCopy.ESTADO" id="activeUserBox" name="activeUserBox" class="form-control"
                                ng-options="o.CODIGO as o.NOMBRE for o in Rols">
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="activeUserBox">Reiniciar</label>
                        <div class="col-md-4">
                            <select ng-model="password.reset" id="activeUserBox" name="activeUserBox" class="form-control"
                                ng-options="o.v as o.n for o in [{ n: 'No', v: false }, { n: 'Si', v: true }]">
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8">
                            <button ng-click="saveEditedUser()" id="buttonsave1" name="buttonsave1" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="newUser.html">
            <fieldset>
                <legend>Nuevo usuario</legend>
                <form name="newUserForm" ng-submit="addNewUserDB()">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombreusuario">Nombre de usuario</label>  
                        <div class="col-md-4">
                            <input required ng-model="userCopy.NOMBREUSUARIO" id="nombreusuario" name="nombreusuario" type="text" placeholder="Nombre usuario" class="form-control input-md">
                            <span ng-messages="newUserForm.nombreusuario.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese un nombre de usuario</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombrecompleto">Nombre completo</label>  
                        <div class="col-md-4">
                            <input required ng-model="userCopy.NOMBRECOMPLETO" id="nombrecompleto" name="nombrecompleto" type="text" placeholder="Nombre" class="form-control input-md">
                            <span ng-messages="newUserForm.nombrecompleto.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese un nombre completo</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="cedulausuario">Cédula:</label>  
                        <div class="col-md-4">
                            <input required ng-model="userCopy.CEDULA" id="cedulausuario" name="cedulausuario" type="text" placeholder="Cédula" class="form-control input-md">
                            <span ng-messages="newUserForm.cedulausuario.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese una identificación</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="correousuario">Correo:</label>  
                        <div class="col-md-4">
                            <input required ng-model="userCopy.CORREO" name="correousuario" id="correousuario" type="email" placeholder="Correo del usuario" class="form-control input-md">
                            <span ng-messages="newUserForm.correousuario.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese un correo</span>
                                <span ng-message="email" class="help-block ng-message">Ingrese correctamente el correo</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="activeUserBox">Estado</label>
                        <div class="col-md-4">
                            <select ng-model="userCopy.ESTADO" id="activeUserBox" name="activeUserBox" class="form-control"
                                ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: false }, { n: 'Activo', v: true }]">
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8">
                            <button type="submit" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>
    </div>
</asp:Content>
