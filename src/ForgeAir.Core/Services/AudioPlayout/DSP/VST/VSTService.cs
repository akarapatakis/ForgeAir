using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST.Interfaces;

namespace ForgeAir.Core.Services.AudioPlayout.DSP.VST
{
    public class VSTService
    {
        private readonly IVSTService _service;

        public VSTService(IVSTService service)
        {
            _service = service;
        }

        public void Start()
        {
            _service.Initialize();
        }

        public void Stop()
        {
            _service.Kill();
        }

        public void ShowUI(nint hwnd)
        {
            _service.ShowConfigurationWindow(hwnd);
        }
    }

}
