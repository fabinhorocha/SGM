app.factory('indicatorNoteFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetPlantDataNote = function(idIndicator, idBudget, cdPredictive){
        return $http({
            url: consts.apiUrl+"PlantData/GetPlantDataNote",
            dataType: 'json',
            method: 'GET',
            params: { idIndicator: idIndicator, idBudget: idBudget, cdPredictive: cdPredictive},                   
            headers: {"Content-Type": "application/json" }
        })

    }

            

    return obj;

}]);