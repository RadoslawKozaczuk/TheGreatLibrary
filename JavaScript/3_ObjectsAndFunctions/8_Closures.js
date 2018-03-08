function greet(whatosay) {
	// instead of doing the work inside this function
	return function(name) {
		console.log(whatosay + " " + name);
	}
	
	// so instead of a value we get a function that does what we need
}

greet("Hi")("Tony");

var sayHi = greet("Hi");
// this function just finished so technically speaking whattosay variable should be gone

// but is not
sayHi("Tony");

// it all thanks to JS Engine
// it doesn't matter when we invoke the function the engine make sure that whatever function we are running is have an access to all variables it needs to
// JavaScript Engine creates closures we just take advantage of it

function buildFunctions() {
	var arr = [];
	for(var i = 0; i < 3; i++) {
		// arrays are collections of anything
		arr.push(function() {
			console.log(i); // add 3 separate but identical function objects
		});
	}
	
	return arr; // at this moment i is equal to 3, the execution context is destroyed but "whats inside the memory still hangs around"
}

var fs = buildFunctions();
fs[0](); // returns 3
fs[1](); // returns 3
fs[2](); // returns 3

// thanks to closure the 'i' variable is still accessible outside of the scope
// all three function objects reference the same spot in memory through their outer environment variable
// and when they cannot find 'i' variable in their scope they look in the outer environment and they find i which is equals to 3



// what if we actually wanted to make it work
function buildFunctions2() {
	var arr = [];
	for(var i = 0; i < 3; i++) {
		let j = i; // this variable is being created each run but this is ES6 functionality (new version of JS)
		
		arr.push(function() {
			console.log(j);
		});
	}
	
	return arr;
}

var fs2 = buildFunctions2();
fs2[0](); // returns 3
fs2[1](); // returns 3
fs2[2](); // returns 3


// what if we actually wanted to make it work AND not use the new JS syntax
function buildFunctions2() {
	var arr = [];
	for(var i = 0; i < 3; i++) {
		// 2) we need a separate execution context
		arr.push(
			// 3) now we immediately invoked function expression
			// and every time the loop runs it will execute it with a different j
			// and thanks to closures all this j's will be hanging out
			function(j) {
				// 1) in order to preserve the value of 'i' for this function
				return function() {
					console.log(j);	
				}
			}(i) // 4) executing this function gives us the internal function and the particular j is just one scope above
		);
	}
	
	return arr;
}

var fs3 = buildFunctions2();
fs3[0](); // returns 3
fs3[1](); // returns 3
fs3[2](); // returns 3
