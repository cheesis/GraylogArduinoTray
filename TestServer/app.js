var http = require('http');
var fs = require('fs');

var serverPort = 8090;
var responseFile = 'response.json';

http.createServer(function (request, response) {
	console.log(request.method, request.url);
	if (request.method === "POST") {
		var requestBody = '';
		request.on('data', function (data) {
			requestBody += data;
		});
		request.on('end', function () {
			var postData = JSON.parse(requestBody);
			console.log('posted data', postData);
			response.writeHead(200, { 'Content-Type': 'application/json' });
            var contents = fs.readFileSync(responseFile).toString();
            //console.log('sent data', contents);
			response.end(contents);
		});
	}
	else {
		response.writeHead(404, 'Resource ' + request.url + ' not found', { 'Content-Type': 'text/html' });
		response.end('<!doctype html><html><head><title>404</title></head><body>404: Resource Not Found</body></html>');
	}
}).listen(serverPort);
console.log('Server running at localhost:'+serverPort);
