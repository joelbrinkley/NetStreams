# NetStreams

A light weight .NET streaming api for Kafka.

## Running the project locally

Clone the repository and execute docker compose up  to standup zookeeper and a broker for testing

```
docker-compose up -d
```

## Create a stream and handle messages

``` .net
  var stream = new NetStreamBuilder<Null, MyMessage>(
        cfg =>
        {
            cfg.BootstrapServers = "localhost:9092";
            cfg.ConsumerGroup = "MyConsumer";
        })
        .Stream(sourceTopic)
        .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
        .Build()
        .StartAsync(new CancellationToken());
```

## Filter Messages to be handled


``` .net
  var stream = new NetStreamBuilder<Null, MyMessage>(
        cfg =>
        {
            cfg.BootstrapServers = "localhost:9092";
            cfg.ConsumerGroup = "MyConsumer";
        })
        .Stream(sourceTopic)
        .Filter(context => context.Message.Value % 3 == 0)
        .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
        .Build()
        .StartAsync(new CancellationToken());
```


## Use the transform method and write output to kafka topic

This example leverages mediator to dispatch a command and then writes the output to the destination topic

```
var stream = new NetStreamBuilder<string, OrderCommand>(
    cfg =>
    {
        cfg.BootstrapServers = "localhost:9092";
        cfg.ConsumerGroup = "Orders.Consumer";
    })
    .Stream(sourceTopic)
    .TransformAsync(async context => await mediator.Send(context.Message))
    .ToTopic<string, OrderEvent>("Order.Events", message => message.Key)
    .Build()
    .StartAsync(CancellationToken.None);
```


## Configure topics for auto creation

This example leverages mediator to dispatch a command and then writes the output to the destination topic

```
  var builder = new NetStreamBuilder(
        cfg =>
        {
            cfg.BootstrapServers = "localhost:9092";
            cfg.ConsumerGroup = "Orders.Consumer";
            cfg.AddTopicConfiguration(cfg =>
            {
                cfg.Name = "Order.Commands";
                cfg.Partitions = 2;
            });
            cfg.AddTopicConfiguration(cfg =>
            {
                cfg.Name = "Order.Events";
                cfg.Partitions = 2;
            });
        });
```

## Delivery Mode

A delivery mode can be configured to control committing the offset.

For more information regarding delivery modes, check out this [article](https://dzone.com/articles/kafka-clients-at-most-once-at-least-once-exactly-o)

Options:
- At Most Once (Default Behavior if left blank)
- At Least Once
- Custom Delivery Mode

```  
var builder = new NetStreamBuilder(
        cfg =>
        {
            cfg.BootstrapServers = "localhost:9092";
            cfg.ConsumerGroup = "MyConsumer";
            cfg.DeliveryMode = DeliveryMode.At_Least_Once
            //cfg.DeliveryMode = DeliveryMode.At_Most_Once
            //cfg.DeliveryMode = new DeliveryMode(true, 10000);
        });
```

## Custom Pipeline Steps

The pipeline for handling messages is extensible by adding custom pipeline steps.

```
  // extended
  public class CustomPipelineStep<TKey, TMessage> : PipelineStep<TKey,TMessage>
    {
        ...
    }

    // Implement Execute. Send your result on 
    public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result = null)
    {
        // do logic pre next step
        
        var response = await Next.Execute(consumeContext, token, result);

        // do logic post next step 
    }
     ...
```

This example logs application insights telemetry.

```
var stream = new NetStreamBuilder<Null, MyMessage>(
                cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = "AppInsightsExample.Consumer";
                    cfg.PipelineSteps
                        .Add(new ApplicationInsightsPipelineStep<TKey, TMessage>(GetTelemetryClient()));
                })
                .Stream(sourceTopic)
                .Filter(context => context.Message.Value % 3 == 0)
                .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
                .Build()
                .StartAsync(new CancellationToken());
```


# Skipping Malformed Messages

By default, NetStreams will skip a malformed message and log the offset and partition that was skipped.  To turn this feature off, set the skip malformed messages property to false

```
new NetStreamBuilder<Null, MyMessage>(
    cfg =>
    {
        cfg.BootstrapServers = "localhost:9092";
        cfg.ConsumerGroup = "my-consumer";
        cfg.ShouldSkipMalformedMessages = false;
    })
```
