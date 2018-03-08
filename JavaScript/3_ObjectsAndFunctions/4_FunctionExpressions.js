/* === Expression === 
	a unit of code that results in a value. */


// function statement - it does not return a value
greet(); // and due to hoisting it is available ahead of time
function greet() {
	console.log("Hi");
}

// this variable will contain a function object
// the name of the function is null but we don't need that because we have a reference to it
// this is a function expression because it result in an function object being created
var anonymousGreet = function() {
	console.log("Hi");
}

// now how to invoke object's code?
// oh guess what is so easy just add parenthesis to its name
anonymousGreet();


function log(a) {
	console.log(a); // log whatever you get
	a(a); // run it - that may throw a TypeError exception that something is not a function
}

log(function(a) {
	console.log(a);
});

// it returns the code of the function twice
// why?
// first it simply log the object which in case of a function means log its code
// then it run the function with itself as a object and the function simple log itself


/* === Mutate === 
	to change something. "Immutable" means it can't be changed. */

// All primitives are copied by value
// All objects including functions are copied by reference