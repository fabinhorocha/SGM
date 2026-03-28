app.controller('modalInstanceController2', function ($uibModalInstance, $scope, $filter,$uibModal, item, commonService, oilManagementFactory) {   
  
    var $ctrl = this;  
        
    $ctrl.item = angular.copy(item);

    $ctrl.selected = {        
        OilType : undefined,
        SupplyDateTime : undefined,
        InsDateTime : undefined,
    };

    $ctrl.saveDisabled = true;

    if ($ctrl.item.action == 'Insert'){
        
        $ctrl.saveDisabled = false;
        $ctrl.item.model.CreatedBy = $ctrl.item.user.Login;

        //New component
        if ($ctrl.item.object === 'component')   {
            $ctrl.item.model.EnabledOilTypes = [];
            $ctrl.EnabledOilTypesIds = [];
        }
        //New oil supply
        else if($ctrl.item.object === 'supply') {

            $ctrl.item.model.EditionEnabled = true;
            $ctrl.item.model.ViewEnabled = true;
            $ctrl.item.model.SupplyDateTime = $filter('date')(new Date(),'dd/MM/yyyy');
            $ctrl.item.model.InsDateTime = $filter('date')(new Date(),'dd/MM/yyyy');
            $ctrl.FilteredOil = "";
            $ctrl.FilteredOilStatus = ["SIM", "NÃO"];
            $ctrl.SelectedShift = undefined;
            $ctrl.Shifts = [ { id : 1},{ id : 2}, { id : 3} ];
        }
        else if($ctrl.item.object === 'alarm') {
            alert('Operação não definida!!');
        }
        else if($ctrl.item.object === 'group') {
            $ctrl.item.model.EditionEnabled = true;
            $ctrl.UsersAllSelected = [];
            $ctrl.UsersSelected = [];
        }
    }
    else if ($ctrl.item.action == 'Edit'){
        if ($ctrl.item.object === 'component') {
            alert('Operação não definida!!');
        }
        else if ($ctrl.item.object === 'supply') {
            $ctrl.item.model.ModificatedBy = $ctrl.item.user.Login;
            $ctrl.item.model.SupplyDateTime = $filter('date')($ctrl.item.model.SupplyDateTime,"dd/MM/yyyy");
            $ctrl.item.model.InsDateTime = $filter('date')($ctrl.item.model.InsDateTime,"dd/MM/yyyy");
            $ctrl.item.model.OilType = $ctrl.item.model.Component.EnabledOilTypes.filter(function(ot){  return ot.idOilType === $ctrl.item.model.OilType.idOilType})[0];
            $ctrl.item.model.OilSupplyType = $ctrl.item.model.OilSupplyTypes.filter(function(ost){  return ost.idOilSupplyType === $ctrl.item.model.OilSupplyType.idOilSupplyType})[0];
            
            if ($ctrl.item.model.OilSupplyType.Name === 'Corretivo')
            {
                $ctrl.item.model.StoppageType = $ctrl.item.model.StoppageTypes.filter(function(st){  return st.idStoppageType === $ctrl.item.model.StoppageType.idStoppageType})[0];
            }
            else
            {
                $ctrl.item.model.StoppageTime = null;
            }
    
            $ctrl.Shifts = [ { id : 1},{ id : 2}, { id : 3} ];
            $ctrl.SelectedShift = $ctrl.Shifts[$ctrl.item.model.Shift - 1];
    
            $ctrl.FilteredOilStatus = ["SIM", "NÃO"];
            $ctrl.FilteredOil = $ctrl.item.model.IsReuseOil ? $ctrl.FilteredOilStatus[0] : $ctrl.FilteredOilStatus[1];
    
        }
        else if ($ctrl.item.object === 'alarm'){

            $ctrl.item.model.CreatedBy = $ctrl.item.user.Login;
            $ctrl.item.model.ModificatedBy = $ctrl.item.user.Login;
            $ctrl.item.model.EditionEnabled = true;

            var $ctrl2 = { 
                action: null,
                model: null,
                title: null,
                object : 'groupsByAlarm',
                alarmsService : null,
                user : null
            };
        
        }
        else if ($ctrl.item.object === 'group'){
            $ctrl.UsersAllSelected = [];
            $ctrl.UsersSelected = [];
        }
    }
  
    $ctrl.save = function (){

        if($ctrl.item.action == 'Insert')
        {
            if($ctrl.item.object === 'component')
                $ctrl.insertComponent();
            else if($ctrl.item.object === 'supply')
                $ctrl.insertSupply();
            else if($ctrl.item.object === 'alarm')
                alert('Operação não definida!!');
            else if($ctrl.item.object === 'group')
                $ctrl.confirmInsertAlarmGroup();
            else
                alert('Opção indisponivel!!');
        }            
        else if($ctrl.item.action == 'Edit') {
            if($ctrl.item.object === 'component')
                alert('Operação não definida!!');
            else if($ctrl.item.object === 'supply')
                $ctrl.updateOilSupply();
            else if($ctrl.item.object === 'alarm')
                $ctrl.updateAlarmsByComponent();
            else if($ctrl.item.object === 'group')
                $ctrl.confirmEditAlarmGroup();
            else
                alert('Opção indisponivel!!');
        }
    }
  
    $ctrl.insertComponent = function () {

        $ctrl.item.model.Equipment = $ctrl.item.equipment;
        $ctrl.item.model.EnabledOilTypes = [];
        var EnabledOilTypesIdsWithOutRepeat = [];

        if ($ctrl.EnabledOilTypesIds.length === 0) {
            alert('O Tipo de Óleo é um campo obrigatorio.\r\nFavor, informar pelo menos um tipo!');
            return false;
        }

        angular.forEach($ctrl.EnabledOilTypesIds, function (obj, key) {
            if (EnabledOilTypesIdsWithOutRepeat.indexOf(obj) < 0){
                EnabledOilTypesIdsWithOutRepeat.push(obj);
            } 
        });

        angular.forEach($ctrl.item.model.OilTypes, function (obj, key) {
            if (EnabledOilTypesIdsWithOutRepeat.indexOf(obj.idOilType) >= 0){
                $ctrl.item.model.EnabledOilTypes.push(obj);
            } 
        });

        $ctrl.item.model.OilManagement = $ctrl.item.user.OilManagement;
        $ctrl.item.model.OilUser = $ctrl.item.user.OilUser;
        $ctrl.item.model.cdUser = $ctrl.item.user.Login;

        oilManagementFactory.NewComponent($ctrl.item.model).success(function (response){
  
            if(response.status){
            
                alert('Unidade Hidráulica adicionada com sucesso!');

                oilManagementFactory.GetAllComponents().success(function (response){
    
                    if(response.length > 0){
                        
                        $ctrl.item.componentsService.setComponents(response);
                        $uibModalInstance.close($ctrl.item);
        
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

    $ctrl.insertSupply = function () {

        $ctrl.selected.OilType = undefined;
        $ctrl.selected.SupplyDateTime = undefined;
        $ctrl.selected.InsDateTime = undefined;

        //Check all required fields
        if ($ctrl.item.model.Quantity === null || $ctrl.item.model.Quantity === undefined || $ctrl.item.model.Quantity === 0) {
            alert('Favor inserir a Quantidade de Óleo do abastecimento!');
            return false;
        }

        if ($ctrl.FilteredOil === null || $ctrl.FilteredOil === undefined || $ctrl.FilteredOil === '') {
            alert('Favor selecionar se o abastecimento usou Óleo de Reuso ou não!');
            return false;
        }
        
        if ($ctrl.SelectedShift === null || $ctrl.SelectedShift === undefined || $ctrl.SelectedShift === '') {
            alert('Favor selecionar o Turno para o abastecimento!');
            return false;
        }

        if ($ctrl.item.model.OilSupplyType.Code === 'C') {
            if ($ctrl.item.model.StoppageType === null || $ctrl.item.model.StoppageType === undefined || $ctrl.item.model.StoppageType === '') {
                alert('Favor selecionar o Tipo de Parada!');
                return false;
            }
            else if ($ctrl.item.model.StoppageType.Code === 'PNO'){
                if ($ctrl.item.model.StoppageTime === null || $ctrl.item.model.StoppageTime === undefined || $ctrl.item.model.StoppageTime === 0) {
                    alert('Favor inserir o Tempo de Parada!');
                    return false;
                }
            }
        }

        $ctrl.selected.SupplyDateTime = $ctrl.item.model.SupplyDateTime;
        $ctrl.selected.InsDateTime = $ctrl.item.model.InsDateTime;
        $ctrl.selected.OilType = $ctrl.item.model.OilType;

        $ctrl.item.model.Component = $ctrl.item.component;
        $ctrl.item.model.SupplyDateTime = $ctrl.item.model.SupplyDateTime ? commonService.TryGetDateFromValue($ctrl.item.model.SupplyDateTime, 2, 1, 0, '/') : null;
        $ctrl.item.model.Shift = $ctrl.SelectedShift.id;
        $ctrl.item.model.IsReuseOil = $ctrl.FilteredOil === "SIM" ? true : false;

        $ctrl.item.model.OilManagement = $ctrl.item.user.OilManagement;
        $ctrl.item.model.OilUser = $ctrl.item.user.OilUser;
        $ctrl.item.model.cdUser = $ctrl.item.user.Login;

        oilManagementFactory.NewOilSupply($ctrl.item.model).success(function (response){
            
            if(response.status){

                alert('Abastecimento adicionado com sucesso!');

                oilManagementFactory.GetLastOilSupplyHistory('', $ctrl.item.component.Equipment.idEquipment, $ctrl.item.component.idComponent).success(function (response){
    
                    if(response){

                        $ctrl.item.suppliesService.setLastOilSupplies(response);
                        $uibModalInstance.close($ctrl.item);
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
                $ctrl.item.model.OilType  = $ctrl.selected.OilType;
                $ctrl.item.model.SupplyDateTime = $ctrl.selected.SupplyDateTime;
                $ctrl.item.model.InsDateTime = $ctrl.selected.InsDateTime;
                $ctrl.item.model.OilType = $ctrl.item.model.Component.EnabledOilTypes.filter(function(ot){  return ot.idOilType === $ctrl.item.model.OilType.idOilType})[0];

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
    
    $ctrl.updateOilSupply = function () {
        
        $ctrl.selected.OilType = undefined;
        $ctrl.selected.SupplyDateTime = undefined;
        $ctrl.selected.InsDateTime = undefined;

        //Check all required fields
        if ($ctrl.item.model.Quantity === null || $ctrl.item.model.Quantity === undefined || $ctrl.item.model.Quantity === 0) {
            alert('Favor inserir a Quantidade de Óleo do abastecimento!');
            return false;
        }

        if ($ctrl.FilteredOil === null || $ctrl.FilteredOil === undefined || $ctrl.FilteredOil === '') {
            alert('Favor selecionar se o abastecimento usou Óleo de Reuso ou não!');
            return false;
        }
        
        if ($ctrl.SelectedShift === null || $ctrl.SelectedShift === undefined || $ctrl.SelectedShift === '') {
            alert('Favor selecionar o Turno para o abastecimento!');
            return false;
        }

        if ($ctrl.item.model.OilSupplyType.Code === 'C') {
            if ($ctrl.item.model.StoppageType === null || $ctrl.item.model.StoppageType === undefined || $ctrl.item.model.StoppageType === '') {
                alert('Favor selecionar o Tipo de Parada!');
                return false;
            }
            else if ($ctrl.item.model.StoppageType.Code === 'PNO'){
                if ($ctrl.item.model.StoppageTime === null || $ctrl.item.model.StoppageTime === undefined || $ctrl.item.model.StoppageTime === 0) {
                    alert('Favor inserir o Tempo de Parada!');
                    return false;
                }
            }
        }
        
        $ctrl.selected.OilType = $ctrl.item.model.OilType;
        $ctrl.selected.SupplyDateTime = $ctrl.item.model.SupplyDateTime;
        $ctrl.selected.InsDateTime = $ctrl.item.model.InsDateTime;

        $ctrl.item.model.Component = $ctrl.item.component;
        $ctrl.item.model.SupplyDateTime = $ctrl.item.model.SupplyDateTime ? commonService.TryGetDateFromValue($ctrl.item.model.SupplyDateTime, 2, 1, 0, '/') : null;
        $ctrl.item.model.Shift = $ctrl.SelectedShift.id;
        $ctrl.item.model.IsReuseOil = $ctrl.FilteredOil === "SIM" ? true : false;

        $ctrl.item.model.OilManagement = $ctrl.item.user.OilManagement;
        $ctrl.item.model.OilUser = $ctrl.item.user.OilUser;
        $ctrl.item.model.cdUser = $ctrl.item.user.Login;

        oilManagementFactory.EditOilSupply($ctrl.item.model).success(function (response){
            
            if(response.status){
                
                alert('Abastecimento atualizado com sucesso!');

                oilManagementFactory.GetLastOilSupplyHistory('', $ctrl.item.component.Equipment.idEquipment, $ctrl.item.component.idComponent).success(function (response){
    
                    if(response){

                        $ctrl.item.suppliesService.setLastOilSupplies(response);
                        $uibModalInstance.close($ctrl.item);
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
                
                $ctrl.item.model.OilType  = $ctrl.selected.OilType;
                $ctrl.item.model.SupplyDateTime = $ctrl.selected.SupplyDateTime;
                $ctrl.item.model.InsDateTime = $ctrl.selected.InsDateTime;
                

                $ctrl.item.model.OilType = $ctrl.item.model.Component.EnabledOilTypes.filter(function(ot){  return ot.idOilType === $ctrl.item.model.OilType.idOilType})[0];
                //$ctrl.item.model.OilSupplyType = $ctrl.item.model.OilSupplyTypes.filter(function(ost){  return ost.idOilSupplyType === $ctrl.item.model.OilSupplyType.idOilSupplyType})[0];

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
    
    $ctrl.updateAlarmsByComponent = function () {
        
        // //Check all required fields
        // if ($ctrl.item.model.Quantity === null || $ctrl.item.model.Quantity === undefined || $ctrl.item.model.Quantity === 0) {
        //     alert('Favor inserir a Quantidade de Óleo do abastecimento!');
        //     return false;
        // }

        // if ($ctrl.FilteredOil === null || $ctrl.FilteredOil === undefined || $ctrl.FilteredOil === '') {
        //     alert('Favor selecionar se o abastecimento usou Óleo de Reuso ou não!');
        //     return false;
        // }
        
        // if ($ctrl.SelectedShift === null || $ctrl.SelectedShift === undefined || $ctrl.SelectedShift === '') {
        //     alert('Favor selecionar o Turno para o abastecimento!');
        //     return false;
        // }

        // if ($ctrl.item.model.OilSupplyType.Code === 'C') {
        //     if ($ctrl.item.model.StoppageType === null || $ctrl.item.model.StoppageType === undefined || $ctrl.item.model.StoppageType === '') {
        //         alert('Favor selecionar o Tipo de Parada!');
        //         return false;
        //     }
        //     else if ($ctrl.item.model.StoppageType.Code === 'PNO'){
        //         if ($ctrl.item.model.StoppageTime === null || $ctrl.item.model.StoppageTime === undefined || $ctrl.item.model.StoppageTime === 0) {
        //             alert('Favor inserir o Tempo de Parada!');
        //             return false;
        //         }
        //     }
        // }
        
        oilManagementFactory.EditAlarmsByComponent($ctrl.item.model).success(function (response){
            
            if(response.status){
                
                alert('Alarmes atualizados com sucesso!');

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

    $ctrl.confirmInsertAlarmGroup = function(){

        if ($ctrl.item.model.Name.trim()  === '') {
            alert('Não foi preenchido o nome do novo grupo.\r\nFavor, verificar o campo Nome!');
            return false;
        }

        if ($ctrl.item.model.Users.length  === 0) {

            $.smallBox({
                title : 'Criação de Grupo',
                content : "Não foram incluidos usuários no Grupo.\r\nMesmo assim deseja criar o grupo?<p class='text-align-right'><a id='btnSim' class='btn btn-primary btn-sm'>Sim</a> <a href='javascript:void(0);' class='btn btn-primary btn-sm'>Não</a></p>",            
                color : "#3276B1",
                icon : "fa fa-warning swing animated" },
                function(e){
                    if(e.target.id == 'btnSim'){
                        $ctrl.insertAlarmGroup();
                    }
                }
            );
        }
        else{
            $ctrl.insertAlarmGroup();
        }
    }

    $ctrl.insertAlarmGroup = function () {

        var model = angular.copy($ctrl.item.model);
        model.UsersAll = [];
        oilManagementFactory.NewAlarmGroup(model).success(function (response){
  
            if(response.status){

                alert('Grupo criado com sucesso!');

                var userLogged = $ctrl.item.user.Domain + '\\' + $ctrl.item.user.Login;
                var onlyActives = true;

                oilManagementFactory.GetAlarmGroups(userLogged, onlyActives, $ctrl.item.user.OilManagement, $ctrl.item.user.OilUser).success(function (response){
    
                    if(response){
                        
                        $ctrl.item.alarmsService.setAlarmGroupsAll(response);
                        $uibModalInstance.close($ctrl.item);
        
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

    $ctrl.confirmEditAlarmGroup = function(){

        if ($ctrl.item.model.Name.trim()  === '') {
            alert('Não foi preenchido o nome do novo grupo.\r\nFavor, verificar o campo Nome!');
            return false;
        }

        if ($ctrl.item.model.Users.length  === 0) {

            $.smallBox({
                title : 'Criação de Grupo',
                content : "Não foram incluidos usuários no Grupo.\r\nMesmo assim deseja criar o grupo?<p class='text-align-right'><a id='btnSim' class='btn btn-primary btn-sm'>Sim</a> <a href='javascript:void(0);' class='btn btn-primary btn-sm'>Não</a></p>",            
                color : "#3276B1",
                icon : "fa fa-warning swing animated" },
                function(e){
                    if(e.target.id == 'btnSim'){
                        $ctrl.editAlarmGroup();
                    }
                }
            );
        }
        else{
            $ctrl.editAlarmGroup();
        }
    }

    $ctrl.editAlarmGroup = function () {

        var model = angular.copy($ctrl.item.model);
        model.UsersAll = [];

        oilManagementFactory.EditAlarmGroup(model).success(function (response){
  
            if(response.status){

                alert('Grupo editado com sucesso!');

                var userLogged = $ctrl.item.user.Domain + '\\' + $ctrl.item.user.Login;
                var onlyActives = true;

                oilManagementFactory.GetAlarmGroups(userLogged, onlyActives, $ctrl.item.user.OilManagement, $ctrl.item.user.OilUser).success(function (response){
    
                    if(response){
                        
                        $ctrl.item.alarmsService.setAlarmGroupsAll(response);
                        $uibModalInstance.close($ctrl.item);
        
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

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
  
    $ctrl.addEnabledOilType = function() {
        //alert($ctrl.item.model.EnabledOilTypes);
        //alert($ctrl.EnabledOilTypesIds);
    }

    $ctrl.checkChange = function(idx) {

        if ($ctrl.item.model.EditionEnabled)
        {
            if (idx != null || idx != undefined) {
                $ctrl.item.model.Alarms[idx].Edited = true;
            }            
            $ctrl.saveDisabled = false;
        }
        else
            $ctrl.saveDisabled = true;
    }

    $ctrl.transferToRightAllItems = function ()
    {
        angular.forEach($ctrl.item.model.UsersAll, function (obj, key) {
            $ctrl.item.model.Users.push(obj);
        });

        $ctrl.item.model.UsersAll = [];
        $ctrl.UsersAllSelected = [];

        $ctrl.saveDisabled = false;
    }

    $ctrl.transferToRightSelectedItems = function ()
    {
        var newUsersAll = [];
        var newUsersIds = [];

        angular.forEach($ctrl.UsersAllSelected, function (obj, key) {
            angular.forEach($ctrl.item.model.UsersAll, function (obj2, key2) {
                if (obj == obj2.idUser) {
                    $ctrl.item.model.Users.push(obj2);
                    newUsersIds.push(obj2.idUser);
                }
            });
        });

        angular.forEach($ctrl.item.model.UsersAll, function (obj, key) {
            if (newUsersIds.indexOf(obj.idUser) < 0) {
                newUsersAll.push(obj);
            }
        });

        $ctrl.item.model.UsersAll = [];
        $ctrl.item.model.UsersAll = newUsersAll;
        $ctrl.UsersAllSelected = [];

        $ctrl.saveDisabled = false;
    }

    $ctrl.transferToLeftSelectedItems = function ()
    {
        var newUsers = [];
        var transferIds = [];

        angular.forEach($ctrl.UsersSelected, function (obj, key) {
            angular.forEach($ctrl.item.model.Users, function (obj2, key2) {
                if (obj == obj2.idUser) {
                    $ctrl.item.model.UsersAll.push(obj2);
                    transferIds.push(obj2.idUser);
                }
            });
        });

        angular.forEach($ctrl.item.model.Users, function (obj, key) {
            if (transferIds.indexOf(obj.idUser) < 0) {
                newUsers.push(obj);
            }
        });

        $ctrl.item.model.Users = [];
        $ctrl.item.model.Users = newUsers;
        $ctrl.UsersSelected = [];

        $ctrl.saveDisabled = false;
    }

    $ctrl.transferToLeftAllItems = function ()
    {
        angular.forEach($ctrl.item.model.Users, function (obj, key) {
            $ctrl.item.model.UsersAll.push(obj);
        });

        $ctrl.item.model.Users = [];
        $ctrl.UsersSelected = [];

        $ctrl.saveDisabled = false;
    }

    $ctrl.editGroupsInAlarm = function(idx){

        $ctrl2.action = 'Edit';
        $ctrl2.title = 'Editar Grupos';
        $ctrl2.object = 'groupsByAlarm';
        $ctrl2.user = $ctrl.user;
        $ctrl2.alarmsService = $scope.alarmsService;

        $ctrl2.model = $ctrl.item.model.Alarms[idx];

        var modalInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: 'groupsInAlarmContent.html',
            controller: 'modalInstanceGroupController',
            controllerAs: '$ctrl2',
            size: 'lg',
            resolve: {
                item: function () {
                return $ctrl2;
                }
            }               
        });
    }

    //Função responsável por atualizar o objeto original
    function copyTo(srcObj, destObj) {
        for (var key in destObj) {
          if(destObj.hasOwnProperty(key) && srcObj.hasOwnProperty(key)) {
            destObj[key] = srcObj[key];
          }
        }
    }

  });
    
  