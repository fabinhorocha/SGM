app.controller('indicatorOilManagementController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, commonService, userFactory, oilManagementFactory,indicatorOilManagementFactory) {

    //$scope.idIndicator = $routeParams.id ? $routeParams.id : 1 ;
    //$scope.Type = $routeParams.type ?  $routeParams.type : 'Exec';
    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    var initialDate = new Date(y, m, 1);
    $scope.startDate = $filter('date')(initialDate, 'dd/MM/yyyy');
    $scope.endDate = $filter('date')(new Date(),'dd/MM/yyyy');

    $scope.model = undefined;

    $scope.stoppageType = undefined;
    $scope.oilSupplyType = undefined;
    $scope.oilType = undefined;

    $scope.idReportType = 1;
    $scope.dataArray = [];

    $scope.reportsType = [
        { idReportType: 1, Name : 'Periodo' },
        { idReportType: 2, Name : 'Mensal' },
        { idReportType: 3, Name : 'Diario' }
    ];

    $scope.reporType = $scope.reportsType[0]; 

    switch($routeParams.id){
        case "1": //
            $scope.Title = 'Indicadores de Abastecimento';
            $scope.idReportClass = 1;
            break;
        case "2":
            $scope.Title = 'Indicadores de Paradas';
            $scope.idReportClass = 2;
            break;
        default:
            $scope.Title = '';
        break;
    }

    if ($rootScope.user === null){
        GetUser();
    }

    $scope.oilFactoriesAll = [];
    $scope.oilAreasAll = [];
    $scope.oilEquipmentsAll = [];
    $scope.oilComponentsAll = [];

    $scope.emptyOption = { 
                    idComponent : -1, 
                    Equipment : null, 
                    Name: null, 
                    Capacity : null, 
                    Enableds: [], 
                    OilGradeISO : null,
                    ISOLimitCode : null,
                    CriticalComponent : null,
                    MachinesServed: null, 
                    FlowRate: null, 
                    Preassure: null, 
                    Active: null, 
                    CreatedBy : null, 
                    ModificatedBy : null, 
                    cdIntegrate : null,
                    InsDateTime : null, 
                    UpdDateTime : null
                };

    $scope.factoryEmptyOption = { 
                    idFactory : -1, 
                    Code : null, 
                    Name: null, 
                    Active: null, 
                    InsDateTime : null, 
                    UpdDateTime : null
                };

    $scope.areaEmptyOption = { 
                    idArea : -1, 
                    idFactory : -1, 
                    Factory: null,
                    Code : null, 
                    Name: null, 
                    Active: null, 
                    InsDateTime : null, 
                    UpdDateTime : null
                };

    $scope.equipmentEmptyOption = { 
                    idEquipment : -1,
                    idArea : -1,
                    Area: null,
                    Code : null, 
                    Name: null, 
                    Active: null, 
                    InsDateTime : null, 
                    UpdDateTime : null
                };

    GetMenu();
    GetHidraulicUnits();
    GetFilterInfo();

    //userLogged, $scope.oilFactory.idFactory, $scope.oilArea.idArea, $scope.oilEquipment.idEquipment, $scope.oilComponent.idComponent, startDate, endDate,reportType, onlyActive

    var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
    var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');
    GetGraphicReportData($scope.idReportClass, $scope.reporType.idReportType,-1,-1,-1,-1,startDate,endDate,-1,-1,-1,true,true);

    $scope.oilManagementData = [];
    var graphicReport = { 
        idChartReport : $routeParams.id, 
        chartReportTitle : $scope.idReportClass == 1 ? "ABASTECIMENTOS" : "PARADAS NÃO OPERATIVAS",
        colors : [],
        dataChartReport : [],
        labelsChartReport : [],
        datasetOverride : []
    };
    
    $scope.oilManagementData.push(graphicReport);

    function GetUser(){
        
        userFactory.GetUser().success(function (response){
    
                if(response){
                    $rootScope.user = response;
                }            
            }).error(function(error){
                
                $.bigBox({
                    title : "Erro!",
                    content : error ? error.Message : "Falha de comunicação com o servidor.",            
                    color : "#C46A69",              
                    icon : "fa fa-warning swing animated",              
                    timeout : 6000 });
            });    
    }            

    function GetMenu(){
     
        oilManagementFactory.GetAllEquipments(true).success(function (response){

                    if(response != null){

                    $scope.oilFactoriesAll = response.Factories;
                    $scope.oilAreasAll = response.Areas;
                    $scope.oilEquipmentsAll = response.Equipments;

                    $scope.oilFactories = [];
                    $scope.oilFactories.push($scope.factoryEmptyOption);
                    angular.forEach($scope.oilFactoriesAll, function (obj, key) {
                        $scope.oilFactories.push(obj);
                    });
                    $scope.oilFactory = $scope.oilFactories[0];

                    $scope.oilAreas = [];
                    $scope.oilAreas.push($scope.areaEmptyOption);
                    angular.forEach($scope.oilAreasAll, function (obj, key) {
                        $scope.oilAreas.push(obj);
                    });
                    $scope.oilArea = $scope.oilAreas[0];

                    $scope.oilEquipments = [];
                    $scope.oilEquipments.push($scope.equipmentEmptyOption);
                    angular.forEach($scope.oilEquipmentsAll, function (obj, key) {
                        $scope.oilEquipments.push(obj);
                    });
                    $scope.oilEquipment = $scope.oilEquipments[0];
                }            
            }).error(function(error){
                
                $.bigBox({
                    title : "Erro!",
                    content : error ? error.Message : "Falha de comunicação com o servidor.",            
                    color : "#C46A69",              
                    icon : "fa fa-warning swing animated",              
                    timeout : 6000 });
    
            });    
    }

    function GetHidraulicUnits(){
     
        oilManagementFactory.GetAllComponents().success(function (response){

                $scope.stillHiddenOilSupplyEdit = true;
    
                if(response.length > 0){
                    $scope.oilComponentsAll = response;

                    $scope.oilComponents = [];
                    $scope.oilComponents.push($scope.emptyOption);
                    angular.forEach(response, function (obj, key) {
                        $scope.oilComponents.push(obj);
                    });
                    $scope.oilComponent = $scope.oilComponents[0];
                }            
            }).error(function(error){
                
                $.bigBox({
                    title : "Erro!",
                    content : error ? error.Message : "Falha de comunicação com o servidor.",            
                    color : "#C46A69",              
                    icon : "fa fa-warning swing animated",              
                    timeout : 6000 });
    
            });    
    }

    function GetFilterInfo() {

        indicatorOilManagementFactory.GetFilterInfo().success(function (response){

            if(response){

                $scope.model = response;

                $scope.oilSupplyType = $scope.model.OilSupplyTypes[0];
                $scope.stoppageType = $scope.model.StoppageTypes[0];
                $scope.oilType = $scope.model.OilTypes[0]; 

            }            
        }).error(function(error){
            
            $.bigBox({
                title : "Erro!",
                content : error ? error.Message : "Falha de comunicação com o servidor.",            
                color : "#C46A69",              
                icon : "fa fa-warning swing animated",              
                timeout : 6000 });

        });  

    }

    $scope.clearFilters = function () {

        $scope.stoppageType = undefined;
        $scope.oilSupplyType = undefined;
        $scope.oilType = undefined;

        $scope.oilFactories = [];
        $scope.oilFactories.push($scope.factoryEmptyOption);
        angular.forEach($scope.oilFactoriesAll, function (obj, key) {
            $scope.oilFactories.push(obj);
        });
        $scope.oilFactory = $scope.oilFactories[0];

        $scope.oilAreas = [];
        $scope.oilAreas.push($scope.areaEmptyOption);
        angular.forEach($scope.oilAreasAll, function (obj, key) {
            $scope.oilAreas.push(obj);
        });
        $scope.oilArea = $scope.oilAreas[0];

        $scope.oilEquipments = [];
        $scope.oilEquipments.push($scope.equipmentEmptyOption);
        angular.forEach($scope.oilEquipmentsAll, function (obj, key) {
            $scope.oilEquipments.push(obj);
        });
        $scope.oilEquipment = $scope.oilEquipments[0];

        $scope.oilComponents = [];
        $scope.oilComponents.push($scope.emptyOption);
        angular.forEach($scope.oilComponentsAll, function (obj, key) {
            $scope.oilComponents.push(obj);
        });
        $scope.oilComponent = $scope.oilComponents[0];

        $scope.oilSupplyType = $scope.model.OilSupplyTypes[0];
        $scope.stoppageType = $scope.model.StoppageTypes[0];
        $scope.oilType = $scope.model.OilTypes[0]; 

        $scope.labels = [];
        $scope.data = [];
        $scope.series = [];

        $scope.stillHidden = true;
        $scope.stillHiddenOilSupplyEdit = true;
        $scope.viewLoaded = false;
    }

    $scope.selectedLevelMenuChange = function (level, modelItemSelected) {
        
        var areasOptions = [];
        var equipmentsOptions = [];
        var idOilType = -1;
        var idOilSupplyType = -1;
        var idStoppageType = -1;


        if ($scope.oilType != undefined)
            idOilType = $scope.oilType.idOilType;
        if ($scope.oilSupplyType != undefined)
            idOilSupplyType = $scope.oilSupplyType.idOilSupplyType;
        if ($scope.stoppageType != undefined)
            idStoppageType = $scope.stoppageType.idStoppageType;
        
        //Factory selected
        if (level == 1) {

            var oilAreasAux = $scope.oilAreasAll.filter(function(a){ if (a.idFactory === $scope.oilFactory.idFactory) { areasOptions.push(a.idArea); return true; } else return false; });
            $scope.oilAreas = [];
            $scope.oilAreas.push($scope.areaEmptyOption);
            angular.forEach(oilAreasAux, function (obj, key) {
                $scope.oilAreas.push(obj);
            });
            $scope.oilArea = $scope.oilAreas[0];

            var oilEquipmentsAux = $scope.oilEquipmentsAll.filter(function(e){ if (areasOptions.indexOf(e.idArea) >= 0) { equipmentsOptions.push(e.idEquipment); return true; }  else return false; });
            $scope.oilEquipments = [];
            $scope.oilEquipments.push($scope.equipmentEmptyOption);
            angular.forEach(oilEquipmentsAux, function (obj, key) {
                $scope.oilEquipments.push(obj);
            });
            $scope.oilEquipment = $scope.oilEquipments[0];

            var oilComponentsAux = $scope.oilComponentsAll.filter(function(c){ if (equipmentsOptions.indexOf(c.Equipment.idEquipment) >= 0) return true; else return false; });
            $scope.oilComponents = [];
            $scope.oilComponents.push($scope.emptyOption);
            angular.forEach(oilComponentsAux, function (obj, key) {
                $scope.oilComponents.push(obj);
            });
            $scope.oilComponent = $scope.oilComponents[0];

            var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
            var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');
            
            GetGraphicReportData($scope.idReportClass,$scope.reporType.idReportType,$scope.oilFactory.idFactory,-1,-1,-1,startDate,endDate,idOilType,idOilSupplyType,idStoppageType,true,true);
        }

        //Area selected
        if (level == 2) {

            var oilEquipmentsAux = $scope.oilEquipmentsAll.filter(function(e){ if (e.idArea === $scope.oilArea.idArea) { equipmentsOptions.push(e.idEquipment); return true; } else return false; });
            $scope.oilEquipments = [];
            $scope.oilEquipments.push($scope.equipmentEmptyOption);
            angular.forEach(oilEquipmentsAux, function (obj, key) {
                $scope.oilEquipments.push(obj);
            });
            $scope.oilEquipment = $scope.oilEquipments[0];

            var oilComponentsAux = $scope.oilComponentsAll.filter(function(c){ if (equipmentsOptions.indexOf(c.Equipment.idEquipment) >= 0) return true; else return false; });
            $scope.oilComponents = [];
            $scope.oilComponents.push($scope.emptyOption);
            angular.forEach(oilComponentsAux, function (obj, key) {
                $scope.oilComponents.push(obj);
            });
            $scope.oilComponent = $scope.oilComponents[0];

            $scope.oilFactory = $scope.oilFactories.filter(function(f){ if (f.idFactory === $scope.oilArea.idFactory) return true ; else return false; })[0];

            $scope.viewLoaded = false;
            $scope.stillHidden = true;
            $scope.stillHiddenOilSupplyEdit = true;

            var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
            var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');
            
            GetGraphicReportData($scope.idReportClass,$scope.reporType.idReportType,$scope.oilFactory.idFactory,$scope.oilArea.idArea,-1,-1,startDate,endDate,idOilType,idOilSupplyType,idStoppageType,true,true);
        }

        //Equipment selected
        if (level == 3) {

            var oilComponentsAux = $scope.oilComponentsAll.filter(function(c){ if (c.Equipment.idEquipment == $scope.oilEquipment.idEquipment) return true; else return false; });
            $scope.oilComponents = [];
            $scope.oilComponents.push($scope.emptyOption);
            angular.forEach(oilComponentsAux, function (obj, key) {
                $scope.oilComponents.push(obj);
            });
            $scope.oilComponent = $scope.oilComponents[0];

            $scope.oilArea = $scope.oilAreas.filter(function(a){ if (a.idArea === $scope.oilEquipment.idArea) return true ; else return false; })[0];
            $scope.oilFactory = $scope.oilFactories.filter(function(f){ if (f.idFactory === $scope.oilArea.idFactory) return true ; else return false; })[0];

            $scope.viewLoaded = false;
            $scope.stillHidden = $rootScope.user.OilManagement ? false : true;
            $scope.stillHiddenOilSupplyEdit = true;

            var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
            var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');
            
            GetGraphicReportData($scope.idReportClass,$scope.reporType.idReportType,$scope.oilFactory.idFactory,$scope.oilArea.idArea,$scope.oilEquipment.idEquipment,-1,startDate,endDate,idOilType,idOilSupplyType,idStoppageType,true,true);
        }

        //Component selected
        if (level == 4)
        {
            $scope.oilEquipment = $scope.oilEquipments.filter(function(e){ if (e.idEquipment === $scope.oilComponent.Equipment.idEquipment) return true ; else return false; })[0];
            $scope.oilArea = $scope.oilAreas.filter(function(a){ if (a.idArea === $scope.oilEquipment.idArea) return true ; else return false; })[0];
            $scope.oilFactory = $scope.oilFactories.filter(function(f){ if (f.idFactory === $scope.oilArea.idFactory) return true ; else return false; })[0];

            var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
            var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');
            
            GetGraphicReportData($scope.idReportClass, $scope.reporType.idReportType,$scope.oilFactory.idFactory,$scope.oilArea.idArea,$scope.oilEquipment.idEquipment,$scope.oilComponent.idComponent,startDate,endDate,idOilType,idOilSupplyType,idStoppageType,true,true);
        }
    };

    $scope.refreshGraphic = function () {

        var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
        var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');

        var idOilType = -1;
        var idOilSupplyType = -1;
        var idStoppageType = -1;


        if ($scope.oilType != undefined)
            idOilType = $scope.oilType.idOilType;
        if ($scope.oilSupplyType != undefined)
            idOilSupplyType = $scope.oilSupplyType.idOilSupplyType;
        if ($scope.stoppageType != undefined)
            idStoppageType = $scope.stoppageType.idStoppageType;
    
        GetGraphicReportData($scope.idReportClass,$scope.reporType.idReportType,$scope.oilFactory.idFactory,$scope.oilArea.idArea,$scope.oilEquipment.idEquipment,$scope.oilComponent.idComponent,startDate,endDate,idOilType,idOilSupplyType,idStoppageType,true,true);

    }

    $scope.colors = ['#CC0066',
                    '#000099',
                    '#803690', 
                    '#00ADF9', 
                    '#DCDCDC', 
                    '#FDB45C',
                    '#949FB1',
                    '#4D5360',
                    '#ED402A',
                    '#36A2EB',
                    '#FFCE56',
                    '#F0AB05',
                    '#A0B421',
                    '#00A39F'];

    function GetGraphicReportData(idReportClass, idReportType, idFactory, idArea, idEquipment, idComponent,startDate, endDate, idOilType, idOilSupplyType, idStoppageType, OilManagement,OilUser){

        $scope.labels = [];
        $scope.data = [];
        $scope.series = [];

        indicatorOilManagementFactory.GetOilSupplyData(idReportClass, idReportType, idFactory, idArea, idEquipment, idComponent,startDate, endDate, idOilType, idOilSupplyType, idStoppageType, OilManagement,OilUser).success(function (response){    

            if(response){
                
                if ($scope.reporType.idReportType == 1) //Periodo
                {
                    var data = [];
                    $scope.series = ['Serie 1'];

                    angular.forEach(response, function (obj, key) {
                        $scope.labels.push(obj.Label);
                        data.push(obj.Data);
                    });

                    $scope.data.push(data);

                    $scope.datasetOverride = [ 
                        {                    
                            label: $scope.idReportClass == 1 ? "Qtd (lts)" : "Tempo (min)",
                            type: 'bar', 
                            backgroundColor : $scope.colors
                        }];    
                }
                else if ($scope.reporType.idReportType == 2 || $scope.reporType.idReportType == 3) //Mensal e Diario
                {
                    //Series igual a fabrica, area, equipamento, componente
                    $scope.series = response[0].Series;

                    //Labels igual a meses, dias                    
                    $scope.labels = response[0].Labels;

                    //Data Crear un arreglo por cada fabrica, en donde cada posicion corresponde a un mes o dia 
                    angular.forEach(response[0].Data, function (obj, key) {
                        var data = [];
                        angular.forEach(obj, function (obj2, key2) {
                            data.push(obj2);
                        });

                        $scope.data.push(data);
                    });

                    $scope.datasetOverride = [];

                    angular.forEach($scope.series, function (item, idx) {

                        var backgroundColors = [];
                        angular.forEach($scope.labels, function (label, idxLabel) {
                            backgroundColors.push($scope.colors[idx]);
                        });

                        var styleBar =  {
                                fill: true,
                                type: 'bar',
                                backgroundColor: backgroundColors
                           }
                        
                         $scope.datasetOverride.push(styleBar);
                    });
                }
            
                $scope.options = {
                    responsive: true,
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero:true,
                            },
                        }],
                        xAxes: [{
                            stacked: false,
                            ticks : {
                                autoSkip: false
                            }
                        }]
                    },
                    
                    
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            generateLabels: function(chart) {
                                
                                var result = [];

                                if ($scope.reporType.idReportType == 1) {//Periodo
                                    angular.forEach(response, function (item, index) {
                                        result.push({text : item.Label, fillStyle: $scope.colors[index]});
                                    });
                                }
                                if ($scope.reporType.idReportType == 2 || $scope.reporType.idReportType == 3) {//Periodo
                                    angular.forEach(response[0].Series, function (item, index) {
                                        result.push({text : item, fillStyle: $scope.colors[index]});
                                    });
                                }

                                return result;
                            }
                        }
                    },
            
                    elements: {
                        line: {
                            tension: 0 // disables bezier curves                       
                        },
                        point:{
                            pointStyle: 'circle'
                        }
                    },
                    tooltips: {
                        
                        filter: function(tooltipItem, data) {
                            if(tooltipItem.y) {
                                if (data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index] > 0) // if value es more than 0
                                    return !data.datasets[tooltipItem.datasetIndex].tooltipHidden;
                                else
                                return data.datasets[tooltipItem.datasetIndex].tooltipHidden;
                            }
                            else
                                return data.datasets[tooltipItem.datasetIndex].tooltipHidden;
                            }
                    }
                };
            }

        }).error(function(error){

            $.bigBox({
            title : "Erro!",
            content : error ? error.Message : "Falha de comunicação com o servidor.",            
            color : "#C46A69",              
            icon : "fa fa-warning swing animated",              
            timeout : 6000 });

        });    
    };    
   

});    
