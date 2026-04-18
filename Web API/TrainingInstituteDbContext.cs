using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Web_API;

public partial class TrainingInstituteDbContext : DbContext
{
    public TrainingInstituteDbContext()
    {
    }

    public TrainingInstituteDbContext(DbContextOptions<TrainingInstituteDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CertificationRequiredCourse> CertificationRequiredCourses { get; set; }

    public virtual DbSet<CertificationTrack> CertificationTracks { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<ClassroomEquipment> ClassroomEquipments { get; set; }

    public virtual DbSet<Coordinator> Coordinators { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseSession> CourseSessions { get; set; }

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

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Waitlist> Waitlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("userID");

            entity.ToTable("AppUser");

            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .HasColumnName("firstName");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.LastName)
                .HasMaxLength(25)
                .HasColumnName("lastName");
            entity.Property(e => e.Password)
                .HasMaxLength(60)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Role).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_Role_AppUser_fk");
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.AssessmentId).HasName("assessmentID");

            entity.ToTable("Assessment");

            entity.Property(e => e.AssessmentId).HasColumnName("assessmentID");
            entity.Property(e => e.AssessmentDate)
                .HasColumnType("datetime")
                .HasColumnName("assessmentDate");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.EnrollmentId).HasColumnName("enrollmentID");
            entity.Property(e => e.InstructorId).HasColumnName("instructorID");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .HasColumnName("remarks");
            entity.Property(e => e.Result)
                .HasMaxLength(10)
                .HasColumnName("result");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Enrollment_Assesment_fk");

            entity.HasOne(d => d.Instructor).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Instructor_Assesment_fk");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("categoryID");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ParentCategoryId).HasColumnName("Parent_categoryID");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("Category_Category_fk");
        });

        modelBuilder.Entity<CertificationRequiredCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("certReqCourseID");

            entity.ToTable("Certification_Required_Course");

            entity.HasIndex(e => new { e.CourseId, e.CertificationId }, "Certification_Required_Course_unique_comb").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CertificationId).HasColumnName("certificationID");
            entity.Property(e => e.CourseId).HasColumnName("courseID");
            entity.Property(e => e.IsMandatory)
                .HasDefaultValue(true)
                .HasColumnName("isMandatory");
            entity.Property(e => e.SequenceOrder).HasColumnName("sequenceOrder");

            entity.HasOne(d => d.Certification).WithMany(p => p.CertificationRequiredCourses)
                .HasForeignKey(d => d.CertificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Certification_Track_Certification_Required_Course_fk");

            entity.HasOne(d => d.Course).WithMany(p => p.CertificationRequiredCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Certification_Required_Course_fk");
        });

        modelBuilder.Entity<CertificationTrack>(entity =>
        {
            entity.HasKey(e => e.CertificationId).HasName("certificationID");

            entity.ToTable("Certification_Track");

            entity.Property(e => e.CertificationId).HasColumnName("certificationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.ValidityPeriod).HasColumnName("validityPeriod");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.ClassroomId).HasName("classroomID");

            entity.ToTable("Classroom");

            entity.Property(e => e.ClassroomId).HasColumnName("classroomID");
            entity.Property(e => e.Building)
                .HasMaxLength(100)
                .HasColumnName("building");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Floor)
                .HasMaxLength(20)
                .HasColumnName("floor");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ClassroomEquipment>(entity =>
        {
            entity.HasKey(e => new { e.EquipmentId, e.ClassroomId }).HasName("equipmentID");

            entity.ToTable("Classroom_Equipment");

            entity.Property(e => e.EquipmentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("equipmentID");
            entity.Property(e => e.ClassroomId).HasColumnName("classroomID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.EquipmentType)
                .HasMaxLength(50)
                .HasColumnName("equipmentType");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Classroom).WithMany(p => p.ClassroomEquipments)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Classroom_Classroom_Equipment_fk");
        });

        modelBuilder.Entity<Coordinator>(entity =>
        {
            entity.HasKey(e => e.CoordinatorId).HasName("coordinatorID");

            entity.ToTable("Coordinator");

            entity.Property(e => e.CoordinatorId).HasColumnName("coordinatorID");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Coordinators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_Coordinator_fk");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("courseID");

            entity.ToTable("Course");

            entity.Property(e => e.CourseId)
                .ValueGeneratedNever()
                .HasColumnName("courseID");
            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(30)
                .HasColumnName("courseCode");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.DurationHours)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("durationHours");
            entity.Property(e => e.EnrollmentFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("enrollmentFee");
            entity.Property(e => e.EquipmentRequirements)
                .HasMaxLength(500)
                .HasColumnName("equipmentRequirements");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.MaxCapacity).HasColumnName("maxCapacity");
            entity.Property(e => e.PrerequisiteCourseId).HasColumnName("prerequisiteCourseID");
            entity.Property(e => e.SubjectAreaId).HasColumnName("subjectAreaID");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Category_Course_fk");

            entity.HasOne(d => d.PrerequisiteCourse).WithMany(p => p.InversePrerequisiteCourse)
                .HasForeignKey(d => d.PrerequisiteCourseId)
                .HasConstraintName("Course_Course_fk");

            entity.HasOne(d => d.SubjectArea).WithMany(p => p.Courses)
                .HasForeignKey(d => d.SubjectAreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Subject_Area_fk");
        });

        modelBuilder.Entity<CourseSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("sessionID");

            entity.ToTable("Course_Session");

            entity.Property(e => e.SessionId).HasColumnName("sessionID");
            entity.Property(e => e.ClassroomId).HasColumnName("classroomID");
            entity.Property(e => e.CoordinatorId).HasColumnName("coordinatorID");
            entity.Property(e => e.CourseId).HasColumnName("courseID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.CurrentEnrollment).HasColumnName("currentEnrollment");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("endTime");
            entity.Property(e => e.InstructorId).HasColumnName("instructorID");
            entity.Property(e => e.MaxCapacity).HasColumnName("maxCapacity");
            entity.Property(e => e.SessionDate)
                .HasColumnType("datetime")
                .HasColumnName("sessionDate");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("startTime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Classroom).WithMany(p => p.CourseSessions)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Classroom_Course_Session_fk");

            entity.HasOne(d => d.Coordinator).WithMany(p => p.CourseSessions)
                .HasForeignKey(d => d.CoordinatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Coordinator_Course_Session_fk");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseSessions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Course_Session_fk");

            entity.HasOne(d => d.Instructor).WithMany(p => p.CourseSessions)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Instructor_Course_Session_fk");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("enrollmentID");

            entity.ToTable("Enrollment");

            entity.Property(e => e.EnrollmentId).HasColumnName("enrollmentID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.DropReason)
                .HasMaxLength(255)
                .HasColumnName("dropReason");
            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("enrollmentDate");
            entity.Property(e => e.EnrollmentStatusId).HasColumnName("enrollmentStatusID");
            entity.Property(e => e.SessionId).HasColumnName("sessionID");
            entity.Property(e => e.StatusChangedAt)
                .HasColumnType("datetime")
                .HasColumnName("statusChangedAt");
            entity.Property(e => e.TraineeId).HasColumnName("traineeID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.EnrollmentStatus).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.EnrollmentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Enrollment_Status_Enrollment_fk");

            entity.HasOne(d => d.Session).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Session_Enrollment_fk");

            entity.HasOne(d => d.Trainee).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.TraineeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Trainee_Enrollment_fk");
        });

        modelBuilder.Entity<EnrollmentStatus>(entity =>
        {
            entity.HasKey(e => e.EnrollmentStatusId).HasName("Enrollment_Status_pk");

            entity.ToTable("Enrollment_Status");

            entity.Property(e => e.EnrollmentStatusId).HasColumnName("enrollmentStatusID");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId).HasName("instructorID");

            entity.ToTable("Instructor");

            entity.Property(e => e.InstructorId).HasColumnName("instructorID");
            entity.Property(e => e.Bio)
                .HasMaxLength(500)
                .HasColumnName("bio");
            entity.Property(e => e.HireDate)
                .HasColumnType("datetime")
                .HasColumnName("hireDate");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Instructors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_Instructor_fk");
        });

        modelBuilder.Entity<InstructorAvailability>(entity =>
        {
            entity.HasKey(e => new { e.AvailabilityId, e.InstructorId }).HasName("availabilityID");

            entity.ToTable("Instructor_Availability");

            entity.Property(e => e.AvailabilityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("availabilityID");
            entity.Property(e => e.InstructorId).HasColumnName("instructorID");
            entity.Property(e => e.DayOfWeek).HasColumnName("dayOfWeek");
            entity.Property(e => e.EffectiveFrom)
                .HasColumnType("datetime")
                .HasColumnName("effectiveFrom");
            entity.Property(e => e.EffectiveTo)
                .HasColumnType("datetime")
                .HasColumnName("effectiveTo");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("endTime");
            entity.Property(e => e.IsRecurring)
                .HasDefaultValue(true)
                .HasColumnName("isRecurring");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("startTime");

            entity.HasOne(d => d.Instructor).WithMany(p => p.InstructorAvailabilities)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Instructor_Instructor_Availability_fk");
        });

        modelBuilder.Entity<InstructorExpertise>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ID");

            entity.ToTable("Instructor_Expertise");

            entity.HasIndex(e => new { e.InstructorId, e.SubjectAreaId }, "Instructor_Expertise_unique_comb").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.InstructorId).HasColumnName("instructorID");
            entity.Property(e => e.ProficiencyLevel)
                .HasMaxLength(20)
                .HasColumnName("proficiencyLevel");
            entity.Property(e => e.SubjectAreaId).HasColumnName("subjectAreaID");

            entity.HasOne(d => d.Instructor).WithMany(p => p.InstructorExpertises)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Instructor_Instructor_Expertise_fk");

            entity.HasOne(d => d.SubjectArea).WithMany(p => p.InstructorExpertises)
                .HasForeignKey(d => d.SubjectAreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Subject_Area_Instructor_Expertise_fk");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notificationID");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notificationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsRead).HasColumnName("isRead");
            entity.Property(e => e.Message)
                .HasMaxLength(1000)
                .HasColumnName("message");
            entity.Property(e => e.ReadAt)
                .HasColumnType("datetime")
                .HasColumnName("readAt");
            entity.Property(e => e.RelatedEntityType)
                .HasMaxLength(50)
                .HasColumnName("relatedEntityType");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_Notification_fk");
        });

        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.HasKey(e => e.PaymentRecordId).HasName("paymentRecordID");

            entity.ToTable("Payment_Record");

            entity.Property(e => e.PaymentRecordId).HasColumnName("paymentRecordID");
            entity.Property(e => e.CoordinatorId).HasColumnName("coordinatorID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("dueDate");
            entity.Property(e => e.EnrollmentId).HasColumnName("enrollmentID");
            entity.Property(e => e.IssuedDate)
                .HasColumnType("datetime")
                .HasColumnName("issuedDate");
            entity.Property(e => e.StatusId).HasColumnName("statusID");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("totalAmount");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Coordinator).WithMany(p => p.PaymentRecords)
                .HasForeignKey(d => d.CoordinatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Record_Coordinator_fk");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.PaymentRecords)
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Enrollment_Payment_Record_fk");

            entity.HasOne(d => d.Status).WithMany(p => p.PaymentRecords)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Status_Payment_Record_fk");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("Payment_Status_pk");

            entity.ToTable("Payment_Status");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("statusID");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("transactionID");

            entity.ToTable("Payment_Transaction");

            entity.Property(e => e.TransactionId).HasColumnName("transactionID");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CoordinatorId).HasColumnName("coordinatorID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Notes)
                .HasMaxLength(300)
                .HasColumnName("notes");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("paymentDate");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .HasColumnName("paymentMethod");
            entity.Property(e => e.PaymentRecordId).HasColumnName("paymentRecordID");

            entity.HasOne(d => d.Coordinator).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.CoordinatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Coordinator_Payment_Transaction_fk");

            entity.HasOne(d => d.PaymentRecord).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.PaymentRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_Record_Payment_Transaction_fk");
        });

        modelBuilder.Entity<SubjectArea>(entity =>
        {
            entity.HasKey(e => e.SubjectAreaId).HasName("subjectAreaID");

            entity.ToTable("Subject_Area");

            entity.Property(e => e.SubjectAreaId).HasColumnName("subjectAreaID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Trainee>(entity =>
        {
            entity.HasKey(e => e.TraineeId).HasName("traineeID");

            entity.ToTable("Trainee");

            entity.Property(e => e.TraineeId).HasColumnName("traineeID");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("address");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("dateOfBirth");
            entity.Property(e => e.EmergencyContact)
                .HasMaxLength(50)
                .HasColumnName("emergencyContact");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Trainees)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_Trainee_fk");
        });

        modelBuilder.Entity<TraineeCertification>(entity =>
        {
            entity.HasKey(e => e.TraineeCertId).HasName("traineeCertID");

            entity.ToTable("Trainee_Certification");

            entity.HasIndex(e => new { e.TraineeId, e.CertificationId }, "Trainee_Certification_unique_comb").IsUnique();

            entity.Property(e => e.TraineeCertId).HasColumnName("traineeCertID");
            entity.Property(e => e.CertificateIssuedDate)
                .HasColumnType("datetime")
                .HasColumnName("certificateIssuedDate");
            entity.Property(e => e.CertificateNumber)
                .HasMaxLength(50)
                .HasColumnName("certificateNumber");
            entity.Property(e => e.CertificationId).HasColumnName("certificationID");
            entity.Property(e => e.EligibleDate)
                .HasColumnType("datetime")
                .HasColumnName("eligibleDate");
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("expiryDate");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TraineeId).HasColumnName("traineeID");

            entity.HasOne(d => d.Certification).WithMany(p => p.TraineeCertifications)
                .HasForeignKey(d => d.CertificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Certification_Track_Trainee_Certification_fk");

            entity.HasOne(d => d.Trainee).WithMany(p => p.TraineeCertifications)
                .HasForeignKey(d => d.TraineeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Trainee_Trainee_Certification_fk");
        });

        modelBuilder.Entity<TraineeCourseCompletion>(entity =>
        {
            entity.HasKey(e => e.CompletionId).HasName("completionID");

            entity.ToTable("Trainee_Course_Completion");

            entity.Property(e => e.CompletionId).HasColumnName("completionID");
            entity.Property(e => e.CompletionDate)
                .HasColumnType("datetime")
                .HasColumnName("completionDate");
            entity.Property(e => e.CourseId).HasColumnName("courseID");
            entity.Property(e => e.Result)
                .HasMaxLength(10)
                .HasColumnName("result");
            entity.Property(e => e.SessionId).HasColumnName("sessionID");
            entity.Property(e => e.TraineeId).HasColumnName("traineeID");

            entity.HasOne(d => d.Course).WithMany(p => p.TraineeCourseCompletions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Trainee_Course_Completion_fk");

            entity.HasOne(d => d.Trainee).WithMany(p => p.TraineeCourseCompletions)
                .HasForeignKey(d => d.TraineeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Trainee_Trainee_Course_Completion_fk");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("User_Role_pk");

            entity.ToTable("User_Role");

            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Waitlist>(entity =>
        {
            entity.HasKey(e => e.WaitlistId).HasName("waitlistID");

            entity.ToTable("Waitlist");

            entity.HasIndex(e => new { e.SessionId, e.TraineeId }, "Waitlist_unique_comb").IsUnique();

            entity.Property(e => e.WaitlistId).HasColumnName("waitlistID");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("addedAt");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expiresAt");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.SessionId).HasColumnName("sessionID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TraineeId).HasColumnName("traineeID");

            entity.HasOne(d => d.Session).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Session_Waitlist_fk");

            entity.HasOne(d => d.Trainee).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.TraineeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Trainee_Waitlist_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
