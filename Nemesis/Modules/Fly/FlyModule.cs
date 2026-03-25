using System;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nemesis.Modules.Fly
{
    internal class FlyModule : IModule
    {
        public string Name => "Fly";

        private readonly FlyConfig _config;

        public FlyModule(FlyConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Fly.Msg("Initialized");
        }

        public void Shutdown() { }

        public void OnUpdate()
        {
            if (!_config.Enabled) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            try
            {
                var player = PlayerAPI.GetLocalPlayer();
                if (player == null) return;

                var cam = Camera.main;
                if (cam == null) return;

                Vector3 direction = Vector3.zero;
                Vector3 forward = cam.transform.forward;
                Vector3 right = cam.transform.right;

                // Flatten forward/right for horizontal movement
                forward.y = 0f;
                forward.Normalize();
                right.y = 0f;
                right.Normalize();

                if (keyboard.wKey.isPressed) direction += forward;
                if (keyboard.sKey.isPressed) direction -= forward;
                if (keyboard.dKey.isPressed) direction += right;
                if (keyboard.aKey.isPressed) direction -= right;
                if (keyboard.spaceKey.isPressed) direction += Vector3.up;
                if (keyboard.leftShiftKey.isPressed) direction -= Vector3.up;

                if (direction.sqrMagnitude > 0.01f)
                {
                    direction.Normalize();
                    player.transform.position += direction * _config.FlySpeed * Time.deltaTime;
                }

                // Prevent fall damage by keeping grounded true via reflection
                ReflectionHelper.SetPropertyValue(player, "grounded", true);
            }
            catch (Exception ex)
            {
                Log.Fly.Warn($"Fly error: {ex.Message}");
            }
        }

        public void OnGUI() { }
    }
}
