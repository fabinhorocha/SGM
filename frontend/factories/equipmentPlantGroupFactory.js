app.factory('equipmentPlantGroupFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetEquipmentsPlantGroup = function(index, sheedMan){
        return $http({
            url: consts.apiUrl+"EquipmentPlantGroup/GetEquipmentsPlantGroup",
            dataType: 'json',
            method: 'GET',
            params: { index: index, sheedMan: sheedMan},                   
            headers: {"Content-Type": "application/json" }
        })

    }               

    return obj;

}]);