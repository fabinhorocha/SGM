app.controller('oilManagementAlarmsController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, userFactory, oilManagementFactory, commonService, oilSupplyService, oilComponentsService) {

    var initialDate = new Date();
    initialDate.setDate(new Date().getDate() - 30);
    $scope.startDate = $filter('date')(initialDate, 'dd/MM/yyyy');
    $scope.endDate = $filter('date')(new Date(),'dd/MM/yyyy');

    $scope.stillHiddenAlarmEdit = true;

    $scope.reports = undefined;

    $scope.dataLoading = false;
    $scope.idSelectedRow = null;
    $scope.isSelectedRow = false;

    $scope.selTab = 1;
    $scope.viewbyTab = "25";
    $scope.viewbyTab2 = "25";
    $scope.viewLoaded = false;
    
    $scope.fileName = "gestaoOleoAlarmes";
    $scope.exportData = [];

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

    var $ctrl = { 
        action: null,
        model: null,
        title: null,
        object : 'alarm',
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

    // $scope.suppliesService = oilSupplyService;
    // $scope.componentsService = oilComponentsService;

    // $scope.$watch('suppliesService.getLastOilSupplies()', function(){
    //     $scope.reports = $scope.suppliesService.getLastOilSupplies();
    //     },true
    // );

    // $scope.$watch('componentsService.getComponents()', function(){
        
    //     $scope.oilComponentsAll = $scope.componentsService.getComponents();

    //     $scope.oilComponents = [];
    //     $scope.oilComponents.push($scope.emptyOption);
    //     var newIdComponent = -1;
    //     var idx = 0;
    //     var idxNewComponent = -1;
    //     angular.forEach($scope.oilComponentsAll, function (obj, key) {
    //         idx++;
    //         if (obj.idComponent > newIdComponent){
    //             newIdComponent = obj.idComponent;
    //             idxNewComponent = idx;    
    //         } 
    //         $scope.oilComponents.push(obj);
    //     });

    //     if (idxNewComponent > -1) {
    //         $scope.oilComponent = $scope.oilComponents[idxNewComponent];
    //         $scope.LoadTabInfo($scope.selTab);
    //         $scope.stillHidden = $rootScope.user.OilManagement ? false : true;
    //         $scope.stillHiddenAlarmEdit = true;
    //         $scope.viewLoaded = true;
    //     }

    //     },true
    // );

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

            if (newValue === null || newValue === undefined)
                return;
                
            if ($scope.selTab === 1) {

                $scope.totalItemsTab = newValue.length;
                $scope.currentPageTab = 1;
                $scope.itemPageStartTab = ((($scope.currentPageTab-1)*$scope.itemsPerPageTab)+1);            
                $scope.itemPageEndTab = $scope.itemsPerPageTab * $scope.currentPageTab;
                
                $scope.exportData = [];
    
                $scope.exportData.push($scope.alarmsReport.columnsToExport);
    
                angular.forEach(newValue, function(value, key) {
                    $scope.exportData.push([
                                value.factoryName,
                                value.areaName,
                                value.equipmentName,
                                value.componentName,
                                value.alarmTypeName,
                                value.CreatedBy,
                                value.InsDateTime,
                                value.ModificatedBy,
                                value.UpdDateTime]);
                });        
            }
            else if ($scope.selTab === 2) {

                $scope.totalItemsTab2 = newValue.length;
                $scope.currentPageTab2 = 1;
                $scope.itemPageStartTab2 = ((($scope.currentPageTab2-1)*$scope.itemsPerPageTab2)+1);            
                $scope.itemPageEndTab2 = $scope.itemsPerPageTab2 * $scope.currentPageTab2;
                
                $scope.exportData = [];
    
                $scope.exportData.push($scope.alarmsReport.columnsToExport);
    
                angular.forEach(newValue, function(value, key) {
                    $scope.exportData.push([
                                value.alarmTypeName,
                                value.factoryName,
                                value.areaName,
                                value.equipmentName,
                                value.componentName,
                                value.InsDateTime,
                                value.Quantity,
                                value.SupplyDateTime]);
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

    function GetAlarms(userCode, idFactory, idArea, idEquipment, idComponent, onlyActives){
     
        $scope.idSelectedRow = null;
        $scope.isSelectedRow = false;
        $scope.XLfilters = { list: [], dict: {}, results: [] };

        oilManagementFactory.GetAlarms(userCode, idFactory, idArea, idEquipment, idComponent, onlyActives, $rootScope.user.OilManagement, $rootScope.user.OilUser).success(function (response){
    
            if(response)
            {   
                $scope.alarmsReport = response;
                $scope.totalItemsTab = response.filters.length;
                $scope.currentPageTab = 1;
                $scope.itemsPerPageTab = $scope.viewbyTab;
                $scope.itemPageStartTab = ((($scope.currentPageTab-1)*$scope.itemsPerPageTab)+1);            
                $scope.itemPageEndTab = $scope.itemsPerPageTab * $scope.currentPageTab;
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

    function GetAlarmsHistory(userCode, idFactory, idArea, idEquipment, idComponent, startDate, endDate, onlyActives){

        $scope.XLfilters = { list: [], dict: {}, results: [] };

        oilManagementFactory.GetAlarmsHistory(userCode, idFactory, idArea, idEquipment, idComponent, startDate, endDate, onlyActives, $rootScope.user.OilManagement, $rootScope.user.OilUser)
        .success(function (response){
    
            if(response)
            {   
                $scope.alarmsHistory = response;
                $scope.totalItemsTab2 = response.filters.length;
                $scope.currentPageTab2 = 1;
                $scope.itemsPerPageTab2 = $scope.viewbyTab2;
                $scope.itemPageStartTab2 = ((($scope.currentPageTab2-1)*$scope.itemsPerPageTab2)+1);            
                $scope.itemPageEndTab2 = $scope.itemsPerPageTab2 * $scope.currentPageTab2;                        
                $scope.maxSize = 4; //Number of pager buttons to show

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

        $scope.stillHiddenAlarmEdit = true;
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
            $scope.stillHiddenAlarmEdit = true;

            if ($rootScope.user != null && $rootScope.user.OilManagement){

                // if ($scope.selTab == 2)
                //     $scope.selTab = 1;

                $scope.tabsRequestView = [
                    {id: 1, name:'Alarmes',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                    {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
                ];
                        
                $scope.dataLoading = false;
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
            $scope.stillHiddenAlarmEdit = true;

            if ($rootScope.user != null && $rootScope.user.OilManagement){

                // if ($scope.selTab == 2)
                //     $scope.selTab = 1;

                $scope.tabsRequestView = [
                    {id: 1, name:'Alarmes',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                    {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
                ];
                        
                $scope.dataLoading = false;
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
            $scope.stillHiddenAlarmEdit = true;

            if ($rootScope.user != null && $rootScope.user.OilManagement){

                // if ($scope.selTab == 2)
                //     $scope.selTab = 1;

                $scope.tabsRequestView = [
                    {id: 1, name:'Alarmes',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                    {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
                ];

                $scope.dataLoading = false;
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
            if ($rootScope.user != null && $rootScope.user.OilManagement)
                hideTab = false;

            $scope.tabsRequestView = [
                {id: 1, name:'Alarmes',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
            ];

            $scope.dataLoading = false;
            $scope.LoadTabInfo($scope.selTab);

            $scope.stillHiddenAlarmEdit = $rootScope.user.OilManagement ? false : true;
        }
    };

    $scope.setItemsPerPageTab = function(num) {
        
        $scope.itemsPerPageTab = num;
        $scope.currentPageTab = 1; //reset to first page

        $scope.itemPageStartTab = ((($scope.currentPageTab-1)*$scope.itemsPerPageTab)+1);            
        $scope.itemPageEndTab = $scope.itemsPerPageTab * $scope.currentPageTab;            
    }

    $scope.setItemsPerPageTab2 = function(num) {
        
        $scope.itemsPerPageTab2 = num;
        $scope.currentPageTab2 = 1; //reset to first page

        $scope.itemPageStartTab2 = ((($scope.currentPageTab2-1)*$scope.itemsPerPageTab2)+1);            
        $scope.itemPageEndTab2 = $scope.itemsPerPageTab2 * $scope.currentPageTab2;            
    }

    $scope.LoadTabInfo = function(tab){
        
        $scope.selTab = tab;
        var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
        var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');

        if(tab == 1){
            //Alarmes
            var userLogged = $rootScope.user.Domain + '\\' + $rootScope.user.Login;
            var onlyActives = false;

            GetAlarms(userLogged, $scope.oilFactory.idFactory, $scope.oilArea.idArea, $scope.oilEquipment.idEquipment, $scope.oilComponent.idComponent, onlyActives);
            $scope.viewLoaded = true;
        }
        else if(tab == 2){
            //Historico
            var userLogged = $rootScope.user.Domain + '\\' + $rootScope.user.Login;
            var onlyActives = false;

            GetAlarmsHistory(userLogged, $scope.oilFactory.idFactory, $scope.oilArea.idArea, $scope.oilEquipment.idEquipment, $scope.oilComponent.idComponent, startDate, endDate, onlyActives);
            $scope.viewLoaded = true;
        }
    };

    $scope.setSelectedRow = function (idSelectedRow) {
        
        $scope.idSelectedRow = idSelectedRow;
        $scope.isSelectedRow = true;

        if ($rootScope.user != null)
        {
            if ($rootScope.user.OilManagement) {
                $scope.stillHiddenAlarmEdit = false;
            }
            else if ($rootScope.user.OilUser) {
                $scope.stillHiddenAlarmEdit = false;
            }
            else {
                $scope.stillHiddenAlarmEdit = true;
            }
        }
        else {
            $scope.stillHiddenAlarmEdit = true;
        }
    };

    $scope.newAlarm = function() {

        if ($scope.oilComponent === null || $scope.oilComponent === undefined)
        {
            alert('Não foi selecionada a Unidade Hidráulica!');
            return false;
        }

        if ($scope.oilComponent.Active === false)
        {
            alert('Não é possivel cadastrar um alarme para uma Unidade Hidráulica inativa.\r\n Favor, verificar status da Unidade Hidráulica ' + $scope.oilComponent.Name + '.');
            return false;
        }

        $ctrl.action = 'Edit';
        $ctrl.title = 'Editar Alarmes';
        $ctrl.object = 'alarm';
        $ctrl.equipment = null;
        $ctrl.equipmentAll = null;
        $ctrl.component = $scope.oilComponentsAll.filter(function(c){ if (c.idComponent === $scope.oilComponent.idComponent) return true ; else return false; })[0];
        $ctrl.componentsAll = $scope.oilComponentsAll;
        $ctrl.user = $rootScope.user;

        oilManagementFactory.GetAlarmsByComponent($rootScope.user.Login, $scope.oilComponent.idComponent, $rootScope.user.OilManagement, $rootScope.user.OilUser).success(function (response){

            if(response){

                $ctrl.model = response;

                var modalInstance = $uibModal.open({
                    animation: $ctrl.animationsEnabled,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    templateUrl: 'alarmsContent.html',
                    controller: 'modalInstanceController2',
                    controllerAs: '$ctrl',
                    size: 'lg',
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

    $scope.checkChange = function() {

        if ($rootScope.user != null)
        {
            if ($rootScope.user.OilManagement) {
                $scope.saveDisabled = false;
            }
            else if ($rootScope.user.OilUser) {
                $scope.saveDisabled = true;;
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
                    return true;
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

});    
