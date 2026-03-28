app.controller('menuController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window, treeConfig, equipmentsService, equipmentsFactory) {

    

    $scope.service = equipmentsService;

    $scope.$watch('service.getEquipments()', function(){

       // alert('equipmentsService alterado');
        $scope.equipmentsAll = $scope.service.getEquipments();
        GetMenus();

    },
    true
    );

    $scope.treeFilter = $filter('uiTreeFilter');
    $scope.availableFields = ['Name'];
    $scope.supportedFields = ['Name'];

        
   // GetMenus();
   

   

    $scope.expandedChilds = function (scope) {

       // if((scope.$modelValue.EquipmentsChilds == null || scope.$modelValue.EquipmentsChilds.length == 0) && scope.$modelValue.CountChilds > 0)
        GetEquipmentsChilds(scope.$modelValue);
               
        scope.toggle();
    };




    $scope.visibleEquips = function (item) {

        if((item.EquipmentsChilds == null || item.EquipmentsChilds.length == 0)  && $scope.queryEquips && $scope.queryEquips.length > 0 && item.CountActiveChilds > 0)
            GetEquipmentsChilds(item);

        return !($scope.queryEquips && $scope.queryEquips.length > 0
        && item.Name.toUpperCase().indexOf($scope.queryEquips.toUpperCase()) == -1 && item.idEquipment != -1);

    };
    

         
    function GetMenus(){
     
        // if($scope.equipmentsAll && $scope.equipmentsAll.length > 0){
        //     $scope.equipsMenu = $scope.equipmentsAll.filter(function(f){  return f.cdEquipment === null});
        // }    
        // else {
                equipmentsFactory.GetAllEquipments(true).success(function (response){
            
                        if(response.length > 0){
             
                            $scope.equipmentsAll = response;
                            
                            $scope.equipsMenu = $scope.equipmentsAll.filter(function(f){  return f.cdEquipment === null});

                            // $timeout(function(){
                            //     $('nav ul').jarvismenu({
                            //         accordion : true,
                            //         speed : $.menu_speed,
                            //         closedSign : '<em class="fa fa-expand-o"></em>',
                            //         openedSign : '<em class="fa fa-collapse-o"></em>'
                            //     },
                            //     0
                            // );
                            // })                                                                    
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

           
    }

    function GetEquipmentsChilds(scope){
        
        scope.EquipmentsChilds = $scope.equipmentsAll.filter(function(f) { return f.cdEquipment === scope.idEquipment});
        
    }


   

});