CREATE DATABASE TrainingInstituteDB
ON PRIMARY 
(
    NAME = TrainingInstituteDB_Data,
    FILENAME = 'C:\Users\Ahmad\TrainingInstituteDB.mdf', 
    SIZE = 10MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10%
)
LOG ON 
(
    NAME = TrainingInstituteDB_Log,
    FILENAME = 'C:\Users\Ahmad\TrainingInstituteDB_log.ldf', 
    SIZE = 5MB,
    MAXSIZE = 50MB,
    FILEGROWTH = 5MB
);
GO
CREATE TABLE Enrollment_Status (
                enrollmentStatusID INT IDENTITY NOT NULL,
                status NVARCHAR(30) NOT NULL,
                CONSTRAINT Enrollment_Status_pk PRIMARY KEY (enrollmentStatusID)
)

CREATE TABLE User_Role (
                roleID INT IDENTITY NOT NULL,
                roleName NVARCHAR(20) NOT NULL,
                CONSTRAINT User_Role_pk PRIMARY KEY (roleID)
)

CREATE TABLE Payment_Status (
                statusID INT NOT NULL,
                status NVARCHAR(30) NOT NULL,
                CONSTRAINT Payment_Status_pk PRIMARY KEY (statusID)
)

CREATE TABLE Certification_Track (
                certificationID INT IDENTITY NOT NULL,
                name NVARCHAR(150) NOT NULL,
                description NVARCHAR(1000),
                validityPeriod INT,
                isActive BIT DEFAULT 1 NOT NULL,
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                updatedAt DATETIME NOT NULL,
                CONSTRAINT certificationID PRIMARY KEY (certificationID)
)

CREATE TABLE Classroom (
                classroomID INT IDENTITY NOT NULL,
                name NVARCHAR(100) NOT NULL,
                location NVARCHAR(200) NOT NULL,
                building NVARCHAR(100) NOT NULL,
                floor NVARCHAR(20) NOT NULL,
                capacity INT NOT NULL,
                isActive BIT DEFAULT 1 NOT NULL,
                CONSTRAINT classroomID PRIMARY KEY (classroomID)
)

CREATE TABLE Classroom_Equipment (
                equipmentID INT IDENTITY NOT NULL,
                classroomID INT NOT NULL,
                equipmentType NVARCHAR(50) NOT NULL,
                quantity INT NOT NULL,
                description NVARCHAR(255),
                CONSTRAINT equipmentID PRIMARY KEY (equipmentID, classroomID)
)

CREATE TABLE Subject_Area (
                subjectAreaID INT IDENTITY NOT NULL,
                name NVARCHAR(100) NOT NULL,
                description NVARCHAR(500),
                CONSTRAINT subjectAreaID PRIMARY KEY (subjectAreaID)
)

CREATE TABLE Category (
                categoryID INT IDENTITY NOT NULL,
                name NVARCHAR(100) NOT NULL,
                description NVARCHAR(500),
                Parent_categoryID INT,
                CONSTRAINT categoryID PRIMARY KEY (categoryID)
)

CREATE TABLE Course (
                courseID INT NOT NULL,
                subjectAreaID INT NOT NULL,
                categoryID INT NOT NULL,
                prerequisiteCourseID INT,
                courseCode NVARCHAR(30) NOT NULL,
                title NVARCHAR(150) NOT NULL,
                description NVARCHAR(1000),
                durationHours DECIMAL(5,2) NOT NULL,
                maxCapacity INT NOT NULL,
                enrollmentFee DECIMAL(10,2) NOT NULL,
                equipmentRequirements NVARCHAR(500),
                isActive BIT DEFAULT 1 NOT NULL,
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                updatedAt DATETIME NOT NULL,
                CONSTRAINT courseID PRIMARY KEY (courseID)
)

CREATE TABLE Certification_Required_Course (
                ID INT IDENTITY NOT NULL,
                courseID INT NOT NULL,
                certificationID INT NOT NULL,
                sequenceOrder INT,
                isMandatory BIT DEFAULT 1 NOT NULL,
                CONSTRAINT certReqCourseID PRIMARY KEY (ID)
)
CREATE UNIQUE  NONCLUSTERED INDEX Certification_Required_Course_unique_comb
 ON Certification_Required_Course
 ( courseID, certificationID )


