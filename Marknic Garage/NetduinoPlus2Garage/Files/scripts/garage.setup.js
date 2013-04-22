$(document).ready(function () {

    var lightCount = '0';

    $('#doorCount').change(function () {
        var doorCount = $('#doorCount').val();

        updateDoorCount(doorCount);
    });

    $("#garageSetupForm").validate({
        rules: {
            garageName: "required",
            doorName1: "required",
            doorName2: "required",
            lightName1: "required",
            lightName2: "required"
        },
        messages: {
            garageName: "Please enter the garageName.",
            doorName1: "Please enter door name 1.",
            doorName2: "Please enter door name 1.",
            lightName1: "Please enter light name 1.",
            lightName2: "Please enter light name 2."
        }
    });


    $('#resetConfig').click(function () {
        $.ajax({
            type: 'POST',
            url: '/reset',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                var i;
                var lightCountToRemove = +$('#lightCount').val();
                var doorCountToRemove = +$('#doorCount').val();

                for (i = 0; i < lightCountToRemove; i++) {
                    $('#lightTable > tbody:last tr:last').remove();
                }

                for (i = 0; i < doorCountToRemove - 1; i++) {
                    $('#doorTable > tbody:last tr:last').remove();
                }

                setValuesFromConfiguration();

                alert('Configuration Values Reset');
            },
            error: function (err) {
                alert('Error:' + err.responseText + '  Status: ' + err.status);
            }
        });
    });

    $('#lightCount').change(function () {
        var lightCountTmp = $('#lightCount').val();

        updateLightCount(lightCount, lightCountTmp);

        lightCount = lightCountTmp;
    });



    function setValuesFromConfiguration() {
        lightCount = '0';

        $.ajax({
            type: 'GET',
            url: '/configuration',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {

                var lightLength = 0, i;

                $('#doorCount').val(data.Door.length);

                if (data.Light !== undefined) {
                    lightLength = data.Light.length;
                }

                $('#lightCount').val(lightLength);

                if (data.Door.length == 2) {
                    $('#doorTable > tbody:last').append('<tr><td>Door #2 Name:</td><td><input id="doorName2" name="doorName2" type="text" /></td></tr>');
                }

                $('#locked').val(data.Locked.toString());

                updateLightCount(lightCount, lightLength.toString());

                lightCount = lightLength.toString();

                $('#garageName').val(data.Name);

                for (i = 0; i < data.Door.length; i++) {
                    $('#doorName' + (i + 1)).val(data.Door[i].Name);
                }

                for (i = 0; i < lightLength; i++) {
                    $('#lightName' + (i + 1)).val(data.Light[i].Name);
                    $('#lightSetting' + (i + 1)).val(data.Light[i].Status);
                }
            },
            error: function (err) {
                alert('Error:' + err.responseText + '  Status: ' + err.status);
            }
        });
    }


    //function convertOnOff(status) {
    //    return status.toLowerCase() === 'on' ? 'on' : 'off';
    //}

    function updateDoorCount(currentDoorCount) {
        if (currentDoorCount == 2) {
            $('#doorTable > tbody:last').append('<tr><td>Door #2 Name:</td><td><input id="doorName2" name="doorName2" type="text" /></td></tr>');
        } else {
            $('#doorTable > tbody:last tr:last').remove();
        }
    }

    function updateLightCount(currentLightCount, newLightCount) {

        if ((currentLightCount == 0) && (newLightCount == 0)) return;

        switch (currentLightCount) {
            case '0':
                $('#lightTable > tbody:last').append('<tr><td>Light #1 Name: <input id="lightName1" name="lightName1" type="text" /></td><td>Initial Setting: <select id="lightSetting1" name="lightSetting1"><option value="off" selected="selected">off</option><option value="on">on</option></select></td></tr>');
                if (newLightCount === '2') {
                    $('#lightTable > tbody:last').append('<tr><td>Light #2 Name: <input id="lightName2" name="lightName2" type="text" /></td><td>Initial Setting: <select id="lightSetting2" name="lightSetting2"><option value="off" selected="selected">off</option><option value="on">on</option></select></td></tr>');
                }
                break;

            case '1':
                if (newLightCount === '0') {
                    $('#lightTable > tbody:last tr:last').remove();
                }

                if (newLightCount === '2') {
                    $('#lightTable > tbody:last').append('<tr><td>Light #2 Name: <input id="lightName2" name="lightName2" type="text" /></td><td>Initial Setting: <select id="lightSetting2" name="lightSetting1"><option value="off" selected="selected">off</option><option value="on">on</option></select></td></tr>');
                }
                break;

            case '2':
                $('#lightTable > tbody:last tr:last').remove();

                if (newLightCount === '0') {
                    $('#lightTable > tbody:last tr:last').remove();
                }
                break;
        }

    }

    setValuesFromConfiguration();
});

