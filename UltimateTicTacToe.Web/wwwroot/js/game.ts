let connection = new signalR.HubConnection('/game');

connection.on('send', data => {
    console.log(data);
});

connection.start()
    .then(() => connection.invoke('send', 'Hello World'));