var myApp = angular.module('myApp', []);

myApp.controller('mainController', ['$scope', function($scope) {
    
    $scope.name = "Main";
    
}]);

myApp.controller('secondController', ['$scope', function($scope) {
    
	// scope object is unique to each controller
    $scope.name = "Second";
    
}]);


// we can have as many different areas of HTML file attached to different controllers as we want