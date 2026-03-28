app.factory('locationFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetLocations = function(){
        return $http({
            url: consts.apiUrl+"Location/GetLocations",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }     
    
    obj.GetLocationsPlants = function(){
        return $http({
            url: consts.apiUrl+"LocationPlant/GetLocationsPlants",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }     

    return obj;

}]);