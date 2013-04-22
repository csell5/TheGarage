$(function() {
  $.connection.sampleHub.client.broadcast = function(value) {
    $('#BroadcastText').html('<pre>Broadcast: ' + value.Now + ' ' + value.Integer + ' ' + value.Text + '</pre>');
  };

  $.connection.sampleHub.client.broadcastToGroup = function(value) {
    $('#BroadcastToGroupText').html('<pre>BroadcastToGroup: ' + value + '</pre>');
  };

  $.connection.sampleHub.client.othersCallback = function(value) {
    $('#OthersCallbackText').html('<pre>OthersCallback: ' + value.Text + '</pre>');
  };

  $.connection.hub.logging = true;

  $.connection.hub.start().done(function () {
    var request = {};
    request.Text = 'This is a request from a WebApp!';

    $.connection.sampleHub.server.request(request).done(function(response) {
      $('#ResponseText').html('<pre>Response: ' + response.Text + '</pre>');
    });
    $.connection.sampleHub.server.requestAsync(request).done(function(response) {
      $('#ResponseAsyncText').html('<pre>ResponseAsync: ' + response.Text + '</pre>');
    });
    $.connection.sampleHub.server.requestWithCallbackAsync(request);
    $.connection.sampleHub.server.joinGroup('WebApp');
  });
  
});