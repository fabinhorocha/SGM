app.factory('notificationFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetNotification = function(idPlantGroup, dtMonth, dtYear, type){
        return $http({
            url: consts.apiUrl+"Notification/GetNotification",
            dataType: 'json',
            method: 'GET',
            params: { idPlantGroup: idPlantGroup, dtMonth: dtMonth, dtYear: dtYear, type: type},               
            headers: {"Content-Type": "application/json" }
        })

    }     
       

    return obj;

}]);