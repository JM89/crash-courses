#  Lesson 2: Creation of a REST API controller

## Description of WeatherForecastController class

Before going to the main topic of this lesson, let's have a look at the WeatherForecast code of the previous lesson. 

Go to `WeatherForecastController.cs`. 
The ASP.NET Core framework provides you with many libraries and foundations for building code. 
For instance, the `WeatherForecastController` below defines an endpoint for your API, while removing a lot of complexity of managing HTTP Requests and Responses. What makes this C# class an API controller is the following information:

```csharp
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    // [...]
}
```

* `[ApiController]` and `[Route("[controller]")]` are called attributes. Attributes associate metadata to a class, information. Here these attributes are used by the ASP.NET Framework to "API Controllers" behaviors to our code. 
    - ApiController is used by the framework to detect the list of available endpoints accross the project. 
    - Route is used to build a routing table. If you remember well, our API endpoint was called using `https://localhost:5001/weatherforecast`, the `weatherforecast` is coming from this `[controller]`: which is the name of the controller in lowercase. This is a standard way of doing, you can override this value.
* `ControllerBase` is the base class and contains generic methods and properties that would be common to all our API controllers. For instance, this would help you build easily HTTP Responses.

This is a glimpse of `ControllerBase` content: 

![01](images/01.png)

The next bit of code that interest us in the context of API Controller is this one: 

```csharp
[HttpGet]
public IEnumerable<WeatherForecast> Get()
{
    // ...
}
```

This defines a GET endpoint. When we called our endpoint through CURL, we use the GET HTTP Method.

```
curl -X GET https://localhost:5001/weatherforecast
```

* The `[HttpGet]` method attribute is telling ASP.NET Core that this specific C# method is an endpoint to expose and make available for HTTP Requests. 
* The return type of this method is an `IEnumerable<WeatherForecast>`: which is a list of objects of type "WeatherForecast" (we define its class, in our Models/ folder). Put simple, when the list of objects is returned by this method, the framework will convert it into in a JSON object and wrap it under an HTTP Response. This is why you will have this results displayed when running the CURL command (or in the browser). If you pay attention to the models/WeatherForecast.cs, you will see similarities on the properties displayed. 

```json
[
  {
    "date":"2021-01-13T08:24:34.7359833+00:00",
    "temperatureC":36,
    "temperatureF":96,
    "summary":"Warm"
  },
  {
    "date":"2021-01-14T08:24:34.7363396+00:00",
    "temperatureC":0,
    "temperatureF":32,
    "summary":"Sweltering"
  },
  ...
]
```

Finally, you might have recognized the constructor of the `WeatherForecastController` and the `ILogger` field. We will come to the structure of the constructor in a next lesson. 

```csharp
public WeatherForecastController(...)
{
    // ...
}
```

Now it is time to take a deep dive into REST API. 

## REST Foundations

There are lots of documentation on the Internet for this that would explain it a lot better, but to keep it short, REST API is a software architectural style to define Web Services in a structured manner. The main idea was to reuse the foundation of HTTP Methods (GET, PUT, POST, DELETE) defined by the HTTP protocol, to describe an API is an easy way. This was also in an effort of designing less complicated web services. Often, an API's job would be to do a certain number of operations such as reading data from a table (or SQL entity) in a database, inserting a row, updating a row using its ID, deleting a row.

| Operations | SQL Operation | HTTP Method |
|---|---|---|
| **C**reate | `INSERT INTO <table> VALUES (...)`  | POST |
| **R**ead | `SELECT * FROM <table>` | GET |
| **U**pdate | `UPDATE <table> SET <prop>=<newval> ...`  | PUT |
| **D**elete | `DELETE FROM <table> ...`  | DELETE |

So, if we have a table `WeatherType` with 4 entries `1: rain`, `2: snow`, `3: wind`, `4: frost` and you wish to create an API on top of this table, you would create an `weathertype` GET, POST, PUT, DELETE endpoints that can be called to manage (read, create, update, delete) entries in your database. 

It might look simple, but in reality, it is slightly more tricky to manage this approach while keeping the request latency low and maintaining associations between different linked SQL entities. They remain 3 different worlds (Network Protocol, Code, RDBMS) trying to cope together. 

But for the purpose of this course, we will keep things simple.

## Create a new controller

TODO