using System.Collections.Generic;
using UnityEngine;

namespace User_Interface
{
    public class TravelMap : MonoBehaviour
    {
        [Header("References")]
        public RectTransform map;                     // The whole map UI
        public List<RectTransform> buttons;           // Buttons inside the map

        [Header("Settings")]
        public float controllerMoveSpeed = 10f;
        public float mouseMoveSpeed = 5f;
        public float mouseInfluence = 0.1f;           // How strongly mouse pulls the map

        private int selectedIndex = 0;
        private bool usingController = false;

        private Vector2 targetMapPos;

        void Update()
        {
            HandleInputModeSwitch();
            
            if (usingController)
                HandleControllerNavigation();
            else
                HandleMouseMovement();

            SmoothMoveMap();
        }

        // ---------------------------------------------------------
        // INPUT MODE SWITCHING
        // ---------------------------------------------------------
        void HandleInputModeSwitch()
        {
            // If joystick is moved, switch to controller mode
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f ||
                Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f)
            {
                usingController = true;
            }

            // If mouse moved, switch to mouse mode
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                usingController = false;
            }
        }

        // ---------------------------------------------------------
        // CONTROLLER MODE
        // ---------------------------------------------------------
        void HandleControllerNavigation()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Only change selection when joystick is pushed strongly
            if (Mathf.Abs(h) > 0.5f || Mathf.Abs(v) > 0.5f)
            {
                Vector2 dir = new Vector2(h, v);

                int best = selectedIndex;
                float bestDot = -1f;

                // Find the button most in the direction of the joystick
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (i == selectedIndex) continue;

                    Vector2 toButton = (buttons[i].anchoredPosition - buttons[selectedIndex].anchoredPosition).normalized;
                    float dot = Vector2.Dot(dir.normalized, toButton);

                    if (dot > bestDot)
                    {
                        bestDot = dot;
                        best = i;
                    }
                }

                selectedIndex = best;
            }

            // Center map on selected button
            targetMapPos = -buttons[selectedIndex].anchoredPosition;
        }

        // ---------------------------------------------------------
        // MOUSE MODE
        // ---------------------------------------------------------
        void HandleMouseMovement()
        {
            Vector2 mouse = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            Vector2 offset = (mouse - screenCenter) * mouseInfluence;

            targetMapPos = map.anchoredPosition + offset;
        }

        // ---------------------------------------------------------
        // SMOOTH MAP MOVEMENT
        // ---------------------------------------------------------
        void SmoothMoveMap()
        {
            float speed = usingController ? controllerMoveSpeed : mouseMoveSpeed;

            map.anchoredPosition = Vector2.Lerp(
                map.anchoredPosition,
                targetMapPos,
                Time.deltaTime * speed
            );
        }
    }
}