CREATE TABLE AppUser (
                userID INT IDENTITY NOT NULL,
                roleID INT NOT NULL,
                firstName NVARCHAR(25) NOT NULL,
                lastName NVARCHAR(25) NOT NULL,
                email NVARCHAR(100) NOT NULL,
                password NVARCHAR(60),
                phone NVARCHAR(15) NOT NULL,
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                updatedAt DATETIME NOT NULL,
                isActive BIT DEFAULT 1 NOT NULL,
                CONSTRAINT userID PRIMARY KEY (userID)
)

CREATE TABLE Notification (
                notificationID INT IDENTITY NOT NULL,
                userID INT NOT NULL,
                title NVARCHAR(150) NOT NULL,
                message NVARCHAR(1000) NOT NULL,
                type NVARCHAR(50) NOT NULL,
                relatedEntityType NVARCHAR(50),
                isRead BIT DEFAULT 0 NOT NULL,
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                readAt DATETIME,
                CONSTRAINT notificationID PRIMARY KEY (notificationID)
)

CREATE TABLE Coordinator (
                coordinatorID INT IDENTITY NOT NULL,
                userID INT NOT NULL,
                department NVARCHAR(100) NOT NULL,
                CONSTRAINT coordinatorID PRIMARY KEY (coordinatorID)
)

CREATE TABLE Instructor (
                instructorID INT IDENTITY NOT NULL,
                userID INT NOT NULL,
                hireDate DATETIME NOT NULL,
                bio NVARCHAR(500),
                CONSTRAINT instructorID PRIMARY KEY (instructorID)
)

CREATE TABLE Course_Session (
                sessionID INT IDENTITY NOT NULL,
                coordinatorID INT NOT NULL,
                classroomID INT NOT NULL,
                courseID INT NOT NULL,
                instructorID INT NOT NULL,
                sessionDate DATETIME NOT NULL,
                startTime DATETIME NOT NULL,
                endTime DATETIME NOT NULL,
                currentEnrollment INT DEFAULT 0 NOT NULL,
                maxCapacity INT NOT NULL,
                status NVARCHAR(20) NOT NULL,
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                updatedAt DATETIME NOT NULL,
                CONSTRAINT sessionID PRIMARY KEY (sessionID)
)

-- Comment for table [Course_Session]: Scheduled Instance of a Course


CREATE TABLE Instructor_Availability (
                availabilityID INT IDENTITY NOT NULL,
                instructorID INT NOT NULL,
                dayOfWeek INT NOT NULL,
                startTime DATETIME NOT NULL,
                endTime DATETIME NOT NULL,
                effectiveFrom DATETIME NOT NULL,
                effectiveTo DATETIME,
                isRecurring BIT DEFAULT 1 NOT NULL,
                CONSTRAINT availabilityID PRIMARY KEY (availabilityID, instructorID)
)

CREATE TABLE Instructor_Expertise (
                ID INT IDENTITY NOT NULL,
                instructorID INT NOT NULL,
                subjectAreaID INT NOT NULL,
                proficiencyLevel NVARCHAR(20) NOT NULL,
                CONSTRAINT ID PRIMARY KEY (ID)
)
CREATE UNIQUE  NONCLUSTERED INDEX Instructor_Expertise_unique_comb
 ON Instructor_Expertise
 ( instructorID, subjectAreaID )


CREATE TABLE Trainee (
                traineeID INT IDENTITY NOT NULL,
                userID INT NOT NULL,
                dateOfBirth DATETIME NOT NULL,
                address NVARCHAR(50) NOT NULL,
                emergencyContact NVARCHAR(50) NOT NULL,
                CONSTRAINT traineeID PRIMARY KEY (traineeID)
)

CREATE TABLE Waitlist (
                waitlistID INT IDENTITY NOT NULL,
                sessionID INT NOT NULL,
                traineeID INT NOT NULL,
                position INT NOT NULL,
                addedAt DATETIME DEFAULT GETDATE() NOT NULL,
                status NVARCHAR(20) NOT NULL,
                expiresAt DATETIME,
                CONSTRAINT waitlistID PRIMARY KEY (waitlistID)
)

-- Comment for table [Waitlist]: Might Remove later (Probably)

CREATE UNIQUE  NONCLUSTERED INDEX Waitlist_unique_comb
 ON Waitlist
 ( sessionID, traineeID )


