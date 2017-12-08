using System.Collections.Generic;

namespace ContainerDemoApp.Models
{
    public class OutgoingResponse
    {
        public string NewCartItem { get; set; }

        public Dictionary<string, string> CartItemsOfOthers { get; set; }
    }
}
