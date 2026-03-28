app.controller('modalInstanceController', function ($uibModalInstance, $rootScope, item, equipmentsFactory) {   
  
  var $ctrl = this;  
    
  $ctrl.item = angular.copy(item);

  if ($ctrl.item.action == 'Insert'){
    $ctrl.item.model = null;
    $ctrl.item.model = {
      idEquipment: null,
      CountChilds: 0,
      EquipmentsChilds:[],
      cdEquipment: item.model.idEquipment, 
      Active: true,
      cdIntegrate: false,
      idArea: null,
      idType: null,
      cdUser: $rootScope.user.Login
    };
  }


  $ctrl.save = function (){
    if($ctrl.item.action == 'Insert')
      $ctrl.insert();
    else
      $ctrl.update();

  }

  $ctrl.insert = function () {
        
   // if(isValid){
      equipmentsFactory.InsertEquipment($ctrl.item.model).success(function (response){

          if(response.status){
            
          $ctrl.item.model.idEquipment = response.id;                      
          item.equips.push($ctrl.item.model);

          
          if(item.equipsRoot){
            item.equipsRoot.push($ctrl.item.model);            
          }

          //Validação para nó root
          if($ctrl.item.model.cdEquipment){
            item.model.EquipmentsChilds.push($ctrl.item.model);
            item.model.CountChilds = item.model.EquipmentsChilds.length;

          }
         
          $ctrl.item.service.setEquipments(item.equipmentsAll);
          $uibModalInstance.close($ctrl.item);

          }
          else{                      
            $.bigBox({
              title : "Erro!",
              content : response.message,            
              color : "#C46A69",            
              icon : "fa fa-warning swing animated",
              timeout : 6000 });
          }

                
      }).error(function(error){
                  
        $.bigBox({
          title : "Erro!",
          content : error ? error.Message : "Falha de comunicação com o servidor.",            
          color : "#C46A69",              
          icon : "fa fa-warning swing animated",              
          timeout : 6000 });   
      });    
    //}
  };
  
  $ctrl.update = function () {
      
    //if(isValid){
      $ctrl.item.model.cdUser = $rootScope.user.Login;
      equipmentsFactory.UpdateEquipment($ctrl.item.model).success(function (response){

        if(response.status){            
          copyTo($ctrl.item.model, item.model);  
             
          $ctrl.item.service.setEquipments($ctrl.item.equipmentsAll);
          $uibModalInstance.close($ctrl.item);
        }
        else{
          $.bigBox({
            title : "Erro!",
            content : response.message,            
            color : "#C46A69",            
            icon : "fa fa-warning swing animated",
            timeout : 6000 });
        }         
                
        }).error(function(error){                             
            $.bigBox({
              title : "Erro!",
              content : error ? error.Message : "Falha de comunicação com o servidor.",            
              color : "#C46A69",              
              icon : "fa fa-warning swing animated",              
              timeout : 6000 });   
        });    
   // }
  };
  
  $ctrl.cancel = function () {
      $uibModalInstance.dismiss('cancel');
  };

  //Função responsável por atualizar o objeto original
  function copyTo(srcObj, destObj) {
      for (var key in destObj) {
        if(destObj.hasOwnProperty(key) && srcObj.hasOwnProperty(key)) {
          destObj[key] = srcObj[key];
        }
      }
  }
    
    
});
  
