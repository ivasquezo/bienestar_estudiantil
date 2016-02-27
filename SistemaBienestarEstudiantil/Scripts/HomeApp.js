(function () {
    var app = angular.module('HomeApp', []);

<<<<<<< HEAD
    app.controller('UsuarioController', function ($scope) {
        $scope.Message = "Usuarios controller funcionando";
    });
=======
    app.controller('HomeController', function ($scope) {
        $scope.Message = "Mensaje controller funcionando";
        $scope.prueba = "";
    });

>>>>>>> e8e21922537f0b9838ab1cc0e0f7d2a417991ba2
    app.controller('DatosController', function ($scope) {
        $scope.Message = "DAtos controller funcionando";
    });

    app.controller('TareasController', function ($scope) {
        $scope.Message = "Tareas Mensaje controller funcionando";
    });

    app.controller('UsuarioController', ['$scope', '$http', function ($scope, $http) {
        $scope.Message = "Tareas Mensaje controller funcionando";
        $scope.cargarUsuarios = function () {
            $http.post('../../WebServices/Users.asmx/allUser', {
            }).success(function (data, status, headers, config) {
                $scope.users = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios");
            });
        };

        $scope.cargarUsuarios();
    } ]);
})();