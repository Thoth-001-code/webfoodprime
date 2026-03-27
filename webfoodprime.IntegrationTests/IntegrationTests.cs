using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using webfoodprime.DTOs.Order;
using Xunit;

namespace webfoodprime.IntegrationTests
{
    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public IntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Public_GetFoods_ReturnsList()
        {
            var client = _factory.CreateClient();

            var res = await client.GetAsync("/api/Food");
            res.EnsureSuccessStatusCode();

            var content = await res.Content.ReadAsStringAsync();
            Assert.Contains("Burger", content);
            Assert.Contains("Pizza", content);
        }

        [Fact]
        public async Task Staff_Can_Create_InStore_Order_And_View_Dashboard()
        {
            var client = _factory.CreateClient();

            // set headers to simulate staff user
            client.DefaultRequestHeaders.Add("X-User-Role", "Staff");
            client.DefaultRequestHeaders.Add("X-User-Id", "staff-1");

            var dto = new CreateInStoreOrderDTO
            {
                Items = new System.Collections.Generic.List<InStoreItemDTO>
                {
                    new InStoreItemDTO { FoodId = 1, Quantity = 2 },
                    new InStoreItemDTO { FoodId = 2, Quantity = 1 }
                },
                PaymentMethod = webfoodprime.Helpers.Enum.PaymentMethod.Cash
            };

            var postRes = await client.PostAsJsonAsync("/api/staff/instore", dto);
            postRes.EnsureSuccessStatusCode();

            var dash = await client.GetAsync("/api/staff/dashboard");
            dash.EnsureSuccessStatusCode();

            var body = await dash.Content.ReadAsStringAsync();
            Assert.Contains("pendingOnlineOrders", body);
            Assert.Contains("myOrders", body);
        }
    }
}
