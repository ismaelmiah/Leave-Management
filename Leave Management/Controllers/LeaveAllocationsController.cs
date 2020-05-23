using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ActionResult Index()
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
        public IActionResult Delete(int id)
        {
            var Data = _uow.LeaveType.Get(id);
            if (Data == null)
                return Json(new { success = false, message = "Data Not Found!" });
            _uow.LeaveType.Delete(Data);
            _uow.Save();
            return Json(new { success = true, message = "Delete Operation Successfully" });
        }
        #endregion

        // GET: LeaveAllocation/Details/5
        public ActionResult Details(string id)
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
        public ActionResult SetLeave(int id)
        {
            var leavetypes = _uow.LeaveType.Get(id);
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
                    NumberOfDays = leavetypes.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var leaveallocation = _mapper.Map<LeaveAllocation>(allocation);
                _uow.LeaveAllocation.Create(leaveallocation);
                _uow.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult ListEmployee()
        {
            return View();
        }

        // POST: LeaveAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Edit/5
        public ActionResult Edit(int id)
        {
            var leaveAllocation = _uow.LeaveAllocation
                .GetAllWithTwoEntity((x => x.Id == id), includeProperties: "LeaveType", includeProperty: "Employee");
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveAllocation);
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EditLeaveAllocationVM modeEditLeaveAllocationVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(modeEditLeaveAllocationVm);
                }

                var record = _uow.LeaveAllocation.Get(id);
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

        // POST: LeaveAllocation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}