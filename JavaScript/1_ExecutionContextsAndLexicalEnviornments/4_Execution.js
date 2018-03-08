/* JavaScript is:
Single Threaded - one command at a time. Under the hood of the browser it maybe be different. 
	But for us programmers JS behaves like a single threaded language.
Synchronous - one at a time, and in order.

*/

// Invocation - running a function. In JavaScript by using parenthesis

// even though a is above b it is present in the memory
function a() {
	b();
	var c;
}

function b() {
	var d; // like in c# this does not collide with the one below because of the different context
}

a();
var d;

/*
	when it comes to executing a() line 18 another execution context is being created and put on the top of the execution stack
	and the one that is on top is the one that is currently running
	and then it creates another execution context of b() and put it on the top of the stack
	so basically every function creates an execution context that goes through he creation phase and executes code line by line
*/