(function () {
    var app = angular.module('HomeApp', ['ui.grid']);

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
                $scope.gridOptions.data = data;
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...");
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            columnDefs: [
              {name:'Código', field: 'CODIGOUSUARIO'},
              {name:'Nombre', field: 'NOMBREUSUARIO'},
              {name:'Usuario', field: 'USUARIO1'},
              {name:'Cédula', field: 'CEDULAUSUARIO'},
              {name:'Acción', field: 'CODIGOUSUARIO', cellTemplate: 'actionsUsers.html' }
            ]
        };

        $scope.cargarUsuarios();

        this.removeUser = function (code) {
            var parentObject = this;
            
            $http.post('../../WebServices/Users.asmx/inactiveUserById', {
                id: code
            }).success(function (data, status, headers, config) {
                console.log(data);
                parentObject.removeElementArray($scope.gridOptions.data, code);
            }).error(function (data, status, headers, config) {
                console.log("error al cargar los usuarios...");
            });

        };
        
        this.editUser = function (code) {
            console.log("Editar: " + code);
        };

        this.removeElementArray = function(arrayUser, userCode) {
            for (var i=0; i<arrayUser.length; i++) {
                if (arrayUser[i].CODIGOUSUARIO == userCode) {
                    arrayUser.splice(i, 1);
                }
            }
        };
    }]);
})();