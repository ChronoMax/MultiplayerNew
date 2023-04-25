<?php 
    include 'credentials.php';

    //Checking if the name already excists in the database.
    $username = $_POST ["username"];
    $password = $_POST["password"];

    $namequerycheck = "SELECT username FROM players WHERE username='" . $username ."'; ";
    $namecheck = mysqli_query($conn, $namequerycheck) or die ("2: Name check failed.");

    if(mysqli_num_rows($namecheck) = 0){
        echo"3: Name already exist."; //if the name already exists, give back this code.
        exit();
    }

    //Adding user to database
    $salt = "\$5\$round=5000\$" . "steamedhams" . $username . "\$";
    $hash = crypt($password, $salt);

    $insertuserquery = "INSERT INTO players (username, hash, salt) VALUES ('" . $username . "', '" . $hash . "', '" . $salt . "'); ";

    mysqli_query($conn, $insertuserquery) or die ("4: inserting player failed");
?>