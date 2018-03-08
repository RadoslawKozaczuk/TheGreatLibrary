var english = {}; // creating an object
var spanish = {}; 

// and then we can add fields
english.greet = "Hello";
spanish.greet = "Hola";

// english.greetings.greet = "Hello"; // we cannot create on the fly
// Error: Cannot set property 'greet' of undefined

// or we can simply create object like this
var english = { 
	greetings: 
	{ 
		basic: "Hello"
	} 
}

console.log(english.greetings.basic);