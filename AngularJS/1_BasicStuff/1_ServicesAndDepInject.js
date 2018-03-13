// Problems that AngularJS try to solve

// AngularJS use the MV* - Model, View, Whatever

// === HTML Aside ===
// there is something like custom attribute
// we can create them and they does not affect how the browser display the page

// in HTML5 we suppose to use the data-something pattern
// AngularJS kind of a follow this pattern and all built in attributes are ng-something


// MODULE
var angularApp = angular.module('angularApp', ["ngResource"]); // the second parameter are dependencies

// CONTROLLERS
// the last parameter is the function of the controller
// rest of the parameters are the parameter of the function
// it is done this way to make the controller resistant to minification - of course the order matters
angularApp.controller('mainController', ['$scope', "$log", function ($scope, $log) {
	// AngularJS defines something called $scope. $ in $scope is just part of the name
	// it was injected by Angular
	
	// we can add variables and function to it
	$scope.name = 'Jane Doe';
	$scope.getname = function() {
		return $scope.name;
	}
	
	// and $scope becomes this middle piece between the model and view
	// everything with a $ is a service and will be injected
	
	// to understand it 
	var searchPeople = function(firstName, lastName, $scope, age, occupation) {
		return "Jane Doe";
	}

	console.log(searchPeople(1,2,3,4,5));

	// returns ["firstName", "lastName", "height", "age", "occupation"]
	console.log(angular.injector().annotate(searchPeople));
	// if any of the variables is what AngularJS know it will be injected
}]);
