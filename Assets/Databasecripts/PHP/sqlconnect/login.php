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

    //Checking if the name already excists in the database.
    $username = $_POST ["username"];
    $password = $_POST["password"];

    $namequerycheck = "SELECT username, salt , hash, score FROM players WHERE username='" . $username ."'; ";
    $namecheck = mysqli_query($conn, $namequerycheck) or die ("2: Name check failed.");

    if(mysqli_num_rows($namecheck) != 1){
        echo "5: username does not excist";
        exit();
    }

    $existinginfo = mysqli_fetch_assoc($namecheck);
    $salt = $existinginfo["salt"];
    $hash = $existinginfo["hash"];

    $loginhash = crypt($password, $salt);

    if ($hash != $loginhash) {
	    echo("6: password does not match!");
        exit();
    }

    echo "0\t" . $existinginfo["score"];


?>