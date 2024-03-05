using Microsoft.AspNetCore.Components.Forms;

namespace WebApiJwtBearerSample.Dtos
{
    public class UserLoginDto
    {

        public string Account { get; set; }


        public string Password { get; set; }

        public string Email { get; set; }
    }
}
