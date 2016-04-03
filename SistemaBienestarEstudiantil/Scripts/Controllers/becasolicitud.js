(function () {

    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages','cgBusy']);

    app.controller('BecaSolicitudController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        
        // for procesing message
        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = true;
        $scope.delay = 2;
        $scope.minDuration = 2;

        $scope.seleccion = {
            TIPO: null
        };

        $scope.ALUMNO = null;
        $scope.BECA_SOLICITUD = null;
        $scope.TIPO = null;

        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        $scope.cargarTipos = function () {
            $scope.promise = $http.get('../../WebServices/Becas.asmx/getTipos')
            .success(function (data, status, headers, config) {
                $scope.TIPOS = data;
                console.log("tipos cargados correctamente: ", data);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los tipos...", data);
            });
        };

        $scope.cargarTipos();

        $scope.cargarCodigosAdjunto = function () {
            $scope.promise = $http.get('../../WebServices/Becas.asmx/getListAttach')
            .success(function (data, status, headers, config) {
                $scope.CODIGOSADJUNTOS = data;
                console.log("adjuntos cargados correctamente: ", data);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los tipos...", data);
            });
        };

        $scope.cargarCodigosAdjunto();

        $scope.uploadFileDataBase = function () {
            
           if (this.formFiles.$valid && this.becaSolicitudForm.$valid) {

                if ($scope.BECA_SOLICITUD == null) $scope.BECA_SOLICITUD = {};
                $scope.BECA_SOLICITUD['CEDULA'] = $scope.ALUMNO.DTPCEDULAC;
                $scope.BECA_SOLICITUD['CODIGOTIPO'] = $scope.seleccion.TIPO.CODIGO;
                $scope.BECA_SOLICITUD['APROBADA'] = 0;
                $scope.BECA_SOLICITUD['BE_BECA_ADJUNTO'] = null;
                $scope.BECA_SOLICITUD['BE_BECA_TIPO'] = null;
                $scope.BECA_SOLICITUD['BE_BECA_SOLICITUD_HISTORIAL'] = null;
                $scope.BECA_SOLICITUD['DATOSPERSONALE'] = null;

                console.log($scope.BECA_SOLICITUD);

                $scope.promise = $http.post('../../WebServices/Becas.asmx/saveBecaSolicitud', {
                    beca_solicitud: $scope.BECA_SOLICITUD
                }).success(function (data, status, headers, config) {
                    
                    console.log("beca_solicitud added: ", data);
                    $scope.BECA_SOLICITUD = data;

                    var formElement = document.getElementById('formFiles');
                    var formData = new FormData(formElement);

                    $scope.promise = $http.post('../../WebServices/Becas.asmx/addUploadedFileDataBase?codigoSolicitud=' + $scope.BECA_SOLICITUD.CODIGO,
                        formData,
                        {
                            withCredentials: true,
                            headers: {'Content-Type': undefined
                        },
                        transformRequest: angular.identity
                    }).success(function (data, status, headers, config) {
                        
                        console.log("files uploaded successfull", data);
                        $scope.BECA_SOLICITUD = data.beca_solicitud;

                    }).error(function (data, status, headers, config) {
                        console.log("error al cargar los files...", data);
                    });

                }).error(function (data, status, headers, config) {
                    console.log("error al saveBecaSolicitud", data);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Complete los campos erróneos'}]);
            }
        }

        $scope.removeAttach = function (attachCode) {

            $scope.promise = $http.post('../../WebServices/Becas.asmx/removeAttach', {
                attachCode: attachCode
            }).success(function (data, status, headers, config) {
                console.log(data);
                $scope.cargarCodigosAdjunto();
            }).error(function (data, status, headers, config) {
                console.log("error al eliminarAdjunto...", data);
            });
        }

        $scope.printConsole = function () {
            console.log($scope);
        }

        $scope.getCodeTypesDocuments = function (typesDocuments) {
            var codesTypesDocuments = "";
            if (typesDocuments != undefined)
                for (var i = 0; i < typesDocuments.length; i++) {
                    codesTypesDocuments += typesDocuments[i].CODIGO + ",";
                };
            console.log(codesTypesDocuments);
            return codesTypesDocuments;
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

                        scope.promise = $http.post('../../WebServices/Becas.asmx/getStudentSolicitud', {
                            cedula: ngModelValue
                        }).success(function (data, status, headers, config) {

                            console.log("getStudentSolicitud correctamente: ", data);
                            
                            if (data.alumno != null) {
                                ctrl.$setValidity('cedulaValidator', true);
                                ctrl.$setValidity('cedulaChecking', true);
                                ctrl.$setValidity('cedulaExist', true);

                                scope.BECA_SOLICITUD = data.beca_solicitud;    
                                scope.ALUMNO = data.alumno;
                                if (data.beca_solicitud != null) scope.seleccion.TIPO = data.beca_solicitud.BE_BECA_TIPO;

                            } else {                            
                                ctrl.$setValidity('cedulaExist', false);
                                ctrl.$setValidity('cedulaValidator', true);

                                scope.ALUMNO = null;
                                scope.BECA_SOLICITUD = null;
                                scope.seleccion.TIPO = null;

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

    app.directive('validFileInput', ['$http', function($http) {
        return {

            // limit usage to argument only
            restrict: 'A',

            // require NgModelController, i.e. require a controller of ngModel directive
            require: 'ngModel',

            // create linking function and pass in our NgModelController as a 4th argument
            link: function(scope, element, attr, ctrl) {

                ctrl.$setValidity('validFile', element.val() != '');
                //change event is fired when file is selected
                element.bind('change',function(){
                    
                    if (element.get(0).files.length > 0) {

                        ctrl.$setValidity('validFile', true);
                        ctrl.$setValidity('validFileSize', element.get(0).files[0].size < 2000000);
                        ctrl.$setValidity('validFileEmpty', element.get(0).files[0].size != 0);
                        ctrl.$setValidity('validFileType',
                            element.get(0).files[0].type.toUpperCase().indexOf('APPLICATION/PDF') != -1 || 
                            element.get(0).files[0].type.toUpperCase().indexOf('IMAGE') != -1);
                        
                    } else {
                        ctrl.$setValidity('validFile', false);
                        ctrl.$setValidity('validFileSize', true);
                        ctrl.$setValidity('validFileEmpty', true);
                        ctrl.$setValidity('validFileType', true);
                    }
                    scope.$apply(function(){
                        ctrl.$setViewValue(element.val());
                        ctrl.$render();
                    });
                });
            }
        };
    }]);

})();