﻿// 3rd party
using AutoMapper;
using Newtonsoft.Json;
// Microsoft
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// System
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
// VipcoMaintenance
using VipcoSageX3.Helpers;
using VipcoSageX3.Services;
using Microsoft.EntityFrameworkCore.Query;
using VipcoSageX3.Services.ExcelExportServices;

namespace VipcoSageX3.Controllers.SageX3Extends
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericSageX3ExtendController<Entity> : ControllerBase where Entity : class
    {
        public readonly IRepositorySageX3Extend<Entity> repository;
        public readonly IMapper mapper;
        public readonly JsonSerializerSettings DefaultJsonSettings;
        public readonly IHelperService helperService;

        public GenericSageX3ExtendController(
            IRepositorySageX3Extend<Entity> repo,
            IMapper mapper,
            IHelperService helper,
            JsonSerializerService jsonService)
        {
            this.repository = repo;
            this.mapper = mapper;
            this.DefaultJsonSettings = jsonService.Create();
            this.helperService = helper;
        }

        #region PrivateMethod
        /// <summary>
        /// FindFunc
        /// </summary>
        /// <param name="stringData"></param>
        /// <param name="keyword"></param>

        public Func<string,string,bool> 
            FindFunc = (stringData, keyword) => !string.IsNullOrEmpty(stringData) ? stringData.Trim().ToLower().Contains(keyword) : false;

        #endregion

        // GET: api/controller
        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            var ListData = await this.repository.GetToListAsync(x => x);
            return new JsonResult(ListData, this.DefaultJsonSettings);
        }

        // GET: api/controller/5
        [HttpGet("GetKeyNumber")]
        public virtual async Task<IActionResult> Get(int key)
        {
            var HasData = await this.repository.GetAsync(key);
            return new JsonResult(HasData, this.DefaultJsonSettings);
        }

        // GET: api/controller/5
        [HttpGet("GetKeyString")]
        public virtual async Task<IActionResult> Get(string key)
        {
            return new JsonResult(await this.repository.GetAsync(key), this.DefaultJsonSettings);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] Entity record)
        {
            // Set date for CrateDate Entity
            if (record == null)
                return BadRequest();
            // +7 Hour
            // record = this.helperService.AddHourMethod(record);

            if (record.GetType().GetProperty("CreateDate") != null)
                record.GetType().GetProperty("CreateDate").SetValue(record, DateTime.Now);
            if (await this.repository.AddAsync(record) == null)
                return BadRequest();
            return new JsonResult(record, this.DefaultJsonSettings);
        }

        [HttpPut]
        public virtual async Task<IActionResult> Update(int key, [FromBody] Entity record)
        {
            if (key < 1)
                return BadRequest();
            if (record == null)
                return BadRequest();

            // +7 Hour
            // record = this.helperService.AddHourMethod(record);

            // Set date for CrateDate Entity
            if (record.GetType().GetProperty("ModifyDate") != null)
                record.GetType().GetProperty("ModifyDate").SetValue(record, DateTime.Now);
            if (await this.repository.UpdateAsync(record, key) == null)
                return BadRequest();
            return new JsonResult(record, this.DefaultJsonSettings);
        }

        [HttpDelete()]
        public virtual async Task<IActionResult> Delete(int key)
        {
            if (await this.repository.DeleteAsync(key) == 0)
                return BadRequest();
            return NoContent();
        }
    }
}
