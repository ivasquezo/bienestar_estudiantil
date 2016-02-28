(function () {
    var app = angular.module('HomeApp', []);

    app.controller('HomeController', function ($scope) {
        $scope.Message = "Mensaje controller funcionando";
        $scope.prueba = "";
    });

    app.controller('DatosController', function ($scope) {
        $scope.Message = "DAtos controller funcionando";
    });

    app.controller('TareasController', function ($scope) {
        $scope.Message = "Tareas Mensaje controller funcionando";
    });

    app.controller('UsuarioController', ['$scope', '$http', function ($scope, $http) {
        $scope.Message = "Tareas Mensaje controller funcionando";
        $scope.cargarUsuarios = function () {
            $http.post('../../WebServices/Users.asmx/getAllActivedUser', {
            }).success(function (data, status, headers, config) {
                $scope.users = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios");
            });
        };

        $scope.cargarUsuarios();
    } ]);
})();