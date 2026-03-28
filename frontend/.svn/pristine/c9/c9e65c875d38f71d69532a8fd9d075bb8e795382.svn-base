app.controller('indicatorNoteController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, commonService, indicatorNoteFactory, budgetFactory, notificationFactory) {

   
    $scope.idIndicator = $routeParams.id;      
    $scope.type = $routeParams.type; 

    $scope.overrideOptions = {
        "bStateSave": true,
        "iCookieDuration": 2419200, /* 1 month */
        "bJQueryUI": true,
        "bPaginate": true,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": true,
        "bDestroy": true                
    };

    $scope.columnDefsNotes = [ 
                { "mDataProp": "Data", "aTargets":[0]},
                { "mDataProp": "Nota", "aTargets":[1] },
                { "mDataProp": "Ordem", "aTargets":[2] },
                { "mDataProp": "Prioridade", "aTargets":[3] },
                { "mDataProp": "Descrição", "aTargets":[4] },
                { "mDataProp": "Local", "aTargets":[5] },
                { "mDataProp": "Criado por", "aTargets":[6] },
                { "mDataProp": "Grupo", "aTargets":[7] },
                { "mDataProp": "Tipo de Nota", "aTargets":[8] },
                { "mDataProp": "Status", "aTargets":[9] },                
                { "mDataProp": "C. Custo", "aTargets":[10] }                                
    ]; 

   

    switch($routeParams.type){
        case "Pred":            
            $scope.cdPredictive = true;
            $scope.Title = "Notas Pendentes (Preditiva)";
        break;        
        default:
            $scope.cdPredictive = false;
            $scope.Title = "Notas Pendentes";
        break;
    }

    
    $scope.plantData = [];

    GetBudgets(true);   
    
    $scope.setLocation = function(id){
        
        $scope.selLocation = id;
    };

    $scope.setBudget = function (model){
        
         angular.forEach($scope.budgets, function(obj, key){
             obj.cls = "";
         });
        
        model.cls = "fa fa-check";
        $scope.selectedBudget = model;
    };
    
    $scope.fillBudget = function (model){              
            GetPlantDataNote($scope.idIndicator,model.idBudget,$scope.cdPredictive);
    };
    
    function GetPlantDataNote(idIndicator, idBudget, cdPredictive){
        
        indicatorNoteFactory.GetPlantDataNote(idIndicator, idBudget, cdPredictive).success(function (response){    

            $scope.plantsData = [];
            $scope.locations = [];

            $scope.locations.push({idLocation: -1, Name: "TODAS PLANTAS"});

            $scope.selLocation =  $scope.locations[0].idLocation;
            
            if(response.length > 0){        
                                
            
                angular.forEach(response, function(objPlant, keyPlant){ 
                    
                    var plantData = {
                        idPlant: objPlant.idPlant,
                        plant: objPlant.Plant,                    
                        table: [],
                        labels: [],                    
                        series: [],
                        data: [],                    
                        colors: [],
                        datasetOverride:[],                                                                        
                        tableNotesClosed: [],
                        tableNotesPendingDelay: [],
                        tableNotesPendingOnTime: [],
                    };    

                    $scope.locations.push({idLocation: objPlant.idPlant, Name: objPlant.Plant});
                                
                    angular.forEach(objPlant.tableNotesClosed, function(objDetail, keyDetail){
                        
                        var val = [
                                    $filter('date')(objDetail.InsDateTime,'dd/MM/yyyy') ,                                     
                                    objDetail.idNote, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.cdUser, 
                                    objDetail.PlantGroup, 
                                    objDetail.Type, 
                                    objDetail.Status,                                     
                                    objDetail.CenterCost                            
                                ]; 

                        plantData.tableNotesClosed.push(val);
                    });

                    angular.forEach(objPlant.tableNotesPendingDelay, function(objDetail, keyDetail){
                        
                        var val = [
                                    $filter('date')(objDetail.InsDateTime,'dd/MM/yyyy') , 
                                    objDetail.idNote, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.cdUser, 
                                    objDetail.PlantGroup, 
                                    objDetail.Type, 
                                    objDetail.Status,                                     
                                    objDetail.CenterCost                            
                                ]; 

                        plantData.tableNotesPendingDelay.push(val);
                    });

                    angular.forEach(objPlant.tableNotesPendingOnTime, function(objDetail, keyDetail){
                        
                        var val = [
                                    $filter('date')(objDetail.InsDateTime,'dd/MM/yyyy') , 
                                    objDetail.idNote, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.cdUser, 
                                    objDetail.PlantGroup, 
                                    objDetail.Type, 
                                    objDetail.Status,                                     
                                    objDetail.CenterCost                            
                                ]; 

                        plantData.tableNotesPendingOnTime.push(val);
                    });
                    

                    plantData.colors.push("#666666");
                    plantData.colors.push("#cc0066");
                    plantData.colors.push("#009900");
                   
                                                       
                


                    plantData.datasetOverride = [                         
                                                       
                        {
                            label: "Encerradas",
                            type: 'bar',
                                  
                        },
                        {                    
                            label: "Pendentes Fora do Prazo",
                            type: 'line',
                            pointStyle: 'rectRot',
                            pointRadius: 7, 
                            fill: false                           
                        },
                        {                        
                            label: "Pendentes no Prazo",
                            type: 'line',
                            pointStyle: 'rectRot',
                            pointRadius: 7, 
                            fill: false                              
                        },                                            
                       
                    ];


                                                        
                  
                   // var valuesGoal = [];   
                    var valuesClosed = [];
                    var valuesPendingDelay = [];
                    var valuesPendingOnTime= [];                    
                                       
                                                    
                
                    angular.forEach(objPlant.data, function(objData, keyData){ 
                                        
                        plantData.labels.push(objData.monthYear);
                        
                        //valuesGoal.push(objData.vlGoal); 

                        valuesClosed.push(objData.vlClosed);
                        
                        valuesPendingDelay.push(objData.vlPendingDelay);
                        
                        valuesPendingOnTime.push(objData.vlPending - objData.vlPendingDelay);
                                                                                                                
                        
                    });
                    
                 
                                  
                    
                    //plantData.data.push(valuesGoal);
                    plantData.data.push(valuesClosed);
                    plantData.data.push(valuesPendingDelay);
                    plantData.data.push(valuesPendingOnTime);
                                                                           
                                            
                    $scope.plantsData.push(plantData);
                });



                //Set options Chart
                $scope.options = {
                    responsive: true,
                    
                
                    // scales: {
                    //     yAxes: [{
                    //      stacked: true,
                    //         ticks: {
                    //             beginAtZero:true,                         
                    //         },
                    //     }],
                    //     xAxes: [{
                    //             stacked: true,                                                       
                    //         }                                                                                                     
                    //     ]
                    // },
                
                    
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            generateLabels: function(chart) {

                                var result = [{text: "Encerradas", fillStyle:"#666666" }, {text: "Pendentes Fora do Prazo", fillStyle:"#cc0066" }, {text: "Pendentes no Prazo", fillStyle:"#009900" }]
                                
                                return result;
                                
                            }
                        }
                        
                    },

                    elements: {
                        line: {
                            tension: 0 // disables bezier curves                       
                        },
                        point:{
                            pointStyle: 'circle'
                        }
                    },

                    tooltips: {
                        
                        filter: function(tooltipItem, data) {
                            if(tooltipItem.y)
                                return !data.datasets[tooltipItem.datasetIndex].tooltipHidden; // custom added prop to dataset
                            else
                                return data.datasets[tooltipItem.datasetIndex].tooltipHidden;
                        }
                    }
                    
                };

            }
                   
        }).error(function(error){

            $.bigBox({
            title : "Erro!",
            content : error ? error.Message : "Falha de comunicação com o servidor.",            
            color : "#C46A69",              
            icon : "fa fa-warning swing animated",              
            timeout : 6000 });

        });    
    };
             
    function GetBudgets(active){
        
        budgetFactory.GetBudgets(active).success(function (response){    

        if(response.length > 0){        
            $scope.budgets = response;   
            
            GetActualBudget();
        }

        }).error(function(error){

            $.bigBox({
            title : "Erro!",
            content : error ? error.Message : "Falha de comunicação com o servidor.",            
            color : "#C46A69",              
            icon : "fa fa-warning swing animated",              
            timeout : 6000 });

        });    
    };

    function GetActualBudget(){
        
        budgetFactory.GetActualBudget().success(function (response){    

            if(response){        
                $scope.selectedBudget = response;    
                $scope.selectedBudget.cls = "fa fa-check";    

                $scope.budgets.filter(function(f) { return f.idBudget === $scope.selectedBudget.idBudget})[0].cls = "fa fa-check";
              
                GetPlantDataNote($scope.idIndicator,$scope.selectedBudget.idBudget,$scope.cdPredictive);
            }

        }).error(function(error){

            $.bigBox({
            title : "Erro!",
            content : error ? error.Message : "Falha de comunicação com o servidor.",            
            color : "#C46A69",              
            icon : "fa fa-warning swing animated",              
            timeout : 6000 });

        });    
    };    
          
});    




