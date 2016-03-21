(function () {
    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages']);

    app.controller('ActivitiesController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

		$scope.activityCopy = {
            CODIGO: ''
        };

        // Convierte la fecha en dd/mm/aaaa
        $scope.convertDate = function(arrayActivity) {
            for (var i = 0; i < arrayActivity.length; i++) {
                if (arrayActivity[i].FECHA != null)
                    arrayActivity[i].FECHA = arrayActivity[i].FECHA.substring(6, arrayActivity[i].FECHA.length-2);
            };
        };
		
        // Cargar todos las actividades
        $scope.chargeActivities = function () {
            // Llama al servicio que obtiene todas las actividades
            $http.post('../../WebServices/Activities.asmx/getAllActivities', {
            }).success(function (data, status, headers, config) {
                console.log("Cargar actividades... ", data);
                // Si los datos se obtuvieron sin problemas
                if (data.success) {
                    $scope.convertDate(data.response);
                    $scope.gridOptions.data = data.response;
                } else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar las actividades... ", data);
                // Si hubo error al obtener las actividades
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las actividades'}]);
            });
        };
		
        // Diseno de los datos de la tabla
        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: false,
            columnDefs: [
              {name:'Código', field: 'CODIGO'},
              {name:'Nombre', field: 'NOMBRE'},
              {name:'Fecha', field: 'FECHA', type: 'date', cellFilter: 'date:\'dd/MM/yyyy\''},
              {name:'Estado', field: 'ESTADO'},
              {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsActivities.html', width: 80}
            ]
        };

        // Llama al metodo cargar actividades
        $scope.chargeActivities();

        // Eliminar una actividad
        this.removeActivity = function (code) {
            var parentObject = this;
            
            // Llama al servicio que elimina una actividad por su codigo
            $http.post('../../WebServices/Activities.asmx/removeActivityById', {
                activityId: code
            }).success(function (data, status, headers, config) {
                console.log("Eliminar actividad... ", data);
                // Si se elimina correctamente el rol
                if (data.success)
                    parentObject.removeElementArray($scope.gridOptions.data, code);

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al eliminar la actividad... ", data);
                // Si hubo error al eliminar la actividad
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al eliminar la actividad'}]);
            });
        };

        // Elimina la actividad de la vista
		this.removeElementArray = function(arrayActivity, activityCode) {
            for (var i=0; i<arrayActivity.length; i++) {
                if (arrayActivity[i].CODIGO == activityCode) {
                    arrayActivity.splice(i, 1);
                }
            }
        };
		
        // Editar una actividad
        this.editActivity = function (code) {
            // Obtiene la actividad que se va a editar
            $scope.activityEdit = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            
            $scope.activityCopy.CODIGO = $scope.activityEdit.CODIGO;
            $scope.activityCopy.NOMBRE = $scope.activityEdit.NOMBRE;
            $scope.activityCopy.FECHA = $scope.toDate($scope.activityEdit.FECHA);
            $scope.activityCopy.ESTADO = $scope.activityEdit.ESTADO;
            $scope.activityCopy.OBSERVACION = $scope.activityEdit.OBSERVACION;
			
            // Abre el pop up para editar la actividad
            ngDialog.open({
                template: 'editActivity.html',
                className: 'ngdialog-theme-flat ngdialog-theme-custom',
                closeByDocument: true,
                closeByEscape: true,
                scope: $scope,
                controller: $controller('ngDialogController', {
                    $scope: $scope,
                    $http: $http
                })
            });
        };

        // Tranforma la fecha de milisegundos a fecha completa
        $scope.toDate = function(dateTime) {
            var mEpoch = parseInt(dateTime); 
            var dDate = new Date();

            if(mEpoch<10000000000) mEpoch *= 1000;
        
            dDate.setTime(mEpoch)
            return dDate;
        }

        // Obtiene los datos de una actividad
		$scope.getElementArray = function(arrayActividad, activityCode) {
            for (var i=0; i<arrayActividad.length; i++) {
                if (arrayActividad[i].CODIGO == activityCode) 
                    return arrayActividad[i];
            }
            return null;
        }

        // Actualiza la actividad en la parte visual
		$scope.updateElementArray = function(arrayActivity, activityId, activityName, activityDate, activityStatus, activityObservation) {
            for (var i=0; i<arrayActivity.length; i++) {
                if (arrayActivity[i].CODIGO == activityId) {
                    arrayActivity[i].NOMBRE = activityName;
                    arrayActivity[i].FECHA = activityDate;
                    arrayActivity[i].ESTADO = activityStatus;
                    arrayActivity[i].OBSERVACION = activityObservation;
                }
            }
        };
		
		$scope.addNewRolDialog = function() {
            $scope.rolCopy = {
                NOMBRE: ''
            };
            $scope.getAccessByRol(0);
            $scope.accessRols = [];

            ngDialog.open({
                template: 'newRol.html',
                className: 'ngdialog-theme-flat ngdialog-theme-custom',
                closeByDocument: true,
                closeByEscape: true,
                scope: $scope,
                controller: $controller('ngDialogController', {
                    $scope: $scope,
                    $http: $http
                })
            });
        };
		
        $scope.addElementArray = function(arrayRol, newRol) {
            arrayRol.push(newRol);
        }; 

		
		
		$scope.existAccess = function (code) {
			if ($scope.rolsAccess.length > 0) {
				for (var i = 0; i < $scope.rolsAccess.length; i++) {
                    if($scope.rolsAccess[i].CODIGO == code && $scope.rolsAccess[i].VALIDO == true) return true;
                };
			}
            return false;
		};

        $scope.setAccessRol = function(codeAccess) {            
            $scope.accessRols.push(codeAccess);            
        };      
    }]);

    // Pop up para actualizar y agregar actividades
    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {
        // Al editar una actividad
        $scope.saveEditedActivity = function () {
            var parentObject = this;

            $http.post('../../WebServices/Activities.asmx/saveActivityData', {
                activityId: $scope.activityCopy.CODIGO,
                activityName: $scope.activityCopy.NOMBRE,
                activityDate: $scope.activityCopy.FECHA,
                activityStatus: $scope.activityCopy.ESTADO,
                activityObservation: $scope.activityCopy.OBSERVACION
            }).success(function (data, status, headers, config) {
                console.log("Editar actividad: ", data);
                // Si los datos se editaron correctamente
                if (data.success) {
                    $scope.updateElementArray($scope.gridOptions.data, $scope.activityCopy.CODIGO, $scope.activityCopy.NOMBRE, 
                        Date.parse($scope.activityCopy.FECHA), $scope.activityCopy.ESTADO, $scope.activityCopy.OBSERVACION);
                    // Se cierra el pop up
                    parentObject.closeThisDialog();
                } 

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al editar la actividad...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al actualizar la actividad'}]);
            });
        };

        // Al agregar un rol
        $scope.addNewRolDB = function () {
            var newParentObject = this;

            $http.post('../../WebServices/Rols.asmx/addNewRol', {                
                rolName: $scope.rolCopy.NOMBRE,
                accessRols: $scope.accessRols
            }).success(function (data, status, headers, config) {
                console.log("Agregar rol: ", data);
                if (data.success) {
                    $scope.addElementArray($scope.gridOptions.data, data.response);
                    // Se cierra el pop up
                    newParentObject.closeThisDialog();
                }

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al agregar el rol...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar el rol'}]);
            });
        };
    }]);
})();