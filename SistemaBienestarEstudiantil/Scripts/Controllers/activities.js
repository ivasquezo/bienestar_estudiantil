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
                    $scope.cargarEstadoActividades(data.response);
                    $scope.gridOptions.data = data.response;
                } else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar las actividades... ", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las actividades'}]);
            });
        };

        $scope.cargarNombreEstado = function(statusId) {
            if (statusId == 0) {
                return "Inactivo";
            } else if (statusId == 1) {
                return "En proceso";
            } else if (statusId == 2) {
                return "Procesado";
            } else if (statusId == 3) {
                return "Finalizado";
            }
        };

        $scope.cargarEstadoActividades = function(actividades){
            for (var i = 0; i < actividades.length; i++)
                actividades[i].NOMBREESTADO = $scope.cargarNombreEstado(actividades[i].ESTADO);
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            columnDefs: [
              {name:'Código', field: 'CODIGO'},
              {name:'Actividad general', field: 'NOMBREACTIVIDAD'},
              {name:'Actividad', field: 'NOMBRE'},
              {name:'Fecha', field: 'FECHA', type: 'date', cellFilter: 'date:\'dd/MM/yyyy\''},
              {name:'Estado', field: 'NOMBREESTADO'},
              {name:'Acción', field: 'CODIGO', cellTemplate: 'actionsActivities.html', width: 160, enableFiltering: false}
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
            $scope.activityCopy.CODIGOUSUARIO = $scope.activityEdit.CODIGOUSUARIO;
            $scope.activityCopy.NOMBREESTADO = $scope.cargarNombreEstado($scope.activityEdit.ESTADO);
            
            // Llena el combo de las actividades            
            $scope.getGeneralActivities();
            // Llena los niveles
            $scope.getGroupLevel($scope.activityEdit.CODIGO);
            $scope.groupActivity = [];
            $scope.groupLevelActivity = [];
            // Llena los responsables
            $scope.getAllResponsables();

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

        $scope.getElementArray = function(arrayActividad, activityCode) {
            for (var i=0; i<arrayActividad.length; i++) {
                if (arrayActividad[i].CODIGO == activityCode) 
                    return arrayActividad[i];
            }
            return null;
        }

        $scope.toDate = function(dateTime) {
            var mEpoch = parseInt(dateTime); 
            var dDate = new Date();

            if(mEpoch<10000000000) mEpoch *= 1000;
        
            dDate.setTime(mEpoch)
            return dDate;
        }

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

        $scope.getGroupLevel = function (code) {
            if (code > 0) {
                $http.post('../../WebServices/Activities.asmx/getGroupLevelByActivityId', {
                    activityId: code
                }).success(function (data, status, headers, config) {
                    console.log("Niveles de la actividad... ", data);
                    $scope.groupActivity = data.response;

                    $scope.allLevelAssistance = [];

                    for (var i = 0; i < $scope.groupActivity.length; i++) {
                        console.log("Estado: ", $scope.groupActivity[i].ESTADO);
                        if ($scope.groupActivity[i].ESTADO == true)
                            $scope.allLevelAssistance.push({value: $scope.groupActivity[i].CODIGOGRUPO, name:$scope.groupActivity[i].NIVEL + " " + $scope.groupActivity[i].PARALELO + " - " + $scope.groupActivity[i].MODALIDAD});
                    }                        
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
                    if($scope.groupActivity[i].CODIGOGRUPO == code && $scope.groupActivity[i].ESTADO == true) 
                        return true;
                };
            }
            return false;
        };

        $scope.setGroupLevel = function(code) { 
            var existGroup = false;

            if ($scope.groupLevelActivity != null && $scope.groupLevelActivity.length > 0) {
                for (var i=0; i<$scope.groupLevelActivity.length; i++) {
                    if ($scope.groupLevelActivity[i] == code)
                        existGroup = true;
                }
            } 

            if (existGroup) {
                for (var i=0; i<$scope.groupLevelActivity.length; i++) {
                    if ($scope.groupLevelActivity[i] == code)
                        $scope.groupLevelActivity.splice(i, code);
                }
            }                
            else
                $scope.groupLevelActivity.push(code); 
        };

        $scope.getAllResponsables = function () {     
            $http.post('../../WebServices/Activities.asmx/getAllResponsables'
            ).success(function (data, status, headers, config) {
                console.log("Docentes... ", data);
                $scope.allResponsables = [];

                for (var i = 0; i < data.response.length; i++)
                    $scope.allResponsables.push({value: data.response[i].CODIGO, name:data.response[i].NOMBRECOMPLETO});
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar los docentes...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los docentes existentes'}]);
            });
        };
        
        $scope.updateElementArray = function(arrayActivity, generalActivityId, activityId, activityName, activityDate, activityStatus, observation, responsableId) {
            var nameActivity = '';

            for (var i = 0; i < $scope.allGeneralActivities.length; i++)
                if ($scope.allGeneralActivities[i].value == generalActivityId)
                    nameActivity = $scope.allGeneralActivities[i].name;

            for (var i=0; i<arrayActivity.length; i++) {
                if (arrayActivity[i].CODIGO == activityId) {                    
                    arrayActivity[i].CODIGOACTIVIDAD = generalActivityId;
                    arrayActivity[i].NOMBREACTIVIDAD = nameActivity;
                    arrayActivity[i].NOMBRE = activityName;
                    arrayActivity[i].FECHA = activityDate;
                    arrayActivity[i].ESTADO = activityStatus;
                    arrayActivity[i].NOMBREESTADO =  $scope.cargarNombreEstado(activityStatus);
                    arrayActivity[i].OBSERVACION = observation;  
                    arrayActivity[i].CODIGOUSUARIO = responsableId;
                }
            }
        };

        $scope.addNewActivityDialog = function() {
            $scope.activityCopy = {
                OBSERVACION: ''
            };

            // Llena el combo de las actividades            
            $scope.getGeneralActivities();
            // Llena los niveles
            $scope.getGroupLevel(0);
            $scope.groupActivity = [];
            $scope.groupLevelActivity = [];
            // Llena los responsables
            $scope.getAllResponsables();

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

        $scope.addElementArray = function(arrayActivity, newActivity, activityDate, generalActivityId, activityStatus) {
            var nameActivity = '';

            for (var i = 0; i < $scope.allGeneralActivities.length; i++)
                if ($scope.allGeneralActivities[i].value == generalActivityId)
                    nameActivity = $scope.allGeneralActivities[i].name;
            newActivity.FECHA = activityDate;
            newActivity.CODIGOACTIVIDAD = generalActivityId;
            newActivity.NOMBREACTIVIDAD = nameActivity;
            newActivity.NOMBREESTADO =  $scope.cargarNombreEstado(activityStatus);

            arrayActivity.push(newActivity);
        }; 

        this.getAssistance = function (code) {
            $scope.activityAssistanceCopy = {
                CODIGO: ''
            };

            $scope.activityAssistanceCopy.CODIGOACTIVIDAD = code;

            // Llena los niveles
            $scope.getGroupLevel(code);
            $scope.assistance = [];
            $scope.studentsData = [];
            
            ngDialog.open({
                template: 'assistanceActivity.html',
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

        $scope.chargeStudents = function (code) {
            $scope.studentsData = [];
            $scope.assistance = [];
            $http.post('../../WebServices/Activities.asmx/getAssistanceList', {
                activityId: $scope.activityAssistanceCopy.CODIGOACTIVIDAD,
                levelId: code
            }).success(function (data, status, headers, config) {
                console.log("Cargar estudiantes... ", data);
                if (data.success)
                    $scope.studentsData = data.response;                  
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al eliminar la actividad... ", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al eliminar la actividad'}]);
            });
        };

        $scope.setAssistanceStudents = function(code) { 
            var existGroup = false;

            if ($scope.assistance != null && $scope.assistance.length > 0) {
                for (var i=0; i<$scope.assistance.length; i++) {
                    if ($scope.assistance[i] == code)
                        existGroup = true;
                }
            } 

            if (existGroup) {
                for (var i=0; i<$scope.assistance.length; i++) {
                    if ($scope.assistance[i] == code)
                        $scope.assistance.splice(i, code);
                }
            }                
            else
                $scope.assistance.push(code); 
        };

       








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
        
        
    }]);

    app.controller('ngDialogController', ['$scope', '$http', function($scope, $http) {
        $scope.saveEditedActivity = function () {
            if (!this.activityForm.$invalid) {
                var parentObject = this;

                $http.post('../../WebServices/Activities.asmx/saveActivityData', {
                    activityId: $scope.activityCopy.CODIGO,
                    activityName: $scope.activityCopy.NOMBRE.toUpperCase(),
                    activityDate: $scope.activityCopy.FECHA,
                    activityStatus: $scope.activityCopy.ESTADO,
                    activityObservation: $scope.activityCopy.OBSERVACION.toUpperCase(),
                    generalActivityId: $scope.activityCopy.CODIGOACTIVIDAD,
                    userId: $scope.activityCopy.CODIGOUSUARIO,
                    groupLevelActivity: $scope.groupLevelActivity
                }).success(function (data, status, headers, config) {
                    console.log("Editar actividad: ", data);
                    if (data.success) {
                        $scope.updateElementArray($scope.gridOptions.data, $scope.activityCopy.CODIGOACTIVIDAD, $scope.activityCopy.CODIGO, 
                            $scope.activityCopy.NOMBRE.toUpperCase(), Date.parse($scope.activityCopy.FECHA), 
                            $scope.activityCopy.ESTADO, $scope.activityCopy.OBSERVACION.toUpperCase(), $scope.activityCopy.CODIGOUSUARIO);
                        parentObject.closeThisDialog();
                    } 

                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al editar la actividad...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al actualizar la actividad'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };

        $scope.addNewActivityDB = function () {
            if (!this.newActivityForm.$invalid) {
                var newParentObject = this;

                $http.post('../../WebServices/Activities.asmx/addNewActivity', {                
                    activityName: $scope.activityCopy.NOMBRE,
                    activityDate: $scope.activityCopy.FECHA,
                    activityStatus: $scope.activityCopy.ESTADO,
                    activityObservation: $scope.activityCopy.OBSERVACION,
                    generalActivityId: $scope.activityCopy.CODIGOACTIVIDAD,
                    userId: $scope.activityCopy.CODIGOUSUARIO,
                    groupLevelActivity: $scope.groupLevelActivity
                }).success(function (data, status, headers, config) {
                    console.log("Agregar actividad: ", data);
                    if (data.success) {
                        $scope.addElementArray($scope.gridOptions.data, data.response, Date.parse($scope.activityCopy.FECHA), 
                            $scope.activityCopy.CODIGOACTIVIDAD, $scope.activityCopy.ESTADO);
                        newParentObject.closeThisDialog();
                    }

                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al agregar el rol...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar el rol'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };

        $scope.saveAssistanceDB = function () {
            if (!this.assistanceForm.$invalid) {
                var newParentObject = this;

                $http.post('../../WebServices/Activities.asmx/saveAssistanceData', {                
                    assistance: $scope.assistance
                }).success(function (data, status, headers, config) {
                    console.log("Agregar actividad: ", data);
                    if (data.success) {
                        
                    }

                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al agregar el rol...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar el rol'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };
    }]);
})();