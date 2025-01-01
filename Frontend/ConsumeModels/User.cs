using System.ComponentModel.DataAnnotations;

namespace MaReSy2.ConsumeModels
{
    public class User
    {

        public int? userId { get; set; }

        public string? username { get; set; }

        public string? firstname { get; set; }

        public string? lastname { get; set; }

        public string? email { get; set; }

        public String? role {  get; set; }

        public String? password { get; set; }

        public String? confirmPassword { get; set; }
        public String? newPassword { get; set; }

        public User()
        {
            
        }

        public User(int Userid, string username, string firstname, string lastname, string email, string role)
        {
            userId = Userid;
            this.username = username;
            this.firstname = firstname;
           this. lastname = lastname;
            this.email = email;
            this.role = role;
        }

        public User(int Userid, string username, string firstname, string lastname, string email, string role, string password)
        {
            userId = Userid;
            this.username = username;
            this.firstname = firstname;
            this.lastname = lastname;
            this.email = email;
            this.role = role;
            this.password = password;
        }
    }
}
