(function () {

    var app = angular.module('BienestarApp', ['ui.grid']);

    app.controller('EncuestasController', ['$scope', '$http', '$controller', function ($scope, $http, $controller) {

        $scope.cargarEncuestas = function () {
            $http.post('../../WebServices/Encuestas.asmx/getAllEncuestas', {
            }).success(function (data, status, headers, config) {
                console.log("cargarEncuestas",data);
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
              {name:'TITULO', field: 'TITULO'}
            ]
        };

        $scope.cargarEncuestas();

        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        $scope.generateId = function(){
            return Math.floor(Math.random() * 999999) + 100000;
        };

        $scope.poll = {
            title: null,
            description: null,
            questions: [],
            addQuestion: function() {

                var question = {
                    id: $scope.generateId(),
                    title: null,
                    type:2,
                    required: true,
                    responses: [],
                    answereds:[],
                    addResponse: function(){

                        var response = {
                            id: $scope.generateId(),
                            text: null,
                            checked: false
                        };

                        this.responses.push(response);
                    },
                    removeResponse: function(id){

                        var index = null;
                        for (var i = 0; i < this.responses.length; i++) {
                            if (this.responses[i].id == id) {
                                index = i;
                                break;
                            }
                        };
                        if (index != null) this.responses.splice(index,1);
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
                this.questions.push(question);
            },
            removeQuestion: function(id){
                
                var index = null;
                for (var i = 0; i < this.questions.length; i++) {
                    if (this.questions[i].id == id) {
                        index = i;
                        break;
                    }
                };
                if (index != null) this.questions.splice(index,1);
            }
        };

        // init poll
        $scope.poll.addQuestion();

        $scope.$watch('poll.title', function() {
            var text = document.getElementById('encuestaTitulo');
            if (text != null) {
                text.style.height = 'auto';
                text.style.height = text.scrollHeight + 'px';
            }
        });

        $scope.pollConsole = function(){
            console.log("JSON:",JSON.stringify(this.poll));
            console.log("ANGULAR:", angular.toJson(this.poll));
        }

        $scope.mode = null;

        $scope.addEncuesta = function(){
            $scope.mode = "edit";
            $scope.$apply();
        }
    }]);

})();