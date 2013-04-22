(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            initSignalR();
        }
    });

    var isInsideLightOn, isOutsideLightOn, isDoorOpen;

    function initSignalR() {

        //TODO: add this to the screen so we don't have to recompile...

        var signalRServer = "http://localhost:18628",
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
            };

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
                };

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
                };
            }

        });

    }

})();
