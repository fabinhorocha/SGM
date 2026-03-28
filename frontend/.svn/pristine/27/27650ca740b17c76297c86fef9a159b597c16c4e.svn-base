app.filter('capitalize', function() {
    return function(input) {
        var result = '';
        angular.forEach(input.split(' '),
            function (obj, key) {
                result+= key == 0 ? '' : ' ';
                result+= (!!obj) ? obj.charAt(0).toUpperCase() + obj.substr(1).toLowerCase() : '';
            });

      return result;
    }
});

app.filter('replace', function() {
    return function(input) {
        //var result = input.replace('/','');        
		var result = input.split('/').join(' ');         

      return result;
    }
});


app.filter("trust", ['$sce', function($sce) {
    return function(item) {       
      return $sce.trustAsHtml(item);
    };
}]);