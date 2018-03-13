// Events are always thrown we just need to listen to them

// AngularJS add on the Angular Context

// when you add something on the $scope Angular know it needs to keep track on them
// everytime something that might have changed the value Angular check the old and new value

// there is something knows an the Digest Loop which checks if anything changes
// and if so it checks if something else might have changed after that change
// and it loop over until all old and new values match and then at the end it updates the dom


var myApp = angular.module("myApp", []);
myApp.controller("mainController", ["$scope", "$filter", function($scope, $filter) {
	$scope.lowercasehandle = function() {
		return $filter("lowercase")($scope.handle);
	}
	
	$scope.handle = "";
	
	// everytime 
	$scope.$watch("handle", function(newValue, oldValue) {
		console.info("Changed!");
		console.log("Old: " + oldValue);
		console.log("New: " + newValue);
	});
	
	
	// setTimeout set asynchronous call that executes after 3 seconds
	// this will not be noticed by Angular because it happens outside of its context
	setTimeout(function() {
		
		// to avoid this problem we may either use timeout provided by Angular or
		// use the use function $apply
		// normally whatever you do is automatically wrapped up with the $apply function
		$scope.$apply(function() {
			$scope.handle = "newtwitterhandle";
			console.log("Scope Changed!");
		});
	}, 3000);
}]);