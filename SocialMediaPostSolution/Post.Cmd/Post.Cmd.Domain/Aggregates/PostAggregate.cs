using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Messages;
using Microsoft.Extensions.Logging;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool Active { get { return _active; } set { _active = value; } }

        public PostAggregate()
        {
            
        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _author = @event.Author;
            _active = true;
        }

        public void EditMessage(string message) {
            if (!_active)
            {
                throw new InvalidOperationException("You can not edit the message of inactive post!");
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty. Please provide a vali {nameof(message)}!");
            }
            RaiseEvent(new MessageUpdatedEvent
            {
                Id = _id,
                Message = message,
            });
        }

        public void Apply(MessageUpdatedEvent @event) {
            _id = @event.Id;
        }

        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("You can not like an inactive post!");
            }
            RaiseEvent(new PostLikedEvent { Id = _id });
        }

        public void Apply(PostLikedEvent @event) {
            _id = @event.Id;
        }

        public void AddComment(string comment, string username) {
            if (!_active)
            {
                throw new InvalidOperationException("You can not comment on an inactive post!");
            }
            if (string.IsNullOrEmpty(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!");
            }
            if (string.IsNullOrEmpty(username))
            {
                throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}!");
            }
            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                Username = username,
                CommentDate = DateTime.Now,
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
        }

        public void EditComment(Guid commentId, string comment, string username) {
            if (!_active)
            {
                throw new InvalidOperationException("You can not edit comment on an inactive post!");
            }
            if (!_comments.ContainsKey(commentId))
            {
                throw new InvalidOperationException($"No {nameof(comment)} was found with this id: {commentId}!");
            }
            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"You can not edit {nameof(comment)} of another user.");
            }
            if (string.IsNullOrEmpty(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!");
            }
            if (string.IsNullOrEmpty(username))
            {
                throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}!");
            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.Now,
            });
        }

        public void Apply(CommentUpdatedEvent @event) {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string,string>(@event.Comment, @event.Username);
        }

        public void RemoveComment(Guid commentId, string username) {
            if (!_active)
            {
                throw new InvalidOperationException("You can not remove comment on an inactive post!");
            }
            if (!_comments.ContainsKey(commentId))
            {
                throw new InvalidOperationException($"No comment was found with this id: {commentId}!");
            }
            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"You can not remove comment of another user.");
            }

            RaiseEvent(new CommentRemovedEvent {
                Id = _id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent @event) {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        public void DeletePost(string username) {
            if (!_active)
            {
                throw new InvalidOperationException("Post already removed!");
            }
            if(_author != username)
            {
                throw new InvalidOperationException("Can not delete post from different author!");
            }

            RaiseEvent(new PostRemovedEvent {
                Id = _id
            });
        }

        public void Apply(PostRemovedEvent @event) {
            _id = @event.Id;
            _active = false;
        }
    }
}
