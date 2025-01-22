using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using UnityEngine.Rendering;

namespace _Common
{
    public enum SignalColor
    {
        Green,
        Yellow,
        Red
    }

    public static class MovementFunctions
    {
        public static Vector3 AlignMovementInputToCamera(Vector2 movementInput, Transform cameraTransform)
        {
            float cameraDirection = cameraTransform.eulerAngles.y;

            Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);

            moveDirection = Quaternion.Euler(0f, cameraDirection, 0f) * moveDirection;

            return moveDirection;
        }

        public static Quaternion GetLookRotation(Vector3 moveDirection)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);

            lookRotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);

            return lookRotation;
        }

        public static float GetFallingSpeed(CharacterController controller, bool jumpButtonDown, float fallMultiplier, float lowJumpMultiplier, float gravityMultiplier)
        {
            float fallSpeed;

            if(controller.velocity.y < 0f)
            {
                fallSpeed = (Physics.gravity.y * gravityMultiplier) * (fallMultiplier - 1f) * Time.deltaTime;
            }
            else if(controller.velocity.y > 0f && !jumpButtonDown)
            {
                fallSpeed = (Physics.gravity.y * gravityMultiplier) * (lowJumpMultiplier - 1f) * Time.deltaTime;
            }
            else
            {
                fallSpeed = (Physics.gravity.y * gravityMultiplier) * Time.deltaTime;
            }

            return fallSpeed;
        }

        public static float GetJumpSpeed(float jumpHeight, float gravityMultiplier)
        {
            float jumpSpeed = Mathf.Sqrt(2 * jumpHeight * -(Physics.gravity.y * gravityMultiplier));
            return jumpSpeed;
        }
    }
    

    public static class LevelFunctions
    {
        
        public static float Map (float value, float inputFrom, float inputTo, float outputFrom, float outputTo) {
            return (value - inputFrom) / (inputTo - inputFrom) * (outputTo - outputFrom) + outputFrom;
        }
    }

    public static class PlayerFunctions
    {
        public static Transform GetChildObjectWithTag(Transform parent, string tag)
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>();

            foreach (var child in children)
            {
                if (child.CompareTag(tag))
                {
                    return child;
                }
            }

            return null;
        }

        public static GameObject SpawnSkin(Transform parent, int playerIndex)
        {
            if(PlayerConfigurationManager.Instance == null) return null;
            var desiredSkin = PlayerConfigurationManager.Instance.playerConfigurations[playerIndex].ActivePlayer.PlayerSkin;
            return Object.Instantiate(desiredSkin, parent);
        }

        public static void UpdateSkinName(string name, ref TextMeshProUGUI nameText)
        {
            nameText.text = name;
        }
    }
}