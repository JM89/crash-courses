# Lesson 4: Adding application metrics & monitoring [BONUS]

The more services you had into your distributed system, the more difficult it becomes to monitor it. Decoupling helps on service scalability, availability and reliability, but it also makes the investigations of issues way harder. Being able to monitor the health of your services becomes primordial in this context.

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

Open the Blog Post API solution



TODO

## Update Prometheus Targets

`host.docker.internal` or --network="host" or create images from docker course?

## Conclusion

We have just seen how to add application metrics and provide dashboards to monitor the health of our services. There is commonly a 3rd "pillar" of Observability: Distributed Tracing. 

There isn't any course for this yet, but check out the sample code available [here](https://github.com/JM89/test-distributed-tracing) if you are interested. 