using System;
using System.Collections.Generic;

namespace Web_API;

public partial class TraineeCourseCompletion
{
    public int CompletionId { get; set; }

    public int TraineeId { get; set; }

    public int CourseId { get; set; }

    public int SessionId { get; set; }

    public DateTime CompletionDate { get; set; }

    public string Result { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual Trainee Trainee { get; set; } = null!;
}
