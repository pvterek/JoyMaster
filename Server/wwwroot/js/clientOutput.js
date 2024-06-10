"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/consoleHub").build();

connection.on("ReceiveMessage", function (message) {
    var currentClientId = document.getElementById("clientId").value;
    if (message.clientId === currentClientId) {
        var div = document.createElement("div");
        document.getElementById("consoleOutput").appendChild(div);
        div.textContent = message.message;
    }
});

connection.start();

function sendCommand(clientId) {
    var command = $('#command').val();

    $.ajax({
        type: 'POST',
        url: '/Clients/ExecuteCommand',
        data: JSON.stringify({ ClientId: clientId, Message: command }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    });
}
