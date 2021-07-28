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

# Controlling Netstreams on Error

By default Netstreams is set to set to continue on error.  This means that if an exception is encountered when processing a message, Netstreams will log the exception and move on to the next offset. NetStreams does not commit until a successful message is processed.

NetStreams can be configured to *not* continue on error but instead continue to process the the same offset until it is processed succesfully or manually skipped.

```
new NetStreamBuilder<Null, MyMessage>(
    cfg =>
    {
        cfg.BootstrapServers = "localhost:9092";
        cfg.ConsumerGroup = "my-consumer";
        cfg.ContinueOnError = false;
    })
```

# Cluster Authentication
NetStreams supports the following cluster authentication methods:
- PlainText
- SSL
- SaslScram256
- SaslScram512

```
var builder = new NetStreamBuilder<Null, MyMessage>(
    cfg =>
    {
        cfg.UseAuthentication(new PlainTextAuthentication());
        //cfg.UseAuthentication(new SslAuthentication("sslCaCertPath", "sslClientCertPath", "sslClientKeyPath", "sslClientKeyPwd"));
        //cfg.UseAuthentication(new SaslScram256Authentication("username", "password", "sslCaCertPath"));
        //cfg.UseAuthentication(new SaslScram512Authentication("username", "password", "sslCaCertPath"));
        cfg.BootstrapServers = "localhost:9092";
    })
```

# Generating New Certs for Docker Cluster and Testing

In the ``/secrets`` folder you'll find a handful of bash scripts.  These can be used to generate a new round of certs if you need them.

(The certs, keys, credentials, etc. that have been checked-in with this repo aren't in use anywhere.  They are included simply to make  ``git clone . && npm run build`` possible.)

To generate a new batch of secrets:
1. Open a bash prompt.  I use cygwin on Windows, but the scripts also work in Windows Subsystem for Linux.
2. Copy the following files into the ``/src/NetStream.Specs/ssl`` folder:
    * snakeoil-ca-1.crt
    * client.key
    * client.pem
