# Crash Course: Introduction to Message Based Systems

## Audience

New to .NET, New to C#.

## Objective

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

* [Lesson 0: Starter Kit](./Lesson1/README.md)
* [Lesson 1: Introduction to services (de)coupling](./Lesson1/README.md)
* [Lesson 2: Measuring the health of our "distributed" system](./Lesson2/README.md)
* [Lesson 3: Implementing a background service](./Lesson3/README.md)
* [Lesson 4: Sending & Receiving SQS messages](./Lesson4/README.md)
