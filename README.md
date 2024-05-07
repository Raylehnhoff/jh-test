# Jack Henry Interview

## Description

This is a simple project that uses C# channels to create a data pipeline for Reddit. The pipeline runs entirely in-process using asynchronous Background Workers. 

The pipeline is:

1. Polling
    1. This leverages a timer and fetches data from reddit JSON endpoints
    2. This may not be a well-known thing, but reddit exposes all of its endpoints via a `.json` file extension, that's part of their caching mechanism. Reddit (infamously) changed their API to require OAuth2 tokens and a handshake; this is necessary if you want to take actions against a user account, or allow users to register/login to reddit via your application, but is not necessary to just read post content.
    3. By default, we monitor `/new`, which are all incoming new posts.
    4. This request process goes through Polly, which has a "jittery-backoff" of 1.5s - 3s
2. Ingestion (IngestionWorker.cs)
    1. This part of the pipeline would traditionally be hooked to a Service Bus or other asynchronous message queue. In this implementation, it listens to a channel, and publishes update to the `DataStorage.cs` class, which is a sort of mock database.
    2. I had considered leveraging SQLite or other persistent mechanism, but have had issues with distributing SQLite to unknown environments
    3. The Ingestion process will kick off notifications to Reporting Channels, below
3. Reporting
    1. All reports output to the Console
    1. Two Reporting workers exist:
        1. `UserReportingWorker.cs`
        2. `PostReportingWorker.cs`
    2. I separated these out because I could easily envision wanting reports going to separate destinations
    3. In a Production system, these would typically be separate microservices listening for an Event to push a notification

## Configuration

1. All configuration is stored in `appsettings.json` file in the `Scraper` section
    1. Since we are not leveraging the reddit OAuth flow, no configuration keys are required for this application
    2. I let the system run for several minutes and never encountered a rate limit notification from reddit for hitting these JSON endpoints; I see upvotes coming in in near real-time
2. We will handle listening to multiple subreddits, if they are defined. Top posters are across all subreddits, but top upvote count is per-subreddit
3. Subreddits can be defined as in the example below; I did not test an upper-limit on this.

```json
{
  "Scraper": {
    "UserAgent": "Ray Scraper for an Interview", // can be any non-empty value
    "Subreddits": [ "csharp", "pics" ], // we leverage the multi-reddit capability by concatening subreddits together to browse
    "Sort": "new", //new | top | rising
    "TopNPosts": 5, // how many "top" posts do we want to count
    "TopNUsers": 5 // how many "top" users we want to count
  }
}
```

## Project Layout

1. RedditScraper.Domain
    1. Contains common classes from the Reddit API response
        1. I used QuickType to generate the C# models from JSON -- everything in the `RedditApi` folder was auto-generated
    2. Contains objects that would typically be externally shared in a common nuget file for downstream consumption
2. RedditScraper.Poller
    1. `Workers/` - Contains all of the Background workers that make this system work
    2. `Services - Contains a persistence layer/microservice mock class

## Technologies

1. I used Docker and Visual Studio to create this application
2. I leveraged C# Async Enumerables and Channels pretty heavily. 
3. I leveraged Polly for a retry with a backoff for the communication layer to reddit

## Considerations

### Output out-of-order

Since we're using all the same process and writing to a shared sink (the console), the text can sometimes become out-of-order. This is an unfortunate consequence of the design; a web page output or something not in the console would undoubtedly be a better plan.

### Channels

The usage of Channels here happens where I might typically look to separate a process or microservice boundary. They're a very fast in-process mechanism of passing data around in a loosely coupled way

### Background Services

I leveraged background services in this application to do all of the work; each one represents part of the processing pipeline and tries to not need to know too much about what happens before or after it.

Background Services are a great way to do things like listeners, pollers, and other "background" tasks

### Concurrency, Locking

Since this system leverages Channels and Async Enumerables, and has no external IO, none of the processes block. 

If you "pause" a debugging session and then unpause it, the system will catch up.

This system uses `ConcurrentDictionaries` with a `HashMap` for storing data. I'm doing this for concurrency control, so that we do not double-count posts from users, and we don't "step on ourselves" when doing upvote updates.

### OOM

We're using an Unbound channel for processing the listings (all of the posts we can fetch), which can trigger an Out of Memory Exception if we do not process the Channel. Given this is all running on a single-process, it's not a problem.

In Production, switching to a proper messaging system would prevent this concern, since we would have alerting on a queue and be able to scale-out consuming processes.

### Resiliency

I leveraged Polly for a resiliency retry process. It allows me to specify a specific exception or set of conditions we want to handle. In this project, I handle the `HttpStatusCode` from Reddit, along with a `HttpRequestException` (which is a bit redundant but worth capturing in my experience).

Polly will retry 3 times, backing off on a randomized amount each time until it ultimately fails or succeeds.

The rest of the system relies on Channels which guarantee delivery to listeners in-process. I didn't add resiliency to the message passing in these systems, but in a production system using out-of-process workers (like in a microservice architecture), I would have.

### What If?

* If this weren't something I was doing for an interview, I'd have written the data to an actual database, likely SQL with a concurrency stamp - this would simplify the update process a fair amount over a `ConcurrentDictionary`.
* I'd prefer to not use channels for this kind of pipeline processing. I'd have preferred to leverage a Service Bus or Messaging system, so that we could have near infinite amount of subreddits we listen to, and then "fan out" the ingestion of the data
* The Pipeline itself doesn't have an orchestrator -- if one part of the pipeline crashes, the entire pipeline may eventually fail. There's no recovery
    * An Orchestration layer (like Temporal.io or Azure Durable Functions) would smooth the awkwardness of a crash/recovery out, but those are cloud services and were not available for this
* In a production system, health monitoring and restarting the pod/container (in Kubernetes for example) would allow the system to "wake up" and start processing from the queue again
* I'd have used Reddit.NET or a similar library to deal with the OAuth2 handshake + provide data models. I figured you'd want to see if I knew how to use HttpClients, though, so opted to do without
* I wrote a few tests for the IngestionWorker, which is the worker that has the most business logic. In practice, I'd have tried to fully cover the application, especially the contract/integration points with other services.