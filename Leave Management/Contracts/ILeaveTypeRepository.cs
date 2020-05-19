using Leave_Management.Data;
using System.Collections.Generic;

namespace Leave_Management.Contracts
{
    public interface ILeaveTypeRepository : IRepositoryBase<LeaveType>
    {
        IEnumerable<LeaveType> GetEmployeesByLeaveTypes(int id);
    }
}
