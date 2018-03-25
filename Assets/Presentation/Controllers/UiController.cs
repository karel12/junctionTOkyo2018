using System.Collections.Generic;
using System.Linq;
using Assets.Core;
using Assets.Model;
using Assets.Plugins;
using Assets.Utils;
using UnityEngine;

namespace Assets.Presentation.Controllers
{
    /// <summary>
    /// Main controller for all UI related tasks.
    /// </summary>
    public class UiController : MonoBehaviour, ILayoutController
    {
        private const string BluetoothLeReceiverGameobjectName = "BluetoothLEReceiver";

        private Sprite[] _gaugeSprites;

        private Xdk _xdk;
        private SettingsController _settingsController;
        private ImprintController _imprintController;
        private BluetoothLeMessageHandler _deviceHandler;
        private BluetoothLeMessageRouter _messageRouter;

        private DeviceListController _deviceListController;

        private List<IUpdateController> _controllers;


        private readonly List<ILayoutChild> _allDialogs = new List<ILayoutChild>();
        private ILayoutChild _visibleDialog;
        private WaitController _waitController;
        private CalibrateController _calibrateController;
        private SetXdk3DModelController _setXdk3DModelController;
        private ProtocolCheckController _protocolCheckController;

        private Xdk3DModelManager _xdk3DModelManager;
        private bool _initialised;

        /// <summary>
        /// Initialize all main components and link them together.
        /// </summary>
        public void Start()
        {
            _xdk = new Xdk();

            _gaugeSprites = Resources.LoadAll<Sprite>("Sprites/assets_200x200");

            CreateDialogControllers();

            IBluetoothLeCommander commander = null;

            if (Application.isMobilePlatform)
            {
#if UNITY_IPHONE
                commander = new BluetoothLEiOsCommander();
#endif
#if UNITY_ANDROID
                commander = new BluetoothLeAndroidCommander();
#endif
            }

            if (commander == null)
            {
                commander = new BluetoothLeCommanderMock();
            }

            _deviceHandler = new BluetoothLeMessageHandler(_xdk, XdkProfile.Create(), commander);
            _deviceListController.DeviceHandler = _deviceHandler;
            _settingsController.DeviceHandler = _deviceHandler;
            _protocolCheckController.DeviceHandler = _deviceHandler;

            var receiver = GameObject.Find(BluetoothLeReceiverGameobjectName) ?? new GameObject(BluetoothLeReceiverGameobjectName);
            _messageRouter = receiver.AddComponent<BluetoothLeMessageRouter>();
            _messageRouter.Handler = _deviceHandler;

            CreateUpdateControllers();

            _xdk3DModelManager.Open();
            _initialised = true;
        }

        private void CreateDialogControllers()
        {
            _settingsController = GameObject.Find("SettingsPanel").GetComponent<SettingsController>();
            _deviceListController = GameObject.Find("DeviceListPanel").GetComponent<DeviceListController>();
            _waitController = GameObject.Find("WaitPanel").GetComponent<WaitController>();
            _calibrateController = GameObject.Find("CalibratePanel").GetComponent<CalibrateController>();
            _setXdk3DModelController = GameObject.Find("SetXdk3DModelPanel").GetComponent<SetXdk3DModelController>();
            _protocolCheckController = GameObject.Find("ProtocolCheckPanel").GetComponent<ProtocolCheckController>();
            _imprintController = GameObject.Find("ImprintPanel").GetComponent<ImprintController>();

            _allDialogs.Add(_settingsController);
            _allDialogs.Add(_deviceListController);
            _allDialogs.Add(_calibrateController);
            _allDialogs.Add(_setXdk3DModelController);
            _allDialogs.Add(_waitController);
            _allDialogs.Add(_protocolCheckController);
            _allDialogs.Add(_imprintController);

            foreach (var layoutChild in _allDialogs)
            {
                layoutChild.LayoutController = this;
            }

            _settingsController.Xdk = _xdk;
            _deviceListController.Xdk = _xdk;
            _waitController.Xdk = _xdk;
            _calibrateController.Xdk = _xdk;
            _setXdk3DModelController.Xdk = _xdk;
            _protocolCheckController.Xdk = _xdk;

            _xdk.OnResetOrientation = _setXdk3DModelController.UpdateOffset;

        }

        private void CreateUpdateControllers()
        {
            _controllers = new List<IUpdateController>();

            _controllers.Add(_calibrateController);
            _controllers.Add(_protocolCheckController);

            _xdk3DModelManager = new Xdk3DModelManager();
            var xdk3DModelControllers = _xdk3DModelManager.CreateControllers(_gaugeSprites, _xdk);

            _setXdk3DModelController.SetXdk3DModelAction = _xdk3DModelManager.SetXdk3DModelByName;
            _setXdk3DModelController.GetXdk3DModelAction = _xdk3DModelManager.GetXdk3DModelByName;

            _controllers.AddRange(xdk3DModelControllers);
        }

        public void ShowSetXdk3ModelDialog()
        {
            VisibleDialog = _setXdk3DModelController;
        }


        /// <summary>
        ///  Update is called once per frame
        /// </summary>
        public void Update()
        {
            if (!_initialised)
            {
                return;
            }
            _deviceHandler.Update();
            _controllers.ForEach(gauge => gauge.UpdateStatus());
        }

        public ILayoutChild VisibleDialog
        {
            get { return _visibleDialog; }
            set
            {
                //ignore request if current dialog is forced on top
                if (_visibleDialog != null && _visibleDialog.ForceOnTop)
                {
                    return;
                }

                if (!ReferenceEquals(_visibleDialog, value))
                {
                    if (_visibleDialog != null)
                    {
                        _visibleDialog.IsEnabled = false;
                    }
                    _visibleDialog = value;

                    if (_visibleDialog != null)
                    {
                        _visibleDialog.IsEnabled = true;
                    }
                }
            }
        }

        public void Show<T>() where T : class, ILayoutChild
        {
            var controllerToShow = _allDialogs.OfType<T>().FirstOrDefault();
            if (controllerToShow != null)
            {
                VisibleDialog = controllerToShow;
            }
        }

        public void OnApplicationQuit()
        {
            if (_deviceHandler != null)
                _deviceHandler.DeInitialize();
        }
    }
}
