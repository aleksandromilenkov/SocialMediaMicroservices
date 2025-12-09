using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Post.Query.Domain.Entities
{
    [Table("Comment")]
    public class CommentEntity
    {
        [Key]
        public Guid CommentId { get; set; }
        public required string Username { get; set; }
        public DateTime CommentDate { get; set; }
        public required string Comment { get; set; }
        public bool Edited { get; set; }
        public Guid PostId { get; set; }
        [JsonIgnore]
        public PostEntity Post { get; set; }
    }
}
