using System;
using System.Collections.Generic;
using System.Linq;

namespace Nemesis.Modules.RunMutatorDraft
{
    internal sealed class RunMutatorBridgeState
    {
        public float SpawnMultiplier { get; set; } = 1f;
        public float NoiseMultiplier { get; set; } = 1f;
        public float NoiseDecayMultiplier { get; set; } = 1f;
        public int PreferredWeatherId { get; set; } = 0;
        public string DraftName { get; set; } = "";
        public string[] ActiveMutators { get; set; } = Array.Empty<string>();
    }

    internal static class RunMutatorDraftBridge
    {
        private static readonly object Gate = new object();
        private static RunMutatorBridgeState _state = new RunMutatorBridgeState();

        public static RunMutatorBridgeState Current
        {
            get
            {
                lock (Gate)
                {
                    return Clone(_state);
                }
            }
        }

        public static void Set(RunMutatorBridgeState state)
        {
            if (state == null)
                return;

            lock (Gate)
            {
                _state = Clone(state);
            }
        }

        public static void Reset()
        {
            lock (Gate)
            {
                _state = new RunMutatorBridgeState();
            }
        }

        private static RunMutatorBridgeState Clone(RunMutatorBridgeState state)
        {
            return new RunMutatorBridgeState
            {
                SpawnMultiplier = state.SpawnMultiplier,
                NoiseMultiplier = state.NoiseMultiplier,
                NoiseDecayMultiplier = state.NoiseDecayMultiplier,
                PreferredWeatherId = state.PreferredWeatherId,
                DraftName = state.DraftName,
                ActiveMutators = state.ActiveMutators?.ToArray() ?? Array.Empty<string>()
            };
        }
    }
}
