app.factory('reportsStatusFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetReportsStatus = function(){
        return $http({
            url: consts.apiUrl+"ReportStatus/GetReportsStatus",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }
        

    return obj;

}]);