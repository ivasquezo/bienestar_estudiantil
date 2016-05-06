(function () {
    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages']);

    app.controller('RolsController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        // session listener
        document.onclick = function(){
            $http.get('/WebServices/Users.asmx/checkSession')
            .success(function (data, status, headers, config) {
                if (!data.success) {
                    document.location.href = "/";
                }
            }).error(function (data, status, headers, config) {
                console.log("Error checkSession", data);
            });
        };

        // for procesing message
        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = false;
        $scope.delay = 2;
        $scope.minDuration = 2;

		$scope.rolCopy = {
            CODIGO: ''
        };
		
        $scope.chargeRols = function () {
            $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Rols.asmx/getAllRols', {
            }).success(function (data, status, headers, config) {
                //console.log("Cargar roles... ", data);
                if (data.success)
                    $scope.gridOptions.data = data.response;
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar roles... ", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener roles'}]);
            });
        };
		
        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            enableColumnMenus: false,
            columnDefs: [
              {name:'C\u00F3digo', field: 'CODIGO', width: 85},
              {name:'Nombre', field: 'NOMBRE'},
              {name:'Acci\u00F3n', field: 'CODIGO', cellTemplate: 'actionsRols.html', width: 80, enableFiltering: false, enableSorting: false}
            ]
        };

        $scope.chargeRols();

        this.getRolStatus = function (code) {
            $scope.rolStatus = $scope.getElementArray($scope.gridOptions.data, code);

            if ($scope.rolStatus != null)
                return $scope.rolStatus.ACTIVO;
            else
                return false;
        };

        this.removeRol = function (code) {
            var parentObject = this;
            
            $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Rols.asmx/removeRolById', {
                rolId: code
            }).success(function (data, status, headers, config) {
                //console.log("Eliminar rol... ", data);
                if (data.success)
                    parentObject.removeElementArray($scope.gridOptions.data, code);

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al eliminar el rol... ", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al eliminar el rol'}]);
            });
        };

		this.removeElementArray = function(arrayRol, rolCode) {
            for (var i=0; i<arrayRol.length; i++) {
                if (arrayRol[i].CODIGO == rolCode) {
                    arrayRol.splice(i, 1);
                }
            }
        };
		
        this.editRol = function (code) {
            $scope.rolEdit = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            
            $scope.rolCopy.CODIGO = $scope.rolEdit.CODIGO;
            $scope.rolCopy.NOMBRE = $scope.rolEdit.NOMBRE;
            $scope.rolCopy.ACTIVO = $scope.rolEdit.ACTIVO;

			$scope.getAccessByRol(code);
            $scope.accessRols = [];
            $scope.rolsAccess = [];

            ngDialog.open({
                template: 'editRol.html',
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

		$scope.getElementArray = function(arrayRol, rolCode) {
            for (var i=0; i<arrayRol.length; i++) {
                if (arrayRol[i].CODIGO == rolCode) {
                    return arrayRol[i];
                }
            }
            return null;
        }
        
        $scope.getAccessByRol = function (code) {
            if (code > 0) {
                $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Rols.asmx/getAccessByRolId', {
                    rolId: code
                }).success(function (data, status, headers, config) {
                    //console.log("Accesos del rol... ", data);
                    $scope.rolsAccess = data.response;
                }).error(function (data, status, headers, config) {
                    console.log("Error al cargar accesos del rol...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los accesos del rol'}]);
                });
            }

            $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Rols.asmx/getAllAccess'
            ).success(function (data, status, headers, config) {
                //console.log("Accesos existentes... ", data);
                $scope.allAccess = data.response;
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar accesos...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los accesos existentes'}]);
            });
        };

		$scope.updateElementArray = function(arrayRol, rolId, rolName) {
            for (var i=0; i<arrayRol.length; i++) {
                if (arrayRol[i].CODIGO == rolId) {
                    arrayRol[i].NOMBRE = rolName;
                }
            }
        };
		
		$scope.addNewRolDialog = function() {
            $scope.rolCopy = {
                NOMBRE: ''
            };
            $scope.getAccessByRol(0);
            $scope.accessRols = [];

            ngDialog.open({
                template: 'newRol.html',
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
		
        $scope.addElementArray = function(arrayRol, newRol) {
            arrayRol.push(newRol);
        }; 
		
		$scope.existAccess = function (code) {
			if ($scope.rolsAccess.length > 0) {
				for (var i = 0; i < $scope.rolsAccess.length; i++) {
                    if($scope.rolsAccess[i].CODIGO == code && $scope.rolsAccess[i].VALIDO == true) return true;
                };
			}
            return false;
		};

        $scope.setAccessRol = function(codeAccess) {            
            $scope.accessRols.push(codeAccess);            
        };      
    }]);

    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {
        $scope.saveEditedRol = function () {
            var parentObject = this;

            if (!this.rolForm.$invalid) {
                $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Rols.asmx/saveRolData', {
                    rolId: $scope.rolCopy.CODIGO,
                    rolName: $scope.rolCopy.NOMBRE.toUpperCase(),
                    accessRols: $scope.accessRols
                }).success(function (data, status, headers, config) {
                    //console.log("Editar rol: ", data);
                    if (data.success) {
                        $scope.updateElementArray($scope.gridOptions.data, $scope.rolCopy.CODIGO, $scope.rolCopy.NOMBRE);
                        parentObject.closeThisDialog();
                    } 

                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al editar el rol...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al actualizar el rol'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Editar', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };

        $scope.addNewRolDB = function () {
            var newParentObject = this;

            if (!this.newRolForm.$invalid) {
                $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Rols.asmx/addNewRol', {                
                    rolName: $scope.rolCopy.NOMBRE.toUpperCase(),
                    accessRols: $scope.accessRols
                }).success(function (data, status, headers, config) {
                    //console.log("Agregar rol: ", data);
                    if (data.success) {
                        $scope.addElementArray($scope.gridOptions.data, data.response);
                        newParentObject.closeThisDialog();
                    }

                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al agregar el rol...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar el rol'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Editar', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };
    }]);

    app.directive('validRolName', ['$http', function($http) {
        return {
            require: 'ngModel',

            link: function(scope, element, attr, ctrl) {
                function customValidator(ngModelValue) {
                    if (ngModelValue != null && ngModelValue != scope.rolCopy) {
                        ctrl.$setValidity('rolNameChecking', false);

                        scope.promise = $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Rols.asmx/countRolWithName', {
                            rolName: ngModelValue
                        }).success(function (data, status, headers, config) {
                            if (data.cantidad == 0) {
                                ctrl.$setValidity('rolNameExist', true);
                                ctrl.$setValidity('rolNameValidator', true);
                                ctrl.$setValidity('rolNameChecking', true);
                               
                            } else {                            
                                ctrl.$setValidity('rolNameExist', false);
                                ctrl.$setValidity('rolNameValidator', true);
                            }
                        }).error(function (data, status, headers, config) {
                            console.log("Error al traer rol", data);
                            ctrl.$setValidity('rolNameValidator', false);
                        });
                    } else {
                        ctrl.$setValidity('rolNameExist', true);
                        ctrl.$setValidity('rolNameValidator', true);
                        ctrl.$setValidity('rolNameChecking', true);
                        
                    }
                    return ngModelValue;
                }

                ctrl.$parsers.push(customValidator);
            }
        };
    }]);
})();