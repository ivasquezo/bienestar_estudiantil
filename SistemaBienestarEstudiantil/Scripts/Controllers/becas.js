(function () {

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

        $scope.RUBROS = [
            { n: 'Pensión', v: 0 },
            { n: 'Matrícula', v: 1 },
            { n: 'Pensión y matrícula', v: 2 }
        ];
        
        $scope.ESTADOS = [
            { n: 'Pendiente', v: 0 },
            { n: 'Procesando', v: 1 },
            { n: 'Aprobada', v: 2 },
            { n: 'Rechazada', v: 3 }
        ];

        $scope.convertDate = function(invalidDate) {
            return invalidDate.substring(6, invalidDate.length-2);
        };
        
        $scope.cargarBecas = function () {
            $scope.promise = $http.get('../../WebServices/Becas.asmx/getBecas')
            .success(function (data, status, headers, config) {

                console.log("Becas cargadas: ", data);
                if (data.success) {

                    for (var i = 0; i < data.response.length; i++) {
                        
                        data.response[i].ESTADO = $scope.ESTADOS[data.response[i].APROBADA].n;

                        if (data.response[i].BE_BECA_SOLICITUD_HISTORIAL.length > 0) {
                            var size = data.response[i].BE_BECA_SOLICITUD_HISTORIAL.length;
                            var lastHistorial = data.response[i].BE_BECA_SOLICITUD_HISTORIAL[size - 1];
                            data.response[i].RUBRO = lastHistorial.RUBRO;
                            data.response[i].RUBRONOMBRE = $scope.RUBROS[lastHistorial.RUBRO].n;
                            data.response[i].OTORGADO = lastHistorial.OTORGADO;
                        }
                    };

                    $scope.gridOptions.data = data.response;
                }
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);

            }).error(function (data, status, headers, config) {
                console.log("Error cargar becas...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las becas'}]);
            });
        };
        
        $scope.cargarTipos = function () {
            $scope.promise = $http.get('../../WebServices/Becas.asmx/getTipos')
            .success(function (data, status, headers, config) {
                
                console.log("Tipos de becas cargadas: ", data);
                $scope.gridOptionsTipos.data = data;

            }).error(function (data, status, headers, config) {
                console.log("Error cargar becas...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los tipos de becas'}]);
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
              {name:'Rubro', field: 'RUBRONOMBRE', width: 150},
              {name:'Estado', field: 'ESTADO', width: 100},
              {name:'C\u00E9dula', field: 'CEDULA', width: 100},
              {name:'Nombre', field: 'NOMBRE', width: 320},
              {name:'Acci\u00F3n', field: 'CODIGO', width: 120, cellTemplate: 'actionsBecas.html', enableFiltering: false, enableSorting: false}
            ]
        };

        $scope.gridOptionsTipos = {
            enableSorting: true,
            enableFiltering: true,
            enableColumnMenus: false,
            columnDefs: [
              {name:'Código', field: 'CODIGO', width: 65},
              {name:'Tipo de beca', field: 'NOMBRE'},
              {name:'Acci\u00F3n', field: 'CODIGO', width: 80, cellTemplate: 'actionsTiposBecas.html', enableFiltering: false, enableSorting: false}
            ]
        };

        $scope.cargarBecas();
        $scope.cargarTipos();

        this.removeBeca = function(code){
            var parentObject = this;
            if (confirm("Desea eliminar esta beca?")) {
                $scope.promise = $http.post('../../WebServices/Becas.asmx/removeBeca', {
                    codeBeca: code
                }).success(function (data, status, headers, config) {
                    parentObject.removeElementArray($scope.gridOptions.data, code);
                    $('#messages').puigrowl('show', [{severity: 'info', summary: 'Información', detail: 'Beca eliminada'}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error eliminar beca...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al eliminar la beca'}]);
                });
            }
        };

        this.removeTipoBeca = function(code){
            var parentObject = this;
            if (confirm("Desea eliminar este tipo de beca?")) {

                $scope.promise = $http.post('../../WebServices/Becas.asmx/removeTipoBeca', {
                    codeTipoBeca: code
                }).success(function (data, status, headers, config) {
                    parentObject.removeElementArray($scope.gridOptionsTipos.data, code);
                    $('#messages').puigrowl('show', [{severity: 'info', summary: 'Información', detail: 'Tipo de Beca eliminado'}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error eliminar tipo beca...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al eliminar el tipo de beca'}]);
                });

            }
        };

        this.removeElementArray = function(arrayElement, code) {
            for (var i=0; i<arrayElement.length; i++) {
                if (arrayElement[i].CODIGO == code) {
                    arrayElement.splice(i, 1);
                }
            }
        };

        $scope.getElementArray = function(arrayElement, code) {
            for (var i=0; i<arrayElement.length; i++) {
                if (arrayElement[i].CODIGO == code) {
                    return arrayElement[i];
                }
            }
            return null;
        }

        $scope.cargarAdjuntosSolicitudBecaEdit = function (code) {
            $scope.promise = $http.post('../../WebServices/Becas.asmx/getAttachBeca', {
                codeBeca: code
            }).success(function (data, status, headers, config) {
                console.log(data);
                $scope.solicitudbeca.ADJUNTOS = data;
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar adjuntos de la solicitud de beca:", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al cargar adjuntos de la solicitud de beca'}]);
            });
        }

        $scope.getTipoBecaByCodeSelected = function (code) {
            for (var i = 0; i < $scope.gridOptionsTipos.data.length; i++) {
                if (code == $scope.gridOptionsTipos.data[i].CODIGO) return $scope.gridOptionsTipos.data[i];
            };
            return null;
        }

        this.editBeca = function (code) {
            
            $scope.solicitudbeca = angular.copy($scope.getElementArray($scope.gridOptions.data, code));

            $scope.cargarAdjuntosSolicitudBecaEdit(code);
            
            ngDialog.open({
                template: 'editBecas.html',
                className: 'ngdialog-theme-flat ngdialog-theme-custom',
                closeByDocument: true,
                closeByEscape: true,
                scope: $scope,
                controller: $controller('ngDialogController', {
                    $scope: $scope,
                    $http: $http
                })
            });
        };

        this.editTipoBeca = function (code) {
            
            $scope.tipoBeca = angular.copy($scope.getElementArray($scope.gridOptionsTipos.data, code));
            
            // update ID for remove after
            for (var i = 0; i < $scope.tipoBeca.BE_BECA_TIPO_DOCUMENTO.length; i++) {
                $scope.tipoBeca.BE_BECA_TIPO_DOCUMENTO[i].ID = $scope.tipoBeca.BE_BECA_TIPO_DOCUMENTO[i].CODIGO;
            };

            ngDialog.open({
                template: 'addTipoBeca.html',
                className: 'ngdialog-theme-flat ngdialog-theme-custom',
                closeByDocument: true,
                closeByEscape: true,
                scope: $scope,
                controller: $controller('ngDialogController', {
                    $scope: $scope,
                    $http: $http
                })
            });
        };

        this.viewHistoryChanges = function (code) {
            
            $scope.solicitudbeca = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            
            ngDialog.open({
                template: 'viewHistoryChanges.html',
                className: 'ngdialog-theme-flat ngdialog-theme-custom',
                closeByDocument: true,
                closeByEscape: true,
                scope: $scope,
                controller: $controller('ngDialogController', {
                    $scope: $scope,
                    $http: $http
                })
            });
        };

        $scope.addTipoBecaDialog = function () {
            
            $scope.tipoBeca = {
                NOMBRE: null,
                BE_BECA_TIPO_DOCUMENTO: []
            };
            
            ngDialog.open({
                template: 'addTipoBeca.html',
                className: 'ngdialog-theme-flat ngdialog-theme-custom',
                closeByDocument: true,
                closeByEscape: true,
                scope: $scope,
                controller: $controller('ngDialogController', {
                    $scope: $scope,
                    $http: $http
                })
            });            
        };

        $scope.addDocumento = function () {
            $scope.tipoBeca.BE_BECA_TIPO_DOCUMENTO.push({
                ID: $scope.generateId(),
                NOMBRE:null
            });
        }

        $scope.removeDocumento = function (id) {
            for (var i = 0; i < $scope.tipoBeca.BE_BECA_TIPO_DOCUMENTO.length; i++) {
                if ($scope.tipoBeca.BE_BECA_TIPO_DOCUMENTO[i].ID == id)
                    $scope.tipoBeca.BE_BECA_TIPO_DOCUMENTO.splice(i, 1);
            };
        }

        // function for generate aleatory number for id in encuestas object, for manipulate
        $scope.generateId = function(){
            return Math.floor(Math.random() * 999999) + 100000;
        };

        $scope.saveTipoBeca = function () {
            if (!this.addtipoBecaForm.$invalid) {

                $scope.promise = $http.post('../../WebServices/Becas.asmx/saveBecaTipo', {
                    becaTipo: $scope.tipoBeca
                }).success(function (data, status, headers, config) {
                    console.log("saveBecaTipo: ", data);
                    $('#messages').puigrowl('show', [{severity: "info", summary: "Guardar", detail: "Tipo beca guardado correctamente"}]);
                    $scope.cargarTipos();
                }).error(function (data, status, headers, config) {
                    console.log("Error al guardar el tipo de beca", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al actualizar el tipo de beca'}]);
                });

            } else {
                $('#messages').puigrowl('show', [{severity: "error", summary: "Guardar", detail: "Debe completar los datos que faltan"}]);
            }
        }

        $scope.saveSolicitudBeca = function(){

            if (!this.becaForm.$invalid) {

                $scope.solicitudbeca['CODIGOUSUARIO'] = $scope.CODIGOUSUARIO;

                $scope.promise = $http.post('../../WebServices/Becas.asmx/saveBeca', {
                    editedBeca: $scope.removeUnnecesaryAttributes($scope.solicitudbeca)
                }).success(function (data, status, headers, config) {
                    console.log("solicitudbeca edited: ", data);
                    $scope.cargarBecas();
                    $('#messages').puigrowl('show', [{severity: "info", summary: "Guardar", detail: "Beca guardada correctamente"}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al guardar la beca", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al actualizar la beca'}]);
                });


            } else {
                $('#messages').puigrowl('show', [{severity: "error", summary: "Guardar", detail: "Debe completar los datos que faltan"}]);
            }
        }

        $scope.removeUnnecesaryAttributes = function (solicitudbeca) {
            var temporalSolicitudBeca = angular.copy(solicitudbeca);

            // remove unnecesary attributes
            delete temporalSolicitudBeca['ADJUNTOS'];
            delete temporalSolicitudBeca['BECA'];
            delete temporalSolicitudBeca['BE_BECA_SOLICITUD_HISTORIAL'];
            delete temporalSolicitudBeca['CEDULA'];
            delete temporalSolicitudBeca['ESTADO'];
            delete temporalSolicitudBeca['NOMBRE'];
            delete temporalSolicitudBeca['TIPOCODIGO'];
            
            return temporalSolicitudBeca;            
        };

    }]);

    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {
        
    }]);

})();