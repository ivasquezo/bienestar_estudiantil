(function () {
    var app = angular.module('BienestarApp', ['ui.grid', 'ngDialog', 'ngMessages', 'cgBusy', 'ui.bootstrap']);

    app.controller('ActivitiesController', ['$scope', '$http', 'ngDialog', '$controller', function ($scope, $http, ngDialog, $controller) {
        $('#messages').puigrowl();
        $('#messages').puigrowl('option', { life: 5000 });

        // session listener
        document.onclick = function () {
            $http.get((appContext != undefined ? appContext : "") + '/WebServices/Users.asmx/checkSession')
                .success(function (data, status, headers, config) {
                    if (!data.success) {
                        document.location.reload();
                    }
                }).error(function (data, status, headers, config) {
                    console.log("Error checkSession", data);
                });
        };

        // for procesing message
        $scope.promise = null;
        $scope.message = 'Procesando...';
        $scope.backdrop = true;

        $scope.date = {
            dateFrom: new Date(),
            dateTo: new Date()
        };

        $scope.ESTADOS = [
            { name: 'Inactivo', value: 0 },
            { name: 'En proceso', value: 1 },
            { name: 'Procesado', value: 2 },
            { name: 'Finalizado', value: 3 }
        ];

        $scope.activityCopy = {
            CODIGO: ''
        };

        $scope.presentEditGeneralActivity = false;

        $scope.convertDate = function (arrayActivity) {
            for (var i = 0; i < arrayActivity.length; i++) {
                if (arrayActivity[i].FECHA != null)
                    arrayActivity[i].FECHA = arrayActivity[i].FECHA.substring(6, arrayActivity[i].FECHA.length - 2);
            };
        };

        $scope.cargarNombreEstado = function (statusId) {
            return $scope.ESTADOS[statusId].name;
        };

        $scope.cargarEstadoActividades = function (actividades) {
            for (var i = 0; i < actividades.length; i++)
                actividades[i].NOMBREESTADO = $scope.cargarNombreEstado(actividades[i].ESTADO);
        };

        $scope.chargeGeneralActivities = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getAllGeneralActivitiesWithActivity', {
            }).success(function (data, status, headers, config) {
                // console.log("Cargar actividades... ", data);
                if (data.success) {
                    $scope.convertDate(data.response);
                    $scope.cargarEstadoActividades(data.response);
                    $scope.gridOptions.data = data.response;
                } else
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar las actividades... ", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener las actividades' }]);
            });
        };

        this.getIsTeacher = function () {
            for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                if ($scope.gridOptions.data[i].ISTEACHER)
                    return true;
                return false;
            }
        };

        $scope.getReportActivities = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getActivitiesReport', {
            }).success(function (data, status, headers, config) {
                console.log("Cargar actividades... ", data);
                if (data.success) {
                    $scope.convertDate(data.response);
                    $scope.reportActivitiesData = data.response;
                } else
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar reportes de actividades... ", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener reportes de actividades' }]);
            });
        };

        $scope.gridOptions = {
            enableSorting: true,
            enableFiltering: true,
            enableColumnMenus: false,
            columnDefs: [
                { name: 'C\u00F3digo', field: 'CODIGO', width: 85, type: 'number' },
                { name: 'Actividad general', field: 'NOMBREACTIVIDAD', width: 150 },
                { name: 'Actividad', field: 'NOMBRE' },
                {
                    name: 'Fecha', field: 'FECHA', type: 'date', cellFilter: 'date:\'dd/MM/yyyy\'', width: 100, enableFiltering: false,
                    sortingAlgorithm: function (dateA, dateB) {
                        var a = parseInt(dateA);
                        var b = parseInt(dateB);
                        if (a < b) {
                            return -1;
                        } else if (a > b) {
                            return 1;
                        } else {
                            return 0;
                        }
                    }
                },
                { name: 'Estado', field: 'NOMBREESTADO', width: 90 },
                { name: 'Acci\u00F3n', field: 'CODIGO', cellTemplate: 'actionsActivities.html', width: 230, enableFiltering: false, enableSorting: false }
            ]
        };

        $scope.getGeneralActivities = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getAllGeneralActivity'
            ).success(function (data, status, headers, config) {
                // console.log("Actividades generales existentes... ", data);
                $scope.allGeneralActivities = [];

                for (var i = 0; i < data.response.length; i++)
                    $scope.allGeneralActivities.push({ value: data.response[i].CODIGO, name: data.response[i].NOMBRE });
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar las actividades generales...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener las actividades generales existentes' }]);
            });
        };

        $scope.getAllResponsables = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getAllResponsables'
            ).success(function (data, status, headers, config) {
                // console.log("Docentes... ", data);
                if (data.success) {
                    $scope.allResponsables = [];

                    for (var i = 0; i < data.response.length; i++)
                        $scope.allResponsables.push({ value: data.response[i].CODIGO, name: data.response[i].NOMBRECOMPLETO });
                } else
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar los docentes...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener los docentes existentes' }]);
            });
        };

        $scope.chargeGeneralActivities();
        $scope.getGeneralActivities();
        $scope.getAllResponsables();

        $scope.getElementArray = function (arrayActividad, activityCode) {
            for (var i = 0; i < arrayActividad.length; i++) {
                if (arrayActividad[i].CODIGO == activityCode)
                    return arrayActividad[i];
            }
            return null;
        };

        $scope.toDate = function (dateTime) {
            var mEpoch = parseInt(dateTime);
            var dDate = new Date();

            if (mEpoch < 10000000000) mEpoch *= 1000;

            dDate.setTime(mEpoch)
            return dDate;
        }

        this.editActivity = function (code) {
            $scope.presentEditGeneralActivity = false;
            $scope.activityEdit = angular.copy($scope.getElementArray($scope.gridOptions.data, code));
            $scope.getGeneralActivities();

            $scope.activityCopy.CODIGOACTIVIDAD = $scope.activityEdit.CODIGOACTIVIDAD;
            $scope.activityCopy.NOMBREACTIVIDAD = $scope.activityEdit.NOMBREACTIVIDAD;
            $scope.activityCopy.CODIGO = $scope.activityEdit.CODIGO;
            $scope.activityCopy.NOMBRE = $scope.activityEdit.NOMBRE;
            $scope.activityCopy.FECHA = $scope.toDate($scope.activityEdit.FECHA);
            $scope.activityCopy.ESTADO = $scope.activityEdit.ESTADO;
            $scope.activityCopy.OBSERVACION = $scope.activityEdit.OBSERVACION;
            $scope.activityCopy.CODIGOUSUARIO = $scope.activityEdit.CODIGOUSUARIO;
            $scope.activityCopy.NOMBREESTADO = $scope.cargarNombreEstado($scope.activityEdit.ESTADO);
            $scope.sendMail = false;

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

        $scope.updateElementArray = function (arrayActivity, generalActivityId, activityId, activityName, activityDate, activityStatus, observation, responsableId, generalActivityName) {
            var nameActivity = '';

            if (generalActivityId == 0) {
                nameActivity = generalActivityName;
            } else {
                for (var i = 0; i < $scope.allGeneralActivities.length; i++)
                    if ($scope.allGeneralActivities[i].value == generalActivityId)
                        nameActivity = $scope.allGeneralActivities[i].name;
            }

            for (var i = 0; i < arrayActivity.length; i++) {
                if (arrayActivity[i].CODIGO == activityId) {
                    arrayActivity[i].CODIGOACTIVIDAD = generalActivityId;
                    arrayActivity[i].NOMBREACTIVIDAD = nameActivity;
                    arrayActivity[i].NOMBRE = activityName;
                    arrayActivity[i].FECHA = activityDate;
                    arrayActivity[i].ESTADO = activityStatus;
                    arrayActivity[i].NOMBREESTADO = $scope.cargarNombreEstado(activityStatus);
                    arrayActivity[i].OBSERVACION = observation;
                    arrayActivity[i].CODIGOUSUARIO = responsableId;
                }
            }
        };

        $scope.addNewActivityDialog = function () {
            $scope.presentEditGeneralActivity = false;
            $scope.getGeneralActivities();
            $scope.activityCopy = {
                OBSERVACION: ''
            };
            $scope.sendMail = false;

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

        $scope.addElementArray = function (arrayActivity, newActivity, activityDate, generalActivityId, activityStatus, activityName) {
            var nameActivity = '';

            if (generalActivityId == 0) {
                nameActivity = activityName;
            } else {
                for (var i = 0; i < $scope.allGeneralActivities.length; i++)
                    if ($scope.allGeneralActivities[i].value == generalActivityId)
                        nameActivity = $scope.allGeneralActivities[i].name;
            }
            newActivity.FECHA = activityDate;
            newActivity.CODIGOACTIVIDAD = generalActivityId;
            newActivity.NOMBREACTIVIDAD = nameActivity;
            newActivity.NOMBREESTADO = $scope.cargarNombreEstado(activityStatus);

            arrayActivity.push(newActivity);
        };

        this.getLevelActivity = function (code) {
            $scope.view = 'modality';
            $scope.activityCopy.CODIGO = code;

            $scope.saveAllGroups();

            $scope.allModalities = [];
            $scope.allCareers = [];
            $scope.allLevels = [];
            $scope.allGroups = [];

            $scope.copyAllCareers = [];
            $scope.copyAllGroups = [];

            $scope.selectedCareers = [];
            $scope.selectedModalities = [];
            $scope.selectedLevels = [];
            $scope.selectedGroups = [];

            $scope.originCareers = [];
            $scope.originModalities = [];
            $scope.originLevels = [];
            $scope.originGroups = [];

            $scope.getGroupsByActivity();

            $scope.getAllCareers();
            $scope.getAllModalities();
            $scope.getAllLevels();
            $scope.getAllGroups();

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
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/saveAllGroups'
            ).success(function (data, status, headers, config) {
                console.log("Guardar grupos nuevos... ", data);
            }).error(function (data, status, headers, config) {
                console.log("Error al guardar grupos nuevos...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al guardar los grupos' }]);
            });
        };

        $scope.getGroupsByActivity = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getGroupActivityByActivity', {
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
                            if (existe == false) {
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
                            if (existe == false) {
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
                            if (existe == false) {
                                $scope.selectedLevels.push(data.response[i].CODIGONIVEL);
                                $scope.originLevels.push(data.response[i].CODIGONIVEL);
                            }
                        } else {
                            $scope.selectedLevels.push(data.response[i].CODIGONIVEL);
                            $scope.originLevels.push(data.response[i].CODIGONIVEL);
                        }

                        if ($scope.selectedGroups.length > 0) {
                            existe = false;
                            for (var g = 0; g < $scope.selectedGroups.length; g++) {
                                if ($scope.selectedGroups[g] == data.response[i].PARALELO)
                                    existe = true;
                            }
                            if (existe == false) {
                                $scope.selectedGroups.push(data.response[i].PARALELO);
                            }
                        } else {
                            $scope.selectedGroups.push(data.response[i].CODIGONIVEL);
                        }
                    }
                }
                else
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al obtener los grupos de la actividad...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener los grupos de la actividad' }]);
            });
        };

        $scope.getAllModalities = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getAllModalities'
            ).success(function (data, status, headers, config) {
                console.log("Modalidades... ", data);
                if (data.success)
                    $scope.allModalities = data.response;
                else
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar modalidades...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener las modalidades' }]);
            });
        };

        $scope.getAllCareers = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getAllCareers', {
                modalities: $scope.selectedModalities
            }).success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.copyAllCareers = [];
                    $scope.allCareers = data.response;
                    for (var i = 0; i < $scope.allCareers.length; i++) {
                        if ($scope.copyAllCareers.length > 0) {
                            var existe = false;
                            for (var j = 0; j < $scope.copyAllCareers.length; j++) {
                                if ($scope.copyAllCareers[j].CRRCODIGOI == $scope.allCareers[i].CRRCODIGOI) {
                                    existe = true;
                                }
                            }
                            if (existe == false) {
                                $scope.copyAllCareers.push($scope.allCareers[i]);
                            }
                        } else
                            $scope.copyAllCareers.push($scope.allCareers[i]);
                    }
                }
                else
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar carreras...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener las carreras' }]);
            });
        };

        $scope.getAllLevels = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getAllLevels', {
                modalities: $scope.selectedModalities,
                carees: $scope.selectedCareers
            }).success(function (data, status, headers, config) {
                console.log("Niveles... ", data);
                if (data.success) {
                    $scope.allLevels = data.response;
                } else
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar niveles...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener los niveles' }]);
            });
        };

        $scope.getAllGroups = function () {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getAllGroups', {
                modalities: $scope.selectedModalities,
                carees: $scope.selectedCareers,
                levels: $scope.selectedLevels
            }).success(function (data, status, headers, config) {
                console.log("Paralelos... ", data);
                if (data.success) {
                    $scope.copyAllGroups = [];
                    $scope.allGroups = data.response;
                    for (var i = 0; i < $scope.allGroups.length; i++) {
                        if ($scope.copyAllGroups.length > 0) {
                            var existe = false;
                            for (var j = 0; j < $scope.copyAllGroups.length; j++) {
                                if ($scope.copyAllGroups[j].PARALELO == $scope.allGroups[i].PARALELO) {
                                    existe = true;
                                }
                            }
                            if (existe == false) {
                                $scope.copyAllGroups.push($scope.allGroups[i]);
                            }
                        } else
                            $scope.copyAllGroups.push($scope.allGroups[i]);
                    }
                } else
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar niveles...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener los niveles' }]);
            });
        };

        $scope.cambiarVista = function (viewValue) {
            if (viewValue == "levelCareer") {
                if ($scope.selectedModalities.length > 0) {
                    $scope.getAllCareers();
                    $scope.validateCareerSelected();
                    $scope.view = "career";
                }
            }
            if (viewValue == "careerLevel") {
                if ($scope.selectedCareers.length > 0) {
                    $scope.getAllLevels();
                    $scope.validateLevelSelected();
                    $scope.view = "level";
                }
            }
            if (viewValue == "careerModality") {
                if ($scope.selectedCareers.length > 0) {
                    $scope.getAllModalities();
                    $scope.view = "modality";
                }
            }
            if (viewValue == "levelCareer") {
                if ($scope.selectedLevels.length > 0) {
                    $scope.getAllCareers();
                    $scope.validateCareerSelected();
                    $scope.view = "career";
                }
            }
            if (viewValue == "levelGroup") {
                if ($scope.selectedLevels.length > 0) {
                    $scope.getAllGroups();
                    $scope.validateGroupSelected();
                    $scope.view = "group";
                }
            }
        };

        $scope.validateCareerSelected = function () {
            var careerToErase = [];
            for (var i = 0; i < $scope.selectedCareers.length; i++) {
                for (var j = 0; j < $scope.copyAllCareers.length; j++) {
                    if ($scope.selectedCareers[i] == $scope.copyAllCareers[j].CRRCODIGOI) {
                        careerToErase.push($scope.selectedCareers[i]);
                    }
                }
            };
            $scope.selectedCareers = [];
            $scope.selectedCareers = careerToErase;
        }

        $scope.validateLevelSelected = function () {
            var levelToErase = [];
            for (var i = 0; i < $scope.selectedLevels.length; i++) {
                for (var j = 0; j < $scope.allLevels.length; j++) {
                    if ($scope.selectedLevels[i] == $scope.allLevels[j].NVLCODIGOI) {
                        levelToErase.push($scope.selectedLevels[i]);
                    }
                }
            };
            $scope.selectedLevels = [];
            $scope.selectedLevels = levelToErase;
        };

        $scope.validateGroupSelected = function () {
            var groupToErase = [];
            for (var i = 0; i < $scope.selectedGroups.length; i++) {
                for (var j = 0; j < $scope.copyAllGroups.length; j++) {
                    if ($scope.selectedGroups[i] == $scope.copyAllGroups[j].PARALELO) {
                        groupToErase.push($scope.selectedGroups[i]);
                    }
                }
            };
            $scope.selectedGroups = [];
            $scope.selectedGroups = groupToErase;
        };

        $scope.existAllCareerData = function () {
            if ($scope.selectedCareers.length > $scope.copyAllCareers.length)
                return true;
            return false;
        };

        $scope.existCareerData = function (code) {
            if ($scope.selectedCareers.length > 0) {
                for (var i = 0; i < $scope.selectedCareers.length; i++) {
                    if ($scope.selectedCareers[i] == code) {
                        return true;
                    }
                }
            }
            return false;
        };

        $scope.existAllModalities = function () {
            if ($scope.selectedModalities.length == $scope.allModalities.length)
                return true;
            return false;
        };

        $scope.existModalityData = function (code) {
            if ($scope.selectedModalities.length > 0) {
                for (var i = 0; i < $scope.selectedModalities.length; i++) {
                    if ($scope.selectedModalities[i] == code)
                        return true;
                }
            }
            return false;
        };

        $scope.existAllLevelData = function () {
            if ($scope.selectedLevels.length == $scope.allLevels.length)
                return true;
            return false;
        };

        $scope.existLevelData = function (code) {
            if ($scope.selectedLevels.length > 0) {
                for (var i = 0; i < $scope.selectedLevels.length; i++) {
                    if ($scope.selectedLevels[i] == code) {
                        return true;
                    }
                }
            }
            return false;
        };

        $scope.existAllGroupData = function () {
            if ($scope.selectedGroups.length == $scope.copyAllGroups.length)
                return true;
            return false;
        };

        $scope.existGroupData = function (code) {
            if ($scope.selectedGroups.length > 0) {
                for (var i = 0; i < $scope.selectedGroups.length; i++) {
                    if ($scope.selectedGroups[i] == code) {
                        return true;
                    }
                }
            }
            return false;
        };

        $scope.setSelectedAllCareers = function () {
            if ($scope.selectedCareers.length == $scope.copyAllCareers.length) {
                for (var i = 0; i < $scope.copyAllCareers.length; i++) {
                    $scope.selectedCareers = [];
                }
            } else {
                var existe = false;
                for (var i = 0; i < $scope.copyAllCareers.length; i++) {
                    existe = false;
                    if ($scope.selectedCareers.length == 0) {
                        $scope.selectedCareers.push($scope.copyAllCareers[i].CRRCODIGOI);
                    } else {
                        for (var j = 0; j < $scope.selectedCareers.length; j++) {
                            if ($scope.copyAllCareers[i].CRRCODIGOI == $scope.selectedCareers[j])
                                existe = true;
                        }
                        if (existe == false) {
                            $scope.selectedCareers.push($scope.copyAllCareers[i].CRRCODIGOI);
                        }
                    }
                }
            }
            $scope.getAllLevels();
        };

        $scope.setSelectedCareers = function (id) {
            $scope.selectObjects($scope.selectedCareers, id);
            $scope.getAllLevels();
        };

        $scope.setSelectedAllModalities = function () {
            if ($scope.selectedModalities.length == $scope.allModalities.length) {
                for (var i = 0; i < $scope.allModalities.length; i++) {
                    $scope.selectedModalities = [];
                }
            } else {
                var existe = false;
                for (var i = 0; i < $scope.allModalities.length; i++) {
                    existe = false;
                    if ($scope.selectedModalities.length == 0) {
                        $scope.selectedModalities.push($scope.allModalities[i].MDLCODIGOI);
                    } else {
                        for (var j = 0; j < $scope.selectedModalities.length; j++) {
                            if ($scope.allModalities[i].MDLCODIGOI == $scope.selectedModalities[j])
                                existe = true;
                        }
                        if (existe == false) {
                            $scope.selectedModalities.push($scope.allModalities[i].MDLCODIGOI);
                        }
                    }
                }
            }
            $scope.getAllCareers();
        };

        $scope.setSelectedModalities = function (id) {
            $scope.selectObjects($scope.selectedModalities, id);
            $scope.getAllCareers();
        };

        $scope.setSelectedAllLevels = function () {
            if ($scope.selectedLevels.length == $scope.allLevels.length) {
                for (var i = 0; i < $scope.allLevels.length; i++) {
                    $scope.selectedLevels = [];
                }
            } else {
                var existe = false;
                for (var i = 0; i < $scope.allLevels.length; i++) {
                    existe = false;
                    if ($scope.selectedLevels.length == 0) {
                        $scope.selectedLevels.push($scope.allLevels[i].NVLCODIGOI);
                    } else {
                        for (var j = 0; j < $scope.selectedLevels.length; j++) {
                            if ($scope.allLevels[i].NVLCODIGOI == $scope.selectedLevels[j])
                                existe = true;
                        }
                        if (existe == false) {
                            $scope.selectedLevels.push($scope.allLevels[i].NVLCODIGOI);
                        }
                    }
                }
            }
            $scope.getAllGroups();
        };

        $scope.setSelectedLevels = function (id) {
            $scope.selectObjects($scope.selectedLevels, id);
            $scope.getAllGroups();
        };

        $scope.setSelectedAllGroups = function () {
            if ($scope.selectedGroups.length == $scope.copyAllGroups.length) {
                for (var i = 0; i < $scope.copyAllGroups.length; i++) {
                    $scope.selectedGroups = [];
                }
            } else {
                var existe = false;
                for (var i = 0; i < $scope.copyAllGroups.length; i++) {
                    existe = false;
                    if ($scope.selectedGroups.length == 0) {
                        $scope.selectedGroups.push($scope.copyAllGroups[i].PARALELO);
                    } else {
                        for (var j = 0; j < $scope.selectedGroups.length; j++) {
                            if ($scope.copyAllGroups[i].PARALELO == $scope.selectedGroups[j])
                                existe = true;
                        }
                        if (existe == false) {
                            $scope.selectedGroups.push($scope.copyAllGroups[i].PARALELO);
                        }
                    }
                }
            }
        };

        $scope.setSelectedGroups = function (id) {
            $scope.selectObjects($scope.selectedGroups, id);
        };

        $scope.selectObjects = function (listSelected, id) {
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

        this.getAssistance = function (code) {
            $scope.allLevelAssistance = [];
            $scope.assistance = [];
            $scope.checkedAll = false;
            $scope.activityId = code;

            $scope.getAllStudents(code, function () {
                if ($scope.allLevelAssistance.length > 0) {
                    var countAssistance = 0;

                    for (var i = 0; i < $scope.allLevelAssistance.length; i++) {
                        if ($scope.allLevelAssistance[i].ASISTENCIA)
                            countAssistance++;
                    };

                    if ($scope.allLevelAssistance.length === countAssistance)
                        $scope.checkedAll = true;

                    ngDialog.open({
                        template: 'assistanceActivity.html',
                        className: 'ngdialog-theme-flat ngdialog-theme-custom',
                        closeByDocument: true,
                        closeByEscape: true,
                        scope: $scope,
                        controller: $controller('ngDialogController', {
                            $scope: $scope,
                            $http: $http,
                            code: code,
                        })
                    });
                }
                else
                    $('#messages').puigrowl('show', [{ severity: 'info', summary: 'Informaci&oacute;n', detail: 'No se han encontrado niveles registrados en la actividad' }]);
            });
        };

        $scope.notifyActivityStudents = function () {
            $scope.studentsMails = [];

            $scope.allLevelAssistance.filter(e => e.notify === true).forEach(function (element) {
                $scope.studentsMails.push({
                    codigo: element.CODIGO,
                    correo: element.CORREO,
                });
            }, this);

            if ($scope.studentsMails.length === 0) {
                return;
            }

            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/sendStudentsNotification', {
                studentsMails: $scope.studentsMails,
                activityId: $scope.activityId
            }).success(function (data, status, headers, config) {
                $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
                $scope.getAllStudents($scope.code);
                console.log($scope.promise);
            }).error(function (data, status, headers, config) {
                console.log("Error al enviar notificacion estudiantes...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener enviar la notificaci&oacute;n a los estudiantes' }]);
            });
        }

        $scope.getAllStudents = function (code, successFunction) {
            $scope.code = code;
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getStudentsAssistance', {
                activityId: code
            }).success(function (data, status, headers, config) {
                // console.log("Estudiantes de la actividad... ", data);
                if (data.success) {
                    $scope.allLevelAssistance = data.response;
                }

                if (typeof successFunction === "function")
                    successFunction();
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar niveles de actividad...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener los niveles de la actividad' }]);
            });
        };

        $scope.setAssistanceStudents = function (code) {
            $scope.selectObjects($scope.assistance, code);

            if ($scope.allLevelAssistance.length == $scope.assistance.length)
                $scope.checkedAll = true;
            else
                $scope.checkedAll = false;
        };

        $scope.missingNotify = () => {
            return $scope.allLevelAssistance.filter(e => e.notify === true).length === 0;
        }

        $scope.setNotify = function (code) { };

        $scope.setAllStudents = function () {
            if ($scope.checkedAll) {
                for (var i = 0; i < $scope.allLevelAssistance.length; i++) {
                    $scope.assistance.push($scope.allLevelAssistance[i].CODIGO);
                    $scope.allLevelAssistance[i].ASISTENCIA = false;
                }
                $scope.checkedAll = false;
            }
            else {
                for (var i = 0; i < $scope.allLevelAssistance.length; i++) {
                    if (!$scope.allLevelAssistance[i].ASISTENCIA) {
                        $scope.assistance.push($scope.allLevelAssistance[i].CODIGO);
                        $scope.allLevelAssistance[i].ASISTENCIA = true;
                    }
                }
                $scope.checkedAll = true;
            }
        };

        $scope.setAllNotify = function () {
            if ($scope.checkedAllNotify) {
                for (var i = 0; i < $scope.allLevelAssistance.length; i++) {
                    $scope.allLevelAssistance[i].notify = false;
                }
                $scope.checkedAllNotify = false;
            }
            else {
                for (var i = 0; i < $scope.allLevelAssistance.length; i++) {
                    $scope.allLevelAssistance[i].notify = true;
                }
                $scope.checkedAllNotify = true;
            }
        };

        this.getAttachedActivity = function (code) {
            $scope.activityCopy.CODIGO = code;
            $scope.allAttaches = [];

            $scope.getAllActivitiesAttach();

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

        $scope.getAllActivitiesAttach = function (code, successFunction) {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/getAttachByActivity', {
                activityId: $scope.activityCopy.CODIGO
            }).success(function (data, status, headers, config) {
                console.log("Adjuntos de la actividad... ", data);
                if (data.success)
                    $scope.allAttaches = data.response;
                else {
                    $scope.allAttaches = [];
                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
                }
            }).error(function (data, status, headers, config) {
                console.log("Error al cargar ajuntos de actividad...", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al obtener los archivos adjuntos de la actividad' }]);
            });
        };

        $scope.removeAttach = function (attachCode) {
            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/removeAttach', {
                attachCode: attachCode
            }).success(function (data, status, headers, config) {
                console.log("Eliminar adjunto...", data);
                $scope.getAllActivitiesAttach();
            }).error(function (data, status, headers, config) {
                console.log("Error eliminar adjunto...", data);
            });
        }

        this.removeActivity = function (code) {
            var parentObject = this;

            $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/removeActivityById', {
                activityId: code
            }).success(function (data, status, headers, config) {
                console.log("Eliminar actividad... ", data);
                if (data.success)
                    parentObject.removeElementArray($scope.gridOptions.data, code);

                $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
            }).error(function (data, status, headers, config) {
                console.log("Error al eliminar la actividad... ", data);
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al eliminar la actividad' }]);
            });
        };

        this.removeElementArray = function (arrayActivity, activityCode) {
            for (var i = 0; i < arrayActivity.length; i++) {
                if (arrayActivity[i].CODIGO == activityCode) {
                    arrayActivity.splice(i, 1);
                }
            }
        };

        $scope.setSendMail = function () {
            if ($scope.sendMail)
                $scope.sendMail = false;
            else
                $scope.sendMail = true;
        };

        $scope.openDialogAtivitiesReport = function () {

            //$scope.solicitudbecaReport = angular.copy($scope.gridOptions.data);

            $scope.getReportActivities();

            ngDialog.open({
                template: 'activitiesReport.html',
                className: 'ngdialog-theme-flat ngdialog-report',
                closeByDocument: true,
                closeByEscape: true,
                scope: $scope,
                controller: $controller('ngDialogController', {
                    $scope: $scope,
                    $http: $http
                })
            });
        };

    }]);

    app.controller('ngDialogController', ['$scope', '$http', function ($scope, $http) {
        // Editar una actividad general
        $scope.editGeneralActivities = function () {
            $scope.activityCopy.CODIGOACTIVIDAD = undefined;
            $scope.activityCopy.NOMBREACTIVIDAD = undefined;
            $scope.presentEditGeneralActivity = true;
        };
        // Eliminar una actividad general
        $scope.removeGeneralActivities = function () {
            $scope.activityCopy.CODIGOACTIVIDAD = undefined;
            $scope.activityCopy.NOMBREACTIVIDAD = undefined;
            $scope.presentEditGeneralActivity = false;
        };

        $scope.saveEditedActivity = function () {
            if (!this.activityForm.$invalid) {
                var parentObject = this;
                if ($scope.activityCopy.CODIGOACTIVIDAD == undefined)
                    $scope.activityCopy.CODIGOACTIVIDAD = 0;
                if ($scope.activityCopy.NOMBREACTIVIDAD == undefined)
                    $scope.activityCopy.NOMBREACTIVIDAD = "";

                $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/saveActivityData', {
                    activityId: $scope.activityCopy.CODIGO,
                    activityName: $scope.activityCopy.NOMBRE.toUpperCase(),
                    activityDate: $scope.activityCopy.FECHA,
                    activityStatus: $scope.activityCopy.ESTADO,
                    activityObservation: $scope.activityCopy.OBSERVACION.toUpperCase(),
                    generalActivityId: $scope.activityCopy.CODIGOACTIVIDAD,
                    generalActivityName: $scope.activityCopy.NOMBREACTIVIDAD.toUpperCase(),
                    userId: $scope.activityCopy.CODIGOUSUARIO,
                    groupLevelActivity: $scope.groupLevelActivity,
                    sendMail: $scope.sendMail
                }).success(function (data, status, headers, config) {
                    console.log("Editar actividad: ", data);
                    if (data.success) {
                        $scope.updateElementArray($scope.gridOptions.data, $scope.activityCopy.CODIGOACTIVIDAD, $scope.activityCopy.CODIGO,
                            $scope.activityCopy.NOMBRE.toUpperCase(), Date.parse($scope.activityCopy.FECHA), $scope.activityCopy.ESTADO,
                            $scope.activityCopy.OBSERVACION.toUpperCase(), $scope.activityCopy.CODIGOUSUARIO, $scope.activityCopy.NOMBREACTIVIDAD.toUpperCase());
                        parentObject.closeThisDialog();
                    }

                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al editar la actividad...", data);
                    $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al actualizar la actividad' }]);
                });
            } else {
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Editar', detail: 'Ingrese correctamente todos los datos' }]);
            }
        };

        $scope.addNewActivityDB = function () {
            if (!this.newActivityForm.$invalid) {
                var newParentObject = this;
                if ($scope.activityCopy.CODIGOACTIVIDAD == undefined)
                    $scope.activityCopy.CODIGOACTIVIDAD = 0;
                if ($scope.activityCopy.NOMBREACTIVIDAD == undefined)
                    $scope.activityCopy.NOMBREACTIVIDAD = "";

                $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/addNewActivity', {
                    activityName: $scope.activityCopy.NOMBRE.toUpperCase(),
                    activityDate: $scope.activityCopy.FECHA,
                    activityStatus: $scope.activityCopy.ESTADO,
                    activityObservation: $scope.activityCopy.OBSERVACION.toUpperCase(),
                    generalActivityId: $scope.activityCopy.CODIGOACTIVIDAD,
                    generalActivityName: $scope.activityCopy.NOMBREACTIVIDAD.toUpperCase(),
                    userId: $scope.activityCopy.CODIGOUSUARIO,
                    sendMail: $scope.sendMail
                }).success(function (data, status, headers, config) {
                    console.log("Agregar actividad: ", data);
                    if (data.success) {
                        $scope.addElementArray($scope.gridOptions.data, data.response, Date.parse($scope.activityCopy.FECHA),
                            $scope.activityCopy.CODIGOACTIVIDAD, $scope.activityCopy.ESTADO, $scope.activityCopy.NOMBREACTIVIDAD.toUpperCase());
                        newParentObject.closeThisDialog();
                    }

                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al agregar el rol...", data);
                    $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al agregar la actividad' }]);
                });
            } else {
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos' }]);
            }
        };

        $scope.saveGroupActivity = function () {
            if ($scope.selectedCareers.length == 0 && $scope.selectedModalities.length == 0 && $scope.selectedLevels.length == 0 && $scope.selectedGroups == 0) {
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Editar', detail: 'Debe seleccionar por lo menos una carrera, o un m&oacute;dulo o un nivel' }]);
            } else {
                var parentObject = this;

                $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/saveGroupActivity', {
                    group: $scope.selectedGroups,
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

                    $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al editar grupo actividad...", data);
                    $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al actualizar el grupo de la actividad' }]);
                });
            }
        };

        function pad(number) {
            if (number < 10) {
                return '0' + number;
            }
            return number;
        }

        //2016-07-31T05:00:00.000Z
        var toUTCDateTimeDigits = function (dateParam) {
            return dateParam.getUTCFullYear() + "-" + pad(dateParam.getUTCMonth() + 1) + "-" + pad(dateParam.getUTCDate()) + 'T' +
                pad(dateParam.getUTCHours()) + ":" + pad(dateParam.getUTCMinutes()) + ":" + pad(dateParam.getUTCSeconds()) + "." + pad(dateParam.getUTCMilliseconds()) + 'Z';
        };

        var exportExcelReport = function () {
            var dateFrom = toUTCDateTimeDigits($scope.date.dateFrom);
            var dateTo = toUTCDateTimeDigits($scope.date.dateTo);
            //console.log(JSON.stringify({dateFrom: '2011-04-02 17:15:45'}));
            var url = (appContext ? appContext : "") + '/WebServices/Activities.asmx/exportExcelReport?dateFrom=' + dateFrom + '&dateTo=' + dateTo;
            return url;
        };

        $scope.urlExport = exportExcelReport();

        $scope.$watch('date.dateTo', function () {
            $scope.urlExport = exportExcelReport();
        });

        $scope.$watch('date.dateFrom', function () {
            $scope.urlExport = exportExcelReport();
        });

        $scope.saveAssistanceDB = function () {
            if (!this.assistanceForm.$invalid) {
                var parentObject = this;

                $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/saveAssistanceData', {
                    assistance: $scope.assistance
                }).success(function (data, status, headers, config) {
                    console.log("Actualizar asistencia: ", data);
                    if (data.success)
                        parentObject.closeThisDialog();
                    else
                        $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
                }).error(function (data, status, headers, config) {
                    console.log("Error al actualizar asistencia...", data);
                    $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Error', detail: 'Error al actualizar asistencia' }]);
                });
            } else {
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos' }]);
            }
        };

        $scope.saveAttachedDB = function () {
            if (!this.attachedForm.$invalid) {
                var formElement = document.getElementById('attachedForm');
                var formData = new FormData(formElement);

                $scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/addUploadedFileDataBase?codigoActividad=' + $scope.activityCopy.CODIGO,
                    formData, {
                        withCredentials: true,
                        headers: { 'Content-Type': undefined },
                        transformRequest: angular.identity
                    }).success(function (data, status, headers, config) {
                        // console.log("Adjuntos", data);
                        if (data.success) {
                            $scope.getAllActivitiesAttach();
                            document.getElementById("observacion").value = "";
                            document.getElementById("attachedActivity").value = "";
                        }
                        $('#messages').puigrowl('show', [{ severity: data.severity, summary: data.summary, detail: data.message }]);
                    }).error(function (data, status, headers, config) {
                        console.log("error al cargar los tipos...", data);
                    });
            } else {
                $('#messages').puigrowl('show', [{ severity: 'error', summary: 'Nuevo', detail: 'Ingrese correctamente todos los datos' }]);
            }
        };

        $scope.isImage = function (contentType) {
            //console.log(contentType);
            if (typeof contentType === 'string' && contentType.includes('image')) {
                //console.log(contentType);
                return true;
            }
            return false;
        };

    }]);

    app.directive('validActivityName', ['$http', function ($http) {
        return {
            require: 'ngModel',

            link: function (scope, element, attr, ctrl) {
                function customValidator(ngModelValue) {
                    if (ngModelValue != null && ngModelValue != scope.activityCopy.NOMBRE) {

                        ctrl.$setValidity('activityNameChecking', false);

                        scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/countActivityWithName', {
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

    app.directive('validFileInput', ['$http', function ($http) {
        return {
            restrict: 'A',

            require: 'ngModel',

            link: function (scope, element, attr, ctrl) {
                ctrl.$setValidity('validFile', element.val() != '' || !element.get(0).required);
                element.bind('change', function () {
                    if (element.get(0).files.length > 0) {
                        ctrl.$setValidity('validFile', true);
                        ctrl.$setValidity('validFileSize', element.get(0).files[0].size < 2000000);
                        ctrl.$setValidity('validFileEmpty', element.get(0).files[0].size != 0);
                        ctrl.$setValidity('validFileType',
                            element.get(0).files[0].type.toUpperCase().indexOf('APPLICATION/PDF') != -1 ||
                            element.get(0).files[0].type.toUpperCase().indexOf('IMAGE') != -1);

                    } else {
                        if (element.get(0).required) ctrl.$setValidity('validFile', false);
                        else ctrl.$setValidity('validFile', true);
                        ctrl.$setValidity('validFileSize', true);
                        ctrl.$setValidity('validFileEmpty', true);
                        ctrl.$setValidity('validFileType', true);
                    }
                    scope.$apply(function () {
                        ctrl.$setViewValue(element.val());
                        ctrl.$render();
                    });
                });
            }
        };
    }]);

    app.filter("rangeDateFilter", function () {
        return function (items, from, to) {

            var result = [];
            if (items != undefined) {
                var df = from;
                df.setHours(0, 0, 0, 0);
                var dt = to;
                dt.setHours(24);
                for (var i = 0; i < items.length; i++) {
                    var t = new Date(parseInt(items[i].FECHA));
                    if (df <= t && t <= dt) {
                        result.push(items[i]);
                    }
                }
            }
            return result;
        };
    });

    app.directive('validGeneralActivityName', ['$http', function ($http) {
        return {
            require: 'ngModel',

            link: function (scope, element, attr, ctrl) {
                function customValidator(ngModelValue) {
                    if (ngModelValue != null && ngModelValue != scope.activityCopy.NOMBREACTIVIDAD) {
                        scope.promise = $http.post((appContext != undefined ? appContext : "") + '/WebServices/Activities.asmx/countGeneralActivityWithName', {
                            activityName: ngModelValue
                        }).success(function (data, status, headers, config) {
                            if (data.cantidad == 0) {
                                ctrl.$setValidity('generalActivityNameExist', true);

                            } else {
                                ctrl.$setValidity('generalActivityNameExist', false);
                            }
                        }).error(function (data, status, headers, config) {
                            console.log("Error al traer actividad", data);
                        });
                    } else {
                        ctrl.$setValidity('generalActivityNameExist', true);
                    }
                    return ngModelValue;
                }

                ctrl.$parsers.push(customValidator);
            }
        };
    }]);
})();