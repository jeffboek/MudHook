﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.ObjectModel;

namespace MudHook.Core
{            
    public class Comment
    {                
        public int Id { get; set; }
        public int PostId { get; set; }
        public int CommentStatusValue { get; set; }
        public CommentStatus Status
        {
            get { return (CommentStatus)CommentStatusValue; }
            set { CommentStatusValue = (int)value; }
        }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
    }

    public class Meta
    {        
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Page
    {
        public int Id { get; set; }        
        
        public string Slug { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public int PostStatusValue { get; set; }
        public PostStatus Status {
            get { return (PostStatus)PostStatusValue; }
            set { PostStatusValue = (int)value; } 
        }

        public string Redirect { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }        
    }

    public class Post
    {
        public int Id { get; set; }

        public int? TagId { get; set; }
        public virtual Tag Tag { get; set; }
        public IEnumerable<Tag> AvailableTags { get; set; }

        [Required]        
        public string Title { get; set; }        

        public string Slug { get; set; }
        
        [Required]
        public string Description { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Content")]
        public string Html { get; set; }

        [Display(Name = "Custom CSS")]
        public string Css { get; set; }

        [Display(Name = "Custom JS")]
        public string Js { get; set; }

        public string CustomFields { get; set; }

        [Display(Name = "Date")]
        public DateTime Created { get; set; }
                        
        public DateTime? LastModified { get; set; }
        
        public int Author { get; set; }
        
        public int PostStatusValue{ get; set; }        
        public PostStatus Status { 
            get { return (PostStatus) PostStatusValue; }
            set { PostStatusValue = (int)value; }
        }

        [Display(Name = "Allow Comments")]
        public bool CommentsEnabled { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; }        
    }

    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; }

        [DataType(DataType.Password)]   
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public string RealName { get; set; }        
        public string Bio { get; set; }
        [Display(Name = "Status")]
        public int UserStatusValue { get; set; }
        public UserStatus Status
        {
            get { return (UserStatus)UserStatusValue; }
            set { UserStatusValue = (int)value; }
        }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        
        [NotMapped]
        public virtual string OldPassword
        {
            get { return Password; }
        }
    }
    
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum PostStatus
    {
        draft = 1,
        archived = 2,
        published = 3
    }
    public enum CommentStatus
    {
        Pending = 1,
        Published = 2,
        Spam = 3
    }
    public enum UserStatus
    {
        inactive = 1,
        active = 2
    }    
}
