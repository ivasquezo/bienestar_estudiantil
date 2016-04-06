<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Becas
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    <script type="text/javascript" src="../../Scripts/Controllers/becas.js?nocache=<%=RandomNumber%>"></script>

	<div id="messages"></div>

    <h2>Becas</h2>
    <div ng-controller="BecasController as Main">
	    <div style="font-size:15px;">Administrar <a href="/Home/BecaSolicitud">BECAS</a></div>
    	<div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
	    
	    <div ui-grid="gridOptions"></div>
	    
	    <br/><div style="font-size:15px;">Administrar tipos de becas</div>
        <button ng-click="addTipoBecaDialog()" style="margin-bottom:5px;margin-top:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Agregar tipo de beca">
            <span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span><span class="ui-button-text">Nuevo</span>
        </button>
        <div ui-grid="gridOptionsTipos"></div>

        <script type="text/ng-template" id="actionsBecas.html">
              <div class="ui-grid-cell-contents">
                <button type="button" ng-click="grid.appScope.Main.removeBeca(COL_FIELD)" title="Eliminar solicitud de beca"><span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editBeca(COL_FIELD)" title="Editar solicitud de beca"><span class="ui-icon ui-icon-pencil"></span></button>
              </div>
        </script>

        <script type="text/ng-template" id="actionsTiposBecas.html">
              <div class="ui-grid-cell-contents">
                <button type="button" ng-click="grid.appScope.Main.removeTipoBeca(COL_FIELD)" title="Eliminar tipo de beca"><span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editTipoBeca(COL_FIELD)" title="Editar tipo de beca"><span class="ui-icon ui-icon-pencil"></span></button>
              </div>
        </script>

        <script type="text/ng-template" id="editBecas.html">
            <fieldset>
                <legend>Cambiar datos de la beca</legend>
                <form name="becaForm">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombreusuario">Nombre de usuario</label>  
                        <div class="col-md-4">
                            <input valid-user-name required ng-model="userCopy.NOMBREUSUARIO" id="nombreusuario" name="nombreusuario" type="text" placeholder="Nombre usuario" class="form-control input-md" style="text-transform:lowercase;">
                            <span ng-messages="becaForm.nombreusuario.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese nombre de usuario</span>
                                <span ng-message="userNameExist" class="help-block ng-message">Existe un usuario con este nombre</span>
                                <span ng-message="userNameValidator" class="help-block ng-message">Debe ingresar un nombre de usuario v&aacute;lido</span>
                                <span ng-message="userNameChecking" class="help-block ng-message">Chequeando la base de datos...</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="OTORGADO">Otorgado</label>  
                        <div class="col-md-4">
                            <input required ng-model="userCopy.OTORGADO" id="OTORGADO" name="OTORGADO" type="number" placeholder="Otorgado" class="form-control input-md" style="height:20px; text-align:right;">
                            <span ng-messages="becaForm.OTORGADO.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese el porcentaje otorgado</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="rubro">Rubro</label>  
                        <div class="col-md-4">
                            <select required ng-model="userCopy.ESTADO" id="rubro" name="rubro" class="form-control"
                                ng-options="o.v as o.n for o in RUBROS" style="height:20px; width:150px;margin-top:5px;"></select>
                            <span ng-messages="becaForm.RUBRO.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese el porcentaje otorgado</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="estadobeca">Estado Beca</label>  
                        <div class="col-md-4">
                            <select required ng-model="userCopy.ESTADOBECA" id="estadobeca" name="estadobeca" class="form-control"
                                ng-options="o.v as o.n for o in ESTADOS" style="height:20px; width:150px;margin-top:5px;"></select>
                            <span ng-messages="becaForm.ESTADOBECA.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese el porcentaje otorgado</span>
                            </span>
                        </div>
                    </div>


                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8" style="margin-top:10px;">
                            <button ng-click="saveEditedUser()" id="buttonsave1" name="buttonsave1" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="editTipoBeca.html">
            <fieldset>
                <legend>Cambiar tipo de beca</legend>
                <form name="becaForm">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="tipoBeca">Nombre tipo de beca</label>  
                        <div class="col-md-4">
                            <textarea required ng-model="tipoBeca.NOMBRE" id="tipoBeca" name="tipoBeca"
                            	placeholder="Nombre de tipo de beca" class="form-control input-md"
                            	style="text-transform: uppercase; margin: 3px; height: 76px; width: 226px;"></textarea>
                            <span ng-messages="becaForm.tipoBeca.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese nombre tipo</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="OTORGADO">Documentos</label>  
                        <div class="col-md-4">
                            <div ng-repeat="documento in tipoBeca.BE_BECA_TIPO_DOCUMENTO">
	                            <textarea required ng-model="documento.NOMBRE" id="tipoBeca" name="tipoBeca"
	                            	placeholder="Nombre de documento" class="form-control input-md"
	                            	style="text-transform:uppercase;margin:3px;height:50px;max-height:50px;width:226px;max-width:226px;"></textarea>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8" style="margin-top:10px;">
                            <button ng-click="saveEditedUser()" id="buttonsave1" name="buttonsave1" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="addTipoBeca.html">
            <fieldset>
                <legend>Cambiar tipo de beca</legend>
                <form name="becaForm">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="tipoBeca">Nombre tipo de beca</label>  
                        <div class="col-md-4">
                            <textarea required ng-model="tipoBeca.NOMBRE" id="tipoBeca" name="tipoBeca"
                            	placeholder="Nombre de tipo de beca" class="form-control input-md"
                            	style="text-transform: uppercase; margin: 3px; height: 76px; width: 226px;"></textarea>
                            <span ng-messages="becaForm.tipoBeca.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese nombre tipo</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="OTORGADO">Documentos</label>  
                        <div class="col-md-4">
                            <div ng-repeat="documento in tipoBeca.BE_BECA_TIPO_DOCUMENTO">
	                            <textarea required ng-model="documento.NOMBRE" id="tipoBeca" name="tipoBeca"
	                            	placeholder="Nombre de documento" class="form-control input-md"
	                            	style="text-transform:uppercase;margin:3px;height:50px;max-height:50px;width:226px;max-width:226px;"></textarea>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8" style="margin-top:10px;">
                            <button ng-click="saveEditedUser()" id="buttonsave1" name="buttonsave1" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

    </div>

</asp:Content>
