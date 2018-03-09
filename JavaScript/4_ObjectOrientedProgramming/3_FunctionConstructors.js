/* === Function Constructor ===
	A normal function that is used to construct objects.
	The 'this' variable points a new empty object, and that object is returned from the function automatically.
*/

// this function does not return anything
function Person(firstname, lastname) {
	// 'this' points at an empty object in the memory
	console.log(this);
	this.firstname = firstname;
	this.lastname = lastname;
	console.log("This function is being invoked");
}

// this is the right way of setting the prototype
var john = new Person("John", "Doe");

// new is actually an operator
// when we call new operator it first creates a new empty object and sets 'this' variable of the function Person at this new empty object
// and then because our function does not return anything it returns what 'this' points at

// then it changes what the 'this' variable of that object is pointing at
console.log(john);

// every function in JS has a property named prototype which is always set to null
// prototype property is not __proto__ property - these two are different
// prototype property is set only when we use 'new' keyword
Person.prototype.getFullName = function() {
	return this.firstname + " " + this.lastname;
}

// when you call a 'new' keyword it creates a new object and it sets the prototype to what the __proto__ points at
var jane = new Person("Jane", "Doe");
console.log(jane.getFullName());

// Dangerous thing - if we forget new keyword
var jim = Person("Jim", "Doe");
console.log(jim); // we get undefined
// the only way to mitigate the threat is to follow the convention that every function that was
// intended to be function constructor has name that start with a capital letter


// Build-in function constructors
var num = new Number(3); 
console.log(num); // wow look it is not a primitive type it is an object with a primitive type in it
console.log(num.__proto__); // Number object

// function constructors create objects

// engine can also box something into an object
console.log("John".length); // primitive is being wrap up with an object so we have access to length method

// we can add methods to basic objects
String.prototype.isLengthGreaterThan = function(limit) {
	return this.length > limit;
}
console.log("is 'John' longer than 2? " + "John".isLengthGreaterThan(2));

Number.prototype.isPositive = function() {
	return this > 0;
}
// we cannot use this
//console.log(3.isPositive()); // won't work because while string was automatically converted to an object a number is not

// but this will work
console.log("Number(3).isPositive(): " + Number(3).isPositive());

// NOTE: Extending prototypes of a basic object may cause unexpected events and is not recommended unless it is really necessary 
var a = 3;
var b = new Number(3);

// true because b is converted and false because different types
console.log("a == b is " + (a == b).toString() + " but a === b is " + (a === b).toString());