CREATE TABLE Trainee_Course_Completion (
                completionID INT IDENTITY NOT NULL,
                traineeID INT NOT NULL,
                courseID INT NOT NULL,
                sessionID INT NOT NULL,
                completionDate DATETIME NOT NULL,
                result NVARCHAR(10) NOT NULL,
                CONSTRAINT completionID PRIMARY KEY (completionID)
)

CREATE TABLE Trainee_Certification (
                traineeCertID INT IDENTITY NOT NULL,
                traineeID INT NOT NULL,
                certificationID INT NOT NULL,
                status NVARCHAR(20) NOT NULL,
                eligibleDate DATETIME,
                certificateIssuedDate DATETIME,
                certificateNumber NVARCHAR(50),
                expiryDate DATETIME,
                CONSTRAINT traineeCertID PRIMARY KEY (traineeCertID)
)
CREATE UNIQUE  NONCLUSTERED INDEX Trainee_Certification_unique_comb
 ON Trainee_Certification
 ( traineeID, certificationID )


CREATE TABLE Enrollment (
                enrollmentID INT IDENTITY NOT NULL,
                sessionID INT NOT NULL,
                traineeID INT NOT NULL,
                enrollmentStatusID INT NOT NULL,
                enrollmentDate DATETIME DEFAULT GETDATE() NOT NULL,
                statusChangedAt DATETIME NOT NULL,
                dropReason NVARCHAR(255),
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                updatedAt DATETIME NOT NULL,
                CONSTRAINT enrollmentID PRIMARY KEY (enrollmentID)
)

CREATE TABLE Payment_Record (
                paymentRecordID INT IDENTITY NOT NULL,
                enrollmentID INT NOT NULL,
                coordinatorID INT NOT NULL,
                statusID INT NOT NULL,
                totalAmount DECIMAL(10,2) NOT NULL,
                dueDate DATETIME NOT NULL,
                issuedDate DATETIME NOT NULL,
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                updatedAt DATETIME NOT NULL,
                CONSTRAINT paymentRecordID PRIMARY KEY (paymentRecordID)
)

CREATE TABLE Payment_Transaction (
                transactionID INT IDENTITY NOT NULL,
                paymentRecordID INT NOT NULL,
                coordinatorID INT NOT NULL,
                amount DECIMAL(10,2) NOT NULL,
                paymentMethod NVARCHAR(20) NOT NULL,
                paymentDate DATETIME DEFAULT GETDATE() NOT NULL,
                notes NVARCHAR(300),
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                CONSTRAINT transactionID PRIMARY KEY (transactionID)
)

CREATE TABLE Assessment (
                assessmentID INT IDENTITY NOT NULL,
                enrollmentID INT NOT NULL,
                instructorID INT NOT NULL,
                result NVARCHAR(10) NOT NULL,
                remarks NVARCHAR(500),
                assessmentDate DATETIME NOT NULL,
                createdAt DATETIME DEFAULT GETDATE() NOT NULL,
                CONSTRAINT assessmentID PRIMARY KEY (assessmentID)
)

