using Leave_Management.Contracts;
using Leave_Management.Data;
using System;
using System.Linq;

namespace Leave_Management.Repository
{
    public class LeaveAllocationRepository : RepositoryBase<LeaveAllocation>, ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public bool CheckAllocation(int leavetypeid, string employeeid)
        {
            var period = DateTime.Now.Year;
            var r = _db.LeaveAllocations.ToList();
            var result = r.Where(x => x.LeaveTypeId == leavetypeid && x.Period == period && x.EmployeeId == employeeid).Any();
            return result;
        }
    }
}
