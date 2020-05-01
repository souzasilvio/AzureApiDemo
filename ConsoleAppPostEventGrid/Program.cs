using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppPostEventGrid
{
    class Program
    {
        static void Main(string[] args)
        {

            string topicEndpoint = "https://egdsadinfo.brazilsouth-1.eventgrid.azure.net/api/events";
            string topicHostname = new Uri(topicEndpoint).Host;
            string sasKey = "vvNouwOKw7HqhzoWHFtH8QlWuXF+NfwzFUqK7BqLVMk=";
            var cred = new TopicCredentials(sasKey);
            var client = new EventGridClient(cred);

            for (int i = 1; i <= 1000; i++)
            {
                var p = new Produto() { Id = Guid.NewGuid(), Nome = $"Produto {i}" , Preco = 200};
                var evento = MontarEvento(p);
                client.PublishEventsAsync(topicHostname, evento).GetAwaiter().GetResult();
                if (i % 20 == 0)
                {
                    Console.WriteLine($"Enviando produto {i} de 1000");
                }
            }
        }

        static EventGridEvent[] MontarEvento(Produto p)
        {
            List<EventGridEvent> eventsList = new List<EventGridEvent>();
            eventsList.Add(new EventGridEvent()
            {

                Id = Guid.NewGuid().ToString(),
                EventType = "produto-insert",
                Data = p,
                EventTime = DateTime.Now,
                Subject = "Produto Criado",
                DataVersion = "1.0",
                Topic = "produto"
            });
            return eventsList.ToArray();
        }
    }
}
