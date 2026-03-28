app.controller('reportsController', function ($scope, $rootScope, $routeParams, $location, $filter, $timeout, $window,  $uibModal, consts, commonService, reportsFactory, reportsStatusFactory, reportsTypesFactory, filesFactory, userFactory, prioritiesFactory) {

    Dropzone.autoDiscover = false;

    $scope.Action = 'Insert';
    $scope.Disabled = false;
    $scope.idEquipment = $routeParams.id;
    $scope.Name = $routeParams.name;
    $scope.viewby = "5";
    $scope.searchEquip   = '';
    $scope.startDate = $filter('date')(new Date().setYear(new Date().getFullYear() - 1), 'dd/MM/yyyy');
    $scope.endDate = $filter('date')(new Date(),'dd/MM/yyyy');
    $scope.uploadFiles = [];
    $scope.filesDuplicate = [];

    $scope.FillTabsLanc = function (tab){
        $scope.selTabLanc = tab;
        $scope.tabsLanc = [
            {id: 1, name:' Dados',  class: 'fa fa-fw fa-lg fa-tasks', hide: false},
            {id: 2, name:' Arquivos',  class: 'fa fa-fw fa-lg fa-file', hide: false },
            {id: 3, name:' SAP', class: 'fa fa-fw fa-lg fa-info ', hide: $scope.Report.noteSAP == null || !$scope.Report.Equip.cdIntegrate ? true : false},            
            {id: 4, name:' Análise Técnica', class: 'fa fa-fw fa-lg fa-bar-chart-o ', hide: $scope.Action == 'Insert' ? true : false}            
        ];
    };

    $scope.getReports = function(tab){
        
                $scope.selTab = tab;
        
                var startDate =  commonService.TryGetDateFromValue($scope.startDate, 2, 1, 0, '/');
                var endDate =  commonService.TryGetDateFromValue($scope.endDate, 2, 1, 0, '/');
                
                if(tab == 1){
                    $scope.Action = "Insert";
                    $scope.Disabled = false;

                    $scope.FillTabsLanc(1);
                }
        
                if(tab == 2)            
                    GetReportsByDateRange(startDate, endDate , $routeParams.id, $scope.filterTypeHist);
        
                if(tab == 3)
                    GetMeasurementsByDateRange(startDate, endDate , $routeParams.id, $scope.filterType);  
                
                if(tab == 4)
                    GetReportsOTs(startDate, endDate);  
        
    };

    
    newReport();  

    if ($rootScope.user){
        var hide = $rootScope.user.ReadOnly || ($rootScope.user.Configuration && !$rootScope.user.Maintance ) ? true : false;
        $scope.tabs = [
            {id: 1, name:'Lançamento',  class: 'fa fa-fw fa-lg fa-tasks', hide: hide},
            {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
            {id: 3, name:'Informes de Medições', class: 'fa fa-fw fa-lg fa-info', hide: false},            
        ];

        $scope.selTab = $rootScope.user.ReadOnly || ($rootScope.user.Configuration && !$rootScope.user.Maintance ) ? 2 : 1;

        $scope.getReports($scope.selTab);
     }
     else{
         GetUser();
     };


    GetReportsTypes();

    GetReportsStatus();

    GetPriorities();
   
    $scope.editReport = function(){
        $scope.Disabled = false;
    }

    $scope.saveReport = function(model){

        model.dateInput = model.dateInput ? commonService.TryGetDateFromValue(model.dateInput, 2, 1, 0, '/') : null;
        model.dateInfo = model.dateInfo ? commonService.TryGetDateFromValue(model.dateInfo, 2, 1, 0, '/') : null;
        model.dateMeasure = model.dateMeasure ? commonService.TryGetDateFromValue(model.dateMeasure, 2, 1, 0, '/') : null;
        model.cdUser = $rootScope.user.Login;

        reportsFactory.InsertReport(model).success(function(response){

            if(response.status){

                newReport();

                $scope.selTabLanc = 1;

                $.bigBox({
                    title : "Sucesso",
                    content : "Laudo lançado com sucesso !",
                    color : "#739E73",
                    timeout: 6000,
                    icon : "fa fa-check",
                    //number : "4"
                });
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

    };

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.setType= function (cdType) {

        switch (cdType) {
            case 1 :
                $scope.Report.Title = "PRED.AO - "+ $scope.Name;
                $scope.Report.vlVelocity = null;
                $scope.Report.vlAcceleration = null;
                $scope.Report.vlRotation = null;         
                $scope.Report.vlPressure = null;    
                $scope.Report.vlTempMax = null;
                $scope.Report.vlTempEnvironment = null;   
                $scope.Report.vlEmissivity = null;              
                  
                break;
            case 2 :
                $scope.Report.Title = "PRED.AV - "+ $scope.Name;               
                $scope.Report.vlTempMax = null;
                $scope.Report.vlTempEnvironment = null;   
                $scope.Report.vlEmissivity = null;          
                $scope.Report.vlISOMax = null;
                $scope.Report.vlISOAlarm = null;
                $scope.Report.vlISO = null;
                $scope.Report.vlMore4Micron = null;
                $scope.Report.vlMore6Micron = null;
                $scope.Report.vlMore14Micron = null;
                $scope.Report.vlWaterKarlFischer = null;
                $scope.Report.vlViscosity40 = null;
                $scope.Report.vlViscosity100 = null;
                $scope.Report.vlTAN = null;
                break;
            case 3 :
                $scope.Report.Title = "PRED.AT - "+ $scope.Name;
                $scope.Report.vlVelocity = null;
                $scope.Report.vlAcceleration = null;           
                $scope.Report.vlISOMax = null;
                $scope.Report.vlISOAlarm = null;
                $scope.Report.vlISO = null;
                $scope.Report.vlMore4Micron = null;
                $scope.Report.vlMore6Micron = null;
                $scope.Report.vlMore14Micron = null;
                $scope.Report.vlWaterKarlFischer = null;
                $scope.Report.vlViscosity40 = null;
                $scope.Report.vlViscosity100 = null;
                $scope.Report.vlTAN = null;
                $scope.Report.vlRotation = null;
                $scope.Report.vlPressure = null;
                break;
            default:
                $scope.Report.Title = "";
                $scope.Report.vlVelocity = null;
                $scope.Report.vlAcceleration = null;   
                $scope.Report.vlTempMax= null;          
                $scope.Report.vlISOMax = null;
                $scope.Report.vlISOAlarm = null;
                $scope.Report.vlISO = null;
                $scope.Report.vlMore4Micron = null;
                $scope.Report.vlMore6Micron = null;
                $scope.Report.vlMore14Micron = null;
                $scope.Report.vlWaterKarlFischer = null;
                $scope.Report.vlViscosity40 = null;
                $scope.Report.vlViscosity100 = null;
                $scope.Report.vlTAN = null;
                $scope.Report.vlRotation = null;
                $scope.Report.vlPressure = null;
                $scope.Report.vlTempEnvironment = null;   
                $scope.Report.vlEmissivity = null;    
        }

    };

    $scope.setTabLanc = function(tab){
        $scope.selTabLanc = tab;

        switch(tab){
            case 4 : 
            
                var startDate = $filter('date')(new Date(commonService.TryGetDateFromValue($scope.Report.dateMeasure, 2, 1, 0, '/')).setFullYear(new Date(commonService.TryGetDateFromValue($scope.Report.dateMeasure, 2, 1, 0, '/')).getFullYear() - 1));
                var endDate = new Date() > new Date(commonService.TryGetDateFromValue($scope.Report.dateMeasure, 2, 1, 0, '/')) ? $filter('date')(new Date()) : $filter('date')(new Date(commonService.TryGetDateFromValue($scope.Report.dateMeasure, 2, 1, 0, '/')));
                GetAnalysesValues(startDate,endDate, $scope.Report.cdEquipment, $scope.Report.cdType);
            break;
        }

    }

    $scope.visibleEquips = function (value) {
        return !($scope.searchEquip && $scope.searchEquip.length > 0
        && value[0].toUpperCase().indexOf($scope.searchEquip.toUpperCase()) == -1 );
    };

    $scope.pageChanged = function() {
    //console.log('Page changed to: ' + $scope.currentPage);
    };

    $scope.setItemsPerPage = function(num) {
        $scope.itemsPerPage = num;
        $scope.currentPage = 1; //reset to first page

        $scope.itemPageStart = ((($scope.currentPage-1)*$scope.itemsPerPage)+1);            
        $scope.itemPageEnd = $scope.itemsPerPage * $scope.currentPage;            
    }

    $scope.viewReport = function(item){       
        
        $scope.Action = "Edit";
        $scope.Disabled = true;
        $scope.Title = "Edição de Laudos";  
               
        $scope.itemStatus = item;
        GetReport(item.id);

        
        var modalInstance = $uibModal.open({
        animation: true,
        ariaLabelledBy: 'modal-title',
        ariaDescribedBy: 'modal-body',
        templateUrl: 'views/formReport.html', 
        controller: 'reportsModalController',    
        scope: $scope,
        size: 'lg',
        resolve: {
            item: function () {
            return $scope;
            }
        }                       
        });
    
    }

    $scope.dzOptions = {
    
        url : consts.apiUrl+"ReportFile/UploadFiles?cdUser=null&idReport="+$scope.Report.idReport,
        paramName : 'files',
        autoProcessQueue: true,
        method: "post",
        maxFilesize : '10',
    // acceptedFiles : 'image/jpeg, images/jpg, image/png',
        addRemoveLinks : true,     
        dictDefaultMessage: "Clique para realizar o upload de arquivos",
        dictFallbackMessage: "Seu browser não suporta drag'n'drop para upload de arquivos.",
        dictFallbackText: "Use o formulário de retorno abaixo para fazer o upload de seus arquivos como nos dias anteriores.",
        dictFileTooBig: "Arquivo muito grande ({{filesize}}MiB). O tamanho máximo permitido é: {{maxFilesize}}MiB.",
        dictInvalidFileType: "Tipo de arquivo não permitido.",
        dictResponseError: "Server responded with {{statusCode}} code.",
        dictCancelUpload: "Cancelar upload",
        dictCancelUploadConfirmation: "Tem certeza que deseja cancelar o upload?",
        dictRemoveFile: "Remover",
        dictRemoveFileConfirmation: null,
        dictMaxFilesExceeded: "Não é possível carregar mais arquivos.",
        thumbnailWidth: 120,
        thumbnailHeight: 120
    
    };

    //Handle events for dropzone
    $scope.dzCallbacks = {
        'addedfile' : function(file){
            console.log(file);                
        },
        'success' : function(file, xhr){
            console.log(file, xhr);
            if(xhr){
                if(xhr.status){

                    if(hasWhiteSpace(file.name)){
                        var dropzone = Dropzone.forElement(".dropzone");;
                        dropzone.removeFile(file);
                        $.bigBox({
                            title : "Erro!",
                            content : "Nome de arquivo inválido.",            
                            color : "#C46A69",              
                            icon : "fa fa-warning swing animated",              
                            timeout : 6000 });
                    }
                    else{
                        var existsFile =   $scope.Report.Files.filter(function(f) { return f.Name === file.name}).length == 0 ? false : true;          
                        if(!existsFile)
                            $scope.Report.Files.push({idFile: xhr.id == null ? null : xhr.id, cdReport: $scope.Report.idReport, Name: file.name, Size: file.size/1024, Type: file.type});                
                        else
                            $scope.filesDuplicate.push({idFile: null, cdReport: $scope.Report.idReport, Name: file.name, Size: file.size/1024, Type: file.type});
                    }
                }
            }
        },
        'removedfile' : function(file){           
        
            var itemFile = null;
            if($scope.filesDuplicate.length > 0)
            {
                itemFile = $scope.filesDuplicate.filter(function(f){ return f.Name === file.name})[0];
                $scope.filesDuplicate.splice($scope.filesDuplicate.indexOf(itemFile), 1); 
            }
            else{
                itemFile = $scope.Report.Files.filter(function(f){ return f.Name === file.name})[0];
                RemoveFiles(itemFile);     
            }         
        }
    
    };

    $scope.dzMethods = {};

    $scope.openFile = function(file){

        $window.open(consts.uploadUrl+file.cdReport+'/'+file.Name, '_blank');
    
    }

    $scope.$on("$destroy", function(){

        angular.forEach($scope.Report.Files, function(obj, key){
            RemoveFiles(obj);
        });    

        angular.forEach($scope.filesDuplicate, function(obj, key){
            RemoveFiles(obj);
        });
        

    });
   
    function hasWhiteSpace(s) {
        return s.indexOf(' ') >= 0;
    }

    function newReport (){
        

        if($scope.Report && $scope.Report.Files.length > 0 ){
            $scope.Report.Files = [];
            var dropzone = Dropzone.forElement(".dropzone");
            dropzone.removeAllFiles();
        }

        $scope.Report = {
            idReport: null,
            dateInput: $filter('date')(new Date(),'dd/MM/yyyy'),
            cdEquipment: $scope.idEquipment,
            cdStatus: null,
            cdType: null,
            dateInfo: null,
            Files: [],
            dateMeasure: null,
            Diagnostic: null,
            cdUser: null,
            Title: null,
            cdPriority: null,           
            vlVelocity: null,
            vlAcceleration: null,
            vlRotation: null,
            vlPressure: null,
            vlTempMax: null,
            vlTempEnvironment: null,
            vlEmissivity: null,
            vlISOMax: null,
            vlISOAlarm: null,
            vlISO: null,
            vlMore4Micron: null,
            vlMore6Micron: null,
            vlMore14Micron: null,
            vlWaterKarlFischer: null,
            vlViscosity40: null,
            vlViscosity100: null,
            vlTAN: null,
            noteSAP: null,
            Equip: null,
            cdRecurrent: false,
            cdOTExecuted: false,
            Active: true
        };
        
        $scope.filesDuplicate = [];
    
    };

    function GetReportsTypes(){
        
        reportsTypesFactory.GetReportsTypes().success(function (response){    

        if(response.length > 0){        
            $scope.reportsTypes = response;    
            
            $scope.reportsTypesFilter = [];
            $scope.reportsTypesFilter.push({idType: null, Name:"Todos"});

            angular.forEach($scope.reportsTypes, function(obj, key){
                $scope.reportsTypesFilter.push({idType: obj.idType, Name: obj.Name});
            });

            $scope.filterType = null;
            $scope.filterTypeHist = null;
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

    function GetReportsStatus(){
        
        reportsStatusFactory.GetReportsStatus().success(function (response){    

            if(response.length > 0){        
                $scope.reportsStatus = response;    
                        
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

    function GetPriorities(){
        
        prioritiesFactory.GetPriorities().success(function (response){    

            if(response.length > 0){        
                $scope.priorities = response;    
                        
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

    function GetReportsOTs(startDate, endDate){
        
        reportsFactory.GetReportsOTs(startDate, endDate).success(function (response){    

            if(response.length > 0){ 

                $scope.columnDefs = [ 
                    { "mDataProp": "Laudo", "aTargets":[0]},
                    { "mDataProp": "Aviso SAP", "aTargets":[1] },
                    { "mDataProp": "Ordem SAP", "aTargets":[2] },
                    { "mDataProp": "Prioridade", "aTargets":[3] },
                    { "mDataProp": "Equipamento", "aTargets":[4] },
                    { "mDataProp": "Criticidade", "aTargets":[5] },
                    { "mDataProp": "Medição", "aTargets":[6] },
                    { "mDataProp": "Técnica", "aTargets":[7] },
                    { "mDataProp": "Estado", "aTargets":[8] },
                    { "mDataProp": "Diagnóstico", "aTargets":[9] }
                    
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


                $scope.reportsOTs = response;    



                angular.forEach(response, function(obj, key){
                    
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

    function GetReportsByDateRange(startDate, endDate, idEquipment, idType){

    
        reportsFactory.GetReportsByDateRange(startDate, endDate, idEquipment, idType).success(function (response){    
            
            if(response){        
                        
                $scope.reports = response;    

                $scope.totalItems = $scope.reports.rows.length;
                $scope.currentPage = 1;
                $scope.itemsPerPage = $scope.viewby;
                $scope.itemPageStart = ((($scope.currentPage-1)*$scope.itemsPerPage)+1);            
                $scope.itemPageEnd = $scope.itemsPerPage * $scope.currentPage;                        
                $scope.maxSize = 4; //Number of pager buttons to show
                            
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

    function GetMeasurementsByDateRange(startDate, endDate, idEquipment, idType){
        
            
                reportsFactory.GetMeasurementsByDateRange(startDate, endDate, idEquipment, idType).success(function (response){    
                    
                    if(response){        
                                
                        $scope.measurements = response;    
                                             
                        //Set options Chart
                        $scope.options = {
                            scales: {
                                xAxes: [{
                                stacked: true                               
                                }],
                                yAxes: [{
                                stacked: true                               
                                }]
                            },
                            
                            legend: {
                                display: true,
                                position: 'bottom'
                                
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
        
    }

    function GetAnalysesValues(startDate, endDate, idEquipment, idType){
        
            
                reportsFactory.GetAnalysisValues(startDate, endDate, idEquipment, idType).success(function (response){    
                    
                    $scope.Charts = [];

                    if(response.length > 0){        

                        var analysisValues = {
                            labels: [],
                            series: [],
                            data: [],
                            colors: [],
                            height: null
                        }
                       

                            switch(idType){
                                case 1:

                                    analysisValues.series.push('> 4 mícron (part/ml)');
                                    analysisValues.series.push('> 6 mícron (part/ml)');
                                    analysisValues.series.push('> 14 mícron (part/ml)');

                                    analysisValues.colors.push('#739e73');
                                    analysisValues.colors.push('#c79121');
                                    analysisValues.colors.push('#a90329');

                                    var values4Micron = [];
                                    var values6Micron = [];
                                    var values14Micron = [];

                                    angular.forEach(response, function(obj, key){      
                                        
                                        if(obj.vlMore4Micron > 0 || obj.vlMore6Micron > 0 || obj.vlMore14Micron > 0){
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            values4Micron.push(obj.vlMore4Micron);
                                            values6Micron.push(obj.vlMore6Micron);
                                            values14Micron.push(obj.vlMore14Micron); 
                                        }                                                                                   
                                    });  

                                    analysisValues.data.push(values4Micron);
                                    analysisValues.data.push(values6Micron);
                                    analysisValues.data.push(values14Micron);
                                    analysisValues.height = 110;
                                    $scope.Charts.push(analysisValues);
                                    
                                    analysisValues = {
                                        labels: [],
                                        series: [],
                                        data: [],
                                        colors: []
                                    };

                                    analysisValues.series.push('Água Karl Fischer (ppm)');
                                    analysisValues.colors.push('#a90329');

                                    var values = [];                                   

                                    angular.forEach(response, function(obj, key){   
                                        
                                        if(obj.vlWaterKarlFischer > 0 ){
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            values.push(obj.vlWaterKarlFischer);
                                        }
                                                                                                                            
                                    });  
                                    
                                    analysisValues.data.push(values);
                                    analysisValues.height = 100;

                                    $scope.Charts.push(analysisValues);
                                    
                                    analysisValues = {
                                        labels: [],
                                        series: [],
                                        data: [],
                                        colors: []
                                    };
                                    
                                    analysisValues.series.push('Viscosidade a 40oC (cst)');
                                    analysisValues.series.push('Viscosidade a 100oC (cst)');                                    

                                    analysisValues.colors.push('#739e73');
                                    analysisValues.colors.push('#c79121');                                    

                                    var valuesViscosity40 = [];
                                    var valuesViscosity100 = [];                                    

                                    angular.forEach(response, function(obj, key){      
                                        
                                        if(obj.vlViscosity40 > 0 || obj.vlViscosity100 > 0){
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            valuesViscosity40.push(obj.vlViscosity40);
                                            valuesViscosity100.push(obj.vlViscosity100);
                                            
                                        }                                                                                   
                                    });  

                                    analysisValues.data.push(valuesViscosity40);
                                    analysisValues.data.push(valuesViscosity100);     
                                    analysisValues.height = 100;                               

                                    $scope.Charts.push(analysisValues);

                                    analysisValues = {
                                        labels: [],
                                        series: [],
                                        data: [],
                                        colors: []
                                    };
                                    
                                    analysisValues.series.push('TAN (mg KOH/ml)');
                                    analysisValues.colors.push('#000099');

                                    var values = [];                                   

                                    angular.forEach(response, function(obj, key){   
                                        
                                        if(obj.vlTAN > 0 ){
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            values.push(obj.vlTAN);
                                        }
                                                                                                                            
                                    });  
                                    
                                    analysisValues.data.push(values);
                                    analysisValues.height = 100;

                                    $scope.Charts.push(analysisValues);
                                   

                                    break;
                                case 2:
                                    
                                    $scope.Charts = [];
                                  
                                    analysisValues.series.push('RMS velocidade (mm/s)');
                                    analysisValues.colors.push('#a90329');

                                    var values = [];                                   

                                    angular.forEach(response, function(obj, key){   
                                        
                                        if(obj.vlVelocity > 0 ){
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            values.push(obj.vlVelocity);
                                        }
                                                                                                                            
                                    });  
                                    
                                    analysisValues.data.push(values);
                                    analysisValues.height = 100;

                                    $scope.Charts.push(analysisValues);

                                    analysisValues = {
                                        labels: [],
                                        series: [],
                                        data: [],
                                        colors: []
                                    };


                                    analysisValues.series.push('RMS aceleração (g)');
                                    analysisValues.colors.push('#c79121');

                                    var values = [];                                   

                                    angular.forEach(response, function(obj, key){   
                                        
                                        if(obj.vlAcceleration > 0 ){
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            values.push(obj.vlAcceleration);
                                        }
                                                                                                                            
                                    });  
                                    
                                    analysisValues.data.push(values);
                                    analysisValues.height = 100;

                                    $scope.Charts.push(analysisValues);

                                    analysisValues = {
                                        labels: [],
                                        series: [],
                                        data: [],
                                        colors: []
                                    };

                                    analysisValues.series.push('Rotação (rpm)');
                                    analysisValues.colors.push('#739e73');

                                    var values = [];                                   

                                    angular.forEach(response, function(obj, key){   
                                        
                                        if(obj.vlRotation > 0 ){
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            values.push(obj.vlRotation);
                                        }
                                                                                                                            
                                    });  
                                    
                                    analysisValues.data.push(values);
                                    analysisValues.height = 100;

                                    $scope.Charts.push(analysisValues);

                                    analysisValues = {
                                        labels: [],
                                        series: [],
                                        data: [],
                                        colors: []
                                    };

                                    analysisValues.series.push('Pressão (bar)');
                                    analysisValues.colors.push('#000099');

                                    var values = [];                                   

                                    angular.forEach(response, function(obj, key){   
                                        
                                        if(obj.vlPressure > 0 ){
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            values.push(obj.vlPressure);
                                        }
                                                                                                                            
                                    });  
                                    
                                    analysisValues.data.push(values);
                                    analysisValues.height = 100;

                                    $scope.Charts.push(analysisValues);


                                    break;
                                case 3:

                                    $scope.Charts = [];
                                    analysisValues.series.push('Temperatura Max.');
                                    analysisValues.series.push('Temperatura Ambiente');
                                    analysisValues.colors.push('#a90329');
                                    analysisValues.colors.push('#c79121');

                                    var valuesTempMax = [];                                   
                                    var valuesTempEnv = [];                                   

                                    angular.forEach(response, function(obj, key){   
                                        if(obj.vlTempMax > 0 || obj.vlTempEnvironment){                                    
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            valuesTempMax.push(obj.vlTempMax);
                                            valuesTempEnv.push(obj.vlTempEnvironment);
                                        }
                                                                                                                            
                                    });  
                                    
                                    analysisValues.data.push(valuesTempMax);
                                    analysisValues.data.push(valuesTempEnv);
                                    analysisValues.height = 110;
                                    $scope.Charts.push(analysisValues)
                                                                                                     
                                    analysisValues = {
                                        labels: [],
                                        series: [],
                                        data: [],
                                        colors: []
                                    };

                                    analysisValues.series.push('Emissividade');
                                    analysisValues.colors.push('#739e73');

                                    var values = [];                                   

                                    angular.forEach(response, function(obj, key){   
                                        if(obj.vlEmissivity > 0 ){                                    
                                            analysisValues.labels.push($filter('date')(obj.dateMeasure,"dd/MM/yyyy"));
                                            values.push(obj.vlEmissivity);
                                        }
                                                                                                                            
                                    });  
                                    
                                    analysisValues.data.push(values);
                                    analysisValues.height = 110;

                                    $scope.Charts.push(analysisValues);
                                    break;
                                default:
                                    break;
                            }
                                                                               
                                             
                        //Set options Chart
                        $scope.options = {
                            scales: {
                              yAxes: [
                                {
                                  id: 'y-axis-1',
                                  type: 'linear',
                                  display: true,
                                  position: 'left'
                                }
                              ]
                            },                            
                            
                            legend: {
                                display: true,
                                position: 'bottom'
                                
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
        
    }

    function GetReport(id){
        
        reportsFactory.GetReport(id).success(function (response){    

            if(response){        
                $scope.Report = response;    
                $scope.Report.dateInfo =  $filter('date')($scope.Report.dateInfo,'dd/MM/yyyy');                                                 
                $scope.Report.dateInput = $filter('date')($scope.Report.dateInput,'dd/MM/yyyy'); 
                $scope.Report.dateMeasure = $filter('date')($scope.Report.dateMeasure,'dd/MM/yyyy');    
                $scope.Report.cdUser = $rootScope.user.Login; 

                $scope.FillTabsLanc(1);
                
                var dropzone = Dropzone.forElement(".dropzone");
                
                dropzone.options.url = consts.apiUrl+"ReportFile/UploadFiles?cdUser="+$scope.Report.cdUser+"&idReport="+$scope.Report.idReport;
                
                if($scope.Report.Files && $scope.Report.Files.length > 0)
                    dropzone.element.classList.add("dz-started");

                angular.forEach($scope.Report.Files, function(obj, key){
                    var mockFile = { name: obj.Name, size: obj.Size * 1024, status: 'success'};              
                    dropzone.emit("addedfile", mockFile );            
                    dropzone.emit("complete", mockFile);
                    dropzone.emit("success", mockFile );            
                    dropzone.files.push( mockFile );
                });            
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

    function RemoveFiles(file){
            
        if(file){

            filesFactory.RemoveFiles(file).success(function (response){    

                if(response.status)                
                    $scope.Report.Files.splice( $scope.Report.Files.indexOf(file), 1);            
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
        }
    };

    function GetUser(){
        
            userFactory.GetUser().success(function (response){
        
                    if(response){
        
                        var user = response;
                        var hide = user.ReadOnly || (user.Configuration && !user.Maintance ) ? true : false;
                        $scope.tabs = [
                            {id: 1, name:'Lançamento',  class: 'fa fa-fw fa-lg fa-tasks', hide: hide},
                            {id: 2, name:'Histórico',  class: 'fa fa-fw fa-lg fa-tasks', hide: false },
                            {id: 3, name:'Informes de Medições', class: 'fa fa-fw fa-lg fa-info', hide: false},
                            
                        ];
                
                        $scope.selTab = user.ReadOnly || (user.Configuration && !user.Maintance ) ? 2 : 1;

                        $scope.getReports($scope.selTab);
                                                                                                                
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



