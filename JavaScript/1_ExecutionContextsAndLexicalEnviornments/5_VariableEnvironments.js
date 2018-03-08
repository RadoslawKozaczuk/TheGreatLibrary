// Variable Environment - where the variables live and how they relate to each other.

function b() {
	var myVar;
	console.log("myVar=" + myVar);
}

function a() {
	var myVar = 2;
	console.log("myVar=" + myVar);
    b();
}

var myVar = 1;
console.log("myVar=" + myVar);
a();

/* what happens:
	global execution context is created and myVar is part of the Global Object and it is equal to 1
	then another execution context is created for a and myVat is equal to 2
	and finally the same for b and myVar is equal to undefined
*/