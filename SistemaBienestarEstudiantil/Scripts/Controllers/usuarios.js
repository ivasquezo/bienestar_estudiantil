(function () {

    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages','cgBusy']);

    app.controller('UsuariosController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {

        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        // session listener
        document.onclick = function(){
            $http.get( (appContext != undefined ? appContext : "") + '/WebServices/Users.asmx/checkSession')
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
        $scope.backdrop = true;
        $scope.delay = 2;
        $scope.minDuration = 2;

        $scope.cargarNombreEstado = function(statusId) {
            if (statusId == 0) {
                return "Inactivo";
            } else if (statusId == 1) {
                return "Activo";
            }
        };

        $scope.cargarEstadoUsuario = function(usuarios){
            for (var i = 0; i < usuarios.length; i++)
                usuarios[i].NOMBREESTADO = $scope.cargarNombreEstado(usuarios[i].ESTADO);
        };

        $scope.cargarUsuarios = function () {
            $scope.promise = $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Users.asmx/getAllUser', {
            }).success(function (data, status, headers, config) {
                console.log("Usuarios cargados: ", data);
                if (data.success) {
                    $scope.cargarEstadoUsuario(data.response);
                    $scope.gridOptions.data = data.response;
                }
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error cargar usuarios...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los usuarios'}]);
            });
        };

        $scope.cargarRoles = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Rols.asmx/getAllRols', {
            }).success(function (data, status, headers, config) {
                console.log("Roles cargados: ", data);
                if (data.success)
                    $scope.Rols = data.response;
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error cargar roles...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los roles'}]);
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            enableColumnMenus: false,
            columnDefs: [
              {name:'C\u00F3digo', field: 'CODIGO', width: 85},
              {name:'Nombre', field: 'NOMBRECOMPLETO'},
              {name:'Usuario', field: 'NOMBREUSUARIO', width: 200},
              {name:'C\u00E9dula', field: 'CEDULA', width: 100},
              {name:'Correo', field: 'CORREO'},
              {name:'Estado', field: 'NOMBREESTADO', width: 80},
              {name:'Acci\u00F3n', field: 'CODIGO', width: 80, cellTemplate: 'actionsUsers.html', enableFiltering: false, enableSorting: false}
            ]
        };

        $scope.cargarUsuarios();
        $scope.cargarRoles();

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

        $scope.getElementArray = function(arrayUser, userCode) {
            for (var i=0; i<arrayUser.length; i++) {
                if (arrayUser[i].CODIGO == userCode) {
                    return arrayUser[i];
                }
            }
            return null;
        }

        this.editUser = function (code) {
            $scope.userCopy = angular.copy($scope.getElementArray($scope.gridOptions.data, code));

            $scope.userSavedCedula = $scope.userCopy.CEDULA;
            $scope.userSavedName = $scope.userCopy.NOMBREUSUARIO;

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

        $scope.arrayObjectIndexOf = function (arrayList, searchTerm, property) {
            for(var i = 0, len = arrayList.length; i < len; i++) {
                if (arrayList[i][property] === searchTerm) return i;
            }
            return -1;
        }

        $scope.addNewUserDialog = function() {
            $scope.userCopy = {
                ESTADO: true
            };

            $scope.userSavedCedula = null;
            $scope.userSavedName = null;

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

         $scope.addElementArray = function(arrayUser, newUser) {
            arrayUser.push(newUser);

            for (var i = 0; i < arrayUser.length; i++)
                arrayUser[i].NOMBREESTADO = $scope.cargarNombreEstado(newUser.ESTADO)
        };

        this.removeUser = function (code) {
            var parentObject = this;
            
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Users.asmx/removeUserById', {
                id: code
            }).success(function (data, status, headers, config) {
                console.log("Eliminar usuario... ", data);

                if (data.success)
                    parentObject.removeElementArray($scope.gridOptions.data, code);

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al eliminar usuario... ", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al eliminar usuario'}]);
            });

        };

        this.removeElementArray = function(arrayUser, userCode) {
            for (var i=0; i<arrayUser.length; i++) {
                if (arrayUser[i].CODIGO == userCode) {
                    arrayUser.splice(i, 1);
                }
            }
        };
    }]);

    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {
        $scope.password = {
            reset: false
        };

        $scope.saveEditedUser = function () {
            var parentObject = this;

            $scope.userCopy_copy = angular.copy($scope.userCopy);
            $scope.userCopy_copy.NOMBREUSUARIO = $scope.userCopy.NOMBREUSUARIO.toLowerCase();
            $scope.userCopy_copy.NOMBRECOMPLETO = $scope.userCopy.NOMBRECOMPLETO.toUpperCase();

            delete $scope.userCopy_copy['BE_ACTIVIDAD'];
            delete $scope.userCopy_copy['BE_BECA_SOLICITUD_HISTORIAL'];

            if (!this.userForm.$invalid) {
                $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Users.asmx/saveUserData', {
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

                $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Users.asmx/addNewUser', {
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

                            scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Users.asmx/countUserWithCedula', {
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

    app.directive('validUserName', ['$http', function($http) {
        return {
            require: 'ngModel',

            link: function(scope, element, attr, ctrl) {
                function customValidator(ngModelValue) {
                    if (ngModelValue != null && ngModelValue != scope.userSavedName) {

                        ctrl.$setValidity('userNameChecking', false);

                        scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Users.asmx/countUserWithUserName', {
                            userName: ngModelValue
                        }).success(function (data, status, headers, config) {
                            if (data.cantidad == 0) {
                                 ctrl.$setValidity('userNameExist', true);
                                ctrl.$setValidity('userNameValidator', true);
                                ctrl.$setValidity('userNameChecking', true);
                               
                            } else {                            
                                ctrl.$setValidity('userNameExist', false);
                                ctrl.$setValidity('userNameValidator', true);
                            }
                        }).error(function (data, status, headers, config) {
                            console.log("error al traer usuario", data);
                            ctrl.$setValidity('userNameValidator', false);
                        });
                    } else {
                        ctrl.$setValidity('userNameExist', true);
                        ctrl.$setValidity('userNameValidator', true);
                        ctrl.$setValidity('userNameChecking', true);
                        
                    }
                    return ngModelValue;
                }

                ctrl.$parsers.push(customValidator);
            }
        };
    }]);
})();