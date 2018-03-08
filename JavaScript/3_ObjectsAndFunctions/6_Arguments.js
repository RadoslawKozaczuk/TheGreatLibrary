// in JS arrays are dynamic
var arr = [
	1, 
	false, 
	{
		name: "Tony"
	},
	function(name) {
		var greeting = "Hello ";
		console.log(greeting + name);
	},
	"Hello",
	NaN,
	undefined
];

console.log(arr);
arr[3](arr[2].name);


// JS engine gives you a keyword 'arguments' which contains all the parameters of the particular function
// this variable is attached to the function object

function greet(firstName, lastName)
{
	// arguments is a array-like structure
	if(arguments.length === 0)
	{
		console.log("Missing parameters!");
		return;
	}
	
	// default value
	lastName = lastName || "Default";
	// in case of undefined (which coerce to false) the '||' operator will return the other value
	
	// when we call greet() without any parameters
	// JS will create these variables and simply not assign anything to it
	// so it will be undefined + " " + undefined
	console.log(firstName + " " + lastName);
}

greet();
greet("John");

// there is no function overloading in JS

/* === Automatic Semicolon Insertion ===
	A very dangerous thing.
	Where the Syntax Parser expects semicolon to be it will insert one.
	We don't want JS Engine to do for us because it may lead to errors that are very hard to track down. */

function getPerson() {
	return // Engine will put a semicolon here making this function to return undefined
	{
		firstname: "Tony"
	} 
}
console.log(getPerson());

// to avoid to write it the way Engine won't make this stupid assumption
function getPersonBetter() {
	return {
		firstName: "Tony"
	}
}
console.log(getPersonBetter());