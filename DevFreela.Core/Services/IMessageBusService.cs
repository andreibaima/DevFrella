using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFreela.Core.Services
{
    public interface IMessageBusService
    {
        // fila para qual quer publicar a msg, e a mssegaem em bytes
        void Publish(string queue, byte[] message);
    }
}
