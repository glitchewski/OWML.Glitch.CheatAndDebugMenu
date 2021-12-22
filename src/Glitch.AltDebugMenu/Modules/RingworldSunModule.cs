using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glitch.AltDebugMenu.Interfaces;

namespace Glitch.AltDebugMenu.Modules
{
    public class RingworldSunModule : IModModule
    {
        private RingWorldFlickerController _ringWorldFlickerController;

        private HashSet<OWLight2> _strangerLights = new HashSet<OWLight2>();
        private HashSet<OWEmissiveRenderer> _strangerRenderers = new HashSet<OWEmissiveRenderer>();

        private FieldInfo _lightsField;
        private FieldInfo _renderersField;

        private bool _lightsEnabled = true;

        public RingworldSunModule(RingWorldController ringWorldController)
        {
            var rfcField = typeof(RingWorldController).GetField("_flickerController",
                BindingFlags.NonPublic | BindingFlags.Instance);
            _ringWorldFlickerController = rfcField.GetValue(ringWorldController) as RingWorldFlickerController;
            _lightsField =
                typeof(RingWorldFlickerController).GetField("_lights", BindingFlags.NonPublic | BindingFlags.Instance);
            _renderersField =
                typeof(RingWorldFlickerController).GetField("_renderers",
                    BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void Update()
        {
            if (LoadManager.GetCurrentScene() != OWScene.SolarSystem)
            {
                return;
            }

            // by default, the lights are lit up on init
            if (_lightsEnabled) return;

            // but if we have lights disabled, we need to keep them disabled per frame
            // (we need to override the flickering event if they're currently disabled)
            SetLightsState(_lightsEnabled);
        }

        public void SwitchLights()
        {
            if (LoadManager.GetCurrentScene() != OWScene.SolarSystem)
			{
                return;
			}

            UpdateLightsAndRenderersList();
            _lightsEnabled = !_lightsEnabled;
            SetLightsState(_lightsEnabled);
        }

        private void UpdateLightsAndRenderersList()
        {
            var lights = _lightsField.GetValue(_ringWorldFlickerController) as OWLight2[];
            var renderers = _renderersField.GetValue(_ringWorldFlickerController) as OWEmissiveRenderer[];

            if(lights?.Any() ?? false) lights.ToList().ForEach(x => _strangerLights.Add(x));
            if (renderers?.Any() ?? false) renderers.ToList().ForEach(x => _strangerRenderers.Add(x));
        }

        private void SetLightsState(bool state)
        {
            foreach (var strangerLight in _strangerLights)
            {
                strangerLight.enabled = state;
                strangerLight.GetLight().enabled = state;
            }

            foreach (var owEmissiveRenderer in _strangerRenderers)
            {
                owEmissiveRenderer.enabled = state;
                owEmissiveRenderer.GetRenderer().enabled = state;
            }
        }
    }
}
