(function () {
    var app = angular.module('HomeApp', []);

    app.controller('HomeController', function ($scope) {
        $scope.Message = "Mensaje controller funcionando";
        $scope.prueba = "";
    });
    app.controller('UsuarioController', function ($scope) {
        $scope.Message = "Usuarios controller funcionando";
    });
    app.controller('DatosController', function ($scope) {
        $scope.Message = "DAtos controller funcionando";
    });
    app.controller('TareasController', function ($scope) {
        $scope.Message = "Tareas Mensaje controller funcionando";
    });
})();