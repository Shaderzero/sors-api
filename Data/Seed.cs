using Newtonsoft.Json;
using sors.Data;
using sors.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace sors.Data
{
    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedRoles()
        {
            if (!_context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role {Name = "admin"},
                    new Role {Name = "user"},
                    new Role {Name = "riskManager"},
                    new Role {Name = "riskCoordinator"},
                    new Role {Name = "security"},
                };
                _context.AddRange(roles);

                //добавление подразделений
                var departmentsData = System.IO.File.ReadAllText("Data/Seeds/departmentSeedData.json");
                var departments = JsonConvert.DeserializeObject<List<Department>>(departmentsData);
                _context.AddRange(departments);

                _context.SaveChanges();

                //добавление администраторов
                var uitDep = _context.Departments.FirstOrDefault(d => d.Code == 2076160000);
                var rskDep = _context.Departments.FirstOrDefault(d => d.Code == 2076040000);
                var adminRole = _context.Roles.FirstOrDefault(r => r.Name.Equals("admin"));
                if (uitDep != null && rskDep != null && adminRole != null)
                {
                    var drannikov = new Account
                    {
                        Name = "k.drannikov",
                        DepartmentId = rskDep.Id,
                    };
                    var lostuser = new Account
                    {
                        Name = "lostuser",
                        DepartmentId = rskDep.Id,
                    };
                    var ius1 = new Account
                    {
                        Name = "IUSAdmin1",
                        DepartmentId = uitDep.Id
                    };
                    var ius2 = new Account
                    {
                        Name = "IUSAdmin2",
                        DepartmentId = uitDep.Id
                    };
                    var ius3 = new Account
                    {
                        Name = "IUSAdmin3",
                        DepartmentId = uitDep.Id
                    };
                    lostuser.AccountRoles.Add(new AccountRole
                    {
                        RoleId = adminRole.Id,
                        Account = lostuser
                    });
                    drannikov.AccountRoles.Add(new AccountRole
                    {
                        RoleId = adminRole.Id,
                        Account = lostuser
                    });
                    ius1.AccountRoles.Add(new AccountRole
                    {
                        RoleId = adminRole.Id,
                        Account = ius1
                    });
                    ius2.AccountRoles.Add(new AccountRole
                    {
                        RoleId = adminRole.Id,
                        Account = ius2
                    });
                    ius3.AccountRoles.Add(new AccountRole
                    {
                        RoleId = adminRole.Id,
                        Account = ius3
                    });
                    _context.Add(lostuser);
                    _context.Add(ius1);
                    _context.Add(ius2);
                    _context.Add(ius3);

                    _context.SaveChanges();
                }
            }
        }
    }
}