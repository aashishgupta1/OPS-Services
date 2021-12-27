using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using ModuleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHLEComm_Integeration
{
    public class DHLECommIntegeration : ModuleBase
    {
        public DHLECommIntegeration(ILogger logger, ISettings settings) : base(logger, settings)
        {
        }

        public override void Run()
        {
            try
            {
                FetchRate();
            }
            catch(Exception e)
            {
                logger.LogError(e);
            }
        }
        private bool FetchRate()
        {
            bool result = false;

            return result;
        }
    }
}
