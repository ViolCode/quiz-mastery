using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var teacher = new IdentityRole
            {
                Id = "ROLE_TEACHER",
                Name = "teacher",
                NormalizedName = "TEACHER"
            };

            var student = new IdentityRole
            {
                Id = "ROLE_STUDENT", 
                Name = "student",
                NormalizedName = "STUDENT"
            };

            builder.Entity<IdentityRole>().HasData(teacher, student);

            //Define relationships matching your ERD

            //TeacherAssignment ↔ ApplicationUser(Teacher)
            builder.Entity<TeacherAssignment>()
                .HasOne(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            //StudentAssignment ↔ ApplicationUser(Student)
            builder.Entity<StudentAssignment>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            //StudentAssignment ↔ TeacherAssignment
            builder.Entity<StudentAssignment>()
                .HasOne(s => s.Assignment)
                .WithMany(t => t.StudentAssignments)
                .HasForeignKey(s => s.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            //Question ↔ Quiz
            builder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(z => z.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            //Question ↔ Correct Answer
            builder.Entity<Question>()
                .HasOne(q => q.CorrectAnswer)
                .WithMany()
                .HasForeignKey(q => q.CorrectAnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            //Answer ↔ Question
            builder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            //Answer ↔ StudentAssignment
            builder.Entity<Answer>()
                .HasOne(a => a.StudentAssignment)
                .WithMany(sa => sa.Answers)
                .HasForeignKey(a => a.StudentAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
