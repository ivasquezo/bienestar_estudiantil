(function () {

    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages','cgBusy']);

    app.controller('BecaSolicitudController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        
        // for procesing message
        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = false;
        $scope.delay = 2;
        $scope.minDuration = 2;

        $scope.TIPO = null;

        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        $scope.cargarTipos = function () {
            $scope.promise = $http.post('../../WebServices/Becas.asmx/getTipos', {
            }).success(function (data, status, headers, config) {
                $scope.TIPOS = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los tipos...", data);
            });
        };

        $scope.cargarTipos();

        $scope.uploadFile = function () {
            var formElement = document.getElementById('formFile');
            var formData = new FormData(formElement);
            
            /*
            $scope.promise = $http.post('../../WebServices/Becas.asmx/saveFileTest', {
                form: formData
            }).success(function (data, status, headers, config) {
                console.log(data);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los tipos...", data);
            });
            */

            $http.post('../../WebServices/Becas.asmx/saveFileTest', formData, {
                withCredentials: true,
                headers: {'Content-Type': undefined },
                transformRequest: angular.identity
            }).success(function (data, status, headers, config) {
                console.log(data);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los tipos...", data);
            });
        }

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