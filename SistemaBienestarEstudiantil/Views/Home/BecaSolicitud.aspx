<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Solicitud de Beca
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    <script type="text/javascript" src="../../Scripts/Controllers/becasolicitud.js?nocache=<%=RandomNumber%>"></script>

	<div id="messages"></div>

    <h2>Solicitud de Beca del estudiante 1722776950, 1712755535, 1719089482, 1500788250</h2>
    <div ng-controller="BecaSolicitudController as Main">
    	<form id="becaSolicitudForm" name="becaSolicitudForm">
	    	<div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
	    	<center><img style="width:170px;" src="../../Content/logo-universidad-israel.png"></center>
	    	<input ng-model="ALUMNO.DTPCEDULAC" ng-required="true" valid-identification style="width:200px;height:25px;padding:5px;font-size:18px;"
				name="validIdentification" placeholder="Número de cédula" type="text"/>
			<span ng-messages="becaSolicitudForm.validIdentification.$error" style="display: inline-block;">
	            <span ng-message="cedulaValidator" class="help-block ng-message" style="font-size: 18px;">Debe ingresar un número de cédula válido</span>
	            <span ng-message="cedulaExist" class="help-block ng-message" style="font-size: 18px;">Estudiante existe</span>
	            <span ng-message="cedulaChecking" class="help-block ng-message" style="font-size: 18px;">Buscando en la base de datos...</span>
	        </span>
	        <span style="display:inline-block;font-size:18px;">
	        	{{ALUMNO.DATOSPERSONALE.DTPNOMBREC}}{{ALUMNO.DATOSPERSONALE.DTPAPELLIC}}{{ALUMNO.DATOSPERSONALE.DTPAPELLIC2}}
	        </span>
	    	<hr/>
			<div ng-if="BECA_SOLICITUD == null">
				<div class="document-message">Seleccione el tipo de beca que desea solicitar:</div>
				<select ng-change="printText()" required ng-model="seleccion.TIPO" id="becaTipo" name="becaTipo" class="form-control"
		            ng-options="o as o.NOMBRE for o in TIPOS" style="height: 30px;font-size: 16px;font-weight: bold;">
		        </select>
				<span ng-messages="becaSolicitudForm.becaTipo.$error">
					<span ng-message="required" class="help-block ng-message">* Debe ingresar el tipo de Beca</span>
				</span>
			</div>
			<div ng-if="BECA_SOLICITUD != null">
				<div class="document-message-title" style="display:inline-block;color: black;vertical-align: middle;">
					Beca:
				</div>
				<div class="document-message-title" style="display:inline-block;font-size:24px;vertical-align: middle;">
					{{BECA_SOLICITUD.BE_BECA_TIPO.NOMBRE}}
				</div>
			</div>
    	</form>
    	<form id="formFiles" name="formFiles" enctype="multipart/form-data">
    		<div>

	    		<div class="document-message-title" ng-if="seleccion.TIPO != null">
		    		Ingrese los siguientes documentos para la solicitud de Becas de estudio y apoyo económico a estudiantes<br/>
		    		<div style="font-size:12px;margin-top:5px;">
		    			Es obligatorio entregar TAMBIÉN el documento físico en recepción o en el departamento de bienestar estudiantil
		    		</div>
		    	</div>

				<table style="margin-top:10px;">
					<tr ng-repeat="object in hasDocumentoSolicitud(BECA_SOLICITUD)">
						<td>
							<div class="document-message">- Solicitud personal dirigida al Coordinador del Departamento de Bienestar Universitario</div>
							<div>
								<input valid-file-input ng-model="documentoSolicitud" type="file" name="documentoSolicitud" id="documentoSolicitud" accept="image/*, application/pdf"/>
								<span ng-show="formFiles.documentoSolicitud.$error.validFile" class="help-block ng-message" style="font-size: 18px;">* Debe adjuntar documento</span>
					            <span ng-show="formFiles.documentoSolicitud.$error.validFileSize" class="help-block ng-message" style="font-size: 18px;">* Solo se permiten documentos hasta 2MB</span>
					            <span ng-show="formFiles.documentoSolicitud.$error.validFileEmpty" class="help-block ng-message" style="font-size: 18px;">* El fichero está vacío</span>
					            <span ng-show="formFiles.documentoSolicitud.$error.validFileType" class="help-block ng-message" style="font-size: 18px;">* No se admite el tipo de archivo</span>
							</div>
						</td>
					</tr>
					<tr>
						<td>
							<div class="document-message">Otros documentos adicionales necesarios</div>
						</td>
					</tr>
					<tr ng-repeat="tipoDocumento in seleccion.TIPO.BE_BECA_TIPO_DOCUMENTO">
						<td>
							<div class="document-message">- {{tipoDocumento.NOMBRE}}</div>
						</td>
					</tr>
					<tr ng-if="seleccion.TIPO != null">
						<td>
							<div>
								<input name="descripcion" ng-model="descripcion" type="text" placeholder="Ingrese descripción del documento" ng-required="otrosDocumentosSolicitud != null && otrosDocumentosSolicitud != undefined && otrosDocumentosSolicitud != ''" style="width:90%;height:20px;padding:3px;font-size:14px;margin-bottom:5px;" /><br/>
								<input ng-model="otrosDocumentosSolicitud" type="file" name="otrosDocumentosSolicitud" id="otrosDocumentosSolicitud" accept="image/*, application/pdf"/>
					            <span ng-show="formFiles.otrosDocumentosSolicitud.$error.validFileSize" class="help-block ng-message" style="font-size: 18px;">* Solo se permiten documentos hasta 2MB</span>
					            <span ng-show="formFiles.otrosDocumentosSolicitud.$error.validFileEmpty" class="help-block ng-message" style="font-size: 18px;">* El fichero está vacío</span>
					            <span ng-show="formFiles.otrosDocumentosSolicitud.$error.validFileType" class="help-block ng-message" style="font-size: 18px;">* No se admite el tipo de archivo</span>
							</div>
						</td>
					</tr>
				</table>
				
				<input ng-click="uploadFileDataBase()" type="submit" value="Guardar" id="upload"/>

    		</div>
		</form>

		<div ng-if="BECA_SOLICITUD != null">
			<div class="document-message-title">
				Documentos ingresados:
			</div>
			<hr/>
			<div ng-repeat="adjunto in BECA_SOLICITUD.BE_BECA_ADJUNTO" style="width:100%;display:inline-block; padding: 5px;vertical-align:top;">
				<button title="Eliminar" type="button" style="width:22px; padding-left:1px;display:inline-block;vertical-align:top;"
					ng-click="removeAttach(adjunto.CODIGO, adjunto.CODIGOSOLICITUD)">
					<span class="ui-icon ui-icon-trash"></span>
				</button>
				<div class="document-message" style="margin-left:5px;display:inline-block;width:50%;vertical-align:top;">{{adjunto.DESCRIPCION}}</div>
				<div style="width:120px;height:120px;display:inline-block;" >
					<img ng-if="!isPDF(adjunto.CONTENTTYPE)" style="max-width:100%;max-height:100%;" src="../../WebServices/Becas.asmx/getAttach?code={{adjunto.CODIGO}}">
					<a ng-if="isPDF(adjunto.CONTENTTYPE)" href="../../WebServices/Becas.asmx/getAttach?code={{adjunto.CODIGO}}" target="_blank">{{adjunto.NOMBRE}}</a>
				</div>
			</div>
		</div>

		<br/><button ng-click="printConsole()">print scope</button>
    </div>

</asp:Content>
