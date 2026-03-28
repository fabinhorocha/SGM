app.controller('budgetController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, commonService, plantFactory, locationFactory, budgetFactory, budgetLocationFactory, budgetPlantFactory, userFactory) {

    
    $scope.Action = 'Insert';    
    $scope.goalviewby = "5";  
    $scope.budgetviewby = "5";       
    $scope.locations = [];
    $scope.plants = [];
        

    $scope.FillTabsBudget = function (tab){
        $scope.selTabBudget = tab;
        $scope.tabsBudget = [
            {id: 1, name:' Dados',  class: 'fa fa-fw fa-lg fa-tasks'},
            {id: 2, name:' Consulta',  class: 'fa fa-fw fa-lg fa-info'}            
        ];
    };

    $scope.setTabBudget = function(tab){
        
        $scope.selTabBudget = tab;                      
        
        if(tab == 2)                   
            GetBudgets(null);                                                                     
    };
    
    $scope.FillTabsBudget(1);

    newBudget();      
      
    
    $scope.saveBudget = function(model){

        model.dateStart = model.dateStart ? commonService.TryGetDateFromValue(model.dateStart, 2, 1, 0, '/') : null;
        model.dateEnd = model.dateEnd ? commonService.TryGetDateFromValue(model.dateEnd, 2, 1, 0, '/') : null;  
        model.budgetLocations = [];

        angular.forEach(model.budgetLocationsPrev, function(obj, key){
            model.budgetLocations.push(obj);
        });

        angular.forEach(model.budgetLocationsPred, function(obj, key){
            model.budgetLocations.push(obj);
        });
    
        model.cdUser = $rootScope.user.Login;

        if ($scope.Action == 'Insert'){
            budgetFactory.InsertBudget(model).success(function(response){

                if(response.status){

                    newBudget();

                    $scope.selTabBudget = 1;

                    $.bigBox({
                        title : "Sucesso",
                        content : "Budget lançado com sucesso !",
                        color : "#739E73",
                        timeout: 6000,
                        icon : "fa fa-check",
                        //number : "4"
                    });
                }
                else{
                    
                                            
                    $.bigBox({
                        title : "Erro!",
                        content : response.message,            
                        color : "#C46A69",            
                        icon : "fa fa-warning swing animated",
                        timeout : 6000 });
                    
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
        else{
            budgetFactory.UpdateBudget(model).success(function(response){
                
                                if(response.status){
                
                                    newBudget();
                
                                    $scope.selTabBudget = 1;
                
                                    $.bigBox({
                                        title : "Sucesso",
                                        content : "Budget atualizado com sucesso !",
                                        color : "#739E73",
                                        timeout: 6000,
                                        icon : "fa fa-check",
                                        //number : "4"
                                    });
                                }
                                else{
                                    
                                
                                    $.bigBox({
                                        title : "Erro!",
                                        content : response.message,            
                                        color : "#C46A69",            
                                        icon : "fa fa-warning swing animated",
                                        timeout : 6000 });
                                    
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

    };

    $scope.setPageGoal = function (pageNo) {
        $scope.goalcurrentPage = pageNo;
    };

    $scope.setPageBudget = function (pageNo) {
        $scope.budgetcurrentPage = pageNo;
    };
            
    $scope.setItemsPerPageBudget = function(num) {
        $scope.budgetitemsPerPage = num;
        $scope.budgetcurrentPage = 1; //reset to first page

        $scope.budgetitemPageStart = ((($scope.budgetcurrentPage-1)*$scope.budgetitemsPerPage)+1);            
        $scope.budgetitemPageEnd = $scope.budgetitemsPerPage * $scope.budgetcurrentPage;            
    }

    $scope.setItemsPerPageGoal = function(num) {
        $scope.goaltitemsPerPage = num;
        $scope.goalcurrentPage = 1; //reset to first page

        $scope.goalitemPageStart = ((($scope.goalcurrentPage-1)*$scope.goalitemsPerPage)+1);            
        $scope.goalitemPageEnd = $scope.goalitemsPerPage * $scope.goalcurrentPage;            
    }

    $scope.pageChanged = function() {
        //console.log('Page changed to: ' + $scope.currentPage);
    };

    $scope.editBudget = function(model) {

        GetBudget(model.idBudget);
      
    };

    $scope.updateBudgetStatus = function(model) {
        
        model.cdUser = $rootScope.user.Login;

        budgetFactory.UpdateBudgetStatus(model).success(function(response){
                                       
            if(!response.status){                                  
                model.Active = false;
                $.bigBox({
                    title : "Erro!",
                    content : response.message,            
                    color : "#C46A69",            
                    icon : "fa fa-warning swing animated",
                    timeout : 6000 });
                
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
   
    $scope.newBudget = function(model) {
        newBudget();
    };

    function newBudget (){        

        $scope.Action = 'Insert';  

        $scope.Budget= {
            idBudget: null,
            dateStart: null,
            dateEnd: null,           
            cdUser: null,            
            Active: true,
            budgetLocations: [],
            budgetLocationsPred: [],
            budgetLocationsPrev: [],
            budgetPlants: [],
            budgetPlantsPredictive: []
        };                
    
        GetLocations();
        GetPlants();
    };

    function GetLocations(){
        
        locationFactory.GetLocationsPlants().success(function (response){    

        if(response.length > 0){  

            $scope.locations = response;   
            
            angular.forEach($scope.locations, function(obj, key){

                var budgetLocation = {
                    idBudgetLocation: null,                  
                    cdBudget: null,                    
                    cdLocationPlant: obj.idLocationPlant,                    
                    vlGoal: null,                                                                                                  
                    Active: true,
                    LocationPlant: obj
                };

               if(obj.Plant.Sheet == 'CF9' || obj.Plant.Sheet == 'SR9')
                    $scope.Budget.budgetLocationsPred.push(budgetLocation);
               else
                    $scope.Budget.budgetLocationsPrev.push(budgetLocation);
            });


            $scope.goaltotalItemsPred = $scope.Budget.budgetLocationsPred.length;

            $scope.goaltotalItemsPrev = $scope.Budget.budgetLocationsPrev.length;

            $scope.goalcurrentPage = 1;
            $scope.goalitemsPerPage = $scope.goalviewby;
            $scope.goalitemPageStart = ((($scope.goalcurrentPage-1)*$scope.goalitemsPerPage)+1);            
            $scope.goalitemPageEnd = $scope.goalitemsPerPage * $scope.goalcurrentPage;                        
            $scope.goalmaxSize = 4; //Number of pager            

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
   

    function GetPlants(){
        
        plantFactory.GetPlants().success(function (response){    

        if(response.length > 0){  

            $scope.plants = response;   
            
            angular.forEach($scope.plants, function(obj, key){

                var budgetPlant = {
                    idBudgetPlant: null,                  
                    cdBudget: null,                    
                    cdPlant: obj.idPlant,                    
                    vlGoal: null,                                                                                                  
                    Active: true,
                    Plant: obj,
                    cdPredictive: null
                };
                
               budgetPlant.cdPredictive = false;
               $scope.Budget.budgetPlants.push(budgetPlant);

               budgetPlant.cdPredictive = true;
               $scope.Budget.budgetPlantsPredictive.push(budgetPlant);
            });


            $scope.goaltotalItems = $scope.Budget.budgetPlants.length;
            $scope.goalcurrentPage = 1;
            $scope.goalitemsPerPage = $scope.goalviewby;
            $scope.goalitemPageStart = ((($scope.goalcurrentPage-1)*$scope.goalitemsPerPage)+1);            
            $scope.goalitemPageEnd = $scope.goalitemsPerPage * $scope.goalcurrentPage;                        
            $scope.goalmaxSize = 4; //Number of pager            

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


    function GetBudgets(active){
        
        budgetFactory.GetBudgets(active).success(function (response){    

        if(response.length > 0){        
            $scope.budgets = response;       
            
            $scope.budgettotalItems = $scope.budgets.length;
            $scope.budgetcurrentPage = 1;
            $scope.budgetitemsPerPage = $scope.budgetviewby;
            $scope.budgetitemPageStart = ((($scope.budgetcurrentPage-1)*$scope.budgetitemsPerPage)+1);            
            $scope.budgetitemPageEnd = $scope.budgetitemsPerPage * $scope.budgetcurrentPage;                        
            $scope.budgetmaxSize = 4; //Number of pager buttons to show
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

    function GetBudget(idBudget){
        
        budgetFactory.GetBudget(idBudget).success(function (response){    

            if(response){        
                $scope.Budget = response;    
                $scope.Budget.budgetLocationsPred = response.budgetLocations.filter(function(f){return f.LocationPlant.Plant.Sheet === 'CF9'|| f.LocationPlant.Plant.Sheet === 'SR9'}),
                $scope.Budget.budgetLocationsPrev = response.budgetLocations.filter(function(f){return f.LocationPlant.Plant.Sheet != 'CF9' && f.LocationPlant.Plant.Sheet != 'SR9'}), 
                $scope.Budget.dateStart =  $filter('date')($scope.Budget.dateStart,'dd/MM/yyyy');                                                 
                $scope.Budget.dateEnd = $filter('date')($scope.Budget.dateEnd,'dd/MM/yyyy');                   
                $scope.Budget.cdUser = $rootScope.user.Login;      
                
                $scope.Action = 'Update';  
                $scope.setTabBudget(1);
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



