app.controller('oilManagementController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, userFactory, oilManagementFactory, commonService, oilSupplyService, oilComponentsService) {

    var initialDate = new Date();
    initialDate.setDate(new Date().getDate() - 30);
    $scope.startDate = $filter('date')(initialDate, 'dd/MM/yyyy');
    $scope.endDate = $filter('date')(new Date(),'dd/MM/yyyy');


    $scope.stillHidden = true;
    $scope.stillHiddenOilSupplyEdit = true;

    $scope.reports = undefined;

    $scope.dataLoading = false;
    $scope.idSelectedRow = null;
    $scope.isSelectedRow = false;

    $scope.selTab = 1;
    $scope.viewbyTab2 = "25";
    $scope.viewbyTab3 = "25";
    $scope.viewLoaded = false;
    
    $scope.fileName = "gestaoOleo";
    $scope.exportData = [];

    if ($rootScope.user === null){
        GetUser();
    }

    $scope.oilFactoriesAll = [];
    $scope.oilAreasAll = [];
    $scope.oilEquipmentsAll = [];
    $scope.oilComponentsAll = [];
    $scope.selectedOilTypes = [];

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

    var $ctrl = { 
        action: null,
        model: null,
        title: null,
        object : '',
        equipment: null,
        equipmentsAll : null,
        component: null,
        componentsAll : null,
        oilSupply : null,
        suppliersAll: null,
        suppliesService : null,
        componentsService : null,
        user : null
    };

    $scope.suppliesService = oilSupplyService;
    $scope.componentsService = oilComponentsService;

    $scope.$watch('suppliesService.getLastOilSupplies()', function(){
        $scope.reports = $scope.suppliesService.getLastOilSupplies();
        },true
    );

    $scope.$watch('componentsService.getComponents()', function(){
        
        $scope.oilComponentsAll = $scope.componentsService.getComponents();

        $scope.oilComponents = [];
        $scope.oilComponents.push($scope.emptyOption);
        var newIdComponent = -1;
        var idx = 0;
        var idxNewComponent = -1;
        angular.forEach($scope.oilComponentsAll, function (obj, key) {
            idx++;
            if (obj.idComponent > newIdComponent){
                newIdComponent = obj.idComponent;
                idxNewComponent = idx;    
            } 
            $scope.oilComponents.push(obj);
        });

        if (idxNewComponent > -1) {
            $scope.oilComponent = $scope.oilComponents[idxNewComponent];

            $scope.selTab = 1;

            $scope.LoadTabInfo($scope.selTab);
            
            $scope.stillHidden = $rootScope.user.OilManagement ? false : true;
            $scope.stillHiddenOilSupplyEdit = true;
            $scope.viewLoaded = true;
        }

        },true
    );

    GetMenu();
    GetHidraulicUnits();

    $scope.checkDisabledOption = function() {

        if ($rootScope.user != null)
        {
            if (!$rootScope.user.OilManagement) {
                return true;   
            }
            else {
                return false;
            }
        }
        else {
            return true;            
        }
    }

    //filter functionality
    $scope.XLfilters = { list: [], dict: {}, results: [] };
    $scope.markAll = function(field, b) {
        for (i = 0; i < $scope.XLfilters.dict[field].list.length; i++)  {
        $scope.XLfilters.dict[field].list[i].checked=b;     
        }
    }

    $scope.clearFilter = function(field) {
        $scope.XLfilters.dict[field].searchText='';
        for (i = 0; i < $scope.XLfilters.dict[field].list.length; i++)  {
        $scope.XLfilters.dict[field].list[i].checked=true;
        }
    }

    $scope.XLfiltrate = function() {
        var i,j,k,selected,blocks,filter,option, data=$scope.XLfilters.all,filters=$scope.XLfilters.list;
        $scope.XLfilters.results=[];
        for (j=0; j<filters.length; j++) {
            filter=filters[j];
        filter.regex = filter.searchText.length?new RegExp(filter.searchText, 'i'):false;
        for(k=0,selected=0;k<filter.list.length;k++){
            if(!filter.list[k].checked)selected++;
            filter.list[k].visible=false;
            filter.list[k].match=filter.regex?filter.list[k].title.match(filter.regex):true;
        }
        filter.isActive=filter.searchText.length>0||selected>0;
        }
        for (i=0; i<data.length; i++){
        blocks={allows:[],rejects:[],mismatch:false};
            for (j=0; j<filters.length; j++) {
            filter=filters[j]; option=filter.dict[data[i][filter.field]];
            (option.checked?blocks.allows:blocks.rejects).push(option);
            if(filter.regex && !option.match) blocks.mismatch=true;
            }
        if(blocks.rejects.length==1) blocks.rejects[0].visible=true;
        else if(blocks.rejects.length==0&&!blocks.mismatch){
            $scope.XLfilters.results.push(data[i]);
            //blocks.allows.forEach((x)=>{x.visible=true});
            for (w = 0; w < blocks.allows.length; w++)  {
            blocks.allows[w].visible=true;
            }        	
        }        
        }
        
        for (j=0; j<filters.length; j++) {
            filter=filters[j];filter.options=[];
        for(k=0;k<filter.list.length;k++){
            if(filter.list[k].visible && filter.list[k].match) filter.options.push(filter.list[k]);
        }
        } 
    }

    function createXLfilters(arr, fields) {
        $scope.XLfilters.all = arr;
        for (var j=0; j<fields.length; j++) 
        $scope.XLfilters.list.push($scope.XLfilters.dict[fields[j]]={list:[],dict:{},field:fields[j],searchText:"",active:false,options:[]});
        for (var i=0,z; i<arr.length; i++){
        for (j=0; j<fields.length; j++) {
            z=$scope.XLfilters.dict[fields[j]];
            z.dict[arr[i][fields[j]]] || z.list.push(z.dict[arr[i][fields[j]]]={title:arr[i][fields[j]],checked:true, visible:false,match:false});
            }
        }
    }

    $scope.clearAll = function() {

        $scope.XLfilters = { list: [], dict: {}, results: [] };
        createXLfilters($scope.filters, $scope.filterColumns);
    }

    $scope.$watch('XLfilters.results', function (newValue, oldValue, scope) {
        
        if (oldValue.length != newValue.length) {

            if ($scope.selTab === 2) {

                $scope.totalItemsTab2 = newValue.length;
                $scope.currentPageTab2 = 1;
                $scope.itemPageStartTab2 = ((($scope.currentPageTab2-1)*$scope.itemsPerPageTab2)+1);            
                $scope.itemPageEndTab2 = $scope.itemsPerPageTab2 * $scope.currentPageTab2;
                
                $scope.exportData = [];
    
                $scope.exportData.push($scope.oilSupplyHistory.columnsToExport);
    
                angular.forEach(newValue, function(value, key) {
                    $scope.exportData.push([
                                value.factoryName,
                                value.areaName,
                                value.equipmentName,
                                value.componentName,
                                value.Quantity,
                                value.Capacity,
                                value.SupplyDateTime,
                                value.Shift,
                                value.OilType,
                                value.OilSupplyType,
                                value.StoppageType,
                                value.StoppageTime,
                                value.IsReuseOil,
                                value.Comment,
                                value.CreatedBy,
                                value.InsDateTime,
                                value.ModificatedBy,
                                value.UpdDateTime,
                                value.FlowRate,
                                value.Preassure,
                                value.Active,
                                value.OilGradeISO,
                                value.ISOLimitCode,
                                value.CriticalComponent,
                                value.MachinesServed]);
                });        
    
            }
            else if ($scope.selTab === 3) {

                $scope.totalItemsTab3 = newValue.length;
                $scope.currentPageTab3 = 1;
                $scope.itemPageStartTab3 = ((($scope.currentPageTab3-1)*$scope.itemsPerPageTab3)+1);            
                $scope.itemPageEndTab3 = $scope.itemsPerPageTab3 * $scope.currentPageTab3;

                $scope.exportData = [];
    
                $scope.exportData.push($scope.oilComponentSettings.columnsToExport);
    
                angular.forEach(newValue, function(value, key) {
                    $scope.exportData.push([
                        value.factoryName,
                        value.areaName,
                        value.equipmentName,
                        value.componentName,
                        value.Capacity,
                        value.OilTypes,
                        value.FlowRate,
                        value.Preassure,
                        value.Active,
                        value.OilGradeISO,
                        value.ISOLimitCode,
                        value.CriticalComponent,
                        value.MachinesServed]);
                });    
            }
        }
    }, true);

    $scope.TryXLfiltrate = function() {
        if ($scope.dataLoading)
            $scope.XLfiltrate();
    }

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

    function GetComponentInfo(idComponent){
     
        $scope.stillHiddenOilSupplyEdit = true;
        $scope.idSelectedRow = null;
        $scope.isSelectedRow = false;

        oilManagementFactory.GetComponent(idComponent).success(function (response){
    
                if(response){

                    $scope.Component = response;

                    angular.forEach(response.EnabledOilTypes, function (obj, key) {
                       $scope.Component.EnabledOilTypes.push(obj.idOilType);
                    });
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

    function GetLastOilSupplyHistory(userCode, idEquipment, idComponent){
     
        $scope.stillHiddenOilSupplyEdit = true;
        oilManagementFactory.GetLastOilSupplyHistory(userCode, idEquipment, idComponent).success(function (response){
    
                if(response){

                    $scope.reports = response;
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

        $scope.stillHidden = true;
        $scope.stillHiddenOilSupplyEdit = true;
        $scope.viewLoaded = false;
    }

    $scope.selectedLevelMenuChange = function (level, modelItemSelected) {
        
        var areasOptions = [];
        var equipmentsOptions = [];

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
            
            $scope.viewLoaded = false;
            $scope.stillHidden = true;
            $scope.stillHiddenOilSupplyEdit = true;

            //if ($rootScope.user != null && $rootScope.user.OilManagement){
            if ($rootScope.user != null){

                if ($scope.selTab == 1)
                    $scope.selTab = 2;

                $scope.tabsRequestView = [                
                    {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                    {id: 3, name:'Informações Básicas',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
                ];
                
                $scope.LoadTabInfo($scope.selTab);

                $scope.viewLoaded = true;
            }
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

            //if ($rootScope.user != null && $rootScope.user.OilManagement){
            if ($rootScope.user != null){

                if ($scope.selTab == 1)
                    $scope.selTab = 2;

                $scope.tabsRequestView = [                
                    {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                    {id: 3, name:'Informações Básicas',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
                ];
                
                $scope.LoadTabInfo($scope.selTab);

                $scope.viewLoaded = true;
            }
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

            //if ($rootScope.user != null && $rootScope.user.OilManagement){
            if ($rootScope.user != null){

                if ($scope.selTab == 1)
                    $scope.selTab = 2;

                $scope.tabsRequestView = [                
                    {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                    {id: 3, name:'Informações Básicas',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
                ];
                
                $scope.LoadTabInfo($scope.selTab);

                $scope.viewLoaded = true;
            }
        }

        //Component selected
        if (level == 4)
        {
            $scope.oilEquipment = $scope.oilEquipments.filter(function(e){ if (e.idEquipment === $scope.oilComponent.Equipment.idEquipment) return true ; else return false; })[0];
            $scope.oilArea = $scope.oilAreas.filter(function(a){ if (a.idArea === $scope.oilEquipment.idArea) return true ; else return false; })[0];
            $scope.oilFactory = $scope.oilFactories.filter(function(f){ if (f.idFactory === $scope.oilArea.idFactory) return true ; else return false; })[0];

            var hideTab = true;
            //if ($rootScope.user != null && $rootScope.user.OilManagement)
            if ($rootScope.user != null)
                hideTab = false;

            $scope.tabsRequestView = [
                {id: 1, name:'Abastecimento',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: hideTab },
                {id: 3, name:'Informações Básicas',  class: 'fa fa-fw fa-lg fa-tasks', hide: hideTab }
            ];

            $scope.LoadTabInfo($scope.selTab);

            $scope.stillHidden = $rootScope.user.OilManagement ? false : true;
            $scope.stillHiddenOilSupplyEdit = true;
        }
    };

    $scope.setItemsPerPageTab2 = function(num) {
        
        $scope.itemsPerPageTab2 = num;
        $scope.currentPageTab2 = 1; //reset to first page

        $scope.itemPageStartTab2 = ((($scope.currentPageTab2-1)*$scope.itemsPerPageTab2)+1);            
        $scope.itemPageEndTab2 = $scope.itemsPerPageTab2 * $scope.currentPageTab2;            
    }

    $scope.setItemsPerPageTab3 = function(num) {
        
        $scope.itemsPerPageTab3 = num;
        $scope.currentPageTab3 = 1; //reset to first page

        $scope.itemPageStartTab3 = ((($scope.currentPageTab3-1)*$scope.itemsPerPageTab3)+1);            
        $scope.itemPageEndTab3 = $scope.itemsPerPageTab3 * $scope.currentPageTab3;            
    }

    $scope.LoadTabInfo = function(tab){
        
        $scope.selTab = tab;
        var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
        var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');

        if(tab == 1){
            //Lançamento            
            GetComponentInfo($scope.oilComponent.idComponent);
            GetLastOilSupplyHistory('', $scope.oilComponent.Equipment.idEquipment, $scope.oilComponent.idComponent);            
            $scope.saveDisabled = true;
            $scope.viewLoaded = true;
        }

        if(tab == 2){
            //Historico
            $scope.dataLoading = false;
            var userLogged = $rootScope.user.Domain + '\\' + $rootScope.user.Login;
            var reportType = 1;
            var onlyActive = false;

            GetOilSupplyHistory(userLogged, $scope.oilFactory.idFactory, $scope.oilArea.idArea, $scope.oilEquipment.idEquipment, $scope.oilComponent.idComponent, startDate, endDate,reportType, onlyActive);
            $scope.viewLoaded = true;
        }

        if(tab == 3){
            //Informações básicas
            $scope.dataLoading = false;
            var userLogged = $rootScope.user.Domain + '\\' + $rootScope.user.Login;
            var reportType = 1;
            var onlyActive = false;

            GetComponentsSettings(userLogged, $scope.oilFactory.idFactory, $scope.oilArea.idArea, $scope.oilEquipment.idEquipment, $scope.oilComponent.idComponent, reportType, onlyActive);
            $scope.viewLoaded = true;
        }
    };

    $scope.newComponent = function() {

        $ctrl.action = 'Insert';
        $ctrl.title = 'Adicionar Unidade Hidráulica';
        $ctrl.object = 'component';
        $ctrl.equipment = $scope.oilEquipmentsAll.filter(function(e){ if (e.idEquipment === $scope.oilEquipment.idEquipment) return true ; else return false; })[0];
        $ctrl.equipmentsAll = $scope.oilEquipmentsAll;
        $ctrl.component = null;
        $ctrl.componentsAll = null;
        $ctrl.oilSupply = null;
        $ctrl.componentsService = $scope.componentsService;
        $ctrl.user = $rootScope.user;
        
        oilManagementFactory.GetNewEmptyComponent($scope.oilEquipment.idEquipment).success(function (response){
    
            if(response){
                $ctrl.model = response;

                var modalInstance = $uibModal.open({
                    animation: $ctrl.animationsEnabled,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    templateUrl: 'componentContent.html',
                    controller: 'modalInstanceController2',
                    controllerAs: '$ctrl',
                    resolve: {
                        item: function () {
                        return $ctrl;
                        }
                    }               
                });
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

    $scope.setSelectedRow = function (idSelectedRow) {
        
        $scope.idSelectedRow = idSelectedRow;
        $scope.isSelectedRow = true;

        if ($rootScope.user != null)
        {
            if ($rootScope.user.OilManagement) {
                $scope.stillHiddenOilSupplyEdit = false;
            }
            else if ($rootScope.user.OilUser) {
                $scope.stillHiddenOilSupplyEdit = false;
            }
            else {
                $scope.stillHiddenOilSupplyEdit = true;
            }
        }
        else {
            $scope.stillHiddenOilSupplyEdit = true;
        }
    };

    $scope.editComponent = function() {

        $scope.Component.ModificatedBy = $rootScope.user.Login;
        var editComponent = angular.copy($scope.Component);
        editComponent.EnabledOilTypes = $scope.Component.OilTypes.filter(function(ot){ if ($scope.Component.EnabledOilTypes.indexOf(ot.idOilType) >= 0) return true; else false; });

        oilManagementFactory.EditComponent(editComponent).success(function (response){
    
            if(response){
                alert(response.message);
                $scope.saveDisabled = true;
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

    $scope.newOilSupply = function() {

        if ($scope.oilComponent === null || $scope.oilComponent === undefined)
        {
            alert('Não foi selecionada a Unidade Hidráulica!');
            return false;
        }

        if ($scope.Component.Active === false)
        {
            alert('Não é possivel cadastrar um abastecimento para uma Unidade Hidráulica inativa.\r\n Favor, verificar status da Unidade Hidráulica ' + $scope.oilComponent.Name + '.');
            return false;
        }

        $ctrl.action = 'Insert';
        $ctrl.title = 'Novo Abastecimiento';
        $ctrl.object = 'supply';
        $ctrl.equipment = null;
        $ctrl.equipmentAll = null;
        $ctrl.component = $scope.oilComponentsAll.filter(function(c){ if (c.idComponent === $scope.oilComponent.idComponent) return true ; else return false; })[0];
        $ctrl.componentsAll = $scope.oilComponentsAll;
        $ctrl.oilSupply = null;
        $ctrl.suppliesService = $scope.suppliesService;
        $ctrl.user = $rootScope.user;

        oilManagementFactory.GetNewEmptyOilSupply($scope.oilComponent.idComponent).success(function (response){
    
            if(response){
                $ctrl.model = response;

                var modalInstance = $uibModal.open({
                    animation: $ctrl.animationsEnabled,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    templateUrl: 'oilSupplyContent.html',
                    controller: 'modalInstanceController2',
                    controllerAs: '$ctrl',
                    resolve: {
                        item: function () {
                        return $ctrl;
                        }
                    }               
                });
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

    $scope.editOilSupply = function(idOilSupply) {

        oilManagementFactory.GetOilSupply(idOilSupply, $rootScope.user.Login, $rootScope.user.OilManagement, $rootScope.user.OilUser).success(function (response){
    
            if(response){
                
                if (response.ViewEnabled) {

                    $ctrl.action = 'Edit';
                    $ctrl.object = 'supply';
                    $ctrl.title = 'Editar Abastecimiento';                    
                    $ctrl.equipment = null;
                    $ctrl.equipmentAll = null;
                    $ctrl.component = $scope.oilComponentsAll.filter(function(c){ if (c.idComponent === $scope.oilComponent.idComponent) return true ; else return false; })[0];
                    $ctrl.componentsAll = $scope.oilComponentsAll;
                    $ctrl.oilSupply = response;
                    $ctrl.suppliesService = $scope.suppliesService;
                    $ctrl.user = $rootScope.user;
    
                    $ctrl.model = response;
    
                    var modalInstance = $uibModal.open({
                        animation: $ctrl.animationsEnabled,
                        ariaLabelledBy: 'modal-title',
                        ariaDescribedBy: 'modal-body',
                        templateUrl: 'oilSupplyContent.html',
                        controller: 'modalInstanceController2',
                        controllerAs: '$ctrl',
                        resolve: {
                            item: function () {
                            return $ctrl;
                            }
                        }               
                    });
    
                }
                //ViewEnabled = false
                else {
                    alert('O Usuário não tem permissão para visualizar este Abastecimento!');
                }                
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

    $scope.checkChange = function() {

        if ($rootScope.user != null)
        {
            if ($rootScope.user.OilManagement) {
                $scope.saveDisabled = false;
            }
            else if ($rootScope.user.OilUser) {
                $scope.saveDisabled = false;;
            }
            else {
                $scope.saveDisabled = true;;
            }
        }
        else {
            $scope.saveDisabled = true;
        }
    }

    $scope.checkDisabledAlterOilSupply = function() {
        
        if ($rootScope.user != null)
        {
            if ($scope.idSelectedRow === null || $scope.idSelectedRow === undefined) {

                if ($rootScope.user.OilManagement) {
                    return false;
                }
                else if ($rootScope.user.OilUser) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        else {
            return true;
        }
    }

    function GetOilSupplyHistory(userCode, idFactory, idArea, idEquipment, idComponent, startDate, endDate, reportType, onlyActives){

        $scope.idSelectedRowHistoric = null;
        $scope.isSelectedRowHistoric = false;
        $scope.XLfilters = { list: [], dict: {}, results: [] };

        oilManagementFactory.GetOilSupplyHistory(userCode, idFactory, idArea, idEquipment, idComponent, reportType, onlyActives, startDate, endDate, $rootScope.user.OilManagement, $rootScope.user.OilUser)
        .success(function (response){
    
            if(response)
            {   
                $scope.oilSupplyHistory = response;
                $scope.totalItemsTab2 = response.filters.length;
                $scope.currentPageTab2 = 1;
                $scope.itemsPerPageTab2 = $scope.viewbyTab2;
                $scope.itemPageStartTab2 = ((($scope.currentPageTab2-1)*$scope.itemsPerPageTab2)+1);            
                $scope.itemPageEndTab2 = $scope.itemsPerPageTab2 * $scope.currentPageTab2;                        
                $scope.maxSize = 4; //Number of pager buttons to show

                //$scope.XLfilters = { list: [], dict: {}, results: [] };
                $scope.filters = angular.copy(response.filters);
                $scope.filterColumns = angular.copy(response.filterColumns);
                createXLfilters(response.filters, response.filterColumns);

                $scope.dataLoading = true;
                $scope.TryXLfiltrate();
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

    function GetComponentsSettings(userCode, idFactory, idArea, idEquipment, idComponent, reportType, onlyActives){

        $scope.XLfilters = { list: [], dict: {}, results: [] };

        oilManagementFactory.GetComponentsSettings(userCode, idFactory, idArea, idEquipment, idComponent, reportType, onlyActives, $rootScope.user.OilManagement, $rootScope.user.OilUser)
        .success(function (response){
    
            if(response)
            {   
                $scope.oilComponentSettings = response;
                $scope.totalItemsTab3 = response.filters.length;
                $scope.currentPageTab3 = 1;
                $scope.itemsPerPageTab3 = $scope.viewbyTab3;
                $scope.itemPageStartTab3 = ((($scope.currentPageTab3-1)*$scope.itemsPerPageTab3)+1);            
                $scope.itemPageEndTab3 = $scope.itemsPerPageTab3 * $scope.currentPageTab3;                        
                $scope.maxSize = 4; //Number of pager buttons to show

                //$scope.XLfilters = { list: [], dict: {}, results: [] };
                $scope.filters = angular.copy(response.filters);
                $scope.filterColumns = angular.copy(response.filterColumns);
                createXLfilters(response.filters, response.filterColumns);

                $scope.dataLoading = true;
                $scope.TryXLfiltrate();
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
});    
