using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services
{
    public interface IRepositoryDapperSageX3<Entity> where Entity : class
    {
        Task<List<PurchaseRequestAndOrderViewModel>> GetPurchaseRequestAndOrders(ScrollViewModel scroll);

        Task<List<Entity>> GetListEntites<Parameter>(string SqlCommand, Parameter parameter,int timeout = 60);
        Task<List<Entity>> GetListEntites(string SqlCommand, int timeout = 60);
        Task<ReturnViewModel<Entity>> GetListEntitesAndTotalRow<Parameter>(string SqlCommand, Parameter parameter, int timeout = 60);

    }
}
