using Leave_Management.Contracts;
using Leave_Management.Data;
using System;
using System.Collections.Generic;

namespace Leave_Management.Repository
{
    public class LeaveTypeRepository : RepositoryBase<LeaveType>, ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public LeaveTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<LeaveType> GetEmployeesByLeaveTypes(int id)
        {
            throw new NotImplementedException();
        }
    }
}
