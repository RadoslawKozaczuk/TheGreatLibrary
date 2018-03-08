/* === Asynchronous === 
	More than one at a time. */

/*
	there is a list inside JavaScript Engine named EventQueue
	it holds things like HttpRequest or Click
	when the execution stack is empty JavaScript Engine look in that list
	lets sat there is a Click event
	then Engine creates execution context for lets sat clickHandler function
	then when that finishes Engine looks again on the list and so on
*/

// long running function
function waitThreeSeconds() {
    var ms = 3000 + new Date().getTime();
    while (new Date() < ms){}
    console.log("finished function");
}

function clickHandler() {
    console.log("click event!");   
}

// listen for the click event
document.addEventListener("click", clickHandler);

waitThreeSeconds();
console.log("finished execution");

/*
	no matter if we click or not first the function will finish then the global execution context
	and Engine will process the event only at the moment when the execution stack is empty
	so asynchronous calls are possible but only outside of the engine. Internally JavaScript process 
	event in the order they came in and only after it finished processing current task
*/
