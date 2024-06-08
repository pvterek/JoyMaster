"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/consoleHub").build();

connection.on("ReceiveMessage", function (message) {
    var div = document.createElement("div");
    document.getElementById("consoleOutput").appendChild(div);
    div.textContent = message;
});

connection.start().then(function () {
    console.log("SignalR connected.");
}).catch(function (err) {
    console.error(err.toString());
});

function sendCommand(clientId) {
    var command = $('#command').val();;

    $.ajax({
        type: 'POST',
        url: '/Clients/ExecuteCommand',
        data: JSON.stringify({ ClientId: clientId, Command: command }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    });
}