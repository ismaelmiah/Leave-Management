using Leave_Management.Contracts;
using Leave_Management.Data;

namespace Leave_Management.Repository
{
    public class LeaveAllocationRepository : RepositoryBase<LeaveAllocation>, ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
