var arr1 = [1,2,3];
console.log(arr1);

var arr2 = [];

// a loop that doubles the array
for(var i = 0; i < arr1.length; i++) {
	arr2.push(arr1[i] * 2);
}
console.log(arr2);

// functional programming
function mapForEach(arr, fn) {
	var newArr = [];
	for(var i = 0; i < arr.length; i++) {
		newArr.push(
			fn(arr[i]) // call the 'fn' function and push it to each element
		); 
	}
	return newArr;
}

// and here we apply additional work to do for each element
var arr3 = mapForEach(arr1, function(item) {
	return item * 2;
});
console.log(arr3);

// thanks to it we can do entirely different work every time 
var arr4 = mapForEach(arr1, function(nameDoesntMatter) {
	return nameDoesntMatter > 2;
});
console.log(arr4);

// what if the function have more parameters that the one we used in the mapForEach function
var checkPastLimit = function(limiter, item) {
	return item > limiter; // this gives back a boolean
};

// we have to set one parameter to default value
// bind makes a copy of the function in which the limiter is already a 1
var arr5  = mapForEach(arr1, checkPastLimit.bind(this, 1));
console.log(arr5);

// what if we want to avoid calling bind all the time
// what if wanted to pass the limiter as the only parameter
// can we create a function that we give only the limiter and gives me back something like this: checkPastLimit.bind(this, 1)
var checkPastLimitSimplified = function(limiter) {
	return function(limiter, item) {
		return item > limiter;
	}.bind(this, limiter);
};
var arr6 = mapForEach(arr1, checkPastLimitSimplified(2));
console.log(arr6);