$(function () {
    
    var lightState = ["off", "off"];
    var softlockState = false;
    var doorState = ["down", "down"];

    $.connection.deviceCommunicationHub.client.onDoorChange = function (index, status) {
        var img = 'Images/unknown.png';

        if (status === 'down') {
            img = 'Images/closed.png';
        } else if (status === 'up') {
            img = 'Images/open.png';
        }

        $('#door' + index).attr('src', img);

        doorState[index] = status;
    };

    $.connection.deviceCommunicationHub.client.onLightChange = function (index, status) {
        var img = 'Images/lighton.png';
        
        if (status === "off") {
            img = 'Images/lightoff.png';
        }
        
        $('#light' + index).attr('src', img);
        
        lightState[index] = status;
    };

    $.connection.deviceCommunicationHub.client.onLockChange = function (name, garageLocked, hardlock, softlock) {

        var img = 'Images/unlocked.png';
        
        if (softlock) {
            img = 'Images/locked.png';
        }

        $('#softlockImage').attr('src', img);
        $('#garageName').text(name);
        $('#garageLockStatus').text(garageLocked ? "Locked" : "Unlocked");
        $('#hardlockStatus').text(hardlock ? "Locked" : "Unlocked");

        softlockState = softlock;
    };


    $('.lights').click(function () {
        var name = $(this).attr('id');
        var index = +name.substr(name.length - 1, 1);

        var cmd = "on";

        if (lightState[index] === "on") {
            cmd = "off";
        }
        
        $.connection.deviceCommunicationHub.server.activateLight(index, cmd);
    });

    $('#softlockImage').click(function () {
        var cmd = 'lock';
        
        if (softlockState) {
            cmd = 'unlock';
        }
        
        $.connection.deviceCommunicationHub.server.activateSoftLock(cmd);
    });

    $('.doors').click(function () {
        var name = $(this).attr('id');
        var index = +name.substr(name.length - 1, 1);

        var cmd = 'close';
        if (doorState[index] === "down") {
            cmd = 'open';
        } else if (doorState[index] === "unknown") {
            cmd = 'toggle';
        }
        
        $.connection.deviceCommunicationHub.server.activateDoor(index, cmd);
    });

    $.connection.hub.logging = true;

    $.connection.hub.start().done(function() {
        $.connection.deviceCommunicationHub.server.joinGroup('WebApp');
    });

});