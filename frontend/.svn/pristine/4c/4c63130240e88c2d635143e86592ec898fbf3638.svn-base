app.factory('areasFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetAreas = function(){
        return $http({
            url: consts.apiUrl+"Area/GetAreas",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }
        

    return obj;

}]);