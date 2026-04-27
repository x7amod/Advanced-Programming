using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;

namespace Web_API.Data;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var context = scopedProvider.GetRequiredService<TrainingInstituteDBContext>();
        var userManager = scopedProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        if (await roleManager.Roles.AnyAsync() || await context.SubjectAreas.AnyAsync() || await context.Coordinators.AnyAsync())
        {
            return;
        }

        const string defaultPassword = "Pass@1234";

        static DateTime Dt(int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
            => new(year, month, day, hour, minute, second);

        static string IdentityErrors(IEnumerable<IdentityError> errors)
            => string.Join("; ", errors.Select(e => e.Description));

        async Task<IdentityUser> CreateIdentityUserAsync(
            string firstName,
            string lastName,
            string roleName,
            string phoneNumber)
        {
            var email = $"{firstName}.{lastName}@bticademy.bh".ToLowerInvariant();
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,
                LockoutEnabled = false,
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ConcurrencyStamp = Guid.NewGuid().ToString("D")
            };

            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, defaultPassword);

            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new Exception($"Failed to create user {email}: {IdentityErrors(createResult.Errors)}");
            }

            var roleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                throw new Exception($"Failed to add role '{roleName}' to {email}: {IdentityErrors(roleResult.Errors)}");
            }

            return user;
        }

        var roles = new[] { "Trainee", "Instructor", "Training Coordinator" };
        foreach (var roleName in roles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleResult.Succeeded)
            {
                throw new Exception($"Failed to create role '{roleName}': {IdentityErrors(roleResult.Errors)}");
            }
        }

        var enrollmentStatuses = new List<EnrollmentStatus>
        {
            new() { Status = "Enrolled" },
            new() { Status = "Confirmed" },
            new() { Status = "Attending" },
            new() { Status = "Completed" },
            new() { Status = "Dropped" }
        };
        context.EnrollmentStatuses.AddRange(enrollmentStatuses);
        await context.SaveChangesAsync();
        var enrollmentStatusIds = enrollmentStatuses.ToDictionary(x => x.Status, x => x.EnrollmentStatusId);

        var paymentStatuses = new List<PaymentStatus>
        {
            new() { StatusId = 1, Status = "Unpaid" },
            new() { StatusId = 2, Status = "Partial" },
            new() { StatusId = 3, Status = "Paid" },
            new() { StatusId = 4, Status = "Overdue" }
        };
        context.PaymentStatuses.AddRange(paymentStatuses);
        await context.SaveChangesAsync();
        var paymentStatusIds = paymentStatuses.ToDictionary(x => x.Status, x => x.StatusId);

        var courseSessionStatuses = new List<CourseSessionStatus>
        {
            new() { Status = "Scheduled" },
            new() { Status = "Ongoing" },
            new() { Status = "Completed" },
            new() { Status = "Cancelled" }
        };
        context.CourseSessionStatuses.AddRange(courseSessionStatuses);
        await context.SaveChangesAsync();
        var courseSessionStatusIds = courseSessionStatuses.ToDictionary(x => x.Status, x => x.StatusId);

        var certificationStatuses = new List<CertificationStatus>
        {
            new() { Status = "In Progress" },
            new() { Status = "Eligible" },
            new() { Status = "Issued" },
            new() { Status = "Expired" }
        };
        context.CertificationStatuses.AddRange(certificationStatuses);
        await context.SaveChangesAsync();
        var certificationStatusIds = certificationStatuses.ToDictionary(x => x.Status, x => x.StatusId);

        var waitlistStatuses = new List<WaitlistStatus>
        {
            new() { Status = "Waiting" },
            new() { Status = "Expired" }
        };
        context.WaitlistStatuses.AddRange(waitlistStatuses);
        await context.SaveChangesAsync();
        var waitlistStatusIds = waitlistStatuses.ToDictionary(x => x.Status, x => x.StatusId);

        var subjectAreas = new List<SubjectArea>
        {
            new()
            {
                Name = "Information Technology",
                Description = "Software and systems training pathways."
            },
            new()
            {
                Name = "Business Management",
                Description = "Leadership and business operations development."
            },
            new()
            {
                Name = "Cybersecurity",
                Description = "Defensive security and incident response training."
            },
            new()
            {
                Name = "Data & Analytics",
                Description = "Data handling, analytics, and machine learning foundations."
            }
        };
        context.SubjectAreas.AddRange(subjectAreas);
        await context.SaveChangesAsync();
        var subjectAreaIds = subjectAreas.ToDictionary(x => x.Name, x => x.SubjectAreaId);

        var ictCategory = new Category
        {
            Name = "ICT",
            Description = "Core information and communication technologies."
        };
        var softwareDevelopmentCategory = new Category
        {
            Name = "Software Development",
            Description = "Application design and software implementation.",
            ParentCategory = ictCategory
        };
        var networkingCategory = new Category
        {
            Name = "Networking",
            Description = "Network administration and operations.",
            ParentCategory = ictCategory
        };

        var businessCategory = new Category
        {
            Name = "Business",
            Description = "Core business and organizational capabilities."
        };
        var leadershipCategory = new Category
        {
            Name = "Leadership",
            Description = "Team leadership and management development.",
            ParentCategory = businessCategory
        };
        var financeCategory = new Category
        {
            Name = "Finance",
            Description = "Financial planning and reporting skills.",
            ParentCategory = businessCategory
        };

        var dataScienceCategory = new Category
        {
            Name = "Data Science",
            Description = "Data analysis and insight generation."
        };
        var machineLearningCategory = new Category
        {
            Name = "Machine Learning",
            Description = "Applied ML models and deployment fundamentals.",
            ParentCategory = dataScienceCategory
        };

        var categories = new List<Category>
        {
            ictCategory,
            softwareDevelopmentCategory,
            networkingCategory,
            businessCategory,
            leadershipCategory,
            financeCategory,
            dataScienceCategory,
            machineLearningCategory
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        var categoryIds = categories.ToDictionary(x => x.Name, x => x.CategoryId);

        var certificationTracks = new List<CertificationTrack>
        {
            new()
            {
                Name = "Software Engineering Professional",
                Description = "Builds strong practical software engineering capabilities for enterprise delivery.",
                ValidityPeriod = 24,
                IsActive = true,
                CreatedAt = Dt(2025, 1, 10, 9, 0, 0),
                UpdatedAt = Dt(2025, 1, 10, 9, 0, 0)
            },
            new()
            {
                Name = "Cyber Defense Specialist",
                Description = "Covers foundational and intermediate cyber defense operations.",
                ValidityPeriod = 24,
                IsActive = true,
                CreatedAt = Dt(2025, 1, 10, 9, 15, 0),
                UpdatedAt = Dt(2025, 1, 10, 9, 15, 0)
            },
            new()
            {
                Name = "Data Analytics Practitioner",
                Description = "Focuses on analytics workflows and applied data techniques.",
                ValidityPeriod = 18,
                IsActive = true,
                CreatedAt = Dt(2025, 1, 10, 9, 30, 0),
                UpdatedAt = Dt(2025, 1, 10, 9, 30, 0)
            }
        };
        context.CertificationTracks.AddRange(certificationTracks);
        await context.SaveChangesAsync();
        var certificationTrackIds = certificationTracks.ToDictionary(x => x.Name, x => x.CertificationTrackId);

        var classrooms = new List<Classroom>
        {
            new()
            {
                Name = "Pearl Lab 1",
                Location = "Seef District, Manama",
                Building = "Building 36",
                Floor = "Ground",
                Capacity = 12,
                IsActive = true
            },
            new()
            {
                Name = "Seef Training Room",
                Location = "Seef District, Manama",
                Building = "Building 12",
                Floor = "1",
                Capacity = 24,
                IsActive = true
            },
            new()
            {
                Name = "Riffa Seminar Hall",
                Location = "East Riffa",
                Building = "Building 21",
                Floor = "2",
                Capacity = 28,
                IsActive = true
            },
            new()
            {
                Name = "Muharraq Innovation Lab",
                Location = "Muharraq",
                Building = "Building 44",
                Floor = "1",
                Capacity = 36,
                IsActive = true
            },
            new()
            {
                Name = "Isa Town Auditorium",
                Location = "Isa Town",
                Building = "Building 58",
                Floor = "3",
                Capacity = 45,
                IsActive = true
            }
        };
        context.Classrooms.AddRange(classrooms);
        await context.SaveChangesAsync();
        var classroomIds = classrooms.ToDictionary(x => x.Name, x => x.ClassroomId);

        var baseCourses = new List<Course>
        {
            new()
            {
                CourseId = 1,
                SubjectAreaId = subjectAreaIds["Information Technology"],
                CategoryId = categoryIds["Software Development"],
                CourseCode = "ITC101",
                Title = "C# Fundamentals",
                Description = "Core C# syntax, object-oriented design, and clean coding fundamentals.",
                DurationHours = 24.00m,
                MaxCapacity = 24,
                EnrollmentFee = 180.00m,
                EquipmentRequirements = "Lab Computer, Projector",
                IsActive = true,
                CreatedAt = Dt(2025, 11, 1, 9, 0, 0),
                UpdatedAt = Dt(2025, 11, 5, 9, 0, 0)
            },
            new()
            {
                CourseId = 2,
                SubjectAreaId = subjectAreaIds["Cybersecurity"],
                CategoryId = categoryIds["Networking"],
                CourseCode = "CYB120",
                Title = "Network Essentials",
                Description = "Practical networking fundamentals for secure enterprise operations.",
                DurationHours = 16.00m,
                MaxCapacity = 20,
                EnrollmentFee = 150.00m,
                EquipmentRequirements = "Lab Computer, Smart Board",
                IsActive = true,
                CreatedAt = Dt(2025, 11, 1, 9, 20, 0),
                UpdatedAt = Dt(2025, 11, 6, 9, 20, 0)
            },
            new()
            {
                CourseId = 3,
                SubjectAreaId = subjectAreaIds["Business Management"],
                CategoryId = categoryIds["Leadership"],
                CourseCode = "BMG110",
                Title = "Project Management Basics",
                Description = "Planning, tracking, and communication practices for project delivery.",
                DurationHours = 16.00m,
                MaxCapacity = 22,
                EnrollmentFee = 120.00m,
                EquipmentRequirements = null,
                IsActive = true,
                CreatedAt = Dt(2025, 11, 2, 10, 0, 0),
                UpdatedAt = Dt(2025, 11, 8, 10, 0, 0)
            },
            new()
            {
                CourseId = 4,
                SubjectAreaId = subjectAreaIds["Data & Analytics"],
                CategoryId = categoryIds["Data Science"],
                CourseCode = "DAT130",
                Title = "Data Analysis with Excel",
                Description = "Data cleaning, dashboarding, and reporting workflows in Excel.",
                DurationHours = 16.00m,
                MaxCapacity = 20,
                EnrollmentFee = 140.00m,
                EquipmentRequirements = "Lab Computer, Projector",
                IsActive = true,
                CreatedAt = Dt(2025, 11, 2, 10, 30, 0),
                UpdatedAt = Dt(2025, 11, 8, 10, 30, 0)
            },
            new()
            {
                CourseId = 5,
                SubjectAreaId = subjectAreaIds["Cybersecurity"],
                CategoryId = categoryIds["Networking"],
                CourseCode = "CYB140",
                Title = "Cybersecurity Foundations",
                Description = "Threat landscape, defensive controls, and incident lifecycle fundamentals.",
                DurationHours = 24.00m,
                MaxCapacity = 25,
                EnrollmentFee = 210.00m,
                EquipmentRequirements = "Lab Computer, Video Conferencing System",
                IsActive = true,
                CreatedAt = Dt(2025, 11, 3, 11, 0, 0),
                UpdatedAt = Dt(2025, 11, 10, 11, 0, 0)
            }
        };

        var advancedCourses = new List<Course>
        {
            new()
            {
                CourseId = 6,
                SubjectAreaId = subjectAreaIds["Information Technology"],
                CategoryId = categoryIds["Software Development"],
                PrerequisiteCourseId = 1,
                CourseCode = "ITC220",
                Title = "Advanced C# Development",
                Description = "Intermediate to advanced C# architecture, patterns, and maintainability practices.",
                DurationHours = 24.00m,
                MaxCapacity = 18,
                EnrollmentFee = 260.00m,
                EquipmentRequirements = "Lab Computer, Smart Board",
                IsActive = true,
                CreatedAt = Dt(2025, 11, 5, 12, 0, 0),
                UpdatedAt = Dt(2025, 11, 12, 12, 0, 0)
            },
            new()
            {
                CourseId = 7,
                SubjectAreaId = subjectAreaIds["Cybersecurity"],
                CategoryId = categoryIds["Networking"],
                PrerequisiteCourseId = 2,
                CourseCode = "CYB240",
                Title = "Secure Network Operations",
                Description = "Hardening, monitoring, and secure operations for enterprise networks.",
                DurationHours = 24.00m,
                MaxCapacity = 18,
                EnrollmentFee = 280.00m,
                EquipmentRequirements = "Lab Computer, Projector",
                IsActive = true,
                CreatedAt = Dt(2025, 11, 5, 12, 30, 0),
                UpdatedAt = Dt(2025, 11, 13, 12, 30, 0)
            },
            new()
            {
                CourseId = 8,
                SubjectAreaId = subjectAreaIds["Business Management"],
                CategoryId = categoryIds["Leadership"],
                PrerequisiteCourseId = 3,
                CourseCode = "BMG230",
                Title = "Leadership for Technical Teams",
                Description = "People leadership, decision making, and team alignment for technical units.",
                DurationHours = 12.00m,
                MaxCapacity = 20,
                EnrollmentFee = 170.00m,
                EquipmentRequirements = null,
                IsActive = true,
                CreatedAt = Dt(2025, 11, 6, 13, 0, 0),
                UpdatedAt = Dt(2025, 11, 13, 13, 0, 0)
            },
            new()
            {
                CourseId = 9,
                SubjectAreaId = subjectAreaIds["Data & Analytics"],
                CategoryId = categoryIds["Machine Learning"],
                PrerequisiteCourseId = 4,
                CourseCode = "DAT260",
                Title = "Applied Machine Learning",
                Description = "Model training, evaluation, and deployment-ready ML workflows.",
                DurationHours = 30.00m,
                MaxCapacity = 16,
                EnrollmentFee = 320.00m,
                EquipmentRequirements = "Lab Computer, Smart Board",
                IsActive = true,
                CreatedAt = Dt(2025, 11, 6, 13, 30, 0),
                UpdatedAt = Dt(2025, 11, 14, 13, 30, 0)
            },
            new()
            {
                CourseId = 10,
                SubjectAreaId = subjectAreaIds["Cybersecurity"],
                CategoryId = categoryIds["Networking"],
                PrerequisiteCourseId = 5,
                CourseCode = "CYB280",
                Title = "SIEM and Incident Response",
                Description = "Security monitoring, triage, and coordinated incident handling practices.",
                DurationHours = 20.00m,
                MaxCapacity = 18,
                EnrollmentFee = 300.00m,
                EquipmentRequirements = "Lab Computer, Video Conferencing System",
                IsActive = true,
                CreatedAt = Dt(2025, 11, 7, 14, 0, 0),
                UpdatedAt = Dt(2025, 11, 14, 14, 0, 0)
            }
        };

        context.Courses.AddRange(baseCourses);
        context.Courses.AddRange(advancedCourses);
        await context.SaveChangesAsync();
        var allCourses = baseCourses.Concat(advancedCourses).ToDictionary(x => x.CourseId, x => x);

        var classroomEquipments = new List<ClassroomEquipment>
        {
            new()
            {
                ClassroomId = classroomIds["Pearl Lab 1"],
                EquipmentType = "Projector",
                Quantity = 1,
                Description = "Ceiling-mounted projector"
            },
            new()
            {
                ClassroomId = classroomIds["Pearl Lab 1"],
                EquipmentType = "Whiteboard",
                Quantity = 2,
                Description = "Magnetic whiteboard panels"
            },
            new()
            {
                ClassroomId = classroomIds["Seef Training Room"],
                EquipmentType = "Lab Computer",
                Quantity = 24,
                Description = "Desktop workstations"
            },
            new()
            {
                ClassroomId = classroomIds["Seef Training Room"],
                EquipmentType = "Smart Board",
                Quantity = 1,
                Description = "Interactive smart board"
            },
            new()
            {
                ClassroomId = classroomIds["Seef Training Room"],
                EquipmentType = "Whiteboard",
                Quantity = 1,
                Description = "Standard planning board"
            },
            new()
            {
                ClassroomId = classroomIds["Riffa Seminar Hall"],
                EquipmentType = "Projector",
                Quantity = 1,
                Description = "High-lumen conference projector"
            },
            new()
            {
                ClassroomId = classroomIds["Riffa Seminar Hall"],
                EquipmentType = "Video Conferencing System",
                Quantity = 1,
                Description = "Hybrid classroom conferencing unit"
            },
            new()
            {
                ClassroomId = classroomIds["Muharraq Innovation Lab"],
                EquipmentType = "Lab Computer",
                Quantity = 30,
                Description = "Networked lab computers"
            },
            new()
            {
                ClassroomId = classroomIds["Muharraq Innovation Lab"],
                EquipmentType = "Video Conferencing System",
                Quantity = 1,
                Description = "Remote speaker integration setup"
            },
            new()
            {
                ClassroomId = classroomIds["Muharraq Innovation Lab"],
                EquipmentType = "Smart Board",
                Quantity = 1,
                Description = "Touch-enabled instruction board"
            },
            new()
            {
                ClassroomId = classroomIds["Isa Town Auditorium"],
                EquipmentType = "Projector",
                Quantity = 2,
                Description = "Dual-projector auditorium setup"
            },
            new()
            {
                ClassroomId = classroomIds["Isa Town Auditorium"],
                EquipmentType = "Whiteboard",
                Quantity = 2,
                Description = "Large-format whiteboards"
            }
        };
        context.ClassroomEquipments.AddRange(classroomEquipments);
        await context.SaveChangesAsync();

        var certificationRequiredCourses = new List<CertificationRequiredCourse>
        {
            new()
            {
                CertificationTrackId = certificationTrackIds["Software Engineering Professional"],
                CourseId = 1,
                SequenceOrder = 1,
                IsMandatory = true
            },
            new()
            {
                CertificationTrackId = certificationTrackIds["Software Engineering Professional"],
                CourseId = 3,
                SequenceOrder = 2,
                IsMandatory = true
            },
            new()
            {
                CertificationTrackId = certificationTrackIds["Software Engineering Professional"],
                CourseId = 6,
                SequenceOrder = 3,
                IsMandatory = false
            },
            new()
            {
                CertificationTrackId = certificationTrackIds["Cyber Defense Specialist"],
                CourseId = 2,
                SequenceOrder = 1,
                IsMandatory = true
            },
            new()
            {
                CertificationTrackId = certificationTrackIds["Cyber Defense Specialist"],
                CourseId = 5,
                SequenceOrder = 2,
                IsMandatory = true
            },
            new()
            {
                CertificationTrackId = certificationTrackIds["Cyber Defense Specialist"],
                CourseId = 7,
                SequenceOrder = 3,
                IsMandatory = false
            },
            new()
            {
                CertificationTrackId = certificationTrackIds["Data Analytics Practitioner"],
                CourseId = 4,
                SequenceOrder = 1,
                IsMandatory = true
            },
            new()
            {
                CertificationTrackId = certificationTrackIds["Data Analytics Practitioner"],
                CourseId = 1,
                SequenceOrder = 2,
                IsMandatory = true
            },
            new()
            {
                CertificationTrackId = certificationTrackIds["Data Analytics Practitioner"],
                CourseId = 9,
                SequenceOrder = 3,
                IsMandatory = false
            }
        };
        context.CertificationRequiredCourses.AddRange(certificationRequiredCourses);
        await context.SaveChangesAsync();

        var coordinatorUsers = new Dictionary<string, IdentityUser>();

        var coordinatorNoura = await CreateIdentityUserAsync("noura", "aldosari", "Training Coordinator", "+973 3600 1201");
        coordinatorUsers["noura"] = coordinatorNoura;

        var coordinatorYousif = await CreateIdentityUserAsync("yousif", "janahi", "Training Coordinator", "+973 3600 1202");
        coordinatorUsers["yousif"] = coordinatorYousif;

        var coordinators = new List<Coordinator>
        {
            new()
            {
                UserId = coordinatorNoura.Id,
                Department = "Operations"
            },
            new()
            {
                UserId = coordinatorYousif.Id,
                Department = "Academic Affairs"
            }
        };
        context.Coordinators.AddRange(coordinators);
        await context.SaveChangesAsync();
        var coordinatorIds = new Dictionary<string, int>
        {
            ["noura"] = coordinators[0].CoordinatorId,
            ["yousif"] = coordinators[1].CoordinatorId
        };

        var instructorUsers = new Dictionary<string, IdentityUser>();

        var instructorMohammed = await CreateIdentityUserAsync("mohammed", "alsayed", "Instructor", "+973 3600 2201");
        instructorUsers["mohammed"] = instructorMohammed;

        var instructorFatima = await CreateIdentityUserAsync("fatima", "almansoor", "Instructor", "+973 3600 2202");
        instructorUsers["fatima"] = instructorFatima;

        var instructorAhmed = await CreateIdentityUserAsync("ahmed", "alkhalifa", "Instructor", "+973 3600 2203");
        instructorUsers["ahmed"] = instructorAhmed;

        var instructorSara = await CreateIdentityUserAsync("sara", "alhaddad", "Instructor", "+973 3600 2204");
        instructorUsers["sara"] = instructorSara;

        var instructors = new List<Instructor>
        {
            new()
            {
                UserId = instructorMohammed.Id,
                HireDate = Dt(2021, 5, 3, 9, 0, 0),
                Bio = "Mohammed Al-Sayed is a senior software trainer with extensive enterprise development experience. He leads hands-on labs and mentoring sessions focused on architecture and maintainability."
            },
            new()
            {
                UserId = instructorFatima.Id,
                HireDate = Dt(2022, 2, 14, 9, 0, 0),
                Bio = "Fatima Al-Mansoor specializes in secure network operations and cybersecurity training. She supports learners with practical exercises that align to real incident scenarios."
            },
            new()
            {
                UserId = instructorAhmed.Id,
                HireDate = Dt(2021, 9, 20, 9, 0, 0),
                Bio = "Ahmed Al-Khalifa delivers business and analytics programs for multidisciplinary teams. He combines project delivery frameworks with data-informed decision making."
            },
            new()
            {
                UserId = instructorSara.Id,
                HireDate = Dt(2020, 11, 10, 9, 0, 0),
                Bio = "Sara Al-Haddad focuses on cyber defense and operational leadership in technical environments. Her sessions emphasize clear procedures and effective escalation practices."
            }
        };
        context.Instructors.AddRange(instructors);
        await context.SaveChangesAsync();
        var instructorIds = new Dictionary<string, int>
        {
            ["mohammed"] = instructors[0].InstructorId,
            ["fatima"] = instructors[1].InstructorId,
            ["ahmed"] = instructors[2].InstructorId,
            ["sara"] = instructors[3].InstructorId
        };

        var traineeUsers = new Dictionary<string, IdentityUser>();

        var traineeAli = await CreateIdentityUserAsync("ali", "almutawa", "Trainee", "+973 3600 3201");
        traineeUsers["ali"] = traineeAli;

        var traineeMariam = await CreateIdentityUserAsync("mariam", "alkooheji", "Trainee", "+973 3600 3202");
        traineeUsers["mariam"] = traineeMariam;

        var traineeHassan = await CreateIdentityUserAsync("hassan", "alnajjar", "Trainee", "+973 3600 3203");
        traineeUsers["hassan"] = traineeHassan;

        var traineeNoor = await CreateIdentityUserAsync("noor", "alkhaldi", "Trainee", "+973 3600 3204");
        traineeUsers["noor"] = traineeNoor;

        var traineeAbdullah = await CreateIdentityUserAsync("abdullah", "alsuwaidi", "Trainee", "+973 3600 3205");
        traineeUsers["abdullah"] = traineeAbdullah;

        var traineeZainab = await CreateIdentityUserAsync("zainab", "almahmood", "Trainee", "+973 3600 3206");
        traineeUsers["zainab"] = traineeZainab;

        var traineeKhalid = await CreateIdentityUserAsync("khalid", "alrumaihi", "Trainee", "+973 3600 3207");
        traineeUsers["khalid"] = traineeKhalid;

        var traineeHuda = await CreateIdentityUserAsync("huda", "alansari", "Trainee", "+973 3600 3208");
        traineeUsers["huda"] = traineeHuda;

        var trainees = new List<Trainee>
        {
            new()
            {
                UserId = traineeAli.Id,
                DateOfBirth = Dt(1998, 3, 12),
                Address = "Villa 14, Road 22, Block 320, Riffa",
                EmergencyContact = "+973 3900 1201"
            },
            new()
            {
                UserId = traineeMariam.Id,
                DateOfBirth = Dt(1999, 7, 22),
                Address = "House 88, Road 9, Block 428, Seef",
                EmergencyContact = "+973 3900 1202"
            },
            new()
            {
                UserId = traineeHassan.Id,
                DateOfBirth = Dt(1997, 11, 4),
                Address = "Flat 12, Road 31, Block 711, Sitra",
                EmergencyContact = "+973 3900 1203"
            },
            new()
            {
                UserId = traineeNoor.Id,
                DateOfBirth = Dt(2000, 2, 18),
                Address = "Villa 5, Road 14, Block 915, Hamad Town",
                EmergencyContact = "+973 3900 1204"
            },
            new()
            {
                UserId = traineeAbdullah.Id,
                DateOfBirth = Dt(1995, 8, 9),
                Address = "House 27, Road 45, Block 204, Muharraq",
                EmergencyContact = "+973 3900 1205"
            },
            new()
            {
                UserId = traineeZainab.Id,
                DateOfBirth = Dt(1996, 5, 27),
                Address = "Flat 7, Road 11, Block 815, Isa Town",
                EmergencyContact = "+973 3900 1206"
            },
            new()
            {
                UserId = traineeKhalid.Id,
                DateOfBirth = Dt(1994, 10, 2),
                Address = "Villa 41, Road 6, Block 540, Budaiya",
                EmergencyContact = "+973 3900 1207"
            },
            new()
            {
                UserId = traineeHuda.Id,
                DateOfBirth = Dt(2001, 1, 15),
                Address = "House 16, Road 8, Block 216, Manama",
                EmergencyContact = "+973 3900 1208"
            }
        };
        context.Trainees.AddRange(trainees);
        await context.SaveChangesAsync();
        var traineeIds = new Dictionary<string, int>
        {
            ["ali"] = trainees[0].TraineeId,
            ["mariam"] = trainees[1].TraineeId,
            ["hassan"] = trainees[2].TraineeId,
            ["noor"] = trainees[3].TraineeId,
            ["abdullah"] = trainees[4].TraineeId,
            ["zainab"] = trainees[5].TraineeId,
            ["khalid"] = trainees[6].TraineeId,
            ["huda"] = trainees[7].TraineeId
        };

        var instructorExpertise = new List<InstructorExpertise>
        {
            new()
            {
                InstructorId = instructorIds["mohammed"],
                SubjectAreaId = subjectAreaIds["Information Technology"],
                ProficiencyLevel = "Expert"
            },
            new()
            {
                InstructorId = instructorIds["mohammed"],
                SubjectAreaId = subjectAreaIds["Data & Analytics"],
                ProficiencyLevel = "Intermediate"
            },
            new()
            {
                InstructorId = instructorIds["fatima"],
                SubjectAreaId = subjectAreaIds["Cybersecurity"],
                ProficiencyLevel = "Expert"
            },
            new()
            {
                InstructorId = instructorIds["fatima"],
                SubjectAreaId = subjectAreaIds["Information Technology"],
                ProficiencyLevel = "Intermediate"
            },
            new()
            {
                InstructorId = instructorIds["ahmed"],
                SubjectAreaId = subjectAreaIds["Business Management"],
                ProficiencyLevel = "Expert"
            },
            new()
            {
                InstructorId = instructorIds["ahmed"],
                SubjectAreaId = subjectAreaIds["Data & Analytics"],
                ProficiencyLevel = "Intermediate"
            },
            new()
            {
                InstructorId = instructorIds["sara"],
                SubjectAreaId = subjectAreaIds["Cybersecurity"],
                ProficiencyLevel = "Intermediate"
            },
            new()
            {
                InstructorId = instructorIds["sara"],
                SubjectAreaId = subjectAreaIds["Business Management"],
                ProficiencyLevel = "Expert"
            }
        };
        context.InstructorExpertises.AddRange(instructorExpertise);
        await context.SaveChangesAsync();

        var instructorAvailabilities = new List<InstructorAvailability>
        {
            new()
            {
                InstructorId = instructorIds["mohammed"],
                DayOfWeek = 1,
                StartTime = Dt(1900, 1, 1, 8, 0, 0),
                EndTime = Dt(1900, 1, 1, 12, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["mohammed"],
                DayOfWeek = 3,
                StartTime = Dt(1900, 1, 1, 9, 0, 0),
                EndTime = Dt(1900, 1, 1, 13, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["mohammed"],
                DayOfWeek = 5,
                StartTime = Dt(1900, 1, 1, 10, 0, 0),
                EndTime = Dt(1900, 1, 1, 14, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["fatima"],
                DayOfWeek = 1,
                StartTime = Dt(1900, 1, 1, 8, 30, 0),
                EndTime = Dt(1900, 1, 1, 12, 30, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["fatima"],
                DayOfWeek = 2,
                StartTime = Dt(1900, 1, 1, 9, 0, 0),
                EndTime = Dt(1900, 1, 1, 13, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["fatima"],
                DayOfWeek = 4,
                StartTime = Dt(1900, 1, 1, 10, 0, 0),
                EndTime = Dt(1900, 1, 1, 14, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["ahmed"],
                DayOfWeek = 1,
                StartTime = Dt(1900, 1, 1, 8, 0, 0),
                EndTime = Dt(1900, 1, 1, 11, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["ahmed"],
                DayOfWeek = 3,
                StartTime = Dt(1900, 1, 1, 9, 30, 0),
                EndTime = Dt(1900, 1, 1, 12, 30, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["ahmed"],
                DayOfWeek = 5,
                StartTime = Dt(1900, 1, 1, 10, 30, 0),
                EndTime = Dt(1900, 1, 1, 13, 30, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["sara"],
                DayOfWeek = 2,
                StartTime = Dt(1900, 1, 1, 8, 0, 0),
                EndTime = Dt(1900, 1, 1, 12, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["sara"],
                DayOfWeek = 4,
                StartTime = Dt(1900, 1, 1, 9, 0, 0),
                EndTime = Dt(1900, 1, 1, 13, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            },
            new()
            {
                InstructorId = instructorIds["sara"],
                DayOfWeek = 5,
                StartTime = Dt(1900, 1, 1, 11, 0, 0),
                EndTime = Dt(1900, 1, 1, 15, 0, 0),
                EffectiveFrom = Dt(2025, 1, 1, 0, 0, 0),
                EffectiveTo = null,
                IsRecurring = true
            }
        };
        context.InstructorAvailabilities.AddRange(instructorAvailabilities);
        await context.SaveChangesAsync();

        var courseSessions = new List<CourseSession>
        {
            new()
            {
                CoordinatorId = coordinatorIds["noura"],
                ClassroomId = classroomIds["Seef Training Room"],
                CourseId = 1,
                InstructorId = instructorIds["mohammed"],
                StatusId = courseSessionStatusIds["Completed"],
                SessionDate = Dt(2026, 1, 12),
                StartTime = Dt(2026, 1, 12, 9, 0, 0),
                EndTime = Dt(2026, 1, 12, 15, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 12,
                CreatedAt = Dt(2025, 12, 20, 9, 0, 0),
                UpdatedAt = Dt(2025, 12, 20, 9, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["yousif"],
                ClassroomId = classroomIds["Pearl Lab 1"],
                CourseId = 2,
                InstructorId = instructorIds["fatima"],
                StatusId = courseSessionStatusIds["Completed"],
                SessionDate = Dt(2026, 1, 20),
                StartTime = Dt(2026, 1, 20, 9, 0, 0),
                EndTime = Dt(2026, 1, 20, 14, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 10,
                CreatedAt = Dt(2025, 12, 24, 10, 0, 0),
                UpdatedAt = Dt(2025, 12, 24, 10, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["noura"],
                ClassroomId = classroomIds["Riffa Seminar Hall"],
                CourseId = 3,
                InstructorId = instructorIds["ahmed"],
                StatusId = courseSessionStatusIds["Completed"],
                SessionDate = Dt(2026, 2, 5),
                StartTime = Dt(2026, 2, 5, 8, 0, 0),
                EndTime = Dt(2026, 2, 5, 13, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 14,
                CreatedAt = Dt(2026, 1, 10, 9, 0, 0),
                UpdatedAt = Dt(2026, 1, 10, 9, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["yousif"],
                ClassroomId = classroomIds["Seef Training Room"],
                CourseId = 4,
                InstructorId = instructorIds["mohammed"],
                StatusId = courseSessionStatusIds["Completed"],
                SessionDate = Dt(2026, 2, 18),
                StartTime = Dt(2026, 2, 18, 9, 0, 0),
                EndTime = Dt(2026, 2, 18, 16, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 12,
                CreatedAt = Dt(2026, 1, 20, 10, 0, 0),
                UpdatedAt = Dt(2026, 1, 20, 10, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["noura"],
                ClassroomId = classroomIds["Muharraq Innovation Lab"],
                CourseId = 5,
                InstructorId = instructorIds["sara"],
                StatusId = courseSessionStatusIds["Completed"],
                SessionDate = Dt(2026, 3, 2),
                StartTime = Dt(2026, 3, 2, 9, 0, 0),
                EndTime = Dt(2026, 3, 2, 15, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 15,
                CreatedAt = Dt(2026, 2, 2, 10, 0, 0),
                UpdatedAt = Dt(2026, 2, 2, 10, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["yousif"],
                ClassroomId = classroomIds["Riffa Seminar Hall"],
                CourseId = 6,
                InstructorId = instructorIds["mohammed"],
                StatusId = courseSessionStatusIds["Scheduled"],
                SessionDate = Dt(2026, 6, 10),
                StartTime = Dt(2026, 6, 10, 9, 0, 0),
                EndTime = Dt(2026, 6, 10, 16, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 2,
                CreatedAt = Dt(2026, 4, 1, 9, 0, 0),
                UpdatedAt = Dt(2026, 4, 1, 9, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["noura"],
                ClassroomId = classroomIds["Pearl Lab 1"],
                CourseId = 7,
                InstructorId = instructorIds["fatima"],
                StatusId = courseSessionStatusIds["Scheduled"],
                SessionDate = Dt(2026, 6, 16),
                StartTime = Dt(2026, 6, 16, 10, 0, 0),
                EndTime = Dt(2026, 6, 16, 16, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 1,
                CreatedAt = Dt(2026, 4, 2, 9, 0, 0),
                UpdatedAt = Dt(2026, 4, 2, 9, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["yousif"],
                ClassroomId = classroomIds["Isa Town Auditorium"],
                CourseId = 8,
                InstructorId = instructorIds["ahmed"],
                StatusId = courseSessionStatusIds["Scheduled"],
                SessionDate = Dt(2026, 7, 3),
                StartTime = Dt(2026, 7, 3, 9, 0, 0),
                EndTime = Dt(2026, 7, 3, 14, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 16,
                CreatedAt = Dt(2026, 4, 3, 9, 0, 0),
                UpdatedAt = Dt(2026, 4, 3, 9, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["noura"],
                ClassroomId = classroomIds["Muharraq Innovation Lab"],
                CourseId = 9,
                InstructorId = instructorIds["mohammed"],
                StatusId = courseSessionStatusIds["Ongoing"],
                SessionDate = Dt(2026, 4, 27),
                StartTime = Dt(2026, 4, 27, 8, 0, 0),
                EndTime = Dt(2026, 4, 27, 14, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 12,
                CreatedAt = Dt(2026, 3, 10, 9, 0, 0),
                UpdatedAt = Dt(2026, 4, 20, 9, 0, 0)
            },
            new()
            {
                CoordinatorId = coordinatorIds["yousif"],
                ClassroomId = classroomIds["Seef Training Room"],
                CourseId = 10,
                InstructorId = instructorIds["sara"],
                StatusId = courseSessionStatusIds["Cancelled"],
                SessionDate = Dt(2026, 5, 15),
                StartTime = Dt(2026, 5, 15, 9, 0, 0),
                EndTime = Dt(2026, 5, 15, 15, 0, 0),
                CurrentEnrollment = 0,
                MaxCapacity = 10,
                CreatedAt = Dt(2026, 3, 11, 9, 0, 0),
                UpdatedAt = Dt(2026, 4, 25, 9, 0, 0)
            }
        };
        context.CourseSessions.AddRange(courseSessions);
        await context.SaveChangesAsync();

        var sessionIdByCourseId = courseSessions.ToDictionary(x => x.CourseId, x => x.SessionId);
        var sessionById = courseSessions.ToDictionary(x => x.SessionId, x => x);

        var completedStatusId = enrollmentStatusIds["Completed"];
        var droppedStatusId = enrollmentStatusIds["Dropped"];

        var e1 = new Enrollment
        {
            SessionId = sessionIdByCourseId[1],
            TraineeId = traineeIds["ali"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2025, 12, 20, 10, 0, 0),
            StatusChangedAt = Dt(2026, 1, 12, 16, 0, 0),
            DropReason = null,
            CreatedAt = Dt(2025, 12, 20, 10, 0, 0),
            UpdatedAt = Dt(2026, 1, 12, 16, 0, 0)
        };
        var e2 = new Enrollment
        {
            SessionId = sessionIdByCourseId[1],
            TraineeId = traineeIds["mariam"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2025, 12, 21, 11, 0, 0),
            StatusChangedAt = Dt(2026, 1, 12, 16, 5, 0),
            DropReason = null,
            CreatedAt = Dt(2025, 12, 21, 11, 0, 0),
            UpdatedAt = Dt(2026, 1, 12, 16, 5, 0)
        };
        var e3 = new Enrollment
        {
            SessionId = sessionIdByCourseId[1],
            TraineeId = traineeIds["hassan"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2025, 12, 23, 9, 30, 0),
            StatusChangedAt = Dt(2026, 1, 12, 16, 10, 0),
            DropReason = null,
            CreatedAt = Dt(2025, 12, 23, 9, 30, 0),
            UpdatedAt = Dt(2026, 1, 12, 16, 10, 0)
        };
        var e4 = new Enrollment
        {
            SessionId = sessionIdByCourseId[1],
            TraineeId = traineeIds["noor"],
            EnrollmentStatusId = droppedStatusId,
            EnrollmentDate = Dt(2025, 12, 24, 10, 0, 0),
            StatusChangedAt = Dt(2026, 1, 10, 13, 0, 0),
            DropReason = "Medical leave",
            CreatedAt = Dt(2025, 12, 24, 10, 0, 0),
            UpdatedAt = Dt(2026, 1, 10, 13, 0, 0)
        };
        var e5 = new Enrollment
        {
            SessionId = sessionIdByCourseId[2],
            TraineeId = traineeIds["abdullah"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 1, 2, 9, 0, 0),
            StatusChangedAt = Dt(2026, 1, 20, 14, 30, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 1, 2, 9, 0, 0),
            UpdatedAt = Dt(2026, 1, 20, 14, 30, 0)
        };
        var e6 = new Enrollment
        {
            SessionId = sessionIdByCourseId[2],
            TraineeId = traineeIds["zainab"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 1, 3, 10, 0, 0),
            StatusChangedAt = Dt(2026, 1, 20, 14, 35, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 1, 3, 10, 0, 0),
            UpdatedAt = Dt(2026, 1, 20, 14, 35, 0)
        };
        var e7 = new Enrollment
        {
            SessionId = sessionIdByCourseId[2],
            TraineeId = traineeIds["khalid"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 1, 5, 11, 0, 0),
            StatusChangedAt = Dt(2026, 1, 20, 14, 40, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 1, 5, 11, 0, 0),
            UpdatedAt = Dt(2026, 1, 20, 14, 40, 0)
        };
        var e8 = new Enrollment
        {
            SessionId = sessionIdByCourseId[3],
            TraineeId = traineeIds["ali"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 1, 15, 9, 0, 0),
            StatusChangedAt = Dt(2026, 2, 5, 13, 20, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 1, 15, 9, 0, 0),
            UpdatedAt = Dt(2026, 2, 5, 13, 20, 0)
        };
        var e9 = new Enrollment
        {
            SessionId = sessionIdByCourseId[3],
            TraineeId = traineeIds["huda"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 1, 17, 9, 0, 0),
            StatusChangedAt = Dt(2026, 2, 5, 13, 25, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 1, 17, 9, 0, 0),
            UpdatedAt = Dt(2026, 2, 5, 13, 25, 0)
        };
        var e10 = new Enrollment
        {
            SessionId = sessionIdByCourseId[3],
            TraineeId = traineeIds["mariam"],
            EnrollmentStatusId = droppedStatusId,
            EnrollmentDate = Dt(2026, 1, 18, 9, 0, 0),
            StatusChangedAt = Dt(2026, 2, 3, 9, 0, 0),
            DropReason = "Work assignment conflict",
            CreatedAt = Dt(2026, 1, 18, 9, 0, 0),
            UpdatedAt = Dt(2026, 2, 3, 9, 0, 0)
        };
        var e11 = new Enrollment
        {
            SessionId = sessionIdByCourseId[4],
            TraineeId = traineeIds["hassan"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 1, 28, 10, 0, 0),
            StatusChangedAt = Dt(2026, 2, 18, 16, 10, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 1, 28, 10, 0, 0),
            UpdatedAt = Dt(2026, 2, 18, 16, 10, 0)
        };
        var e12 = new Enrollment
        {
            SessionId = sessionIdByCourseId[4],
            TraineeId = traineeIds["noor"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 1, 29, 10, 0, 0),
            StatusChangedAt = Dt(2026, 2, 18, 16, 15, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 1, 29, 10, 0, 0),
            UpdatedAt = Dt(2026, 2, 18, 16, 15, 0)
        };
        var e13 = new Enrollment
        {
            SessionId = sessionIdByCourseId[4],
            TraineeId = traineeIds["zainab"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 1, 31, 10, 0, 0),
            StatusChangedAt = Dt(2026, 2, 18, 16, 20, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 1, 31, 10, 0, 0),
            UpdatedAt = Dt(2026, 2, 18, 16, 20, 0)
        };
        var e14 = new Enrollment
        {
            SessionId = sessionIdByCourseId[5],
            TraineeId = traineeIds["abdullah"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 2, 12, 9, 0, 0),
            StatusChangedAt = Dt(2026, 3, 2, 15, 10, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 2, 12, 9, 0, 0),
            UpdatedAt = Dt(2026, 3, 2, 15, 10, 0)
        };
        var e15 = new Enrollment
        {
            SessionId = sessionIdByCourseId[5],
            TraineeId = traineeIds["khalid"],
            EnrollmentStatusId = completedStatusId,
            EnrollmentDate = Dt(2026, 2, 13, 9, 0, 0),
            StatusChangedAt = Dt(2026, 3, 2, 15, 15, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 2, 13, 9, 0, 0),
            UpdatedAt = Dt(2026, 3, 2, 15, 15, 0)
        };
        var e16 = new Enrollment
        {
            SessionId = sessionIdByCourseId[6],
            TraineeId = traineeIds["ali"],
            EnrollmentStatusId = enrollmentStatusIds["Confirmed"],
            EnrollmentDate = Dt(2026, 4, 10, 9, 0, 0),
            StatusChangedAt = Dt(2026, 4, 15, 11, 0, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 4, 10, 9, 0, 0),
            UpdatedAt = Dt(2026, 4, 15, 11, 0, 0)
        };
        var e17 = new Enrollment
        {
            SessionId = sessionIdByCourseId[6],
            TraineeId = traineeIds["mariam"],
            EnrollmentStatusId = enrollmentStatusIds["Enrolled"],
            EnrollmentDate = Dt(2026, 4, 11, 9, 0, 0),
            StatusChangedAt = Dt(2026, 4, 16, 11, 0, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 4, 11, 9, 0, 0),
            UpdatedAt = Dt(2026, 4, 16, 11, 0, 0)
        };
        var e18 = new Enrollment
        {
            SessionId = sessionIdByCourseId[7],
            TraineeId = traineeIds["abdullah"],
            EnrollmentStatusId = enrollmentStatusIds["Attending"],
            EnrollmentDate = Dt(2026, 4, 12, 9, 0, 0),
            StatusChangedAt = Dt(2026, 4, 17, 11, 0, 0),
            DropReason = null,
            CreatedAt = Dt(2026, 4, 12, 9, 0, 0),
            UpdatedAt = Dt(2026, 4, 17, 11, 0, 0)
        };

        var enrollments = new List<Enrollment>
        {
            e1, e2, e3, e4, e5, e6, e7, e8, e9,
            e10, e11, e12, e13, e14, e15, e16, e17, e18
        };

        context.Enrollments.AddRange(enrollments);
        await context.SaveChangesAsync();

        var sessionEnrollmentCounts = enrollments
            .GroupBy(x => x.SessionId)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var session in courseSessions)
        {
            session.CurrentEnrollment = sessionEnrollmentCounts.GetValueOrDefault(session.SessionId, 0);
        }

        await context.SaveChangesAsync();

        var waitlists = new List<Waitlist>
        {
            new()
            {
                SessionId = sessionIdByCourseId[6],
                TraineeId = traineeIds["hassan"],
                StatusId = waitlistStatusIds["Waiting"],
                Position = 1,
                AddedAt = Dt(2026, 4, 15, 12, 0, 0),
                Status = "Waiting",
                ExpiresAt = null
            },
            new()
            {
                SessionId = sessionIdByCourseId[6],
                TraineeId = traineeIds["noor"],
                StatusId = waitlistStatusIds["Expired"],
                Position = 2,
                AddedAt = Dt(2026, 4, 10, 12, 0, 0),
                Status = "Expired",
                ExpiresAt = Dt(2026, 4, 20, 12, 0, 0)
            },
            new()
            {
                SessionId = sessionIdByCourseId[7],
                TraineeId = traineeIds["zainab"],
                StatusId = waitlistStatusIds["Waiting"],
                Position = 1,
                AddedAt = Dt(2026, 4, 18, 12, 0, 0),
                Status = "Waiting",
                ExpiresAt = null
            }
        };
        context.Waitlists.AddRange(waitlists);
        await context.SaveChangesAsync();

        var passEnrollments = new List<Enrollment>
        {
            e1, e2, e3, e5, e6, e7, e8, e9, e11, e12, e13, e14, e15
        };

        var traineeCourseCompletions = passEnrollments
            .Select(x => new TraineeCourseCompletion
            {
                TraineeId = x.TraineeId,
                CourseId = sessionById[x.SessionId].CourseId,
                SessionId = x.SessionId,
                CompletionDate = sessionById[x.SessionId].SessionDate.AddDays(2),
                Result = "Pass"
            })
            .ToList();

        context.TraineeCourseCompletions.AddRange(traineeCourseCompletions);
        await context.SaveChangesAsync();

        var certificationTrackValidity = certificationTracks
            .ToDictionary(x => x.CertificationTrackId, x => x.ValidityPeriod ?? 24);

        var trackSoftwareId = certificationTrackIds["Software Engineering Professional"];
        var trackCyberId = certificationTrackIds["Cyber Defense Specialist"];
        var trackDataId = certificationTrackIds["Data Analytics Practitioner"];

        var issuedStatusId = certificationStatusIds["Issued"];
        var eligibleStatusId = certificationStatusIds["Eligible"];

        var t1IssuedDate = Dt(2026, 2, 20, 10, 0, 0);
        var t5IssuedDate = Dt(2026, 3, 20, 10, 0, 0);

        var traineeCertifications = new List<TraineeCertification>
        {
            new()
            {
                TraineeId = traineeIds["ali"],
                CertificationTrackId = trackSoftwareId,
                StatusId = issuedStatusId,
                EligibleDate = Dt(2026, 2, 6, 12, 0, 0),
                CertificateIssuedDate = t1IssuedDate,
                CertificateNumber = "CERT-2024-1001",
                ExpiryDate = t1IssuedDate.AddMonths(certificationTrackValidity[trackSoftwareId])
            },
            new()
            {
                TraineeId = traineeIds["abdullah"],
                CertificationTrackId = trackCyberId,
                StatusId = issuedStatusId,
                EligibleDate = Dt(2026, 3, 4, 12, 0, 0),
                CertificateIssuedDate = t5IssuedDate,
                CertificateNumber = "CERT-2024-1002",
                ExpiryDate = t5IssuedDate.AddMonths(certificationTrackValidity[trackCyberId])
            },
            new()
            {
                TraineeId = traineeIds["hassan"],
                CertificationTrackId = trackDataId,
                StatusId = eligibleStatusId,
                EligibleDate = Dt(2026, 2, 25, 12, 0, 0),
                CertificateIssuedDate = null,
                CertificateNumber = null,
                ExpiryDate = null
            },
            new()
            {
                TraineeId = traineeIds["khalid"],
                CertificationTrackId = trackCyberId,
                StatusId = eligibleStatusId,
                EligibleDate = Dt(2026, 3, 10, 12, 0, 0),
                CertificateIssuedDate = null,
                CertificateNumber = null,
                ExpiryDate = null
            }
        };
        context.TraineeCertifications.AddRange(traineeCertifications);
        await context.SaveChangesAsync();

        var paymentStatusPlan = new[]
        {
            paymentStatusIds["Paid"],
            paymentStatusIds["Paid"],
            paymentStatusIds["Partial"],
            paymentStatusIds["Paid"],
            paymentStatusIds["Unpaid"],
            paymentStatusIds["Overdue"],
            paymentStatusIds["Paid"],
            paymentStatusIds["Partial"],
            paymentStatusIds["Unpaid"],
            paymentStatusIds["Paid"],
            paymentStatusIds["Paid"],
            paymentStatusIds["Overdue"],
            paymentStatusIds["Partial"],
            paymentStatusIds["Paid"],
            paymentStatusIds["Unpaid"],
            paymentStatusIds["Paid"],
            paymentStatusIds["Partial"],
            paymentStatusIds["Paid"]
        };

        var paymentRecords = new List<PaymentRecord>();
        for (var i = 0; i < enrollments.Count; i++)
        {
            var enrollment = enrollments[i];
            var session = sessionById[enrollment.SessionId];
            var course = allCourses[session.CourseId];

            var issuedDate = enrollment.EnrollmentDate.AddDays(1);
            var statusId = paymentStatusPlan[i];

            var paymentRecord = new PaymentRecord
            {
                EnrollmentId = enrollment.EnrollmentId,
                CoordinatorId = i % 2 == 0 ? coordinatorIds["noura"] : coordinatorIds["yousif"],
                StatusId = statusId,
                TotalAmount = course.EnrollmentFee,
                DueDate = enrollment.EnrollmentDate.AddDays(7),
                IssuedDate = issuedDate,
                CreatedAt = issuedDate,
                UpdatedAt = issuedDate.AddHours(1)
            };

            paymentRecords.Add(paymentRecord);
        }

        context.PaymentRecords.AddRange(paymentRecords);
        await context.SaveChangesAsync();

        var paymentMethods = new[] { "Cash", "Bank Transfer", "Credit Card", "Cheque" };
        var paymentTransactions = new List<PaymentTransaction>();

        for (var i = 0; i < paymentRecords.Count; i++)
        {
            var paymentRecord = paymentRecords[i];

            if (paymentRecord.StatusId == paymentStatusIds["Paid"])
            {
                if (i % 2 == 0)
                {
                    paymentTransactions.Add(new PaymentTransaction
                    {
                        PaymentRecordId = paymentRecord.PaymentRecordId,
                        CoordinatorId = paymentRecord.CoordinatorId,
                        Amount = paymentRecord.TotalAmount,
                        PaymentMethod = paymentMethods[i % paymentMethods.Length],
                        PaymentDate = paymentRecord.IssuedDate.AddDays(2),
                        Notes = "Full settlement",
                        CreatedAt = paymentRecord.IssuedDate.AddDays(2)
                    });
                }
                else
                {
                    var firstAmount = decimal.Round(paymentRecord.TotalAmount * 0.60m, 2, MidpointRounding.AwayFromZero);
                    var secondAmount = paymentRecord.TotalAmount - firstAmount;

                    paymentTransactions.Add(new PaymentTransaction
                    {
                        PaymentRecordId = paymentRecord.PaymentRecordId,
                        CoordinatorId = paymentRecord.CoordinatorId,
                        Amount = firstAmount,
                        PaymentMethod = paymentMethods[i % paymentMethods.Length],
                        PaymentDate = paymentRecord.IssuedDate.AddDays(2),
                        Notes = "Installment 1",
                        CreatedAt = paymentRecord.IssuedDate.AddDays(2)
                    });

                    paymentTransactions.Add(new PaymentTransaction
                    {
                        PaymentRecordId = paymentRecord.PaymentRecordId,
                        CoordinatorId = paymentRecord.CoordinatorId,
                        Amount = secondAmount,
                        PaymentMethod = paymentMethods[(i + 1) % paymentMethods.Length],
                        PaymentDate = paymentRecord.IssuedDate.AddDays(4),
                        Notes = "Installment 2",
                        CreatedAt = paymentRecord.IssuedDate.AddDays(4)
                    });
                }
            }
            else if (paymentRecord.StatusId == paymentStatusIds["Partial"])
            {
                var partialAmount = decimal.Round(paymentRecord.TotalAmount * 0.50m, 2, MidpointRounding.AwayFromZero);
                paymentTransactions.Add(new PaymentTransaction
                {
                    PaymentRecordId = paymentRecord.PaymentRecordId,
                    CoordinatorId = paymentRecord.CoordinatorId,
                    Amount = partialAmount,
                    PaymentMethod = paymentMethods[(i + 2) % paymentMethods.Length],
                    PaymentDate = paymentRecord.IssuedDate.AddDays(3),
                    Notes = "Partial payment",
                    CreatedAt = paymentRecord.IssuedDate.AddDays(3)
                });
            }
        }

        context.PaymentTransactions.AddRange(paymentTransactions);
        await context.SaveChangesAsync();

        var completedSessionStatusId = courseSessionStatusIds["Completed"];

        var assessmentCandidates = enrollments
            .Where(x =>
                sessionById[x.SessionId].StatusId == completedSessionStatusId &&
                (x.EnrollmentStatusId == completedStatusId || x.EnrollmentStatusId == droppedStatusId))
            .ToList();

        var assessments = new List<Assessment>();
        for (var i = 0; i < assessmentCandidates.Count; i++)
        {
            var enrollment = assessmentCandidates[i];
            var session = sessionById[enrollment.SessionId];

            var isDropped = enrollment.EnrollmentStatusId == droppedStatusId;
            var assessmentDate = session.SessionDate.AddDays(3);

            assessments.Add(new Assessment
            {
                EnrollmentId = enrollment.EnrollmentId,
                InstructorId = session.InstructorId,
                Result = isDropped ? "Fail" : "Pass",
                Remarks = i % 2 == 0 ? "Assessment completed and archived." : null,
                AssessmentDate = assessmentDate,
                CreatedAt = assessmentDate
            });
        }

        context.Assessments.AddRange(assessments);
        await context.SaveChangesAsync();

        var notifications = new List<Notification>
        {
            new()
            {
                UserId = traineeUsers["ali"].Id,
                Title = "Enrollment Confirmed",
                Message = "Your enrollment in Advanced C# Development has been confirmed.",
                Type = "Enrollment",
                RelatedEntityType = "Enrollment",
                IsRead = false,
                CreatedAt = Dt(2026, 4, 15, 12, 0, 0),
                ReadAt = null
            },
            new()
            {
                UserId = traineeUsers["mariam"].Id,
                Title = "Enrollment Received",
                Message = "Your enrollment in Advanced C# Development has been received.",
                Type = "Enrollment",
                RelatedEntityType = "Enrollment",
                IsRead = true,
                CreatedAt = Dt(2026, 4, 16, 12, 0, 0),
                ReadAt = Dt(2026, 4, 16, 16, 0, 0)
            },
            new()
            {
                UserId = traineeUsers["abdullah"].Id,
                Title = "Session Attendance",
                Message = "Your status for Secure Network Operations is now Attending.",
                Type = "Enrollment",
                RelatedEntityType = "Enrollment",
                IsRead = false,
                CreatedAt = Dt(2026, 4, 17, 12, 0, 0),
                ReadAt = null
            },
            new()
            {
                UserId = traineeUsers["ali"].Id,
                Title = "Assessment Result",
                Message = "Your result for C# Fundamentals has been recorded: Pass.",
                Type = "Assessment",
                RelatedEntityType = "Assessment",
                IsRead = true,
                CreatedAt = Dt(2026, 1, 15, 10, 0, 0),
                ReadAt = Dt(2026, 1, 15, 13, 0, 0)
            },
            new()
            {
                UserId = traineeUsers["noor"].Id,
                Title = "Assessment Result",
                Message = "Your result for C# Fundamentals has been recorded: Fail.",
                Type = "Assessment",
                RelatedEntityType = "Assessment",
                IsRead = false,
                CreatedAt = Dt(2026, 1, 15, 10, 15, 0),
                ReadAt = null
            },
            new()
            {
                UserId = traineeUsers["ali"].Id,
                Title = "Certification Issued",
                Message = "Congratulations! Your Software Engineering Professional certification has been issued.",
                Type = "Certification",
                RelatedEntityType = "Trainee_Certification",
                IsRead = true,
                CreatedAt = Dt(2026, 2, 20, 12, 0, 0),
                ReadAt = Dt(2026, 2, 20, 15, 0, 0)
            },
            new()
            {
                UserId = traineeUsers["abdullah"].Id,
                Title = "Certification Issued",
                Message = "Congratulations! Your Cyber Defense Specialist certification has been issued.",
                Type = "Certification",
                RelatedEntityType = "Trainee_Certification",
                IsRead = false,
                CreatedAt = Dt(2026, 3, 20, 12, 0, 0),
                ReadAt = null
            },
            new()
            {
                UserId = traineeUsers["hassan"].Id,
                Title = "Certification Eligible",
                Message = "Congratulations! You are now eligible for Data Analytics Practitioner.",
                Type = "Certification",
                RelatedEntityType = "Trainee_Certification",
                IsRead = false,
                CreatedAt = Dt(2026, 2, 25, 12, 0, 0),
                ReadAt = null
            },
            new()
            {
                UserId = traineeUsers["ali"].Id,
                Title = "Payment Received",
                Message = "Payment of BHD 180.00 received for C# Fundamentals.",
                Type = "Payment",
                RelatedEntityType = "Payment_Record",
                IsRead = true,
                CreatedAt = Dt(2025, 12, 23, 12, 0, 0),
                ReadAt = Dt(2025, 12, 23, 14, 0, 0)
            },
            new()
            {
                UserId = traineeUsers["abdullah"].Id,
                Title = "Payment Received",
                Message = "Payment of BHD 210.00 received for Cybersecurity Foundations.",
                Type = "Payment",
                RelatedEntityType = "Payment_Record",
                IsRead = true,
                CreatedAt = Dt(2026, 2, 15, 12, 0, 0),
                ReadAt = Dt(2026, 2, 15, 16, 0, 0)
            },
            new()
            {
                UserId = traineeUsers["hassan"].Id,
                Title = "Waitlist Update",
                Message = "You have been added to the waitlist for Advanced C# Development.",
                Type = "Waitlist",
                RelatedEntityType = "Waitlist",
                IsRead = false,
                CreatedAt = Dt(2026, 4, 15, 12, 30, 0),
                ReadAt = null
            },
            new()
            {
                UserId = traineeUsers["noor"].Id,
                Title = "Waitlist Expired",
                Message = "Your waitlist entry for Advanced C# Development has expired.",
                Type = "Waitlist",
                RelatedEntityType = "Waitlist",
                IsRead = false,
                CreatedAt = Dt(2026, 4, 20, 12, 30, 0),
                ReadAt = null
            },
            new()
            {
                UserId = traineeUsers["zainab"].Id,
                Title = "Waitlist Update",
                Message = "You have been added to the waitlist for Secure Network Operations.",
                Type = "Waitlist",
                RelatedEntityType = "Waitlist",
                IsRead = true,
                CreatedAt = Dt(2026, 4, 18, 12, 30, 0),
                ReadAt = Dt(2026, 4, 18, 16, 30, 0)
            },
            new()
            {
                UserId = traineeUsers["khalid"].Id,
                Title = "Assessment Result",
                Message = "Your result for Cybersecurity Foundations has been recorded: Pass.",
                Type = "Assessment",
                RelatedEntityType = "Assessment",
                IsRead = false,
                CreatedAt = Dt(2026, 3, 5, 10, 0, 0),
                ReadAt = null
            }
        };
        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync();
    }
}
