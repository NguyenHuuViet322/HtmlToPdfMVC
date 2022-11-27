using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAB3.Models
{
	public class User
	{
		[Key, Column(Order = 1)]
		[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 3)]
        [Display(Name = "Họ và tên")]
        public string Name { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 3)]
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 3)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

		[NotMapped]
		[Required]
		[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Giá trị không trùng với Mật Khẩu vừa nhập!")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string PasswordConfirms { get; set; }
	}
}
