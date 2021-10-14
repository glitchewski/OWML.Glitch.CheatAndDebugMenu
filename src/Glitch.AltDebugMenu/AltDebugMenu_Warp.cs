using Glitch.AltDebugMenu.Models;
using UnityEngine.InputSystem;

namespace Glitch.AltDebugMenu
{
    public partial class AltDebugMenu
    {
        /// <summary>
        /// Handles basic warp, ie. single or modifier keypress
        /// </summary>
        protected void HandleBasicWarp()
        {
            if (GetKeyDown(DebugKeys.WarpComet))
            {
                if (GetKey(DebugKeys.WarpModifier))
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.SunStation));
                }
                else
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.Comet));
                }
            }
            else if (GetKeyDown(DebugKeys.WarpHourglassTwins))
            {
                if (GetKey(DebugKeys.WarpModifier))
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.HourglassTwin_2));
                }
                else
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.HourglassTwin_1));
                }
            }
            else if (GetKeyDown(DebugKeys.WarpTimberHearth))
            {
                if (GetKey(DebugKeys.WarpModifier))
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.TimberHearth_Alt));
                }
                else
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.TimberHearth));
                }
            }
            else if (GetKeyDown(DebugKeys.WarpBrittleHollow))
            {
                _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.BrittleHollow));
            }
            else if (GetKeyDown(DebugKeys.WarpGiantsDeep))
            {
                _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.GasGiant));
            }
            else if (GetKeyDown(DebugKeys.WarpDarkBramble))
            {
                _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.DarkBramble));
            }
            else if (GetKeyDown(DebugKeys.WarpShip))
            {
                _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.Ship));
            }
            else if (GetKeyDown(DebugKeys.WarpQuantumMoon))
            {
                _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.QuantumMoon));
            }
            else if (GetKeyDown(DebugKeys.WarpStranger))
            {
                if (GetKey(DebugKeys.WarpModifier))
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.InvisiblePlanet_Alt));
                }
                else
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.InvisiblePlanet));
                }
            }
            else if (GetKeyDown(DebugKeys.WarpMoon))
            {
                if (GetKey(DebugKeys.WarpModifier))
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.SignalDish));
                }
                else
                {
                    _spawner.DebugWarp(_spawner.GetSpawnPoint(SpawnLocation.LunarLookout));
                }
            }
        }

        /// <summary>
        /// Handles advanced warp, ie. warp zone selector
        /// </summary>
        protected void HandleAdvancedWarp()
        {
            if (GetKeyDown(DebugKeys.AdvancedWarpPrevious))
            {
                if (_spawnPointIndex == 0)
                {
                    _spawnPointIndex = _spawnPoints.Length - 1;
                }
                else
                {
                    _spawnPointIndex--;
                }
                PlayClick();
            }

            if (GetKeyDown(DebugKeys.AdvancedWarpNext))
            {
                if (_spawnPointIndex == _spawnPoints.Length - 1)
                {
                    _spawnPointIndex = 0;
                }
                else
                {
                    _spawnPointIndex++;
                }
                PlayClick();
            }

            if (GetKeyDown(DebugKeys.AdvancedWarpExecute))
            {
                _spawner.DebugWarp(_spawnPoints[_spawnPointIndex]);
            }
        }

        protected void HandleMemoryWarp()
        {
            if (GetKeyDown(DebugKeys.WarpPoint1))
            {
                PlayClick();
                if (GetKey(DebugKeys.WarpPointSetModifier))
                {
                    _warpPoints[0] = MemorizedWarpPoint.GetCurrent();
                }
                else
                {
                    if (_warpPoints[0] != null)
                    {
                        Locator.GetPlayerBody().MoveToRelativeLocation(_warpPoints[0].RelativeData, _warpPoints[0].RigidBody);
                    }
                }
            }
            else if (GetKeyDown(DebugKeys.WarpPoint2))
            {
                PlayClick();
                if (GetKey(DebugKeys.WarpPointSetModifier))
                {
                    _warpPoints[1] = MemorizedWarpPoint.GetCurrent();
                }
                else
                {
                    if (_warpPoints[1] != null)
                    {
                        Locator.GetPlayerBody().MoveToRelativeLocation(_warpPoints[1].RelativeData, _warpPoints[1].RigidBody);
                        
                    }
                }
            }
            else if (GetKeyDown(DebugKeys.WarpPoint3))
            {
                PlayClick();
                if (GetKey(DebugKeys.WarpPointSetModifier))
                {
                    _warpPoints[2] = MemorizedWarpPoint.GetCurrent();
                }
                else
                {
                    if (_warpPoints[2] != null)
                    {
                        Locator.GetPlayerBody().MoveToRelativeLocation(_warpPoints[2].RelativeData, _warpPoints[2].RigidBody);
                        PlayClick();
                    }
                }
            }
        }
    }
}
