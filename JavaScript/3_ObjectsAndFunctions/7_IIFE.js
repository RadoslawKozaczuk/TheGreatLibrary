// IIFE stands for Immediately Invoked Function Expression

// function statement
function greet(name) {
	console.log("Hello " + name);
}
greet();

// using a function expression
// function can have a name if for some reason we need them to have one
var greetFunc = function troll(name) {
	console.log("Hello " + name);
};
greetFunc("John");

// using an Immediately Invoked Function Expression(IIFE)
var greeting = function(name) {
	return "Hello " + name;
}("John"); // this parenthesis makes the difference
console.log(greeting);


// we cannot have a function expression that is not assigned to anything
//function (name) {
//	console.log("Hello " + name);
//};

// parenthesis makes everything inside it an expression
// so we wrap it up in parenthesis to trick the syntax parser
(function(name) {
	var something = "Inside IIFE: Hello " + name;
	console.log(something + " " + name);
}("Jane")); // and here parenthesis to run it

// and also it can be invoked out of the parenthesis
(function(name) {
	var something = "Inside IIFE: Hello " + name;
	console.log(something + " " + name);
})("Jane");


// what happens behind the scenes
// when engine sees parenthesis it creates an object that stores the function
// and then this object is executed so another execution context is created

// that's why many libraries are written in a way that the whole code is wrapped up with parenthesis to avoid name conflict