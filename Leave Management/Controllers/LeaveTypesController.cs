using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leave_Management.Controllers
{
    public class LeaveTypesController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LeaveTypesController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        // GET: LeaveType
        public ActionResult Index()
        {
            var leaveType = _uow.LeaveType.GetAll().ToList();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveType);
            return View(model);
        }

        // GET: LeaveType/Details/5
        public ActionResult Details(int id)
        {
            var model = _uow.LeaveType.GetFirstOrDefault(u => u.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            var models = _mapper.Map<LeaveTypeVM>(model);

            return View(models);
        }

        // GET: LeaveType
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                //Create
                return View(new LeaveTypeVM());
            }
            //Update

            var model = _uow.LeaveType.GetFirstOrDefault(u => u.Id == id);
            var leaveTypeVm = _mapper.Map<LeaveTypeVM>(model);

            if (leaveTypeVm == null)
            {
                return NotFound();
            }
            return View(leaveTypeVm);
        }

        // POST: LeaveType/Create/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (model.Id == 0) //Create
                {

                    var leaveType = _mapper.Map<LeaveType>(model);
                    leaveType.DateCreated = DateTime.Now;
                    _uow.LeaveType.Create(leaveType);
                }
                else // Update
                {
                    var leaveType = _mapper.Map<LeaveType>(model);
                    _uow.LeaveType.Update(leaveType);
                }
                _uow.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }

        // GET: LeaveType/Delete/5
        public ActionResult Delete(int id)
        {

            var model = _uow.LeaveType.GetFirstOrDefault(u => u.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            var models = _mapper.Map<LeaveTypeVM>(model);

            return View(models);
        }

        // POST: LeaveType/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, LeaveTypeVM model)
        {
            try
            {
                var leaveType = _uow.LeaveType.GetFirstOrDefault(u => u.Id == id);
                if (leaveType == null)
                {
                    return NotFound();
                }

                _uow.LeaveType.Delete(leaveType);
                _uow.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}