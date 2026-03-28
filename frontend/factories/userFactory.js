app.factory('userFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetUser = function(){
        return $http({
            url: consts.apiUrl+"User/GetUser",
            dataType: 'json',
            method: 'GET',
          //  withCredentials: true,
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }
        

    return obj;

}]);