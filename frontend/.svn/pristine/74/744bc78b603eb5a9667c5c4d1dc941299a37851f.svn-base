app.controller("configController", function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window, treeConfig, $uibModal, $compile, equipmentsService, equipmentsFactory, areasFactory, typesFactory, partsFactory, equipmentsBackupFactory){
    
    var $ctrl = { 
        action: null,
        model: null,
        title: null
    };

    $scope.selectBackup = null;


    GetBackups();


    $scope.setRestore = function (model){
       
        angular.forEach($scope.backups, function(obj, key){
            obj.cls = "";
        });
       
       model.cls = "fa fa-check";
       $scope.selectBackup = model;
    };


    $scope.confirmRestoreTree = function(model){
        
        if(model){
            $.smallBox({
                title : "Restaurar!",
                content : "Deseja restaurar a árvore do dia "+$filter('date')(model.dateInput, "dd/MM/yyyy HH:mm:ss")+" ? <p class='text-align-right'><a id='btnSim' class='btn btn-primary btn-sm'>Sim</a> <a href='javascript:void(0);' class='btn btn-primary btn-sm'>Não</a></p>",            
                color : "#3276B1",
                //timeout: 8000,
                icon : "fa fa-warning swing animated" },
                function(e){

                    if(e.target.id == 'btnSim'){
                        $scope.restoreTree(model);
                    }
                }
            );
        }
        else{
            $.bigBox({
                title : "Erro!",
                content : "Não existe backup selecionado !",            
                color : "#C46A69",            
                icon : "fa fa-warning swing animated",
                timeout : 6000 });
        }

    }

    $scope.restoreTree = function (model){

        equipmentsBackupFactory.Restore(model.id).success(function (response){
            
                if(response.status){
                
                    GetEquipmentsByStatus(true);

                    $.bigBox({
                        title : "Sucesso",
                        content : "Restauração realizada com sucesso !",
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
         
    };


    $scope.confirmBackupTree = function(){
        
        $.smallBox({
            title : "Backup!",
            content : "Deseja fazer backup da árvore atual ? <p class='text-align-right'><a id='btnSim' class='btn btn-primary btn-sm'>Sim</a> <a href='javascript:void(0);' class='btn btn-primary btn-sm'>Não</a></p>",            
            color : "#3276B1",
            //timeout: 8000,
            icon : "fa fa-warning swing animated" },
            function(e){

                if(e.target.id == 'btnSim'){
                    $scope.backupTree();
                }
            }
        );

    }

    $scope.backupTree = function (){
        
        equipmentsBackupFactory.Backup().success(function (response){
            
                if(response.status){
                
                    GetBackups();

                    $.bigBox({
                        title : "Sucesso",
                        content : "Backup realizado com sucesso !",
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
                 
    };
        

    $scope.service = equipmentsService;

    $scope.selectStatus = false;

    GetEquipmentsByStatus(true);
   

    treeConfig.defaultCollapsed = true;

    GetAreas();
    GetTypes();
    GetParts();


    $scope.expandedChilds = function (scope) {
              
        GetEquipmentsChilds(scope.$modelValue);
               
        scope.toggle();
    };


    $scope.checkedStatus = function (scope) {
         
                
        GetEquipmentsByStatus(scope.selectStatus ? null : true)

    };


    $scope.editItem = function(scope){       
        
       scope.$modelValue.cdUser = $rootScope.user.Login;

       $ctrl.action = 'Edit';
       $ctrl.title = 'Edição de Dados';
       $ctrl.model = scope.$modelValue;
       $ctrl.areas = scope.areas;
       $ctrl.parts = scope.parts;
       $ctrl.types = scope.types;
       $ctrl.service = $scope.service;
       $ctrl.equipmentsAll = $scope.equipmentsAll;
       $ctrl.equips = $scope.equipmentsAll.filter(function(f) { return f.idEquipment != scope.$modelValue.idEquipment});
       $ctrl.equipsRoot = null;

      var modalInstance = $uibModal.open({
        animation: $ctrl.animationsEnabled,
        ariaLabelledBy: 'modal-title',
        ariaDescribedBy: 'modal-body',
        templateUrl: 'modalContent.html',
        controller: 'modalInstanceController',
        controllerAs: '$ctrl',
        resolve: {
            item: function () {
              return $ctrl;
            }
        }               
      });     
    }

    $scope.newItem = function(scope){       
        
        scope.$modelValue.cdUser = $rootScope.user.Login;

        $ctrl.action = 'Insert';
        $ctrl.title = 'Inserção de Dados';
        $ctrl.model = scope.$modelValue;       
        $ctrl.areas = scope.areas;
        $ctrl.parts = scope.parts;
        $ctrl.types = scope.types;       
        $ctrl.service = $scope.service;    
        $ctrl.equips = $scope.equipmentsAll;
        $ctrl.equipmentsAll = $scope.equipmentsAll;            
        $ctrl.equipsRoot = null;

            
          var modalInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'modalContent.html',
            controller: 'modalInstanceController',
            controllerAs: '$ctrl',
            resolve: {
                item: function () {
                  return $ctrl;
                }
            }               
          });     
    }
    
    $scope.newRoot= function(){       
        
        $ctrl.action = 'Insert';
        $ctrl.title = 'Inserção de Dados';
        $ctrl.model = {idEquipment: null, CountChilds:0, cdEquipment: null, EquipmentsChilds:[], Active: true, cdIntegrate: false, cdUser: $rootScope.user.Login};        
        $ctrl.areas = $scope.areas;
        $ctrl.parts = $scope.parts;
        $ctrl.types = $scope.types;        
        $ctrl.service = $scope.service;
        $ctrl.equips = $scope.equipmentsAll;
        $ctrl.equipmentsAll = $scope.equipmentsAll;
        $ctrl.equipsRoot = $scope.equipsConfig;
        
        
                    
          var modalInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'modalContent.html',
            controller: 'modalInstanceController',
            controllerAs: '$ctrl',
            resolve: {
                item: function () {
                  return $ctrl;
                }
            }               
          });     
    }
    
    $scope.confirmRemove = function(scope){
        
        $.smallBox({
            title : "Exclusão!",
            content : "Deseja excluir o registro selecionado ? <p class='text-align-right'><a id='btnSim' class='btn btn-danger btn-sm'>Sim</a> <a href='javascript:void(0);' class='btn btn-danger btn-sm'>Não</a></p>",            
            color : "#C46A69",
            //timeout: 8000,
            icon : "fa fa-warning swing animated" },
            function(e){

                if(e.target.id == 'btnSim'){
                    $scope.removeItem(scope);
                }
            }
        );

    }

    $scope.confirmRestore = function(scope){
        
        $.smallBox({
            title : "Restaurar!",
            content : "Deseja restaurar o registro selecionado ? <p class='text-align-right'><a id='btnSim' class='btn btn-primary btn-sm'>Sim</a> <a href='javascript:void(0);' class='btn btn-primary btn-sm'>Não</a></p>",            
            color : "#3276B1",
            //timeout: 8000,
            icon : "fa fa-warning swing animated" },
            function(e){

                if(e.target.id == 'btnSim'){
                    $scope.restoreItem(scope);
                }
            }
        );

    }


    $scope.removeItem = function(scope){                  
        
        scope.$modelValue.Active = false;
        scope.$modelValue.cdUser = $rootScope.user.Login;
        equipmentsFactory.UpdateEquipment(scope.$modelValue).success(function (response){            
            if(response.status){  
                
                if (!$scope.selectStatus){
               
                    scope.remove();                   
                }

                $scope.service.setEquipments($scope.equipmentsAll);
                
                if(scope.$parentNodeScope){                         
                    scope.$parentNodeScope.$modelValue.CountChilds = scope.$parentNodeScope.$modelValue.EquipmentsChilds.length;
                }                                
                
               

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


    $scope.restoreItem = function(scope){                  
        
        scope.$modelValue.Active = true;
        equipmentsFactory.UpdateEquipment(scope.$modelValue).success(function (response){            
            if(response.status){            
                
                $scope.service.setEquipments($scope.equipmentsAll);
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


    $scope.treeEquips = {
        accept: function (sourceNodeScope, destNodesScope, destIndex) {

            // if (sourceNodeScope && destNodesScope){

            //     if(destNodesScope.$parent.$modelValue)
            //         $scope.expandedChilds(destNodesScope.$parent);
            // }

            return true;
        },
        dropped: function (e) {
            var sourceNodeData = e.source.nodeScope;
            var destNodeDataParent = e.dest.nodesScope.$parent;

            if(sourceNodeData){
                sourceNodeData.$modelValue.cdEquipment = destNodeDataParent.$modelValue ? destNodeDataParent.$modelValue.idEquipment : null;
                sourceNodeData.$modelValue.cdUser = $rootScope.user.Login;
                equipmentsFactory.UpdateEquipment(sourceNodeData.$modelValue).success(function (response){

                    if(response.status)
                        $scope.service.setEquipments($scope.equipmentsAll);
                    else                                
                        $.bigBox({
                            title : "Erro!",
                            content : response.message,            
                            color : "#C46A69",            
                            icon : "fa fa-warning swing animated",
                            timeout : 6000 });
                            
                }).error(function(error){                  
                    $.bigBox({
                        title : "Erro!",
                        content : error ? error.Message : "Falha de comunicação com o servidor.",            
                        color : "#C46A69",              
                        icon : "fa fa-warning swing animated",              
                        timeout : 6000 });          
                });    
            }
                
        }
    }
        
  
    function GetEquipmentsByStatus(status){
                            

            equipmentsFactory.GetAllEquipments(status).success(function (response){
        
                    if(response.length > 0){
            
                        $scope.equipmentsAll = response;

                        if(status)
                            $scope.service.setEquipments(response);
                        
                        $scope.equipsConfig = $scope.equipmentsAll.filter(function(f){  return f.cdEquipment === null});                                
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
    


    function GetEquipmentsChilds(scope){
        
       scope.EquipmentsChilds = $scope.equipmentsAll.filter(function(f) { return f.cdEquipment === scope.idEquipment});
                            
    }

  

    function GetAreas(){
                     
        areasFactory.GetAreas().success(function (response){    
                
            if(response.length > 0){        
                    $scope.areas = response;                   
                                                    
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
    

    function GetTypes(){
        
        typesFactory.GetTypes().success(function (response){    
        
        if(response.length > 0){        
            $scope.types = response;                                                               
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

    function GetParts(){
        
        partsFactory.GetParts().success(function (response){    
        
        if(response.length > 0){        
            $scope.parts = response;                                                               
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
       


    function GetBackups(){
        

        equipmentsBackupFactory.GetBackups().success(function (response){

        if(response.length > 0){
            $scope.backups = response;                                         
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

})