# Lesson 4: Adding application metrics & monitoring [BONUS]

The more services you had into your distributed system, the more difficult it becomes to monitor it through just the logs. Decoupling helps on service scalability, availability and reliability, but it also makes the investigations of issues way harder. Being able to monitor the health of your services becomes primordial in this context. 

You won't be able to prevent all failures from happening, but you can be better equipped to deal with them. Adding application metrics, such as measuring the average duration of calling an API or counting unexpected errors occur, help to define what's a normal behavior from a not-so one, and having a "baseline" to compare against. 

In this Bonus lesson, we will start all 3 services created/updated in the previous lessons. The ./Prep folder contains everything you need to get started:
- BlogPost API
- Review API
- RequestReviewProcessor Background Service

As well as an updated docker-compose file, that contains a docker setup for Prometheus (App Metrics collector) and Grafana (Dashboard). 

To get started, run the `docker-compose up` command in the ./Prep folder.

## Prometheus & Grafana Monitoring Systems

[Prometheus](https://prometheus.io/docs/introduction/overview/#architecture) collects application metrics by looking at an `/metrics` endpoint exposed by your applications (targets). The Prometheus server jobs will pull metrics of your targets every x seconds. 

In this lesson, Prometheus will run in a docker contain and all applications will run in local. In an real-life setup where the address IPs of your services might be dynamic, you would need a discovery service mechanism to refresh target's configurations when needed. 

The metrics in Prometheus can be queried from the server using [PromQL](https://prometheus.io/docs/prometheus/latest/querying/basics/), but to visualize them in a user-friendly dashboard, you would need a proper tool,

For .NET applications, Prometheus integrates with [AppMetrics](https://www.app-metrics.io/getting-started/), a standard library for managing application metrics.

If your docker containers are running already, go to http://localhost:9090.

[Grafana](https://grafana.com/docs/grafana/latest/dashboards/?pg=docs) is used for creating dashboards and alerts. The integration with Prometheus is quite simple and an docker image is available for it. 

Other data sources are available, amongst DataDog and Amazon CloudWatch.

If your docker containers are running already, go to http://localhost:3000. Username: admin, Password: securitymatters123.

Check that the Prometheus DataSource was configured properly by clicking on the data source and then "Save and Test" in the edit form. The message should be "Data source is working".

![](images/01.png)

In this docker setup, Prometheus and Grafana uses  configuration files in ./Docker folder. Check the docker-compose container definitions for more information.

When our services will be set up, we will update the `prometheus.yml` file and restart the Prometheus container. 

## Setup of AppMetrics in the APIs

Both Blog Post API & Review API will need the same setup so we will only cover the Blog Post API here.

Open the Blog Post API solution.

### Step 1: Use App.Metrics to report metrics to the console

1. Install `App.Metrics` & `App.Metrics.Reporting.Console` Nuget Package.

2. Configure the AppMetrics service. In your Startup.cs, add the following code in the ConfigureServices method along with all your services.

```csharp
// Creation of a metrics root object
var metrics = new MetricsBuilder()
                    .Report.ToConsole() // Add Reporting to Console
                    .Build();
// Add singleton metrics root object 
services.AddSingleton<IMetrics>(metrics);
```
3. Inject the metrics root object in the BlogPostController (`IMetrics _metrics`)

4. Use `_metrics` in the "GET" endpoint: 

```csharp
// GET: api/<BlogPostController>
[HttpGet]
public IActionResult Get()
{
    // Specifications of what to measure
    var requestTimer = new TimerOptions
    {
        Name = "Request Timer",
        MeasurementUnit = Unit.Requests,
        DurationUnit = TimeUnit.Milliseconds,
        RateUnit = TimeUnit.Milliseconds
    };

    // Starting a timer, which will be stopped when `time` is disposed.
    using (var time = _metrics.Measure.Timer.Time(requestTimer))
    { 
        // Content of the GET endpoint

        var blogPostEntities = _blogPostService.GetAll();

        if (blogPostEntities == null)
            return StatusCode(503);

        return Ok(blogPostEntities.Select(x => new BlogPostResponse()
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            CreationDate = x.CreationDate
        }));
    }
}
```

If you run the API at this point, nothing happens. 

The metrics data is not pushed to the Console directly but "buffered" in memory. The main reason is that any instructions (such as logging data in console) takes "time", a very small amount here, but still, we do not want to slow down our endpoint for the purpose of monitoring. 

For the metrics data to be pushed to the console, we need an independent thread. A thread allows to divide a program in multiple running tasks: it will execute in parallel of your main code and let the API available for other requests (in the main thread). Similarly to having a background/hosted service (cf. RequestReviewProcessor), we need a running task in background that deals with pushing the data to the console. Fortunately, the code is much simpler, and we can keep it "inline". 

Below the configuration of your appMetrics service, add the following code: 

```csharp
// Add a scheduler task that will run every 10s to flush the metrics to the Console
var scheduler = new AppMetricsTaskScheduler(TimeSpan.FromSeconds(10), 
    async () => {
        await Task.WhenAll(metrics.ReportRunner.RunAllAsync());
    });
scheduler.Start();
```

If you run the application now, it should start sending "reports" even if you haven't called yet your endpoint.

![](images/02.png)

And when you triggered your endpoint, it will start changing those metrics:

![](images/03.png)

As you can seem allong with the time of the last request, some statistics are also computed (99th, 95th percentile, mean...). 

Having the metrics data in the Console makes it as readable as in a logging system and we can't do analyze/visualize them like this. Our Prometheus integration requires that the application exposes an endpoint `/metrics` so let's configure one. 

### Step 2: Expose a /metrics endpoint

1. Install the `App.Metrics.AspNetCore.Endpoints` Nuget Package.
2. In the Startup.cs file, `ConfigureServices` method, replace `services.AddSingleton<IMetrics>(metrics);` call with: 

```csharp
// Metrics-related
services.AddMetrics(metrics);
services.AddMetricsEndpoints();

// Writing synchronous calls won't work without changing this global settings:
services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});
```

3. Still in the Startup.cs file, `Configure` method this time, add the following code one line before the `app.UseEndpoints(...)` call.

```csharp
app.UseMetricsAllEndpoints();
```

4. Run your application and go the metrics endpoint `https://localhost:5001/metrics`

![](images/04.png)

### Step 3: Prometheus integration

Now that we exposed a metrics endpoint, we need to convert the format to be readable by Prometheus server. We will use the plain-text output format. 

1. Install `App.Metrics.Prometheus` Nuget Package
2. Reconfigure the metrics root (ConfigureServices method), as `OutputMetrics.AsPrometheusPlainText()`

```csharp
var metrics = new MetricsBuilder()
    .Report.ToConsole()
    .OutputMetrics.AsPrometheusPlainText()
    .Build();
```

3. Run your application and go the metrics endpoint `https://localhost:5001/metrics`

![](images/05.png)

### Step 4: Remove the reports to the console

Now that our metrics endpoint is ready, we can remove the reporting to the Console.

In ConfigureServices, remove `.Report.ToConsole()` and `AppMetricsTaskScheduler`

### Step 5: Add Metrics Tags

Right now, our metrics does not contain much data to identify which application and endpoint has been called. When you have a lot more metrics coming from different sources, you will have to know the origin of your data. 

We can add tags to our metrics, which will be used to filter them.

We can add Global Tags, such as the ServiceName:

```csharp
var metrics = new MetricsBuilder()
    .Configuration.Configure(opt => {
        opt.GlobalTags.Add("service", settings.ServiceName);
    })
    .OutputMetrics.AsPrometheusPlainText()
    .Build();
```

Back in BlogPostController class, where TimerOptions was defined, we can add tags. The syntax is a bit unusual with keys and values being defined separately:

```csharp
var requestTimer = new TimerOptions
{
    Name = "Request Timer",
    MeasurementUnit = Unit.Requests,
    DurationUnit = TimeUnit.Milliseconds,
    RateUnit = TimeUnit.Milliseconds,
    Tags = new MetricTags(new string[] { "resource" }, new string[] { "blog-post" })
};
```

We can also pass tags in the measure setup: 

```csharp
using (var time = _metrics.Measure.Timer.Time(requestTimer, new MetricTags(new string[] { "call-type" }, new string[] { "datastore" })))
{

}
```

The TimerOptions can be a unique static object, reused for all endpoints. By setting it static, we can reduce the size of the code slightly, remove memory allocations and standardize our metrics (always use ms for instance). Also you can add some helper methods since the syntax of app metrics can be a bit curious at first.

```csharp
private readonly static TimerOptions _timerOptions = new TimerOptions
{
    Name = "Request Timer",
    MeasurementUnit = Unit.Requests,
    DurationUnit = TimeUnit.Milliseconds,
    RateUnit = TimeUnit.Milliseconds,
    Tags = new MetricTags(new string[] { "resource" }, new string[] { "blog-post" })
};

private MetricTags AddEndpointName(string endpointName) => new MetricTags(new string[] {"endpoint"}, new string[] {endpointName});

[HttpGet]
public IActionResult Get()
{
    using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("get-all")))
    { 
        //...
    }
}

 [HttpGet("{id}")]
public IActionResult Get(int id)
{
    using (var time = _metrics.Measure.Timer.Time(_timerOptions, AddEndpointName("get-by-id")))
    { 
        //...
    }
}
```

![](images/07.png)

### Step 6: Counting Errors

The AppMetrics propose several [metric types](https://www.app-metrics.io/getting-started/metric-types/), but another very common use case is counting errors since this will help detecting service degradation quickly. 

1. Inject _metrics into the BlogPostDataStore, where we already have some try/catch to handle errors. 
2. Set a static object CounterOptions

```csharp
private readonly static CounterOptions _sqlErrorCounterOptions = new CounterOptions() {
    MeasurementUnit = Unit.Errors,
    Name = "sql-errors"
};
```

3. Increment the counter when catching the exception

```csharp
public IEnumerable<BlogPost> SelectAll()
{
    IEnumerable<BlogPost> blogPosts = null;
    using (var conn = new SqlConnection(_connectionString))
    {
        try
        {
            blogPosts = conn.Query<BlogPost>("Select BlogPostId as Id, Title, Content, CreationDate from [BlogPost]").AsList();
        }
        catch (Exception ex)
        {
            _metrics.Measure.Counter.Increment(_sqlErrorCounterOptions);
            _logger.Error(ex, TemplateException, DataStore, "SelectAll");
        }
        finally
        {
            conn.Close();
        }
    }
    return blogPosts;
}
```

To simulate a DB outage, stop the docker container for sql-server. The GetAll Blog Post call will fail several seconds (depending on your SQL timeout parameter).

![](images/08.png)

## Update Prometheus Targets

`host.docker.internal` or --network="host" or create images from docker course?

## Conclusion

We have just seen how to add application metrics and provide dashboards to monitor the health of our services. There is commonly a 3rd "pillar" of Observability: Distributed Tracing. 

There isn't any course for this yet, but check out the sample code available [here](https://github.com/JM89/test-distributed-tracing) if you are interested. 