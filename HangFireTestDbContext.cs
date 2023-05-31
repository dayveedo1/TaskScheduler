using Microsoft.EntityFrameworkCore;

namespace HangfireTest
{
    public class HangFireTestDbContext: DbContext
    {
        public HangFireTestDbContext()
        {
                
        }

        public HangFireTestDbContext(DbContextOptions<HangFireTestDbContext> options) : base(options)
        {
                
        }
    }
}
