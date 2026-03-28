app.config(function ($routeProvider) {
    
    
        $routeProvider.when("/equipments/:id/:name", {
            templateUrl: "views/reports.html",
            controller: "reportsController"
        });
		
		 $routeProvider.when("/dashboard", {
            templateUrl: "views/dashboard.html",
            controller: "indicatorMaintenanceController"
        });

        $routeProvider.when("/config", {
            templateUrl: "views/config.html",
            controller: "configController"
        });

        $routeProvider.when("/budget", {
            templateUrl: "views/Budget.html",
            controller: "budgetController"
        });

        $routeProvider.when("/indicatorMaintenance/:id/:type", {
            templateUrl: "views/indicatorMaintenance.html",
            controller: "indicatorMaintenanceController"
        });

        $routeProvider.when("/indicatorOT/:id", {
            templateUrl: "views/indicatorOT.html",
            controller: "indicatorOTController"
        });

        $routeProvider.when("/indicatorOT/:id/:type", {
            templateUrl: "views/indicatorOT.html",
            controller: "indicatorOTController"
        });

        
        $routeProvider.when("/indicatorOTPlant/:id/:plant", {
            templateUrl: "views/indicatorOTPlant.html",
            controller: "indicatorOTController"
        });

         $routeProvider.when("/indicatorHoursWorked/:id", {
            templateUrl: "views/indicatorHoursWorked.html",
            controller: "indicatorOTController"
        });
        
        $routeProvider.when("/indicatorNote/:id", {
            templateUrl: "views/indicatorNote.html",
            controller: "indicatorNoteController"
        });

        $routeProvider.when("/indicatorNote/:id/:type", {
            templateUrl: "views/indicatorNote.html",
            controller: "indicatorNoteController"
        });

        $routeProvider.when("/indicatorFailureAnalyzed", {
            templateUrl: "views/indicatorFailureAnalyzed.html",
            controller: "indicatorFailureAnalyzedController"
        });
    
        $routeProvider.when("/oilManagement", {
            templateUrl: "views/oilManagement.html",
            controller: "oilManagementController"
        });
    
        $routeProvider.when("/oilManagementAlarms", {
            templateUrl: "views/oilManagementAlarms.html",
            controller: "oilManagementAlarmsController"
        });

        $routeProvider.when("/oilManagementAlarmsGroup", {
            templateUrl: "views/oilManagementAlarmsGroup.html",
            controller: "oilManagementAlarmsGroupController"
        });

        $routeProvider.when("/oilManagementIndicators/:id", {
            templateUrl: "views/indicatorOilManagement.html",
            controller: "indicatorOilManagementController"
        });

        $routeProvider.otherwise({ redirectTo: "/dashboard" });
    });
    