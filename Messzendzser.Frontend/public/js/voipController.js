var socket = new JsSIP.WebSocketInterface('ws://localhost');
var configuration = {
    sockets: [socket],
    uri: 'sip:voip@localhost',
    password: 'Password1!'
};

var ua = new JsSIP.UA(configuration);

ua.start();

// Register callbacks to desired call events
var eventHandlers = {
    'progress': function (e) {
        console.log('call is in progress');
    },
    'failed': function (e) {
        console.log('call failed with cause: ' + e.data.cause);
    },
    'ended': function (e) {
        console.log('call ended with cause: ' + e.data.cause);
    },
    'confirmed': function (e) {
        console.log('call confirmed');
    }
};

var options = {
    'eventHandlers': eventHandlers,
    'mediaConstraints': { 'audio': true, 'video': false }
};

//var session = ua.call('sip:bob@example.com', options);