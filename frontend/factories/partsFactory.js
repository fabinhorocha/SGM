app.factory('partsFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetParts = function(){
        return $http({
            url: consts.apiUrl+"Part/GetParts",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }
        

    return obj;

}]);