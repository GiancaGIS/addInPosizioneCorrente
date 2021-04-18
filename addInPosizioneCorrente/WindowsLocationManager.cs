using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System;
using System.Device.Location;
using System.Windows.Forms;

namespace addInPosizioneCorrente
{
    public class PosizioneAgganciataEventArgs : EventArgs
    {
        public IPoint Punto { get; set; }
    }
    public class WindowsLocationManager
    {
        public event EventHandler<PosizioneAgganciataEventArgs> PosizioneAgganciataEventHandler;
        private readonly IMxDocument _mxDocument = (IMxDocument)ArcMap.Application.Document;
        private readonly IMap _map = null;
        private readonly ISpatialReferenceFactory3 _srFactory = null;
        private readonly ISpatialReference3 _SRMappa = null;
        private readonly ISpatialReference3 _SR_WGS84 = null;
        private const int _EPSG_WGS84 = 4326;
        private readonly int _EPSG_MAPPA = -9999;

        public WindowsLocationManager()
        {
            _map = _mxDocument.FocusMap;
            _srFactory = new SpatialReferenceEnvironment() as ISpatialReferenceFactory3;
            _EPSG_MAPPA = _map.SpatialReference.FactoryCode;
            _SR_WGS84 = (ISpatialReference3)_srFactory.CreateSpatialReference(_EPSG_WGS84);
            _SRMappa = (ISpatialReference3)_srFactory.CreateSpatialReference(_EPSG_MAPPA);
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            watcher.PositionChanged += Watcher_PositionChanged;
            bool bStarted = watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
            if (!bStarted) MessageBox.Show("GeoCoordinateWatcher timed out all'avvio", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            GeoCoordinate coordinateDiWindows = e.Position.Location;

            IPoint punto = new PointClass()
            {
                X = coordinateDiWindows.Longitude,
                Y = coordinateDiWindows.Latitude,
                SpatialReference = _SR_WGS84
            };

            // Proietto il punto
            if (_EPSG_MAPPA != _EPSG_WGS84) punto.Project(_SRMappa);

            PosizioneAgganciataEventArgs posizioneAgganciataEventArgs = new PosizioneAgganciataEventArgs
            {
                Punto = punto
            };

            PosizioneAgganciataEventHandler?.Invoke(this, posizioneAgganciataEventArgs);
        }
    }
}
