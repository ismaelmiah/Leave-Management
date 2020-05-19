using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        ILeaveTypeRepository LeaveType { get; }
        ILeaveHistoryRepository LeaveHistory { get; }
        ILeaveAllocationRepository LeaveAllocation { get; }
        void Save();
    }
}
