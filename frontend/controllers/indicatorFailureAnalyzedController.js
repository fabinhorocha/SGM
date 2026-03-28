
app.controller('indicatorFailureAnalyzedController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, commonService, indicatorFailureAnalyzedFactory, budgetFactory, equipmentPlantGroupFactory) {

   
    //$scope.startDate = $filter('date')(new Date().setYear(new Date().getFullYear() - 1), 'dd/MM/yyyy');
    $scope.startDate = $filter('date')(new Date().setMonth(new Date().getMonth() - 1), 'dd/MM/yyyy');
    $scope.endDate = $filter('date')(new Date(),'dd/MM/yyyy');
    $scope.Title = "Análise de Falhas"      
    $scope.idIndicator = 4;      
    $scope.failureFactories = []    
    $scope.failureAreas = [];
    $scope.failureEquipments = [];
    $scope.failureWorkCenters = [];
    $scope.lastMonthYear = "";
    $scope.budgets = [];
    $scope.titleViewName = "Exibir Nome da Planta"; 
    $scope.viewName = false; 

    $scope.filterTypes = [
        {id:1, Name: "Período", cls:"fa fa-check"},
        {id:2, Name: "Budget", cls:""},
    ]

    $scope.selectedFilterType = $scope.filterTypes[0];

    $scope.hgt = $(window).height();
    
    var optionsChart = {
        responsive: true,
        aspectRatio: 2,
        maintainAspectRatio: false,
        title: {
            display: false,
            text: "Dados"
        },
       
        layout: {
            padding: {            
            bottom: 45
            }
        },
        scales: {
            xAxes: [{
            type: 'hierarchical',            
            offset: true,
            stacked: true,    
            display: true,
            gridLines: {
                offsetGridLines: true
            },
            hierarchySpanWidth: 1,                                       
            },
        
        ],
            yAxes: [{
                stacked: true,
               
            ticks: {
                beginAtZero: true                               
            }
            }]
        },
        tooltips:{
            yAlign: 'bottom',
            xAlign: 'center',            
            //yPadding: -35,
           // xPadding: -10,
            //_bodyAlign: 'left',
           // _footerAlign: 'left'
           filter: function(tooltipItem, data) {
            if(tooltipItem.y){
             
                var contentVal = false;
                angular.forEach(data.datasets[tooltipItem.datasetIndex].data, function(val, index){
                    if(!contentVal && val != 0){
                        contentVal = true;                     
                    }
                });
             
                if(!contentVal)
                    return data.datasets[tooltipItem.datasetIndex].tooltipHidden;

                return !data.datasets[tooltipItem.datasetIndex].tooltipHidden; // custom added prop to dataset
            }
            else
                return data.datasets[tooltipItem.datasetIndex].tooltipHidden;
          }
        },
        
        plugins:{
            datalabels:{
                color: 'white',	
                align: 'center',                
                formatter: function(value, context){
                    if(value == 0)
                        return "";
                    else
                        return value;
                },
                // display: function(context) {
                //     return context.dataset.data[context.dataIndex] > 2;
                // },
                // font: {
                //     weight: 'bold',
                //     size: 9
                // },
                font: function(context) {

                    switch(context.dataset.data[context.dataIndex]){

                        case 1: return { size: 6 };
                        case 2: return { size: 7 };
                        default: return { size: 10 };

                    }

                    
                  }
            }
        },
        // animation: {

        //     onProgress: function () {
                
        //         var chartInstance = this.chart;            
                
        //         $scope.$apply(function () {                        
        //             $scope.widthChartFailure = chartInstance.width                                         
        //         });
                



        //     }


        // },
        legendCallback: function(chart) { 
            
            var text = [];
            if(chart.data.datasets.length > 0){            
                var label = '';
                text.push('<table width=100%>');
                text.push('<tr height=25px>');
                text.push('<td align=center>&nbsp;&nbsp;<span>M2:&nbsp;&nbsp;</span>');
                text.push('</td>');
                for (var i=0; i<4; i++) {
                    
                    text.push((i == 3) ? '<td colspan=4>' :  '<td>');                                                
                    label = chart.data.datasets[i].label.replace('M2 ','');               
                    text.push('<span style="background-color:' + chart.data.datasets[i].borderColor + '">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span>'+label+'</span>&nbsp;&nbsp;');
                    text.push('</td>');
                }
                text.push('</tr>');

                text.push('<tr>');
                 text.push('<td align=center>&nbsp;&nbsp;<span>FA:&nbsp;&nbsp;</span>');
                text.push('</td>');
                for (var i=4; i<chart.data.datasets.length; i++) {
                    
                    text.push('<td>');                                                          
                    label = chart.data.datasets[i].label.replace('FA ','');               
                    text.push('<span style="background-color:' + chart.data.datasets[i].borderColor + '">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span>'+label+'</span>&nbsp;&nbsp;');
                    text.push('</td>');

                }
                text.push('</tr>');
                text.push('</table>');
            }

            return text.join("");

        }        
    };

    $scope.failureAnalyzedData = {
        options: optionsChart,            
        data: { labels : [], datasets: []},
        detailsDataAnalyzed: []          
    };

    GetBudgets(true);

   


    $scope.options = {
        responsive: true,
        scales: {
            yAxes: [{
                stacked: true,
                ticks: {
                    beginAtZero: true,
                },
                scaleLabel: {
                    display: true,
                    labelString: 'Quantidade'
                },
                ticks: {
                    callback: function (label, index, labels) {
                            return label;
                    }
                },
            }],
            xAxes: [{
                id:'xAxis1',
                type:'category',
                display: true,   
                position: 'top',
                gridLines:{
                    drawOnChartArea: true,
                },               
                //barThickness: 30,
                stacked: true,
                ticks: {
                    callback:function(label){
                        var sheet = label.split(";")[0];
                        var monthYear = label.split(";")[1];
                        
                        if(sheet.split('-').length == 2)
                                return $scope.failureFactories.filter(function(e){ return e.Sheet ===  sheet})[0].Name + " - "+monthYear;
                            else
                                if(sheet.split('-').length == 3)
                                    return $scope.failureAreas.filter(function(e){ return e.Sheet ===  sheet})[0].Name+ " - "+monthYear;
                                        else                                
                                            return $scope.failureEquipments.filter(function(e){ return e.Sheet ===  sheet})[0].Name+ " - "+monthYear;                        
                        
                     
                    }
                },
            },
            {
                id:'xAxis2',
                type:'category',
                gridLines:{
                    drawOnChartArea: false,
                },             
                stacked: true,
                ticks: {
                    callback:function(label,e,arr){
                        var sheet = label.split(";")[0];
                        var monthYear = label.split(";")[1];

                        var listLabels = arr.filter(function(e){ return  e.split(";")[1] === monthYear ;});
                        
                        if(sheet.split('-').length == 2){
                            var position = ($scope.failureFactories.length/2)|0;
                            
                            if(listLabels.indexOf(label) == position-1)
                                return monthYear;                                                        

                        }


                        return "";
                        // if($scope.lastMonthYear == monthYear)
                        //     return "";
                        // else{
                        //     $scope.lastMonthYear = monthYear;
                        //     return monthYear;
                        // }
                        // if(month === "07")
                        //     return year;
                        // else    
                        //     return "";
                    }
                },
            }
            ]
        },

        // plugins:{
        //     datalabels:{
        //         align: 'start',
        //         formatter: function(value, context){
        //             if(value == 0)
        //                 return "";
        //             else
        //                 return context.chart.data.labels[context.dataIndex];
        //         }
        //     }
        // },

        legend: {
            display: false,
            position: 'right',            
            //labels: {
               // generateLabels: function (chart) {
                    // var result = [
                    //                 [{ text: "Tipo 1", fillStyle: "rgb(0, 0, 153)" }, { text: "Tipo 2", fillStyle: "rgb(204, 0, 102)" }, { text: "Tipo 3", fillStyle: "rgb(0,153,0)" }, { text: "Tipo 4", fillStyle: "rgb(255,0,0)" }],
                    //                 [{ text: "Tipo 1", fillStyle: "rgb(0, 0, 153)" }, { text: "Tipo 2", fillStyle: "rgb(204, 0, 102)" }, { text: "Tipo 3", fillStyle: "rgb(0,153,0)" }, { text: "Tipo 4", fillStyle: "rgb(255,0,0)" }]
                    //             ]
                    // return result;
                 //   chart.generateLabels();
                    
                //}
           // }
           
            

       },        
        
        // elements: {
        //     line: {
        //         tension: 0 // disables bezier curves                       
        //     },
        //     point: {
        //         pointStyle: 'circle'
        //     }
        // },

        tooltips: {

            filter: function (tooltipItem, data) {

                if (tooltipItem.y)
                    return !data.datasets[tooltipItem.datasetIndex].tooltipHidden; // custom added prop to dataset
                else
                    return data.datasets[tooltipItem.datasetIndex].tooltipHidden;
            }
        },

    };




    $scope.setFilterType = function (model){
        
        angular.forEach($scope.filterTypes, function(obj, key){
            obj.cls = "";
        });
       
       model.cls = "fa fa-check";
       $scope.selectedFilterType = model;                   

        switch($scope.selectedFilterType.id){
            case 1: 
                $scope.startDate = $filter('date')(new Date().setMonth(new Date().getMonth() - 1), 'dd/MM/yyyy');
                $scope.endDate = $filter('date')(new Date(),'dd/MM/yyyy');
            break;
            case 2: 
                $scope.startDate = $filter('date')($scope.budgets[1].dateStart, 'dd/MM/yyyy');
                $scope.endDate = $filter('date')($scope.budgets[0].dateEnd,'dd/MM/yyyy');
            break;
        }


   };

    $scope.selectedLevelMenuChange = function (level, scope) {
        var workCenter = scope.failureWorkCenter == null || scope.failureWorkCenter.Name == "TODOS" ? null : scope.failureWorkCenter.Name;
        $scope.lastMonthYear = "";
        switch(level){
            case 1:
                
                if( scope.failureFactory.Sheet){
                    $scope.titleViewName = "Exibir Nome da Área"; 
                    GetEquipmentsPlantGroup(scope, 3, scope.failureFactory.Sheet, workCenter);                   
                }
                else{
                    $scope.titleViewName = "Exibir Nome da Planta"; 
                    $scope.failureAreas = [];
                    $scope.failureEquipments = [];

                    GetEquipmentsPlantGroup(scope, 2, null, workCenter);
                }
                
            break;
            case 2:
               
                if(scope.failureArea){
                    if( scope.failureArea.Sheet){
                        $scope.titleViewName = "Exibir Nome do Equipamento"; 
                        GetEquipmentsPlantGroup(scope, 4, scope.failureArea.Sheet, workCenter);                       
                    }
                    else {                   
                        $scope.titleViewName = "Exibir Nome da Área"; 
                        $scope.failureEquipments = [];
                        FillChart(scope, scope.failureFactory.Sheet, workCenter);
                    }
                                        
                }
                
            break;
            case 3:       
               
                if(scope.failureEquipment){
                    if( scope.failureEquipment.Sheet)    {                                          
                        FillChart(scope, scope.failureEquipment.Sheet, workCenter);                            
                    }            
                    else {
                        FillChart(scope, scope.failureArea.Sheet, workCenter); 
                    }                                                           
                }
                
            break;
            case 4:
                if(scope.failureEquipment && scope.failureEquipment.Sheet)               
                    FillChart(scope, scope.failureEquipment.Sheet, workCenter);
                else
                    if(scope.failureArea && scope.failureArea.Sheet)               
                        FillChart(scope, scope.failureArea.Sheet, workCenter);
                    else
                        if(scope.failureFactory && scope.failureFactory.Sheet)               
                            FillChart(scope, scope.failureFactory.Sheet, workCenter);
                        else
                            FillChart(scope, null, workCenter);
            break;
        break;
            
            break;
        }
    }


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
        { "mDataProp": "idNoteM2", "aTargets":[0] },
        { "mDataProp": "idNoteFa", "aTargets":[1] },
        { "mDataProp": "SheetEquip", "aTargets":[2] },
        { "mDataProp": "NameEquip", "aTargets":[3] },
        { "mDataProp": "dateStart", "aTargets":[4] },
        { "mDataProp": "dateEnd", "aTargets":[5] },
        { "mDataProp": "dateRef", "aTargets":[6] },
        { "mDataProp": "InsDateTime", "aTargets":[7] },
        { "mDataProp": "CenterCost", "aTargets":[8] },                
        { "mDataProp": "cdTypeOperation", "aTargets":[9] }, 
        { "mDataProp": "statusM2", "aTargets":[10] },  
        { "mDataProp": "statusFA", "aTargets":[11] }                               
    ]; 
    

    $scope.fillChart = function (scope, setViewName){        
        
        if(setViewName)
            $scope.viewName = !$scope.viewName;
        $scope.lastMonthYear = "";

        if(scope.failureEquipment != null && scope.failureEquipment.Sheet != null)
            GetNotesFailure(scope, scope.failureEquipment.Sheet);
        else
            if(scope.failureArea != null && scope.failureArea.Sheet != null)
                GetNotesFailure(scope, scope.failureArea.Sheet);
            else
                if(scope.failureFactory != null && scope.failureFactory.Sheet != null)
                    GetNotesFailure(scope, scope.failureFactory.Sheet);
                else
                    GetNotesFailure(scope, null);
       
    };
    
    $scope.clearFilters = function(scope){

        
        $scope.failureAreas = [];
        $scope.failureArea = null;
        $scope.failureEquipments = [];
        $scope.failureEquipment = null;
        if($scope.failureFactories.length > 0)
            scope.failureFactory = $scope.failureFactories.filter(function(e){ return  e.Sheet === null; })[0];
        scope.failureWorkCenter = null;

        GetNotesFailure(scope, null);
    }
 
    function GetNotesFailure(scope, location){

        var workCenter = scope.failureWorkCenter == null || scope.failureWorkCenter.Name == "TODOS" ? null : scope.failureWorkCenter.Name;

        var startDate = $filter('date')(new Date(commonService.TryGetDateFromValue(scope.startDate, 2, 1, 0, '/')));
        var endDate = $filter('date')(new Date(commonService.TryGetDateFromValue(scope.endDate, 2, 1, 0, '/')));

        indicatorFailureAnalyzedFactory.GetNotesFailure(startDate, endDate, location).success(function (response){                
            
            $scope.FailureAnalyzedDataAll = response;
            
            if(response.length > 0) {
                FillWorkCenter($scope.FailureAnalyzedDataAll);
                FillChart(scope,null, workCenter);

                if(workCenter){                   
                    scope.failureWorkCenter = $scope.failureWorkCenters.filter(function(e){return e.Name === workCenter;})[0];
                }
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

    function FillChart(scope, sheet, workCenter){
        
        var failureAnalyzedDataAll = angular.copy($scope.FailureAnalyzedDataAll);
        var failureAnalyzedData = [];        

         if(sheet || workCenter)  
            failureAnalyzedData = FilterFailureAnalyzedDataByLocation(failureAnalyzedDataAll, sheet, workCenter);
         else
            failureAnalyzedData = failureAnalyzedDataAll;      

        //DataSetOverride
        var type1Color = 'rgb(231, 230, 230)';
        var type2Color = 'rgb(175, 171, 171)';
        var type3Color = 'rgb(59, 56, 56)';
        var type4Color = 'rgb(69, 85, 107)';

        var type5Color = 'rgb(192, 0, 0)';
        var type6Color = 'rgb(255, 255, 0)';
        var type7Color = 'rgb(189, 215, 238)';
        var type8Color = 'rgb(132, 151, 176)';
        var type9Color = 'rgb(167, 205, 149)';
        var type10Color = 'rgb(101, 111, 89)';
        var type11Color = 'rgb(101, 50, 89)';
        

        var datasetOverride = [ 
                                   
            { id: 4,stack: 1, label: 'M2 Tipo 4', fill: true, backgroundColor: type4Color, borderColor: type4Color, pointBackgroundColor: type4Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type4Color, pointHoverBorderColor: type4Color, tree:[], datalabels: { color:'white', align: 'center', anchor: 'center'}},
            { id: 3,stack: 1, label: 'M2 Tipo 3', fill: true, backgroundColor: type3Color, borderColor: type3Color, pointBackgroundColor: type3Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type3Color, pointHoverBorderColor: type3Color, tree:[] , datalabels: { color:'white', align: 'center', anchor: 'center'}},
            { id: 2,stack: 1, label: 'M2 Tipo 2', fill: true, backgroundColor: type2Color, borderColor: type2Color, pointBackgroundColor: type2Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type2Color, pointHoverBorderColor: type2Color, tree:[] , datalabels: { color:'black', align: 'center', anchor: 'center'}},
            { id: 1,stack: 1, label: 'M2 Tipo 1', fill: true, backgroundColor: type1Color, borderColor: type1Color, pointBackgroundColor: type1Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type1Color, pointHoverBorderColor: type1Color, tree:[], datalabels: { color:'black', align: 'center', anchor: 'center'}},
           
            { id: 9,stack: 2, label: 'FA Concluída no Prazo', fill: true, backgroundColor: type9Color, borderColor: type9Color, pointBackgroundColor: type9Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type9Color, pointHoverBorderColor: type9Color, tree:[], datalabels: { color:'black', align: 'center', anchor: 'center'}},
            { id: 8,stack: 2, label: 'FA Concluída não Finalizada', fill: true, backgroundColor: type8Color, borderColor: type8Color, pointBackgroundColor: type8Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type8Color, pointHoverBorderColor: type8Color, tree:[], datalabels: { color:'white', align: 'center', anchor: 'center'}},
            { id: 11,stack: 2, label: 'FA Concluída sem Analise', fill: true, backgroundColor: type11Color, borderColor: type11Color, pointBackgroundColor: type11Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type11Color, pointHoverBorderColor: type11Color, tree:[], datalabels: { color:'white', align: 'center', anchor: 'center'}},
            { id: 10,stack: 2, label: 'FA Concluída Fora do Prazo', fill: true, backgroundColor: type10Color, borderColor: type10Color, pointBackgroundColor: type10Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type10Color, pointHoverBorderColor: type10Color, tree:[], datalabels: { color:'white', align: 'center', anchor: 'center'}},
            { id: 7,stack: 2, label: 'FA Analisada', fill: true, backgroundColor: type7Color, borderColor: type7Color, pointBackgroundColor: type7Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type7Color, pointHoverBorderColor: type7Color, tree:[], datalabels: { color:'white', align: 'center', anchor: 'center'}},
            { id: 6,stack: 2, label: 'FA Aberta', fill: true, backgroundColor: type6Color, borderColor: type6Color, pointBackgroundColor: type6Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type6Color, pointHoverBorderColor: type6Color, tree:[], datalabels: { color:'black', align: 'center', anchor: 'center'}},
            { id: 5,stack: 2, label: 'FA Pendente', fill: true, backgroundColor: type5Color, borderColor: type5Color, pointBackgroundColor: type5Color, pointBorderColor: '#fff', pointHoverBackgroundColor: type5Color, pointHoverBorderColor: type5Color, tree:[], datalabels: { color:'white', align: 'center', anchor: 'center'}}

        ];
        
        //Fill Labels
        var labels = FillLabels(scope,failureAnalyzedData);

        //Fill Data
        FillData(scope, failureAnalyzedData, datasetOverride, labels);       
        
        //Render Chart
        RenderChart(failureAnalyzedData, datasetOverride, labels);
    }

    function FillLabels(scope, failureAnalyzedData) {

        var labels = [];

        var failureFactories = angular.copy($scope.failureFactories);
                        
        angular.forEach(failureAnalyzedData, function(obj, key){                                         

            if(obj.dateStart){

                var locations = [];
                var Sheet = '';
                
                if(scope.failureFactory == null || scope.failureFactory.Sheet == null)
                    Sheet = $scope.viewName ? obj.NameFactory : obj.SheetFactory;
                    else
                        if(scope.failureArea == null || scope.failureArea.Sheet == null)
                            Sheet = $scope.viewName ? obj.NameArea : obj.SheetArea;
                            else                                
                                Sheet = $scope.viewName ? obj.NameEquipGroup : obj.SheetEquip;

              
                var valueLabel =  GetLabel(obj.dateStart);                
                
                var label = labels.filter(function(e){ return e.label === valueLabel;})[0];
                if(!label)                                     
                    labels.push({label: valueLabel, children: [Sheet]} );
                else
                    {
                        var children = label.children.filter(function(e){ return e === Sheet;});
                        if(children.length == 0)
                            label.children.push(Sheet);
                    }            
                               
            }

        });
               
        return labels;
    }

    function FillData(scope, failureAnalyzedData,  datasetOverride, labels){
                     
        //Data
        angular.forEach(datasetOverride, function (dataSet, index) {

            
            angular.forEach(labels, function (label, indexLabel) {          
                
                var failureAnalyzedDataByType = FilterFailureAnalyzedDataByType(scope, failureAnalyzedData, dataSet.id, label.label);
                
                var dataValue = {value: failureAnalyzedDataByType.length, children:[]};

                angular.forEach(label.children, function (children, indexChildren) {                             
                    var failureAnalyzedDataByLabel = failureAnalyzedDataByType.filter(function(e){ 
                        if(scope.failureFactory == null || scope.failureFactory.Sheet == null)
                            return $scope.viewName ? e.NameFactory ===  children : e.SheetFactory ===  children;
                            else
                                if(scope.failureArea == null || scope.failureArea.Sheet == null)
                                    return $scope.viewName ? e.NameArea ===  children : e.SheetArea ===  children;
                                    else
                                        return $scope.viewName ? e.NameEquipGroup ===  children : e.SheetEquip ===  children;
                    });

                    dataValue.children.push(failureAnalyzedDataByLabel.length);
                                            
                });

                dataSet.tree.push(dataValue);

            });

        });
        
    }
    

    function FilterFailureAnalyzedDataByType(scope, failureAnalyzedData, dataSet, label){

        

        var filterFailure = [];
        if(dataSet <= 4)
            filterFailure = failureAnalyzedData.filter(function (e) {         
                  return e.cdTypeOperation === dataSet && GetLabel(e.dateStart) === label; 
            });
        else{
            switch(dataSet){

                case 5: 
                    filterFailure = failureAnalyzedData.filter(function (e) { 
                         return  e.cdPendingFA === 1 && GetLabel(e.dateStart) === label;                             
                    });
                    break;
                case 6: 
                    filterFailure = failureAnalyzedData.filter(function (e) { 
                        return  e.cdOpenFA === 1 && GetLabel(e.dateStart) === label;                                                     
                    });
                    break;
                case 7: 
                    filterFailure = failureAnalyzedData.filter(function (e) { 
                         return  e.cdAnalyzedFA === 1 && GetLabel(e.dateStart) === label;                                                   
                    });
                    break;
                case 8: 
                    filterFailure = failureAnalyzedData.filter(function (e) { 
                         return  e.cdClosedNotEndFA === 1 && GetLabel(e.dateStart) === label;                                                                           
                     });
                    break;
                case 9: 
                    filterFailure = failureAnalyzedData.filter(function (e) { 
                         return  e.cdClosedOnTimeFA === 1 && GetLabel(e.dateStart) === label;                                                                                                   
                    });
                    break;
                case 10: 
                    filterFailure = failureAnalyzedData.filter(function (e) { 
                         return  e.cdClosedDelayFA === 1 &&  GetLabel(e.dateStart) === label;                                                                                                                           
                    });
                    break;
                case 11: 
                    filterFailure = failureAnalyzedData.filter(function (e) { 
                         return  e.cdClosedNotAnalyzedFA === 1 &&  GetLabel(e.dateStart) === label;                                                                                                                           
                    });
                    break;
                default:
                    break;
            }
        }

        return filterFailure;
    }

    function FilterFailureAnalyzedDataByLocation(failureAnalyzedData, sheet, workCenter){
        
        var failureAnalyzedDataFilter = [];

        if(sheet && workCenter)
            failureAnalyzedDataFilter =  failureAnalyzedData.filter(function(e){ return e.SheetEquip.indexOf(sheet) >= 0 && e.CenterCost === workCenter;  })
        else
            if(sheet && !workCenter)
                failureAnalyzedDataFilter = failureAnalyzedData.filter(function(e){ return e.SheetEquip.indexOf(sheet) >= 0;  })
            else
                if(!sheet && workCenter)
                    failureAnalyzedDataFilter =  failureAnalyzedData.filter(function(e){ return e.CenterCost === workCenter;  })
        
        return failureAnalyzedDataFilter;
    }

    function FillWorkCenter(failureAnalyzedData){
        
        $scope.failureWorkCenters = [];

        $scope.failureWorkCenters.push({Name: "TODOS"});
        angular.forEach(failureAnalyzedData,function(obj,index){

            var workCenter = $scope.failureWorkCenters.filter(function(e){return e.Name === obj.CenterCost; });

            if(workCenter.length == 0)
                $scope.failureWorkCenters.push({Name: obj.CenterCost});

        });                       
    }


    function RenderChart(detailsDataAnalyzed, datasetOverride, labels){
        
        var detailsData = [];

        angular.forEach(detailsDataAnalyzed,function(obj,index){
           
                detailsData.push([
                    obj.idNoteM2,
                    obj.idNoteFa,
                    obj.SheetEquip,
                    obj.NameEquip,
                    $filter('date')(obj.dateStart,'dd/MM/yyyy'),
                    $filter('date')(obj.dateEnd,'dd/MM/yyyy'),
                    $filter('date')(obj.dateRef,'dd/MM/yyyy'),
                    $filter('date')(obj.InsDateTime,'dd/MM/yyyy'),
                    obj.CenterCost,
                    obj.cdTypeOperation,
                    obj.statusM2,
                    obj.statusFA                  
                ]);
            
        });

        $scope.failureAnalyzedData.options = optionsChart,        
//            $scope.failureAnalyzedData.options = $scope.optionsChart,            
            $scope.failureAnalyzedData.data = { labels : labels, datasets: datasetOverride},
            $scope.failureAnalyzedData.detailsDataAnalyzed = detailsData;          
        

       
    }

    function GetEquipmentsPlantGroup(scope, index, sheedMan, workCenter){
        
       
        equipmentPlantGroupFactory.GetEquipmentsPlantGroup(index, sheedMan).success(function (response){                
            
            if(response.length > 0)  {

                switch(index){                    
                    case 2 :
                        $scope.failureFactories = response;
                        $scope.failureAreas = [];
                        $scope.failureEquipments = [];
                        scope.failureFactory = $scope.failureFactories.filter(function(e){ return  e.Sheet === null; })[0]; 
                        FillChart(scope, sheedMan, workCenter);                      
                    break;
                    case 3 :
                        $scope.failureAreas = response;
                        $scope.failureEquipments= [];
                        scope.failureArea = $scope.failureAreas.filter(function(e){ return  e.Sheet === null; })[0];
                        FillChart(scope, sheedMan, workCenter);
                    break;
                    case 4 :
                        $scope.failureEquipments = response;
                        scope.failureEquipment = $scope.failureEquipments.filter(function(e){ return  e.Sheet === null; })[0];
                        FillChart(scope, sheedMan, workCenter);
                    break;
                }

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
        
        budgetFactory.GetBudgetsActive().success(function (response){    

        if(response.length > 0){        
            $scope.budgets = response;             
        }

        GetNotesFailure($scope, null);
        
        GetEquipmentsPlantGroup($scope, 2, null, null);

        }).error(function(error){

            $.bigBox({
            title : "Erro!",
            content : error ? error.Message : "Falha de comunicação com o servidor.",            
            color : "#C46A69",              
            icon : "fa fa-warning swing animated",              
            timeout : 6000 });

        });    
    };

    function GetLabel(dateRef){

        var valueLabel = null;
        switch($scope.selectedFilterType.id){
            case 1: 
                valueLabel = $filter('date')(dateRef,'MM/yyyy');
            break;
            case 2:                         
                var filterBudgets = $scope.budgets.filter(function(f){ return f.dateStart <= dateRef &&  f.dateEnd >= dateRef })[0];
                valueLabel = $filter('date')(filterBudgets.dateStart,'MM/yyyy') + ' - ' + $filter('date')(filterBudgets.dateEnd,'MM/yyyy');
            break;
        }

        return valueLabel;
    }
    
    $scope.dataTest = {
        // define label tree
        labels: [
          
          {            
            label: '01/2018',
            
            // children: ['PLANTA SAW', 'PLANTA ERW', 'OFICINA DE VEICULOS', 'PLANTA FABRICAÇÃO DE CONECTORES']
            children: [{ label: 'PLANTA SAW'}, { label: 'PLANTA ERW'}, { label: 'OFICINA DE VEICULOS'}, { label: 'PLANTA FABRICAÇÃO DE CONECTORES'}]
          },
          {
            label: '02/2018',
            
            children: ['PLANTA SAW', 'PLANTA ERW', 'OFICINA DE VEICULOS', 'PLANTA FABRICAÇÃO DE CONECTORES']
          },
          {
            label: '03/2018',
            
            children: ['PLANTA SAW', 'PLANTA ERW', 'OFICINA DE VEICULOS', 'PLANTA FABRICAÇÃO DE CONECTORES']
          },
          {
            label: '04/2018',
            
            children: ['PLANTA SAW', 'PLANTA ERW', 'OFICINA DE VEICULOS', 'PLANTA FABRICAÇÃO DE CONECTORES']
          },
          {
            label: '05/2018',
            
            children: ['PLANTA SAW', 'PLANTA ERW', 'OFICINA DE VEICULOS', 'PLANTA FABRICAÇÃO DE CONECTORES']
          },
        ],
        datasets: [{
          label: 'M2 TIPO 1',
          backgroundColor: 'rgb(231, 230, 230)',
          stack: 'Stack 0',
          // store as the tree attribute for reference, the data attribute will be automatically managed
          tree: [
            
            {
              value: 50,
             
              children: [15, 5, 6, 10, 14]
            },
            {
                value: 40,
                children: [10, 5, 6, 5, 14]
              },
              {
                value: 70,
                children: [20, 7, 9, 15, 19]
              },
              {
                value: 65,
                children: [20, 10, 6, 10, 19]
              },
              {
                value: 25,
                children: [6, 4, 5, 5, 5]
              },
            
          ]
        },
        {
            label: 'M2 Tipo 2',
            backgroundColor: 'rgb(175, 171, 171)',
            stack: 'Stack 0',
          
            tree: [
                {
                    value: 25,
                    children: [6, 4, 5, 5, 5]
                },
                {
                    value: 70,
                    children: [20, 7, 9, 15, 19]
                },
                {
                    value: 50,
                    children: [15, 5, 6, 10, 14]
                },
                {
                    value: 40,
                    children: [10, 5, 6, 5, 14]
                },
               
                {
                    value: 65,
                    children: [20, 10, 6, 10, 19]
                },
               
                  
            ]
          },
          {
            label: 'FA Aberta',
            backgroundColor: 'rgb(175, 16, 171)',
            stack: 'Stack 1',
            // store as the tree attribute for reference, the data attribute will be automatically managed
            tree: [
                {
                    value: 25,
                    children: [6, 4, 5, 5, 5]
                },
                {
                    value: 70,
                    children: [20, 7, 9, 15, 19]
                },
                {
                    value: 50,
                    children: [15, 5, 6, 10, 14]
                },
                {
                    value: 40,
                    children: [10, 5, 6, 5, 14]
                },
               
                {
                    value: 65,
                    children: [20, 10, 6, 10, 19]
                },
               
            ]
          }
        ]
    };

   


      
});    




