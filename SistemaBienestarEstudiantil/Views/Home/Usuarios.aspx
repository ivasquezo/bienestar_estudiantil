﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Acerca de nosotros
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <div ng-controller="UsuarioController as Main">
        <div ui-grid="gridOptions"></div>
        <script type="text/ng-template" id="actionsUsers.html">
              <div class="ui-grid-cell-contents">
                <button class="btn" type="button" ng-click="grid.appScope.Main.removeUser(COL_FIELD)">Borrar</button>
                <button class="btn" type="button" ng-click="grid.appScope.Main.editUser(COL_FIELD)">Editar</button>
              </div>
        </script>
        <script type="text/ng-template" id="editUser.html">
            
            <fieldset>
                <legend>Editar usuario</legend>

                <div class="form-group">
                  <label class="col-md-4 control-label" for="usuario">Nombre de usuario</label>  
                  <div class="col-md-4">
                  <input id="usuario" name="usuario" type="text" placeholder="Nombre usuario" class="form-control input-md">
                  <br/><span class="help-block">Nombre de inicio de sesión</span>  
                  </div>
                </div>

                <div class="form-group">
                  <label class="col-md-4 control-label" for="nombreusuario">Nombre completo:</label>  
                  <div class="col-md-4">
                  <input id="nombreusuario" name="nombreusuario" type="text" placeholder="Nombre" class="form-control input-md">
                  <br/><span class="help-block">Nombre completo del usuario</span>  
                  </div>
                </div>

                <div class="form-group">
                  <label class="col-md-4 control-label" for="cedulausuario">Cédula:</label>  
                  <div class="col-md-4">
                  <input id="cedulausuario" name="cedulausuario" type="text" placeholder="Cédula" class="form-control input-md">
                  <br/><span class="help-block">Ingrese la cédula del usuario</span>  
                  </div>
                </div>

                <div class="form-group">
                  <label class="col-md-4 control-label" for="rolusuario">Seleccione el rol:</label>
                  <div class="col-md-4">
                    <select id="rolusuario" name="rolusuario" class="form-control">
                      <option value="1">Administrador</option>
                      <option value="2">Mentor</option>
                    </select>
                  </div>
                </div>

                <div class="form-group">
                  <label class="col-md-4 control-label" for="estadousuario">Select Basic</label>
                  <div class="col-md-4">
                    <select id="estadousuario" name="estadousuario" class="form-control">
                      <option value="0">Activado</option>
                      <option value="1">Desactivado</option>
                    </select>
                  </div>
                </div>

                <div class="form-group">
                  <label class="col-md-4 control-label" for="buttonsave1"></label>
                  <div class="col-md-8">
                    <button id="buttonsave1" name="buttonsave1" class="btn btn-success">Registrar</button>
                    <button id="buttoncancel1" name="buttoncancel1" class="btn btn-danger">Cancelar</button>
                  </div>
                </div>
            </fieldset>            
        
        </script>
    </div>
    <p>
        <input type="submit" value="Iniciar sesión" style="float:right" />
    </p>
    <h2>
        Usuarios</h2>
    <p>
        Incluya aquí el contenido.
    </p>
</asp:Content>
