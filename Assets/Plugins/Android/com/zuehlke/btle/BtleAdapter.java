package com.zuehlke.btle;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCallback;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattDescriptor;
import android.bluetooth.BluetoothGattService;
import android.bluetooth.BluetoothManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Parcelable;
import android.util.Base64;
import android.util.Log;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.UUID;
import java.util.concurrent.atomic.AtomicBoolean;

import com.unity3d.player.UnityPlayer;

public class BtleAdapter {
	public static BtleAdapter _instance;
	public static final String DEBUGTAG = "UnityBtleAdapter";
	public static final String UNITYOBJECT = "BluetoothLEReceiver";
	public static final String ONBLUETOOTHMESSAGE = "OnBluetoothMessage";
	public AtomicBoolean running = new AtomicBoolean();
	private BluetoothAdapter bluetoothAdapter;
	private ArrayList<UUID> serviceUuidList = null;
	private Map<String, BluetoothDevice> deviceMap = null;
	private Map<String, BluetoothGatt> deviceGattMap = null;
	private Map<String, BluetoothGattCharacteristic> deviceCharacteristics; 
	private Activity activity = null;
	
	private BtleAdapter() {
		this.serviceUuidList = new ArrayList<UUID>();
		this.deviceMap = new HashMap<String, BluetoothDevice>();
		this.deviceCharacteristics = new HashMap<String, BluetoothGattCharacteristic>();
		this.deviceGattMap = new HashMap<String, BluetoothGatt>();
	}
	
	public static BtleAdapter getInstance() {
		if (_instance == null) {
			_instance = new BtleAdapter();
		}
		return _instance;
	}
	
    public void setActivity(Activity activity) {
        this.activity = activity;
    }
    
    public Activity getActivity()
    {
    	return this.activity;
    }

	public static void sendMessage(String message) {
		UnityPlayer.UnitySendMessage(UNITYOBJECT,ONBLUETOOTHMESSAGE, message);
	}
	
	public static void sendMessage(String message, String arg1) {
		UnityPlayer.UnitySendMessage(UNITYOBJECT,ONBLUETOOTHMESSAGE, message + "~" + arg1);
	}
	
	public static void sendMessage(String message, String arg1, String arg2) {
		UnityPlayer.UnitySendMessage(UNITYOBJECT,ONBLUETOOTHMESSAGE, message + "~" + arg1 + "~" + arg2);
	}

	public static void sendMessage(String message, String arg1, String arg2, String arg3) {
		UnityPlayer.UnitySendMessage(UNITYOBJECT,ONBLUETOOTHMESSAGE, message + "~" + arg1 + "~" + arg2 + "~" + arg3);
	}
/*
	public static void UnitySend(byte[] data, int length) {

		String message = Base64.encodeToString(
				Arrays.copyOfRange(data, 0, length), 0);
		UnityPlayer.UnitySendMessage("BluetoothLEReceiver", "OnBluetoothData", message);
	}
*/

