app.factory('prioritiesFactory',['$http','consts', function($http, consts){
    
        var obj = {};
    
        obj.GetPriorities = function(){
            return $http({
                url: consts.apiUrl+"Priority/GetPriorities",
                dataType: 'json',
                method: 'GET',
                data: '',            
                headers: {"Content-Type": "application/json" }
            })
    
        }
            
    
        return obj;
    
}]);