using ForgeAir.Core.RDS.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.RDS.Helpers
{
    public class RDSdataSender : IRDS
    {
        StringChunker chunker = new StringChunker();

        public RDSdataSender() {
            Shared.RDSParams.Instance.psChanged += OnPSChanged;
            Shared.RDSParams.Instance.rtChanged += OnRTChanged;
        }

        public async Task SetPS(string text)
        {
            text = VariableCollections.OverrideText.OverrideTextWithVariable(text);
            await foreach (var chunk in chunker.ChunkPS(text))
            {
                Shared.RDSParams.Instance.chunkedPS = chunk;
                Shared.RDSParams.Instance.rdsEncoder.UpdatePS(chunk);
            }
        }

        public async Task SetRT(string text)
        {
            text = VariableCollections.OverrideText.OverrideTextWithVariable(text);
            await foreach (var chunk in chunker.ChunkRT(text))
            {
                Shared.RDSParams.Instance.chunkedRT = chunk;
                Shared.RDSParams.Instance.rdsEncoder.UpdateRT(chunk);
            }
        }


        private void OnPSChanged(object? sender, EventArgs e)
        {
            Task.Run(() => SetPS(Shared.RDSParams.Instance.currentPS));
        }
        private void OnRTChanged(object? sender, EventArgs e)
        {
            Task.Run(() => SetRT(Shared.RDSParams.Instance.currentRT));
        }
    }
}
