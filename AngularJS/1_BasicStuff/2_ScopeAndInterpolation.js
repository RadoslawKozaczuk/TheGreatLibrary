/* === Interpolation ===
	Creating a string by combining strings and placeholders.
	'My name is' + name is interpolated, and results in 'My name is Tony'. */
	
	
var myApp = angular.module("myApp", []);
myApp.controller("mainController", ["$scope", "$timeout", "$filter", function($scope, $timeout, $filter) {
	$scope.name = "Tony";
	
	// we can just worry about updating variables and view is going along with us
	$timeout(function() {
		$scope.name = "Everybody";
	}, 3000);
	
	// two-way data binding means view change the model and model change the view
	$scope.lowercasehandle = function() {
		return $filter('lowercase')($scope.handle);
	}
	
	$scope.handle = "";
}]);