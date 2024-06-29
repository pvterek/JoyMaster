"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/imageHub")
    .build();

var updateImageElement = (imageElement, base64Image) => {
    imageElement.src = `data:image/png;base64,${base64Image}`;
};

var createImageElement = (base64Image) => {
    var newImageElement = new Image();
    newImageElement.id = "clientImage";
    newImageElement.src = `data:image/png;base64,${base64Image}`;
    newImageElement.style.width = "100%";
    newImageElement.style.height = "auto";
    newImageElement.style.objectFit = "contain";

    return newImageElement;
};

var handleImageData = (imageData) => {
    var currentConnectionGuid = document.getElementById("connectionGuid").value;
    if (imageData.connectionGuid !== currentConnectionGuid) {
        return;
    }

    var base64Image = imageData.base64Image;
    var imageElement = document.getElementById("clientImage");

    if (imageElement) {
        updateImageElement(imageElement, base64Image);
    } else {
        var newImageElement = createImageElement(base64Image);
        var clientDesktop = document.getElementById("clientDesktop");
        clientDesktop.appendChild(newImageElement);
    }
};

connection.on("ReceiveImageData", handleImageData);

connection.start();
