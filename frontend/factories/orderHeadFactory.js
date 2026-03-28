app.factory('orderHeadFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetOrderHead= function(idPlantGroup, dtMonth, dtYear, type){
        return $http({
            url: consts.apiUrl+"OrderHead/GetOrderHead",
            dataType: 'json',
            method: 'GET',
            params: { idPlantGroup: idPlantGroup, dtMonth: dtMonth, dtYear: dtYear, type: type},               
            headers: {"Content-Type": "application/json" }
        })

    }     
       

    return obj;

}]);