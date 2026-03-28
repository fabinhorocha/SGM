app.factory('reportsTypesFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetReportsTypes = function(){
        return $http({
            url: consts.apiUrl+"ReportType/GetReportsTypes",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }
        

    return obj;

}]);