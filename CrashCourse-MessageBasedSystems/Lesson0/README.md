# Lesson 0: Starter Kit

In the [CrashCourse API](../CrashCourse-API/README.md), we learnt how to build an API from scratch and some basic concept of programming. 

In order to get started quickly and focus on service communication, the ./Lesson1/Prep folder contains the following items:

* A VisualStudio solution containing two projects:
    * A lightweight BlogPost API 
    * A lightweight Review API 
* A docker-compose file starting SEQ for logging.

In order to better explain the reasons for using message-based systems, we will first cover the direct communication between two APIs [Lesson 1](../Lesson1/README.md) and its challenges. We will start sending app metrics to a monitoring system (Prometheus/Grafana) [Lesson 2](../Lesson2/README.md). Then, we will use a SQS queue (localstack in docker) to decouple the two, and processing messages by a background service [Lesson 3](../Lesson3/README.md) & [Lesson 4](../Lesson4/README.md).  

## Getting started

To start the docker-compose, go to the ./Lesson1/Prep folder and run the following command to start the container in background:

```
docker-compose up -d
```

And check SEQ: `http://localhost:5341/#/events`

To start the two APIs, stay in the ./Lesson1/Prep folder 

```sh
dotnet build
```

Then run in two separated consoles the two projects.

**BlogPost.Api**

```sh
dotnet run --project ./CrashCourseApi.BlogPost.Web/CrashCourseApi.BlogPost.Web.csproj --configuration Release
```

There is only one endpoint available: `https://localhost:5001/api/blogpost`.

**Review.Api**

```
dotnet run --project ./CrashCourseApi.Review.Web/CrashCourseApi.Review.Web.csproj --configuration Release
```

There are two endpoints available:
- GET `https://localhost:5003/api/review`
- POST `https://localhost:5003/api/review`

Example of POST (check the logs to see the logged entry):
```sh
curl -k -X POST https://localhost:5003/api/review -H "Content-Type: application/json" --data "{\"blogpostid\": 1, \"reviewers\":[\"jack\", \"matt\", \"steven\"]}"
```

## API Structure

If you checked the API, you will see that there are fairly simple. If you compare them to [CrashCourse API Lesson 5](../CrashCourse-API/Lesson5/Final/CrashCourseApi.sln), you might spot a couple of differences:
* A lot of classes & methods have disappeared: we do not access to a datastorage anymore. These are very simple endpoints doing very simple things. 
* A Settings.cs file and appSettings section have been added: this is in preparation of the specific environment specific configuration that we will add. This file is initialize in the Startup.cs, ConfigureServices method. 
* The ILogger has been added to the controllers so we can log the requests. 
* The Review GET endpoint is marked as asynchronous. We will not cover async/await in this crashcourse.

## Simulate Latency & Errors

In order to simulate latency and random errors, the Review API contains two configurations used in the GET endpoint:
- InducedFailureRateFactor: number between 0 and 100, represents the percentage of failures happening for a given request.
- InducedLatencyFactor: min duration in seconds to enforce during the call.

As soon as you are familiar with this structure, let's jump to Lesson 1.