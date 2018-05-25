using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "MRCoreServer";
        public const string AUDIENCE = "http://localhost:50333/";
        const string KEY = "SOME_SECRET_KEYs";
        public const int LIFETIME = 43200;

        public static SymmetricSecurityKey GetKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
