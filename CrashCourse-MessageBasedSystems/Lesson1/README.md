# Lesson 1: Introduction to services (de)coupling

*Note: If you haven't done yet, get started with the "Getting Started" section in [CrashCourse Message-based system](./CrashCourse-MessageBasedSystems/README.md)*

Consider the following business requirement: 

```
Given I am writing blog post content 
When I create a new Blog Post 
Then a Review Request should be submitted
```

To give a bit more business context, when a review is submitted, a list of reviewers can comment and approve the blog post content, but this is not part of the scope of the requirement. From the Blog Post API perspective, we only have to create the Review Request. In this Lesson 1, we will call directly the Review API POST endpoint.

## Side note: monolithic/microservices & atomic transactions

In a monolithic architecture design, you would probably have a table `Review` defined in the same database that we have already created. When creating a blog post, you would ensure that the `Review` table is also updated as part of the same SQL transactions. This would bring atomicity: if one of the insert in any of the table fails (specially the second one), the whole transaction will be rollbacked and your data kept consistent. 

In a microservice architecture design, different business domain are splitted into different services/APIs, potentially coded by a different development team. The downstream services could be reliable, available, scalable, or maybe not. In case of error, rolling back a request is not as easy than in the context of atomic SQL transaction. With distributed systems, there are plenty of inventive ways for things to go wrong in addition of the application bugs and bad resource management (network latency/congestion, infrastructure), so handling failures should be treated as first class citizen. 

Both approaches have pros and cons. There are plenty of articles on the Internet that compares Monolithic and Microservices architectures, and detailed their characteristics. Often, the microservice approach is favored for large system as the benefits outweight the drawbacks, availability is chosen over consistency. But the adoption requires a shift in the coding experience and beyond, and this should be taken in consideration when choosing an architecture style.

But enough for this side note, I'm taking a microservice approach here for this message-based system crash course, and chose that BlogPost and Review belongs to different domains. The split between BlogPost/Review is rather arbitrary here, in a real project, this would obviously require more thinking. 

## Calling the Review API directly



- [ ] RPC calls
- [ ] Contracts 
- [ ] retries
- [ ] circuit breaker
- [ ] latency/errors

