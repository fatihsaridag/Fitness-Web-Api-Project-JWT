using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAuthSever.Core.Services
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        void Save();
    }
}
