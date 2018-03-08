// a bit about JSON
var objectLiteral = {
	firstName: "Mary",
	lastName: "Watson"
}
console.log(objectLiteral);

var jsonValue = JSON.parse('{ "firstName": "Mary", "lastName": "Watson"}');
console.log(jsonValue);

/* === First Class Functions ===
	Everything you can do with other types you can do with functions. 
	Assign them to variables, pass them around, create them on the fly.
*/


/* Function is a special type of object.
	It may have attached primitives, other objects, functions
	And two special properties:
	- Name // which is optional and when equal to null we call the funciton anonymous funciton
	- Code // which can be run	
	So function can be passed to other object just like a number or boolean value */

function greet() {
	console.log("hi");
}

greet.language = "english"; // we can attacha property to an object

console.log(greet); // JS engine retuns the code of the function as text
console.log(greet.language);