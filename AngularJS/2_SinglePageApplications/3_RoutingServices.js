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

// we create a service that will be a singleton - AngularJS takes care of the dependency injection
myApp.service("nameService", function () {
	var self = this; // to make sure we have a reference to the outer function
	this.name = "John Doe";
	
	this.namelength = function () {
		return self.name.length;
	};
});

myApp.controller("mainController", ["$scope", "$location", "$log", "nameService", function ($scope, $location, $log, nameService) {
    
	// AngularJS already has a utility to grab whatever is in the hash
	$log.info("location: " + $location.path());
	
	$scope.name = nameService.name;
	
	// but we can a watch that will run a function whenever the value changes
	$scope.$watch("name", function() {
		nameService.name = $scope.name;
	});
	
	$log.log("nameService: " + nameService.name);
	$log.log("nameService: " + nameService.namelength());
}]);

myApp.controller("secondController", ["$scope", "$routeParams", "nameService", function ($scope, $routeParams, nameService) {
    
	// AngularJS already has a utility to grab whatever is in the hash
	console.log("second controller operational");
    
	// the scope is what Digest Loop looks at
	$scope.name = nameService.name;
	
	// but we can a watch that will run a function whenever the value changes
	$scope.$watch("name", function() {
		nameService.name = $scope.name;
	});
	
	// if it exist leave it as it is if not assign 1 to it
	$scope.num = $routeParams.num || 1;
}]);