﻿"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/commandHub")
    .build();

connection.on("ReceiveMessage", function (message) {
    var currentConnectionGuid = document.getElementById("connectionGuid").value;
    if (message.connectionGuid === currentConnectionGuid) {
        var div = document.createElement("div");
        document.getElementById("consoleOutput").appendChild(div);
        div.textContent = message.messageContent;
    }
});

connection.start();

function sendCommand(connectionGuid) {
    var command = $('#command').val();

    $.ajax({
        type: 'POST',
        url: '/Clients/ExecuteCommand',
        data: JSON.stringify({ ConnectionGuid: connectionGuid, MessageContent: command }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    });
}
$(function () {
    $('#command').on('keydown', function (event) {
        if (event.key === 'Enter') {
            sendCommand($('#connectionGuid').val());
            $('#command').val('');
            event.preventDefault();
        }
    });
});
