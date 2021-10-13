using System.Linq;
using System.Reflection;
using Glitch.AltDebugMenu.Models;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Glitch.AltDebugMenu
{
    public partial class AltDebugMenu : ModBehaviour
    {
        protected OWAudioSource _audioSource;

        protected SpawnPoint[] _spawnPoints;
        protected int _spawnPointIndex = 0;

        protected PlayerSpawner _spawner;
        protected bool _isStarted;
        protected bool _gui;

        protected MemorizedWarpPoint[] _warpPoints = new MemorizedWarpPoint[3];
        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(AltDebugMenu)}", MessageType.Success);
            ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
            ModHelper.Events.Subscribe<DebugInputManager>(Events.AfterStart);
            ModHelper.Events.Subscribe<PlayerAudioController>(Events.AfterStart);
            ModHelper.Events.Event += OnEvent;

            
        }

        private void PlayClick()
        {
            _audioSource.PlayOneShot(AudioType.Menu_ChangeTab);
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            if (behaviour is DebugInputManager && ev == Events.AfterStart)
            {
                _isStarted = true;

                GetSpawnPoints();
            }
            else if (behaviour is PlayerAudioController pac && ev == Events.AfterStart)
            {
                HookAudioSource(pac);
            }
        }

        private void HookAudioSource(PlayerAudioController pac)
        {
            _audioSource = new OWAudioSource();
            var audioSource = typeof(PlayerAudioController)
                .GetField("_oneShotExternalSource", BindingFlags.NonPublic | BindingFlags.Instance);
            _audioSource = audioSource?.GetValue(pac) as OWAudioSource;
            if (_audioSource == null) ModHelper.Console.WriteLine("Unable to hook audio source");
        }

        public void OnGUI()
        {
            if (!_gui) return;

            GUI.Box(new Rect(10, 10, 200, 25), "Time left:");
            GUI.Box(new Rect(230, 10, 800, 25), $"{((int)TimeLoop.GetSecondsRemaining())/60} : {((int)TimeLoop.GetSecondsRemaining())%60}");

            GUI.Box(new Rect(10, 45, 200, 25), "Warp point:");
            GUI.Box(new Rect(230, 45, 800, 25), $"{_spawnPoints[_spawnPointIndex]} ({_spawnPointIndex + 1}/{_spawnPoints.Length})");
        }

        public void Update()
        {
            if (!_isStarted)
            {
                return;
            }

            HandleBasicWarp();
            HandleAdvancedWarp();
            HandleMemoryWarp();


            if (GetKeyDown(DebugKeys.ShowModGui))
            {
                _gui = !_gui;
            }

            if (GetKeyDown(DebugKeys.HideGui))
            {
                GUIMode.SetRenderMode(GUIMode.IsHiddenMode() ? GUIMode.RenderMode.FPS : GUIMode.RenderMode.Hidden);
            }

            if (GetKeyDown(DebugKeys.SuitUp))
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

            if (GetKeyDown(DebugKeys.Invincibility))
            {
                Locator.GetPlayerTransform().GetComponent<PlayerResources>().ToggleInvincibility();
                Locator.GetDeathManager().ToggleInvincibility();
                Transform shipTransform = Locator.GetShipTransform();
                if (shipTransform)
                {
                    shipTransform.GetComponentInChildren<ShipDamageController>().ToggleInvincibility();
                }
            }
        }

        protected void GetSpawnPoints()
        {
            _spawner = GameObject.FindGameObjectWithTag("Player").GetRequiredComponent<PlayerSpawner>();
            var spawnPointsField = typeof(PlayerSpawner)
                .GetField("_spawnList", BindingFlags.NonPublic | BindingFlags.Instance);
            var spawnPoints = spawnPointsField?.GetValue(_spawner) as SpawnPoint[];
            _spawnPoints = spawnPoints.OrderBy(x => x.name).ToArray();

            ModHelper.Console.WriteLine($"Registered {spawnPoints.Length} spawn points", MessageType.Info);
        }

        private bool GetKey(Key keyCode)
        {
            return Keyboard.current[keyCode].IsPressed();
        }

        private bool GetKeyDown(Key keyCode)
        {
            return Keyboard.current[keyCode].wasPressedThisFrame;
        }
    }
}
