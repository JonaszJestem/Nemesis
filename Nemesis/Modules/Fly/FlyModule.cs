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
        private const float MinDirectionSqrMagnitude = 0.01f;
        private const float FallbackDownwardDampen = 0.2f;

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

                Vector3 direction = GetDirectionFromInput(keyboard, cam.transform);
                if (direction.sqrMagnitude <= MinDirectionSqrMagnitude)
                    return;

                direction.Normalize();
                Vector3 delta = direction * _config.FlySpeed * Time.deltaTime;
                if (!TryMoveWithCharacterController(player, delta))
                {
                    // Fallback when controller is unavailable; damp downward movement
                    // to reduce clipping through floors.
                    if (delta.y < 0f)
                        delta.y *= FallbackDownwardDampen;
                    player.transform.position += delta;
                }

                // Prevent fall damage by keeping grounded true via reflection
                ReflectionHelper.SetPropertyValue(player, "grounded", true);
            }
            catch (Exception ex)
            {
                Log.Fly.Warn($"Fly error: {ex.Message}");
            }
        }

        private Vector3 GetDirectionFromInput(Keyboard keyboard, Transform cameraTransform)
        {
            Vector3 direction = Vector3.zero;

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Flatten forward/right for horizontal movement.
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            if (keyboard.wKey.isPressed) direction += forward;
            if (keyboard.sKey.isPressed) direction -= forward;
            if (keyboard.dKey.isPressed) direction += right;
            if (keyboard.aKey.isPressed) direction -= right;

            bool ascend = keyboard.eKey.isPressed || (_config.UseSpaceShiftVertical && keyboard.spaceKey.isPressed);
            bool descend = keyboard.qKey.isPressed || (_config.UseSpaceShiftVertical && keyboard.leftShiftKey.isPressed);

            if (ascend) direction += Vector3.up;
            if (descend) direction -= Vector3.up;

            return direction;
        }

        private static bool TryMoveWithCharacterController(Component player, Vector3 delta)
        {
            try
            {
                var characterController = player.GetComponent("CharacterController");
                if (characterController == null)
                    return false;

                ReflectionHelper.InvokeMethod(characterController, "Move", delta);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void OnGUI() { }
    }
}
