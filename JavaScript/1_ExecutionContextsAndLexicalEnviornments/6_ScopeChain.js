// Scope
//	Where a variable is available in your code. And it maybe the same variable or a new copy.

// lexical environment make the difference
// b sits in the file so it will have reference to the global object
function b() {
	console.log("myVar = " + myVar);
}

// same for a
function a() {
	var myVar = "a";
	console.log("myVar = " + myVar);
    b();
}

var myVar = "global";
console.log("myVar = " + myVar);
a();

/* what happens:
	Every execution context has a reference to the outer environment.
	In this case both functions sits lexically sits at the global level.
	So both function has the reference to the Global Execution Context,
	and when compilator could not find myVar inside the b context 
	it looked for it in the Outer Environment.
	
	It goes down to the global context and it is called Scope Chain
*/

// here we change nesting a little bit
function c() {
	function d() {
		console.log("myVar2 = " + myVar2);
	}
	
	var myVar2 = "c";
	console.log("myVar2 = " + myVar2);
    d();
}

var myVar2 = "global2";
console.log("myVar2 = " + myVar2);
c();


// in ES6 there is also let keyword
if(2 > 1) {
	let c = true; // this variable is available only in the scope in was created in
}

for (i = 0; i < 10; i++) { 
    let troll = 123; // this variable will be recreated in every cycle
}