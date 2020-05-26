using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationsController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<Employee> _userManager;
        private readonly IMapper _mapper;

        public LeaveAllocationsController(IUnitOfWork uow, UserManager<Employee> userManager, IMapper mapper)
        {
            _uow = uow;
            _userManager = userManager;
            _mapper = mapper;
        }
        // GET: LeaveAllocation
        public IActionResult Index()
        {
            var leaveType = _uow.LeaveType.GetAll().ToList();
            var mappsLeaveTypeVms = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveType);
            var model = new CreateLeaveAllocationVM
            {
                NumberUpdated = 0,
                LeaveTypes = mappsLeaveTypeVms
            };
            return View(model);
        }
        #region API Calls

        public IActionResult GetAll()
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeVm>>(employees);
            return Json(new { data = model });
        }
        // GET: LeaveType/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var leaveType = await _uow.LeaveType.Get(id);
            if (leaveType == null)
                return Json(new { success = false, message = "Data Not Found!" });
            _uow.LeaveType.Delete(leaveType);
            _uow.Save();
            return Json(new { success = true, message = "Delete Operation Successfully" });
        }
        #endregion

        // GET: LeaveAllocation/Details/5
        public IActionResult Details(string id)
        {
            var employees = _mapper.Map<EmployeeVm>(_userManager.FindByIdAsync(id).Result);
            var allocations =
                _mapper.Map<List<LeaveAllocationVm>>(_uow.LeaveAllocation.GetAll(includeProperties: "LeaveType").Where(x => x.EmployeeId == id && x.Period == DateTime.Now.Year)
                    .ToList());
            var model = new ViewLeaveAllocationVM
            {
                Employee = employees,
                LeaveAllocationVms = allocations
            };

            return View(model);
        }

        // GET: LeaveAllocation/Create
        public async Task<ActionResult> SetLeave(int id)
        {
            var leaveTypes = await _uow.LeaveType.Get(id);
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            foreach (var emp in employees)
            {
                if (_uow.LeaveAllocation.CheckAllocation(id, emp.Id))
                    continue;
                var allocation = new LeaveAllocationVm
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leaveTypes.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                _uow.LeaveAllocation.Create(leaveAllocation);
                _uow.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ListEmployee()
        {
            return View();
        }
        public async Task<ActionResult> Edit(int id)
        {
            var leaveAllocation = await _uow.LeaveAllocation
                .GetAllWithTwoEntity((x => x.Id == id), includeProperties: "LeaveType", includeProperty: "Employee");
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveAllocation);
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditLeaveAllocationVM modeEditLeaveAllocationVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(modeEditLeaveAllocationVm);
                }

                var record = await _uow.LeaveAllocation.Get(id);
                record.NumberOfDays = modeEditLeaveAllocationVm.NumberOfDays;
                _uow.LeaveAllocation.Update(record);
                _uow.Save();

                return RedirectToAction(nameof(Details), new { id = modeEditLeaveAllocationVm.EmployeeId });
            }
            catch
            {
                return View();
            }
        }

    }
}