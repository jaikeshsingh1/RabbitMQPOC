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
            channel.QueueDeclare(queue: "HotelBooking", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine(" [x] Received {0}:", message);
            };
            channel.BasicConsume(queue: "HotelBooking", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to book Indigo flight.");
            Console.ReadLine();

            ///////////////////////////////Hotel Booking Ack ////////////////////

            using (var channel2 = connection.CreateModel())
            {
                channel2.QueueDeclare(queue: "HotelBookingAck", durable: false, exclusive: false, autoDelete: false, arguments: null);

                string message = "Your Hotel Taj booked successfully !";
                var body = Encoding.UTF8.GetBytes(message);

                channel2.BasicPublish(exchange: "", routingKey: "HotelBookingAck", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}:", message);



            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();




            ////////////////////////////////Book Flight //////////////////////


            using (var channel2 = connection.CreateModel())
            {
                channel2.QueueDeclare(queue: "FlightBooking", durable: false, exclusive: false, autoDelete: false, arguments: null);

                string message = "Please Book flight Indigo";
                var body = Encoding.UTF8.GetBytes(message);

                channel2.BasicPublish(exchange: "", routingKey: "FlightBooking", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}:", message);



            }



            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();














        }
    }
}
