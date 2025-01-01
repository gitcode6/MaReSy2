using MaReSy2.ConsumeModels;
using Newtonsoft.Json.Linq;

namespace MaReSy2.Services
{
    public class UserManager
    {
        private static User angemeldeterUser;

        public static User GetUser()
        {
            return angemeldeterUser;
        }

        // Methode zum Setzen des Tokens
        public static void SetUser(User user)
        {
            angemeldeterUser = user;
        }

    }
}
