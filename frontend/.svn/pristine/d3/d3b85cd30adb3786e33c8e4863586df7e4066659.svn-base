app.controller('modalInstanceGroupController', function ($uibModalInstance, $rootScope, item, oilManagementFactory) {   
  
    var $ctrl2 = this;  
    
    $ctrl2.item = angular.copy(item);
  
    $ctrl2.saveDisabled = true;

    if ($ctrl2.item.action == 'Edit'){
        if ($ctrl2.item.object === 'groupsByAlarm') {
            $ctrl2.treeDetail = $ctrl2.item.model.Alarm.Component.Name + ' > Alarme de ' + $ctrl2.item.model.Alarm.AlarmType.Name;
            $ctrl2.GroupsSelected = [];
            $ctrl2.GroupsAllSelected = [];
            $ctrl2.GroupsSelectedCC = [];
        }
        else {
            alert('Operação não definida!!');
        }
    }
  
    $ctrl2.save = function (){
      if($ctrl2.item.action == 'Edit')
        $ctrl2.update();
  }
    
    $ctrl2.update = function () {
        
        var model = angular.copy($ctrl2.item.model);
        oilManagementFactory.EditGroupsByAlarm(model).success(function (response){
  
            if(response.status){
                alert('Grupos da alarme editados com sucesso!');
                $uibModalInstance.close($ctrl2.item);
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
    
    $ctrl2.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $ctrl2.transferToRightAllItems = function ()
    {
        angular.forEach($ctrl2.item.model.GroupsAll, function (obj, key) {
            $ctrl2.item.model.Alarm.Groups.push(obj);
        });

        $ctrl2.item.model.GroupsAll = [];
        $ctrl2.GroupsAllSelected = [];

        $ctrl2.saveDisabled = false;
    }

    $ctrl2.transferToRightSelectedItems = function ()
    {
        var newGroupsAll = [];
        var transferIds = [];

        angular.forEach($ctrl2.GroupsAllSelected, function (obj, key) {
            angular.forEach($ctrl2.item.model.GroupsAll, function (obj2, key2) {
                if (obj == obj2.idAlarmGroup) {
                    $ctrl2.item.model.Alarm.Groups.push(obj2);
                    transferIds.push(obj2.idAlarmGroup);
                }
            });
        });

        angular.forEach($ctrl2.item.model.GroupsAll, function (obj, key) {
            if (transferIds.indexOf(obj.idAlarmGroup) < 0) {
                newGroupsAll.push(obj);
            }
        });

        $ctrl2.item.model.GroupsAll = [];
        $ctrl2.item.model.GroupsAll = newGroupsAll;
        $ctrl2.GroupsAllSelected = [];

        $ctrl2.saveDisabled = false;
    }

    $ctrl2.transferToLeftSelectedItems = function ()
    {
        var newGroups = [];
        var transferIds = [];

        angular.forEach($ctrl2.GroupsSelected, function (obj, key) {
            angular.forEach($ctrl2.item.model.Alarm.Groups, function (obj2, key2) {
                if (obj == obj2.idAlarmGroup) {
                    $ctrl2.item.model.GroupsAll.push(obj2);
                    transferIds.push(obj2.idAlarmGroup);
                }
            });
        });

        angular.forEach($ctrl2.item.model.Alarm.Groups, function (obj, key) {
            if (transferIds.indexOf(obj.idAlarmGroup) < 0) {
                newGroups.push(obj);
            }
        });

        $ctrl2.item.model.Alarm.Groups = [];
        $ctrl2.item.model.Alarm.Groups = newGroups;
        $ctrl2.GroupsSelected = [];

        $ctrl2.saveDisabled = false;
    }

    $ctrl2.transferToLeftAllItems = function ()
    {
        angular.forEach($ctrl2.item.model.Alarm.Groups, function (obj, key) {
            $ctrl2.item.model.GroupsAll.push(obj);
        });

        $ctrl2.item.model.Alarm.Groups = [];
        $ctrl2.GroupsSelected = [];

        $ctrl2.saveDisabled = false;
    }

    $ctrl2.transferToRightAllItemsCC = function ()
    {
        angular.forEach($ctrl2.item.model.GroupsAll, function (obj, key) {
            $ctrl2.item.model.Alarm.GroupsCC.push(obj);
        });

        $ctrl2.item.model.GroupsAll = [];
        $ctrl2.GroupsAllSelected = [];

        $ctrl2.saveDisabled = false;
    }

    $ctrl2.transferToRightSelectedItemsCC = function ()
    {
        var newGroupsAll = [];
        var transferIds = [];

        angular.forEach($ctrl2.GroupsAllSelected, function (obj, key) {
            angular.forEach($ctrl2.item.model.GroupsAll, function (obj2, key2) {
                if (obj == obj2.idAlarmGroup) {
                    $ctrl2.item.model.Alarm.GroupsCC.push(obj2);
                    transferIds.push(obj2.idAlarmGroup);
                }
            });
        });

        angular.forEach($ctrl2.item.model.GroupsAll, function (obj, key) {
            if (transferIds.indexOf(obj.idAlarmGroup) < 0) {
                newGroupsAll.push(obj);
            }
        });

        $ctrl2.item.model.GroupsAll = [];
        $ctrl2.item.model.GroupsAll = newGroupsAll;
        $ctrl2.GroupsAllSelected = [];

        $ctrl2.saveDisabled = false;
    }

    $ctrl2.transferToLeftSelectedItemsCC = function ()
    {
        var newGroups = [];
        var transferIds = [];

        angular.forEach($ctrl2.GroupsSelectedCC, function (obj, key) {
            angular.forEach($ctrl2.item.model.Alarm.GroupsCC, function (obj2, key2) {
                if (obj == obj2.idAlarmGroup) {
                    $ctrl2.item.model.GroupsAll.push(obj2);
                    transferIds.push(obj2.idAlarmGroup);
                }
            });
        });

        angular.forEach($ctrl2.item.model.Alarm.GroupsCC, function (obj, key) {
            if (transferIds.indexOf(obj.idAlarmGroup) < 0) {
                newGroups.push(obj);
            }
        });

        $ctrl2.item.model.Alarm.GroupsCC = [];
        $ctrl2.item.model.Alarm.GroupsCC = newGroups;
        $ctrl2.GroupsSelectedCC = [];

        $ctrl2.saveDisabled = false;
    }

    $ctrl2.transferToLeftAllItemsCC = function ()
    {
        angular.forEach($ctrl2.item.model.Alarm.GroupsCC, function (obj, key) {
            $ctrl2.item.model.GroupsAll.push(obj);
        });

        $ctrl2.item.model.Alarm.GroupsCC = [];
        $ctrl2.GroupsSelectedCC = [];

        $ctrl2.saveDisabled = false;
    }

    $ctrl2.checkChange = function() {
        $ctrl2.saveDisabled = false;
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
  