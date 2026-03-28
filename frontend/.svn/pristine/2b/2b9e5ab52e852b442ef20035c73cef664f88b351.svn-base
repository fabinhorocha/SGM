app.factory('oilManagementFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetAllEquipments = function(value){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetAllEquipments",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { status: value},
            headers: {"Content-Type": "application/json" }
        })
        
    }

    obj.GetAllComponents = function(){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetComponents",
            dataType: 'json',
            method: 'GET',
            data: '', 
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetNewEmptyComponent = function(idEquipment){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetNewEmptyComponent",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { idEquipment: idEquipment},  
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetComponent = function(idComponent){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetComponent",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { idComponent: idComponent},  
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetComponentsSettings = function(userCode, idFactory, idArea, idEquipment, idComponent, reportType, onlyActives, OilManagement, OilUser){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetComponentsSettings",
            dataType: 'json',
            method: 'GET',
            data: '',
            params: { 
                userCode : userCode,
                idFactory : idFactory,
                idArea : idArea,
                idEquipment : idEquipment,
                idComponent : idComponent,
                reportType : reportType,
                onlyActives : onlyActives,
                OilManagement : OilManagement,
                OilUser : OilUser
            },
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetOilSupply = function(idOilSupply, cdUser, oilManagement, oilUser){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetOilSupply",
            dataType: 'json',
            method: 'GET',
            params: {   
                        idOilSupply: idOilSupply, 
                        cdUser : cdUser, 
                        oilManagement : oilManagement,
                        oilUser : oilUser
                    },
            headers: {"Content-Type": "application/json" }
        })
    }

    obj.GetLastOilSupplyHistory = function(userCode, idEquipment, idComponent){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetLastOilSupplyHistory",
            dataType: 'json',
            method: 'GET',
            data: '',
            params: { userCode : userCode, idEquipment : idEquipment, idComponent : idComponent },
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetOilSupplyHistory = function(userCode, idFactory, idArea, idEquipment, idComponent, reportType, onlyActives, startDate, endDate, OilManagement, OilUser){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetOilSupplyHistory",
            dataType: 'json',
            method: 'GET',
            data: '',
            params: { 
                userCode : userCode,
                idFactory : idFactory,
                idArea : idArea,
                idEquipment : idEquipment,
                idComponent : idComponent,
                reportType : reportType,
                onlyActives : onlyActives,
                startDate : startDate,
                endDate : endDate,
                OilManagement : OilManagement,
                OilUser : OilUser
            },
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.NewOilSupply = function(newOilSupply){
        
        return $http({
            url: consts.apiUrl+"OilManagement/NewOilSupply",
            dataType: 'json',
            method: 'POST',
            data: newOilSupply,
            headers: {"Content-Type": "application/json" }
        })
    }

    obj.GetNewEmptyOilSupply = function(idComponent){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetNewEmptyOilSupply",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { idComponent: idComponent},  
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.NewComponent = function(newComponent){
        
        return $http({
            url: consts.apiUrl+"OilManagement/NewComponent",
            dataType: 'json',
            method: 'POST',
            data: newComponent,
            headers: {"Content-Type": "application/json" }
        })
    }

    obj.EditComponent = function(currentComponent){
        
        return $http({
            url: consts.apiUrl+"OilManagement/EditComponent",
            dataType: 'json',
            method: 'POST',
            data: currentComponent,
            headers: {"Content-Type": "application/json" }
        })
    }
    
    obj.EditOilSupply = function(currentOilSupply){
        
        return $http({
            url: consts.apiUrl+"OilManagement/EditOilSupply",
            dataType: 'json',
            method: 'POST',
            data: currentOilSupply,
            headers: {"Content-Type": "application/json" }
        })
    }

    obj.GetAlarms = function(userCode, idFactory, idArea, idEquipment, idComponent, onlyActives, OilManagement, OilUser){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetAlarms",
            dataType: 'json',
            method: 'GET',
            data: '',
            params: { 
                userCode : userCode,
                idFactory : idFactory,
                idArea : idArea,
                idEquipment : idEquipment,
                idComponent : idComponent,
                onlyActives : onlyActives,
                OilManagement : OilManagement,
                OilUser : OilUser
            },
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetAlarmsByComponent = function(userCode, idComponent, OilManagement, OilUser){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetAlarmsByComponent",
            dataType: 'json',
            method: 'GET',
            data: '',
            params: { 
                userCode : userCode,
                idComponent : idComponent,
                OilManagement : OilManagement,
                OilUser : OilUser
            },
            headers: {"Content-Type": "application/json" }
        })
    }
 
    obj.EditAlarmsByComponent = function(currentAlarms){
        
        return $http({
            url: consts.apiUrl+"OilManagement/EditAlarmsByComponent",
            dataType: 'json',
            method: 'POST',
            data: currentAlarms,
            headers: {"Content-Type": "application/json" }
        })
    }
    
    obj.EditGroupsByAlarm = function(alarm){
        
        return $http({
            url: consts.apiUrl+"OilManagement/EditGroupsByAlarm",
            dataType: 'json',
            method: 'POST',
            data: alarm,
            headers: {"Content-Type": "application/json" }
        })
    }
    
    obj.GetAlarmsHistory= function(userCode, idFactory, idArea, idEquipment, idComponent, startDate, endDate, onlyActives, OilManagement, OilUser){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetAlarmsHistory",
            dataType: 'json',
            method: 'GET',
            data: '',
            params: { 
                userCode : userCode,
                idFactory : idFactory,
                idArea : idArea,
                idEquipment : idEquipment,
                idComponent : idComponent,
                onlyActives : onlyActives,
                startDate : startDate,
                endDate : endDate,
                OilManagement : OilManagement,
                OilUser : OilUser
            },
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetAlarmGroup = function(userCode, idAlarmGroup, OilManagement, OilUser)
    {
        return $http({
            url: consts.apiUrl+"OilManagement/GetAlarmGroup",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { 
                userCode : userCode,
                idAlarmGroup : idAlarmGroup,
                OilManagement : OilManagement,
                OilUser : OilUser
            },
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetNewEmptyAlarmGroup = function()
    {
        return $http({
            url: consts.apiUrl+"OilManagement/GetNewEmptyAlarmGroup",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: {},  
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.NewAlarmGroup = function(newAlarmGroup){
        
        return $http({
            url: consts.apiUrl+"OilManagement/NewAlarmGroup",
            dataType: 'json',
            method: 'POST',
            data: newAlarmGroup,
            headers: {"Content-Type": "application/json" }
        })
    }

    obj.EditAlarmGroup = function(editedAlarmGroup){
        
        return $http({
            url: consts.apiUrl+"OilManagement/EditAlarmGroup",
            dataType: 'json',
            method: 'POST',
            data: editedAlarmGroup,
            headers: {"Content-Type": "application/json" }
        })
    }

    obj.GetAlarmGroups = function(userCode, onlyActives, OilManagement, OilUser){
        
        return $http({
            url: consts.apiUrl+"OilManagement/GetAlarmGroups",
            dataType: 'json',
            method: 'GET',
            data: '',
            params: { 
                userCode : userCode,
                onlyActives : onlyActives,
                OilManagement : OilManagement,
                OilUser : OilUser
            },
            headers: {"Content-Type": "application/json" }
        })

    }

    return obj;

    }]);