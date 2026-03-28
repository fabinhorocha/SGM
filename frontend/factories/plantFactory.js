app.factory('plantFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetPlants = function(){
        return $http({
            url: consts.apiUrl+"Plant/GetPlants",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }     
       

    return obj;

}]);