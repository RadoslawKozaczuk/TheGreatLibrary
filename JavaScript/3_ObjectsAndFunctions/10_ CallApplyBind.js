// all functions have code and name properties
// but additionally they have access to call(), apply() and bind() methods
// and all of them apply to this variable
var person = {
	firstname: "John",
	lastname: "Doe",
	
	// method
	getFullName: function() {
		var fullname = this.firstname + " " + this.lastname;
		return fullname;
	}
}


var logName = function(lang1, lang2) {
	// this will fail because 'this' points at the global object
	console.log("Logged: " + this.getFullName());
	
	console.log("Arguments: " + lang1 + " " + lang2);
}

// wouldn't it be nice to have an access to the person object?
// yes! so lets bind it to it
// now it will not fail becasue this will point at the person object
// bind creates a copy of the function
var logPersonName = logName.bind(person);

logPersonName();
logPersonName("english");

// call() simply call the method
// and it first argument is what the 'this' will point at
logName.call(person, "english", "spanish");

// apply is exactly like call with just that difference that parameters are passed in an array
logName.apply(person, ["en", "es"]);

// and obviously it can be applied on the fly
(function something(arg1, arg2) {
	console.log("apply on the fly: " + arg1 + " " + arg2);
}).apply(person, [NaN, 2.718281]);


// where we can use it in real life?

// a) function borrowing
var person2 = {
	firstname: "Jane",
	lastname: "Doe",
}

// we use method of person object on person2 object - we borrowed the function 
// it works as long as we have correct property names
console.log("Let's borrow: " + person.getFullName.apply(person2));


/* === Function Currying
	Creating a copy of a function but with some preset parameters.
	Very useful in mathematical situations.
*/

// b) function currying
function multiply(a, b) {
	return a*b;
}
var multipleByTwo = multiply.bind(this, 2); // bind is permanently setting a to two
console.log("result is: " + multipleByTwo(4));

var multipleByTwo = multiply.bind(this, 2, 2); // now no matter what we pass we will get always the same result
console.log("result is: " + multipleByTwo(3, 17, undefined, NaN, 123.333, "ur mother"));