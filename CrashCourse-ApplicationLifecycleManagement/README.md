# Crash Course: Application Lifecycle Management

## Introduction

Unlike the two previous courses, this one will not contain so much C# coding. This course aims to bring some insights into all the underlying phases involved in software development, release and maintenance. In an Agile and microservice context, this often occupies a significant amount of time for the developers.

Developing a new application "from scratch" can be a thrilling activity. But the life of your software truly starts when it is used, by end-users or other services, somewhere else than on your local machine. Past the big moment of the first release, comes the maintenance of the software, which to some aspects, might not appear as exciting. 

A software application has a cyclic lifecycle which often revolves into these steps:
1. Plan business features, changes, bug fixes
2. Develop (Local)
3. Test (QA)
4. Release (Production)

Repeat. 

When an application is released and starts being used, there is usually a feedback loop: changes and bug fixes must be done to the applications, which then needs to be released again. 

Agile methodology favours shorter iterations of the ALM cycle for quicker feedback and adjustments to business requirements. With this approach, we prefer small changes, that can be rollbacked easily if needs, with minimised disruption. As a consequence of this, the time between development and release must be as short as possible, while limiting the risk of issues. Automated pipelines for Continuous Integration and Deployment (CI/CD), are keys to achieving this. 

## Lessons

* [Lesson 1: Local Development & Source Control](./Lesson1/README.md)
* [Lesson 2: Continuous Integration](./Lesson2/README.md)
* [Lesson 3: Continuous Deployment](./Lesson3/README.md)
* [Lesson 4: Containerization with Docker](./Lesson4/README.md)

## Objectives

By the end of this course, you should have an understanding of the main phases of the software development lifecycle, CI/CD, as well as containerization. 

## Disclaimer

We are in the .NET 6 era for few months now when I am writing the content of this course. However, I'll stay on .NET 3.1 for continuity with the previous crash courses. I actually see this as an excellent opportunity to cover code migration and its challenges in another tutorial (like continuously outdated content :wink:)!