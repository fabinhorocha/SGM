app.factory('budgetLocationFactory',['$http','consts', function($http, consts){

    var obj = {};
   
    obj.UpdateBudgetLocation= function (budget) {
        return $http({
            url: consts.apiUrl+"BudgetLocation/UpdateBudgetLocation",
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