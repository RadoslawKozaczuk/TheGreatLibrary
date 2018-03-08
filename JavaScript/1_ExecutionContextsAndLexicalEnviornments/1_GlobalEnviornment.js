// type 'this' in console to see the object created by the engine
// in a browser the Global Object is the Window object
// in the global context the variable 'this' refers to the Window Object

// In JS global means 'not inside a function'


// this is global
var a = "Hello World!";

function b() {
    
}

// a and b are part of the Window object
// typing 'a', 'this.a' and 'window.a' gives the same result

// Engine also creates a variable names 'Outer Environment' and at this moment it is equal to null