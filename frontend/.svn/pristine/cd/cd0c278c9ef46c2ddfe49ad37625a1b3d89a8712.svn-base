app.factory('indicatorOTFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetPlantData = function(idIndicator, idBudget, cdPredictive){
        return $http({
            url: consts.apiUrl+"PlantData/GetPlantData",
            dataType: 'json',
            method: 'GET',
            params: { idIndicator: idIndicator, idBudget: idBudget, cdPredictive: cdPredictive},                   
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetPlantDataByPlant = function(idIndicator, idBudget, cdPredictive, plant){
        return $http({
            url: consts.apiUrl+"PlantData/GetPlantDataByPlant",
            dataType: 'json',
            method: 'GET',
            params: { idIndicator: idIndicator, idBudget: idBudget, cdPredictive: cdPredictive, plant: plant},                   
            headers: {"Content-Type": "application/json" }
        })

    }
        

    return obj;

}]);