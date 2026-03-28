app.controller('indicatorOTController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, commonService, indicatorOTFactory, budgetFactory, notificationFactory, orderHeadFactory) {

    $scope.plant = $routeParams.plant;
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
                { "mDataProp": "Prioridade", "aTargets":[2] },
                { "mDataProp": "Descrição", "aTargets":[3] },
                { "mDataProp": "Local", "aTargets":[4] },
                { "mDataProp": "Criado por", "aTargets":[5] },
                { "mDataProp": "Grupo", "aTargets":[6] },
                { "mDataProp": "Tipo de Nota", "aTargets":[7] },
                { "mDataProp": "Status", "aTargets":[8] },                
                { "mDataProp": "C. Custo", "aTargets":[9] }                                
    ]; 

    $scope.columnDefsOts = [ 
        { "mDataProp": "Tipo", "aTargets":[0]},
        { "mDataProp": "Data Inicial", "aTargets":[1] },
        { "mDataProp": "Data Final", "aTargets":[2] },
        { "mDataProp": "Localização", "aTargets":[3] },
        { "mDataProp": "Ordem", "aTargets":[4] },
        { "mDataProp": "Prioridade", "aTargets":[5] },
        { "mDataProp": "Descrição", "aTargets":[6] },
        { "mDataProp": "Local", "aTargets":[7] },
        { "mDataProp": "Status", "aTargets":[8] },                
        { "mDataProp": "Grupo", "aTargets":[9] },   
        { "mDataProp": "Atividade", "aTargets":[10] },   
        { "mDataProp": "C. Custo", "aTargets":[11] },
        { "mDataProp": "Hh", "aTargets":[12] }                                                                
]; 

    switch($routeParams.type){
        case "Pred":            
            $scope.cdPredictive = true;
            $scope.Title = "OT's Pendentes (Preditiva)";
        break;        
        default:
            $scope.cdPredictive = false;
            $scope.Title = "OT's Pendentes";
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
        if($scope.plant)
            GetPlantDataByPlant($scope.idIndicator,model.idBudget,$scope.cdPredictive, $scope.plant);
        else
            GetPlantData($scope.idIndicator,model.idBudget,$scope.cdPredictive);
    };

    $scope.FillNotes = function (idPlantGroup, dtMonth, dtYear, type){
        GetNotification(idPlantGroup, dtMonth, dtYear, type);
    }

    function GetPlantData(idIndicator, idBudget, cdPredictive){
        
        indicatorOTFactory.GetPlantData(idIndicator, idBudget, cdPredictive).success(function (response){    

            $scope.plantsData = [];
            $scope.locations = [];

            $scope.locations.push({idLocation: -1, Name: "TODAS PLANTAS"});

            $scope.selLocation =  $scope.locations[0].idLocation;
            
            if(response.length > 0){        
                                
            
                angular.forEach(response, function(objPlant, keyPlant){ 
                    
                    var plantData = {
                        idPlant: objPlant.idPlant,
                        plant: objPlant.Plant,
                        dtMonth: objPlant.dataAccActual ? objPlant.dataAccActual.dtMonth : null,
                        dtYear: objPlant.dataAccActual ? objPlant.dataAccActual.dtYear: null,
                        table: [],
                        labels: [],                    
                        series: [],
                        data: [],                    
                        colors: [],
                        datasetOverride:[],
                        datasetOverrideEquips:[],
                        equips: null,
                        hoursWorked: null,
                        hoursWorkedDetails: null,
                        countPriorityDetails: null,
                        tableNotesPending: [],
                        tableNotesOpen: [],
                        tableOtsPending: [],
                        tableOtsOpen:[],
                        tableOtsClosed:[]
                    };    

                    $scope.locations.push({idLocation: objPlant.idPlant, Name: objPlant.Plant});
                                
                    angular.forEach(objPlant.tableNotesPending, function(objDetail, keyDetail){
                        
                        var val = [
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy') , 
                                    objDetail.idNote, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.cdUser, 
                                    objDetail.PlantGroup, 
                                    objDetail.Type, 
                                    objDetail.Status,                                     
                                    objDetail.CenterCost                            
                                ]; 

                        plantData.tableNotesPending.push(val);
                    });

                    angular.forEach(objPlant.tableNotesOpen, function(objDetail, keyDetail){
                        
                        var val = [
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy') , 
                                    objDetail.idNote, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.cdUser, 
                                    objDetail.PlantGroup, 
                                    objDetail.Type, 
                                    objDetail.Status,                                     
                                    objDetail.CenterCost                            
                                ]; 

                        plantData.tableNotesOpen.push(val);
                    });

                    angular.forEach(objPlant.tableOtsPending, function(objDetail, keyDetail){
                        
                        var val = [
                                    objDetail.Type,
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy'), 
                                    $filter('date')(objDetail.dateEnd,'dd/MM/yyyy'),
                                    objDetail.SheetLocation, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.Status, 
                                    objDetail.PlantGroup, 
                                    objDetail.TypeActivity,                                     
                                    objDetail.CenterCost,                            
                                    objDetail.hoursWorked
                                ]; 

                        plantData.tableOtsPending.push(val);
                    });

                    angular.forEach(objPlant.tableOtsOpen, function(objDetail, keyDetail){
                        
                        var val = [
                                    objDetail.Type,
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy'), 
                                    $filter('date')(objDetail.dateEnd,'dd/MM/yyyy'),
                                    objDetail.SheetLocation, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.Status, 
                                    objDetail.PlantGroup, 
                                    objDetail.TypeActivity,                                     
                                    objDetail.CenterCost,                            
                                    objDetail.hoursWorked
                                ]; 

                        plantData.tableOtsOpen.push(val);
                    });

                    angular.forEach(objPlant.tableOtsClosed, function(objDetail, keyDetail){
                        
                        var val = [
                                    objDetail.Type,
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy'), 
                                    $filter('date')(objDetail.dateEnd,'dd/MM/yyyy'),
                                    objDetail.SheetLocation, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.Status, 
                                    objDetail.PlantGroup, 
                                    objDetail.TypeActivity,                                     
                                    objDetail.CenterCost,                            
                                    objDetail.hoursWorked
                                ]; 

                        plantData.tableOtsClosed.push(val);
                    });

                    plantData.colors.push("#000099");
                    plantData.colors.push("#cc0066");

                    
                    plantData.colors.push("#000099");
                    plantData.colors.push("#cc0066");


                    plantData.colors.push("#009900");
                    plantData.colors.push("#666666");
                    
                

                    plantData.colors.push("#000099");
                    plantData.colors.push("#cc0066");
                


                    plantData.datasetOverride = [ 
                        {                    
                            label: "Meta",
                            type: 'bar', 
                                                                                                                    
                        },
                        {                        
                            label: "Total Pendentes",
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
                            label: "Total Pendentes",
                            type: 'line',
                            pointStyle: 'rectRot',
                            pointRadius: 7, 
                            fill: false               
                        },
                        {                    
                            label: "Total Abertas",
                            type: 'bar',                        
                        },
                        {                        
                            label: "OT's Encerradas",
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
                            label: "Total Pendentes",
                            type: 'bar',                        
                        }
                    ];


                    plantData.datasetOverrideEquips = [ 
                        
                        {
                            label: "OTs Pendentes",
                            type: 'line' ,
                            pointStyle: 'rectRot',
                            pointRadius: 7,  
                            fill: false                        
                        },                                  
                        
                        {                    
                            label: "Elevado; Muito Elevado",
                            type: 'bar' ,
                                                    
                        }

                        
                        
                    ];

                    plantData.datasetOverrideHoursWorked = [ 
                        
                    
                    {                    
                        label: "Hh Necessárias",
                        type: 'bar' ,
                                                    
                    }
                                        
                    ];

                    
                    
                    var valuesPending = [];
                    var valuesGoal = [];   
                    var valuesOpen = [];
                    var valuesClosed = [];                    
                    var valuesPendingAcc = [];
                    var valuesGoalAcc = [];
                    var valuesPendingAccActual = [];
                    var valuesGoalAccActual = [];
                    
                

                    angular.forEach(objPlant.dataAcc, function(objData, keyData){                     
                        
                        if(objData.vlPending > 0){
                            plantData.labels.push(["Budget",objData.yearToYear]);                                                     
                            valuesPendingAcc.push(objData.vlPending);
                            valuesGoalAcc.push(objData.vlGoal);                  
                            
                            valuesPending.push(null);
                            valuesGoal.push(null); 

                            valuesOpen.push(null);
                            valuesClosed.push(null);
                                
                            valuesPendingAccActual.push(null);
                            valuesGoalAccActual.push(null); 
                        }       
                                                    
                    });   
                                        

                
                    angular.forEach(objPlant.data, function(objData, keyData){ 
                                        
                        plantData.labels.push(objData.monthYear);
                        
                        valuesPending.push(objData.vlPending);
                        valuesGoal.push(objPlant.dataAccActual.vlGoal); 

                        valuesOpen.push(objData.vlOpen);
                        valuesClosed.push(objData.vlClosed);
                                                                        
                        valuesPendingAccActual.push(null);
                        valuesGoalAccActual.push(null);
                        
                        valuesPendingAcc.push(null);
                        valuesGoalAcc.push(null);
                        
                    });
                    
                    if(objPlant.dataAccActual){
                        plantData.labels.push(["Total",objPlant.dataAccActual.yearToYear]);
                        valuesPendingAccActual.push(objPlant.dataAccActual.vlPending);
                        valuesGoalAccActual.push(objPlant.dataAccActual.vlGoal);
                    }

                    valuesPendingAcc.push(null);
                    valuesGoalAcc.push(null);
                    
                    valuesPending.push(null);
                    valuesGoal.push(null);

                    valuesOpen.push(null);
                    valuesClosed.push(null);
                
                    plantData.data.push(valuesGoalAcc);
                    plantData.data.push(valuesPendingAcc);
                
                
                    
                    plantData.data.push(valuesGoal);
                    plantData.data.push(valuesPending);

                    plantData.data.push(valuesOpen);
                    plantData.data.push(valuesClosed);
                    
                                    
                    plantData.data.push(valuesGoalAccActual);                
                    plantData.data.push(valuesPendingAccActual);

                    plantData.equips = GetDataEquips(objPlant.dataEquip);

                    plantData.hoursWorked = GetDataHours(objPlant.data);

                    plantData.hoursWorkedDetails = GetDataHoursDetails(objPlant.dataTotalHours);

                    plantData.countPriorityDetails = GetDataCountDetails(objPlant.dataTotalHours);
                                            
                    $scope.plantsData.push(plantData);
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
                            }                                                                                                     
                        ]
                    },
                
                    
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            generateLabels: function(chart) {

                                var result = [{text: "Meta", fillStyle:"#000099" },{text: "Pendente", fillStyle:"#cc0066" }, {text: "Aberto", fillStyle:"#009900" }, {text: "Encerrado", fillStyle:"#666666" }]
                                
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



                //Set options Chart
                $scope.optionsEquips = {
                    responsive: true,
                    
                
                    scales: {
                        yAxes: [{
                        // stacked: true,
                            ticks: {
                                beginAtZero:true,                         
                            },
                        }],
                        xAxes: [{
                                ticks:{
                                    autoSkip: false
                                },                                                     
                            },                                                                              
                        ]
                    },
                
                    
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            generateLabels: function(chart) {

                                var result = [{text: "OTs Pendentes", fillStyle:"#000099" },{text: "Elevado ; Muito Elevado", fillStyle:"#cc0066" }]
                                
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



                    //Set options Chart
                $scope.optionsHoursWorked = {
                    responsive: true,
                    
                
                    // scales: {
                    //     yAxes: [{
                    //        // stacked: true,
                    //         ticks: {
                    //             beginAtZero:true,                         
                    //         },
                    //     }],
                    //     xAxes: [{
                    //             ticks:{
                    //                 autoSkip: false
                    //             },                                                     
                    //           },                                                                              
                    //     ]
                    // },
                
                    
                    // // legend: {
                    // //     display: true,
                    // //     position: 'bottom',
                    // //     labels: {
                    // //         generateLabels: function(chart) {

                    // //             var result = [{text: "Hh Necessárias", fillStyle:"#cc0066" }]
                                
                    // //             return result;
                    // //         }
                    // //     }
                        
                    // // },
                

                    // tooltips: {
                        
                    //     filter: function(tooltipItem, data) {
                    //         if(tooltipItem.y)
                    //             return !data.datasets[tooltipItem.datasetIndex].tooltipHidden; // custom added prop to dataset
                    //         else
                    //             return data.datasets[tooltipItem.datasetIndex].tooltipHidden;
                    //       }
                    // }
                    
                };
                

                $scope.optionsWorkedDetails = {
                    responsive: true,                              
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            generateLabels: function(chart) {

                                var result = [{text: "Muito Elevado", fillStyle:"#000099" },{text: "Elevado", fillStyle:"#009900" },{text: "Demais Prioridades", fillStyle:"#cc0066" }]
                                
                                return result;
                            }
                        }
                        
                    },
                
                    
                    events: false,
                    tooltips: {
                        enabled: false
                    },
                    hover: {
                        animationDuration: 0
                    },
                    animation: {
                        duration: 1,
                        onComplete: function () {
                            var ctx = this.chart.ctx;
                            ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontFamily, 'normal', Chart.defaults.global.defaultFontFamily);
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            this.data.datasets.forEach(function (dataset) {
                    
                            for (var i = 0; i < dataset.data.length; i++) {
                                var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model,
                                    total = dataset._meta[Object.keys(dataset._meta)[0]].total,
                                    mid_radius = model.innerRadius + (model.outerRadius - model.innerRadius)/2,
                                    start_angle = model.startAngle,
                                    end_angle = model.endAngle,
                                    mid_angle = start_angle + (end_angle - start_angle)/2;
                    
                                var x = mid_radius * Math.cos(mid_angle);
                                var y = mid_radius * Math.sin(mid_angle);
                    
                                ctx.fillStyle = '#fff';
                                if (i == 3){ // Darker text color for lighter background
                                ctx.fillStyle = '#444';
                                }
                    
                                var val = dataset.data[i];
                                var percent = String(Math.round(val/total*100)) + "%";
                    
                                if(val != 0) {
                                ctx.fillText(dataset.data[i], model.x + x, model.y + y);
                                // Display percent in another line, line break doesn't work for fillText
                                ctx.fillText(percent, model.x + x, model.y + y + 15);
                                }
                            }
                            });               
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
          
    function GetPlantDataByPlant(idIndicator, idBudget, cdPredictive, plant){
        
        indicatorOTFactory.GetPlantDataByPlant(idIndicator, idBudget, cdPredictive, plant).success(function (response){    

            $scope.plantsDataByPlant = [];
            $scope.locations = [];                       
            
            if(response.length > 0){        
                                
            
                angular.forEach(response, function(objPlant, keyPlant){ 
                    
                    var plantData = {
                        idPlant: objPlant.idPlant,
                        plant: objPlant.Plant,
                        dtMonth: objPlant.dataAcc ? objPlant.dataAcc.dtMonth : null,
                        dtYear: objPlant.dataAcc ? objPlant.dataAcc.dtYear: null,                       
                        labelsPending: [],                    
                        labelsClosed: [],
                        labelsPendingAcc: ["Notas","Ots Abertas", "Ots Liberadas"],
                        labelsClosedAcc: ["Fora do Prazo","No Prazo"],
                        series: [],
                        dataPending: [],   
                        dataClosed: [],   
                        dataPendingAcc:[], 
                        dataClosedAcc:[],                       
                        colorsPending: [],
                        colorsClosed: [],
                        colorsPendingAcc:["#009900","#666666","#000099"],
                        colorsClosedAcc:["#009900","#cc0066"],
                        datasetOverridePending:[],
                        datasetOverrideClosed:[],                       
                        tablePending: [],                        
                        tableOpen:[],
                        tableReleased:[],
                        tableClosedOnTime:[],
                        tableClosedDelay:[]
                    };    

                    $scope.locations.push({idLocation: objPlant.idPlant, Name: objPlant.Plant});
                    $scope.selLocation =  $scope.locations[0].idLocation;
                                
                    angular.forEach(objPlant.tablePending, function(objDetail, keyDetail){
                        
                        var val = [
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy') , 
                                    objDetail.idNote, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.cdUser, 
                                    objDetail.PlantGroup, 
                                    objDetail.Type, 
                                    objDetail.Status,                                     
                                    objDetail.CenterCost                            
                                ]; 

                        plantData.tablePending.push(val);
                    });
                                       

                    angular.forEach(objPlant.tableOpen, function(objDetail, keyDetail){
                        
                        var val = [
                                    objDetail.Type,
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy'), 
                                    $filter('date')(objDetail.dateEnd,'dd/MM/yyyy'),
                                    objDetail.SheetLocation, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.Status, 
                                    objDetail.PlantGroup, 
                                    objDetail.TypeActivity,                                     
                                    objDetail.CenterCost,                            
                                    objDetail.hoursWorked
                                ]; 

                        plantData.tableOpen.push(val);
                    });

                    angular.forEach(objPlant.tableReleased, function(objDetail, keyDetail){
                        
                        var val = [
                                    objDetail.Type,
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy'), 
                                    $filter('date')(objDetail.dateEnd,'dd/MM/yyyy'),
                                    objDetail.SheetLocation, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.Status, 
                                    objDetail.PlantGroup, 
                                    objDetail.TypeActivity,                                     
                                    objDetail.CenterCost,                            
                                    objDetail.hoursWorked
                                ]; 

                        plantData.tableReleased.push(val);
                    });

                    angular.forEach(objPlant.tableClosedOnTime, function(objDetail, keyDetail){
                        
                        var val = [
                                    objDetail.Type,
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy'), 
                                    $filter('date')(objDetail.dateEnd,'dd/MM/yyyy'),
                                    objDetail.SheetLocation, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.Status, 
                                    objDetail.PlantGroup, 
                                    objDetail.TypeActivity,                                     
                                    objDetail.CenterCost,                            
                                    objDetail.hoursWorked
                                ]; 

                        plantData.tableClosedOnTime.push(val);
                    });

                    angular.forEach(objPlant.tableClosedDelay, function(objDetail, keyDetail){
                        
                        var val = [
                                    objDetail.Type,
                                    $filter('date')(objDetail.dateStart,'dd/MM/yyyy'), 
                                    $filter('date')(objDetail.dateEnd,'dd/MM/yyyy'),
                                    objDetail.SheetLocation, 
                                    objDetail.idOrder, 
                                    objDetail.cdPriority, 
                                    objDetail.Description, 
                                    objDetail.Location, 
                                    objDetail.Status, 
                                    objDetail.PlantGroup, 
                                    objDetail.TypeActivity,                                     
                                    objDetail.CenterCost,                            
                                    objDetail.hoursWorked
                                ]; 

                        plantData.tableClosedDelay.push(val);
                    });

                    plantData.colorsPending.push("#cc0066");                  
                    plantData.colorsPending.push("#009900");
                    plantData.colorsPending.push("#666666");
                    plantData.colorsPending.push("#000099");
                    plantData.colorsPending.push("#cc0066");
                  
                    


                    plantData.colorsClosed.push("#cc0066");
                    plantData.colorsClosed.push("#000099");
                    plantData.colorsClosed.push("#009900");

                    
                    

                   


                    plantData.datasetOverridePending = [ 
                        {
                            label: "Ots/Notas Pendentes",
                            type: 'line' ,
                            pointStyle: 'rectRot',
                            pointRadius: 7,  
                            fill: false                        
                        }, 
                        {                    
                            label: "Notas",
                            type: 'bar', 
                                                                                                                    
                        },
                        {                        
                            label: "Ots Abertas",
                            type: 'bar',  
                                                                                                                    
                        }, 
                        {                        
                            label: "Ots Liberadas",
                            type: 'bar',  
                                                                                                                    
                        },   
                        
                        {                        
                            label: "Acumulado",
                            type: 'bar',  
                                                                                                                    
                        },                                  
                        
                    ];


                    plantData.datasetOverrideClosed = [ 
                        {                    
                            label: "Encerradas no Prazo",
                            type: 'bar', 
                                                                                                                    
                        },
                        {                        
                            label: "Total Encerradas",
                            type: 'bar',  
                                                                                                                    
                        }, 

                        
                        
                    ];

                                       
                    
                    var valuesPending = [];
                    var valuesOpen = [];   
                    var valuesReleased = [];
                    var valuesTotal = [];
                    var valuesTotalACC = [];


                    var valuesPendingACC = [];
                    var valuesOpenACC = [];   
                    var valuesReleasedACC = [];

                   
                   


                    var valuesClosedOnTime = [];                    
                    var valuesClosed = [];
                    var valuesClosedOnTimeACC = [];                    
                    var valuesClosedACC = [];
                                                    
                                                      
                
                    angular.forEach(objPlant.data, function(objData, keyData){ 
                                        
                        plantData.labelsPending.push(objData.monthYear);
                        plantData.labelsClosed.push(objData.monthYear);
                        
                        // Ots/Notas Pendentes
                        valuesPending.push(objData.vlPending);
                        valuesOpen.push(objData.vlOpen);
                        valuesReleased.push(objData.vlReleased);
                        valuesTotal.push(objData.vlPending+objData.vlOpen+objData.vlReleased);
                        valuesTotalACC.push(null);
                        
                        //Ots Encerradas
                        valuesClosedOnTime.push(objData.vlClosedOnTime);
                        valuesClosed.push(objData.vlClosed);
                        
                    });
                    
                    if(objPlant.dataAcc){
                        plantData.labelsPending.push(["Acum",objPlant.dataAcc.yearToYear]);
                        valuesPending.push(null);
                        valuesOpen.push(null);
                        valuesReleased.push(null);
                        valuesTotal.push(null);
                        valuesTotalACC.push(objPlant.dataAcc.vlPending + objPlant.dataAcc.vlOpen + objPlant.dataAcc.vlReleased);
                        

                        plantData.dataPendingAcc.push(objPlant.dataAcc.vlPending);
                        plantData.dataPendingAcc.push(objPlant.dataAcc.vlOpen);
                        plantData.dataPendingAcc.push(objPlant.dataAcc.vlReleased);
                       
                    
                    }


                    if(objPlant.dataTotal){

                        plantData.dataClosedAcc.push(objPlant.dataTotal.vlClosed - objPlant.dataTotal.vlClosedOnTime);
                        plantData.dataClosedAcc.push(objPlant.dataTotal.vlClosedOnTime);                        
                    }

                   
                    plantData.dataPending.push(valuesTotal);
                    plantData.dataPending.push(valuesPending);
                    plantData.dataPending.push(valuesOpen);
                    plantData.dataPending.push(valuesReleased);                   
                    plantData.dataPending.push(valuesTotalACC);


                    plantData.dataClosed.push(valuesClosedOnTime);
                    plantData.dataClosed.push(valuesClosed);
                                                                  
                                            
                    $scope.plantsDataByPlant.push(plantData);
                });



                //Set options Chart
                $scope.optionsPending = {
                    responsive: true,
                    
                
                    scales: {
                        yAxes: [{
                          stacked: true,
                            ticks: {
                                beginAtZero:true,                         
                            },
                        }],
                        xAxes: [{
                                stacked: true,                                                       
                            }                                                                                                     
                        ]
                    },                  
                    
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            generateLabels: function(chart) {

                                var result = [{text: "Notas", fillStyle:"#009900" },{text: "Ots Abertas", fillStyle:"#666666" }, {text: "Ots Liberadas", fillStyle:"#000099" }, {text: "Ots/Notas Pendentes", fillStyle:"#cc0066" }]
                                
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



                //Set options Chart
                $scope.optionsClosed = {
                    responsive: true,
                    
                
                    scales: {
                        yAxes: [{
                          stacked: true,
                            ticks: {
                                beginAtZero:true,                         
                            },
                        }],
                        xAxes: [{
                                stacked: true,                                                       
                            }                                                                                                     
                        ]
                    },           
                    
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            generateLabels: function(chart) {

                                var result = [{text: "Encerradas no Prazo", fillStyle:"#cc0066" },{text: "Total Encerradas", fillStyle:"#000099" }]
                                
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



                 

                $scope.optionsPendingPie = {
                    responsive: true,                              
                    legend: {
                        display: true,
                        position: 'right',
                        labels: {
                            generateLabels: function(chart) {

                                var result = [{text: "Notas", fillStyle:"#009900" },{text: "Ots Abertas", fillStyle:"#666666" }, {text: "Ots Liberadas", fillStyle:"#000099" }]
                                
                                return result;
                            }
                        }
                        
                    },                                    
                    events: false,
                    tooltips: {
                        enabled: false
                    },
                    hover: {
                        animationDuration: 0
                    },
                    animation: {
                        duration: 1,
                        onComplete: function () {
                            var ctx = this.chart.ctx;
                            ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontFamily, 'normal', Chart.defaults.global.defaultFontFamily);
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            this.data.datasets.forEach(function (dataset) {
                    
                            for (var i = 0; i < dataset.data.length; i++) {
                                var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model,
                                    total = dataset._meta[Object.keys(dataset._meta)[0]].total,
                                    mid_radius = model.innerRadius + (model.outerRadius - model.innerRadius)/2,
                                    start_angle = model.startAngle,
                                    end_angle = model.endAngle,
                                    mid_angle = start_angle + (end_angle - start_angle)/2;
                    
                                var x = mid_radius * Math.cos(mid_angle);
                                var y = mid_radius * Math.sin(mid_angle);
                    
                                ctx.fillStyle = '#fff';
                                if (i == 3){ // Darker text color for lighter background
                                ctx.fillStyle = '#444';
                                }
                    
                                var val = dataset.data[i];
                                var percent = String(Math.round(val/total*100)) + "%";
                    
                                if(val != 0) {
                                ctx.fillText(dataset.data[i], model.x + x, model.y + y);
                                // Display percent in another line, line break doesn't work for fillText
                                ctx.fillText(percent, model.x + x, model.y + y + 15);
                                }
                            }
                            });               
                        }
                
                    }
                };

                $scope.optionsClosedPie = {
                    responsive: true,                              
                    legend: {
                        display: true,
                        position: 'right',
                        labels: {
                            generateLabels: function(chart) {

                                var result = [{text: "Fora do Prazo", fillStyle:"#009900" },{text: "No Prazo", fillStyle:"#cc0066" }]
                                
                                return result;
                            }
                        }
                        
                    },                                    
                    events: false,
                    tooltips: {
                        enabled: false
                    },
                    hover: {
                        animationDuration: 0
                    },
                    animation: {
                        duration: 1,
                        onComplete: function () {
                            var ctx = this.chart.ctx;
                            ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontFamily, 'normal', Chart.defaults.global.defaultFontFamily);
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            this.data.datasets.forEach(function (dataset) {
                    
                            for (var i = 0; i < dataset.data.length; i++) {
                                var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model,
                                    total = dataset._meta[Object.keys(dataset._meta)[0]].total,
                                    mid_radius = model.innerRadius + (model.outerRadius - model.innerRadius)/2,
                                    start_angle = model.startAngle,
                                    end_angle = model.endAngle,
                                    mid_angle = start_angle + (end_angle - start_angle)/2;
                    
                                var x = mid_radius * Math.cos(mid_angle);
                                var y = mid_radius * Math.sin(mid_angle);
                    
                                ctx.fillStyle = '#fff';
                                if (i == 3){ // Darker text color for lighter background
                                ctx.fillStyle = '#444';
                                }
                    
                                var val = dataset.data[i];
                                var percent = String(Math.round(val/total*100)) + "%";
                    
                                if(val != 0) {
                                ctx.fillText(dataset.data[i], model.x + x, model.y + y);
                                // Display percent in another line, line break doesn't work for fillText
                                ctx.fillText(percent, model.x + x, model.y + y + 15);
                                }
                            }
                            });               
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
                
                if($scope.plant)
                    GetPlantDataByPlant($scope.idIndicator,$scope.selectedBudget.idBudget,$scope.cdPredictive, $scope.plant);
                else
                    GetPlantData($scope.idIndicator,$scope.selectedBudget.idBudget,$scope.cdPredictive);
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
   
    function GetDataEquips(listData){
        
        var resultData = { 
            labels: [],
            series: [],
            data: [],  
            colors: []              
        };   

        resultData.colors.push("#000099");
        resultData.colors.push("#cc0066");
        
        var valuesPending = [];
        var valuesHigh = [];

        angular.forEach(listData, function(obj, key){     
                                          
                   
            resultData.labels.push(obj.name);
            
            valuesPending.push(obj.vlPending);
            valuesHigh.push(obj.vlHigh);
                    
          
        
        });

        resultData.data.push(valuesPending);
        resultData.data.push(valuesHigh);

        return resultData;
    }

    function GetDataHours(listData){
        
        var resultData = { 
            labels: [],
            series: [],
            data: [],  
            colors: []              
        };   

        //resultData.colors.push("#000099");
        resultData.colors.push("#cc0066");
        
        var valuesHours = [];
       

        angular.forEach(listData, function(obj, key){     
                                                             
            resultData.labels.push(obj.monthYear);            
            valuesHours.push(obj.vlHours);                                          
        
        });

        resultData.data.push(valuesHours);

        return resultData;
    }

    function GetDataHoursDetails(listData, type){
        
        var resultData = { 
            labels: ["Demais Prioridades","Elevado","Muito Elevado"],
            series: [],
            data: [],  
            colors: []              
        };   

        resultData.colors.push("#cc0066");
        resultData.colors.push("#009900");
        resultData.colors.push("#000099");
                      
 
        angular.forEach(listData, function(obj, key){     
         
                     
            resultData.data.push(obj.vlHoursOther);
            resultData.data.push(obj.vlHoursHigh);
            resultData.data.push(obj.vlHoursVeryHigh);
        
        });

     

        return resultData;
    }
        
    function GetDataCountDetails(listData, type){
        
        var resultData = { 
            labels: ["Demais Prioridades","Elevado","Muito Elevado"],
            series: [],
            data: [],  
            colors: []              
        };   

        resultData.colors.push("#cc0066");
        resultData.colors.push("#009900");
        resultData.colors.push("#000099");
                      
 
        angular.forEach(listData, function(obj, key){     
         
                     
            resultData.data.push(obj.vlCountOther);
            resultData.data.push(obj.vlCountHigh);
            resultData.data.push(obj.vlCountVeryHigh);
        
        });

     

        return resultData;
    }            
        
    function GetNotification(idPlantGroup, dtMonth, dtYear, type){
        
        notificationFactory.GetNotification(idPlantGroup, dtMonth, dtYear, type).success(function (response){    

        if(response.length > 0){   
            
            // $scope.columnDefs = [ 
            //     { "mDataProp": "Data", "aTargets":[0]},
            //     { "mDataProp": "Nota", "aTargets":[1] },
            //     { "mDataProp": "Prioridade", "aTargets":[2] },
            //     { "mDataProp": "Descrição", "aTargets":[3] },
            //     { "mDataProp": "Local", "aTargets":[4] },
            //     { "mDataProp": "Criado por", "aTargets":[5] },
            //     { "mDataProp": "Grupo", "aTargets":[6] },
            //     { "mDataProp": "Tipo de Nota", "aTargets":[7] },
            //     { "mDataProp": "Status", "aTargets":[8] },
            //     { "mDataProp": "Localização", "aTargets":[9] },
            //     { "mDataProp": "C. Custo", "aTargets":[10] }                                
            // ]; 

            // var columns = [ 
            //     { title: "Data"},
            //     { title: "Nota"},
            //     { title: "Prioridade"},
            //     { title: "Descrição"},
            //     { title: "Local"},
            //     { title: "Criado por"},
            //     { title: "Grupo"},
            //     { title: "Tipo de Nota"},
            //     { title: "Status"},
            //     { title: "Localização"},
            //     { title: "C. Custo"}                                
            // ]; 

           
           

            
           var notifications = [];
           angular.forEach(response, function(obj, key){
               
                var data = [obj.dateStart, obj.idNote, obj.cdPriority, obj.Description, obj.Location, obj.cdUser, obj.PlantGroup, obj.Type, obj.Status, obj.SheetLocation, obj.CenterCost];
                
                notifications.push(data);
            });


           LoadDataTable("#tTableNotesPendingDetails"+idPlantGroup, notifications, columns);
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

    function GetOrderHead(idPlantGroup, dtMonth, dtYear, type){
        
        orderHeadFactory.GetOrderHead(idPlantGroup, dtMonth, dtYear, type).success(function (response){    

        if(response.length > 0){        
            $scope.orderHeads = response;                         
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

    function LoadDataTable(id, data, columns){
        
            $(id).dataTable({
                "data": data,
                "columns" : [
                    { "data" : "dateStart" },
                    { "data" : "idNote" },
                    { "data" : "cdPriority" },
                    { "data" : "Description" },
                    { "data" : "Location" },
                    { "data" : "cdUser" },
                    { "data" : "PlantGroup" },
                    { "data" : "Type" },
                    { "data" : "Status" },
                    { "data" : "SheetLocation" },
                    { "data" : "CenterCost" }
                 ],
                // "sDom" : "<'dt-top-row'Tlf>r<'dt-wrapper't><'dt-row dt-bottom-row'<'row'<'col-sm-6'i><'col-sm-6 text-right'p>>",
                // "oTableTools" : {
                //     "aButtons" : [ {
                //         "sExtends" : "collection",
                //         "sButtonText" : 'Salvar <span class="caret" />',
                //         "aButtons" : ["xls", "pdf"]
                //     }],
                //     "sSwfPath" : "content/vendor/plugin/datatables/media/swf/copy_csv_xls_pdf.swf"
                // },
                // "fnInitComplete" : function(oSettings, json) {
                //     $(this).closest('#dt_table_tools_wrapper').find('.DTTT.btn-group').addClass('table_tools_group').children('a.btn').each(function() {
                //         $(this).addClass('btn-sm btn-default');
                //     });
                // }
            });
        
            // watch for any changes to our data, rebuild the DataTable
            // $scope.$watch(data, function(value) {
            //     var val = value || null;
            //     if (val) {
            //         dataTable.fnClearTable();
            //         dataTable.fnAddData(scope.$eval(attrs.aaData));
            //     }
            // });
        }
});    




