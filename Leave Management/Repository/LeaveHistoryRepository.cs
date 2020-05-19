using Leave_Management.Contracts;
using Leave_Management.Data;

namespace Leave_Management.Repository
{
    public class LeaveHistoryRepository : RepositoryBase<LeaveHistory>, ILeaveHistoryRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveHistoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
