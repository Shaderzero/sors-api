using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using sors.Data.Dto;

namespace sors.Authentication
{
    public interface IUserService
    {
        string GetFullname(string name);
        string GetEmail(string name);
        AccountForListDto FillAccountInfo(AccountForListDto source);

        List<DomainUser> GetDomainUsers();
        void UpdateDomainUsers();
    }
}