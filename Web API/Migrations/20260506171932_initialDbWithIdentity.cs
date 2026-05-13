using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class initialDbWithIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    categoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Parent_categoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categoryID", x => x.categoryID);
                    table.ForeignKey(
                        name: "Category_Category_fk",
                        column: x => x.Parent_categoryID,
                        principalTable: "Category",
                        principalColumn: "categoryID");
                });

            migrationBuilder.CreateTable(
                name: "CertificationStatus",
                columns: table => new
                {
                    statusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CertificationStatus_pk", x => x.statusID);
                });

            migrationBuilder.CreateTable(
                name: "CertificationTrack",
                columns: table => new
                {
                    certificationTrackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    validityPeriod = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("certificationID", x => x.certificationTrackID);
                });

            migrationBuilder.CreateTable(
                name: "Classroom",
                columns: table => new
                {
                    classroomID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    building = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    floor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("classroomID", x => x.classroomID);
                });

            migrationBuilder.CreateTable(
                name: "Coordinator",
                columns: table => new
                {
                    coordinatorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("coordinatorID", x => x.coordinatorID);
                });

            migrationBuilder.CreateTable(
                name: "CourseSessionStatus",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CourseSessionStatus_pk", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentStatus",
                columns: table => new
                {
                    enrollmentStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("EnrollmentStatus_pk", x => x.enrollmentStatusID);
                });

            migrationBuilder.CreateTable(
                name: "Instructor",
                columns: table => new
                {
                    instructorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    hireDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("instructorID", x => x.instructorID);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    relatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    isRead = table.Column<bool>(type: "bit", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    readAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notificationID", x => x.notificationID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    paymentMethodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PaymentMethod_pk", x => x.paymentMethodID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatus",
                columns: table => new
                {
                    statusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PaymentStatus_pk", x => x.statusID);
                });

            migrationBuilder.CreateTable(
                name: "SubjectArea",
                columns: table => new
                {
                    subjectAreaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subjectAreaID", x => x.subjectAreaID);
                });

            migrationBuilder.CreateTable(
                name: "Trainee",
                columns: table => new
                {
                    traineeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    dateOfBirth = table.Column<DateTime>(type: "datetime", nullable: false),
                    address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    emergencyContact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("traineeID", x => x.traineeID);
                });

            migrationBuilder.CreateTable(
                name: "WaitlistStatus",
                columns: table => new
                {
                    statusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("WaitlistStatus_pk", x => x.statusID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassroomEquipment",
                columns: table => new
                {
                    equipmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    classroomID = table.Column<int>(type: "int", nullable: false),
                    equipmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("equipmentID", x => new { x.equipmentID, x.classroomID });
                    table.ForeignKey(
                        name: "Classroom_Classroom_Equipment_fk",
                        column: x => x.classroomID,
                        principalTable: "Classroom",
                        principalColumn: "classroomID");
                });

            migrationBuilder.CreateTable(
                name: "InstructorAvailability",
                columns: table => new
                {
                    availabilityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    instructorID = table.Column<int>(type: "int", nullable: false),
                    dayOfWeek = table.Column<int>(type: "int", nullable: false),
                    startTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    endTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    effectiveFrom = table.Column<DateTime>(type: "datetime", nullable: false),
                    effectiveTo = table.Column<DateTime>(type: "datetime", nullable: true),
                    isRecurring = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("availabilityID", x => new { x.availabilityID, x.instructorID });
                    table.ForeignKey(
                        name: "Instructor_Instructor_Availability_fk",
                        column: x => x.instructorID,
                        principalTable: "Instructor",
                        principalColumn: "instructorID");
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    courseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subjectAreaID = table.Column<int>(type: "int", nullable: false),
                    categoryID = table.Column<int>(type: "int", nullable: false),
                    prerequisiteCourseID = table.Column<int>(type: "int", nullable: true),
                    courseCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    durationHours = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    maxCapacity = table.Column<int>(type: "int", nullable: false),
                    enrollmentFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    equipmentRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("courseID", x => x.courseID);
                    table.ForeignKey(
                        name: "Category_Course_fk",
                        column: x => x.categoryID,
                        principalTable: "Category",
                        principalColumn: "categoryID");
                    table.ForeignKey(
                        name: "Course_Course_fk",
                        column: x => x.prerequisiteCourseID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                    table.ForeignKey(
                        name: "Course_Subject_Area_fk",
                        column: x => x.subjectAreaID,
                        principalTable: "SubjectArea",
                        principalColumn: "subjectAreaID");
                });

            migrationBuilder.CreateTable(
                name: "InstructorExpertise",
                columns: table => new
                {
                    expertiseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    instructorID = table.Column<int>(type: "int", nullable: false),
                    subjectAreaID = table.Column<int>(type: "int", nullable: false),
                    proficiencyLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ID", x => x.expertiseID);
                    table.ForeignKey(
                        name: "Instructor_Instructor_Expertise_fk",
                        column: x => x.instructorID,
                        principalTable: "Instructor",
                        principalColumn: "instructorID");
                    table.ForeignKey(
                        name: "Subject_Area_Instructor_Expertise_fk",
                        column: x => x.subjectAreaID,
                        principalTable: "SubjectArea",
                        principalColumn: "subjectAreaID");
                });

            migrationBuilder.CreateTable(
                name: "TraineeCertification",
                columns: table => new
                {
                    traineeCertID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    traineeID = table.Column<int>(type: "int", nullable: false),
                    certificationTrackID = table.Column<int>(type: "int", nullable: false),
                    statusID = table.Column<int>(type: "int", nullable: false),
                    eligibleDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    certificateIssuedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    certificateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    expiryDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("traineeCertID", x => x.traineeCertID);
                    table.ForeignKey(
                        name: "Certification_Status_Trainee_Certification_fk",
                        column: x => x.statusID,
                        principalTable: "CertificationStatus",
                        principalColumn: "statusID");
                    table.ForeignKey(
                        name: "Certification_Track_Trainee_Certification_fk",
                        column: x => x.certificationTrackID,
                        principalTable: "CertificationTrack",
                        principalColumn: "certificationTrackID");
                    table.ForeignKey(
                        name: "Trainee_Trainee_Certification_fk",
                        column: x => x.traineeID,
                        principalTable: "Trainee",
                        principalColumn: "traineeID");
                });

            migrationBuilder.CreateTable(
                name: "CertificationRequiredCourse",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseID = table.Column<int>(type: "int", nullable: false),
                    certificationTrackID = table.Column<int>(type: "int", nullable: false),
                    sequenceOrder = table.Column<int>(type: "int", nullable: true),
                    isMandatory = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("certReqCourseID", x => x.ID);
                    table.ForeignKey(
                        name: "Certification_Track_Certification_Required_Course_fk",
                        column: x => x.certificationTrackID,
                        principalTable: "CertificationTrack",
                        principalColumn: "certificationTrackID");
                    table.ForeignKey(
                        name: "Course_Certification_Required_Course_fk",
                        column: x => x.courseID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                });

            migrationBuilder.CreateTable(
                name: "CourseSession",
                columns: table => new
                {
                    sessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    coordinatorID = table.Column<int>(type: "int", nullable: false),
                    classroomID = table.Column<int>(type: "int", nullable: false),
                    courseID = table.Column<int>(type: "int", nullable: false),
                    instructorID = table.Column<int>(type: "int", nullable: false),
                    StatusID = table.Column<int>(type: "int", nullable: false),
                    sessionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    startTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    endTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    currentEnrollment = table.Column<int>(type: "int", nullable: false),
                    maxCapacity = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sessionID", x => x.sessionID);
                    table.ForeignKey(
                        name: "Classroom_Course_Session_fk",
                        column: x => x.classroomID,
                        principalTable: "Classroom",
                        principalColumn: "classroomID");
                    table.ForeignKey(
                        name: "Coordinator_Course_Session_fk",
                        column: x => x.coordinatorID,
                        principalTable: "Coordinator",
                        principalColumn: "coordinatorID");
                    table.ForeignKey(
                        name: "Course_Course_Session_fk",
                        column: x => x.courseID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                    table.ForeignKey(
                        name: "Course_Session_Status_Course_Session_fk",
                        column: x => x.StatusID,
                        principalTable: "CourseSessionStatus",
                        principalColumn: "StatusID");
                    table.ForeignKey(
                        name: "Instructor_Course_Session_fk",
                        column: x => x.instructorID,
                        principalTable: "Instructor",
                        principalColumn: "instructorID");
                });

            migrationBuilder.CreateTable(
                name: "TraineeCourseCompletion",
                columns: table => new
                {
                    completionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    traineeID = table.Column<int>(type: "int", nullable: false),
                    courseID = table.Column<int>(type: "int", nullable: false),
                    sessionID = table.Column<int>(type: "int", nullable: false),
                    completionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    result = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("completionID", x => x.completionID);
                    table.ForeignKey(
                        name: "Course_Trainee_Course_Completion_fk",
                        column: x => x.courseID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                    table.ForeignKey(
                        name: "Trainee_Trainee_Course_Completion_fk",
                        column: x => x.traineeID,
                        principalTable: "Trainee",
                        principalColumn: "traineeID");
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    enrollmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sessionID = table.Column<int>(type: "int", nullable: false),
                    traineeID = table.Column<int>(type: "int", nullable: false),
                    enrollmentStatusID = table.Column<int>(type: "int", nullable: false),
                    enrollmentDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    statusChangedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    dropReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("enrollmentID", x => x.enrollmentID);
                    table.ForeignKey(
                        name: "Course_Session_Enrollment_fk",
                        column: x => x.sessionID,
                        principalTable: "CourseSession",
                        principalColumn: "sessionID");
                    table.ForeignKey(
                        name: "Enrollment_Status_Enrollment_fk",
                        column: x => x.enrollmentStatusID,
                        principalTable: "EnrollmentStatus",
                        principalColumn: "enrollmentStatusID");
                    table.ForeignKey(
                        name: "Trainee_Enrollment_fk",
                        column: x => x.traineeID,
                        principalTable: "Trainee",
                        principalColumn: "traineeID");
                });

            migrationBuilder.CreateTable(
                name: "Waitlist",
                columns: table => new
                {
                    waitlistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sessionID = table.Column<int>(type: "int", nullable: false),
                    traineeID = table.Column<int>(type: "int", nullable: false),
                    statusID = table.Column<int>(type: "int", nullable: false),
                    position = table.Column<int>(type: "int", nullable: false),
                    addedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    expiresAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("waitlistID", x => x.waitlistID);
                    table.ForeignKey(
                        name: "Course_Session_Waitlist_fk",
                        column: x => x.sessionID,
                        principalTable: "CourseSession",
                        principalColumn: "sessionID");
                    table.ForeignKey(
                        name: "Trainee_Waitlist_fk",
                        column: x => x.traineeID,
                        principalTable: "Trainee",
                        principalColumn: "traineeID");
                    table.ForeignKey(
                        name: "Waitlist_Status_Waitlist_fk",
                        column: x => x.statusID,
                        principalTable: "WaitlistStatus",
                        principalColumn: "statusID");
                });

            migrationBuilder.CreateTable(
                name: "Assessment",
                columns: table => new
                {
                    assessmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    enrollmentID = table.Column<int>(type: "int", nullable: false),
                    instructorID = table.Column<int>(type: "int", nullable: false),
                    result = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    assessmentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("assessmentID", x => x.assessmentID);
                    table.ForeignKey(
                        name: "Enrollment_Assesment_fk",
                        column: x => x.enrollmentID,
                        principalTable: "Enrollment",
                        principalColumn: "enrollmentID");
                    table.ForeignKey(
                        name: "Instructor_Assesment_fk",
                        column: x => x.instructorID,
                        principalTable: "Instructor",
                        principalColumn: "instructorID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentRecord",
                columns: table => new
                {
                    paymentRecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    enrollmentID = table.Column<int>(type: "int", nullable: false),
                    coordinatorID = table.Column<int>(type: "int", nullable: false),
                    statusID = table.Column<int>(type: "int", nullable: false),
                    totalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    dueDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    issuedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("paymentRecordID", x => x.paymentRecordID);
                    table.ForeignKey(
                        name: "Enrollment_Payment_Record_fk",
                        column: x => x.enrollmentID,
                        principalTable: "Enrollment",
                        principalColumn: "enrollmentID");
                    table.ForeignKey(
                        name: "Payment_Record_Coordinator_fk",
                        column: x => x.coordinatorID,
                        principalTable: "Coordinator",
                        principalColumn: "coordinatorID");
                    table.ForeignKey(
                        name: "Payment_Status_Payment_Record_fk",
                        column: x => x.statusID,
                        principalTable: "PaymentStatus",
                        principalColumn: "statusID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransaction",
                columns: table => new
                {
                    transactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paymentRecordID = table.Column<int>(type: "int", nullable: false),
                    coordinatorID = table.Column<int>(type: "int", nullable: false),
                    paymentMethodID = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    paymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    paymentDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("transactionID", x => x.transactionID);
                    table.ForeignKey(
                        name: "Coordinator_Payment_Transaction_fk",
                        column: x => x.coordinatorID,
                        principalTable: "Coordinator",
                        principalColumn: "coordinatorID");
                    table.ForeignKey(
                        name: "PaymentMethod_PaymentTransaction_fk",
                        column: x => x.paymentMethodID,
                        principalTable: "PaymentMethod",
                        principalColumn: "paymentMethodID");
                    table.ForeignKey(
                        name: "Payment_Record_Payment_Transaction_fk",
                        column: x => x.paymentRecordID,
                        principalTable: "PaymentRecord",
                        principalColumn: "paymentRecordID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_enrollmentID",
                table: "Assessment",
                column: "enrollmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_instructorID",
                table: "Assessment",
                column: "instructorID");

            migrationBuilder.CreateIndex(
                name: "IX_Category_Parent_categoryID",
                table: "Category",
                column: "Parent_categoryID");

            migrationBuilder.CreateIndex(
                name: "Certification_Required_Course_unique_comb",
                table: "CertificationRequiredCourse",
                columns: new[] { "courseID", "certificationTrackID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificationRequiredCourse_certificationTrackID",
                table: "CertificationRequiredCourse",
                column: "certificationTrackID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomEquipment_classroomID",
                table: "ClassroomEquipment",
                column: "classroomID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_categoryID",
                table: "Course",
                column: "categoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_prerequisiteCourseID",
                table: "Course",
                column: "prerequisiteCourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_subjectAreaID",
                table: "Course",
                column: "subjectAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSession_classroomID",
                table: "CourseSession",
                column: "classroomID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSession_coordinatorID",
                table: "CourseSession",
                column: "coordinatorID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSession_courseID",
                table: "CourseSession",
                column: "courseID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSession_instructorID",
                table: "CourseSession",
                column: "instructorID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSession_StatusID",
                table: "CourseSession",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_enrollmentStatusID",
                table: "Enrollment",
                column: "enrollmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_sessionID",
                table: "Enrollment",
                column: "sessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_traineeID",
                table: "Enrollment",
                column: "traineeID");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorAvailability_instructorID",
                table: "InstructorAvailability",
                column: "instructorID");

            migrationBuilder.CreateIndex(
                name: "Instructor_Expertise_unique_comb",
                table: "InstructorExpertise",
                columns: new[] { "instructorID", "subjectAreaID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstructorExpertise_subjectAreaID",
                table: "InstructorExpertise",
                column: "subjectAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecord_coordinatorID",
                table: "PaymentRecord",
                column: "coordinatorID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecord_enrollmentID",
                table: "PaymentRecord",
                column: "enrollmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecord_statusID",
                table: "PaymentRecord",
                column: "statusID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_coordinatorID",
                table: "PaymentTransaction",
                column: "coordinatorID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_paymentMethodID",
                table: "PaymentTransaction",
                column: "paymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_paymentRecordID",
                table: "PaymentTransaction",
                column: "paymentRecordID");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeCertification_certificationTrackID",
                table: "TraineeCertification",
                column: "certificationTrackID");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeCertification_statusID",
                table: "TraineeCertification",
                column: "statusID");

            migrationBuilder.CreateIndex(
                name: "Trainee_Certification_unique_comb",
                table: "TraineeCertification",
                columns: new[] { "traineeID", "certificationTrackID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TraineeCourseCompletion_courseID",
                table: "TraineeCourseCompletion",
                column: "courseID");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeCourseCompletion_traineeID",
                table: "TraineeCourseCompletion",
                column: "traineeID");

            migrationBuilder.CreateIndex(
                name: "IX_Waitlist_statusID",
                table: "Waitlist",
                column: "statusID");

            migrationBuilder.CreateIndex(
                name: "IX_Waitlist_traineeID",
                table: "Waitlist",
                column: "traineeID");

            migrationBuilder.CreateIndex(
                name: "Waitlist_unique_comb",
                table: "Waitlist",
                columns: new[] { "sessionID", "traineeID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Assessment");

            migrationBuilder.DropTable(
                name: "CertificationRequiredCourse");

            migrationBuilder.DropTable(
                name: "ClassroomEquipment");

            migrationBuilder.DropTable(
                name: "InstructorAvailability");

            migrationBuilder.DropTable(
                name: "InstructorExpertise");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PaymentTransaction");

            migrationBuilder.DropTable(
                name: "TraineeCertification");

            migrationBuilder.DropTable(
                name: "TraineeCourseCompletion");

            migrationBuilder.DropTable(
                name: "Waitlist");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "PaymentRecord");

            migrationBuilder.DropTable(
                name: "CertificationStatus");

            migrationBuilder.DropTable(
                name: "CertificationTrack");

            migrationBuilder.DropTable(
                name: "WaitlistStatus");

            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "PaymentStatus");

            migrationBuilder.DropTable(
                name: "CourseSession");

            migrationBuilder.DropTable(
                name: "EnrollmentStatus");

            migrationBuilder.DropTable(
                name: "Trainee");

            migrationBuilder.DropTable(
                name: "Classroom");

            migrationBuilder.DropTable(
                name: "Coordinator");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "CourseSessionStatus");

            migrationBuilder.DropTable(
                name: "Instructor");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "SubjectArea");
        }
    }
}
