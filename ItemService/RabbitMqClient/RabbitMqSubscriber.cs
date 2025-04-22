using ItemService.EventProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ItemService.RabbitMqClient;

public class RabbitMqSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly string _nomeDaFila = "restaurantes";
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IProcessaEvento _processaEvento;

    public RabbitMqSubscriber(IConfiguration configuration, IProcessaEvento processaEvento)
    {
        _configuration = configuration;
        _connection = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = Int32.Parse(_configuration["RabbitMQPort"])
        }.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        _nomeDaFila = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _nomeDaFila, exchange: "trigger", routingKey: "");

        _processaEvento = processaEvento;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumidor = new EventingBasicConsumer(_channel);

        consumidor.Received += (ModuleHandle, ea) =>
        {
            var body = ea.Body;
            var mensagem = Encoding.UTF8.GetString(body.ToArray());
            _processaEvento.Processa(mensagem);
        };

        try
        {
            _channel.BasicConsume(queue: _nomeDaFila, autoAck: true, consumer: consumidor);
            Console.WriteLine("Mensagem consumida");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao consumir mensagem");
            Console.WriteLine(ex.Message);
        }        

        return Task.CompletedTask;
    }
}
