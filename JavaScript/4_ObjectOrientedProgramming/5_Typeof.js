var a = 3;
console.log("typeof 3 is: " + typeof a);

var b = "Hello";
console.log("typeof 'Hello' is: " + typeof b);

var c = {};
console.log("typeof {} is: " + typeof c);

var d = [];
console.log("typeof [] is: " + typeof d + " // not very useful information"); // weird!

// but we can use toString and change what 'this' should point to 
// by using 'call' method - neat trick
console.log("toString.call([]) is: " + Object.prototype.toString.call(d)); // better!

function Person(name) {
    this.name = name;
}

var e = new Person('Jane');
console.log("typeof e is: " + typeof e);

// if we can find 
console.log("e instanceof Person is: " + (e instanceof Person).toString());

console.log("typeof undefined is: " + typeof undefined); // makes sense
console.log("typeof null is: " + typeof null + " // this is BUG"); // a bug since, like, forever...

var z = function() { };
console.log("typeof function() { } is: " + typeof z);

// strict mode
// we use as a literally string
var person;
persom = {}; // object is added to the global object because of the typo
console.log(persom)

// it can be use in a function
function logNewPerson() {
	"use strict" // this is an extra thing and not every browser implement it the same way
	
	var person2;
	persom2 = {};
	console.log(persom2); // throws an exception because in the strict mode
};
logNewPerson();