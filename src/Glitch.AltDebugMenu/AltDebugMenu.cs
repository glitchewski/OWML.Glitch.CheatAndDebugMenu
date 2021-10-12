using System.Linq;
using System.Reflection;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DebuggestMenu
{
    public class AltDebugMenu : ModBehaviour
    {
        private SpawnPoint[] _spawnPoints;
        private int _spawnPointIndex = 0;

        private PlayerSpawner _spawner;
        private bool _isStarted;
        private bool _gui;

        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(AltDebugMenu)}");
            ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
            ModHelper.Events.Subscribe<DebugInputManager>(Events.AfterStart);
            ModHelper.Events.Event += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            if (behaviour is DebugInputManager && ev == Events.AfterStart)
            {
                _isStarted = true;

                GetSpawnPoints();
            }
        }

        public void OnGUI()
        {
            if (!_gui) return;

            GUI.Box(new Rect(10, 10, 200, 20), "Time left:");
            GUI.Box(new Rect(230, 10, 800, 20), $"{((int)TimeLoop.GetSecondsRemaining())/60} : {((int)TimeLoop.GetSecondsRemaining())%60}");

            GUI.Box(new Rect(10, 40, 200, 20), "Warp point:");
            GUI.Box(new Rect(230, 40, 800, 20), $"{_spawnPoints[_spawnPointIndex]} ({_spawnPointIndex + 1}/{_spawnPoints.Length})");
        }

        public void Update()
        {
            if (!_isStarted)
            {
                return;
            }

            if (Keyboard.current[Key.F12].wasPressedThisFrame)
            {
                _gui = !_gui;
            }

            if (Keyboard.current[Key.F1].wasPressedThisFrame)
            {
                GUIMode.SetRenderMode(GUIMode.IsHiddenMode() ? GUIMode.RenderMode.FPS : GUIMode.RenderMode.Hidden);
            }

            if (Keyboard.current[Key.F7].wasPressedThisFrame)
            {
                Locator.GetPlayerTransform().GetComponent<PlayerResources>().ToggleInvincibility();
                Locator.GetDeathManager().ToggleInvincibility();
                Transform shipTransform = Locator.GetShipTransform();
                if (shipTransform)
                {
                    shipTransform.GetComponentInChildren<ShipDamageController>().ToggleInvincibility();
                }
            }

            if (Keyboard.current[Key.LeftBracket].wasPressedThisFrame)
            {
                if (_spawnPointIndex == 0)
                {
                    _spawnPointIndex = _spawnPoints.Length - 1;
                }
                else
                {
                    _spawnPointIndex--;
                }
            }

            if (Keyboard.current[Key.RightBracket].wasPressedThisFrame)
            {
                if (_spawnPointIndex == _spawnPoints.Length - 1)
                {
                    _spawnPointIndex = 0;
                }
                else
                {
                    _spawnPointIndex++;
                }
            }

            if (Keyboard.current[Key.Backslash].wasPressedThisFrame)
            {
                _spawner.DebugWarp(_spawnPoints[_spawnPointIndex]);
            }

            if (Keyboard.current[Key.F2].wasPressedThisFrame)
            {
                if (!Locator.GetPlayerSuit().IsWearingSuit())
                {
                    Locator.GetPlayerSuit().SuitUp();
                }
                else
                {
                    Locator.GetPlayerSuit().RemoveSuit();
                }
            }

            if (Keyboard.current[Key.Backspace].wasPressedThisFrame)
            {
                TimeLoop.SetTimeLoopEnabled(false);
            }
        }

        private void GetSpawnPoints()
        {
            _spawner = GameObject.FindGameObjectWithTag("Player").GetRequiredComponent<PlayerSpawner>();
            var spawnPointsField = typeof(PlayerSpawner)
                .GetField("_spawnList", BindingFlags.NonPublic | BindingFlags.Instance);
            var spawnPoints = spawnPointsField?.GetValue(_spawner) as SpawnPoint[];
            _spawnPoints = spawnPoints.OrderBy(x => x.name).ToArray();

            ModHelper.Console.WriteLine($"Registered {spawnPoints.Length} spawn points");
        }
    }
}
