// hash change is thrown when hash changes
window.addEventListener('hashchange', function() {
	// so we can see when the hash change and gets its value
	// it is not a real URL value but we will use it as it was
	if (window.location.hash === '#/bookmark/1') {
        console.log('Page 1 is cool.');
    }
    
    if (window.location.hash === '#/bookmark/2') {
        console.log('Let me go get Page 2.');
    }
    
    if (window.location.hash === '#/bookmark/3') {
        console.log('Here\'s Page 3.');
    }
});