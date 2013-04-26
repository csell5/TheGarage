(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            initSignalR();
        }
    });

    var isInsideLightOn, isOutsideLightOn, isDoorOpen, isDoorLocked;

    function initSignalR() {

        //TODO: add this to the screen so we don't have to recompile...

        var signalRServer = "http://thegarageportal.cloudapp.net",
            hubName = "deviceCommunicationHub";

        var connection = $.hubConnection(signalRServer);
        var proxy = connection.createHubProxy(hubName);

        connection.start().done(function () {

            $("#InsideButton, #Inside").click(function () {
                if (isInsideLightOn) {
                    proxy.invoke("ActivateLight", "0", "off");
                } else {
                    proxy.invoke("ActivateLight", "0", "on");
                }
            });

            $("#OutsideButtonRight, #OutsideRight, #OutsideButtonLeft, #OutsideLeft").click(function () {
                if (isOutsideLightOn) {
                    proxy.invoke("ActivateLight", "1", "off");
                } else {
                    proxy.invoke("ActivateLight", "1", "on");
                }
            });

            $("#Door, #DoorClickPath").click(function () {
                if (isDoorOpen) {
                    proxy.invoke("ActivateDoor", "0", "close");
                } else {
                    proxy.invoke("ActivateDoor", "0", "open");
                }

                //TODO: THERE IS ALSO A TOGGLE STATE
            });

            
            $("#lockButton").click(function () {
                
                if (isDoorLocked) {
                    proxy.invoke("ActivateSoftLock", "unlock");
                } else {
                    proxy.invoke("ActivateSoftLock", "lock");
                }

                //TODO: THERE IS ALSO A TOGGLE STATE
            });

        });

        proxy.on('OnDoorChange', function (id, status) {
            
            switch (status) {
                case 'up':
                    isDoorOpen = true;

                    $('#Door').removeClass('doorClosed doorUnknown');
                    $('#Door').attr('class', 'doorOpen');

                    break;

                case 'down':
                    isDoorOpen = false;

                    $('#Door').removeClass('doorOpen doorUnknown');
                    $('#Door').attr('class', 'doorClosed');

                    break;

                case 'unknown':
                    $('#Door').removeClass('doorOpen doorClosed');
                    $('#Door').attr('class', 'doorUnknown');

                    //TODO some animation here should be cool.

                    break;
                default:
            }

        });

        //Inside Lights toggle
        proxy.on('OnLightChange', function (id, status) {

            if ( id === 0) {
                switch (status) {
                    case 'on':
                        isInsideLightOn = true;

                        $('#Inside').removeClass('lightsOff');
                        $('#Inside').attr('class', 'lightsOn');

                        break;
                        //WHAT HERE

                    case 'off':
                        isInsideLightOn = false;

                        $('#Inside').removeClass('lightsOn');
                        $('#Inside').attr('class', 'lightsOff');

                        break;

                    default:
                        //WHAT HERE
                }

            } else if (id === 1) {

                switch (status) {
                    case 'on':
                        isOutsideLightOn = true;

                        $('#OutsideRight, #OutsideLeft').removeClass('lightsOff');
                        $('#OutsideRight, #OutsideLeft').attr('class', 'lightsOn');

                        break;

                    case 'off':
                        isOutsideLightOn = false;

                        $('#OutsideRight, #OutsideLeft').removeClass('lightsOn');
                        $('#OutsideRight, #OutsideLeft').attr('class', 'lightsOff');

                        break;

                    default:
                        //WHAT HERE
                }
            }

        });

        proxy.on('OnLockChange', function (name, garage, hardlock, softlock) {
            
            $("#hardLock").hide();

            if (hardlock || softlock) {
                isDoorLocked = true;
                //update UI text here.
                $("#lockStatus").text("Locked");
            }

            if (hardlock) {
                // do something to represent the hardlock
                $("#hardLock").show();
            }

            if (!hardlock && !softlock) {
                isDoorLocked = false;
                
                $("#lockStatus").text("UnLocked");
                $("#hardLock").hide();
            }
        });
    }
})();
