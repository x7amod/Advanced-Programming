/* =============================================================================
   Created by GitHub Copilot in SSMS - review carefully before executing

   02_seed.sql
   Seed data for TrainingInstituteDB
   Theme: Bahrain-based training institute
   ============================================================================= */

SET NOCOUNT ON;
GO

BEGIN TRANSACTION;

/* -----------------------------------------------------------------------------
   0. Cleanup Existing Data (Reverse Dependency Order)
   ----------------------------------------------------------------------------- */
PRINT 'Cleaning up existing data...';
DELETE FROM dbo.Notification;
DELETE FROM dbo.Assessment;
DELETE FROM dbo.Payment_Transaction;
DELETE FROM dbo.Payment_Record;
DELETE FROM dbo.Trainee_Certification;
DELETE FROM dbo.Trainee_Course_Completion;
DELETE FROM dbo.Waitlist;
DELETE FROM dbo.Enrollment;
DELETE FROM dbo.Course_Session;
DELETE FROM dbo.Instructor_Availability;
DELETE FROM dbo.Instructor_Expertise;
DELETE FROM dbo.Trainee;
DELETE FROM dbo.Instructor;
DELETE FROM dbo.Coordinator;
DELETE FROM dbo.AppUser;
DELETE FROM dbo.Certification_Required_Course;
DELETE FROM dbo.Classroom_Equipment;
DELETE FROM dbo.Course;
DELETE FROM dbo.Classroom;
DELETE FROM dbo.Certification_Track;
DELETE FROM dbo.Category WHERE Parent_categoryID IS NOT NULL; -- Children first
DELETE FROM dbo.Category;                                     -- Parents second
DELETE FROM dbo.Subject_Area;
DELETE FROM dbo.Payment_Status;
DELETE FROM dbo.User_Role;
DELETE FROM dbo.Enrollment_Status;

