using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPSaturnEMoney.State
{
    public interface IState
    {
        Task<IState> Execute(CancellationToken cancellationToken);
    }
}
