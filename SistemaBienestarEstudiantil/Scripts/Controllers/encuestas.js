(function () {

    var app = angular.module('BienestarApp', []);

    app.controller('EncuestasController', ['$scope', '$http', '$controller', function ($scope, $http, $controller) {

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
            text.style.height = 'auto';
            text.style.height = text.scrollHeight + 'px';
        });

    }]);

})();