(function () {
    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages']);

    app.controller('RolsController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

		$scope.rolCopy = {
            CODIGO: ''
        };
		
        $scope.chargeRols = function () {
            $http.post('../../WebServices/Rols.asmx/getAllRols', {
            }).success(function (data, status, headers, config) {
                console.log("chargeRols",data);
                $scope.gridOptions.data = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los roles...", data);
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: false,
            columnDefs: [
              {name:'Código', field: 'CODIGO'},
              {name:'Nombre', field: 'NOMBRE'},
              {name:'Estado', field: 'ESTADO', cellTemplate: "<div style='margin-top:2px;'>{{row.entity.ESTADO == true ? 'Activo' : 'Inactivo'}}</div>"},
              {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsRols.html' }
            ]
        };

        $scope.chargeRols();

        this.removeRol = function (code) {
            var parentObject = this;
            
            $http.post('../../WebServices/Rols.asmx/removeRolById', {
                id: code
            }).success(function (data, status, headers, config) {
                console.log("removeRol", data);
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Borrar', detail: 'Rol eliminado...'}]);
                parentObject.removeElementArray($scope.gridOptions.data, code);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los roles...");
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
            var rol = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            $scope.rolCopy.CODIGO = rol.CODIGO;
            $scope.rolCopy.NOMBRE = rol.NOMBRE;
            $scope.rolCopy.ESTADO = rol.ESTADO;

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
        
		$scope.updateElementArray = function(arrayRol, rolId, rolName, rolStatus) {
            for (var i=0; i<arrayRol.length; i++) {
                if (arrayRol[i].CODIGO == rolId) {
                    arrayRol[i].NOMBRE = rolName;
					arrayRol[i].ESTADO = rolStatus;
                }
            }
        };
		
		$scope.addNewRolDialog = function() {
            $scope.rolCopy = {
                ESTADO: true
            };

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
    }]);

    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {
        $scope.saveEditedRol = function () {
            $http.post('../../WebServices/Rols.asmx/saveRolData', {
                rolId: $scope.rolCopy.CODIGO,
                rolName: $scope.rolCopy.NOMBRE,
                rolStatus: $scope.rolCopy.ESTADO

            }).success(function (data, status, headers, config) {
                console.log("saveEditedRol: ", data);
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Nuevo', detail: 'Datos del rol guardados...'}]);
				$scope.updateElementArray($scope.gridOptions.data, $scope.rolCopy.CODIGO, $scope.rolCopy.NOMBRE, $scope.rolCopy.ESTADO);
            }).error(function (data, status, headers, config) {
                console.log("error al editar el rol...", data);
            });

            this.closeThisDialog();
        };

        $scope.addNewRolDB = function () {
            $http.post('../../WebServices/Rols.asmx/addNewRol', {                
                rolName: $scope.rolCopy.NOMBRE,
                rolStatus: $scope.rolCopy.ESTADO

            }).success(function (data, status, headers, config) {
                console.log("addNewRol: ", data);
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Nuevo', detail: 'Rol añadido...'}]);
                $scope.addElementArray($scope.gridOptions.data, data);
            }).error(function (data, status, headers, config) {
                console.log("error al añadir un nuevo rol...", data);
            });

            this.closeThisDialog();
        };
    }]);
})();