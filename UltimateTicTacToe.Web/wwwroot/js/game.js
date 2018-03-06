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
                        console.log(message);

                        if (connId != undefined) {
                            var playerObj = { "Name": $('#playerName').val(), "ConnectionId": connId }                         
                            $('#playerObjectString').val(JSON.stringify(playerObj));

                            if ($('#gameObjectString').val() != undefined && $('#gameObjectString').val() !== "") {
                                var gameObj = $.parseJSON($('#gameObjectString').val());
                                // This is joining Player so set him up in game.
                                connection.invoke('AddPlayer', $('#playerObjectString').val(), JSON.stringify(gameObj.GameId));
                            }
                        }
                    });

                connection.on('newGameInit',
                    function () {
                        if ($('#gameObjectString').val() === "" || $('#gameObjectString').val() == undefined) {
                            connection.invoke('newgame', $('#playerObjectString').val());
                        }
                    });

                connection.on('newGameComplete',
                    function (gamecode) {                        
                        $('#inviteCode').html('Hi ' +
                            playerName +
                            ', you\'re flyin solo right now. Get yourself a wingman (or wingwoman) and send them this link to join up: <span id="gameURL">http://localhost:65350/Game?id=' + gamecode + '</span>');
                        $('#gameCode').html("Game code: " + gamecode);
                        $('#playerAlbl').html('Player One: ' +  $('#playerName').val());
                    });

                connection.on('playerTurn',
                    function (game) {
                        var thisPlayer = $.parseJSON($('#playerObjectString').val());
                        $('#playerBlbl').html('Player Two: ' + game.playerB.name);                        
                        // alter gameboard based on gameObject (active player, moveHistory, etc)
                        if (game.activePlayer === thisPlayer.ConnectionId) {
                            TileSelect(game.moveHistory);
                        } else {
                            DisableTiles();
                        }
                    });
            })
            .then(function (connection) {
             $(".tile").on('click',
                    function() {
                        connection.invoke('tileClicked', $("#playerName"));
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
                console.log(`Starting connection using ${signalR.TransportType[transport]} transport`);
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


var TileSelect = function (moveHistory) {
    $('#inviteCode').html('Make your move!!');
    if (moveHistory != null) {
        // Look at last move and depending on what was clicked, highlight that tic tac toe game.
    }
};

var DisableTiles = function () {
    $('#inviteCode').html('Wait your turn...');
    $('.tile-button').addClass('disabled').attr('disabled');
};