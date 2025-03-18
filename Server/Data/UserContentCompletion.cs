using Server.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class UserContentCompletion
{
    [Key]
    public int UserContentCompletionId { get; set; }

    [ForeignKey("ApiUser")]
    public string UserId { get; set; }
    public virtual ApiUser User { get; set; }

    [ForeignKey("Content")]
    public int ContentId { get; set; }
    public virtual Content Content { get; set; }

    public bool IsCompleted { get; set; } 
}
