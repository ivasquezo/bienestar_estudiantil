(function () {
    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages']);

    app.controller('RolsController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

		$scope.rolCopy = {
            CODIGO: ''
        };
		
        // Cargar todos los roles
        $scope.chargeRols = function () {
            // Llama al servicio que obtiene todos los roles
            $http.post('../../WebServices/Rols.asmx/getAllRols', {
            }).success(function (data, status, headers, config) {
                console.log("Cargar roles... ", data);
                // Si los datos se obtuvieron sin problemas
                if (data.success)
                    $scope.gridOptions.data = data.response;
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar los roles... ", data);
                // Si hubo error al obtener los roles
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener roles'}]);
            });
        };
		
        // Diseno de los datos de la tabla
        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: false,
            columnDefs: [
              {name:'Código', field: 'CODIGO'},
              {name:'Nombre', field: 'NOMBRE'},
              {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsRols.html', width: 80}
            ]
        };

        // Llama al metodo cargar roles
        $scope.chargeRols();

        // Eliminar un rol
        this.removeRol = function (code) {
            var parentObject = this;
            
            // Llama al servicio que elimina un rol por su codigo
            $http.post('../../WebServices/Rols.asmx/removeRolById', {
                rolId: code
            }).success(function (data, status, headers, config) {
                console.log("Eliminar rol... ", data);
                // Si se elimina correctamente el rol
                if (data.success)
                    parentObject.removeElementArray($scope.gridOptions.data, code);

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al eliminar el rol... ", data);
                // Si hubo error al obtener los roles
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al eliminar el rol'}]);
            });
        };

        // Elimina el rol de la vista
		this.removeElementArray = function(arrayRol, rolCode) {
            for (var i=0; i<arrayRol.length; i++) {
                if (arrayRol[i].CODIGO == rolCode) {
                    arrayRol.splice(i, 1);
                }
            }
        };
		
        // Editar un rol
        this.editRol = function (code) {
            // Obtiene el rol que se va a editar
            $scope.rolEdit = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            
            $scope.rolCopy.CODIGO = $scope.rolEdit.CODIGO;
            $scope.rolCopy.NOMBRE = $scope.rolEdit.NOMBRE;
			$scope.getAccessByRol(code);
            $scope.accessRols = [];

            // Abre el pop up para editar el rol
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

        // Obtiene los datos de un rol
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
                $http.post('../../WebServices/Rols.asmx/getAccessByRolId', {
                    rolId: code
                }).success(function (data, status, headers, config) {
                    console.log("Accesos del rol... ", data);
                    $scope.rolsAccess = data.response;
                }).error(function (data, status, headers, config) {
                    console.log("Error al cargar accesos del rol...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los accesos del rol'}]);
                });
            }

            $http.post('../../WebServices/Rols.asmx/getAllAccess'
            ).success(function (data, status, headers, config) {
                console.log("Accesos existentes... ", data);
                $scope.allAccess = data.response;
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar accesos...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los accesos existentes'}]);
            });
        };

        // Actualiza el rol en la parte visual
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

    // Pop up para actualizar y agregar rol
    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {
        // Al editar un rol
        $scope.saveEditedRol = function () {
            var parentObject = this;

            $http.post('../../WebServices/Rols.asmx/saveRolData', {
                rolId: $scope.rolCopy.CODIGO,
                rolName: $scope.rolCopy.NOMBRE,
                accessRols: $scope.accessRols
            }).success(function (data, status, headers, config) {
                console.log("Editar rol: ", data);
                // Si los datos se editaron correctamente
                if (data.success) {
                    $scope.updateElementArray($scope.gridOptions.data, $scope.rolCopy.CODIGO, $scope.rolCopy.NOMBRE);
                    // Se cierra el pop up
                    parentObject.closeThisDialog();
                } 

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al editar el rol...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al actualizar el rol'}]);
            });
        };

        // Al agregar un rol
        $scope.addNewRolDB = function () {
            var newParentObject = this;

            $http.post('../../WebServices/Rols.asmx/addNewRol', {                
                rolName: $scope.rolCopy.NOMBRE,
                accessRols: $scope.accessRols
            }).success(function (data, status, headers, config) {
                console.log("Agregar rol: ", data);
                if (data.success) {
                    $scope.addElementArray($scope.gridOptions.data, data.response);
                    // Se cierra el pop up
                    newParentObject.closeThisDialog();
                }

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al agregar el rol...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar el rol'}]);
            });
        };
    }]);
})();