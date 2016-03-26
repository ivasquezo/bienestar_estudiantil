(function () {

    var app = angular.module('BienestarApp', ['ngMessages','cgBusy']);

    app.controller('EncuestaController', ['$scope', '$http', '$controller', function ($scope, $http, $controller) {

        $scope.validName = false;
        $scope.student = {
            CEDULA: ""
        };
        
        // procesing message
        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = true;
        $scope.delay = 2;
        $scope.minDuration = 2;

        // prepare messages
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        $scope.$watch('student.CEDULA', function() {
            
            if ($scope.student != null && $scope.student.CEDULA != undefined && $scope.student.CEDULA.toString().length > 10)
                $scope.student.CEDULA = Number($scope.student.CEDULA.toString().substring(0, 10));
        });

        // function for generate aleatory number for id in encuestas object, for manipulate
        $scope.generateId = function(){
            return Math.floor(Math.random() * 999999) + 100000;
        };

        $scope.loadDefaultSurvey = function(){

            $scope.defaultSurvey = null;
            $scope.promise = $http.post('../../WebServices/Encuestas.asmx/getDefaultSurvey', {
            }).success(function (data, status, headers, config) {

                if (data.success != undefined && data.success) {
                    $scope.defaultSurvey = data.response;
                    
                    // if multiple selection, add handler reponse for validations
                    for (var i = 0; i < $scope.defaultSurvey.ENCUESTA_PREGUNTA.length; i++) {
                        if ($scope.defaultSurvey.ENCUESTA_PREGUNTA[i].TIPO == 1){

                            $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].addAnswered = function(response){

                                if (response.checked) {
                                    this.answereds.push(response.CODIGO);
                                }else{
                                    this.removeAnswered(response.CODIGO);
                                };
                            };
                            
                            $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].removeAnswered = function(id){
                                var index = null;
                                for (var i = 0; i < this.answereds.length; i++) {
                                    if (this.answereds[i] == id) {
                                        index = i;
                                        break;
                                    }
                                };
                                if (index != null) this.answereds.splice(index,1);
                            };

                            $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].answereds = [];
                        }
                    };
                } else
                    $scope.defaultSurvey = null;
                console.log("loadDefaultSurvey:", data);

            }).error(function (data, status, headers, config) {
                console.log("error al traer la encuesta seleccionada", data);
            });
        };

        $scope.loadDefaultSurvey();

        // Tranforma la fecha de milisegundos a fecha completa
        $scope.toDate = function(dateTime) {
            var mEpoch = parseInt(dateTime); 
            var dDate = new Date();

            if(mEpoch<10000000000) mEpoch *= 1000;
        
            dDate.setTime(mEpoch)
            return dDate;
        }

        $scope.enviarForm = function () {

            if (!$scope.formEncuesta.$invalid) {

                var surveySelectResult = [];
                var surveyTextResult = [];

                for (var i = 0; i < $scope.defaultSurvey.ENCUESTA_PREGUNTA.length; i++) {
                    if ($scope.defaultSurvey.ENCUESTA_PREGUNTA[i].TIPO == 1) {

                        for (var j = 0; j < $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].answereds.length; j++) {

                            var question = {
                                FECHA: new Date(),
                                CODIGOENCUESTA: $scope.defaultSurvey.CODIGO,
                                CODIGOALUMNO: $scope.student.CODIGO,
                                CODIGOPREGUNTA: $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].CODIGO,
                                CODIGORESPUESTA: $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].answereds[j]
                            };
                            surveySelectResult.push(question);
                        };
                        
                    } else if ($scope.defaultSurvey.ENCUESTA_PREGUNTA[i].TIPO == 2) {

                        if ($scope.defaultSurvey.ENCUESTA_PREGUNTA[i].VALUE != undefined && $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].VALUE != null) {

                            var question = {
                                FECHA: new Date(),
                                CODIGOENCUESTA: $scope.defaultSurvey.CODIGO,
                                CODIGOALUMNO: $scope.student.CODIGO,
                                CODIGOPREGUNTA: $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].CODIGO,
                                CODIGORESPUESTA: parseInt($scope.defaultSurvey.ENCUESTA_PREGUNTA[i].VALUE)
                            };
                            surveySelectResult.push(question);
                        }

                    } else {

                        if ($scope.defaultSurvey.ENCUESTA_PREGUNTA[i].VALUE != undefined && $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].VALUE != null) {
                            var question = {
                                FECHA: new Date(),
                                CODIGOENCUESTA: $scope.defaultSurvey.CODIGO,
                                CODIGOALUMNO: $scope.student.CODIGO,
                                CODIGOPREGUNTA: $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].CODIGO,
                                TEXTO: $scope.defaultSurvey.ENCUESTA_PREGUNTA[i].VALUE
                            };
                            surveyTextResult.push(question);
                        }
                    }
                };

                console.log("surveySelectResult:", surveySelectResult);
                console.log("surveyTextResult:", surveyTextResult);

                $scope.promise = $http.post('../../WebServices/Encuestas.asmx/saveResponseStudent', {
                    listResponseSelect: surveySelectResult,
                    listResponseText: surveyTextResult
                }).success(function (data, status, headers, config) {
                    console.log(data);
                    $('#messages').puigrowl('show', [{severity: 'info', summary: 'Encuesta realizada', detail: 'Gracias por responder la encuesta. La respuesta fue enviada'}]);
                    $scope.student = null;
                    $scope.loadDefaultSurvey();
                }).error(function (data, status, headers, config) {
                    console.log("error al añadir respuesta...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Ocurrió un error al guardar respuesta'}]);
                });

            } else
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Complete los campos erróneos'}]);
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
                        
                        ctrl.$setValidity('cedulaChecking', false);

                        scope.promise = $http.post('../../WebServices/Encuestas.asmx/getStudentByCedula', {
                            cedula: ngModelValue,
                            codigoEncuesta: scope.defaultSurvey.CODIGO
                        }).success(function (data, status, headers, config) {

                            if (data.CEDULA == ngModelValue) {
                                ctrl.$setValidity('cedulaValidator', true);
                                ctrl.$setValidity('cedulaChecking', true);
                                ctrl.$setValidity('cedulaSurveyDone', true);
                                scope.student = data;
                            } else {                            
                                ctrl.$setValidity('cedulaSurveyDone', false);
                                ctrl.$setValidity('cedulaValidator', true);
                            }

                        }).error(function (data, status, headers, config) {
                            console.log("error al traer alumno", data);
                            ctrl.$setValidity('cedulaValidator', false);
                            scope.student = null;
                        });
                    } else {
                        ctrl.$setValidity('cedulaValidator', false);
                        scope.student = null;
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