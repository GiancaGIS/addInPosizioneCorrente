using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using GiancaGISLibrary;
using System;
using System.Windows.Forms;

namespace addInPosizioneCorrente
{
    public class btnPosizione : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private readonly IMxDocument _mxDocument = (IMxDocument)ArcMap.Application.Document;
        private readonly IMap _map = null;
        private readonly IActiveView _activeView = null;
        private IPoint _punto = null;


        public btnPosizione()
        {
            _mxDocument = (IMxDocument)ArcMap.Application.Document;
            _map = _mxDocument.FocusMap;
            _activeView = _mxDocument.ActiveView;
            WindowsLocationManager windowsLocationManager = new WindowsLocationManager();
            windowsLocationManager.PosizioneAgganciataEventHandler += WindowsLocationManager_PosizioneAgganciataEventHandler;
        }

        protected override void OnClick()
        {
            if (_punto == null)
                MessageBox.Show("Posizione non disponibile!\nVerificare i permessi utente", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                IEnvelope currentEnv = _activeView.Extent;
                currentEnv.CenterAt(_punto);
                _activeView.Extent = currentEnv;
                _map.MapScale = 5000;
                IRgbColor spazioColoreRGB = new RgbColor();
                Random random = new Random();
                spazioColoreRGB.Green = random.Next(0, 255);
                spazioColoreRGB.Blue = random.Next(0, 255);
                spazioColoreRGB.Red = random.Next(0, 255);
                
                GiancaGISLibrary.ActiveView activeViewGiancaGIS = new GiancaGISLibrary.ActiveView();
                activeViewGiancaGIS.AggiungiGraphicInMappa(_map, _punto, spazioColoreRGB, spazioColoreRGB, true, esriSimpleMarkerStyle.esriSMSDiamond);
                _activeView.Refresh();
            }
        }

        private void WindowsLocationManager_PosizioneAgganciataEventHandler(object sender, PosizioneAgganciataEventArgs e)
        {
            _punto = e.Punto;
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
