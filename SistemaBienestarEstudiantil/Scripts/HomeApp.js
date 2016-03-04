(function () {
    var app = angular.module('HomeApp', ['ui.grid', 'ngDialog']);

    app.controller('HomeController', function ($scope) {
        $scope.Message = "Mensaje controller funcionando";
        $scope.prueba = "";
    });

    app.controller('DatosController', function ($scope) {
        $scope.Message = "DAtos controller funcionando";
    });

    app.controller('TareasController', function ($scope) {
        $scope.Message = "Tareas Mensaje controller funcionando";
    });

    app.controller('UsuarioController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {

        this.ngDialogCustom = ngDialog;

        $scope.Message = "Tareas Mensaje controller funcionando";

        $scope.cargarUsuarios = function () {
            $http.post('../../WebServices/Users.asmx/getAllActivedUser', {
            }).success(function (data, status, headers, config) {
                console.log("cargarUsuarios",data);
                $scope.gridOptions.data = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...");
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            columnDefs: [
              {name:'Código', field: 'CODIGOUSUARIO'},
              {name:'Nombre', field: 'NOMBREUSUARIO'},
              {name:'Usuario', field: 'USUARIO1'},
              {name:'Cédula', field: 'CEDULAUSUARIO'},
              {name:'Acción', field: 'CODIGOUSUARIO', cellTemplate: 'actionsUsers.html' }
            ]
        };

        $scope.cargarUsuarios();

        this.removeUser = function (code) {
            var parentObject = this;
            
            $http.post('../../WebServices/Users.asmx/inactiveUserById', {
                id: code
            }).success(function (data, status, headers, config) {
                console.log("removeUser", data);
                parentObject.removeElementArray($scope.gridOptions.data, code);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...");
            });

        };

        this.editUser = function (code) {
            ngDialog.open({
                template: 'editUser.html',
                className: 'ngdialog-theme-flat ngdialog-theme-custom',
                closeByDocument: true,
                closeByEscape: true,
                scope: $scope,
                controller: $controller('ngDialogController', {
                    $scope: $scope,
                    $http: $http,
                    user: $scope.getElementArray($scope.gridOptions.data, code)
                })
            });
        };

        this.removeElementArray = function(arrayUser, userCode) {
            for (var i=0; i<arrayUser.length; i++) {
                if (arrayUser[i].CODIGOUSUARIO == userCode) {
                    arrayUser.splice(i, 1);
                }
            }
        };

        $scope.getElementArray = function(arrayUser, userCode) {
            for (var i=0; i<arrayUser.length; i++) {
                if (arrayUser[i].CODIGOUSUARIO == userCode) {
                    return arrayUser[i];
                }
            }
            return null;
        }

    }]);

    app.controller('ngDialogController', ['$scope', '$http', 'user', function($scope, $http, user) {
        console.log("ngDialogController", user);
        
        $scope.user = user;
        $scope.password = {
            reset: false
        };

        $scope.saveEditedUser = function () {
            
            $http.post('../../WebServices/Users.asmx/saveUserData', {
                
                userCode: user.CODIGOUSUARIO,
                userName: user.USUARIO1,
                userCompleteName: user.NOMBREUSUARIO,
                userIdentificationNumber: user.CEDULAUSUARIO,
                resetPassword: $scope.password.reset

            }).success(function (data, status, headers, config) {
                console.log("saveEditedUser", data);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...", data);
            });

            this.closeThisDialog();
        };
    }])
})();