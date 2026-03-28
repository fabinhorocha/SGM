app.service('oilAlarmsService',function(){
    var alarmGroupsAll  = [];


    return{

        getAlarmGroupsAll : function(){
            return alarmGroupsAll.filter(function(g){return g.Active === true});
        },

        setAlarmGroupsAll: function(value){

            alarmGroupsAll = value;
        }
    };

});