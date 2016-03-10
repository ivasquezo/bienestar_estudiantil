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
    <form id="formEncuesta">
    <div ng-controller="EncuestasController as Main" class="encuestas">
    
	    <div class="poll-display">
	    	<textarea id="encuestaTitulo" style="height:1em;" ng-model="poll.title" class="title question" placeholder="Título de la encuesta" row="1"></textarea><br/><br/>
	    	
	    	<!-- questions -->
	    	<div ng-repeat="question in poll.questions">
	    		<textarea ng-model="question.title" class="question" placeholder="Pregunta" row="1"></textarea>
	    		<select ng-options="o.v as o.n for o in [{n:'Selección Múltiple', v:1},{n:'Selección única', v:2}, {n:'Párrafo', v:3}]"
	    			ng-model="question.type"></select><br/><br/>

	    		<!-- responses -->
	    		<div ng-repeat="response in question.responses">
		    		<input type="checkbox" ng-show="question.type == 1"/>
		    		<input type="radio" ng-show="question.type == 2" name="response{{poll.questions.length}}"/>
		    		<input type="text" ng-model="question.text" placeholder="Respuesta" /><br/><br/>
		    	</div>
		    	<button ng-click="question.addResponse()">Añadir respuesta</button><br/><br/>
	    	</div>

	    	<button ng-click="addQuestion()">Añadir pregunta</button>
    	</div>
		
		<div class="poll-display">
	    	<span>{{poll.title}}</span></span><br/><br/>
	    	<div ng-repeat="question in poll.questions">
	    		<span>{{question.title}}</span><br/><br/>
	    	</div>
		</div>
		<br/><br/><button type="submit" >Guardar encuesta</button>
    </div>
    </form>
</asp:Content>