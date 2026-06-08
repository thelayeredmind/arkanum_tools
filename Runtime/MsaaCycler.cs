// SPDX-License-Identifier: MIT
// Cycles the active URP asset's MSAA sample count (Off/2x/4x/8x) on a controller button press.
// Only defines what's being cycled — QuestToolCycler handles the input and reports the
// new value to QuestToolFeedbackDisplay.Instance after each Apply().

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ArkanumQuestTools
{
    [AddComponentMenu("Arkanum Quest Tools/MSAA Cycler")]
    public class MsaaCycler : QuestToolCycler
    {
        static readonly int[] k_SampleCounts = { 1, 2, 4, 8 };
        static readonly string[] k_Options = { "Off", "2x", "4x", "8x" };

        protected override string SettingName => "MSAA";
        protected override string[] Options => k_Options;

        protected override int CurrentIndex
        {
            get
            {
                var asset = UniversalRenderPipeline.asset;
                if (asset == null) return 0;

                int samples = asset.msaaSampleCount;
                for (int i = 0; i < k_SampleCounts.Length; i++)
                    if (k_SampleCounts[i] == samples) return i;
                return 0;
            }
        }

        protected override void Apply(int index)
        {
            var asset = UniversalRenderPipeline.asset;
            if (asset == null) return;

            asset.msaaSampleCount = k_SampleCounts[index];
        }
    }
}
