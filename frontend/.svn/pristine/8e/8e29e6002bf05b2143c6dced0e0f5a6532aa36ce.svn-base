app.run(function($rootScope, uibPaginationConfig, userFactory) {
    
    uibPaginationConfig.firstText='Primeiro';
    uibPaginationConfig.previousText='Anterior';
    uibPaginationConfig.nextText='Próximo';
    uibPaginationConfig.lastText='Último';

    GetUser();

    function GetUser(){
        
            userFactory.GetUser().success(function (response){
        
                    if(response){
        
                        $rootScope.user= response;
                                                                                                                
                    }            
                }).error(function(error){
                    
                    $.bigBox({
                        title : "Erro!",
                        content : error ? error.Message : "Falha de comunicação com o servidor.",            
                        color : "#C46A69",              
                        icon : "fa fa-warning swing animated",              
                        timeout : 6000 });
        
                });    
  
              
    }

});