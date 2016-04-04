﻿(function () {

    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages','cgBusy']);

    app.controller('BecasController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        
        // for procesing message
        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = true;
        $scope.delay = 2;
        $scope.minDuration = 2;

        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});
        
        $scope.cargarBecas = function () {
            $scope.promise = $http.get('../../WebServices/Becas.asmx/getBecas')
            .success(function (data, status, headers, config) {
                console.log("Becas cargadas: ", data);

                for (var i = 0; i < data.response.length; i++) {
                    if (data.response[i].APROBADA == 0)
                        data.response[i].ESTADO = "Pendiente";
                    else if (data.response[i].APROBADA == 1)
                        data.response[i].ESTADO = "Procesando";
                    else if (data.response[i].APROBADA == 2)
                        data.response[i].ESTADO = "Aprobada";
                    else if (data.response[i].APROBADA == 3)
                        data.response[i].ESTADO = "Rechazada";
                };

                if (data.success) {
                    $scope.gridOptions.data = data.response;
                }
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error cargar becas...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las becas'}]);
            });
        };
        
        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            enableColumnMenus: false,
            columnDefs: [
              {name:'Código', field: 'CODIGO', width: 65},
              {name:'Beca', field: 'BECA'},
              {name:'Otorgado', field: 'OTORGADO', width: 80},
              {name:'Rubro', field: 'RUBRO', width: 150},
              {name:'Estado', field: 'ESTADO', width: 100},
              {name:'C\u00E9dula', field: 'CEDULA', width: 100},
              {name:'Nombre', field: 'NOMBRE', width: 320}
            ]
        };

        $scope.cargarBecas();

    }]);
})();