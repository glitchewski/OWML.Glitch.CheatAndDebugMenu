namespace Glitch.AltDebugMenu.Models
{
    public class MemorizedWarpPoint
    {
        public OWRigidbody RigidBody { get; set; }
        public RelativeLocationData RelativeData { get; set; }

        public MemorizedWarpPoint(OWRigidbody rigidBody, RelativeLocationData relativeData)
        {
            RigidBody = rigidBody;
            RelativeData = relativeData;
        }

        public static MemorizedWarpPoint GetCurrent()
        {
            var rigidBody = Locator.GetPlayerSectorDetector().GetLastEnteredSector().GetOWRigidbody();
            var relativeData = new RelativeLocationData(Locator.GetPlayerBody(), rigidBody);
            return new MemorizedWarpPoint(rigidBody, relativeData);
        }
    }
}
