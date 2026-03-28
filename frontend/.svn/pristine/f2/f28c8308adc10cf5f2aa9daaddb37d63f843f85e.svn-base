app.factory('budgetPlantFactory',['$http','consts', function($http, consts){

    var obj = {};
   
    obj.UpdateBudgetPlant= function (budget) {
        return $http({
            url: consts.apiUrl+"BudgetPlant/UpdateBudgetPlant",
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