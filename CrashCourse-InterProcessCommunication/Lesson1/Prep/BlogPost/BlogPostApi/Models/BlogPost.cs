using System;
using System.Collections.Generic;

namespace BlogPostApi.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<string> PictureReferences { get; set; }
    } 
}
