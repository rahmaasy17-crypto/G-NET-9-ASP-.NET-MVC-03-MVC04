using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    public class PlanConfigurations : IEntityTypeConfiguration<Plan>
    {
     public void  Configure(EntityTypeBuilder<Plan> builder)
        {
           builder.Property(b=>b.Name).HasColumnType("varchar").HasMaxLength(50);
            builder.Property(b => b.Description).HasMaxLength(200);
            builder.Property(b => b.Price).HasPrecision(10, 2);
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.ToTable(t => {
                t.HasCheckConstraint("PlanDurationCheck", "DurationDays Between 1 and 365");
            });
        }
    }

}
