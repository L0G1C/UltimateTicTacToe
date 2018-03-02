$(document).ready(function () {

    // Get the user name and store it to prepend chat and move history.
    if ($("#playerName").val() === "" || $("#playerName").val() == undefined) {

        var name = $(".modal").modal();

        $("#newModalForm").validate({
            rules: {
                name: {
                    required: true,
                    minlength: 3
                },
                action: "required"
            },
            messages: {
                name: {
                    required: "Please enter a name",
                    minlength: "Your name must be at least 3 characters"
                },
                action: "Please provide some data"
            }
        });

        name.on('shown.bs.modal',
            function() {
                $('#name').focus();
            });

        $('#playerName').keypress(function (e) {
            if (e.keyCode == 13)
                $('#btnSave').click();
        });
    } else {
        var playerName = $("#playerName").val();
        // Start the connection.
        startConnection('/gamehub',
            function (connection) {

                // Create a function that the hub can call to broadcast messages.
                connection.on('broadcastMessage',
                    function(name, message) {
                        // Add the move to the page history
                        $('#moveHistory').append(name + ": " + message);
                    });

                connection.on('consoleLog',
                    function (message, connId) {
                        var playerObj = { "Name": $('#playerName').val(), "ConnectionId" : connId}
                        console.log(message);
                        $('#playerObjectString').val(JSON.stringify(playerObj));
                    });

                connection.on('newGameInit',
                    function() {
                        connection.invoke('newgame', $('#playerObjectString').val());
                });

                connection.on('newGameComplete',
                    function (gamecode) {
                        $('#inviteDiv').removeClass("hidden");
                        $('#inviteCode').html('Hi ' +
                            playerName +
                            ', you\'re flyin solo right now. Get yourself a wingman (or wingwoman) and send them this link to join up: <span id="gameURL">http://localhost:65350/Game?id=' + gamecode + '</span>');
                        $('#gameCode').html("Game code: " + gamecode);
                    });
            })
            .then(function(connection) {
                //$(".tile").on('click',
                //    function() {
                //        connection.invoke('send', $("#playerName"));
                //    });
            })
            .catch(error => {
                console.error(error.message);
            });

        // Starts a connection with transport fallback - if the connection cannot be started using
        // the webSockets transport the function will fallback to the serverSentEvents transport and
        // if this does not work it will try longPolling. If the connection cannot be started using
        // any of the available transports the function will return a rejected Promise.
        function startConnection(url, configureConnection) {
            return function start(transport) {
                console.log(`Starting connection using ${signalR.TransportType[transport]} transport`)
                var connection = new signalR.HubConnection(url, { transport: transport });
                if (configureConnection && typeof configureConnection === 'function') {
                    configureConnection(connection);
                }

                return connection.start()
                    .then(function() {
                        return connection;
                    })
                    .catch(function(error) {
                        console.log(
                            `Cannot start the connection use ${signalR.TransportType[transport]} transport. ${error
                            .message}`);
                        if (transport !== signalR.TransportType.LongPolling) {
                            return start(transport + 1);
                        }

                        return Promise.reject(error);
                    });
            }(signalR.TransportType.WebSockets);
        }
    }
});