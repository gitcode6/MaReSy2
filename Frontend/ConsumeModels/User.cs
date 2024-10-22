using System.ComponentModel.DataAnnotations;

namespace MaReSy2.ConsumeModels
{
    public class User
    {

        public int userId { get; set; }


        [Required]
        public string username { get; set; }

        [Required]
        public string firstname { get; set; }

        [Required]
        public string lastname { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public String role {  get; set; }

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
    }
}
