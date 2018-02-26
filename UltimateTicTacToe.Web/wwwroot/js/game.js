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
    }

    //$('#myModal').on('hidden.bs.modal', function (e) {
    //    $("#playerName").val(e);
    //})
   

    // Start the connection.
    startConnection('/gamehub', function (connection) {
        // Create a function that the hub can call to broadcast messages.
        connection.on('broadcastMessage', function (name, message) {
            // Add the move to the page history
            $("#moveHistory").append(name + ": " + message);
        });

        connection.on('connectionmade',
            function (message) {
                console.log("Connection Established!!");
            });
    })
    .then(function (connection) {
        $(".tile").on('click',
            function() {
                connection.invoke('send', name, $("#playerName"));
            });
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
                .then(function () {
                    return connection;
                })
                .catch(function (error) {
                    console.log(`Cannot start the connection use ${signalR.TransportType[transport]} transport. ${error.message}`);
                    if (transport !== signalR.TransportType.LongPolling) {
                        return start(transport + 1);
                    }

                    return Promise.reject(error);
                });
        }(signalR.TransportType.WebSockets);
    }
});