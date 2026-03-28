app.factory('filesFactory',['$http','consts', function($http, consts){

    var obj = {};
    

    obj.RemoveFile = function (id) {
        return $http({
            url: consts.apiUrl+"ReportFile/DeleteFileReport",
            dataType: 'json',
            method: 'POST',
            data:  {id: id},
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    obj.RemoveFiles = function (file) {
        return $http({
            url: consts.apiUrl+"ReportFile/RemoveFiles",
            dataType: 'json',
            method: 'POST',
            data:  file,
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }


    return obj;

}]);