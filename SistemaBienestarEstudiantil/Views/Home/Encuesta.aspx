<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Encuesta
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    <h2>Encuesta</h2>

    <link href="../../Content/encuestas.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/encuesta.js?nocache=<%=RandomNumber%>"></script>
    <div id="messages"></div>
    <div ng-controller="EncuestaController as Main" class="encuestas">
    	<hr/>
		<form name="formEncuesta" ng-submit="enviarForm()">
		<input ng-model="studientID" ng-required="true" valid-id style="width: 200px;height: 25px;padding: 5px;"
			placeholder="Ingrese su número de cédula" type="number"/>
    	<hr/>
		<div class="poll-preview">
			<div class="content" ng-show="defaultPoll != null">
		    	<div class="title">{{defaultPoll.TITULO}}</div>
		    	<div ng-repeat="question in defaultPoll.ENCUESTA_PREGUNTA" class="question">
		    		<div style="padding-top:5px;padding-bottom:5px;">{{question.TITULO}} {{question.REQUERIDO ? '*' : ''}}</div>
			    	<div ng-show="question.TIPO != 3" ng-repeat="response in question.ENCUESTA_RESPUESTA" class="response">
			    		<div ng-show="question.TIPO == 1"><input ng-model="response.checked" type="checkbox" value="{{response.id}}" ng-required="question.REQUERIDO && question.answereds.length == 0 && question.TIPO == 1" ng-click="question.addAnswered(response)"/>{{response.TEXTO}}</div>
			    		<div ng-show="question.TIPO == 2"><input ng-model="question.VALUE" type="radio" name="question{{question.CODIGO}}"
			    			ng-required="question.REQUERIDO && (question.VALUE == '' || question.VALUE == null || question.VALUE == undefined)"
			    			value="{{response.CODIGO}}" />{{response.TEXTO}}</div>
			    	</div>
			    	<div ng-show="question.type == 3"><textarea ng-model="prueba3" ng-required="question.required" row="4" style="width:100%;"></textarea></div>
		    	</div>
		    	<br/>
		    	<br/>
		    	<center>
		    		<button type="submit" ng-click="encuesta.removeQuestion(question.id)" style="margin-bottom:5px;"
		    			class="ui-button ui-widget ui-corner-all ui-button-text-icon-primary" role="button">
			    		<span class="ui-button-icon-primary ui-icon ui-icon-mail-closed"></span>
			    		<span class="ui-button-text">Enviar</span>
			    	</button>
			    </center>

			    prueba {{formEncuesta.$invalid}}
			</div>
			<div class="content" ng-show="defaultPoll == null">
				No se ha definido la encuesta, consulte al administrador del sitio.
			</div>
		</form>
		</div>
    </div>

</asp:Content>
