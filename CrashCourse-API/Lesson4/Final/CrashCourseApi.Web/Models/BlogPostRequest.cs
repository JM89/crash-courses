using System.ComponentModel.DataAnnotations;

namespace CrashCourseApi.Web.Models
{
    public class BlogPostRequest
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; }
    }
}
