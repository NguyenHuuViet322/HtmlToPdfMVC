using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAB3.Models
{
    public class HistoryItem
    {
        private readonly IHttpContextAccessor _httpAcessor;
		public HistoryItem(string url, DateTime timeAct, string status)
		{
			Url = url;
			TimeAct = timeAct;
			Status = status;
            this.userRefId = 0;
		}

		[Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Url cần chuyển đổi")]
        public string Url { get; set; }

        public DateTime TimeAct { get; set; }
        public string Status { get; set; }

        [ForeignKey("user")]
        public int? userRefId { get; set; }

        public User user { get; set; }
    }
}
