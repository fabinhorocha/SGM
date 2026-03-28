app.service('commonService', function ($filter) {
    this.TryGetDateFromValue = function (val, yearPos, monthPos, dayPos, separator) {
        if (!angular.isDate(val)) {
            val = $filter('date')(new Date(val.split(separator)[yearPos], val.split(separator)[monthPos] - 1, val.split(separator)[dayPos]));
        }
        return val;
    };

   

});
