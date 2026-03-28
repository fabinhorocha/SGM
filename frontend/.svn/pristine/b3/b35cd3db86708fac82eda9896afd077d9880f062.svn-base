app.factory('equipmentsBackupFactory',['$http','consts', function($http, consts){

    var obj = {};

    obj.GetBackups = function(){
        return $http({
            url: consts.apiUrl+"EquipmentBackup/GetBackups",
            dataType: 'json',
            method: 'GET',
            data: '',            
            headers: {"Content-Type": "application/json" }
        })

    }
   

    obj.Backup = function () {
        return $http({
            url: consts.apiUrl+"EquipmentBackup/Backup",
            dataType: 'json',
            method: 'POST',
            data:  '',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    obj.Restore = function (id) {
        return $http({
            url: consts.apiUrl+"EquipmentBackup/Restore",
            dataType: 'json',
            method: 'POST',
            data: id,                        
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        })
    }

    return obj;

}]);