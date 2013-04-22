(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            initSignalR();
        }
    });

    function initSignalR() {

        var signalRServer = "http://10.0.0.45:18628",
            hubName = "deviceCommunicationHub";

        var connection = $.hubConnection(signalRServer);
        var proxy = connection.createHubProxy(hubName);

        connection.start().done(function () {

            $("#InsideButton, #Inside").click(function () {
                proxy.invoke("XX", "")
            });

            $("#OutsideButtonRight, #OutsideRight, #OutsideButtonLeft, #OutsideLeft").click(function () {
                proxy.invoke("XX", "")
            });

            $("#Door").click(function () {
                proxy.invoke("XX", "")
            });

        });


        //DOOR
        proxy.on('popNotification', function (msg) {
            $('#Door').removeClass('doorClosed');
            $('#Door').attr('class', 'doorOpen');
        });

        //Inside Lights toggle
        proxy.on('XXX', function (id, cmd) {

            switch(cmd)
            {
                case 'on':

                    $('#Inside').removeClass('lightsOff');
                    $('#Inside').attr('class', 'lightsOn');

                    break;
                case 'off':

                    $('#Inside').removeClass('lightsOn');
                    $('#Inside').attr('class', 'lightsOff');

                    break;

                default:
                    //TODO: this should be RED for an ERROR
                    $('#Inside').removeClass('lightsOn');
                    $('#Inside').attr('class', 'lightsOff');
            }

        });

        //OUTSIDE
        proxy.on("", function (msg) {
            $('#OutsideRight, #OutsideLeft').removeClass('lightsOff');
            $('#OutsideRight, #OutsideLeft').attr('class', 'lightsOn');
        });

    }

})();
