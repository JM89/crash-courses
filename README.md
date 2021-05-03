# Crash-Courses (.NET)

These crash courses are aimed to an audience with a technical background with no prior experience in C#/.NET/OOP, who wishes to understand better software engineering.

## Pre-requisites

Each course have a couple of pre-requisites, but these are a MUST. 

* Visual Studio 2019 Community is installed (3.1 SDK installed)
    - To check your install, run the `dotnet --list-sdk` command
    - If another version of the SDK is installed, syntax/template might be different
* Docker is installed

## Structure

The concepts, methodologies, architecture patterns are unveiled progressively and the difficulty increases incrementally as you go through each lessons. For this reason, I recommend you read them sequentially and in this order:

|| Course | Concepts (Intro) | Technologies | Libraries |
|---|---|---|---|---|
|1| [API](./CrashCourse-API/README.md) | REST API <br/>Layered-Architecture <br/> RDBMS/NoSQL <br/> Model Validation <br/> Error Handling (Try/Catch) <br/> Unit-Testing <br/> Dependency Injection <br/> | Docker <br/> SQL Server <br/> SEQ | ASP.NET Core <br/> System.Data.SqlClient <br/> Serilog |
|2| [Inter-Process Communication](./CrashCourse-InterProcessCommunication/README.md) | Microservices/Monoliths <br/> Distributed Systems<br/> Service decoupling <br/> Worker service <br/> Few OOP Concepts | Docker <br/> SEQ <br/> Localstack <br/> AWS SQS <br/> Prometheus <br/> Grafana <br/> | System.Net.Http <br/> Serilog <br/> Polly <br/> AWSSDK.SQS <br/> App.Metrics |

Most lessons contains:
- a `./Prep` folder that brings docker dependencies and optionally some VS solutions for the lesson. 
- a `./Final` folder with the solutions. 

As this content targets beginners, I try to follow some storyline to keep the readers interested. As a result, more advanced concepts are sometimes cover superficially (eg. async, threading, dispose patterns, etc.).