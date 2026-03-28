app.factory('indicatorOilManagementFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetOilSupplyData = function(idReportClass,idReportType, idFactory, idArea, idEquipment, idComponent,startDate, endDate, idOilType, idOilSupplyType, idStoppageType, OilManagement,OilUser){
        return $http({
            url: consts.apiUrl+"OilManagementData/GetOilSupplyData",
            dataType: 'json',
            method: 'GET',
            params: {
                idReportClass : idReportClass,
                idReportType : idReportType, 
                idFactory : idFactory,
                idArea : idArea,
                idEquipment : idEquipment,
                idComponent : idComponent,
                startDate : startDate,
                endDate : endDate,
                idOilType : idOilType,
                idOilSupplyType : idOilSupplyType,
                idStoppageType : idStoppageType,
                OilManagement : OilManagement,
                OilUser : OilUser
             },
            headers: {"Content-Type": "application/json" }
        })

    }
    
    obj.GetFilterInfo = function(){
        
        return $http({
            url: consts.apiUrl+"OilManagementData/GetFilterInfo",
            dataType: 'json',
            method: 'GET',
            data: '',
            params: {},
            headers: {"Content-Type": "application/json" }
        })

    }

    return obj;

}]);