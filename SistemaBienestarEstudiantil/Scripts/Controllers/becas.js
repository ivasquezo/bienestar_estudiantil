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
            { n: 'Rechazada', v: 2 }
        ];
        
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
        
        $scope.cargarTipos = function () {
            $scope.promise = $http.get('../../WebServices/Becas.asmx/getTipos')
            .success(function (data, status, headers, config) {
                
                console.log("Tipos cargadas: ", data);
                $scope.gridOptionsTipos.data = data;

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
              {name:'Nombre', field: 'NOMBRE', width: 320},
              {name:'Acci\u00F3n', field: 'CODIGO', width: 80, cellTemplate: 'actionsBecas.html', enableFiltering: false, enableSorting: false}
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
                    console.log("Error eliminar beca...", data);
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

        this.editBeca = function (code) {
            
            $scope.solicitudbeca = angular.copy($scope.getElementArray($scope.gridOptions.data, code));

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

            ngDialog.open({
                template: 'editTipoBeca.html',
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
        }

    }]);


    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {
        
        $scope.saveEditedUser = function () {
            var parentObject = this;

            $scope.userCopy_copy = angular.copy($scope.userCopy);
            $scope.userCopy_copy.NOMBREUSUARIO = $scope.userCopy.NOMBREUSUARIO.toLowerCase();
            $scope.userCopy_copy.NOMBRECOMPLETO = $scope.userCopy.NOMBRECOMPLETO.toUpperCase();

            delete $scope.userCopy_copy['BE_ACTIVIDAD'];
            delete $scope.userCopy_copy['BE_BECA_SOLICITUD_HISTORIAL'];

            if (!this.userForm.$invalid) {
                $scope.promise = $http.post('../../WebServices/Users.asmx/saveUserData', {
                    user: $scope.userCopy_copy,
                    resetPassword: $scope.password.reset
                }).success(function (data, status, headers, config) {
                    console.log("Editar usuario: ", data);
                    if (data.success) {
                        var index = $scope.arrayObjectIndexOf($scope.gridOptions.data, data.response.CODIGO, "CODIGO");
                        $scope.gridOptions.data[index] = data.response;
                        $scope.gridOptions.data[index].NOMBREESTADO = $scope.cargarNombreEstado(data.response.ESTADO);
                        parentObject.closeThisDialog();
                    } 

                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error editar usuario...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al actualizar el usuario'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Editar', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };

        $scope.addNewUserDB = function () {
            var parentObject = this;

            if (!this.newUserForm.$invalid) {
                $scope.userCopy.NOMBREUSUARIO = $scope.userCopy.NOMBREUSUARIO.toLowerCase();
                $scope.userCopy.NOMBRECOMPLETO = $scope.userCopy.NOMBRECOMPLETO.toUpperCase();

                $scope.promise = $http.post('../../WebServices/Users.asmx/addNewUser', {
                    newUser: $scope.userCopy
                }).success(function (data, status, headers, config) {
                    console.log("Agregar usuario: ", data);
                    if (data.success) {
                        $scope.addElementArray($scope.gridOptions.data, data.response);
                        parentObject.closeThisDialog();
                    }

                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error agregar usuario...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar el usuario'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };
    }]);

})();