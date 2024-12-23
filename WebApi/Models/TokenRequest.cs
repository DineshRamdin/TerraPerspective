using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class TokenRequest
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string UserId { get; set; }
    }
}
