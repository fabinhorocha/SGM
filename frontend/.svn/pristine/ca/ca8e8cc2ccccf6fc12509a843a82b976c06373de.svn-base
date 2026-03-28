app.controller('oilManagementAlarmsGroupController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, userFactory, oilManagementFactory, commonService, oilAlarmsService) {

    $scope.stillHiddenAlarmGroupEdit = true;

    $scope.dataLoading = false;
    $scope.idSelectedRow = null;
    $scope.isSelectedRow = false;

    $scope.selTab = 1;
    $scope.viewbyTab = "10";
    $scope.viewLoaded = false;
    
    // $scope.fileName = "gestaoOleoAlarmes";
    // $scope.exportData = [];

    var $ctrl = { 
        action: null,
        model: null,
        title: null,
        object : 'group',
        alarmsService : null,
        user : null
    };

    $scope.LoadTabInfo = function(tab){
        
        $scope.selTab = tab;
        if(tab == 1){
            //Alarmes
            var userLogged = $rootScope.user.Domain + '\\' + $rootScope.user.Login;
            var onlyActives = true;

            GetAlarmGroups(userLogged, onlyActives);
            $scope.viewLoaded = true;
        }
    };

    if ($rootScope.user === null || $rootScope.user === undefined){
        GetUser();
    }
    else{

        $scope.tabsRequestView = [
            {id: 1, name:'Grupos',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
        ];
        
        $scope.dataLoading = false;
        $scope.LoadTabInfo($scope.selTab);
    }

    $scope.alarmsService = oilAlarmsService;
    $scope.$watch('alarmsService.getAlarmGroupsAll()', function(){
        $scope.alarmGroupsReport = $scope.alarmsService.getAlarmGroupsAll();
       },true);

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

    // $scope.$watch('XLfilters.results', function (newValue, oldValue, scope) {
        
    //     if (oldValue.length != newValue.length) {

    //         if (newValue === null || newValue === undefined)
    //             return;
                
    //         if ($scope.selTab === 1) {

    //             $scope.totalItemsTab = newValue.length;
    //             $scope.currentPageTab = 1;
    //             $scope.itemPageStartTab = ((($scope.currentPageTab-1)*$scope.itemsPerPageTab)+1);            
    //             $scope.itemPageEndTab = $scope.itemsPerPageTab * $scope.currentPageTab;
                
    //             $scope.exportData = [];
    
    //             $scope.exportData.push($scope.alarmsReport.columnsToExport);
    //         }
    //     }
    // }, true);

    $scope.TryXLfiltrate = function() {
        if ($scope.dataLoading)
            $scope.XLfiltrate();
    }

    function GetUser(){
        
        userFactory.GetUser().success(function (response){
    
                if(response){
                    $rootScope.user = response;

                    $scope.tabsRequestView = [
                        {id: 1, name:'Grupos',  class: 'fa fa-fw fa-lg fa-tasks', hide: false }
                    ];
                    
                    $scope.dataLoading = false;
                    $scope.LoadTabInfo($scope.selTab);

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

    function GetAlarmGroups(userCode, onlyActives){
     
        $scope.idSelectedRow = null;
        $scope.isSelectedRow = false;
        $scope.XLfilters = { list: [], dict: {}, results: [] };

        oilManagementFactory.GetAlarmGroups(userCode, onlyActives, $rootScope.user.OilManagement, $rootScope.user.OilUser).success(function (response){
    
            if(response)
            {   
                $scope.alarmGroupsReport = response;
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

    $scope.setItemsPerPageTab = function(num) {
        
        $scope.itemsPerPageTab = num;
        $scope.currentPageTab = 1; //reset to first page

        $scope.itemPageStartTab = ((($scope.currentPageTab-1)*$scope.itemsPerPageTab)+1);            
        $scope.itemPageEndTab = $scope.itemsPerPageTab * $scope.currentPageTab;            
    }

    $scope.setSelectedRow = function (idSelectedRow) {
        
        $scope.idSelectedRow = idSelectedRow;
        $scope.isSelectedRow = true;

        $scope.stillHiddenAlarmGroupEdit = false;
        // if ($rootScope.user != null)
        // {
        //     if ($rootScope.user.OilManagement) {
        //         $scope.stillHiddenAlarmEdit = false;
        //     }
        //     else if ($rootScope.user.OilUser) {
        //         $scope.stillHiddenAlarmEdit = false;
        //     }
        //     else {
        //         $scope.stillHiddenAlarmEdit = true;
        //     }
        // }
        // else {
        //     $scope.stillHiddenAlarmEdit = true;
        // }
    };

    $scope.newAlarmGroup = function() {

        $ctrl.action = 'Insert';
        $ctrl.title = 'Criar Grupo';
        $ctrl.object = 'group';
        $ctrl.user = $rootScope.user;
        $ctrl.alarmsService = $scope.alarmsService;

        oilManagementFactory.GetNewEmptyAlarmGroup().success(function (response){
    
            if(response){
                $ctrl.model = response;

                var modalInstance = $uibModal.open({
                    animation: $ctrl.animationsEnabled,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    templateUrl: 'alarmGroupContent.html',
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

    $scope.editAlarmGroup = function() {

        $ctrl.action = 'Edit';
        $ctrl.title = 'Editar Grupo';
        $ctrl.object = 'group';
        $ctrl.user = $rootScope.user;
        $ctrl.alarmsService = $scope.alarmsService;

        var idAlarmGroup = $scope.idSelectedRow;
        var userCode = $rootScope.user.Domain + '\\' + $rootScope.user.Login;

        oilManagementFactory.GetAlarmGroup(userCode, idAlarmGroup, $rootScope.user.OilManagement, $rootScope.user.OilUser).success(function (response){
    
            if(response){

                $ctrl.model = response;

                var modalInstance = $uibModal.open({
                    animation: $ctrl.animationsEnabled,
                    ariaLabelledBy: 'modal-title',
                    ariaDescribedBy: 'modal-body',
                    templateUrl: 'alarmGroupContent.html',
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
});   