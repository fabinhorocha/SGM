app.config(function (cfpLoadingBarProvider,uiTreeFilterSettingsProvider, chosenProvider, ChartJsProvider) {
        
    cfpLoadingBarProvider.includeSpinner = true;
    uiTreeFilterSettingsProvider.addresses = ['Name'];
    uiTreeFilterSettingsProvider.descendantCollection="EquipmentsChilds";

    chosenProvider.setOption({
        no_results_text: 'Não existem resultados!',
        placeholder_text_multiple: 'Selecione um ou mais registros',
        placeholder_text_single: 'Selecione'
    });
  
    ChartJsProvider.setOptions({ colors : [ '#803690', '#00ADF9', '#DCDCDC', '#46BFBD', '#FDB45C', '#949FB1', '#4D5360'] });


    
});