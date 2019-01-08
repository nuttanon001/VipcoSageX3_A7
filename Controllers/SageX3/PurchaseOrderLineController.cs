using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using VipcoSageX3.Services;
using VipcoSageX3.ViewModels;
using VipcoSageX3.Models.SageX3;

using AutoMapper;
using VipcoSageX3.Helper;


namespace VipcoSageX3.Controllers.SageX3
{
    // : api/PurchaseOrderLine/GetByMaster/
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderLineController : GenericSageX3Controller<Porderp>
    {
        private readonly IRepositorySageX3<Porderq> repositoryLine2;
        public PurchaseOrderLineController(IRepositorySageX3<Porderp> repo,
            IRepositorySageX3<Porderq> repoLine2,
            IMapper mapper) : base(repo, mapper)
        {
            this.repositoryLine2 = repoLine2;
        }


    }
}
