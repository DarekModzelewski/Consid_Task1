### ﻿Azure Functions app fetching weather data for London every minute and storing results in Azure Storage (Blob + Table).

## Tech Stack

    .NET 6
    Azure Functions v4 (in-process)
    TimerTrigger & HttpTrigger
    Azure Blob Storage (local: Azurite)
    Azure Table Storage (local: Azurite)
    OpenWeatherMap API
    Dependency Injection (Startup.cs)

## Functionality
Trigger 	Description
TimerTrigger 	Fetches weather data every 1 minute for London
HttpTrigger /logs 	Lists logs from Table Storage in a given date range
HttpTrigger /payload 	Returns payload (weather JSON) for a specific RowKey

## Data Flow

    TimerTrigger hits OpenWeather API
    Saves response to:
        Blob Storage (<RowKey>.json)
        Table Storage (WeatherLog entity)
    RowKey is shared across both storages


## Configuration

Create a local.settings.json file (not committed to Git):

```json
{
“IsEncrypted”: false,
“Values”: {
“AzureWebJobsStorage”: “UseDevelopmentStorage=true”,
“FUNCTIONS_WORKER_RUNTIME”: “dotnet”,
“OpenWeatherApiKey”: “YOUR_API_KEY_HERE”
}
}
```

## Running Locally

Step 1: Start Azurite (Azure Storage Emulator) 

```bash
-docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002
-—name azurite
-mcr.microsoft.com/azure-storage/azurite
```
This starts Azurite with ports:

    Blob: 10000
    Queue: 10001
    Table: 10002

Option 1 – Recommended: Use Docker Compose 

If you have docker-compose installed, simply run:
 ```bash
docker-compose up -d 
 ```
This will start Azurite on ports:
Blob: http://localhost:10000 
Queue: http://localhost:10001
Table: http://localhost:10002 

### This project includes a preconfigured docker-compose.yml file for convenience. 

### 🛠 Option 2 – Manual Docker Run (if not using Compose) 
bash 
 ```
docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 --name azurite mcr.microsoft.com/azure-storage/azurite

 ```

Step 2: Start the Function App

Navigate to the function project directory (where host.json is located) and run:

bash 
 ```
func start

 ```

You should see output like:

Functions: 
#### GetLogs: [GET] http://localhost:7071/api/logs 
#### GetPayload: [GET] http://localhost:7071/api/payload 
#### WeatherTimerFunction: timerTrigger


#### Every 1 minute, the timer function will trigger and store data in local Azure Storage.
API Endpoints: URL Method Description
####  /api/logs?from=…&to=… 
GET Logs from Table Storage Example: 
#### /api/logs?from=2025-04-06T16:00:00Z&to=2025-04-06T17:00:00Z GET /api/payload/{logId}
Fetches the weather payload from Blob Storage based on the log ID (rowKey).
#### Example: 
#### /api/payload/573915d0-c748-44a1-96a3-a456db52a670

## NuGet Packages

##### Microsoft.Azure.Functions.Extensions
##### Microsoft.Extensions.Http
##### Azure.Data.Tables
##### Azure.Storage.Blobs


## Unit & Integration Tests

### This project includes two types of tests for the WeatherApiService: Type Description 

 #### -Unit Test Mocks HttpClient to test how the service behaves on success/failure.
 #### -Integration Test Sends real HTTP request to OpenWeatherMap API to validate real responses. 

### API Key Required for Integration Test

#### ⚠️ To run the integration test, you must provide a real OpenWeather API key.
#### It can be injected via InMemoryCollection in the test or by environment variable:

C# 
 ```
var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "OpenWeatherApiKey", "YOUR_REAL_API_KEY" } }).Build();

```
#### Without a valid API key, the integration test will fail or throw an exception.




