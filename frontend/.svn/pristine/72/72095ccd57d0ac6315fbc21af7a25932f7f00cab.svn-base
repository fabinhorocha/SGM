app.controller('reportsModalController', function ($uibModalInstance, $scope, $rootScope, $filter, item, commonService, reportsFactory) {   

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');       
        newReport();
    };

    $scope.updateReport = function(model){
        
        model.dateInput = model.dateInput ? commonService.TryGetDateFromValue(model.dateInput, 2, 1, 0, '/') : null;
        model.dateInfo = model.dateInfo ? commonService.TryGetDateFromValue(model.dateInfo, 2, 1, 0, '/') : null;
        model.dateMeasure = model.dateMeasure ? commonService.TryGetDateFromValue(model.dateMeasure, 2, 1, 0, '/') : null;
        model.cdUser = $rootScope.user.Login;
        //Atualiza status do item da lista
        var status = $scope.reportsStatus.filter(function(f) {return f.idStatus === model.cdStatus})[0];
        $scope.itemStatus.idStatus = status.idStatus;
        $scope.itemStatus.nameStatus = status.Name;
        
        reportsFactory.UpdateReport(model).success(function(response){
    
            if(response.status){
                newReport();
                
                $.bigBox({
                    title : "Sucesso",
                    content : "Laudo atualizado com sucesso !",
                    color : "#739E73",
                    timeout: 1000,
                    icon : "fa fa-check",
                    //number : "4"
                });

              //  CreateNoteSAP(model);

                $uibModalInstance.close();            
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
    
    $scope.confirmDelete = function(model){
        
        $.smallBox({
            title : "Delete!",
            content : "Deseja excluir o laudo ? <p class='text-align-right'><a id='btnSim' class='btn btn-primary btn-sm'>Sim</a> <a href='javascript:void(0);' class='btn btn-primary btn-sm'>Não</a></p>",            
            color : "#3276B1",
            //timeout: 8000,
            icon : "fa fa-warning swing animated" },
            function(e){

                if(e.target.id == 'btnSim'){
                    $scope.deleteReport(model);
                }
            }
        );

    }

    $scope.deleteReport = function(model){
        
        model.dateInput = model.dateInput ? commonService.TryGetDateFromValue(model.dateInput, 2, 1, 0, '/') : null;
        model.dateInfo = model.dateInfo ? commonService.TryGetDateFromValue(model.dateInfo, 2, 1, 0, '/') : null;
        model.dateMeasure = model.dateMeasure ? commonService.TryGetDateFromValue(model.dateMeasure, 2, 1, 0, '/') : null;
        model.cdUser = $rootScope.user.Login;
        //Atualiza status do item da lista
        var status = $scope.reportsStatus.filter(function(f) {return f.idStatus === model.cdStatus})[0];
        $scope.itemStatus.idStatus = status.idStatus;
        $scope.itemStatus.nameStatus = status.Name;
        
        reportsFactory.DeleteReport(model).success(function(response){
    
            if(response.status){
                newReport();
                $scope.itemStatus.Active = 0;
                $.bigBox({
                    title : "Sucesso",
                    content : "Laudo excluído com sucesso !",
                    color : "#739E73",
                    timeout: 1000,
                    icon : "fa fa-check",
                    //number : "4"
                });

              //  CreateNoteSAP(model);

                $uibModalInstance.close();            
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

    $scope.confirmCreateNoteSAP  = function(model){
        
        $.smallBox({
            title : "Interface SAP !",
            content : "Deseja enviar o laudo "+model.idReport+" para o SAP ? <p class='text-align-right'><a id='btnSim' class='btn btn-primary btn-sm'>Sim</a> <a href='javascript:void(0);' class='btn btn-primary btn-sm'>Não</a></p>",            
            color : "#3276B1",
            //timeout: 8000,
            icon : "fa fa-warning swing animated" },
            function(e){

                if(e.target.id == 'btnSim'){
                    $scope.createNoteSAP(model);
                }
            }
        );

    }


    $scope.createNoteSAP = function(model){               
        
        reportsFactory.CreateNoteSAP(model).success(function(response){
            
            model.noteSAP = {
                cdSAP: response.id == 0 ? null : response.id,
                cdReport: model.idReport,
                cdStatus: response.status,
                statusMessage: response.message
            };           
            
            if(response.status){
                              
                $.bigBox({
                    title : "Sucesso",
                    content : response.message,
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

            $scope.FillTabsLanc(3);
                        
            
        }).error(function(error){
            $.bigBox({
                title : "Erro!",
                content : error ? error.Message : "Falha de comunicação com o servidor.",            
                color : "#C46A69",              
                icon : "fa fa-warning swing animated",              
                timeout : 6000 });
        });
    
    };

    function newReport(){
                        
        item.Report = {
            idReport: null,
            dateInput: $filter('date')(new Date(),'dd/MM/yyyy'),
            cdEquipment: item.idEquipment,
            cdStatus: null,
            cdType: null,
            dateInfo: null,
            fileReport: null,
            dateMeasure: null,
            Diagnostic: null,
            cdUser: null,
            Files:[],
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

        item.filesDuplicate = [];
    }

    

});