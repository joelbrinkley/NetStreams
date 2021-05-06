# NetStreams

A light weight .NET streaming api for Kafka.

## Running the project locally

Clone the repository and execute docker compose up  to standup zookeeper and a broker for testing

```
docker-compose up -d
```

## Create a stream and handle messages

``` .net
  var builder = new NetStreamBuilder(
        cfg =>
        {
            cfg.BootstrapServers = "localhost:9092";
            cfg.ConsumerGroup = "MyConsumer";
        });

   builder.Stream<Null, MyMessage>(sourceTopic)
         .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
         .StartAsync(new CancellationToken());
```

## Filter Messages to be handled


``` .net
  var builder = new NetStreamBuilder(
        cfg =>
        {
            cfg.BootstrapServers = "localhost:9092";
            cfg.ConsumerGroup = "MyConsumer";
        });

  builder.Stream<Null, MyMessage>(sourceTopic)
         .Filter(context => context.Message.Value % 3 == 0)
         .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
         .StartAsync(new CancellationToken());
```


## Use the transform method and write output to kafka topic

This example leverages mediator to dispatch a command and then writes the output to the destination topic

```
var builder = new NetStreamBuilder(
    cfg =>
    {
        cfg.BootstrapServers = "localhost:9092";
        cfg.ConsumerGroup = "Orders.Consumer";
    });

builder.Stream<string, OrderCommand>(sourceTopic)
       .TransformAsync(async context => await mediator.Send(context.Message))
       .ToTopic<string, OrderEvent>("Order.Events", message => message.Key)
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

```  

A delivery mode can be configured to control committing the offset.

For more information regarding delivery modes, check out this [article](https://dzone.com/articles/kafka-clients-at-most-once-at-least-once-exactly-o)

Options:
- At Most Once (Default Behavior if left blank)
- At Least Once
- Custom Delivery Mode

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