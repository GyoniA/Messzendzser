using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Messzendzser.Model.DB.Models;
using MySqlConnector;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Messzendzser.Model.DB
{
    public partial class MySQLDbConnection : DbContext, IDataSource
    {
        public MySQLDbConnection()
        {
        }

        public MySQLDbConnection(DbContextOptions<MySQLDbConnection> options)
            : base(options)
        {
        }

        protected virtual DbSet<Chatroom> Chatrooms { get; set; } = null!;
        protected virtual DbSet<ImageChatMessage> ImageChatMessages { get; set; } = null!;
        protected virtual DbSet<TextChatMessage> TextChatMessages { get; set; } = null!;
        protected virtual DbSet<User> Users { get; set; } = null!;
        protected virtual DbSet<VoiceChatMessage> VoiceChatMessages { get; set; } = null!;
        protected virtual DbSet<Whiteboard> Whiteboards { get; set; } = null!;

        public void CreateUser(string email, string username, string password)
        {
            try
            {
                this.Database.ExecuteSqlInterpolated($"call messzendzser.register_user({email}, {username}, {password});");
            }
            catch (MySqlException ex)
            {
                switch (ex.SqlState)
                {
                    case "50001":
                        throw new Managers.User.EmailTakenException();
                    case "50002":
                        throw new Managers.User.UsernameTakenException();
                    default:
                        throw;
                }
            }
        }
        /// <summary>
        /// Finds a user in DataSource by searching for their username or email address
        /// </summary>
        /// <param name="username">Username or email of user</param>
        /// <returns>User identified if found, null otherwise</returns>
        public User FindUserByUsernameOrEmail(string username)
        {
            try { 
                User user = Users.Where(u => u.Username==username||u.Email==username).First<User>();
                return user;
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=messzendzser;Uid=root;Pwd=secret;", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.14-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<ImageChatMessage>(entity =>
            {
                entity.HasOne(d => d.Chatroom)
                    .WithMany(p => p.ImageChatMessages)
                    .HasForeignKey(d => d.ChatroomId)
                    .HasConstraintName("image_chat_message_chatroom_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ImageChatMessages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("image_chat_message_user_id");
            });

            modelBuilder.Entity<TextChatMessage>(entity =>
            {
                entity.HasOne(d => d.Chatroom)
                    .WithMany(p => p.TextChatMessages)
                    .HasForeignKey(d => d.ChatroomId)
                    .HasConstraintName("text_chat_message_chatroom_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TextChatMessages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("text_chat_message_user_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(d => d.Chatrooms)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "ChatroomAssociation",
                        l => l.HasOne<Chatroom>().WithMany().HasForeignKey("ChatroomId").HasConstraintName("chatroom_associations_chatroom_id"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").HasConstraintName("chatroom_associations_user_id"),
                        j =>
                        {
                            j.HasKey("UserId", "ChatroomId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("chatroom_associations").HasCharSet("latin1").UseCollation("latin1_swedish_ci");

                            j.HasIndex(new[] { "ChatroomId" }, "chatroom_id_idx");

                            j.HasIndex(new[] { "UserId" }, "user_id_idx");

                            j.IndexerProperty<int>("UserId").HasColumnType("int(11)").HasColumnName("user_id");

                            j.IndexerProperty<int>("ChatroomId").HasColumnType("int(11)").HasColumnName("chatroom_id");
                        });
            });

            modelBuilder.Entity<VoiceChatMessage>(entity =>
            {
                entity.HasOne(d => d.Chatroom)
                    .WithMany(p => p.VoiceChatMessages)
                    .HasForeignKey(d => d.ChatroomId)
                    .HasConstraintName("voice_chat_message_chatroom_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.VoiceChatMessages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("voice_chat_message_user_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
