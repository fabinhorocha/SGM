app.service('oilSupplyService',function(){
    var lastOilSupplies  = [];


    return{

        getLastOilSupplies: function(){
            return lastOilSupplies;
        },

        setLastOilSupplies: function(value){

            lastOilSupplies = value;
        }
    };

});