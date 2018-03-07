/*
	Execution context is created in two phases
		1) Global Object, 'this' and Outer Enviornment are being created. 
			Setup Memory Space for Variables and Functions a.k.a. 'Hoisting'
		2) Execution phase 
*/

b(); // gives correct value
console.log(a); // valid but gives undefined
console.log(c); // gives and error

var a = 'Hello World!';
function b() {
    console.log('Called b!');
}

// The reason why it is like this is because at the moment of execution a and b() exist 
// but all variables in JS are initialized with undefined and computer simply return what is knows about a so undefined