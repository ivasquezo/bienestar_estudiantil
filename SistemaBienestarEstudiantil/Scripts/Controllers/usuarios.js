(function () {

    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages','cgBusy']);

    app.controller('UsuariosController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {

        // for procesing message
        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = false;
        $scope.delay = 2;
        $scope.minDuration = 2;

        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        $scope.userCopy = {
            CEDULA: ''
        };

        $scope.$watch('userCopy.CEDULA', function() {
            $scope.userCopy.CEDULA = $scope.onlyNumber($scope.userCopy.CEDULA, 10);
        });

        $scope.onlyNumber = function(number, countMax){
            var n = '';
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
            $scope.promise = $http.post('../../WebServices/Users.asmx/getAllUser', {
            }).success(function (data, status, headers, config) {
                $scope.gridOptions.data = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...", data);
            });
        };

        $scope.cargarRoles = function () {
            $scope.promise = $http.post('../../WebServices/Rols.asmx/getAllRols', {
            }).success(function (data, status, headers, config) {
                $scope.Rols = data.response;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los roles...", data);
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            columnDefs: [
              {name:'Código', field: 'CODIGO'},
              {name:'Nombre', field: 'NOMBRECOMPLETO'},
              {name:'Usuario', field: 'NOMBREUSUARIO'},
              {name:'Cédula', field: 'CEDULA'},
              {name:'Correo', field: 'CORREO'},
              {name:'Estado', field: 'ESTADO', cellTemplate: "<div style='margin-top:2px;'>{{row.entity.ESTADO == true ? 'Activo' : 'Inactivo'}}</div>"},
              {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsUsers.html', width: 80, enableFiltering: false}
            ]
        };

        $scope.cargarUsuarios();
        $scope.cargarRoles();

        this.removeUser = function (code) {
            var parentObject = this;
            
            $scope.promise = $http.post('../../WebServices/Users.asmx/removeUserById', {
                id: code
            }).success(function (data, status, headers, config) {
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Borrar', detail: 'Usuario eliminado...'}]);
                parentObject.removeElementArray($scope.gridOptions.data, code);
            }).error(function (data, status, headers, config) {
                console.log("error al eliminar usuarios...");
            });

        };

        this.editUser = function (code) {

            $scope.userCopy = angular.copy($scope.getElementArray($scope.gridOptions.data, code));

            $scope.userSavedCedula = $scope.userCopy.CEDULA;

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

            $scope.userSavedCedula = null;

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

            $scope.promise = $http.post('../../WebServices/Users.asmx/saveUserData', {
                
                user: $scope.userCopy,
                resetPassword: $scope.password.reset

            }).success(function (data, status, headers, config) {
                console.log("saveEditedUser: ", data);
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Editar', detail: 'Datos del usuario guardados correctamente.'}]);
            }).error(function (data, status, headers, config) {
                console.log("error al editar el usuario...", data);
            });

            this.closeThisDialog();
        };

        $scope.addNewUserDB = function () {

            if (!this.newUserForm.$invalid) {

                $scope.promise = $http.post('../../WebServices/Users.asmx/addNewUser', {
                    
                    newUser: $scope.userCopy

                }).success(function (data, status, headers, config) {
                    console.log("addNewUser: ", data);
                    $('#messages').puigrowl('show', [{severity: 'info', summary: 'Nuevo', detail: 'Usuario añadido correctamente.'}]);
                    $scope.addElementArray($scope.gridOptions.data, data);
                }).error(function (data, status, headers, config) {
                    console.log("error al añadir un nuevo usuario...", data);
                });

                this.closeThisDialog();
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };
    }]);

    app.directive('validIdentification', ['$http', function($http) {
        return {

            // limit usage to argument only
            restrict: 'A',

            // require NgModelController, i.e. require a controller of ngModel directive
            require: 'ngModel',

            // create linking function and pass in our NgModelController as a 4th argument
            link: function(scope, element, attr, ctrl) {
                
                // please note you can name your function & argument anything you like
                function customValidator(ngModelValue) {
                    
                    // check if contains uppercase
                    // if it does contain uppercase, set our custom `uppercaseValidator` to valid/true
                    // otherwise set it to non-valid/false
                    
                    if (ngModelValue != null && ngModelValue.toString().length == 10) {
                        
                        if (ngModelValue != scope.userSavedCedula) {

                            ctrl.$setValidity('cedulaChecking', false);

                            scope.promise = $http.post('../../WebServices/Users.asmx/countUserWithCedula', {
                                cedula: ngModelValue
                            }).success(function (data, status, headers, config) {

                                if (data.cantidad == 0) {
                                    ctrl.$setValidity('cedulaValidator', true);
                                    ctrl.$setValidity('cedulaChecking', true);
                                    ctrl.$setValidity('cedulaExist', true);
                                } else {                            
                                    ctrl.$setValidity('cedulaExist', false);
                                    ctrl.$setValidity('cedulaValidator', true);
                                }

                            }).error(function (data, status, headers, config) {
                                console.log("error al traer alumno", data);
                                ctrl.$setValidity('cedulaValidator', false);
                            });
                        } else {
                            ctrl.$setValidity('cedulaValidator', true);
                            ctrl.$setValidity('cedulaChecking', true);
                            ctrl.$setValidity('cedulaExist', true);
                        }
                    } else {
                        ctrl.$setValidity('cedulaValidator', false);
                    }

                    // we need to return our ngModelValue, to be displayed to the user(value of the input)
                    return ngModelValue;
                }

                // we need to add our customValidator function to an array of other(build-in or custom) functions
                // I have not notice any performance issues, but it would be worth investigating how much
                // effect does this have on the performance of the app
                ctrl.$parsers.push(customValidator);
            }
        };
    }]);

})();