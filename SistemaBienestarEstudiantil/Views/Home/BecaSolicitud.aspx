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
	            <span ng-message="cedulaChecking" class="help-block ng-message" style="font-size: 18px;">Chequeando la base de datos...</span>
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
				<div class="document-message-title" style="display:inline-block;color: black;">
					Beca:
				</div>
				<div class="document-message-title" style="display:inline-block;">
					{{BECA_SOLICITUD.BE_BECA_TIPO.NOMBRE}}
				</div>
			</div>
    	</form>
    	<form ng-if="BECA_SOLICITUD == null || true" id="formFiles" name="formFiles" enctype="multipart/form-data">
    		<div>

	    		<div class="document-message-title" ng-if="seleccion.TIPO != null">
		    		Ingrese los siguientes documentos para la solicitud de Becas de estudio y apoyo económico a estudiantes<br/>
		    		<div style="font-size:12px;margin-top:5px;">
		    			Es obligatorio entregar TAMBIÉN el documento físico en recepción o en el departamento de bienestar estudiantil
		    		</div>
		    	</div>

				<input type="hidden" value="{{getCodeTypesDocuments(seleccion.TIPO.BE_BECA_TIPO_DOCUMENTO)}}" name="codesTypesDocuments" />

				<table style="margin-top:10px;">
					<tr>
						<td>
							<div class="document-message">- Solicitud personal dirigida al Coordinador del Departamento de Bienestar Universitario</div>
							<div>
							<ng-form name="innerForm">
								<input valid-file-input ng-model="documentosSolicitud[tipoDocumento.CODIGO]" type="file" name="documentosSolicitud" id="documentosSolicitud" accept="image/*, application/pdf"/>
								<span ng-show="innerForm.documentosSolicitud.$error.validFile" class="help-block ng-message" style="font-size: 18px;">* Debe adjuntar documento</span>
					            <span ng-show="innerForm.documentosSolicitud.$error.validFileSize" class="help-block ng-message" style="font-size: 18px;">* Solo se permiten documentos hasta 2MB</span>
					            <span ng-show="innerForm.documentosSolicitud.$error.validFileEmpty" class="help-block ng-message" style="font-size: 18px;">* El fichero está vacío</span>
					            <span ng-show="innerForm.documentosSolicitud.$error.validFileType" class="help-block ng-message" style="font-size: 18px;">* No se admite el tipo de archivo</span>
							</ng-form>
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
							<input ng-model="descripcion" type="text" placeholder="Ingrese descripción del documento" ng-required="false" style="width:90%;height:20px;padding:3px;font-size:14px;margin-bottom:5px;" /><br/>
							<ng-form name="innerForm">
								<input ng-model="otrosDocumentosSolicitud[tipoDocumento.CODIGO]" type="file" name="otrosDocumentosSolicitud" id="otrosDocumentosSolicitud" accept="image/*, application/pdf"/>
					            <span ng-show="innerForm.otrosDocumentosSolicitud.$error.validFileSize" class="help-block ng-message" style="font-size: 18px;">* Solo se permiten documentos hasta 2MB</span>
					            <span ng-show="innerForm.otrosDocumentosSolicitud.$error.validFileEmpty" class="help-block ng-message" style="font-size: 18px;">* El fichero está vacío</span>
					            <span ng-show="innerForm.otrosDocumentosSolicitud.$error.validFileType" class="help-block ng-message" style="font-size: 18px;">* No se admite el tipo de archivo</span>
							</ng-form>
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
			<div ng-repeat="adjunto in CODIGOSADJUNTOS" style="width:100%;display:inline-block; padding: 5px;vertical-align:top;">
				<button title="Eliminar" type="button" style="width:22px; padding-left:1px;display:inline-block;vertical-align:top;"
					ng-click="removeAttach(adjunto.CODIGO)">
					<span class="ui-icon ui-icon-trash"></span>
				</button>
				<div class="document-message" style="margin-left:5px;display:inline-block;width:50%;vertical-align:top;">{{adjunto.BE_BECA_TIPO_DOCUMENTO.NOMBRE}}</div>
				<div style="width:120px;height:120px;display:inline-block;" >
					<img style="max-width:100%;max-height:100%;" src="../../WebServices/Becas.asmx/getImage?codigoAdjunto={{adjunto.CODIGO}}">
				</div>
			</div>
		</div>

		<br/><button ng-click="printConsole()">print</button>
    </div>

</asp:Content>
