var Online = function (){
    var conn = new signalR.HubConnectionBuilder()
        .withUrl("/UsersOnlineHub")
        .build(); 


    conn.start().then(function () {
        
    }).catch(function (err) {
        console.error(err.toString())
    });

    conn.on("GetUsersCounter", function (UsersCounter) {
        document.getElementById("usersOnline").textContent = UsersCounter;
    });
}