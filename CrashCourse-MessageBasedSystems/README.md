# Crash Course: Introduction to Message Based Systems

## Audience

New to .NET, New to C#.

## Objective

In the [CrashCourse API](../CrashCourse-API/README.md), we learnt how to build an API from scratch and some basic concept of programming. 

In order to better explain the reasons for using message-based systems, we will first cover the direct communication between two APIs [Lesson 1](../Lesson1/README.md) and its challenges. We will start sending app metrics to a monitoring system (Prometheus/Grafana) [Lesson 2](../Lesson2/README.md). Then, we will use a SQS queue (localstack in docker) to decouple the two, and processing messages by a background service [Lesson 3](../Lesson3/README.md) & [Lesson 4](../Lesson4/README.md).  

By the end of the course, you will have implemented:
- an API: sending messages to a SQS queue (localstack in docker)
- a background service: reading the messages and calling another API

## Pre-requisites

* Visual Studio 2019 Community is installed (3.1 SDK installed)
    - To check your install, run `dotnet --list-sdk` command
    - If another version of the SDK is installed, syntax/template might be different
* Docker is installed
* [CrashCourse API](./CrashCourse-API/README.md) completed

## Lessons

* [Lesson 1: Introduction to services (de)coupling](./Lesson1/README.md)
* [Lesson 2: Measuring the health of a distributed system](./Lesson2/README.md)
* [Lesson 3: Implementing a background service](./Lesson3/README.md)
* [Lesson 4: Sending & Receiving SQS messages](./Lesson4/README.md)

## Getting started

In order to get started quickly and focus on service communication, the ./Lesson1/Prep folder contains the following items:

* A VisualStudio solution containing our BlogPost API created in the [CrashCourse API Lesson 5](../CrashCourse-API/Lesson5/Final/CrashCourseApi.sln).
* A VisualStudio solution containing a lightweight Review API containing a single endpoint.
* A docker-compose file starting SEQ for logging.

### Docker Setup 

To start the docker-compose, go to the ./Lesson1/Prep folder and run the following command to start the container in background:

```
docker-compose up -d
```

Check SEQ: `http://localhost:5341/#/events` and the SQL Server `localhost,1433` with SSMS.

### BlogPost.Api endpoints

Open the solution `/BlogPost/CrashCourseMessaging.BlogPost.Api.sln` and run it.

The same endpoints are available for the Blog Post API using the Postman collection. Couple of changes were made:
- The namespaces were renamed
- The weather forecast controller was removed
- The Settings class and Settings section in configuration file were added, and configured in Startup class (ConfigureServices).

### Review.Api endpoints

Open the solution `/Review/CrashCourseMessaging.Review.Api.sln` and run it.

Two endpoints are available:
- GET `https://localhost:5003/api/review` that does nothing.
- POST `https://localhost:5003/api/review` 

Example of POST (check the logs to see the logged entry):
```sh
curl -k -X POST https://localhost:5003/api/review -H "Content-Type: application/json" --data "{\"blogpostid\": 1, \"reviewers\":[\"jack\", \"matt\", \"steven\"]}"
```

If you check the class `ReviewController.cs`, you will notice that the POST controller isn't empty. 

```csharp
[HttpPost]
public async Task<IActionResult> Post([FromBody] ReviewRequest request, CancellationToken token)
{
    // Log to SEQ
    _logger.Information("Review POST endpoint called");

    // Simulate a failure based on InducedFailureRateFactor settings
    if (random.Next(1, 100) < _settings.InducedFailureRateFactor)
    {
        // Log to SEQ
        _logger.Error("Review GET endpoint failed");
        
        return UnprocessableEntity("Review GET endpoint failed");
    }

    // Simulate a latency based on InducedLatencyFactor settings
    await Task.Delay(_settings.InducedLatencyFactor * 1000, token);

    // Transform the request object into a string
    var flatRequest = JsonConvert.SerializeObject(request);

    return Ok($"Review POST endpoint called with body request {flatRequest}");
}
```

In order to simulate latency and random errors, the Review API contains two configurations used in the GET endpoint (appSettings.json):
- InducedFailureRateFactor: number between 0 and 100, represents the percentage of failures happening for a given request.
- InducedLatencyFactor: min duration in seconds to enforce during the call.

*Note that the endpoint is marked as asynchronous. We will not cover async/await in this crash course.*

As soon as you are familiar with this structure, let's jump to Lesson 1.

