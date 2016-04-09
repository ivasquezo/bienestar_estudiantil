(function () {
    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages']);

    app.controller('ActivitiesController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', {life: 5000});

        // for procesing message
        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = false;
        $scope.delay = 2;
        $scope.minDuration = 2;

        $scope.activityCopy = {
            CODIGO: ''
        };

        $scope.convertDate = function(arrayActivity) {
            for (var i = 0; i < arrayActivity.length; i++) {
                if (arrayActivity[i].FECHA != null)
                    arrayActivity[i].FECHA = arrayActivity[i].FECHA.substring(6, arrayActivity[i].FECHA.length-2);
            };
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

        $scope.chargeGeneralActivities = function () {
            $http.post('../../WebServices/Activities.asmx/getAllGeneralActivitiesWithActivity', {
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

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            enableColumnMenus: false,
            columnDefs: [
              {name:'C\u00F3digo', field: 'CODIGO', width: 80},
              {name:'Actividad general', field: 'NOMBREACTIVIDAD'},
              {name:'Actividad', field: 'NOMBRE'},
              {name:'Fecha', field: 'FECHA', type: 'date', cellFilter: 'date:\'dd/MM/yyyy\'', width: 100, enableFiltering: false},
              {name:'Estado', field: 'NOMBREESTADO', width: 90},
              {name:'Acci\u00F3n', field: 'CODIGO', cellTemplate: 'actionsActivities.html', width: 190, enableFiltering: false, enableSorting: false}
            ]
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

        $scope.getAllResponsables = function () {     
            $http.post('../../WebServices/Activities.asmx/getAllResponsables'
            ).success(function (data, status, headers, config) {
                console.log("Docentes... ", data);
                if (data.success) {
                    $scope.allResponsables = [];

                    for (var i = 0; i < data.response.length; i++)
                        $scope.allResponsables.push({value: data.response[i].CODIGO, name:data.response[i].NOMBRECOMPLETO});
                } else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar los docentes...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los docentes existentes'}]);
            });
        };

        $scope.chargeGeneralActivities();
        $scope.getGeneralActivities();
        $scope.getAllResponsables();

        $scope.getElementArray = function(arrayActividad, activityCode) {
            for (var i=0; i<arrayActividad.length; i++) {
                if (arrayActividad[i].CODIGO == activityCode) 
                    return arrayActividad[i];
            }
            return null;
        };

        $scope.toDate = function(dateTime) {
            var mEpoch = parseInt(dateTime); 
            var dDate = new Date();

            if(mEpoch<10000000000) mEpoch *= 1000;
        
            dDate.setTime(mEpoch)
            return dDate;
        }

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

        this.getLevelActivity = function (code) {
            $scope.view = 'faculty';
            $scope.activityCopy.CODIGO = code;

            $scope.saveAllGroups();
            
            $scope.selectedFaculties = [];
            $scope.selectedSchools = [];
            $scope.selectedCareers = [];
            $scope.selectedModalities = [];
            $scope.selectedLevels = [];

            $scope.selectedExistFaculties = [];
            $scope.selectedExistSchools = [];
            $scope.selectedExistCareers = [];
            $scope.selectedExistLevels = [];

            $scope.originCareers = [];
            $scope.originModalities = [];
            $scope.originLevels = [];

            $scope.getGroupsByActivity();

            $scope.getAllFaculty();
            $scope.getAllSchools();
            $scope.getAllCareers();
            $scope.getAllModalities();
            $scope.getAllLevels();

            ngDialog.open({
                template: 'getLevel.html',
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

        $scope.saveAllGroups = function () {
            $http.post('../../WebServices/Activities.asmx/saveAllGroups'
            ).success(function (data, status, headers, config) {
                console.log("Guardar grupos nuevos... ", data);
            }).error(function (data, status, headers, config) {
                console.log("Error al guardar grupos nuevos...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al guardar los grupos'}]);
            });
        };

        $scope.getGroupsByActivity = function () {
            $http.post('../../WebServices/Activities.asmx/getGroupActivityByActivity', {
                activityId: $scope.activityCopy.CODIGO
            }).success(function (data, status, headers, config) {
                console.log("Obtener grupos de actividad...", data);
                var existe = false;
                if (data.success) {
                    for (var i = 0; i < data.response.length; i++) {
                        if ($scope.selectedCareers.length > 0) {
                            existe = false;
                            for (var c = 0; c < $scope.selectedCareers.length; c++) {
                                if ($scope.selectedCareers[c] == data.response[i].CODIGOCARRERA)
                                    existe = true;
                            }
                            if (!existe) {
                                $scope.selectedCareers.push(data.response[i].CODIGOCARRERA);
                                $scope.originCareers.push(data.response[i].CODIGOCARRERA);
                            }
                        } else {
                            $scope.selectedCareers.push(data.response[i].CODIGOCARRERA);
                            $scope.originCareers.push(data.response[i].CODIGOCARRERA);
                        }

                        if ($scope.selectedModalities.length > 0) {
                            existe = false;
                            for (var m = 0; m < $scope.selectedModalities.length; m++) {
                                if ($scope.selectedModalities[m] == data.response[i].CODIGOMODALIDAD)
                                    existe = true;
                            }
                            if (!existe) {
                                $scope.selectedModalities.push(data.response[i].CODIGOMODALIDAD);
                                $scope.originModalities.push(data.response[i].CODIGOMODALIDAD);
                            }
                        } else {
                            $scope.selectedModalities.push(data.response[i].CODIGOMODALIDAD);
                            $scope.originModalities.push(data.response[i].CODIGOMODALIDAD);
                        }

                        if ($scope.selectedLevels.length > 0) {
                            existe = false;
                            for (var l = 0; l < $scope.selectedLevels.length; l++) {
                                if ($scope.selectedLevels[l] == data.response[i].CODIGONIVEL)
                                    existe = true;
                            }
                            if (!existe) {
                                $scope.selectedLevels.push(data.response[i].CODIGONIVEL);
                                $scope.originLevels.push(data.response[i].CODIGONIVEL);
                            }
                        } else {
                            $scope.selectedLevels.push(data.response[i].CODIGONIVEL);
                            $scope.originLevels.push(data.response[i].CODIGONIVEL);
                        }
                    }
                }
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al obtener los grupos de la actividad...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los grupos de la actividad'}]);
            });
        };

        $scope.getAllFaculty = function () {     
            $http.post('../../WebServices/Activities.asmx/getAllFaculties'
            ).success(function (data, status, headers, config) {
                console.log("Facultades... ", data);
                if (data.success)
                    $scope.allFaculties = data.response;
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar facultades...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las facultades'}]);
            });
        };

        $scope.getAllSchools = function () {
            $http.post('../../WebServices/Activities.asmx/getAllSchools', {
                faculties: $scope.selectedFaculties
            }).success(function (data, status, headers, config) {
                console.log("Escuelas... ", data);
                if (data.success)
                    $scope.allSchools = data.response;
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar escuelas...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las escuelas'}]);
            });
        };

        $scope.getAllCareers = function () {     
            $http.post('../../WebServices/Activities.asmx/getAllCareers', {
                schools: $scope.selectedSchools
            }).success(function (data, status, headers, config) {
                console.log("Carreras... ", data);
                if (data.success)
                    $scope.allCareers = data.response;
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar carreras...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las carreras'}]);
            });
        };

        $scope.getAllModalities = function () {     
            $http.post('../../WebServices/Activities.asmx/getAllModalities'
            ).success(function (data, status, headers, config) {
                console.log("Modalidades... ", data);
                if (data.success)
                    $scope.allModalities = data.response;
                else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar modalidades...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener las modalidades'}]);
            });
        };

        $scope.getAllLevels = function () {     
            $http.post('../../WebServices/Activities.asmx/getAllLevels', {
                modalities: $scope.selectedModalities,
                carees: $scope.selectedCareers
            }).success(function (data, status, headers, config) {
                console.log("Niveles... ", data);
                if (data.success) {
                    $scope.allLevels = data.response;
                } else
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar niveles...", data);
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al obtener los niveles'}]);
            });
        };  

        $scope.cambiarVista = function(viewValue) {
            if (viewValue == "faculty") 
                $scope.getAllFaculty();

            if (viewValue == "school") {
                $scope.validateSchoolSelected();
                $scope.selectedExistSchools = [];
                $scope.getAllSchools();
            }

            if (viewValue == "career") {
                $scope.validateSchoolSelected();
                $scope.validateCareerSelected();
                $scope.selectedExistCareers = [];
                $scope.getAllCareers();
            }

            if (viewValue == "modality") {
                $scope.getAllModalities();
            }

            if (viewValue == "level") {
                $scope.validateLevelSelected();
                $scope.selectedExistLevels = [];
                $scope.getAllLevels();
            }

            $scope.view = viewValue;
        };

        $scope.validateSchoolSelected = function () {
            var exist = false;

            for (var i = 0; i < $scope.selectedSchools.length; i++) {
                for (var j = 0; j < $scope.selectedExistSchools.length; j++) {
                    if ($scope.selectedSchools[i] == $scope.selectedExistSchools[j]) {
                        exist = true;
                    }
                }
                if (!exist) {
                    $scope.selectedSchools.splice(i, 1);
                }
            };
        }

        $scope.validateCareerSelected = function () {
            var exist = false;

            for (var i = 0; i < $scope.selectedCareers.length; i++) {
                for (var j = 0; j < $scope.selectedExistCareers.length; j++) {
                    if ($scope.selectedCareers[i] == $scope.selectedExistCareers[j]) {
                        exist = true;
                    }
                }
                if (!exist) {
                    $scope.selectedCareers.splice(i, 1);
                }
            };
        }

        $scope.validateLevelSelected = function () {
            var exist = false;

            for (var i = 0; i < $scope.selectedLevels.length; i++) {
                for (var j = 0; j < $scope.selectedExistLevels.length; j++) {
                    if ($scope.selectedLevels[i] == $scope.selectedExistLevels[j]) {
                        exist = true;
                    }
                }
                if (!exist) {
                    $scope.selectedLevels.splice(i, 1);
                }
            };
        };

         $scope.existFacultyData = function (code) {
            if ($scope.selectedFaculties.length > 0) {
                for (var i = 0; i < $scope.selectedFaculties.length; i++) {
                    if ($scope.selectedFaculties[i] == code) return true; 
                }
            }
            return false;
        };

        $scope.existSchoolData = function (code) {
            if ($scope.selectedSchools.length > 0) {
                for (var i = 0; i < $scope.selectedSchools.length; i++) {
                    if ($scope.selectedSchools[i] == code) { 
                        $scope.selectedExistSchools.push(code);
                        return true; 
                    }
                }
            }
            return false;
        };

        $scope.existCareerData = function (code) {
            if ($scope.selectedCareers.length > 0) {
                for (var i = 0; i < $scope.selectedCareers.length; i++) {
                    if ($scope.selectedCareers[i] == code) { 
                        $scope.selectedExistCareers.push(code);
                        return true; 
                    }
                }
            }
            return false;
        };

        $scope.existModalityData = function (code) {
            if ($scope.selectedModalities.length > 0) {
                for (var i = 0; i < $scope.selectedModalities.length; i++) {
                    if ($scope.selectedModalities[i] == code) return true; 
                }
            }
            return false;
        };

        $scope.existLevelData = function (code) {
            if ($scope.selectedLevels.length > 0) {
                for (var i = 0; i < $scope.selectedLevels.length; i++) {
                    if ($scope.selectedLevels[i] == code) { 
                        $scope.selectedExistLevels.push(code);
                        return true; 
                    }
                }
            }
            return false;
        };

        $scope.setSelectedFaculties = function(id) {
            $scope.selectObjects($scope.selectedFaculties, id);
            $scope.getAllSchools();
        };

        $scope.setSelectedSchools = function(id) {
            $scope.selectObjects($scope.selectedSchools, id);    
            $scope.getAllCareers();       
        };

        $scope.setSelectedCareers = function(id) {
            $scope.selectObjects($scope.selectedCareers, id);
            $scope.getAllModalities();
            $scope.getAllLevels();
        };

        $scope.setSelectedModalities = function(id) {  
            $scope.selectObjects($scope.selectedModalities, id);
            $scope.getAllLevels();         
        };

        $scope.setSelectedLevels = function(id) {  
            $scope.selectObjects($scope.selectedLevels, id);           
        };

        $scope.selectObjects = function(listSelected, id) {
            var index = null;
            if (listSelected == null || listSelected.length == 0)
                listSelected.push(id);
            else {
                for (var i = 0; i < listSelected.length; i++) {
                    if (listSelected[i] == id)
                        index = i;
                };

                if (index != null)
                    listSelected.splice(index, 1);
                else
                    listSelected.push(id);
            }     
        };








        

        $scope.getGroupLevel = function (code, successFunction) {
            if (code > 0) {
                $http.post('../../WebServices/Activities.asmx/getGroupLevelByActivityId', {
                    activityId: code
                }).success(function (data, status, headers, config) {
                    console.log("Niveles de la actividad... ", data);
                    $scope.groupActivity = data.response;

                    $scope.allLevelAssistance = [];

                    for (var i = 0; i < $scope.groupActivity.length; i++) {
                        if ($scope.groupActivity[i].ESTADO == true)
                            $scope.allLevelAssistance.push({value: $scope.groupActivity[i].CODIGOGRUPO, name:$scope.groupActivity[i].NIVEL + " " + $scope.groupActivity[i].PARALELO + " - " + $scope.groupActivity[i].MODALIDAD});
                    }

                    if (typeof successFunction === "function")
                        successFunction();
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

        

        this.getAssistance = function (code) {
            $scope.activityAssistanceCopy = {
                CODIGO: ''
            };
            $scope.allLevelAssistance = [];

            $scope.activityAssistanceCopy.CODIGOACTIVIDAD = code;

            // Llena los niveles
            $scope.getGroupLevel(code, function(){
                $scope.assistance = [];
                $scope.studentsData = [];
                
                if ($scope.allLevelAssistance.length > 0) {
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
                }
                else
                    $('#messages').puigrowl('show', [{severity: 'info', summary: 'Información', detail: 'No se han encontrado niveles registrados en la actividad'}]);
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

        this.getAttachedActivity = function (code) {
            
            ngDialog.open({
                template: 'attachedActivity.html',
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
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Editar', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };

        $scope.addNewActivityDB = function () {
            if (!this.newActivityForm.$invalid) {
                var newParentObject = this;

                $http.post('../../WebServices/Activities.asmx/addNewActivity', {                
                    activityName: $scope.activityCopy.NOMBRE.toUpperCase(),
                    activityDate: $scope.activityCopy.FECHA,
                    activityStatus: $scope.activityCopy.ESTADO,
                    activityObservation: $scope.activityCopy.OBSERVACION.toUpperCase(),
                    generalActivityId: $scope.activityCopy.CODIGOACTIVIDAD,
                    userId: $scope.activityCopy.CODIGOUSUARIO
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
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar la actividad'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };

        $scope.saveGroupActivity = function () {
            if ($scope.selectedCareers.length == 0 && $scope.selectedModalities.length == 0 && $scope.selectedLevels.length == 0) {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Editar', detail: 'Debe seleccionar por lo menos una carrera, o un módulo o un nivel'}]);
            } else {
                var parentObject = this;

                console.log("carrera: ", $scope.selectedCareers);
                console.log("modalidad: ", $scope.selectedModalities);
                console.log("nivel: ", $scope.selectedLevels);
                if (($scope.selectedCareers.length > 0 || $scope.selectedModalities.length > 0) && $scope.selectedLevels.length == 0) {
                    for (var i = 0; i < $scope.allLevels.length; i++)
                        $scope.selectedLevels.push($scope.allLevels[i].NVLCODIGOI)
                }
                else if (($scope.selectedCareers.length == 0 && $scope.selectedModalities.length == 0) && $scope.selectedLevels.length > 0) {
                    for (var i = 0; i < $scope.allCareers.length; i++)
                        $scope.selectedCareers.push($scope.allCareers[i].CRRCODIGOI);
                }

                $http.post('../../WebServices/Activities.asmx/saveGroupActivity', {
                    careersV: $scope.selectedCareers,
                    modalitiesV: $scope.selectedModalities,
                    levelsV: $scope.selectedLevels,
                    activityId: $scope.activityCopy.CODIGO,
                    originCareersV: $scope.originCareers,
                    originModalitiesV: $scope.originModalities,
                    originLevelsV: $scope.originLevels
                }).success(function (data, status, headers, config) {
                    console.log("Editar grupo actividad: ", data);
                    if (data.success)
                        parentObject.closeThisDialog();

                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al editar grupo actividad...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al actualizar el grupo de la actividad'}]);
                });
            }
        };

        $scope.saveAssistanceDB = function () {
            if (!this.assistanceForm.$invalid) {
                var newParentObject = this;

                $http.post('../../WebServices/Activities.asmx/saveAssistanceData', {                
                    assistance: $scope.assistance
                }).success(function (data, status, headers, config) {
                    console.log("Agregar actividad: ", data);
                    if (!data.success)
                        $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al agregar el rol...", data);
                    $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar el rol'}]);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };

        $scope.saveAttachedDB = function () {
            if (!this.attachedForm.$invalid) {
                var formElement = document.getElementById('attachedForm');
                var formData = new FormData(formElement);

                $http.post('../../WebServices/Activities.asmx/addUploadedFileDataBase', formData, {
                    withCredentials: true,
                    headers: {'Content-Type': undefined },
                    transformRequest: angular.identity
                }).success(function (data, status, headers, config) {
                    console.log("Adjuntos", data);
                    if (data.success) {
                        $scope.activityAttached = data.response;
                        $scope.activityId = 7;
                        $http.post('../../WebServices/Activities.asmx/saveActivityAttached', {                
                            activityAttached: $scope.activityAttached,
                            activityId: $scope.activityId
                        }).success(function (data, status, headers, config) {
                            console.log("Agregar adjunto: ", data);
                            $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                        }).error(function (data, status, headers, config) {
                            console.log("Error al agregar adjunto...", data);
                            $('#messages').puigrowl('show', [{severity: 'error', summary: 'Error', detail: 'Error al agregar el rol'}]);
                        });
                    }
                    $('#messages').puigrowl('show', [{severity: data.severity, summary: data.summary, detail: data.message}]);
                }).error(function (data, status, headers, config) {
                    console.log("error al cargar los tipos...", data);
                });
            } else {
                $('#messages').puigrowl('show', [{severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos'}]);
            }
        };
    }]);

    app.directive('validActivityName', ['$http', function($http) {
        return {
            require: 'ngModel',

            link: function(scope, element, attr, ctrl) {
                function customValidator(ngModelValue) {
                    if (ngModelValue != null && ngModelValue != scope.activityCopy.NOMBRE) {

                        ctrl.$setValidity('activityNameChecking', false);

                        scope.promise = $http.post('../../WebServices/Activities.asmx/countActivityWithName', {
                            activityName: ngModelValue
                        }).success(function (data, status, headers, config) {
                            if (data.cantidad == 0) {
                                 ctrl.$setValidity('activityNameExist', true);
                                ctrl.$setValidity('activityNameValidator', true);
                                ctrl.$setValidity('activityNameChecking', true);
                               
                            } else {                            
                                ctrl.$setValidity('activityNameExist', false);
                                ctrl.$setValidity('activityNameValidator', true);
                            }
                        }).error(function (data, status, headers, config) {
                            console.log("Error al traer actividad", data);
                            ctrl.$setValidity('activityNameValidator', false);
                        });
                    } else {
                        ctrl.$setValidity('activityNameExist', true);
                        ctrl.$setValidity('activityNameValidator', true);
                        ctrl.$setValidity('activityNameChecking', true);
                        
                    }
                    return ngModelValue;
                }

                ctrl.$parsers.push(customValidator);
            }
        };
    }]);
})();