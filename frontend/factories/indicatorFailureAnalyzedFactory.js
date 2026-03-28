app.factory('indicatorFailureAnalyzedFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetFailureDataNote = function(idIndicator, idBudget){
        return $http({
            url: consts.apiUrl+"FailureData/GetFailureDataNote",
            dataType: 'json',
            method: 'GET',
            params: { idIndicator: idIndicator, idBudget: idBudget},                   
            headers: {"Content-Type": "application/json" }
        })

    }


    obj.GetNotesFailure = function(dateStart, dateEnd, location){
        return $http({
            url: consts.apiUrl+"NoteFailure/GetNotesFailure",
            dataType: 'json',
            method: 'GET',
            params: { dateStart: dateStart, dateEnd: dateEnd, location: location},                   
            headers: {"Content-Type": "application/json" }
        })

    }


            

    return obj;

}]);