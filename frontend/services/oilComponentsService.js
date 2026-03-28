app.service('oilComponentsService',function(){
    var componentsAll  = [];


    return{

        getComponents: function(){
            return componentsAll.filter(function(f){return f.Active === true});
        },

        setComponents: function(value){

            componentsAll = value;
        }
    };

});