app.factory('indicatorMaintenanceFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetLocationData = function(idIndicator, idBudget){
        return $http({
            url: consts.apiUrl+"LocationData/GetLocationData",
            dataType: 'json',
            method: 'GET',
            params: { idIndicator: idIndicator, idBudget: idBudget},                   
            headers: {"Content-Type": "application/json" }
        })

    }
        

    return obj;

}]);