ALTER TABLE Enrollment ADD CONSTRAINT Enrollment_Status_Enrollment_fk
FOREIGN KEY (enrollmentStatusID)
REFERENCES Enrollment_Status (enrollmentStatusID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE AppUser ADD CONSTRAINT User_Role_AppUser_fk
FOREIGN KEY (roleID)
REFERENCES User_Role (roleID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Payment_Record ADD CONSTRAINT Payment_Status_Payment_Record_fk
FOREIGN KEY (statusID)
REFERENCES Payment_Status (statusID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Certification_Required_Course ADD CONSTRAINT Certification_Track_Certification_Required_Course_fk
FOREIGN KEY (certificationID)
REFERENCES Certification_Track (certificationID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Trainee_Certification ADD CONSTRAINT Certification_Track_Trainee_Certification_fk
FOREIGN KEY (certificationID)
REFERENCES Certification_Track (certificationID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Classroom_Equipment ADD CONSTRAINT Classroom_Classroom_Equipment_fk
FOREIGN KEY (classroomID)
REFERENCES Classroom (classroomID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Course_Session ADD CONSTRAINT Classroom_Course_Session_fk
FOREIGN KEY (classroomID)
REFERENCES Classroom (classroomID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Course ADD CONSTRAINT Course_Subject_Area_fk
FOREIGN KEY (subjectAreaID)
REFERENCES Subject_Area (subjectAreaID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Instructor_Expertise ADD CONSTRAINT Subject_Area_Instructor_Expertise_fk
FOREIGN KEY (subjectAreaID)
REFERENCES Subject_Area (subjectAreaID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Course ADD CONSTRAINT Category_Course_fk
FOREIGN KEY (categoryID)
REFERENCES Category (categoryID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Category ADD CONSTRAINT Category_Category_fk
FOREIGN KEY (Parent_categoryID)
REFERENCES Category (categoryID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Course_Session ADD CONSTRAINT Course_Course_Session_fk
FOREIGN KEY (courseID)
REFERENCES Course (courseID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Certification_Required_Course ADD CONSTRAINT Course_Certification_Required_Course_fk
FOREIGN KEY (courseID)
REFERENCES Course (courseID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Trainee_Course_Completion ADD CONSTRAINT Course_Trainee_Course_Completion_fk
FOREIGN KEY (courseID)
REFERENCES Course (courseID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Course ADD CONSTRAINT Course_Course_fk
FOREIGN KEY (prerequisiteCourseID)
REFERENCES Course (courseID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Trainee ADD CONSTRAINT User_Trainee_fk
FOREIGN KEY (userID)
REFERENCES AppUser (userID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Instructor ADD CONSTRAINT User_Instructor_fk
FOREIGN KEY (userID)
REFERENCES AppUser (userID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Coordinator ADD CONSTRAINT User_Coordinator_fk
FOREIGN KEY (userID)
REFERENCES AppUser (userID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Notification ADD CONSTRAINT User_Notification_fk
FOREIGN KEY (userID)
REFERENCES AppUser (userID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Course_Session ADD CONSTRAINT Coordinator_Course_Session_fk
FOREIGN KEY (coordinatorID)
REFERENCES Coordinator (coordinatorID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Payment_Record ADD CONSTRAINT Payment_Record_Coordinator_fk
FOREIGN KEY (coordinatorID)
REFERENCES Coordinator (coordinatorID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Payment_Transaction ADD CONSTRAINT Coordinator_Payment_Transaction_fk
FOREIGN KEY (coordinatorID)
REFERENCES Coordinator (coordinatorID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Instructor_Expertise ADD CONSTRAINT Instructor_Instructor_Expertise_fk
FOREIGN KEY (instructorID)
REFERENCES Instructor (instructorID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Instructor_Availability ADD CONSTRAINT Instructor_Instructor_Availability_fk
FOREIGN KEY (instructorID)
REFERENCES Instructor (instructorID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Course_Session ADD CONSTRAINT Instructor_Course_Session_fk
FOREIGN KEY (instructorID)
REFERENCES Instructor (instructorID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Assessment ADD CONSTRAINT Instructor_Assesment_fk
FOREIGN KEY (instructorID)
REFERENCES Instructor (instructorID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Enrollment ADD CONSTRAINT Course_Session_Enrollment_fk
FOREIGN KEY (sessionID)
REFERENCES Course_Session (sessionID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Waitlist ADD CONSTRAINT Course_Session_Waitlist_fk
FOREIGN KEY (sessionID)
REFERENCES Course_Session (sessionID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Enrollment ADD CONSTRAINT Trainee_Enrollment_fk
FOREIGN KEY (traineeID)
REFERENCES Trainee (traineeID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Trainee_Certification ADD CONSTRAINT Trainee_Trainee_Certification_fk
FOREIGN KEY (traineeID)
REFERENCES Trainee (traineeID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Trainee_Course_Completion ADD CONSTRAINT Trainee_Trainee_Course_Completion_fk
FOREIGN KEY (traineeID)
REFERENCES Trainee (traineeID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Waitlist ADD CONSTRAINT Trainee_Waitlist_fk
FOREIGN KEY (traineeID)
REFERENCES Trainee (traineeID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Assessment ADD CONSTRAINT Enrollment_Assesment_fk
FOREIGN KEY (enrollmentID)
REFERENCES Enrollment (enrollmentID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Payment_Record ADD CONSTRAINT Enrollment_Payment_Record_fk
FOREIGN KEY (enrollmentID)
REFERENCES Enrollment (enrollmentID)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE Payment_Transaction ADD CONSTRAINT Payment_Record_Payment_Transaction_fk
FOREIGN KEY (paymentRecordID)
REFERENCES Payment_Record (paymentRecordID)
ON DELETE NO ACTION
ON UPDATE NO ACTION
