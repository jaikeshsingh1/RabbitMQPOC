using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Program
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "FlightBooking", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine(" [x] Received {0}:", message);
            };
            channel.BasicConsume(queue: "FlightBooking", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            /////////////////////////////////////Flight Booking Ack//////////////////////////


            using (var channel2 = connection.CreateModel())
            {
                channel2.QueueDeclare(queue: "FlightBookingAck", durable: false, exclusive: false, autoDelete: false, arguments: null);

                string message = "Your flight booked successfully !";
                var body = Encoding.UTF8.GetBytes(message);

                channel2.BasicPublish(exchange: "", routingKey: "FlightBookingAck", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}", message);



            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }
    }
}
