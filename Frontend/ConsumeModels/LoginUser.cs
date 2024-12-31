namespace MaReSy2.ConsumeModels
{
    public class LoginUser
    {
        public LoginUser(int? userId, string? username, string? password, string? role)
        {
            this.userId = userId;
            this.username = username;
            this.password = password;
            this.role = role;
        }

        public LoginUser()
        {

        }


        public int? userId { get; set; }
        public string? username { get; set; }
        public String? password { get; set; }
        public String? role { get; set; }
    }
}
