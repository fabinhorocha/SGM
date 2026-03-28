app.factory('typesFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetTypes = function(){
        return $http({
            url: consts.apiUrl+"Type/GetTypes",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }
        

    return obj;

}]);