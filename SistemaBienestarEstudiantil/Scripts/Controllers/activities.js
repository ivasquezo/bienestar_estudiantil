(function () {
    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages']);

    app.controller('ActivitiesController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        $scope.activityCopy = {
            CODIGO: ''
        };

        $scope.convertDate = function(arrayActivity) {
            for (var i = 0; i < arrayActivity.length; i++) {
                if (arrayActivity[i].FECHA != null)
                    arrayActivity[i].FECHA = arrayActivity[i].FECHA.substring(6, arrayActivity[i].FECHA.length-2);
            };
        };

        $scope.chargeGeneralActivities = function () {
            $http.post('../../WebServices/Activities.asmx/getAllGeneralActivities', {
            }).success(function (data, status, headers, config) {
                console.log("Cargar actividades... ", data);
                if (data.success) {
                    $scope.convertDate(data.response);
                    $scope.gridOptions.data = data.response;
                } else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar las actividades... ", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las actividades'}]);
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            columnDefs: [
              {name:'Código', field: 'CODIGO'},
              {name:'Actividad general', field: 'NOMBREACTIVIDAD'},
              {name:'Actividad', field: 'NOMBRE'},
              {name:'Fecha', field: 'FECHA', type: 'date', cellFilter: 'date:\'dd/MM/yyyy\''},
              {name:'Estado', field: 'ESTADO', cellTemplate: "<div style='margin-top:2px;'>{{row.entity.ESTADO == 0 ? 'Inactivo' : row.entity.ESTADO == 1 ? 'En Proceso' : row.entity.ESTADO == 2 ? 'Procesado' : 'Finalizado'}}</div>"},
              {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsActivities.html', width: 80, enableFiltering: false}
            ]
        };

        $scope.chargeGeneralActivities();

        this.editActivity = function (code) {
            $scope.activityEdit = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            
            $scope.activityCopy.CODIGOACTIVIDAD = $scope.activityEdit.CODIGOACTIVIDAD;
            $scope.activityCopy.NOMBREACTIVIDAD = $scope.activityEdit.NOMBREACTIVIDAD;
            $scope.activityCopy.CODIGO = $scope.activityEdit.CODIGO;
            $scope.activityCopy.NOMBRE = $scope.activityEdit.NOMBRE;
            $scope.activityCopy.FECHA = $scope.toDate($scope.activityEdit.FECHA);
            $scope.activityCopy.ESTADO = $scope.activityEdit.ESTADO;
            $scope.activityCopy.OBSERVACION = $scope.activityEdit.OBSERVACION;
            $scope.getGeneralActivities();
            $scope.getGroupLevel($scope.activityEdit.CODIGO);
            $scope.groupActivity = [];
            $scope.groupLevelActivity = [];

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

        $scope.getGroupLevel = function (code) {
            if (code > 0) {
                $http.post('../../WebServices/Activities.asmx/getGroupLevelByActivityId', {
                    activityId: code
                }).success(function (data, status, headers, config) {
                    console.log("Niveles de la actividad... ", data);
                    $scope.groupActivity = data.response;
                }).error(function (data, status, headers, config) {
                    console.log("Error al cargar niveles de actividad...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los niveles de la actividad'}]);
                });
            }

            $http.post('../../WebServices/Activities.asmx/getAllGroupLevels'
            ).success(function (data, status, headers, config) {
                console.log("Niveles existentes... ", data);
                $scope.allGroupLevel = data.response;
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar los niveles...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los niveles existentes'}]);
            });
        };

        $scope.existGroupLevel = function (code) {
            if ($scope.groupActivity.length > 0) {
                for (var i = 0; i < $scope.groupActivity.length; i++) {
                    if($scope.groupActivity[i].CODIGOGRUPO == code) 
                        return true;
                };
            }
            return false;
        };

        $scope.setGroupLevel = function(code) {            
            $scope.groupLevelActivity.push(code);            
        };  

        $scope.getGeneralActivities = function () {     
            $http.post('../../WebServices/Activities.asmx/getAllGeneralActivity'
            ).success(function (data, status, headers, config) {
                console.log("Actividades generales existentes... ", data);
                $scope.allGeneralActivities = [];

                for (var i = 0; i < data.response.length; i++)
                    $scope.allGeneralActivities.push({value: data.response[i].CODIGO, name:data.response[i].NOMBRE});
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar las actividades generales...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las actividades generales existentes'}]);
            });
        };

        $scope.toDate = function(dateTime) {
            var mEpoch = parseInt(dateTime); 
            var dDate = new Date();

            if(mEpoch<10000000000) mEpoch *= 1000;
        
            dDate.setTime(mEpoch)
            return dDate;
        }

        this.removeActivity = function (code) {
            var parentObject = this;
            
            $http.post('../../WebServices/Activities.asmx/removeActivityById', {
                activityId: code
            }).success(function (data, status, headers, config) {
                console.log("Eliminar actividad... ", data);
                
                if (data.success)
                    parentObject.removeElementArray($scope.gridOptions.data, code);

                $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al eliminar la actividad... ", data);
                // Si hubo error al eliminar la actividad
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al eliminar la actividad'}]);
            });
        };

		this.removeElementArray = function(arrayActivity, activityCode) {
            for (var i=0; i<arrayActivity.length; i++) {
                if (arrayActivity[i].CODIGO == activityCode) {
                    arrayActivity.splice(i, 1);
                }
            }
        };

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
		
        // Para agregar una actividad
		$scope.addNewActivityDialog = function() {
            $scope.activityCopy = {
                ESTADO: true,
                OBSERVACION: ''
            };

            ngDialog.open({
                template: 'newActivity.html',
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
		
        // Agrega item a la lista
        $scope.addElementArray = function(arrayActivity, newActivity, activityDate) {
            newActivity.FECHA = activityDate;
            arrayActivity.push(newActivity);
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

        // Al agregar una actividad
        $scope.addNewActivityDB = function () {
            var newParentObject = this;

            $http.post('../../WebServices/Activities.asmx/addNewActivity', {                
                activityName: $scope.activityCopy.NOMBRE,
                activityDate: $scope.activityCopy.FECHA,
                activityStatus: $scope.activityCopy.ESTADO,
                activityObservation: $scope.activityCopy.OBSERVACION
            }).success(function (data, status, headers, config) {
                console.log("Agregar actividad: ", data);
                if (data.success) {
                    $scope.addElementArray($scope.gridOptions.data, data.response, Date.parse($scope.activityCopy.FECHA));
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