using System.Reflection;
using Glitch.AltDebugMenu.Interfaces;

namespace Glitch.AltDebugMenu.Modules
{
    public class BoosterModule : IModModule
    {
        public float ShipBoostMultiplier { get; private set; } = 1f;
        public float PlayerBoostMultiplier { get; private set; } = 1f;

        private readonly float _shipBoostDefault;
        private readonly float _playerBoostDefault;

        private readonly FieldInfo _fieldMaxTransationalThrust;

        private readonly ThrusterModel _playerThrusterModel;
        private readonly ThrusterModel _shipThrusterModel;

        public BoosterModule()
        {
            _playerThrusterModel = Locator.GetPlayerSuit().GetComponent<JetpackThrusterModel>();
            _shipThrusterModel = Locator.GetShipBody().GetComponent<ShipThrusterModel>();
            _fieldMaxTransationalThrust = typeof(ThrusterModel).GetField("_maxTranslationalThrust", BindingFlags.NonPublic | BindingFlags.Instance);

            _shipBoostDefault = _shipThrusterModel.GetMaxTranslationalThrust();
            _playerBoostDefault = _playerThrusterModel.GetMaxTranslationalThrust();
        }

        public void Update()
        {
            
        }

        public void IncreasePlayerBoost(float amount = 0.5f)
        {
            PlayerBoostMultiplier += amount;
            SetMaxThrust(_playerThrusterModel, PlayerBoostMultiplier * _playerBoostDefault);
        }

        public void DecreasePlayerBoost(float amount = 0.5f)
        {
            PlayerBoostMultiplier -= amount;
            SetMaxThrust(_playerThrusterModel, PlayerBoostMultiplier * _playerBoostDefault);
        }

        public void IncreaseShipBoost(float amount = 0.5f)
        {
            ShipBoostMultiplier += amount;
            SetMaxThrust(_shipThrusterModel, ShipBoostMultiplier * _shipBoostDefault);
        }

        public void DecreaseShipBoost(float amount = 0.5f)
        {
            ShipBoostMultiplier -= amount;
            SetMaxThrust(_shipThrusterModel, ShipBoostMultiplier * _shipBoostDefault);
        }

        private void SetMaxThrust(ThrusterModel thruster, float thrust)
        {
            _fieldMaxTransationalThrust.SetValue(thruster, thrust);
        }
    }
}
