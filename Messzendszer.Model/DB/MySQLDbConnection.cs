using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Messzendzser.Model.DB.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Messzendzser.Model.DB
{
    public partial class MySQLDbConnection : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public override void Dispose()
        {
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }

        public MySQLDbConnection()
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public MySQLDbConnection(DbContextOptions<MySQLDbConnection> options)
            : base(options)
        {            
        }

        public virtual DbSet<Chatroom> Chatrooms { get; set; } = null!;
        public virtual DbSet<ImageChatMessage> ImageChatMessages { get; set; } = null!;
        public virtual DbSet<TextChatMessage> TextChatMessages { get; set; } = null!;
        //public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<VoiceChatMessage> VoiceChatMessages { get; set; } = null!;
        public virtual DbSet<VoipCredential> VoipCredentials { get; set; } = null!;
        public virtual DbSet<Whiteboard> Whiteboards { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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

            modelBuilder.Entity<VoipCredential>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithOne(p => p.VoipCredential)
                    .HasForeignKey<VoipCredential>(d => d.UserId)
                    .HasConstraintName("voip_credentials_user_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
