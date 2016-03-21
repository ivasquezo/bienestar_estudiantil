(function () {

    var app = angular.module('BienestarApp');

    app.controller('EncuestaController', ['$scope', '$http', '$controller', function ($scope, $http, $controller) {

        $scope.validName = false;

        // prepare messages
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        // function for generate aleatory number for id in encuestas object, for manipulate
        $scope.generateId = function(){
            return Math.floor(Math.random() * 999999) + 100000;
        };

        $scope.loadDefaultSurvey = function(){

            $scope.defaultPoll = null;
            $http.post('../../WebServices/Encuestas.asmx/getDefaultSurvey', {
            }).success(function (data, status, headers, config) {
                console.log("loadDefaultSurvey:", data);
                if (data.success != undefined && data.success) {
                    $scope.defaultPoll = data.response;
                } else $scope.defaultPoll = null;
            }).error(function (data, status, headers, config) {
                console.log("error al traer la encuesta seleccionada", data);
            });
        };

        $scope.loadDefaultSurvey();

        $scope.enviarForm = function () {
            alert("enviado");
        };

    }]);

    app.directive('validId', function() {
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
                    console.log(ngModelValue);
                    if (ngModelValue == "1234567890") {
                        ctrl.$setValidity('idValidator', true);
                    } else {
                        ctrl.$setValidity('idValidator', false);
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
    });

})();