<?php 
    include 'credentials.php';

    //Checking if the name already excists in the database.
    $username = $_POST ["username"];
    $score = $_POST["score"];

    $namequerycheck = "SELECT username FROM players WHERE username='" . $username ."'; ";
    $namecheck = mysqli_query($conn, $namequerycheck) or die ("2: Name check failed.");
        if(mysqli_num_rows($namecheck) != 1){
        echo "5: username does not excist";
        exit();
    }

    $updatequery = "UPDATE players SET score = " . $score . " WHERE username = '" . $username . "';";
    mysqli_query($conn, $updatequery) or die ("7 : Saving query failed");

    echo 0;
?>