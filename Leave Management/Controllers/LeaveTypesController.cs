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

        // GET: LeaveType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveType/Create
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

                var leaveType = _mapper.Map<LeaveType>(model);
                leaveType.DateCreated = DateTime.Now;
                _uow.LeaveType.Create(leaveType);
                _uow.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}