	private final BroadcastReceiver actionFoundReceiver = new BroadcastReceiver() {
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			if ("android.bluetooth.device.action.UUID".equals(action)) {
			BtleAdapter.this.log("got action_uuid");
				BluetoothDevice device = (BluetoothDevice) intent.getParcelableExtra("android.bluetooth.device.extra.DEVICE");
				Parcelable[] uuidExtra = intent.getParcelableArrayExtra("android.bluetooth.device.extra.UUID");
				for (Parcelable uuidParcel : uuidExtra) {
					UUID uuid = UUID.fromString(uuidParcel.toString());
					BtleAdapter.this.log("checking uuid " + uuid);
					if (BtleAdapter.this.serviceUuidList.contains(uuid)) {
						BtleAdapter.this.sendDiscoveredDevice(device);
					}
				}
			}
		}
	};

	public void log(String message) {
		Log.i(DEBUGTAG, message);
		sendMessage("Log", message);
	}
	
	/*
	 * Initialize the bluetooth low energy adapter in central mode.
	 * Devices in peripheral mode advertise their services, which can only be discovered by central mode devices.
	 */
	public void initialize() {
		Log.d(DEBUGTAG, "initialize");

		this.serviceUuidList.clear();
		this.deviceMap.clear();
		this.deviceGattMap.clear();

		IntentFilter filter = new IntentFilter("android.bluetooth.device.action.FOUND");
		filter.addAction("android.bluetooth.device.action.UUID");
		filter.addAction("android.bluetooth.adapter.action.DISCOVERY_STARTED");
		filter.addAction("android.bluetooth.adapter.action.DISCOVERY_FINISHED");
		this.activity.registerReceiver(this.actionFoundReceiver, filter);

		BluetoothManager bluetoothManager = (BluetoothManager) this.activity.getSystemService("bluetooth");

		this.bluetoothAdapter = bluetoothManager.getAdapter();

		log("Initialized");
		sendMessage("Initialized");
	}

	public void deInitialize() {
		Log.d(DEBUGTAG, "DeInitialize");

		onDestroy();
		log("DeInitialized");
		sendMessage("DeInitialized");
	}
	
	public void onDestroy() {
		if (this.activity != null) {
			this.activity.unregisterReceiver(this.actionFoundReceiver);
			for (BluetoothGatt gatt : this.deviceGattMap.values()) {
				unSubscribeAllCharacteristics(gatt);
				gatt.disconnect();
				gatt.close();
			}
			clearAll();
		}
	}

	private void clearAll() {
		for (BluetoothGatt gatt : this.deviceGattMap.values()) {
			unSubscribeAllCharacteristics(gatt);
			gatt.disconnect();
		}
		this.serviceUuidList.clear();
		this.deviceMap.clear();
		this.deviceCharacteristics.clear();
		this.deviceGattMap.clear();
	}

	private void sendDiscoveredDevice(BluetoothDevice device) {
		String deviceName;
		if (device.getName() != null) {
			deviceName = device.getName();
		}
		else {
			deviceName = "No Name";
		}

		String deviceAddress = device.getAddress();
		if (!this.deviceMap.containsKey(deviceAddress))
		{			
			this.deviceMap.put(device.getAddress(), device);			
		}

		Log.d(DEBUGTAG, "sendDiscoveredDevice: " + deviceName + ", " + deviceAddress);
		sendMessage("DiscoveredPeripheral", deviceAddress, deviceName);
	}

	private BluetoothAdapter.LeScanCallback mLeScanCallback = new BluetoothAdapter.LeScanCallback() {
		public void onLeScan(BluetoothDevice device, int rssi, byte[] scanRecord) {
			BtleAdapter.this.sendDiscoveredDevice(device);
		}
	};

	/*
	 * Scan for peripherals
	 * serviceUUIDsFilter: restrict to the services identified by given UUIDs
	 */
	public void scanForPeripheralsWithServices(String serviceUUIDsFilter) {
		log("start scan");
		if (this.bluetoothAdapter != null) {
			ArrayList<UUID> uuidList = new ArrayList<UUID>();
			if (serviceUUIDsFilter != null) {
				String[] serviceUUIDs = serviceUUIDsFilter.split("|");
				for (String serviceUuid : serviceUUIDs) {
					if (serviceUuid != null && !serviceUuid.isEmpty())
					{
						Log.d(DEBUGTAG, "scanForPeripheralsWithServices ServiceUuid: " + serviceUuid);
						uuidList.add(UUID.fromString(serviceUuid));
					}
				}
			}
			
			Log.d(DEBUGTAG, "startLeScan");
			
			if (uuidList.size() > 0) {
				UUID[] serviceUuids = (UUID[]) uuidList.toArray();
				this.bluetoothAdapter.startLeScan(serviceUuids, this.mLeScanCallback);
			} else {
				this.bluetoothAdapter.startLeScan(this.mLeScanCallback);
			}
		}
	}

	public void stopScan() {
		Log.d(DEBUGTAG, "stopScan");
		if (this.bluetoothAdapter != null) {
			Log.d(DEBUGTAG, "stopLeScan");
			if (this.bluetoothAdapter.getScanMode() != BluetoothAdapter.SCAN_MODE_NONE) {
				this.bluetoothAdapter.stopLeScan(this.mLeScanCallback);
			}
		}
	}

	private final BluetoothGattCallback mGattCallback = new BluetoothGattCallback() 
	{
		public void onConnectionStateChange(BluetoothGatt gatt, int status,	int newState) {
			Log.d(DEBUGTAG, "onConnectionStateChange: " + status);

			String deviceAddress = gatt.getDevice().getAddress();
			if (newState == BluetoothGatt.STATE_CONNECTED) {
				BtleAdapter.this.deviceGattMap.put(deviceAddress, gatt);
				BtleAdapter.this.log("ConnectedPeripheral: " + deviceAddress + ", " + gatt.getDevice().getName());
				BtleAdapter.sendMessage("ConnectedPeripheral", deviceAddress, gatt.getDevice().getName());
				
				// mandatory step to retrieve available services and characteristics
				
				BtleAdapter.this.log("Starting service discovery");
				gatt.discoverServices();
				
			} else if (newState == BluetoothGatt.STATE_DISCONNECTED) {
				clearAll();
				gatt.close();
				BtleAdapter.this.log("DisconnectedPeripheral: " + deviceAddress);
				BtleAdapter.sendMessage("DisconnectedPeripheral", deviceAddress, gatt.getDevice().getName());
			}
		}

		public void onServicesDiscovered(final BluetoothGatt gatt, int status) {
			if (status != BluetoothGatt.GATT_SUCCESS) {
				BtleAdapter.this.log("Error~Service Discovery "	+ status);
			}
			else {
				List<BluetoothGattService> services = gatt.getServices();
				for (BluetoothGattService service : services) {
					BtleAdapter.this.log("Service discovered: "	+ service.getUuid());
					BtleAdapter.sendMessage("DiscoveredService", service.getUuid().toString());
				
					for (final BluetoothGattCharacteristic characteristic : service.getCharacteristics()) {
						String characteristicsUuid = characteristic.getUuid().toString();
						BtleAdapter.this.deviceCharacteristics.remove(characteristicsUuid);
						BtleAdapter.this.deviceCharacteristics.put(characteristicsUuid, characteristic);
						String stringValue = characteristic.getValue() != null ? Base64.encodeToString(characteristic.getValue(), Base64.DEFAULT) : "";
						BtleAdapter.sendMessage("DiscoveredCharacteristic", characteristic.getUuid().toString(), stringValue);
					}
				}
			}
		}

		public void onCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
			if (status == BluetoothGatt.GATT_SUCCESS) {
				String stringValue = characteristic.getValue() != null ? Base64.encodeToString(characteristic.getValue(), Base64.DEFAULT) : "";
				BtleAdapter.sendMessage("DidReadCharacteristic", characteristic.getUuid().toString(), stringValue);
			}
		}
				
		public void onCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
			BtleAdapter.this.log("Characteristic was written: " + characteristic.getUuid());
			if (status == BluetoothGatt.GATT_SUCCESS) {
				BtleAdapter.sendMessage("DidWriteCharacteristic", characteristic.getUuid().toString());
			}
		}
		
		/*
		 * Called by notifications for the given characteristic
		 * */
		public void onCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic) {
			Log.d(DEBUGTAG, "onCharacteristicChanged");
			String stringValue = characteristic.getValue() != null ? Base64.encodeToString(characteristic.getValue(), Base64.DEFAULT) : "";
			BtleAdapter.sendMessage("DidUpdateValueForCharacteristic", characteristic.getUuid().toString(), stringValue);
		}
	};

	/*
	 * Connect device to the given device address
	 * @param deviceAddress the unique device address
	 */
	public void connectToPeripheral(String deviceAddress) {
		if ((this.bluetoothAdapter != null) && (this.activity != null)) {
			Log.d(DEBUGTAG, "connectToPeripheral :" + deviceAddress);
			final BluetoothDevice device = (BluetoothDevice) this.deviceMap.get(deviceAddress);
			stopScan();
			if (device != null) {
				BtleAdapter.this.getActivity().runOnUiThread(new Runnable() {
					public void run() {
						device.connectGatt(BtleAdapter.this.activity, false, BtleAdapter.this.mGattCallback);
					}
				});
			}
		}
	}

	public void disconnectPeripheral(String deviceAddress) {
		if ((this.bluetoothAdapter != null) && (this.activity != null)) {
			Log.d(DEBUGTAG, "disconnectPeripheral :" + deviceAddress);
			BluetoothGatt gatt = getGatt(deviceAddress);
			if (gatt != null)
			{
				gatt.disconnect();
			}
		}
	}
	
	public void readCharacteristic(String deviceAddress, String serviceUuid, String characteristicUuid) {		
		final BluetoothGattCharacteristic characteristic = this.deviceCharacteristics.get(characteristicUuid);
		if (characteristic == null) {
			Log.i(DEBUGTAG, "Characteristic not found: " + characteristicUuid);
			return;
		}
		
		final BluetoothGatt gatt = getGatt(deviceAddress);
		if (gatt == null) {
			Log.i(DEBUGTAG, "GATT device not found: " + deviceAddress);
			return;
		}
		
		BtleAdapter.this.getActivity().runOnUiThread(new Runnable() {
			public void run() {
				gatt.readCharacteristic(characteristic);							}
		});
	}
	
	public void writeCharacteristic(String deviceAddress, String serviceUuid, String characteristicUuid, byte[] data) {
		BluetoothGatt gatt = getGatt(deviceAddress);
		BluetoothGattCharacteristic characteristic = getCharacteristic(characteristicUuid);
		characteristic.setValue(data);
		boolean success = gatt.writeCharacteristic(characteristic);
		if (success) {
			Log.e(DEBUGTAG, "Characteristic " + characteristicUuid + " written to device: ");
		}
	}
	
	public void subscribeCharacteristic(String deviceAddress, String serviceUuid, String characteristicUuid) {
		BluetoothGatt gatt = getGatt(deviceAddress);
		if (gatt == null) {
			Log.e(DEBUGTAG, "Could not find GATT " + deviceAddress);
			return;
		}
		
		BluetoothGattService service = gatt.getService(UUID.fromString(serviceUuid));
		if (service == null) {
			Log.e(DEBUGTAG, "Could not find service " + serviceUuid);
			return;
		}
		
		BluetoothGattCharacteristic characteristic = service.getCharacteristic(UUID.fromString(characteristicUuid));
		if (characteristic == null) {
			Log.e(DEBUGTAG, "Could not find characteristic " + characteristicUuid);
			return;
		}
		
		gatt.setCharacteristicNotification(characteristic, true);
		Log.d(DEBUGTAG, "Subscribed to characteristic " + characteristicUuid);
	}
	
	public void unSubscribeCharacteristic(String deviceAddress, String serviceUuid, String characteristicUuid) {
		BluetoothGatt gatt = getGatt(deviceAddress);
		if (gatt == null) {
			Log.i(DEBUGTAG, "Could not find GATT " + deviceAddress);
			return;
		}
		
		BluetoothGattService service = gatt.getService(UUID.fromString(serviceUuid));
		if (service == null) {
			Log.i(DEBUGTAG, "Could not find service " + serviceUuid);
			return;
		}
		
		BluetoothGattCharacteristic characteristic = service.getCharacteristic(UUID.fromString(characteristicUuid));
		if (characteristic == null) {
			Log.i(DEBUGTAG, "Could not find characteristic " + characteristicUuid);
			return;
		}
		
		gatt.setCharacteristicNotification(characteristic, false);
	}
	
	public void unSubscribeAllCharacteristics(BluetoothGatt gatt)
	{
		for (BluetoothGattService service : gatt.getServices())
		{
			for (BluetoothGattCharacteristic characteristic : service.getCharacteristics())
			{
				gatt.setCharacteristicNotification(characteristic, false);
			}			
		}
	}
	
	public BluetoothGatt getGatt(String deviceAddress) {
		return this.deviceGattMap.get(deviceAddress);
	}
	
	public BluetoothGattCharacteristic getCharacteristic(String characteristicUuid) {
		return this.deviceCharacteristics.get(characteristicUuid);
	}
}
