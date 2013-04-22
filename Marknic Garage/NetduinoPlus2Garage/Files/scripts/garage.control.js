
var garageManager = new function () {
  var alerts = '';
  var operational = true;

  function getLockStatus(state) { return state ? 'Locked' : 'Unlocked'; };

  function extractCommand(commandString, command) {

    commandString = commandString.toLowerCase();
    command = command.toLowerCase();

    var component = commandString.substr(0, 4);
    var componentNoPos = 5;

    if (component === 'soft') {
      component = 'softlock';
      componentNoPos = +8;
    } else if (component === 'door') {
      componentNoPos = +4;
      var prefix = command.substr(0, 4);

      switch (prefix) {
        case 'open':
          command = 'open';
          break;
        case 'clos':
          command = 'close';
          break;
        default:
          command = 'toggle';
      }
    } else {
      component = 'light';

      if (command.substr(-2, 2) == 'on') {
        command = 'on';
      } else {
        command = 'off';
      }
    }

    var componentNo = Number(commandString.substr(componentNoPos, 1));

    command = command.toLowerCase();

    var commandReturn = { component: component, componentNumber: componentNo, command: command };

    return commandReturn;
  };


  function getSoftLockButtonText(state) { return state ? 'Unlock' : 'Lock'; };
  function getLightStatus(state) { return state.toLowerCase() === 'on' ? 'On' : 'Off'; };

  function getLightButtonText(state) { return state.toLowerCase() === 'on' ? 'Off' : 'On'; };

  function getDoorStatus(statusNumber) {
    switch (statusNumber) {
      case 'up':
        return 'Door is Up';
      case 'down':
        return 'Door is Down';
    }
    return 'Unknown';
  };

  function getDoorButtonText(statusNumber) {
    switch (statusNumber) {
      case 'down':
        return 'Open';
      case 'up':
        return 'Close';
    }
    return 'Toggle';
  };

  return {
    updateStatusValues: function () {
      $.ajax({
        type: 'GET',
        url: '/status',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

          alerts = '';
          var hardwareLock = getLockStatus(data.HardwareLock);
          operational = true;

          $('#garageCaption').text('Garage ' + data.Name + ' Status: ' + getLockStatus(data.Locked));

          $('#hardwareLock').text(hardwareLock);

          $('#softwareLock').text(getLockStatus(data.SoftLock));
          $('#softLock0').attr('value', getSoftLockButtonText(data.SoftLock));

          if (data.Door.length > 0) {

            for (var i = 0; i < data.Door.length; i++) {
              var doorButtonText = getDoorButtonText(data.Door[i].Status);

              $('#door' + i + 'Status').text(getDoorStatus(data.Door[i].Status));
              $('#door' + i).attr('value', doorButtonText + ' Door');

              if ((data.Door[i].Duration > 20)) {
                if (data.Door[i].Status === 'up') {
                  alerts += '<p class="highlight-2"><strong>Notice:</strong> The garage door (' + data.Door[i].Name + ') has been open for over 2 hours.</p>';
                } else if (data.Door[i].Status === 'unknown') {
                  alerts += '<p class="highlight-2"><strong>Notice:</strong> The garage door (' + data.Door[i].Name + ') has been in an unknown state for over 2 hours.</p>';
                }
              }
            }
          }

          if (data.Light.length > 0) {

            for (i = 0; i < data.Light.length; i++) {
                var lightButton = getLightButtonText(data.Light[i].Status);
                var tst = getLightStatus(data.Light[i].Status);

              $('#light' + i + 'Status').text(getLightStatus(data.Light[i].Status));
              $('#light' + i).attr('value', 'Turn ' + lightButton);

              if ((data.Light[i].Duration > 30) && (data.Light[i].Status === 'on')) {
                alerts += '<p class="highlight-2"><strong>Notice:</strong> The lights (' + data.Light[i].Name + ') has been on for over 2 hours.</p>';
              }
            }
          }

          if (operational) {
            alerts += '<p class="highlight-3"><strong>Confirmation:</strong> All garage systems are active.</p>';
          }

          $('#alerts').html(alerts);
        },
        error: function (err) {
          operational = false;

          alerts = '<p class="highlight-1"><strong>Alert:</strong> ' + 'Error: web service failed to respond.</p>';

          $('#alerts').html(alerts);
        }
      });
    },

    updateStatus: function () {
      $.ajax({
        type: 'GET',
        url: '/status',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
          var tableRows = '';
          var hardwareLock = getLockStatus(data.HardwareLock);
          this.operational = true;

          $('#garageCaption').text('Garage ' + data.Name + ' Status: ' + getLockStatus(data.Locked));
          $('#statusTable').append('<tbody><tr><th>Lock</th><th>Status</th><th>Action</th></tr></tbody>');
          tableRows += '<tr><td>Hardware Lock</td><td><div id="hardwareLock">' + hardwareLock + '</div></td><td>&nbsp;</td></tr>';
          tableRows += '<tr><td>Soft Lock</td><td><div id="softwareLock">' + getLockStatus(data.SoftLock) + '</div></td><td><input id="softLock0" type="button" value="' + getSoftLockButtonText(data.SoftLock) + '" /></td></tr>';
          tableRows += '<tr><th>Lock</th><th>Status</th><th>Action</th></tr></tr><tr><td colSpan="3">&nbsp;</td></tr>';
          tableRows += '<tr><th>Door</th><th>Status</th><th>Action</th>';

          if (data.Door.length > 0) {

            for (var i = 0; i < data.Door.length; i++) {
              var doorButtonText = getDoorButtonText(data.Door[i].Status);

              tableRows += '<tr><td>' + data.Door[i].Name + '</td><td><div id=door' + i + 'Status>' + getDoorStatus(data.Door[i].Status) + '</div></td><td><input id="door' + i + '" type="button" value="' + doorButtonText + ' Door" /></td></tr>';
            }

            tableRows += '<tr><th>Door</th><th>Status</th><th>Action</th></tr><tr><td colSpan="3">&nbsp;</td></tr>';
          }

          if (data.Light.length > 0) {

            tableRows += '<tr><th>Light</th><th>Status</th><th>Action</th></tr>';

            for (i = 0; i < data.Light.length; i++) {
              var lightButton = getLightButtonText(data.Light[i].Status);

              tableRows += '<tr><td>' + data.Light[i].Name + '</td><td><div id=light' + i + 'Status>' + getLightStatus(data.Light[i].Status) + '</div></td><td><input id="light' + i + '" type="button" value="Turn ' + lightButton + '" /></td></tr>';
            }

            tableRows += '<tr><th>Light</th><th>Status</th><th>Action</th></tr>';
          }

          $('#statusTable tr:last').after(tableRows);

          if (operational) {
            alerts += '<p class="highlight-3"><strong>Confirmation:</strong> All garage systems are active.</p>';
          }

          $('#alerts').html(alerts);

          $('input').click(function () {

            var jsonDataToSend = JSON.stringify(extractCommand($(this).attr('id'), $(this).attr('value')));

            $.ajax({
              type: 'POST',
              url: '/trigger',
              data: jsonDataToSend,
              contentType: 'application/json;charset=utf-8',
              dataType: 'json',
              error: function (err) {
                window.alert('Error:' + err.responseText + '  Status: ' + err.status);
              }
            });
          });
        },
        error: function (err) {
          operational = false;

          alerts += '<p class="highlight-1"><strong>Alert:</strong> ' + 'Error: web service failed to respond.</p>';

          $('#alerts').html(alerts);
        }
      });
    }
  };

};

$(document).ready(function () {

  window.setInterval('garageManager.updateStatusValues()', 2000);
  garageManager.updateStatus();

});
