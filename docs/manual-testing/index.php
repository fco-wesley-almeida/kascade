<?php

$body='{"a": 1}';
$len_body=strlen($body);

header("Content-Length: $len_body");

echo $body;


