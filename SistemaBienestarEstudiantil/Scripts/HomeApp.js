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
            $http.post('../../WebServices/Users.asmx/getAllUser', {
            }).success(function (data, status, headers, config) {
                console.log("cargarUsuarios", data);
                $scope.gridOptions.data = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...", data);
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: false,
            columnDefs: [
              { name: 'Código', field: 'CODIGO' },
              { name: 'Nombre', field: 'NOMBRECOMPLETO' },
              { name: 'Usuario', field: 'NOMBREUSUARIO' },
              { name: 'Cédula', field: 'CEDULA' },
              { name: 'Activo', field: 'ESTADO' },
              { name: 'Estado', field: 'ESTADO', cellTemplate: "<div>{{row.entity.ESTADO == true ? 'Activo' : 'Inactivo'}}</div>" },
              { name: 'Acción', field: 'CODIGO', cellTemplate: 'actionsUsers.html' }
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

            $scope.user = $scope.getElementArray($scope.gridOptions.data, code);

            ngDialog.open({
                template: 'editUser.html',
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

        this.removeElementArray = function (arrayUser, userCode) {
            for (var i = 0; i < arrayUser.length; i++) {
                if (arrayUser[i].CODIGO == userCode) {
                    arrayUser.splice(i, 1);
                }
            }
        };

        $scope.addElementArray = function (arrayUser, newUser) {
            arrayUser.push(newUser);
        };

        $scope.getElementArray = function (arrayUser, userCode) {
            for (var i = 0; i < arrayUser.length; i++) {
                if (arrayUser[i].CODIGO == userCode) {
                    return arrayUser[i];
                }
            }
            return null;
        }

        $scope.addNewUserDialog = function () {

            console.log("addNewUser");

            $scope.user = {
                ESTADO: true
            };

            ngDialog.open({
                template: 'newUser.html',
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

    } ]);

    app.controller('ngDialogController', ['$scope', '$http', function ($scope, $http) {

        $scope.password = {
            reset: false
        };

        $scope.saveEditedUser = function () {

            console.log("saveEditedUser");

            $http.post('../../WebServices/Users.asmx/saveUserData', {

                userCode: $scope.user.CODIGO,
                userName: $scope.user.NOMBREUSUARIO,
                userCompleteName: $scope.user.NOMBRECOMPLETO,
                userIdentificationNumber: $scope.user.CEDULA,
                userState: $scope.user.ESTADO,
                resetPassword: $scope.password.reset

            }).success(function (data, status, headers, config) {
                console.log("saveEditedUser: ", data);
            }).error(function (data, status, headers, config) {
                console.log("error al editar el usuario...", data);
            });

            this.closeThisDialog();
        };

        $scope.addNewUser1 = function () {

            $http.post('../../WebServices/Users.asmx/addNewUser', {

                userName: $scope.user.NOMBREUSUARIO,
                userCompleteName: $scope.user.NOMBRECOMPLETO,
                userIdentificationNumber: $scope.user.CEDULA,
                userState: $scope.user.ESTADO

            }).success(function (data, status, headers, config) {
                console.log("addNewUser: ", data);
                $scope.addElementArray($scope.gridOptions.data, $scope.user);
            }).error(function (data, status, headers, config) {
                console.log("error al añadir un nuevo usuario...", data);
            });

            this.closeThisDialog();
        };
    } ])
})();