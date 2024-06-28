"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/screenHub")
    .build();

connection.on("ReceiveScreenData", (base64Image) => {
    const imageElement = document.getElementById("clientImage");

    if (imageElement) {
        imageElement.src = 'data:image/png;base64,' + base64Image;
    } else {
        const newImageElement = new Image();
        newImageElement.id = "clientImage";
        newImageElement.src = 'data:image/png;base64,' + base64Image;
        newImageElement.style.width = "100%";
        newImageElement.style.height = "auto";
        newImageElement.style.objectFit = "contain";

        const clientDesktop = document.getElementById("clientDesktop");
        clientDesktop.appendChild(newImageElement);
    }
});

connection.start();