PRINT 'Resetting Identity Columns...';
IF OBJECTPROPERTY(OBJECT_ID('dbo.Notification'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Notification', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Assessment'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Assessment', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Payment_Transaction'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Payment_Transaction', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Payment_Record'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Payment_Record', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Trainee_Certification'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Trainee_Certification', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Trainee_Course_Completion'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Trainee_Course_Completion', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Waitlist'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Waitlist', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Enrollment'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Enrollment', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Course_Session'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Course_Session', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Instructor_Availability'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Instructor_Availability', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Instructor_Expertise'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Instructor_Expertise', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Trainee'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Trainee', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Instructor'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Instructor', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Coordinator'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Coordinator', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.AppUser'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.AppUser', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Certification_Required_Course'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Certification_Required_Course', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Classroom_Equipment'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Classroom_Equipment', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Course'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Course', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Classroom'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Classroom', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Certification_Track'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Certification_Track', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Category'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Category', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Subject_Area'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Subject_Area', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Payment_Status'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Payment_Status', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.User_Role'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.User_Role', RESEED, 0);
IF OBJECTPROPERTY(OBJECT_ID('dbo.Enrollment_Status'), 'TableHasIdentity') = 1 DBCC CHECKIDENT ('dbo.Enrollment_Status', RESEED, 0);

/* -----------------------------------------------------------------------------
   1. Enrollment_Status
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Enrollment_Status (status) VALUES
    (N'Enrolled'),    -- 1
    (N'Confirmed'),   -- 2
    (N'Completed'),   -- 3
    (N'Dropped'),     -- 4
    (N'Attending');   -- 5

/* -----------------------------------------------------------------------------
   2. User_Role
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.User_Role (roleName) VALUES
    (N'Trainee'),               -- 1
    (N'Instructor'),            -- 2
    (N'Training Coordinator');  -- 3

/* -----------------------------------------------------------------------------
   3. Payment_Status  (explicit IDs, not IDENTITY)
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Payment_Status (statusID, status) VALUES
    (1, N'Unpaid'),
    (2, N'Partial'),
    (3, N'Paid'),
    (4, N'Overdue');

/* -----------------------------------------------------------------------------
   4. Subject_Area
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Subject_Area (name, description) VALUES
    (N'Information Technology', N'Software, systems, and IT infrastructure topics.'),   -- 1
    (N'Business Management',    N'Organizational leadership and business operations.'), -- 2
    (N'Project Management',     N'Planning, executing, and closing projects.'),         -- 3
    (N'Cybersecurity',          N'Information security, threats, and defense.'),        -- 4
    (N'Data & Analytics',       N'Data analysis, visualization, and business intelligence.'); -- 5

/* -----------------------------------------------------------------------------
   5. Category  (parents first, then children)
   ----------------------------------------------------------------------------- */
-- Parents
INSERT INTO dbo.Category (name, description, Parent_categoryID) VALUES
    (N'ICT',      N'Information and Communication Technology umbrella.', NULL), -- 1 (parent)
    (N'Business', N'Business and management umbrella.',                  NULL); -- 2 (parent)

-- Children
INSERT INTO dbo.Category (name, description, Parent_categoryID) VALUES
    (N'Software Development', N'Programming and software engineering.',     1), -- 3
    (N'Networking',           N'Computer networking and administration.',   1), -- 4
    (N'Security',             N'Cybersecurity discipline.',                 1), -- 5
    (N'Leadership',           N'Management and leadership skills.',         2), -- 6
    (N'Finance',              N'Financial management and accounting.',      2), -- 7
    (N'Analytics',            N'Business analytics and data-driven decisions.', 2); -- 8

/* -----------------------------------------------------------------------------
   6. Certification_Track
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Certification_Track (name, description, validityPeriod, isActive, createdAt, updatedAt) VALUES
    (N'Certified IT Professional',       N'Foundational IT and networking certification.',          24, 1, '2025-01-05 09:00:00', '2025-01-05 09:00:00'), -- 1
    (N'Certified Project Manager',       N'Comprehensive project management certification.',        36, 1, '2025-01-10 09:00:00', '2025-01-10 09:00:00'), -- 2
    (N'Certified Cybersecurity Analyst', N'Security fundamentals and ethical hacking certification.', 24, 1, '2025-01-15 09:00:00', '2025-01-15 09:00:00'); -- 3

/* -----------------------------------------------------------------------------
   7. Classroom
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Classroom (name, location, building, floor, capacity, isActive) VALUES
    (N'Seef Hall A',    N'Seef District, Manama',   N'Building 36', N'1st',  12, 1), -- 1 (small)
    (N'Riffa Room 101', N'East Riffa',              N'Building 12', N'1st',  25, 1), -- 2 (medium)
    (N'Muharraq Lab 1', N'Muharraq, Arad Highway',  N'Building 45', N'2nd',  20, 1), -- 3 (medium)
    (N'Isa Town Hall',  N'Isa Town, Block 801',     N'Building 88', N'3rd',  40, 1), -- 4 (large)
    (N'Hamad Town Auditorium', N'Hamad Town, Roundabout 17', N'Building 22', N'Ground', 50, 1); -- 5 (large)

/* -----------------------------------------------------------------------------
   8. Course  (explicit IDs, NOT IDENTITY)
      - Courses 1-7: no prerequisite
      - Courses 8-10: have prerequisites (single level)
   ----------------------------------------------------------------------------- */
-- Courses without prerequisites
INSERT INTO dbo.Course (courseID, subjectAreaID, categoryID, prerequisiteCourseID, courseCode, title, description, durationHours, maxCapacity, enrollmentFee, equipmentRequirements, isActive, createdAt, updatedAt) VALUES
    (1, 1, 3, NULL, N'IT-101', N'IT Fundamentals',            N'Introduction to computing, hardware, and software basics.',          16.00, 20, 120.00, N'Lab computers with standard office software', 1, '2025-02-01 09:00:00', '2025-02-01 09:00:00'),
    (2, 2, 6, NULL, N'BM-110', N'Business Communication',     N'Effective written and verbal communication in the workplace.',        8.00, 25,  80.00, NULL,                                           1, '2025-02-01 09:00:00', '2025-02-01 09:00:00'),
    (3, 3, 6, NULL, N'PM-101', N'Project Management Basics',  N'Core concepts of initiating, planning, and executing projects.',     24.00, 25, 220.00, NULL,                                           1, '2025-02-01 09:00:00', '2025-02-01 09:00:00'),
    (4, 4, 5, NULL, N'CS-101', N'Cybersecurity Essentials',   N'Foundations of information security, threats, and best practices.', 16.00, 20, 180.00, N'Lab computers with virtualization software',  1, '2025-02-01 09:00:00', '2025-02-01 09:00:00'),
    (5, 5, 8, NULL, N'DA-101', N'Data Analytics Introduction',N'Hands-on introduction to Excel, SQL, and data visualization.',      24.00, 20, 200.00, N'Lab computers with Excel and Power BI',       1, '2025-02-01 09:00:00', '2025-02-01 09:00:00'),
    (6, 2, 7, NULL, N'FI-120', N'Financial Management',       N'Budgeting, financial analysis, and reporting for managers.',        16.00, 25, 150.00, NULL,                                           1, '2025-02-01 09:00:00', '2025-02-01 09:00:00'),
    (7, 1, 4, NULL, N'NW-140', N'Network Administration',     N'Managing LANs, WANs, and enterprise network services.',             24.00, 18, 230.00, N'Lab computers with Packet Tracer and VMs',    1, '2025-02-01 09:00:00', '2025-02-01 09:00:00');

-- Courses with prerequisites (must be inserted AFTER their prerequisites)
INSERT INTO dbo.Course (courseID, subjectAreaID, categoryID, prerequisiteCourseID, courseCode, title, description, durationHours, maxCapacity, enrollmentFee, equipmentRequirements, isActive, createdAt, updatedAt) VALUES
    (8,  1, 3, 1, N'IT-210', N'Advanced Python Development', N'Object-oriented Python, APIs, and application development.',      32.00, 15, 320.00, N'Lab computers with Visual Studio Code and Python 3.x', 1, '2025-02-05 09:00:00', '2025-02-05 09:00:00'),
    (9,  3, 6, 3, N'PM-210', N'Advanced Project Management', N'Risk management, agile methods, and program governance.',         24.00, 20, 300.00, NULL,                                                    1, '2025-02-05 09:00:00', '2025-02-05 09:00:00'),
    (10, 4, 5, 4, N'CS-220', N'Ethical Hacking',             N'Penetration testing methodology, tools, and reporting.',          32.00, 15, 400.00, N'Lab computers with Kali Linux and target VMs',         1, '2025-02-05 09:00:00', '2025-02-05 09:00:00');

/* -----------------------------------------------------------------------------
   9. Classroom_Equipment
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Classroom_Equipment (classroomID, equipmentType, quantity, description) VALUES
    (1, N'Projector',                1, N'HD overhead projector'),
    (1, N'Whiteboard',               2, N'Dual magnetic whiteboards'),
    (2, N'Projector',                1, N'HD ceiling-mounted projector'),
    (2, N'Smart Board',              1, N'Interactive smart board'),
    (2, N'Whiteboard',               1, N'Standard whiteboard'),
    (3, N'Lab Computer',            20, N'Desktop PCs with development tools'),
    (3, N'Projector',                1, N'Short-throw projector'),
    (3, N'Whiteboard',               1, N'Magnetic whiteboard'),
    (4, N'Projector',                2, N'Dual HD projectors'),
    (4, N'Video Conferencing System',1, N'Logitech Rally conferencing system'),
    (4, N'Smart Board',              1, N'Large-format smart board'),
    (5, N'Projector',                2, N'Auditorium dual projectors'),
    (5, N'Video Conferencing System',1, N'Full-room conferencing system'),
    (5, N'Whiteboard',               2, N'Whiteboards at front and back');

/* -----------------------------------------------------------------------------
   10. Certification_Required_Course
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Certification_Required_Course (courseID, certificationID, sequenceOrder, isMandatory) VALUES
    -- Certified IT Professional
    (1, 1, 1, 1),
    (7, 1, 2, 1),
    (8, 1, 3, 1),
    (5, 1, 4, 0), 

    -- Certified Project Manager
    (3, 2, 1, 1),
    (9, 2, 2, 1),
    (2, 2, 3, 0), 
    (6, 2, 4, 0), 

    -- Certified Cybersecurity Analyst
    (1, 3, 1, 1),
    (4, 3, 2, 1),
    (10, 3, 3, 1),
    (7, 3, 4, 0); 

/* -----------------------------------------------------------------------------
   11. AppUser
   ----------------------------------------------------------------------------- */
-- Coordinators
INSERT INTO dbo.AppUser (roleID, firstName, lastName, email, password, phone, createdAt, updatedAt, isActive) VALUES
    (3, N'Mohammed', N'Al-Dosari', N'mohammed.aldosari@bticademy.bh', N'pass123', N'+973 3311 2233', '2024-06-01 09:00:00', '2024-06-01 09:00:00', 1), -- 1
    (3, N'Fatima',   N'Janahi',    N'fatima.janahi@bticademy.bh',    N'pass123', N'+973 3322 4455', '2024-06-05 09:00:00', '2024-06-05 09:00:00', 1); -- 2

-- Instructors
INSERT INTO dbo.AppUser (roleID, firstName, lastName, email, password, phone, createdAt, updatedAt, isActive) VALUES
    (2, N'Yousif',  N'Al-Khalifa', N'yousif.alkhalifa@bticademy.bh', N'pass123', N'+973 3344 5566', '2024-07-01 09:00:00', '2024-07-01 09:00:00', 1), -- 3
    (2, N'Noor',    N'Al-Mannai',  N'noor.almannai@bticademy.bh',    N'pass123', N'+973 3355 6677', '2024-07-10 09:00:00', '2024-07-10 09:00:00', 1), -- 4
    (2, N'Khalid',  N'Al-Zayani',  N'khalid.alzayani@bticademy.bh',  N'pass123', N'+973 3366 7788', '2024-08-01 09:00:00', '2024-08-01 09:00:00', 1), -- 5
    (2, N'Aisha',   N'Al-Sayed',   N'aisha.alsayed@bticademy.bh',    N'pass123', N'+973 3377 8899', '2024-08-15 09:00:00', '2024-08-15 09:00:00', 1); -- 6

-- Trainees
INSERT INTO dbo.AppUser (roleID, firstName, lastName, email, password, phone, createdAt, updatedAt, isActive) VALUES
    (1, N'Ali',      N'Al-Mahmood',  N'ali.almahmood@bticademy.bh',    N'pass123', N'+973 3400 1100', '2025-03-01 10:00:00', '2025-03-01 10:00:00', 1), -- 7
    (1, N'Layla',    N'Al-Ansari',   N'layla.alansari@bticademy.bh',   N'pass123', N'+973 3400 1101', '2025-03-05 10:00:00', '2025-03-05 10:00:00', 1), -- 8
    (1, N'Omar',     N'Al-Binali',   N'omar.albinali@bticademy.bh',    N'pass123', N'+973 3400 1102', '2025-03-10 10:00:00', '2025-03-10 10:00:00', 1), -- 9
    (1, N'Mariam',   N'Al-Sulaiti',  N'mariam.alsulaiti@bticademy.bh', N'pass123', N'+973 3400 1103', '2025-03-15 10:00:00', '2025-03-15 10:00:00', 1), -- 10
    (1, N'Hassan',   N'Al-Hajri',    N'hassan.alhajri@bticademy.bh',   N'pass123', N'+973 3400 1104', '2025-04-01 10:00:00', '2025-04-01 10:00:00', 1), -- 11
    (1, N'Sara',     N'Al-Dossary',  N'sara.aldossary@bticademy.bh',   N'pass123', N'+973 3400 1105', '2025-04-05 10:00:00', '2025-04-05 10:00:00', 1), -- 12
    (1, N'Abdulla',  N'Al-Rumaihi',  N'abdulla.alrumaihi@bticademy.bh',N'pass123', N'+973 3400 1106', '2025-04-10 10:00:00', '2025-04-10 10:00:00', 1), -- 13
    (1, N'Huda',     N'Al-Kooheji',  N'huda.alkooheji@bticademy.bh',   N'pass123', N'+973 3400 1107', '2025-05-01 10:00:00', '2025-05-01 10:00:00', 1), -- 14
    (1, N'Rashid',   N'Al-Fadhel',   N'rashid.alfadhel@bticademy.bh',  N'pass123', N'+973 3400 1108', '2025-05-10 10:00:00', '2025-05-10 10:00:00', 1), -- 15
    (1, N'Zainab',   N'Al-Thawadi',  N'zainab.althawadi@bticademy.bh', N'pass123', N'+973 3400 1109', '2025-05-20 10:00:00', '2025-05-20 10:00:00', 1); -- 16

/* -----------------------------------------------------------------------------
   12. Coordinator
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Coordinator (userID, department) VALUES
    (1, N'Academic Affairs'), 
    (2, N'Operations');       

/* -----------------------------------------------------------------------------
   13. Instructor
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Instructor (userID, hireDate, bio) VALUES
    (3, '2024-07-01 09:00:00', N'Senior IT and cybersecurity instructor with over 10 years of industry experience. Specializes in network defense and penetration testing.'),
    (4, '2024-07-10 09:00:00', N'Seasoned business and project management trainer. PMP-certified with consulting background across the GCC.'), 
    (5, '2024-08-01 09:00:00', N'Data analytics and IT systems expert. Former lead analyst at a regional bank with deep expertise in SQL, Power BI, and Python.'), 
    (6, '2024-08-15 09:00:00', N'Management consultant turned trainer, focusing on leadership, finance, and project delivery for mid-sized organizations in Bahrain.'); 

/* -----------------------------------------------------------------------------
   14. Trainee
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Trainee (userID, dateOfBirth, address, emergencyContact) VALUES
    ( 7, '1995-05-14 00:00:00', N'Villa 14, Road 22, Block 320, Riffa',      N'+973 3900 1100'),
    ( 8, '1998-11-02 00:00:00', N'Flat 5, Building 18, Block 338, Seef',     N'+973 3900 1101'), 
    ( 9, '1993-07-21 00:00:00', N'Villa 7, Road 44, Block 702, Isa Town',    N'+973 3900 1102'), 
    (10, '2000-02-10 00:00:00', N'Flat 12, Building 40, Block 428, Muharraq',N'+973 3900 1103'), 
    (11, '1991-09-30 00:00:00', N'Villa 9, Road 15, Block 521, Budaiya',     N'+973 3900 1104'), 
    (12, '1999-03-18 00:00:00', N'Flat 3, Building 77, Block 304, Manama',   N'+973 3900 1105'), 
    (13, '1996-12-05 00:00:00', N'Villa 21, Road 31, Block 615, Sitra',      N'+973 3900 1106'), 
    (14, '1994-08-22 00:00:00', N'Flat 9, Building 22, Block 908, Hamad Town', N'+973 3900 1107'), 
    (15, '2001-06-15 00:00:00', N'Villa 3, Road 9, Block 336, Adliya',       N'+973 3900 1108'), 
    (16, '1997-01-28 00:00:00', N'Flat 22, Building 65, Block 342, Juffair', N'+973 3900 1109'); 

/* -----------------------------------------------------------------------------
   15. Instructor_Expertise
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Instructor_Expertise (instructorID, subjectAreaID, proficiencyLevel) VALUES
    (1, 1, N'Expert'),
    (1, 4, N'Expert'),
    (2, 2, N'Expert'),
    (2, 3, N'Intermediate'),
    (3, 1, N'Intermediate'),
    (3, 5, N'Expert'),
    (4, 3, N'Expert'),
    (4, 2, N'Intermediate');

/* -----------------------------------------------------------------------------
   16. Instructor_Availability
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Instructor_Availability (instructorID, dayOfWeek, startTime, endTime, effectiveFrom, effectiveTo, isRecurring) VALUES
    (1, 1, '2026-01-01 08:00:00', '2026-01-01 16:00:00', '2025-01-01 00:00:00', NULL, 1),
    (1, 5, '2026-01-01 08:00:00', '2026-01-01 16:00:00', '2025-01-01 00:00:00', NULL, 1),
    (2, 2, '2026-01-01 09:00:00', '2026-01-01 17:00:00', '2025-01-01 00:00:00', NULL, 1),
    (2, 3, '2026-01-01 09:00:00', '2026-01-01 17:00:00', '2025-01-01 00:00:00', NULL, 1),
    (3, 3, '2026-01-01 08:30:00', '2026-01-01 16:30:00', '2025-01-01 00:00:00', '2027-12-31 00:00:00', 1),
    (3, 5, '2026-01-01 08:30:00', '2026-01-01 16:30:00', '2025-01-01 00:00:00', '2027-12-31 00:00:00', 1),
    (4, 2, '2026-01-01 10:00:00', '2026-01-01 18:00:00', '2025-01-01 00:00:00', NULL, 1);

/* -----------------------------------------------------------------------------
   17. Course_Session
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Course_Session (coordinatorID, classroomID, courseID, instructorID, sessionDate, startTime, endTime, currentEnrollment, maxCapacity, status, createdAt, updatedAt) VALUES
    (1, 3, 1, 1, '2026-01-12 00:00:00', '2026-01-12 09:00:00', '2026-01-12 17:00:00', 4, 4, N'Completed',  '2025-11-01 09:00:00', '2026-01-13 18:00:00'), 
    (2, 2, 2, 2, '2026-02-09 00:00:00', '2026-02-09 09:00:00', '2026-02-09 17:00:00', 3, 5, N'Completed',  '2025-12-01 09:00:00', '2026-02-09 18:00:00'), 
    (1, 4, 3, 4, '2026-02-23 00:00:00', '2026-02-23 10:00:00', '2026-02-23 18:00:00', 4, 6, N'Completed',  '2025-12-15 09:00:00', '2026-02-25 18:00:00'), 
    (2, 3, 4, 1, '2026-03-16 00:00:00', '2026-03-16 09:00:00', '2026-03-16 17:00:00', 3, 5, N'Completed',  '2026-01-10 09:00:00', '2026-03-17 18:00:00'), 
    (1, 2, 5, 3, '2026-04-15 00:00:00', '2026-04-15 09:00:00', '2026-04-15 16:00:00', 3, 5, N'Ongoing',    '2026-02-10 09:00:00', '2026-04-17 09:00:00'), 
    (2, 3, 7, 1, '2026-05-11 00:00:00', '2026-05-11 09:00:00', '2026-05-11 16:00:00', 2, 5, N'Scheduled',  '2026-03-01 09:00:00', '2026-04-01 09:00:00'), 
    (1, 3, 8, 3, '2026-06-07 00:00:00', '2026-06-07 09:00:00', '2026-06-07 17:00:00', 2, 5, N'Scheduled',  '2026-03-15 09:00:00', '2026-04-01 09:00:00'), 
    (2, 4, 9, 2, '2026-05-17 00:00:00', '2026-05-17 10:00:00', '2026-05-17 18:00:00', 2, 5, N'Scheduled',  '2026-03-10 09:00:00', '2026-04-01 09:00:00'), 
    (1, 1, 6, 4, '2026-03-02 00:00:00', '2026-03-02 10:00:00', '2026-03-02 16:00:00', 0, 5, N'Cancelled',  '2026-01-05 09:00:00', '2026-02-20 09:00:00'), 
    (2, 3, 10, 1, '2026-04-05 00:00:00', '2026-04-05 09:00:00', '2026-04-05 17:00:00', 2, 4, N'Completed', '2026-02-01 09:00:00', '2026-04-08 18:00:00'); 

/* -----------------------------------------------------------------------------
   18. Enrollment
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Enrollment (sessionID, traineeID, enrollmentStatusID, enrollmentDate, statusChangedAt, dropReason, createdAt, updatedAt) VALUES
    (1,  1, 3, '2025-12-01 10:00:00', '2026-01-13 18:00:00', NULL, '2025-12-01 10:00:00', '2026-01-13 18:00:00'),
    (1,  2, 3, '2025-12-02 10:00:00', '2026-01-13 18:00:00', NULL, '2025-12-02 10:00:00', '2026-01-13 18:00:00'), 
    (1,  3, 3, '2025-12-05 10:00:00', '2026-01-13 18:00:00', NULL, '2025-12-05 10:00:00', '2026-01-13 18:00:00'), 
    (1,  4, 3, '2025-12-10 10:00:00', '2026-01-13 18:00:00', NULL, '2025-12-10 10:00:00', '2026-01-13 18:00:00'), 
    (2,  5, 3, '2026-01-05 10:00:00', '2026-02-09 18:00:00', NULL,                                     '2026-01-05 10:00:00', '2026-02-09 18:00:00'), 
    (2,  6, 3, '2026-01-07 10:00:00', '2026-02-09 18:00:00', NULL,                                     '2026-01-07 10:00:00', '2026-02-09 18:00:00'), 
    (2,  7, 4, '2026-01-09 10:00:00', '2026-02-08 12:00:00', N'Withdrew due to scheduling conflict.',  '2026-01-09 10:00:00', '2026-02-08 12:00:00'), 
    (3,  1, 3, '2026-01-15 10:00:00', '2026-02-25 18:00:00', NULL, '2026-01-15 10:00:00', '2026-02-25 18:00:00'), 
    (3,  3, 3, '2026-01-15 10:00:00', '2026-02-25 18:00:00', NULL, '2026-01-15 10:00:00', '2026-02-25 18:00:00'), 
    (3,  5, 3, '2026-01-18 10:00:00', '2026-02-25 18:00:00', NULL, '2026-01-18 10:00:00', '2026-02-25 18:00:00'), 
    (3,  8, 3, '2026-01-20 10:00:00', '2026-02-25 18:00:00', NULL, '2026-01-20 10:00:00', '2026-02-25 18:00:00'), 
    (4,  2, 3, '2026-02-05 10:00:00', '2026-03-17 18:00:00', NULL, '2026-02-05 10:00:00', '2026-03-17 18:00:00'), 
    (4,  4, 3, '2026-02-06 10:00:00', '2026-03-17 18:00:00', NULL, '2026-02-06 10:00:00', '2026-03-17 18:00:00'), 
    (4,  9, 3, '2026-02-10 10:00:00', '2026-03-17 18:00:00', NULL, '2026-02-10 10:00:00', '2026-03-17 18:00:00'), 
    (5,  6, 5, '2026-03-05 10:00:00', '2026-04-15 09:00:00', NULL, '2026-03-05 10:00:00', '2026-04-15 09:00:00'), 
    (5,  8, 5, '2026-03-07 10:00:00', '2026-04-15 09:00:00', NULL, '2026-03-07 10:00:00', '2026-04-15 09:00:00'), 
    (5, 10, 5, '2026-03-10 10:00:00', '2026-04-15 09:00:00', NULL, '2026-03-10 10:00:00', '2026-04-15 09:00:00'), 
    (6,  7, 2, '2026-03-15 10:00:00', '2026-03-20 10:00:00', NULL, '2026-03-15 10:00:00', '2026-03-20 10:00:00'), 
    (6,  9, 2, '2026-03-18 10:00:00', '2026-03-22 10:00:00', NULL, '2026-03-18 10:00:00', '2026-03-22 10:00:00'), 
    (7,  1, 1, '2026-04-01 10:00:00', '2026-04-01 10:00:00', NULL, '2026-04-01 10:00:00', '2026-04-01 10:00:00'), 
    (7,  2, 1, '2026-04-02 10:00:00', '2026-04-02 10:00:00', NULL, '2026-04-02 10:00:00', '2026-04-02 10:00:00'), 
    (8,  1, 1, '2026-04-03 10:00:00', '2026-04-03 10:00:00', NULL, '2026-04-03 10:00:00', '2026-04-03 10:00:00'), 
    (8,  5, 1, '2026-04-04 10:00:00', '2026-04-04 10:00:00', NULL, '2026-04-04 10:00:00', '2026-04-04 10:00:00'), 
    (10, 2, 3, '2026-02-20 10:00:00', '2026-04-08 18:00:00', NULL, '2026-02-20 10:00:00', '2026-04-08 18:00:00'), 
    (10, 4, 3, '2026-02-22 10:00:00', '2026-04-08 18:00:00', NULL, '2026-02-22 10:00:00', '2026-04-08 18:00:00'); 

/* -----------------------------------------------------------------------------
   19. Waitlist
   ----------------------------------------------------------------------------- */
UPDATE dbo.Course_Session SET maxCapacity = 3 WHERE sessionID = 2;

INSERT INTO dbo.Waitlist (sessionID, traineeID, position, addedAt, status, expiresAt) VALUES
    (1,  5, 1, '2025-12-12 10:00:00', N'Expired', '2026-01-10 00:00:00'), 
    (1,  6, 2, '2025-12-14 10:00:00', N'Expired', '2026-01-10 00:00:00'),
    (1,  9, 3, '2025-12-15 10:00:00', N'Expired', '2026-01-10 00:00:00'),
    (2,  8, 1, '2026-01-20 10:00:00', N'Expired', '2026-02-05 00:00:00'),  
    (2, 10, 2, '2026-01-22 10:00:00', N'Expired', '2026-02-05 00:00:00');

/* -----------------------------------------------------------------------------
   20. Trainee_Course_Completion
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Trainee_Course_Completion (traineeID, courseID, sessionID, completionDate, result) VALUES
    (1, 1, 1, '2026-01-13 18:00:00', N'Pass'),
    (2, 1, 1, '2026-01-13 18:00:00', N'Pass'),
    (3, 1, 1, '2026-01-13 18:00:00', N'Pass'),
    (4, 1, 1, '2026-01-13 18:00:00', N'Pass'),
    (5, 2, 2, '2026-02-09 18:00:00', N'Pass'),
    (6, 2, 2, '2026-02-09 18:00:00', N'Pass'),
    (1, 3, 3, '2026-02-25 18:00:00', N'Pass'),
    (3, 3, 3, '2026-02-25 18:00:00', N'Pass'),
    (5, 3, 3, '2026-02-25 18:00:00', N'Pass'),
    (8, 3, 3, '2026-02-25 18:00:00', N'Pass'),
    (2, 4, 4, '2026-03-17 18:00:00', N'Pass'),
    (4, 4, 4, '2026-03-17 18:00:00', N'Pass'),
    (2, 10, 10, '2026-04-08 18:00:00', N'Pass'),
    (4, 10, 10, '2026-04-08 18:00:00', N'Pass');

/* -----------------------------------------------------------------------------
   21. Trainee_Certification
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Trainee_Certification (traineeID, certificationID, status, eligibleDate, certificateIssuedDate, certificateNumber, expiryDate) VALUES
    (2, 3, N'Issued',      '2026-04-08 18:00:00', '2026-04-10 10:00:00', N'CERT-2026-0001', '2028-04-10 10:00:00'),
    (4, 3, N'Issued',      '2026-04-08 18:00:00', '2026-04-10 10:00:00', N'CERT-2026-0002', '2028-04-10 10:00:00'),
    (1, 1, N'In Progress', NULL,                  NULL,                   NULL,              NULL),
    (1, 2, N'In Progress', NULL,                  NULL,                   NULL,              NULL);

/* -----------------------------------------------------------------------------
   22. Payment_Record
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Payment_Record (enrollmentID, coordinatorID, statusID, totalAmount, dueDate, issuedDate, createdAt, updatedAt) VALUES
    ( 1, 1, 3, 120.00, '2025-12-08 00:00:00', '2025-12-01 10:00:00', '2025-12-01 10:00:00', '2025-12-05 10:00:00'),  
    ( 2, 1, 3, 120.00, '2025-12-09 00:00:00', '2025-12-02 10:00:00', '2025-12-02 10:00:00', '2025-12-06 10:00:00'),  
    ( 3, 1, 3, 120.00, '2025-12-12 00:00:00', '2025-12-05 10:00:00', '2025-12-05 10:00:00', '2025-12-09 10:00:00'),  
    ( 4, 1, 3, 120.00, '2025-12-17 00:00:00', '2025-12-10 10:00:00', '2025-12-10 10:00:00', '2025-12-12 10:00:00'),  
    ( 5, 2, 3,  80.00, '2026-01-12 00:00:00', '2026-01-05 10:00:00', '2026-01-05 10:00:00', '2026-01-08 10:00:00'),  
    ( 6, 2, 2,  80.00, '2026-01-14 00:00:00', '2026-01-07 10:00:00', '2026-01-07 10:00:00', '2026-01-10 10:00:00'),  
    ( 7, 2, 1,  80.00, '2026-01-16 00:00:00', '2026-01-09 10:00:00', '2026-01-09 10:00:00', '2026-01-09 10:00:00'),  
    ( 8, 1, 3, 220.00, '2026-01-22 00:00:00', '2026-01-15 10:00:00', '2026-01-15 10:00:00', '2026-01-18 10:00:00'),  
    ( 9, 1, 3, 220.00, '2026-01-22 00:00:00', '2026-01-15 10:00:00', '2026-01-15 10:00:00', '2026-01-19 10:00:00'),  
    (10, 1, 2, 220.00, '2026-01-25 00:00:00', '2026-01-18 10:00:00', '2026-01-18 10:00:00', '2026-02-01 10:00:00'),  
    (11, 1, 3, 220.00, '2026-01-27 00:00:00', '2026-01-20 10:00:00', '2026-01-20 10:00:00', '2026-01-24 10:00:00'),  
    (12, 2, 3, 180.00, '2026-02-12 00:00:00', '2026-02-05 10:00:00', '2026-02-05 10:00:00', '2026-02-08 10:00:00'),  
    (13, 2, 3, 180.00, '2026-02-13 00:00:00', '2026-02-06 10:00:00', '2026-02-06 10:00:00', '2026-02-09 10:00:00'),  
    (14, 2, 4, 180.00, '2026-02-17 00:00:00', '2026-02-10 10:00:00', '2026-02-10 10:00:00', '2026-03-01 10:00:00'),  
    (15, 1, 3, 200.00, '2026-03-12 00:00:00', '2026-03-05 10:00:00', '2026-03-05 10:00:00', '2026-03-10 10:00:00'),  
    (16, 1, 2, 200.00, '2026-03-14 00:00:00', '2026-03-07 10:00:00', '2026-03-07 10:00:00', '2026-03-12 10:00:00'),  
    (17, 1, 1, 200.00, '2026-03-17 00:00:00', '2026-03-10 10:00:00', '2026-03-10 10:00:00', '2026-03-10 10:00:00'),  
    (18, 2, 1, 230.00, '2026-03-22 00:00:00', '2026-03-15 10:00:00', '2026-03-15 10:00:00', '2026-03-15 10:00:00'),  
    (19, 2, 3, 230.00, '2026-03-25 00:00:00', '2026-03-18 10:00:00', '2026-03-18 10:00:00', '2026-03-22 10:00:00'),  
    (20, 1, 3, 320.00, '2026-04-08 00:00:00', '2026-04-01 10:00:00', '2026-04-01 10:00:00', '2026-04-05 10:00:00'),  
    (21, 1, 1, 320.00, '2026-04-09 00:00:00', '2026-04-02 10:00:00', '2026-04-02 10:00:00', '2026-04-02 10:00:00'),  
    (22, 2, 2, 300.00, '2026-04-10 00:00:00', '2026-04-03 10:00:00', '2026-04-03 10:00:00', '2026-04-08 10:00:00'),  
    (23, 2, 3, 300.00, '2026-04-11 00:00:00', '2026-04-04 10:00:00', '2026-04-04 10:00:00', '2026-04-09 10:00:00'),  
    (24, 2, 3, 400.00, '2026-02-27 00:00:00', '2026-02-20 10:00:00', '2026-02-20 10:00:00', '2026-03-01 10:00:00'),  
    (25, 2, 3, 400.00, '2026-03-01 00:00:00', '2026-02-22 10:00:00', '2026-02-22 10:00:00', '2026-03-02 10:00:00');  

/* -----------------------------------------------------------------------------
   23. Payment_Transaction
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Payment_Transaction (paymentRecordID, coordinatorID, amount, paymentMethod, paymentDate, notes, createdAt) VALUES
    ( 1, 1, 120.00, N'Credit Card',   '2025-12-03 11:00:00', N'Full payment received.',       '2025-12-03 11:00:00'),
    ( 2, 1, 120.00, N'Bank Transfer', '2025-12-04 11:00:00', N'Full payment received.',       '2025-12-04 11:00:00'),
    ( 3, 1, 120.00, N'Cash',          '2025-12-07 11:00:00', N'Paid at reception.',           '2025-12-07 11:00:00'),
    ( 4, 1, 120.00, N'Credit Card',   '2025-12-11 11:00:00', N'Full payment received.',       '2025-12-11 11:00:00'),
    ( 5, 2,  80.00, N'Bank Transfer', '2026-01-06 11:00:00', NULL,                            '2026-01-06 11:00:00'),
    ( 6, 2,  40.00, N'Cash',          '2026-01-08 11:00:00', N'Partial payment; balance due.', '2026-01-08 11:00:00'),
    ( 8, 1, 220.00, N'Bank Transfer', '2026-01-17 11:00:00', NULL,                            '2026-01-17 11:00:00'),
    ( 9, 1, 220.00, N'Credit Card',   '2026-01-18 11:00:00', NULL,                            '2026-01-18 11:00:00'),
    (10, 1,  60.00, N'Cash',          '2026-01-19 11:00:00', N'First installment.',           '2026-01-19 11:00:00'),
    (10, 1,  40.00, N'Cash',          '2026-02-01 11:00:00', N'Second installment.',          '2026-02-01 11:00:00'),
    (11, 1, 220.00, N'Bank Transfer', '2026-01-23 11:00:00', NULL,                            '2026-01-23 11:00:00'),
    (12, 2, 180.00, N'Credit Card',   '2026-02-07 11:00:00', NULL,                            '2026-02-07 11:00:00'),
    (13, 2, 180.00, N'Cheque',        '2026-02-08 11:00:00', N'Cheque cleared.',              '2026-02-08 11:00:00'),
    (15, 1, 200.00, N'Bank Transfer', '2026-03-06 11:00:00', NULL,                            '2026-03-06 11:00:00'),
    (16, 1,  80.00, N'Cash',          '2026-03-09 11:00:00', N'Partial payment.',             '2026-03-09 11:00:00'),
    (19, 2, 230.00, N'Credit Card',   '2026-03-20 11:00:00', NULL,                            '2026-03-20 11:00:00'),
    (20, 1, 320.00, N'Bank Transfer', '2026-04-03 11:00:00', NULL,                            '2026-04-03 11:00:00'),
    (22, 2, 150.00, N'Cash',          '2026-04-06 11:00:00', N'Half paid; remainder by May.', '2026-04-06 11:00:00'),
    (23, 2, 300.00, N'Bank Transfer', '2026-04-07 11:00:00', NULL,                            '2026-04-07 11:00:00'),
    (24, 2, 400.00, N'Credit Card',   '2026-02-25 11:00:00', NULL,                            '2026-02-25 11:00:00'),
    (25, 2, 400.00, N'Credit Card',   '2026-02-26 11:00:00', NULL,                            '2026-02-26 11:00:00');

/* -----------------------------------------------------------------------------
   24. Assessment
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Assessment (enrollmentID, instructorID, result, remarks, assessmentDate, createdAt) VALUES
    ( 1, 1, N'Pass', N'Strong performance throughout the course.',          '2026-01-13 17:00:00', '2026-01-13 18:00:00'),
    ( 2, 1, N'Pass', NULL,                                                   '2026-01-13 17:00:00', '2026-01-13 18:00:00'),
    ( 3, 1, N'Pass', N'Consistent engagement and solid practical work.',    '2026-01-13 17:00:00', '2026-01-13 18:00:00'),
    ( 4, 1, N'Pass', NULL,                                                   '2026-01-13 17:00:00', '2026-01-13 18:00:00'),
    ( 5, 2, N'Pass', NULL,                                                   '2026-02-09 17:00:00', '2026-02-09 18:00:00'),
    ( 6, 2, N'Pass', N'Clear written communication; minor gaps in delivery.','2026-02-09 17:00:00', '2026-02-09 18:00:00'),
    ( 7, 2, N'Fail', N'Did not complete the course; enrollment dropped.',   '2026-02-09 17:00:00', '2026-02-09 18:00:00'),
    ( 8, 4, N'Pass', NULL,                                                   '2026-02-25 17:00:00', '2026-02-25 18:00:00'),
    ( 9, 4, N'Pass', NULL,                                                   '2026-02-25 17:00:00', '2026-02-25 18:00:00'),
    (10, 4, N'Pass', N'Excellent planning exercises.',                      '2026-02-25 17:00:00', '2026-02-25 18:00:00'),
    (11, 4, N'Pass', NULL,                                                   '2026-02-25 17:00:00', '2026-02-25 18:00:00'),
    (12, 1, N'Pass', NULL,                                                   '2026-03-17 17:00:00', '2026-03-17 18:00:00'),
    (13, 1, N'Pass', N'Good grasp of core security concepts.',              '2026-03-17 17:00:00', '2026-03-17 18:00:00'),
    (14, 1, N'Fail', N'Did not meet the minimum passing threshold.',        '2026-03-17 17:00:00', '2026-03-17 18:00:00'),
    (24, 1, N'Pass', N'Impressive penetration testing report.',             '2026-04-08 17:00:00', '2026-04-08 18:00:00'),
    (25, 1, N'Pass', NULL,                                                   '2026-04-08 17:00:00', '2026-04-08 18:00:00');

/* -----------------------------------------------------------------------------
   25. Notification
   ----------------------------------------------------------------------------- */
INSERT INTO dbo.Notification (userID, title, message, type, relatedEntityType, isRead, createdAt, readAt) VALUES
    -- Enrollments
    ( 7, N'Enrollment Confirmed', N'Your enrollment in IT Fundamentals has been confirmed.',      N'Enrollment',    N'Enrollment',           1, '2025-12-01 10:05:00', '2025-12-01 11:00:00'),
    ( 8, N'Enrollment Confirmed', N'Your enrollment in IT Fundamentals has been confirmed.',      N'Enrollment',    N'Enrollment',           1, '2025-12-02 10:05:00', '2025-12-02 12:00:00'),
    (11, N'Enrollment Confirmed', N'Your enrollment in Cybersecurity Essentials has been confirmed.', N'Enrollment', N'Enrollment',           0, '2026-02-05 10:10:00', NULL),
    (13, N'Enrollment Confirmed', N'Your enrollment in Network Administration has been confirmed.', N'Enrollment',  N'Enrollment',           1, '2026-03-18 10:10:00', '2026-03-18 15:30:00'),
    -- Assessments
    ( 7, N'Assessment Result',    N'Your result for IT Fundamentals has been recorded: Pass.',   N'Assessment',    N'Assessment',           1, '2026-01-13 19:00:00', '2026-01-14 08:00:00'),
    ( 8, N'Assessment Result',    N'Your result for IT Fundamentals has been recorded: Pass.',   N'Assessment',    N'Assessment',           1, '2026-01-13 19:00:00', '2026-01-14 09:15:00'),
    (15, N'Assessment Result',    N'Your result for Cybersecurity Essentials has been recorded: Fail.', N'Assessment', N'Assessment',       0, '2026-03-17 19:00:00', NULL),
    ( 8, N'Assessment Result',    N'Your result for Ethical Hacking has been recorded: Pass.',   N'Assessment',    N'Assessment',           1, '2026-04-08 19:00:00', '2026-04-09 08:30:00'),
    -- Certifications
    ( 8, N'Certification Issued', N'Congratulations! You are now eligible for Certified Cybersecurity Analyst.', N'Certification', N'Trainee_Certification', 1, '2026-04-10 10:05:00', '2026-04-10 11:00:00'),
    (10, N'Certification Issued', N'Congratulations! You are now eligible for Certified Cybersecurity Analyst.', N'Certification', N'Trainee_Certification', 0, '2026-04-10 10:05:00', NULL),
    -- Payments
    ( 7, N'Payment Received',     N'Payment of BHD 120.00 received for IT Fundamentals.',        N'Payment',       N'Payment_Record',       1, '2025-12-03 11:05:00', '2025-12-03 12:00:00'),
    (11, N'Payment Overdue',      N'Payment of BHD 180.00 for Cybersecurity Essentials is overdue. Please settle at earliest.', N'Payment', N'Payment_Record', 0, '2026-02-18 09:00:00', NULL),
    (12, N'Payment Received',     N'Partial payment of BHD 40.00 received for Business Communication.', N'Payment', N'Payment_Record',       1, '2026-01-08 11:05:00', '2026-01-08 14:00:00'),
    (11, N'Waitlist Added',       N'You have been added to the waitlist for IT Fundamentals (position 1).', N'Waitlist', N'Waitlist',        1, '2025-12-12 10:05:00', '2025-12-12 15:00:00'),
    (14, N'Waitlist Added',       N'You have been added to the waitlist for Business Communication (position 1).', N'Waitlist', N'Waitlist', 0, '2026-01-20 10:05:00', NULL),
    ( 9, N'Session Cancelled',    N'The Financial Management session on 2026-03-02 has been cancelled.', N'Enrollment', N'Course_Session',   1, '2026-02-20 09:00:00', '2026-02-20 10:30:00');

COMMIT TRANSACTION;
GO

PRINT 'Seed data inserted successfully.';
GO
