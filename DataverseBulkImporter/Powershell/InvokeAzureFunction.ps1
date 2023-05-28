# Start Time for Application

$StartDatetime = Get-Date;
Write-Host $StartDatetime.ToUniversalTime();
Write-Host "Simulating 100 worker threads x 20 degree of parallelism (2000 threads)."


# ThrottleLimit for Parellel ForEach. Azure functions can timeout 

1..100 | ForEach-Object -ThrottleLimit 16 -Parallel {
	Write-Host "Trigger worker $_ ...starting"
	$userIndex = $_ % 15;
    $baseUrl = "https://<your-az-func>.azurewebsites.net/api/AzFuncBulkOperationOrchestrator?code=&records=100000&dop=50&batch=100&userIndex=$userIndex&correlationId=24bd4ffa-9069-43c9-a863-83a4301be3f4";
	$RequestApp = Invoke-WebRequest -Uri $baseUrl -Headers @{ "Content-Type" = "application/json"} -Method Get -Body $json  -UseBasicParsing;

	Write-Host "Trigger worker $_...complete"
}

$Enddatetime = Get-Date;
Write-Host $Enddatetime.ToUniversalTime();