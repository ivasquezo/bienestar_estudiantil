﻿(function () {

    var app = angular.module('BienestarApp', ['ui.grid','chart.js','cgBusy']);

    app.controller('EncuestasController', ['$scope', '$http', '$controller', function ($scope, $http, $controller) {

        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = false;
        $scope.delay = 2;
        $scope.minDuration = 2;
        $scope.mode = 'init';
        $scope.view = 'summary';

        // method for load encuestas from server
        $scope.cargarEncuestas = function () {
            $scope.promise = $http.post('../../WebServices/Encuestas.asmx/getAllEncuestas', {
            }).success(function (data, status, headers, config) {
                $scope.gridOptions.data = data;
                $scope.loadDefaultSurvey();
                $scope.cargarEncuestasContestadas(data);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...", data);
            });
        };

        // get survey answered
        $scope.cargarEncuestasContestadas = function () {

            $scope.promise = $http.post('../../WebServices/Encuestas.asmx/surveyAnsweredCodesServices', {
            }).success(function (data, status, headers, config) {
                
                for (var i = 0; i < data.length; i++) {
                    var row = $scope.getElementArray($scope.gridOptions.data, data[i]);
                    row.answered = true;
                };

                console.log($scope.gridOptions.data);

            }).error(function (data, status, headers, config) {
                console.log("error al cargar encuestas contestadas ...", data);
            });
        };

        // grid define options
        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            selectedItems: $scope.mySelections,
            multiSelect: false,
            columnDefs: [
                {name:'Título', field: 'TITULO'},
                {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsEncuestas.html', width: 110, enableFiltering: false}
            ]
        };

        // load encuestas from server and set in the grid
        $scope.cargarEncuestas();

        $scope.labels = ["Download Sales", "In-Store Sales", "Mail-Order Sales"];
        $scope.data = [300, 500, 100];

        // prepare messages
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        // function for generate aleatory number for id in encuestas object, for manipulate
        $scope.generateId = function(){
            return Math.floor(Math.random() * 999999) + 100000;
        };

        // test method for view object in the console
        $scope.encuestaConsole = function(){
            console.log("JSON:",JSON.stringify(this.encuesta));
            console.log("ANGULAR:", angular.toJson(this.encuesta));
        }

        $scope.getElementArray = function(arrayElements, codeElement) {
            for (var i=0; i<arrayElements.length; i++) {
                if (arrayElements[i].CODIGO == codeElement) {
                    return arrayElements[i];
                }
            }
            return null;
        };

        $scope.addEncuesta = function(){
            $scope.mode = "new";
            $scope.encuesta = {
                TITULO: null,
                DESCRIPCION: null,
                ENCUESTA_PREGUNTA: []
            };

            $scope.addHandlerEncuesta($scope.encuesta);
            // init encuesta
            $scope.encuesta.addQuestion();
        }

        this.editEncuesta = function(code){
            var encuesta = $scope.getElementArray($scope.gridOptions.data, code);
            if (encuesta.answered == undefined || !encuesta.answered) {
                $scope.mode = "edit";
                $scope.encuesta = angular.copy(encuesta);
                $scope.addHandlerEncuesta($scope.encuesta);
            } else {
                $scope.mode = "init";
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Información', detail: 'Encuesta ya ha sido contestada, no se puede editar'}]);
            }
        };

        this.getSurverDefaultInput = function(code) {
            var row = $scope.getElementArray($scope.gridOptions.data, code);
            return row.selected != undefined && row.selected == true ? true : false;
        }

        this.setSurverDefaultInput = function(code) {
            var row = $scope.getElementArray($scope.gridOptions.data, code);
            if (row.selected != undefined && row.selected){
                row.selected = false;
            } else {
                for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                    if ($scope.gridOptions.data[i].selected != undefined && $scope.gridOptions.data[i].selected) {
                        $scope.gridOptions.data[i].selected = false;
                        break;
                    }
                };
                $scope.setDefaultSurvey(row);
            }
        }

        $scope.separateQuestionResponse = function (encuesta) {
            for (var i = 0; i < encuesta.preguntas.length; i++) {
                if (encuesta.preguntas[i].tipo != 3) {
                    encuesta.preguntas[i].labelRespuestas = [];
                    encuesta.preguntas[i].valorRespuestas = [];
                    for (var j = 0; j < encuesta.preguntas[i].respuestas.length; j++) {
                        encuesta.preguntas[i].labelRespuestas.push(encuesta.preguntas[i].respuestas[j].nombre);
                        encuesta.preguntas[i].valorRespuestas.push(encuesta.preguntas[i].respuestas[j].cantidad);
                    };
                };
            };
        };

        this.showReport = function(code){
            $scope.mode = "report";
            $scope.promise = $http.post('../../WebServices/Encuestas.asmx/surveysReport', {
                surveyCode: code
            }).success(function (data, status, headers, config) {

                $scope.separateQuestionResponse(data);
                $scope.encuestaReport = data;

            }).error(function (data, status, headers, config) {
                console.log("error in report ...", data);
            });
        };

        $scope.setDefaultSurvey = function(row){
            $scope.promise = $http.post('../../WebServices/Encuestas.asmx/setDefaultSurvey', {
                surveyCode: row.CODIGO
            }).success(function (data, status, headers, config) {
                console.log("setDefaultSurvey:",data);
                row.selected = true;
            }).error(function (data, status, headers, config) {
                console.log("error in setDefaultSurvey...", data);
            });
        };

        $scope.verifyQuestion = function(question){
            if (question.TIPO == 3) {
                question.ENCUESTA_RESPUESTA = [];
            } else {
                if (question.ENCUESTA_RESPUESTA.length == 0)
                    question.addResponse();
            }
        }

        $scope.addHandlerEncuesta = function(encuesta){

            encuesta.addQuestion = function() {

                var question = {
                    id: $scope.generateId(),
                    TITULO: null,
                    TIPO:2,
                    REQUERIDO: true,
                    ENCUESTA_RESPUESTA: [],
                    addResponse: function(){

                        var response = {
                            id: $scope.generateId(),
                            TEXTO: null
                        };

                        this.ENCUESTA_RESPUESTA.push(response);
                    },
                    removeResponse: function(id){

                        var index = null;
                        for (var i = 0; i < this.ENCUESTA_RESPUESTA.length; i++) {
                            if (this.ENCUESTA_RESPUESTA[i].id == id) {
                                index = i;
                                break;
                            }
                        };
                        if (index != null) this.ENCUESTA_RESPUESTA.splice(index,1);
                    },
                    addAnswered: function(response){

                        if (response.checked) {
                            this.answereds.push(response.id);
                        }else{
                            this.removeAnswered(response.id);
                        };
                    },
                    removeAnswered: function(id){
                        var index = null;
                        for (var i = 0; i < this.answereds.length; i++) {
                            if (this.answereds[i] == id) {
                                index = i;
                                break;
                            }
                        };
                        if (index != null) this.answereds.splice(index,1);
                    }
                };

                question.addResponse();
                this.ENCUESTA_PREGUNTA.push(question);
            };
            encuesta.removeQuestion = function(id){
                
                var index = null;
                for (var i = 0; i < this.ENCUESTA_PREGUNTA.length; i++) {
                    if (this.ENCUESTA_PREGUNTA[i].id == id) {
                        index = i;
                        break;
                    }
                };
                if (index != null) this.ENCUESTA_PREGUNTA.splice(index,1);
            };

            for (var i = 0; i < encuesta.ENCUESTA_PREGUNTA.length; i++) {
                encuesta.ENCUESTA_PREGUNTA[i].addResponse = function(){

                    var response = {
                        id: $scope.generateId(),
                        TEXTO: null
                    };

                    this.ENCUESTA_RESPUESTA.push(response);
                };
                encuesta.ENCUESTA_PREGUNTA[i].removeResponse = function(id){

                    var index = null;
                    for (var i = 0; i < this.ENCUESTA_RESPUESTA.length; i++) {
                        if (this.ENCUESTA_RESPUESTA[i].id == id) {
                            index = i;
                            break;
                        }
                    };
                    if (index != null) this.ENCUESTA_RESPUESTA.splice(index,1);
                };

                encuesta.ENCUESTA_PREGUNTA[i].id = $scope.generateId();
                for (var j = 0; j < encuesta.ENCUESTA_PREGUNTA[i].ENCUESTA_RESPUESTA.length; j++) {
                    encuesta.ENCUESTA_PREGUNTA[i].ENCUESTA_RESPUESTA[j].id = $scope.generateId();
                }
            }
        }; // end addHandlerEncuesta

        $scope.addNewEncuesta = function(){

            $http.post('../../WebServices/Encuestas.asmx/addNewEncuesta', {
                encuesta: $scope.encuesta
            }).success(function (data, status, headers, config) {
                $scope.gridOptions.data.push(data);
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Nueva', detail: 'Encuesta guardada.'}]);
            }).error(function (data, status, headers, config) {
                console.log("error al añadir nueva encuesta...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Ocurrió un error al guardar la encuesta.'}]);
            });
        };

        $scope.saveEncuesta = function(){

            console.log($scope.encuesta);

            $http.post('../../WebServices/Encuestas.asmx/saveEncuesta', {
                encuestaEdited: $scope.encuesta
            }).success(function (data, status, headers, config) {
                console.log("saveEncuesta", data);
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Editar', detail: 'Encuesta guardada.'}]);
            }).error(function (data, status, headers, config) {
                console.log("error al añadir nueva encuesta...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Ocurrió un error al guardar la encuesta.'}]);
            });
        };

        this.removeEncuesta = function(code){
            var parentObject = this;
            $http.post('../../WebServices/Encuestas.asmx/removeEncuestaByCode', {
                code: code
            }).success(function (data, status, headers, config) {
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Eliminar', detail: 'Encuesta borrada.'}]);
                parentObject.removeElementArray($scope.gridOptions.data, code);
            }).error(function (data, status, headers, config) {
                console.log("error al borrar encuesta...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Ocurrió un error al borrar la encuesta.'}]);
            });
        };

        this.removeElementArray = function(arrayElements, codeElement) {
            for (var i=0; i<arrayElements.length; i++) {
                if (arrayElements[i].CODIGO == codeElement) {
                    arrayElements.splice(i, 1);
                }
            }
        };

        // marca de inicio como seleccionada la encuesta habilitada para los estudiantes 
        $scope.loadDefaultSurvey = function(){

            $http.post('../../WebServices/Encuestas.asmx/getDefaultSurvey', {
            }).success(function (data, status, headers, config) {
                console.log("loadDefaultSurvey:", data);
                if (data.success != undefined && data.success) {
                    $scope.defaultPoll = data.response;
                    var row = $scope.getElementArray($scope.gridOptions.data, data.response.CODIGO);
                    if (row != undefined && row != null) row.selected = true;
                }
            }).error(function (data, status, headers, config) {
                console.log("error al traer la encuesta seleccionada", data);
            });
        };

        $scope.cambiarVista = function(viewValue) {
            $scope.view = viewValue;
        };

    }]);

})();