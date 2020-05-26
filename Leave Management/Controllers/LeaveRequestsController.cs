using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var leaveRequest = _uow.LeaveRequest.GetAll(includeProperties: "LeaveType",
                includeProperty: "RequestingEmployee", includeProperte: "ApprovedBy");
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
        public async Task<ActionResult> Details(int id)
        {
            var leaveRequest = await _uow.LeaveRequest.GetAllWithThreeEntity((x => x.Id == id), includeProperties: "ApprovedBy", includeProperty: "RequestingEmployee", includeProperte: "LeaveType");
            var model = _mapper.Map<LeaveRequestVm>(leaveRequest);
            return View(model);
        }


        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var leaveRequest = await _uow.LeaveRequest.Get(id);
                var employeeId = leaveRequest.RequestingEmployeeId;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var allocation = await _uow.LeaveAllocation.GetAllWithTwoEntity((x =>
                    x.EmployeeId == employeeId && x.Period == DateTime.Now.Year &&
                    x.LeaveType.Id == leaveTypeId));
                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                //allocation.NumberOfDays -= daysRequested;
                allocation.NumberOfDays = allocation.NumberOfDays - daysRequested;

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                _uow.LeaveRequest.Update(leaveRequest);
                _uow.LeaveAllocation.Update(allocation);
                _uow.Save();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var leaveRequest = await _uow.LeaveRequest.Get(id);
                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                _uow.LeaveRequest.Update(leaveRequest);
                _uow.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }


        public ActionResult MyLeave()
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var employeeId = employee.Id;
            var employeeAllocation = _uow.LeaveAllocation.GetAll(includeProperties: "LeaveType")
                .Where(x => x.EmployeeId == employeeId && x.Period == DateTime.Now.Year)
                .ToList();
            var employeeRequest = _uow.LeaveRequest.GetAll((x => x.RequestingEmployeeId == employeeId),
                includeProperties: "LeaveType", includeProperty: "RequestingEmployee", includeProperte: "ApprovedBy");
            var employeeAllocationModel = _mapper.Map<List<LeaveAllocationVm>>(employeeAllocation);
            var employeeRequestModel = _mapper.Map<List<LeaveRequestVm>>(employeeRequest);

            var model = new EmployeeLeaveRequestVm
            {
                LeaveAllocations = employeeAllocationModel,
                LeaveRequests = employeeRequestModel
            };

            return View(model);
        }

        public async Task<IActionResult> CancelRequest(int id)
        {
            var leaveRequest = await _uow.LeaveRequest.Get(id);
            leaveRequest.Cancelled = true;
            _uow.LeaveRequest.Update(leaveRequest);
            _uow.Save();
            return RedirectToAction("MyLeave");
        }

        // GET: LeaveRequests/Upsert
        public IActionResult Upsert()
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
        public async Task<IActionResult> Upsert(CreateLeaveRequestVm collection)
        {
            try
            {
                var startDate = Convert.ToDateTime(collection.StartDate);
                var endDate = Convert.ToDateTime(collection.EndDate);
                var employee = await _userManager.GetUserAsync(User);

                var allocation = await _uow.LeaveAllocation.GetAllWithTwoEntity((x =>
                    x.EmployeeId == employee.Id && x.Period == DateTime.Now.Year &&
                    x.LeaveType.Id == collection.LeaveTypeId));

                int dayRequested = (int)(endDate - startDate).TotalDays;

                var leaveTypes = _uow.LeaveType.GetAll();
                var leaveTypesItem = leaveTypes.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                collection.LeaveTypes = leaveTypesItem;

                if (allocation == null)
                {
                    ModelState.AddModelError("", "You Have No Days Left");
                }
                if (DateTime.Compare(startDate, endDate) > 1)
                {
                    ModelState.AddModelError("", "Start Date cannot be further in the future than the End Date");
                }
                if (dayRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You Do Not Sufficient Days For This Request");
                }
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }

                var leaveRequestVm = new LeaveRequestVm
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

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestVm);
                _uow.LeaveRequest.Create(leaveRequest);
                _uow.Save();
                return RedirectToAction(nameof(MyLeave));
            }
            catch (Exception)
            {
                return View(collection);
            }
        }

        public IActionResult GetAll()
        {
            var leaveRequest = _uow.LeaveRequest.GetAll(includeProperties: "LeaveType",
                includeProperty: "RequestingEmployee", includeProperte: "ApprovedBy");
            var leaveRequestModel = _mapper.Map<List<LeaveRequestVm>>(leaveRequest);
            var model = new AdminLeaveRequestVm
            {
                TotalRequest = leaveRequestModel.Count,
                ApprovedRequest = leaveRequestModel.Count(x => x.Approved == true),
                PendingRequest = leaveRequestModel.Count(x => x.Approved == null),
                RejectedRequest = leaveRequestModel.Count(x => x.Approved == false),
                LeaveRequest = leaveRequestModel
            };
            return Json(new { data = model });
        }
    }
}