(function () {

    var app = angular.module('BienestarApp', ['ui.grid','chart.js','cgBusy']);

    app.controller('EncuestasController', ['$scope', '$http', '$controller', function ($scope, $http, $controller) {

        // session listener
        document.onclick = function(){
            $http.get('/WebServices/Users.asmx/checkSession')
            .success(function (data, status, headers, config) {
                if (!data.success) {
                    document.location.href = "/";
                }
            }).error(function (data, status, headers, config) {
                console.log("Error checkSession", data);
            });
        };

        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = false;
        $scope.delay = 2;
        $scope.minDuration = 2;
        $scope.mode = 'init';
        $scope.view = 'summary';

        // method for load periodos
        $scope.cargarPeriodos = function () {
            $scope.promise = $http.get( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/getPeriodos')
            .success(function (data, status, headers, config) {
                $scope.PERIODOS = data;
                for (var i = 0; i < $scope.PERIODOS.length; i++) {
                    $scope.PERIODOS[i].PRDFECFINF = $scope.convertDate($scope.PERIODOS[i].PRDFECFINF);
                    $scope.PERIODOS[i].PRDFECINIF = $scope.convertDate($scope.PERIODOS[i].PRDFECINIF);
                };
                //console.log("periodos:", data);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar periodos...", data);
            });
        };

        // method for load encuestas from server
        $scope.cargarEncuestas = function () {
            $scope.promise = $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/getAllEncuestas', {
            }).success(function (data, status, headers, config) {
                $scope.gridOptions.data = data;
                $scope.loadDefaultSurvey();
                $scope.cargarEncuestasContestadas();
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...", data);
            });
        };

        // get survey answered
        $scope.cargarEncuestasContestadas = function () {

            $scope.promise = $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/surveyAnsweredCodesServices', {
            }).success(function (data, status, headers, config) {
                
                for (var i = 0; i < data.length; i++) {
                    var row = $scope.getElementArray($scope.gridOptions.data, data[i]);
                    row.answered = true;
                };

                //console.log($scope.gridOptions.data);

            }).error(function (data, status, headers, config) {
                console.log("error al cargar encuestas contestadas ...", data);
            });
        };

        $scope.convertDate = function(fecha) {
            return fecha.substring(6, fecha.length-2);
        };

        // grid define options
        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            enableColumnMenus: false,
            selectedItems: $scope.mySelections,
            multiSelect: false,
            columnDefs: [
                {name:'Título', field: 'TITULO'},
                {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsEncuestas.html', width: 130, enableFiltering: false, enableSorting: false}
            ]
        };

        // load encuestas from server and set in the grid
        $scope.cargarEncuestas();
        $scope.cargarPeriodos();

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
                BE_ENCUESTA_PREGUNTA: []
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
            return row != null && row.selected != undefined && row.selected == true ? true : false;
        }

        this.setSurverDefaultInput = function(code) {
            var row = $scope.getElementArray($scope.gridOptions.data, code);
            if (row.selected != undefined && row.selected){
                row.selected = false;
                $scope.setDefaultSurvey(0);
            } else {
                for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                    if ($scope.gridOptions.data[i].selected != undefined && $scope.gridOptions.data[i].selected) {
                        $scope.gridOptions.data[i].selected = false;
                        break;
                    }
                };
                row.selected = true;
                $scope.setDefaultSurvey(row.CODIGO);
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

        $scope.getPeriodoSiguiente = function (periodo) {
            
            for (var i = 0; i < $scope.PERIODOS.length; i++) {
                if ($scope.PERIODOS[i].PRDCODIGOI == periodo.PRDCODIGOI && i <= $scope.PERIODOS.length - 1) {
                    return $scope.PERIODOS[i+1];
                };
            };
            return null;
        }

        $scope.toDate = function(dateTime) {
            var mEpoch = parseInt(dateTime); 
            var dDate = new Date();

            if(mEpoch<10000000000) mEpoch *= 1000;
        
            dDate.setTime(mEpoch)
            return dDate;
        }

        $scope.restarDias = function (date, days) {
            if (date == null) return null;
            var date = parseInt(date);
            return date - 24*60*60*1000*days;
        }

        $scope.viewReport = function(){

            if ($scope.PERIODO != undefined && $scope.PERIODO != null) {

                $scope.PERIODOSIGUIENTE = $scope.getPeriodoSiguiente($scope.PERIODO);
                var iniDate = $scope.toDate($scope.PERIODO.PRDFECINIF);
                var endDate = ($scope.PERIODOSIGUIENTE != null ? $scope.toDate($scope.PERIODOSIGUIENTE.PRDFECINIF) : null);

                $scope.promise = $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/surveysReport', {
                    surveyCode: $scope.CODIDOENCUESTA,
                    iniDate: iniDate,
                    endDate: endDate
                }).success(function (data, status, headers, config) {

                    $scope.separateQuestionResponse(data);
                    $scope.encuestaReport = data;
                    //console.log("showReport: ", data);

                }).error(function (data, status, headers, config) {
                    console.log("error in report ...", data);
                });

            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Debe seleccionar el periodo'}]);
            }
        };

        this.showReport = function(code){
            $scope.mode = "report";
            $scope.CODIDOENCUESTA = code;
            if ($scope.PERIODO != undefined && $scope.PERIODO != null) {
                $scope.viewReport();
            }
        };

        $scope.setDefaultSurvey = function(code){
            $scope.promise = $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/setDefaultSurvey', {
                surveyCode: code
            }).success(function (data, status, headers, config) {
                //console.log("setDefaultSurvey:",data);
            }).error(function (data, status, headers, config) {
                console.log("error in setDefaultSurvey...", data);
            });
        };

        $scope.verifyQuestion = function(question){
            if (question.TIPO == 3) {
                question.BE_ENCUESTA_RESPUESTA = [];
            } else {
                if (question.BE_ENCUESTA_RESPUESTA.length == 0)
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
                    BE_ENCUESTA_RESPUESTA: [],
                    addResponse: function(){

                        var response = {
                            id: $scope.generateId(),
                            TEXTO: null
                        };

                        this.BE_ENCUESTA_RESPUESTA.push(response);
                    },
                    removeResponse: function(id){

                        var index = null;
                        for (var i = 0; i < this.BE_ENCUESTA_RESPUESTA.length; i++) {
                            if (this.BE_ENCUESTA_RESPUESTA[i].id == id) {
                                index = i;
                                break;
                            }
                        };
                        if (index != null) this.BE_ENCUESTA_RESPUESTA.splice(index,1);
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
                this.BE_ENCUESTA_PREGUNTA.push(question);
            };
            encuesta.removeQuestion = function(id){
                
                var index = null;
                for (var i = 0; i < this.BE_ENCUESTA_PREGUNTA.length; i++) {
                    if (this.BE_ENCUESTA_PREGUNTA[i].id == id) {
                        index = i;
                        break;
                    }
                };
                if (index != null) this.BE_ENCUESTA_PREGUNTA.splice(index,1);
            };

            for (var i = 0; i < encuesta.BE_ENCUESTA_PREGUNTA.length; i++) {
                encuesta.BE_ENCUESTA_PREGUNTA[i].addResponse = function(){

                    var response = {
                        id: $scope.generateId(),
                        TEXTO: null
                    };

                    this.BE_ENCUESTA_RESPUESTA.push(response);
                };
                encuesta.BE_ENCUESTA_PREGUNTA[i].removeResponse = function(id){

                    var index = null;
                    for (var i = 0; i < this.BE_ENCUESTA_RESPUESTA.length; i++) {
                        if (this.BE_ENCUESTA_RESPUESTA[i].id == id) {
                            index = i;
                            break;
                        }
                    };
                    if (index != null) this.BE_ENCUESTA_RESPUESTA.splice(index,1);
                };

                encuesta.BE_ENCUESTA_PREGUNTA[i].id = $scope.generateId();
                for (var j = 0; j < encuesta.BE_ENCUESTA_PREGUNTA[i].BE_ENCUESTA_RESPUESTA.length; j++) {
                    encuesta.BE_ENCUESTA_PREGUNTA[i].BE_ENCUESTA_RESPUESTA[j].id = $scope.generateId();
                }
            }
        }; // end addHandlerEncuesta

        $scope.addNewEncuesta = function(){

            //console.log($scope.encuesta);
            $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/addNewEncuesta', {
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

            //console.log($scope.encuesta);

            $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/saveEncuesta', {
                encuestaEdited: $scope.encuesta
            }).success(function (data, status, headers, config) {
                //console.log("saveEncuesta", data);
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Editar', detail: 'Encuesta guardada.'}]);
            }).error(function (data, status, headers, config) {
                console.log("error al añadir nueva encuesta...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Ocurrió un error al guardar la encuesta.'}]);
            });
        };

        this.removeEncuesta = function(code){
            var parentObject = this;
            $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/removeEncuestaByCode', {
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

            $http.post( (appContext != undefined ? appContext : "") + '/WebServices/Encuestas.asmx/getDefaultSurvey', {
            }).success(function (data, status, headers, config) {
                //console.log("loadDefaultSurvey:", data);
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