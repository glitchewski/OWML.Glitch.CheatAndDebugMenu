using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glitch.AltDebugMenu.Interfaces;
using Glitch.AltDebugMenu.Models;
using Glitch.AltDebugMenu.Modules;
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

        protected bool _collisionShip = true;
        protected bool _collisionPlayer = true;

        private RingworldSunModule _ringworldSunModule;
        private BoosterModule _boosterModule;
        private List<IModModule> _modModules = new List<IModModule>();

        private bool _flashlightPowered = false;
        private Light _l;

        protected MemorizedWarpPoint[] _warpPoints = new MemorizedWarpPoint[3];
        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(AltDebugMenu)}", MessageType.Success);
            ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
            ModHelper.Events.Subscribe<DebugInputManager>(Events.AfterStart);
            ModHelper.Events.Subscribe<PlayerAudioController>(Events.AfterStart);
            ModHelper.Events.Subscribe<RingWorldController>(Events.AfterStart);
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

                _boosterModule = new BoosterModule();

                // ignore please, for future work
                /* var playerBody = Locator.GetPlayerBody();

                var lightGo = new GameObject("Light");
                var light = lightGo.AddComponent<Light>();
                lightGo.transform.parent = playerBody.transform;
                lightGo.transform.position = new Vector3(1f, 1f, 1f);
                _l = light;

                //light.range = 20f;
                light.intensity = 40f;
                //light.type = LightType.Point;*/
            }
            else if (behaviour is PlayerAudioController pac && ev == Events.AfterStart)
            {
                HookAudioSource(pac);

            }
            else if (behaviour is RingWorldController rc && ev == Events.AfterStart)
            {
                _ringworldSunModule = new RingworldSunModule(rc);
                _modModules.Add(_ringworldSunModule);
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

            GUI.Box(new Rect(10, 80, 200, 25), "Boost:");
            GUI.Box(new Rect(230, 80, 800, 25), $"Plr: {_boosterModule.PlayerBoostMultiplier * 100}% / Ship: {_boosterModule.ShipBoostMultiplier * 100}%");
        }

        public void Update()
        {
            // update every initialized module
            _modModules.ForEach(x => x.Update());

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

            if (GetKeyDown(DebugKeys.LearnLaunchCodes))
            {
                PlayerData.LearnLaunchCodes();
            }

            if (GetKeyDown(DebugKeys.RefillResources))
            {
                Locator.GetPlayerTransform().GetComponent<PlayerResources>().DebugRefillResources();
                if (Locator.GetShipTransform())
                {
                    ShipComponent[] componentsInChildren = Locator.GetShipTransform().GetComponentsInChildren<ShipComponent>();
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        componentsInChildren[i].SetDamaged(false);
                    }
                }
            }

            if (GetKeyDown(DebugKeys.ToggleClipping))
            {
                PlayClick();
                if (PlayerState.AtFlightConsole())
                {
                    if (_collisionShip)
                    {
                        _collisionShip = false;
                        Locator.GetShipBody().DisableCollisionDetection();
                    }
                    else
                    {
                        _collisionShip = true;
                        Locator.GetShipBody().EnableCollisionDetection();
                    }
                }
                else
                {
                    if (_collisionPlayer)
                    {
                        _collisionPlayer = false;
                        Locator.GetPlayerBody().DisableCollisionDetection();
                    }
                    else
                    {
                        _collisionPlayer = true;
                        Locator.GetPlayerBody().EnableCollisionDetection();
                    }
                }
            }

            if (GetKeyDown(DebugKeys.DarkenStranger))
            {
                _ringworldSunModule?.SwitchLights();
            }

            if (GetKeyDown(DebugKeys.FastForward))
            {
                Time.timeScale = 20f;
            }
            else if (GetKeyUp(DebugKeys.FastForward))
            {
                Time.timeScale = 1f;
            }

            if (GetKeyDown(DebugKeys.BoostIncrease))
            {
                PlayClick();
                var amount = GetKey(DebugKeys.WarpPointSetModifier) ? 10f : 0.5f;
                if (PlayerState.AtFlightConsole()) _boosterModule.IncreaseShipBoost(amount);
                else _boosterModule.IncreasePlayerBoost(amount);
            }
            else if (GetKeyDown(DebugKeys.BoostDecrease))
            {
                PlayClick();
                var amount = GetKey(DebugKeys.WarpPointSetModifier) ? 10f : 0.5f;
                if (PlayerState.AtFlightConsole()) _boosterModule.DecreaseShipBoost(amount);
                else _boosterModule.DecreasePlayerBoost(amount);
            }

            if (GetKeyDown(DebugKeys.LightenUp))
            {
                _flashlightPowered = !_flashlightPowered;
                foreach (var owLight2 in Locator.GetFlashlight().GetLights())
                {
                    if (_flashlightPowered)
                    {
                        owLight2.GetLight().range *= 5f;
                        owLight2.GetLight().intensity *= 2f;
                    }
                    else
                    {
                        owLight2.GetLight().range /= 5f;
                        owLight2.GetLight().intensity /= 2f;
                    }
                }

                PlayClick();
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

        private bool GetKeyUp(Key keyCode)
        {
            return Keyboard.current[keyCode].wasReleasedThisFrame;
        }
    }
}
