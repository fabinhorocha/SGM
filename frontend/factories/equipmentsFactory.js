app.factory('equipmentsFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetEquipments = function(){
        return $http({
            url: consts.apiUrl+"Equipment/GetEquipments",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }

    obj.GetAllEquipments = function(value){
        
        return $http({
            url: consts.apiUrl+"Equipment/GetAllEquipments",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { status: value},                   
            headers: {"Content-Type": "application/json" }
        })
        
    }

    obj.InsertEquipment = function (equipment) {
        return $http({
            url: consts.apiUrl+"Equipment/InsertEquipment",
            dataType: 'json',
            method: 'POST',
            data:  equipment,
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    obj.UpdateEquipment = function (equipment) {
        return $http({
            url: consts.apiUrl+"Equipment/UpdateEquipment",
            dataType: 'json',
            method: 'POST',
            data:  equipment,
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    return obj;

}]);