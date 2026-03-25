using System;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nemesis.Modules.Jump
{
    /// <summary>
    /// Enhanced jump using Harmony patches on ProtoActor's jump/grounded system.
    /// Patches grounded check to allow jump triggering, then applies vertical
    /// displacement scaled by configured velocity.
    /// </summary>
    internal class JumpModule : IModule
    {
        public string Name => "Better Jump";

        private readonly JumpConfig _config;
        private float _jumpCooldown;

        public static bool IsEnabled { get; private set; }
        public static float JumpVelocity { get; private set; } = 5.0f;
        // Track if we're in a jump to apply continuous upward force
        public static bool IsJumping { get; set; }
        public static float JumpTimer { get; set; }

        public JumpModule(JumpConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Jump.Msg("Initialized");
        }

        public void Shutdown()
        {
            IsEnabled = false;
            IsJumping = false;
        }

        public void OnUpdate()
        {
            IsEnabled = _config.Enabled;
            JumpVelocity = _config.JumpVelocity;

            if (!_config.Enabled) return;

            _jumpCooldown -= Time.deltaTime;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            try
            {
                var player = PlayerAPI.GetLocalPlayer();
                if (player == null) return;

                // Apply upward movement while jump is active
                if (IsJumping)
                {
                    JumpTimer -= Time.deltaTime;
                    if (JumpTimer <= 0)
                    {
                        IsJumping = false;
                    }
                    else
                    {
                        // Apply upward velocity that decays over the jump duration
                        float t = JumpTimer / 0.3f; // normalized remaining time
                        player.transform.position += Vector3.up * _config.JumpVelocity * t * Time.deltaTime;
                    }
                }

                // Start new jump on space press
                if (keyboard.spaceKey.wasPressedThisFrame && _jumpCooldown <= 0 && player.grounded)
                {
                    IsJumping = true;
                    JumpTimer = 0.3f; // 300ms jump impulse
                    _jumpCooldown = 0.5f;
                }
            }
            catch (Exception ex)
            {
                Log.Jump.Warn($"Jump error: {ex.Message}");
            }
        }

        public void OnGUI() { }
    }
}
