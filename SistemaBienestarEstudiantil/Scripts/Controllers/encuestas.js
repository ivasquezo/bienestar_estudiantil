(function () {

    var app = angular.module('BienestarApp', ['ui.grid']);

    app.controller('EncuestasController', ['$scope', '$http', '$controller', function ($scope, $http, $controller) {

        // method for load encuestas from server
        $scope.cargarEncuestas = function () {
            $http.post('../../WebServices/Encuestas.asmx/getAllEncuestas', {
            }).success(function (data, status, headers, config) {
                console.log("cargarEncuestas",data);
                $scope.gridOptions.data = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...", data);
            });
        };

        // grid define options
        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: false,
            selectedItems: $scope.mySelections,
            multiSelect: false,
            columnDefs: [
                {name:'Código', field: 'CODIGO', width: 70},
                {name:'Título', field: 'TITULO'},
                {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsEncuestas.html', width: 80}
            ]
        };

        // load encuestas from server and set in the grid
        $scope.cargarEncuestas();

        // prepare messages
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        // function for generate aleatory number for id in encuestas object, for manipulate
        $scope.generateId = function(){
            return Math.floor(Math.random() * 999999) + 100000;
        };

        // listener for encuesta TITULO
        $scope.$watch('encuesta.TITULO', function() {
            var text = document.getElementById('encuestaTitulo');
            if (text != null) {
                text.style.height = 'auto';
                text.style.height = text.scrollHeight + 'px';
            }
        });

        // test method for view object in the console
        $scope.encuestaConsole = function(){
            console.log("JSON:",JSON.stringify(this.encuesta));
            console.log("ANGULAR:", angular.toJson(this.encuesta));
        }

        // quit mode
        $scope.mode = null;

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
            $scope.mode = "edit";
            $scope.encuesta = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            $scope.addHandlerEncuesta($scope.encuesta);
        };

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
                    encuesta.ENCUESTA_PREGUNTA[i].ENCUESTA_RESPUESTA[i].id = $scope.generateId();
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

            $http.post('../../WebServices/Encuestas.asmx/saveEncuesta', {
                encuestaEdited: $scope.encuesta
            }).success(function (data, status, headers, config) {
                console.log("saveEncuesta", data);
                $('#messages').puigrowl('show', [{severity: 'info', summary: 'Nueva', detail: 'Encuesta guardada.'}]);
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

    }]);

})();