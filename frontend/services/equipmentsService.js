app.service('equipmentsService',function(){
    var equipmentsAll  = [];


    return{

        getEquipments: function(){
            return equipmentsAll.filter(function(f){return f.Active === true});
        },

        setEquipments: function(value){

            equipmentsAll = value;
        }
    };

});