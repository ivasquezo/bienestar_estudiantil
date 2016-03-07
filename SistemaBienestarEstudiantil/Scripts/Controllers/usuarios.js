﻿(function () {
    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages']);

    app.controller('UsuariosController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {

        $scope.userCopy = {
            CEDULA: ''
        };

        $scope.$watch('userCopy.CEDULA', function() {
            $scope.userCopy.CEDULA = $scope.onlyNumber($scope.userCopy.CEDULA, 10);
        });

        $scope.onlyNumber = function(number, countMax){
            var n = '';
            console.log("number: " + number);
            if (number === undefined || number === '') return '';
            for (var i=0; i<number.length; i++) {
                if (number.charCodeAt(i) > 47 && number.charCodeAt(i) < 58) {
                    if (countMax != -1 && i == countMax) return n;
                        n += number[i];
                }
            }
            return n;
        };

        $scope.cargarUsuarios = function () {
            $http.post('../../WebServices/Users.asmx/getAllUser', {
            }).success(function (data, status, headers, config) {
                console.log("cargarUsuarios",data);
                $scope.gridOptions.data = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...", data);
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: false,
            columnDefs: [
              {name:'Código', field: 'CODIGO'},
              {name:'Nombre', field: 'NOMBRECOMPLETO'},
              {name:'Usuario', field: 'NOMBREUSUARIO'},
              {name:'Cédula', field: 'CEDULA'},
              {name:'Correo', field: 'CORREO'},
              {name:'Activo', field: 'ESTADO'},
              {name:'Estado', field: 'ESTADO', cellTemplate: "<div>{{row.entity.ESTADO == true ? 'Activo' : 'Inactivo'}}</div>"},
              {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsUsers.html' }
            ]
        };

        $scope.cargarUsuarios();

        this.removeUser = function (code) {
            var parentObject = this;
            
            $http.post('../../WebServices/Users.asmx/removeUserById', {
                id: code
            }).success(function (data, status, headers, config) {
                console.log("removeUser", data);
                alert("Usuario borrado correctamente!");
                parentObject.removeElementArray($scope.gridOptions.data, code);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...");
            });

        };

        this.editUser = function (code) {

            var user = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            $scope.userCopy.CODIGO = user.CODIGO;
            $scope.userCopy.CEDULA = user.CEDULA;
            $scope.userCopy.CORREO = user.CORREO;
            $scope.userCopy.NOMBREUSUARIO = user.NOMBREUSUARIO;
            $scope.userCopy.NOMBRECOMPLETO = user.NOMBRECOMPLETO;
            $scope.userCopy.ESTADO = user.ESTADO;

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

        this.removeElementArray = function(arrayUser, userCode) {
            for (var i=0; i<arrayUser.length; i++) {
                if (arrayUser[i].CODIGO == userCode) {
                    arrayUser.splice(i, 1);
                }
            }
        };

        $scope.addElementArray = function(arrayUser, newUser) {
            arrayUser.push(newUser);
        };

        $scope.getElementArray = function(arrayUser, userCode) {
            for (var i=0; i<arrayUser.length; i++) {
                if (arrayUser[i].CODIGO == userCode) {
                    return arrayUser[i];
                }
            }
            return null;
        }

        $scope.addNewUserDialog = function() {

            $scope.userCopy = {
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

    }]);

    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {

        $scope.password = {
            reset: false
        };

        $scope.saveEditedUser = function () {

            console.log("saveEditedUser");
            
            $http.post('../../WebServices/Users.asmx/saveUserData', {
                
                userCode: $scope.userCopy.CODIGO,
                userName: $scope.userCopy.NOMBREUSUARIO,
                userCompleteName: $scope.userCopy.NOMBRECOMPLETO,
                userIdentificationNumber: $scope.userCopy.CEDULA,
                userMail: $scope.userCopy.CORREO,
                userState: $scope.userCopy.ESTADO,
                resetPassword: $scope.password.reset

            }).success(function (data, status, headers, config) {
                console.log("saveEditedUser: ", data);
                alert("Usuario guardado correctamente!");
            }).error(function (data, status, headers, config) {
                console.log("error al editar el usuario...", data);
            });

            this.closeThisDialog();
        };

        $scope.addNewUser1 = function () {

            $http.post('../../WebServices/Users.asmx/addNewUser', {
                
                userName: $scope.userCopy.NOMBREUSUARIO,
                userCompleteName: $scope.userCopy.NOMBRECOMPLETO,
                userIdentificationNumber: $scope.userCopy.CEDULA,
                userMail: $scope.userCopy.CORREO,
                userState: $scope.userCopy.ESTADO

            }).success(function (data, status, headers, config) {
                console.log("addNewUser: ", data);
                $scope.addElementArray($scope.gridOptions.data, $scope.userCopy);
            }).error(function (data, status, headers, config) {
                console.log("error al añadir un nuevo usuario...", data);
            });

            this.closeThisDialog();
        };
    }]);

})();