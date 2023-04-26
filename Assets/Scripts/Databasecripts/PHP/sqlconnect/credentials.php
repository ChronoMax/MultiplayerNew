<?php 
    $servername = "localhost";
    $username = "root";
    $password = "";
    $database = "multiplayergame";

    // Create connection
    $conn = mysqli_connect($servername, $username, $password, $database);

    // Check connection
    if (mysqli_connect_errno()) {
        echo "1: Connection failed";
    }
?>