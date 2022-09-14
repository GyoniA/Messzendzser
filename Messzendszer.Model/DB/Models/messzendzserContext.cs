using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Messzendzser.Model.DB.Models
{
    public partial class messzendzserContext : DbContext
    {
        public messzendzserContext()
        {
        }

        public messzendzserContext(DbContextOptions<messzendzserContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Chatroom> Chatrooms { get; set; } = null!;
        public virtual DbSet<ImageChatMessage> ImageChatMessages { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<TextChatMessage> TextChatMessages { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<VoiceChatMessage> VoiceChatMessages { get; set; } = null!;
        public virtual DbSet<Whiteboard> Whiteboards { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;database=messzendzser;uid=root;pwd=secret", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.14-mariadb"));
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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Role1).HasDefaultValueSql("'user'");
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
