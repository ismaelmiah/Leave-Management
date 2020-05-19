using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace Leave_Management.Models
{
    public class LeaveAllocationVM
    {
        public int Id { get; set; }
        [Required]
        public int NumberOfDays { get; set; }
        public DateTime DateCreated { get; set; }
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public IEquatable<SelectListItem> Employees { get; set; }
        public IEquatable<SelectListItem> LeaveTypes { get; set; }
    }
}
