var myApp = angular.module("myApp", ["ngRoute"]);

// ngRoute is a router - it will grab whatever is in the hash and then run the proper code and the proper html
myApp.config(function ($routeProvider) {
	$routeProvider
	.when("/", {
		templateUrl: "3_MainPage.html",
		controller: "mainController"
	})
	.when("/second", {
		templateUrl: "3_SecondPage.html",
		controller: "secondController"
	})
	// it is possible to make a pattern matching and connect it to the same controller
	.when("/second:num", {
		templateUrl: "3_SecondPage.html",
		controller: "secondController"
	});
});

myApp.controller("mainController", ["$scope", "$location", "$log", function ($scope, $location, $log) {
    
	// AngularJS already has a utility to grab whatever is in the hash
	$log.info("location: " + $location.path());
	
	console.log("main controller operational");
	
	$scope.name = "Hello John!";
    
}]);

myApp.controller("secondController", ["$scope", "$routeParams", function ($scope, $routeParams) {
    
	// AngularJS already has a utility to grab whatever is in the hash
	console.log("second controller operational");
    
	$scope.name = "My name is Jane!";
	
	// if it exist leave it as it is if not assign 1 to it
	$scope.num = $routeParams.num || 1;
}]);