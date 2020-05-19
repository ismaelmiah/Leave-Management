﻿using Leave_Management.Contracts;
using Leave_Management.Data;

namespace Leave_Management.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            LeaveType = new LeaveTypeRepository(_db);
            LeaveHistory = new LeaveHistoryRepository(_db);
            LeaveAllocation = new LeaveAllocationRepository(_db);
        }

        public ILeaveTypeRepository LeaveType { get; private set; }
        public ILeaveHistoryRepository LeaveHistory { get; private set; }
        public ILeaveAllocationRepository LeaveAllocation { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
