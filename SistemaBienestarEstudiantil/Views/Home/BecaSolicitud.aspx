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

    <h2>Solicitud de Beca del estudiante</h2>
    <div ng-controller="BecaSolicitudController as Main">
    	<form id="becaSolicitudForm" name="becaSolicitudForm">
	    	<div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
	    	<center><img style="width:210px;" src="../../Content/logo-universidad-israel.png"></center>
	    	<input ng-model="student.CEDULA" ng-required="true" valid-identification style="width:200px;height:25px;padding:5px;font-size:18px;"
				name="validIdentification" placeholder="Número de cédula" type="number"/>
			<span ng-messages="becaSolicitudForm.validIdentification.$error" style="display: inline-block;">
	            <span ng-message="cedulaValidator" class="help-block ng-message" style="font-size: 18px;">Debe ingresar un número de cédula válido</span>
	            <span ng-message="cedulaExist" class="help-block ng-message" style="font-size: 18px;">Estudiante existe</span>
	            <span ng-message="cedulaChecking" class="help-block ng-message" style="font-size: 18px;">Chequeando la base de datos...</span>
	        </span>
	        <span style="display:inline-block;font-size:18px;">
	        	{{student.NOMBRE}}
	        </span>
	    	<hr/>
	    	<div>Documentos para la solicitud de Becas de estudio y apoyo económico a estudiantes</div>
	        <div>a.	Solicitud personal dirigida al Coordinador del Departamento de Bienestar Universitario.</div>
			<div>b.	Copia de la cédula a color del solicitante.</div>
			<br/><div>Seleccione el tipo de beca que desea solicitar:</div>
			<select required ng-model="becasolicitud.TIPO" id="becaTipo" name="becaTipo" class="form-control"
	            ng-options="o as o.NOMBRE for o in TIPOS" style="height: 30px;font-size: 16px;font-weight: bold;">
	        </select>
			<span ng-messages="becaSolicitudForm.becaTipo.$error">
				<span ng-message="required" class="help-block ng-message">Debe ingresar el tipo de Beca</span>
			</span>

			<table style="margin-top:10px;">
				<tr ng-repeat="tipoDocumento in becasolicitud.TIPO.BECA_TIPO_DOCUMENTO">
					<td>
						{{tipoDocumento.NOMBRE}}<br/>
						<div>
							<input />
						</div>
					</td>
				</tr>
			</table>
    	</form>
    	<form id="formFile" enctype="multipart/form-data">
			<input type="file" name="fileName" id="fileName" accept="image/*"/>
			<input ng-click="uploadFile()" type="button" value="Guardar" id="upload"/>
		</form>

		<img src="../../WebServices/Becas.asmx/getImage" />

		<asp:Image id="myImage" runat="server" ImageUrl="../../WebServices/Becas.asmx/getImage" />

    </div>

</asp:Content>
