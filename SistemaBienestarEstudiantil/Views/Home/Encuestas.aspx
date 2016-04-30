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
    <link href="../../Scripts/angular-chart/angular-chart.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
	<script type="text/javascript" src="../../Scripts/angular-chart/Chart.min.js"></script>
	<script type="text/javascript" src="../../Scripts/angular-chart/angular-chart.js"></script>
	<script type="text/javascript" src="../../Scripts/Controllers/encuestas.js?nocache=<%=RandomNumber%>"></script>
	<style type="text/css">
		@media print {
		    .content_print {
		        background-color: white;
		        height: 100%;
		        width: 100%;
		        position: fixed;
		        top: 0;
		        left: 0;
		        margin: 0;
		        padding: 15px;
		        font-size: 14px;
		        line-height: 18px;
		    }
		}
	</style>
    <div id="messages"></div>
    <div ng-controller="EncuestasController as Main" class="encuestas">
	    <div style="position:fixed;top:0px;left:50%;margin-left:-85px;">
    		<div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
    	</div>
		<script type="text/ng-template" id="actionsEncuestas.html">
			<div class="ui-grid-cell-contents" style="text-align:center">
				<button title="Eliminar" type="button" style="width:22px; padding-left:1px;" ng-click="grid.appScope.Main.removeEncuesta(COL_FIELD)">
					<span class="ui-icon ui-icon-trash"></span>
				</button>
				<button title="Editar" type="button" style="width:22px; padding-left:1px;" ng-click="grid.appScope.Main.editEncuesta(COL_FIELD)">
					<span class="ui-icon ui-icon-pencil"></span>
				</button>
				<input type="checkbox" ng-checked="grid.appScope.Main.getSurverDefaultInput(COL_FIELD)"
					ng-click="grid.appScope.Main.setSurverDefaultInput(COL_FIELD)" style="width:23px;height:23px;margin:0 0 8px 0;"
					title="Habilitar/Deshabilitar encuestas para los estudiantes"/>
				<button type="button" style="width:22px; padding-left:1px;" ng-click="grid.appScope.Main.showReport(COL_FIELD)"
					title="Ver reportes">
					<span class="ui-icon ui-icon-calculator"></span>
				</button>
			</div>
        </script>
    	<div style="width:39%;display:inline-block;min-width: 440px;">
	    	<button ng-click="addEncuesta()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
	    		<span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span>
	    		<span class="ui-button-text">Nueva</span>
	    	</button><br/>
    		<div ui-grid="gridOptions" class="grid"></div>
    	</div>
    	<div ng-show="mode == 'edit' || mode == 'new'" style="width:60%;display:inline-block;vertical-align:top;">
    	    <form name="formNewEncuesta" id="formNewEncuesta">
	    	    <div class="poll-display">
			    	<textarea id="encuestaTitulo" ng-model="encuesta.TITULO" class="title" placeholder="Título de la encuesta" row="1" required ng-maxlength="150" maxlength="150"></textarea><br/><br/>
			    	
			    	<!-- questions -->
			    	<div ng-repeat="question in encuesta.BE_ENCUESTA_PREGUNTA">
			    		<textarea ng-model="question.TITULO" class="question" placeholder="Nueva pregunta de la encuesta" row="3" required maxlength="200"></textarea>
			    		<table border="0" style="margin-bottom:5px;"><tr>
			    			<td><select ng-options="o.v as o.n for o in [{n:'Selección múltiple', v:1},{n:'Selección única', v:2}, {n:'Párrafo', v:3}]"
					    			ng-model="question.TIPO"
					    			ng-change="verifyQuestion(question)" ></select></td>
			    			<td>
			    				<button  ng-show="question.TIPO != 3" ng-click="question.addResponse()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
						    		<span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span>
						    		<span class="ui-button-text">Añadir opción de respuesta</span>
						    	</button>
						    </td>
			    			<td>
			    				<button ng-show="encuesta.BE_ENCUESTA_PREGUNTA.length > 1" ng-click="encuesta.removeQuestion(question.id)" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
						    		<span class="ui-button-icon-primary ui-icon ui-icon-circle-minus"></span>
						    		<span class="ui-button-text">Eliminar pregunta</span>
						    	</button>
			    			</td>
			    			<td><input type="checkbox" ng-model="question.REQUERIDO" />* Obligatorio</button></td>
			    		</tr></table>
			    		<!-- responses -->
			    		<div ng-show="question.TIPO != 3" ng-repeat="response in question.BE_ENCUESTA_RESPUESTA" class="response">
				    		<img ng-show="question.TIPO == 2" src="../../Content/radio-button-30.png" class="input-img"/>
				    		<img ng-show="question.TIPO == 1" src="../../Content/checkbox-30.png" class="input-img"/>
				    		<input type="text" ng-model="response.TEXTO" placeholder="Opción de respuesta" ng-required="question.TIPO != 3" />
				    		<button ng-show="question.BE_ENCUESTA_RESPUESTA.length > 1" ng-click="question.removeResponse(response.id)" 
				    			class="ui-button ui-widget ui-state-default"
				    			role="button" title="Elminar opción de respuesta">
					    		<span class="ui-icon ui-icon-circle-minus"></span>
					    	</button>
				    	</div>
			    		<div ng-show="question.TIPO == 3">
			    			<textarea class="question" row="4" readonly placeholder="Párrafo de respuesta larga"></textarea>
			    			<br/><br/>
			    		</div>
			    	</div>

			    	<button ng-click="encuesta.addQuestion()" style="margin-bottom:5px;margin-top:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
			    		<span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span>
			    		<span class="ui-button-text">Añadir pregunta</span>
			    	</button>
			    	<br/>
			    	<br/>

					<button type="submit" ng-disabled="formNewEncuesta.$invalid" ng-click="addNewEncuesta()" ng-show="mode == 'new'"
						style="margin-bottom:5px;" class="ui-button ui-widget ui-corner-all ui-button-text-icon-primary" role="button">
			    		<span class="ui-button-icon-primary ui-icon ui-icon-disk"></span>
			    		<span class="ui-button-text">Guardar</span>
			    	</button>
					<button type="submit" ng-disabled="formNewEncuesta.$invalid" ng-click="saveEncuesta()" ng-show="mode == 'edit'"
						style="margin-bottom:5px;" class="ui-button ui-widget ui-corner-all ui-button-text-icon-primary" role="button">
			    		<span class="ui-button-icon-primary ui-icon ui-icon-disk"></span>
			    		<span class="ui-button-text">Guardar</span>
			    	</button>
		    	</div>
	    	</form>
    	</div>
    	<div ng-show="mode == 'init'" style="width:60%;display:inline-block;vertical-align:top;text-align:center;height:370px;">
    		<br/>
    		<br/>
    		<span style="vertical-align:middle;font-size:27px;">Edite o ingrese una nueva encuesta, click en el botón "Nueva".</span>
    		<br/>
    		<br/>
    		<a href="/Home/Encuesta" style="vertical-align:middle;font-size:18px;">Clic para visualizar encuesta seleccionada</a>
    	</div>
    	<div ng-show="mode == 'report'" style="display:inline-block;vertical-align:top;width:60%;">
			<div>
				<label for="periodos">Seleccione el período:</label>
				<select required name="periodos" ng-options="o as o.PRDCODIGOI for o in PERIODOS"
					    ng-model="PERIODO">
				</select>
		    	<button ng-click="viewReport()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
		    		<span class="ui-button-icon-primary ui-icon ui-icon-circle-arrow-e"></span>
		    		<span class="ui-button-text">Visualizar reporte</span>
		    	</button>
		    	<button ng-if="encuestaReport != null" onclick="window.print()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
		    		<span class="ui-button-icon-primary ui-icon ui-icon-print"></span>
		    		<span class="ui-button-text">Imprimir</span>
		    	</button>
			</div><br/>
	    	<div ng-show="encuestaReport != null" style="margin-left:5px;width:100%;display:inline-block;vertical-align:top;" class="content_print">
				<div ng-click="cambiarVista('summary')" ng-class="view == 'summary' ? 'selected' : '' " class="title-report">RESUMEN</div>
				<div ng-click="cambiarVista('list')" ng-class="view == 'list' ? 'selected' : '' " class="title-report">LISTA DE GRADUADOS</div>
				<br/>
				<br/>
				<div style="display:inline-block;font-size: 18px;">Periodo encuestado:</div>
				<div style="display:inline-block;font-size: 18px;font-style: italic;color: #5C87B2;">{{PERIODO.PRDCODIGOI}}</div>
				<div style="display:inline-block;font-size: 12px;color: rgba(106, 108, 109, 0.68);">
					({{PERIODO.PRDFECINIF|date:"MM/dd/yyyy"}}
					{{PERIODOSIGUIENTE != null ? " - " : ""}}
					{{restarDias(PERIODOSIGUIENTE.PRDFECINIF,1)|date:"MM/dd/yyyy"}})
				</div><br/>
				<div style="display:inline-block;font-size: 18px;">Encuesta:</div>
				<div style="display:inline-block;font-size: 18px;font-style: italic;color: #5C87B2;">{{encuestaReport.TITULO}}</div>
				<div style="display:inline-block;font-size: 12px;color: rgba(106, 108, 109, 0.68);">({{encuestaReport.encuestados}} respuestas)</div>
				<div ng-show="view == 'summary'">
					<div ng-repeat="pregunta in encuestaReport.preguntas">
						<div class="preguntas">{{pregunta.pregunta}}</div>
						<div class="respuestas" style="display:inline-block;vertical-align: top;">
							<table>
								<tr ng-show="pregunta.tipo != 3" ng-repeat="respuesta in pregunta.respuestas">
									<td class="opcion">{{respuesta.nombre}}</td>
									<td class="valor">
										<div style="display: inline-block;">{{respuesta.cantidad}}</div>
										<div style="font-size:10px;display:inline-block;color:#5C87B2;">({{respuesta.cantidad*100/encuestaReport.encuestados | number:1}}%)</div>
									</td>
								</tr>
								<tr ng-show="pregunta.tipo == 3" ng-repeat="respuesta in pregunta.respuestas">
									<td colspan="2" class="opcion">{{respuesta.parrafo}}</td>
								</tr>
							</table>
						</div>
						<div ng-if="pregunta.tipo != 3" style="display:inline-block;">
							<canvas class="chart chart-pie" chart-data="pregunta.valorRespuestas" chart-labels="pregunta.labelRespuestas"
								width="200" height="120"></canvas>
						</div>
					</div>
				</div>
				<div ng-show="view == 'list'" style="margin-top:10px;">
					<table>
						<tr ng-repeat="estudiante in encuestaReport.estudiantes" class="student-list">
							<td class="id">{{estudiante.DATOSPERSONALE.DTPCEDULAC}}</td>
							<td class="name">{{estudiante.DATOSPERSONALE.DTPNOMBREC}}{{estudiante.DATOSPERSONALE.DTPAPELLIC}}{{estudiante.DATOSPERSONALE.DTPAPELLIC2}}</td>
						</tr>
					</table>
					<div ng-show="encuestaReport.estudiantes.length == 0">
						Encuesta no respondida aún.
					</div>
		    	</div>
	    	</div>
	    </div>
    </div>
	    
</asp:Content>