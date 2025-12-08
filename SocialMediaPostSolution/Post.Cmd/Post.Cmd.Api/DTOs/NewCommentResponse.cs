using Post.Common.DTOs;

namespace Post.Cmd.Api.DTOs
{
    public class NewCommentResponse : BaseResponse
    {
        public Guid Id { get; set; }
    }
}
