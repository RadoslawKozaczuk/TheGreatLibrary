// array is an object and each of its object is a name-value pair
var arr = ["John", "Jane", "Jim"];

for (var prop in arr) {
	console.log(prop + ": " + arr[prop]);
	// 0, 1, 2 elements are property names
}

// and here is the problem what if somebody add something to the prototype
Array.prototype.myTrollProperty = "troll";

for (var prop in arr) {
	console.log(prop + ": " + arr[prop]); // we get that extra property
}

// thats why it is not recommended to use for in loop for arrays
for(var i = 0; i < arr.length; i++) {
	// we use standard for loops to avoid iterating over the prototype
	console.log(arr[i]);
}

// Pure Prototypal Inheritance
var person = {
	firstname: "Default",
	lasttime: "Default",
	greet: function() {
		return "Hi " + this.firstname;
	}
}

// creates an object and then use it as a prototype for another object
var john = Object.create(person);
console.log(john); // it creates an empty object with a __proto__ variable pointing at the whatever we passed in

// and then we hide the prototype variable by creating new ones on the object level
john.firstname = "John";
john.lastname = "Doe";
console.log(john.greet());

/* === Polyfill ===
	Code that adds a feature which the engine may lack.
*/

// === ES6 new concepts ===

// even tho JS have 'class' keyword it does not work like expected
// it is not a definition it a an object
// and them by using new keyword we create new instances based on that one instance
class ImportantPerson {
	constructor(firstname, lastname) {
		this.firstname = firstname;
		this.lastname = lastname;
	}
	
	greet() {
		return "Hi " + firstname;
	}
}

var importantJohn = new ImportantPerson("John", "Doe");
console.log(importantJohn);

// there is also 'extends' keyword which basically just sets the __proto__ variable to what we provided
class InformalPerson extends ImportantPerson {
	constructor(firstname, lastname) {
		super(firstname, lastname);
	}
	
	greet() {
		return "Hi " + this.firstname + " (said casually)";
	}
}

var informalJohn = new InformalPerson("John", "Lemon");
console.log(informalJohn.greet());

// Under the hood it is all working the same way it is just a syntactic sugar 
// so 'class' and 'extends' keywords ultimately just do prototypal inheritance but use different syntax