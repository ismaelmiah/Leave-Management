using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leave_Management.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<Employee> _userManager;
        private readonly IMapper _mapper;

        public LeaveRequestsController(IUnitOfWork uow, UserManager<Employee> userManager, IMapper mapper)
        {
            _uow = uow;
            _userManager = userManager;
            _mapper = mapper;
        }
        // GET: LeaveRequests
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var leaveRequest = _uow.LeaveRequest.GetAll(includeProperties: "LeaveType", includeProperty: "RequestingEmployee", includeProperte: "ApprovedBy");
            var leaveRequestModel = _mapper.Map<List<LeaveRequestVm>>(leaveRequest);
            var model = new AdminLeaveRequestVm
            {
                TotalRequest = leaveRequestModel.Count,
                ApprovedRequest = leaveRequestModel.Count(x => x.Approved == true),
                PendingRequest = leaveRequestModel.Count(x => x.Approved == null),
                RejectedRequest = leaveRequestModel.Count(x => x.Approved == false),
                LeaveRequest = leaveRequestModel
            };
            return View(model);
        }

        // GET: LeaveRequests/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult MyLeave()
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var employeeid = employee.Id;
            var employeeAllocation = _uow.LeaveAllocation.GetAll(includeProperties: "LeaveType").Where(x => x.EmployeeId == employeeid && x.Period == DateTime.Now.Year)
                .ToList();
            var employeeRequest = _uow.LeaveRequest.GetAll((x => x.RequestingEmployeeId == employeeid), includeProperties: "LeaveType", includeProperty: "RequestingEmployee", includeProperte: "ApprovedBy");
            var employeeAllocationModel = _mapper.Map<List<LeaveAllocationVm>>(employeeAllocation);
            var employeeRequestModel = _mapper.Map<List<LeaveRequestVm>>(employeeRequest);

            var model = new EmployeeLeaveRequestVm
            {
                LeaveAllocations = employeeAllocationModel,
                LeaveRequests = employeeRequestModel
            };

            return View(model);
        }

        public ActionResult CancelRequest(int id)
        {
            var leaveRequest = _uow.LeaveRequest.Get(id);
            leaveRequest.Cancelled = true;
            _uow.LeaveRequest.Update(leaveRequest);
            _uow.Save();
            return RedirectToAction("MyLeave");
        }

        // GET: LeaveRequests/Upsert
        public ActionResult Upsert()
        {
            var leavetypes = _uow.LeaveType.GetAll();
            var leavetypesItem = leavetypes.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            var model = new CreateLeaveRequestVm
            {
                LeaveTypes = leavetypesItem
            };
            return View(model);
        }

        // POST: LeaveRequests/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(CreateLeaveRequestVm collection)
        {
            try
            {
                var startDate = Convert.ToDateTime(collection.StartDate);
                var endDate = Convert.ToDateTime(collection.EndDate);
                var leavetypes = _uow.LeaveType.GetAll();
                var leavetypesItem = leavetypes.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                collection.LeaveTypes = leavetypesItem;
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }

                if (DateTime.Compare(startDate, endDate) > 1)
                {
                    return View(collection);
                }

                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _uow.LeaveAllocation.GetAllWithTwoEntity((x =>
                    x.EmployeeId == employee.Id && x.Period == DateTime.Now.Year &&
                    x.LeaveType.Id == collection.LeaveTypeId));

                int dayRequested = (int)(endDate - startDate).TotalDays;
                if (dayRequested > allocation.NumberOfDays)
                {
                    return View(collection);
                }

                var leaveRequest = new LeaveRequestVm
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId = collection.LeaveTypeId,
                    RequestComments = collection.RequestComments
                };

                var leaverequest = _mapper.Map<LeaveRequest>(leaveRequest);
                _uow.LeaveRequest.Create(leaverequest);
                _uow.Save();
                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {
                return View(collection);
            }
        }

        // GET: LeaveRequests/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequests/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequests/Delete/5
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