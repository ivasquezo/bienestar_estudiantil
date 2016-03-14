<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
Encuestas
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    <h2>Encuestas</h2>

    <link href="../../Content/encuestas.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/encuestas.js?nocache=<%=RandomNumber%>"></script>
    
    <div id="messages"></div>
    <div ng-controller="EncuestasController as Main" class="encuestas">
    	<script type="text/ng-template" id="actionsEncuestas.html">
			<div class="ui-grid-cell-contents">
				<button type="button" ng-click="grid.appScope.Main.removeEncuesta(COL_FIELD)"><span class="ui-icon ui-icon-trash"></span></button>
				<button type="button" ng-click="grid.appScope.Main.editEncuesta(COL_FIELD)"><span class="ui-icon ui-icon-pencil"></span></button>
			</div>
        </script>
    	<button type="button" ng-click="addEncuesta()" style="margin-bottom:5px;">Nueva</button><br/>
    	<div ui-grid="gridOptions" style="width:39%;display:inline-block;" class="grid"></div>
    	<div ng-show="mode == 'edit' || mode == 'new'" style="width:60%;display:inline-block;vertical-align:top;">
    	    <form name="formNewEncuesta" id="formNewEncuesta">
	    	    <div class="poll-display">
			    	<textarea id="encuestaTitulo" ng-model="encuesta.TITULO" class="title" placeholder="Título de la encuesta" row="1" required ng-maxlength="150" maxlength="150"></textarea><br/><br/>
			    	
			    	<!-- questions -->
			    	<div ng-repeat="question in encuesta.ENCUESTA_PREGUNTA">
			    		<textarea ng-model="question.TITULO" class="question" placeholder="Pregunta" row="3" required></textarea>
			    		<table border="0" style="margin-bottom:5px;"><tr>
			    			<td><select ng-options="o.v as o.n for o in [{n:'Selección múltiple', v:1},{n:'Selección única', v:2}, {n:'Párrafo', v:3}]"
					    			ng-model="question.TIPO"
					    			ng-change="verifyQuestion(question)" ></select></td>
			    			<td><button ng-show="question.TIPO != 3" ng-click="question.addResponse()">+ Añadir opción de respuesta</button></td>
			    			<td><button ng-show="encuesta.ENCUESTA_PREGUNTA.length > 1" ng-click="encuesta.removeQuestion(question.id)">- Eliminar pregunta</button></td>
			    			<td><input type="checkbox" ng-model="question.REQUERIDO" />* Obligatorio</button></td>
			    		</tr></table>
			    		<!-- responses -->
			    		<div ng-show="question.TIPO != 3" ng-repeat="response in question.ENCUESTA_RESPUESTA" class="response">
				    		<img ng-show="question.TIPO == 2" src="../../Content/radio-button-30.png" class="input-img"/>
				    		<img ng-show="question.TIPO == 1" src="../../Content/checkbox-30.png" class="input-img"/>
				    		<input type="text" ng-model="response.TEXTO" placeholder="Opción de respuesta" ng-required="question.TIPO != 3" />
				    		<button ng-show="question.ENCUESTA_RESPUESTA.length > 1" ng-click="question.removeResponse(response.id)" class="button-remove" title="Elminar opción de respuesta">X</button>
				    	</div>
			    		<div ng-show="question.TIPO == 3">
			    			<textarea class="question" row="4" readonly placeholder="Párrafo de respuesta larga"></textarea>
			    			<br/><br/>
			    		</div>
			    	</div>

			    	<button ng-click="encuesta.addQuestion()">Añadir pregunta</button>
			    	<br/>
			    	<br/>
			    	<button type="submit" ng-disabled="formNewEncuesta.$invalid" ng-click="addNewEncuesta()" ng-show="mode == 'new'">Guardar</button>
			    	<button type="submit" ng-disabled="formNewEncuesta.$invalid" ng-click="saveEncuesta()" ng-show="mode == 'edit'">Guardar</button>
		    	</div>
	    	</form>
    	</div>
    	<div ng-show="mode != 'edit' && mode != 'new'"
    		style="width:60%;display:inline-block;vertical-align:top;text-align:center;height:370px;">
    		<span style="vertical-align:middle;font-size:27px;">Edite o ingrese la encuesta que desee, click en NUEVO.</span>
    	</div>
    	<hr/>
		<div class="poll-preview">
			<div class="content">
		    	<div class="title">{{defaultPoll.TITULO}}</div>
		    	<div ng-repeat="question in defaultPoll.ENCUESTA_PREGUNTA" class="question">
		    		<div style="padding-top:5px;padding-bottom:5px;">{{question.TITULO}} {{question.REQUERIDO ? '*' : ''}}</div>
			    	<div ng-show="question.TIPO != 3" ng-repeat="response in question.ENCUESTA_RESPUESTA" class="response">
			    		<div ng-show="question.TIPO == 1"><input ng-model="response.checked" type="checkbox" value="{{response.id}}" ng-required="question.REQUERIDO && question.answereds.length == 0" ng-click="question.addAnswered(response)"/>{{response.TEXTO}}</div>
			    		<div ng-show="question.TIPO == 2"><input ng-model="prueba2" type="radio" name="response{{question.CODIGO}}"  ng-required="question.REQUERIDO" value="{{response.CODIGO}}" />{{response.TEXTO}}</div>
			    	</div>
			    	<div ng-show="question.type == 3"><textarea ng-model="prueba3" ng-required="question.required" row="4" style="width:100%;"></textarea></div>
		    	</div>
			</div>
		</div>
    </div>
	    
</asp:Content>