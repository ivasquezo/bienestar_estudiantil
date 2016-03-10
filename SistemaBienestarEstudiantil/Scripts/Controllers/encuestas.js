(function () {

    var app = angular.module('BienestarApp', []);

    app.controller('EncuestasController', ['$scope', '$http', '$controller', function ($scope, $http, $controller) {

        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        $scope.poll = {
            title: null,
            description: null,
            questions: []
        };

        $scope.addQuestion = function() {

            var question = {
                text: null,
                type:1,
                responses: [''],
                addResponse: function(){
                    
                    var response = {
                        response: null
                    };

                    this.responses.push(response);
                }
            };

            $scope.poll.questions.push(question);

        };

        $scope.$watch('poll.title', function() {
            var text = document.getElementById('encuestaTitulo');
            text.style.height = 'auto';
            text.style.height = text.scrollHeight + 'px';
            console.log(text.scrollHeight);
        });

    }]);

})();