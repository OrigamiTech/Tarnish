<?php
// put this on the server.
$uploadfile=$_FILES['userfile']['name'];
if (move_uploaded_file($_FILES['userfile']['tmp_name'], $uploadfile))
	die("OK\r\n$uploadfile");
die("ERROR");
?>