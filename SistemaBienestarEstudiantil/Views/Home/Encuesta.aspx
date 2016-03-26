<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Encuesta
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    <h2>Responder encuesta</h2>

    <link href="../../Content/encuestas.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/encuesta.js?nocache=<%=RandomNumber%>"></script>
	<div id="messages"></div>
    <div ng-controller="EncuestaController as Main" class="encuestas">
    	<div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
    	<hr/>
		<form id="formEncuesta" name="formEncuesta" ng-submit="enviarForm()">
		<div ng-show="defaultSurvey != null">
			<input ng-model="student.CEDULA" ng-required="true" valid-identification style="width:200px;height:25px;padding:5px;font-size:18px;"
				name="validIdentification" placeholder="Número de cédula" type="number"/>
			<span ng-messages="formEncuesta.validIdentification.$error" style="display: inline-block;">
	            <span ng-message="cedulaValidator" class="help-block ng-message" style="font-size: 18px;">Debe ingresar un número de cédula válido</span>
	            <span ng-message="cedulaSurveyDone" class="help-block ng-message" style="font-size: 18px;">Usted ya realizó la encuesta</span>
	            <span ng-message="cedulaChecking" class="help-block ng-message" style="font-size: 18px;">Chequeando la base de datos...</span>
	        </span>
	        <span style="display:inline-block;font-size:18px;">
	        	{{student.NOMBRE}}
	        </span>
	    	<hr/>
		</div>
		<div class="poll-preview">
			<div class="content" ng-show="defaultSurvey != null">
		    	<div class="title">{{defaultSurvey.TITULO}}</div>
		    	<div ng-repeat="question in defaultSurvey.ENCUESTA_PREGUNTA" class="question">
		    		<div style="padding-top:5px;padding-bottom:5px;">{{question.TITULO}} {{question.REQUERIDO ? '*' : ''}}</div>
			    	<div ng-show="question.TIPO != 3" ng-repeat="response in question.ENCUESTA_RESPUESTA" class="response">
			    		<div ng-show="question.TIPO == 1">
			    			<input ng-model="response.checked" type="checkbox" value="{{response.id}}"
			    				ng-required="question.REQUERIDO && question.answereds.length == 0 && question.TIPO == 1"
			    				ng-click="question.addAnswered(response)"/>
			    				{{response.TEXTO}}
			    		</div>
			    		<div ng-show="question.TIPO == 2"><input ng-model="question.VALUE" type="radio" name="question{{question.CODIGO}}"
			    			ng-required="question.REQUERIDO && (question.VALUE == '' || question.VALUE == null || question.VALUE == undefined) && question.TIPO == 2"
			    			value="{{response.CODIGO}}" />
			    			{{response.TEXTO}}
			    		</div>
			    	</div>
			    	<div ng-show="question.TIPO == 3"><textarea ng-model="question.VALUE" ng-required="question.REQUERIDO && question.TIPO == 3" row="4"
			    		style="width:100%;box-sizing:border-box;"></textarea></div>
		    	</div>
		    	<br/>
		    	<br/>
		    	<center>
		    		<button type="submit" style="margin-bottom:5px;"
		    			class="ui-button ui-widget ui-corner-all ui-button-text-icon-primary" role="button">
			    		<span class="ui-button-icon-primary ui-icon ui-icon-mail-closed"></span>
			    		<span class="ui-button-text">Enviar</span>
			    	</button>
			    </center>
			</div>
			<div class="content" ng-show="defaultSurvey == null">
				No se ha habilitado ninguna encuesta, consulte al administrador del sitio.
			</div>

		</form>
		</div>
    </div>

</asp:Content>