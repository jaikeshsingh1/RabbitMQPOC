using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

class Program
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "HotelBooking", durable: false, exclusive: false, autoDelete: false, arguments: null);

            string message = "Please Book Hotel Taj";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "HotelBooking", basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent {0} :", message);



        }

        Console.WriteLine(" Press [enter] to check hotel booking status.");
        Console.ReadLine();


        ///////////////////////////////////Hotel Book Ack ////////////////////////////
        using (var connection = factory.CreateConnection())
        using (var channel3 = connection.CreateModel())
        {
            channel3.QueueDeclare(queue: "HotelBookingAck", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel3);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine(" [x] Received {0} :", message);
            };
            channel3.BasicConsume(queue: "HotelBookingAck", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to check flight book status.");
            Console.ReadLine();



        }

        ///////////////////////////////////Flight Book Ack ////////////////////////////
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "FlightBookingAck", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine(" [x] Received {0}:", message);
            };
            channel.BasicConsume(queue: "FlightBookingAck", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();



        }
    }
}
