using Web_API.Models;

namespace MVC_Frontend.Models
{
    public class EnrollConfirmViewModel
    {
        public CourseSession Session { get; set; } = null!;

        public bool HasPrerequisite { get; set; }
        public bool HasPassedPrerequisite { get; set; }
        public string? PrerequisiteCourseName { get; set; }
        public string? PrerequisiteCourseCode { get; set; }

        public bool IsAlreadyEnrolled { get; set; }
        public bool IsOnWaitlist { get; set; }

        public bool IsFull => Session.CurrentEnrollment >= Session.MaxCapacity;
        public int SeatsLeft => Math.Max(0, Session.MaxCapacity - Session.CurrentEnrollment);

        // True when all gates pass and the trainee can actually enroll
        public bool CanEnroll =>
            !IsAlreadyEnrolled &&
            !IsOnWaitlist &&
            !IsFull &&
            HasPassedPrerequisite &&
            Session.Status?.Status == "Scheduled";
    }
}
