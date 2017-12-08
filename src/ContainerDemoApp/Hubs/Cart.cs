using ContainerDemoApp.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace ContainerDemoApp.Hubs
{
    public class Cart : Hub
    {
        private const string CONN = "Data Source=<k8s-sql-service-IP>,1433;Initial Catalog=snoopyshoppingcart;Persist Security Info=True;User ID=sa;Password=Password1234;";
        public static List<IncomingRequest> ConnectedIPs;

        public async Task Send()
        {
            //await Clients.All.messageReceived(ConnectedIPs.First().ToString());
            await Clients.Client(Context.ConnectionId).messageReceived(GenerateResponse());
        }

        public void Connect(string IP, string CartItem)
        {
            if (ConnectedIPs == null)
                ConnectedIPs = new List<IncomingRequest>();

            if (! string.IsNullOrWhiteSpace(CartItem))
                StoreData(Context.ConnectionId, IP, CartItem);
            //Clients.Caller.getConnectedUsers(ConnectedIPs);
            //Clients.Others.newUserAdded(IP);
        }

        private string GenerateRandomCartItem()
        {
            Random rnd = new Random();
            int scn = rnd.Next(0, 10);

            return ((ShoppingCategoryEnum)scn).ToString();
        }

        private void StoreData(string ConnectioID, string IP, string CartItem)
        {
            //var incomingRequest = new IncomingRequest { AddedOn = DateTime.UtcNow, ConnectionID = ConnectioID, IP = IP, CartItem = CartItem };
            //ConnectedIPs.Add(incomingRequest);

            using (var connection = new SqlConnection(CONN))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"INSERT INTO shopping(AddedOn,ConnectionID,IP,CartItem) VALUES(@addedOn,@connectionID,@ip,@cartItem)";

                    cmd.Parameters.AddWithValue("@addedOn", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@connectionID", ConnectioID);
                    cmd.Parameters.AddWithValue("@ip", IP);
                    cmd.Parameters.AddWithValue("@cartItem", CartItem);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private OutgoingResponse GenerateResponse()
        {
            using (var connection = new SqlConnection(CONN))
            {
                var command = new SqlCommand("SELECT * FROM shopping WHERE AddedOn > DATEADD(s,-2,GETUTCDATE())", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = new IncomingRequest();

                        record.AddedOn = Convert.ToDateTime(reader["AddedOn"]);
                        record.ConnectionID = reader["ConnectionID"].ToString();
                        record.IP = reader["IP"].ToString();
                        record.CartItem = reader["CartItem"].ToString();

                        ConnectedIPs.Add(record);
                    }
                }
            }

            var latestCartItems = ConnectedIPs.GroupBy(x => x.IP).Select(t => t.OrderByDescending(c => c.AddedOn).First()).ToDictionary(x => x.IP, x => x.CartItem);

            return new OutgoingResponse { NewCartItem = GenerateRandomCartItem(), CartItemsOfOthers = latestCartItems };
        }
    }
}
