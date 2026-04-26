using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models;

public partial class TrainingInstituteDBContext : IdentityDbContext<IdentityUser>
{
    public TrainingInstituteDBContext()
    {
    }

    public TrainingInstituteDBContext(DbContextOptions<TrainingInstituteDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CertificationRequiredCourse> CertificationRequiredCourses { get; set; }

    public virtual DbSet<CertificationStatus> CertificationStatuses { get; set; }

    public virtual DbSet<CertificationTrack> CertificationTracks { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<ClassroomEquipment> ClassroomEquipments { get; set; }

    public virtual DbSet<Coordinator> Coordinators { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseSession> CourseSessions { get; set; }

    public virtual DbSet<CourseSessionStatus> CourseSessionStatuses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<EnrollmentStatus> EnrollmentStatuses { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<InstructorAvailability> InstructorAvailabilities { get; set; }

    public virtual DbSet<InstructorExpertise> InstructorExpertises { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PaymentRecord> PaymentRecords { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<SubjectArea> SubjectAreas { get; set; }

    public virtual DbSet<Trainee> Trainees { get; set; }

    public virtual DbSet<TraineeCertification> TraineeCertifications { get; set; }

    public virtual DbSet<TraineeCourseCompletion> TraineeCourseCompletions { get; set; }

    public virtual DbSet<Waitlist> Waitlists { get; set; }

    public virtual DbSet<WaitlistStatus> WaitlistStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.AssessmentId).HasName("assessmentID");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Assessments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Enrollment_Assesment_fk");

            entity.HasOne(d => d.Instructor).WithMany(p => p.Assessments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Instructor_Assesment_fk");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("categoryID");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory).HasConstraintName("Category_Category_fk");
        });

        modelBuilder.Entity<CertificationRequiredCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("certReqCourseID");

            entity.Property(e => e.IsMandatory).HasDefaultValue(true);

            entity.HasOne(d => d.CertificationTrack).WithMany(p => p.CertificationRequiredCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Certification_Track_Certification_Required_Course_fk");

            entity.HasOne(d => d.Course).WithMany(p => p.CertificationRequiredCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Certification_Required_Course_fk");
        });

        modelBuilder.Entity<CertificationStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("Certification_Status_pk");
        });

        modelBuilder.Entity<CertificationTrack>(entity =>
        {
            entity.HasKey(e => e.CertificationTrackId).HasName("certificationID");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.ClassroomId).HasName("classroomID");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<ClassroomEquipment>(entity =>
        {
            entity.HasKey(e => new { e.EquipmentId, e.ClassroomId }).HasName("equipmentID");

            entity.Property(e => e.EquipmentId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Classroom).WithMany(p => p.ClassroomEquipments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Classroom_Classroom_Equipment_fk");
        });

        modelBuilder.Entity<Coordinator>(entity =>
        {
            entity.HasKey(e => e.CoordinatorId).HasName("coordinatorID");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("courseID");

            entity.Property(e => e.CourseId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Category_Course_fk");

            entity.HasOne(d => d.PrerequisiteCourse).WithMany(p => p.InversePrerequisiteCourse).HasConstraintName("Course_Course_fk");

            entity.HasOne(d => d.SubjectArea).WithMany(p => p.Courses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Subject_Area_fk");
        });

        modelBuilder.Entity<CourseSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("sessionID");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Classroom).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Classroom_Course_Session_fk");

            entity.HasOne(d => d.Coordinator).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Coordinator_Course_Session_fk");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Course_Session_fk");

            entity.HasOne(d => d.Instructor).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Instructor_Course_Session_fk");

            entity.HasOne(d => d.Status).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Session_Status_Course_Session_fk");
        });

        modelBuilder.Entity<CourseSessionStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("Course_Session_Status_pk");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("enrollmentID");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EnrollmentStatus).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Enrollment_Status_Enrollment_fk");

            entity.HasOne(d => d.Session).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Session_Enrollment_fk");

            entity.HasOne(d => d.Trainee).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Trainee_Enrollment_fk");
        });

        modelBuilder.Entity<EnrollmentStatus>(entity =>
        {
            entity.HasKey(e => e.EnrollmentStatusId).HasName("Enrollment_Status_pk");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId).HasName("instructorID");
        });

        modelBuilder.Entity<InstructorAvailability>(entity =>
        {
            entity.HasKey(e => new { e.AvailabilityId, e.InstructorId }).HasName("availabilityID");

            entity.Property(e => e.AvailabilityId).ValueGeneratedOnAdd();
            entity.Property(e => e.IsRecurring).HasDefaultValue(true);

            entity.HasOne(d => d.Instructor).WithMany(p => p.InstructorAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Instructor_Instructor_Availability_fk");
        });

        modelBuilder.Entity<InstructorExpertise>(entity =>
        {
            entity.HasKey(e => e.ExpertiseId).HasName("ID");

            entity.HasOne(d => d.Instructor).WithMany(p => p.InstructorExpertises)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Instructor_Instructor_Expertise_fk");

            entity.HasOne(d => d.SubjectArea).WithMany(p => p.InstructorExpertises)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Subject_Area_Instructor_Expertise_fk");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notificationID");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.HasKey(e => e.PaymentRecordId).HasName("paymentRecordID");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Coordinator).WithMany(p => p.PaymentRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Record_Coordinator_fk");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.PaymentRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Enrollment_Payment_Record_fk");

            entity.HasOne(d => d.Status).WithMany(p => p.PaymentRecords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Status_Payment_Record_fk");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("Payment_Status_pk");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("transactionID");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Coordinator).WithMany(p => p.PaymentTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Coordinator_Payment_Transaction_fk");

            entity.HasOne(d => d.PaymentRecord).WithMany(p => p.PaymentTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Record_Payment_Transaction_fk");
        });

        modelBuilder.Entity<SubjectArea>(entity =>
        {
            entity.HasKey(e => e.SubjectAreaId).HasName("subjectAreaID");
        });

        modelBuilder.Entity<Trainee>(entity =>
        {
            entity.HasKey(e => e.TraineeId).HasName("traineeID");
        });

        modelBuilder.Entity<TraineeCertification>(entity =>
        {
            entity.HasKey(e => e.TraineeCertId).HasName("traineeCertID");

            entity.HasOne(d => d.CertificationTrack).WithMany(p => p.TraineeCertifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Certification_Track_Trainee_Certification_fk");

            entity.HasOne(d => d.Status).WithMany(p => p.TraineeCertifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Certification_Status_Trainee_Certification_fk");

            entity.HasOne(d => d.Trainee).WithMany(p => p.TraineeCertifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Trainee_Trainee_Certification_fk");
        });

        modelBuilder.Entity<TraineeCourseCompletion>(entity =>
        {
            entity.HasKey(e => e.CompletionId).HasName("completionID");

            entity.HasOne(d => d.Course).WithMany(p => p.TraineeCourseCompletions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Trainee_Course_Completion_fk");

            entity.HasOne(d => d.Trainee).WithMany(p => p.TraineeCourseCompletions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Trainee_Trainee_Course_Completion_fk");
        });

        modelBuilder.Entity<Waitlist>(entity =>
        {
            entity.HasKey(e => e.WaitlistId).HasName("waitlistID");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Session).WithMany(p => p.Waitlists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Session_Waitlist_fk");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Waitlists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Waitlist_Status_Waitlist_fk");

            entity.HasOne(d => d.Trainee).WithMany(p => p.Waitlists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Trainee_Waitlist_fk");
        });

        modelBuilder.Entity<WaitlistStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("Waitlist_Status_pk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
