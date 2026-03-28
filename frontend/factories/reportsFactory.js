app.factory('reportsFactory',['$http','consts', function($http, consts){

    var obj = {};


    obj.GetReportsByDateRange = function(startDate, endDate, idEquipment, idType){
        
        return $http({
            url: consts.apiUrl+"Report/GetReportsByDateRange",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { startDate: startDate, endDate: endDate, idEquipment: idEquipment, idType: idType},                   
            headers: {"Content-Type": "application/json" }
        })
        
    } 

    obj.GetMeasurementsByDateRange = function(startDate, endDate, idEquipment, idType){
        
        return $http({
            url: consts.apiUrl+"Report/GetMeasurementsByDateRange",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { startDate: startDate, endDate: endDate, idEquipment: idEquipment, idType: idType},                   
            headers: {"Content-Type": "application/json" }
        })
        
    } 

    obj.GetAnalysisValues = function(startDate, endDate, idEquipment, idType){
        
        return $http({
            url: consts.apiUrl+"Report/GetAnalysisValues",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { startDate: startDate, endDate: endDate, idEquipment: idEquipment, idType: idType},                   
            headers: {"Content-Type": "application/json" }
        })
        
    } 

    obj.GetReportsOTs = function(startDate, endDate){
        
        return $http({
            url: consts.apiUrl+"Report/GetReportsOTs",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { startDate: startDate, endDate: endDate},                   
            headers: {"Content-Type": "application/json" }
        })
        
    } 

    obj.GetReport = function(idReport){
        
        return $http({
            url: consts.apiUrl+"Report/GetReport",
            dataType: 'json',
            method: 'GET',
            data: '', 
            params: { id: idReport},                   
            headers: {"Content-Type": "application/json" }
        })
        
    }

    obj.InsertReport = function (report) {
        return $http({
            url: consts.apiUrl+"Report/InsertReport",
            dataType: 'json',
            method: 'POST',
            data:  report,
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    obj.UpdateReport = function (report) {
        return $http({
            url: consts.apiUrl+"Report/UpdateReport",
            dataType: 'json',
            method: 'POST',
            data:  report,
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    obj.DeleteReport = function (report) {
        return $http({
            url: consts.apiUrl+"Report/DeleteReport",
            dataType: 'json',
            method: 'POST',
            data:  report,
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    obj.CreateNoteSAP = function (report) {
        return $http({
            url: consts.apiUrl+"Report/CreateNoteSAP",
            dataType: 'json',
            method: 'POST',
            data:  report,
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    return obj;

}]);