using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Data;

public class TrainingInstituteDBContext : DbContext
{
    public TrainingInstituteDBContext(DbContextOptions<TrainingInstituteDBContext> options)
        : base(options)
    {
    }
}
