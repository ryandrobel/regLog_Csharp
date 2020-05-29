using System.ComponentModel.DataAnnotations;


namespace ProjectNameloginReg.Models
{
    public class LogUser
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}