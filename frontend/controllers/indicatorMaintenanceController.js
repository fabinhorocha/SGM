app.controller('indicatorMaintenanceController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, commonService, indicatorMaintenanceFactory, budgetFactory) {

    $scope.idIndicator = $routeParams.id ? $routeParams.id : 1 ;
    $scope.Type = $routeParams.type ?  $routeParams.type : 'Exec';

    switch($routeParams.id){
        case "1": 
            if($routeParams.type  == 'Exec')
                $scope.Title = 'Índice de Preventivas Executadas';
            else
                $scope.Title = 'Índice de Preventivas Não Executadas';

            $scope.TitleType = "Especialidades";
        break;
        case "2":
            if($routeParams.type == 'Exec')
                $scope.Title = 'Índice de Preditivas Executadas';
            else
                $scope.Title = 'Índice de Preditivas Não Executadas';
            
            $scope.TitleType = "Técnicas de Preditiva";
        break;
        default:
            $scope.TitleType = "Especialidades";
        break;
    }

    $scope.columnDefs = [ 
        { "mDataProp": "Grupo", "aTargets":[0]},
        { "mDataProp": "Centro Custo", "aTargets":[1] },
        { "mDataProp": "Tipo", "aTargets":[2] },
        { "mDataProp": "Ordem", "aTargets":[3] },
        { "mDataProp": "Descrição", "aTargets":[4] },
        { "mDataProp": "Status", "aTargets":[5] },
        { "mDataProp": "Local", "aTargets":[6] },
        { "mDataProp": "Tipo Atividade", "aTargets":[7] },
        { "mDataProp": "Especialidade", "aTargets":[8] },
        { "mDataProp": "Data Final", "aTargets":[9] }
        
    ]; 
   
    
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

           
    
    $scope.locationData = [];

    GetBudgets(true);   
    

    $scope.setBudget = function (model){
        
         angular.forEach($scope.budgets, function(obj, key){
             obj.cls = "";
         });
        
        model.cls = "fa fa-check";
        $scope.selectedBudget = model;
    };

    $scope.setLocation = function(id){

        $scope.selLocation = id;
    };
    
    $scope.fillBudget = function (model){
        GetLocationData($scope.idIndicator,model.idBudget);
    };

    function GetLocationData(idIndicator, idBudget){
        
        indicatorMaintenanceFactory.GetLocationData(idIndicator, idBudget).success(function (response){    

        $scope.locationsData = [];
        $scope.locations = [];

        $scope.locations.push({idLocation: -1, Name: "TODAS PLANTAS"});

        $scope.selLocation =  $scope.locations[0].idLocation;
        
        if(response.length > 0){        
                            
           
            angular.forEach(response, function(objLocation, keyLocation){ 
                
                var locationData = {
                    idLocation: objLocation.idLocation,
                    location: objLocation.location,
                    table: [],
                    labels: [],
                    labelsNExec: [],
                    series: [],
                    data: [],
                    dataNExec: [],
                    colors: [],
                    datasetOverride:[],
                    datasetOverrideNExec:[],
                    specialitys:[],
                    locals:[]
                };    

                $scope.locations.push({idLocation: objLocation.idLocation, Name: objLocation.location});
                
                angular.forEach(objLocation.dataDetails, function(objDetail, keyDetail){
                    
                    var val = [
                                objDetail.PlantGroup, 
                                objDetail.CenterCost, 
                                objDetail.Type, 
                                objDetail.idOrder,
                                objDetail.Description,
                                objDetail.Status,
                                objDetail.Location,
                                objDetail.TypeActivity,
                                objDetail.speciality.Name,                                
                                $filter('date')(objDetail.dateEnd,'dd/MM/yyyy')                                
                            ]; 
                    locationData.table.push(val);
                });

                
             

                locationData.colors.push("#000099");
                locationData.colors.push("#cc0066");
             
                locationData.colors.push("#000099");
                locationData.colors.push("#cc0066");
               

                locationData.colors.push("#000099");
                locationData.colors.push("#cc0066");
            


                locationData.datasetOverride = [ 
                    {                    
                        label: "Meta",
                        type: 'bar', 
                                                                                                                  
                    },
                    {                        
                        label: "Real",
                        type: 'bar',  
                                                                                                                  
                    },    
                    {
                        label: "Meta",
                        type: 'line' ,
                        pointStyle: 'rectRot',
                        pointRadius: 0,  
                        fill: false                        
                    },                                  
                    {
                        label: "Real",
                        type: 'line',
                        pointStyle: 'rectRot',
                        pointRadius: 7, 
                        fill: false               
                    },
                    
                    {                    
                        label: "Meta",
                        type: 'bar',                        
                    },
                    {                        
                        label: "Real",
                        type: 'bar',                        
                    }
                ];


                locationData.datasetOverrideNExec = [ 
                    
                    
                    {                    
                        label: "Total",
                        type: 'line',   
                        pointStyle: 'rectRot',
                        pointRadius: 7, 
                        fill: false                                                                                                
                    },
                    {                        
                        label: "Total NEXE",
                        type: 'line', 
                        pointStyle: 'rectRot',
                        pointRadius: 7,
                        fill: false                                                                                                       
                    } 
                                                         
                    
                ];
                
                var valuesReal = [];
                var valuesGoal = [];
                var valuesNExec = [];
                var valuesCount = [];
                var valuesRealAcc = [];
                var valuesGoalAcc = [];
                var valuesRealAccActual = [];
                var valuesGoalAccActual = [];
                
            

                angular.forEach(objLocation.dataAcc, function(objData, keyData){                     
                    
                   
                        locationData.labels.push(["Budget",objData.yearToYear]);                                                     
                        valuesRealAcc.push(objData.vlRealAcc);
                        valuesGoalAcc.push(objData.vlGoal);
                        
                        valuesReal.push(null);
                        valuesGoal.push(null); 
                            
                        valuesRealAccActual.push(null);
                        valuesGoalAccActual.push(null);        
                       
                           
                });   
                                    

               
                angular.forEach(objLocation.data, function(objData, keyData){ 

                 if(objData.vlCount > 0){
                        
                    locationData.labels.push(objData.monthYear);
                    
                        valuesReal.push(objData.vlReal);
                        valuesGoal.push(objLocation.dataAccActual.vlGoal); 
                        
                        if(objData.vlNExec > 0){
                            locationData.labelsNExec.push(objData.monthYear);
                            valuesNExec.push(objData.vlNExec);
                            valuesCount.push(objData.vlCount);
                        }        
                        
                        valuesRealAccActual.push(null);
                        valuesGoalAccActual.push(null);
                        
                        valuesRealAcc.push(null);
                        valuesGoalAcc.push(null);
                    }
                });
                
                if(objLocation.dataAccActual){
                    locationData.labels.push(["Total",objLocation.dataAccActual.yearToYear]);
                    valuesRealAccActual.push(objLocation.dataAccActual.vlRealAcc);
                    valuesGoalAccActual.push(objLocation.dataAccActual.vlGoal);
                }

                valuesRealAcc.push(null);
                valuesGoalAcc.push(null);
                
                valuesReal.push(null);
                valuesGoal.push(null);

            
                locationData.data.push(valuesGoalAcc);
                locationData.data.push(valuesRealAcc);
              
               
                
                locationData.data.push(valuesGoal);
                locationData.data.push(valuesReal);
                

                if (valuesNExec.length > 0){
                    locationData.dataNExec.push(valuesCount);
                    locationData.dataNExec.push(valuesNExec);                                                          
                }
                
                
                locationData.data.push(valuesGoalAccActual);                
                locationData.data.push(valuesRealAccActual);

                locationData.specialitys = GetDataDetails(objLocation.specialitys);
                locationData.locals = GetDataDetails(objLocation.locals);
                                        
                $scope.locationsData.push(locationData);
            });



            //Set options Chart
            $scope.options = {
                responsive: true,
                
               
                scales: {
                    yAxes: [{
                       // stacked: true,
                        ticks: {
                            beginAtZero:true,                         
                        },
                    }],
                    xAxes: [{
                            stacked: true,                                                       
                          },                                                                              
                    ]
                },
               
                
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        generateLabels: function(chart) {

                            var result = [{text: "Real", fillStyle:"#cc0066" },{text: "Meta", fillStyle:"#000099" }]
                            
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

                // tooltips: {
                //     mode: 'nearest'
                // }

                tooltips: {
                    
                    filter: function(tooltipItem, data) {
                        if(tooltipItem.y)
                            return !data.datasets[tooltipItem.datasetIndex].tooltipHidden; // custom added prop to dataset
                        else
                            return data.datasets[tooltipItem.datasetIndex].tooltipHidden;
                      }
                }
                
              };
            
        
        
         //Set options Chart
         $scope.optionsNexe = {
            responsive: true,
           
            scales: {
                yAxes: [{
                   // stacked: true,
                    ticks: {
                        beginAtZero:true,                         
                    },
                }],
                xAxes: [{
                        stacked: false,                                                       
                      },                                                                              
                ]
            },
           
            
            legend: {
                display: true,
                position: 'bottom',
                labels: {
                    generateLabels: function(chart) {

                        var result = [{text: "Total NEXE", fillStyle:"#cc0066" },{text: "Total", fillStyle:"#000099" }]
                        
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
                
                GetLocationData($scope.idIndicator,$scope.selectedBudget.idBudget);
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
   
    function GetDataDetails(listData){
        
        var resultsData = [];
        
        angular.forEach(listData, function(obj, key){     
            
            var resultData = {
                id: obj.id,
                name: obj.name,     
                labels: [],
                labelsNExec: [],
                series: [],
                data: [],
                dataNExec: []
            };   

            
            

            var valuesReal = [];
            var valuesGoal = [];
            var valuesNExec = [];
            var valuesCount = [];
            var valuesRealAcc = [];
            var valuesGoalAcc = [];
            var valuesRealAccActual = [];
            var valuesGoalAccActual = [];
            
        

            angular.forEach(obj.dataAcc, function(objData, keyData){                     
                
            
                    resultData.labels.push(["Budget",objData.yearToYear]);                             
                    valuesRealAcc.push(objData.vlRealAcc);
                    valuesGoalAcc.push(objData.vlGoal);
                    
                    valuesReal.push(null);
                    valuesGoal.push(null); 
                        
                    valuesRealAccActual.push(null);
                    valuesGoalAccActual.push(null);        
                
                    
            });   
                                

        
            angular.forEach(obj.data, function(objData, keyData){ 

                if(objData.vlCount > 0){
                    
                    resultData.labels.push(objData.monthYear);
                                    
                    valuesReal.push(objData.vlReal);
                    valuesGoal.push(obj.dataAccActual.vlGoal);                
                    
                    if(objData.vlNExec > 0){
                        resultData.labelsNExec.push(objData.monthYear);
                        valuesCount.push(objData.vlCount);  
                        valuesNExec.push(objData.vlNExec);
                        
                    }      

                    valuesRealAccActual.push(null);
                    valuesGoalAccActual.push(null);
                    
                    valuesRealAcc.push(null);
                    valuesGoalAcc.push(null);
                }   

            });
            
            if(obj.dataAccActual){
                resultData.labels.push(["Total",obj.dataAccActual.yearToYear]);
                valuesRealAccActual.push(obj.dataAccActual.vlRealAcc);
                valuesGoalAccActual.push(obj.dataAccActual.vlGoal);
            }

            valuesRealAcc.push(null);
            valuesGoalAcc.push(null);
            
            valuesReal.push(null);
            valuesGoal.push(null);

        
            resultData.data.push(valuesGoalAcc);
            resultData.data.push(valuesRealAcc);
        

        
            resultData.data.push(valuesGoal);
            resultData.data.push(valuesReal);


            if (valuesNExec.length > 0){
                resultData.dataNExec.push(valuesCount);
                resultData.dataNExec.push(valuesNExec);
            
            }

            resultData.data.push(valuesGoalAccActual);                
            resultData.data.push(valuesRealAccActual);

            resultsData.push(resultData);
        });

        return resultsData;
    }

   
        
                
        

});    



