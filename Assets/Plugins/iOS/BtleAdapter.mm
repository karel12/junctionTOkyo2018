#import "BtleAdapter.h"

extern "C" {
    
    BtleAdapter *_BtleAdapter = nil;
    
    /**
     Add given string message to system log
     */
    void _BtleAdapterLogString (NSString *message)
    {
        NSLog (@"log %@", message);
    }
    
    /**
     Add given char message to system log
     */
    void _BtleAdapterLog (char *message)
    {
        _BtleAdapterLogString ([NSString stringWithFormat:@"%s", message]);
    }
    
    /**
     Initialize the adapter, start central manager.
     */
    void _BtleAdapterInitialize ()
    {
        _BtleAdapter = [BtleAdapter new];
        [_BtleAdapter initialize];
        
        if (_BtleAdapter == nil)
        {
            NSString *outputData = @"_BtleAdapter not initialized!";
            NSLog( @"log: %@", outputData );
        }
    }
    
    /**
     Deinitialize the adapter and clean up.
     */
    void _BtleAdapterDeInitialize ()
    {
        if (_BtleAdapter != nil)
        {
            [_BtleAdapter deInitialize];
            _BtleAdapter = nil;
            
            [_BtleAdapter sendMessageUp:@"DeInitialized"];
        }
    }
    
    /**
     Start discovering peripheral devices.
     */
    void _BtleAdapterScanForPeripheralsWithServices()
    {
        if (_BtleAdapter != nil)
            [_BtleAdapter scanForPeripheralsWithServices];
    }
    
    /**
     Stop peripheral device discovery scan.
     */
    void _BtleAdapterStopScan ()
    {
        if (_BtleAdapter != nil)
            [_BtleAdapter stopScan];
    }
    
    /**
     Connect to the peripheral with the given identifier.
     @param deviceAddress the identifier of the device
     */
    void _BtleAdapterConnectToPeripheral (char *deviceAddress)
    {
        
        if (_BtleAdapter && deviceAddress != nil)
            [_BtleAdapter connectToPeripheral:[NSString stringWithFormat:@"%s", deviceAddress]];
    }
    
    /**
     Disconnect the peripheral with the given identifier.
     @param deviceAddress the identifier of the device
     */
    void _BtleAdapterDisconnectPeripheral (char *deviceAddress)
    {
        
        if (_BtleAdapter && deviceAddress != nil)
            [_BtleAdapter disconnectPeripheral:[NSString stringWithFormat:@"%s", deviceAddress]];
    }
    
    /**
     Request to read a characteristic value from the given peripheral device.
     @param deviceAddress the identifier of the device
     @param service the UUID of the service
     @param characteristic the UUID of the characteristic
     */
    void _BtleAdapterReadCharacteristic (char *deviceAddress, char *service, char *characteristic)
    {
        if (_BtleAdapter && deviceAddress != nil && service != nil && characteristic != nil)
            [_BtleAdapter readCharacteristic:[NSString stringWithFormat:@"%s", deviceAddress] service:[NSString stringWithFormat:@"%s", service] characteristic:[NSString stringWithFormat:@"%s", characteristic]];
    }
    
    /**
     Request to write a characteristic value to the given peripheral device.
     @param deviceAddress the identifier of the device
     @param service the UUID of the service
     @param characteristic the UUID of the characteristic
     @param data the data to write
     @param length length of the data to write
     */
    void _BtleAdapterWriteCharacteristic (char *deviceAddress, char *service, char *characteristic, unsigned char *data, int length)
    {
        if (_BtleAdapter && deviceAddress != nil && service != nil && characteristic != nil && data != nil && length > 0)
            [_BtleAdapter writeCharacteristic:[NSString stringWithFormat:@"%s", deviceAddress] service:[NSString stringWithFormat:@"%s", service] characteristic:[NSString stringWithFormat:@"%s", characteristic] data:[NSData dataWithBytes:data length:length] withResponse:TRUE];
    }
    
    /**
     Subscribe to notification messages of the given characteristic.
     @param deviceAddress the identifier of the device
     @param service the UUID of the service
     @param characteristic the UUID of the characteristic
     */
    void _BtleAdapterSubscribeCharacteristic (char *deviceAddress, char *service, char *characteristic)
    {
        if (_BtleAdapter && deviceAddress != nil && service != nil && characteristic != nil)
            [_BtleAdapter
             subscribeCharacteristic:[NSString stringWithFormat:@"%s", deviceAddress]
             characteristic:[NSString stringWithFormat:@"%s", characteristic]];
    }
    
    /**
     Unsubscribe to notification messages of the given characteristic.
     @param deviceAddress the identifier of the device
     @param service the UUID of the service
     @param characteristic the UUID of the characteristic
     */
    void _BtleAdapterUnSubscribeCharacteristic (char *deviceAddress, char *service, char *characteristic)
    {
        if (_BtleAdapter && deviceAddress != nil && service != nil && characteristic != nil)
            [_BtleAdapter unsubscribeCharacteristic:[NSString stringWithFormat:@"%s", deviceAddress] service:[NSString stringWithFormat:@"%s", service] characteristic:[NSString stringWithFormat:@"%s", characteristic]];
    }
}

@implementation BtleAdapter

@synthesize _peripherals;

- (void)initialize
{
    _centralManager = nil;
    _services = nil;
    _characteristics = nil;
    _centralManagerIsPoweredOn = false;
    
    _centralManager = [[CBCentralManager alloc] initWithDelegate:self queue:nil];
    
    _services = [[NSMutableDictionary alloc] init];
    _characteristics = [[NSMutableDictionary alloc] init];
    _peripherals = [[NSMutableDictionary alloc] init];
    _peripheralCharacteristics = [[NSMutableDictionary alloc] init];
}

- (void)deInitialize
{
    [self removeCharacteristics];
    [self removeServices];
    
    if (_centralManager != nil)
        [self stopScan];
    
    [_peripherals removeAllObjects];
    [_peripheralCharacteristics removeAllObjects];
}

- (void)removeService:(NSString *)uuid
{
    if (_services != nil)
    {
        [_services removeObjectForKey:uuid];
    }
}

- (void)removeServices
{
    if (_services != nil)
    {
        [_services removeAllObjects];
    }
}

- (void)removeCharacteristic:(NSString *)uuid
{
    if (_characteristics != nil)
        [_characteristics removeObjectForKey:uuid];
}

- (void)removeCharacteristics
{
    if (_characteristics != nil)
        [_characteristics removeAllObjects];
}

// central delegate implementation -------------------------------------------------------------------------------

/**
 Invoked when the overall state of the CBCentralManager has changed.
 This is used to wait for the power on event. Scanning for peripheral devices will only work when it is started after this event.
 */
- (void)centralManagerDidUpdateState:(CBCentralManager *)central
{
    NSString *stateMessage = nil;
    switch (central.state)
    {
        case CBCentralManagerStatePoweredOff:
            stateMessage = @"powered off";
            break;
        case CBCentralManagerStatePoweredOn:
            stateMessage = @"powered on";
            _centralManagerIsPoweredOn = true;
            [self sendMessageUp:@"Initialized"];
            break;
        case CBCentralManagerStateResetting:
            stateMessage = @"resetting";
            break;
        case CBCentralManagerStateUnauthorized:
            stateMessage = @"unauthorized";
            break;
        case CBCentralManagerStateUnknown:
            stateMessage = @"unknown";
            break;
        case CBCentralManagerStateUnsupported:
            stateMessage = @"unsupported";
    }
    _BtleAdapterLogString ([NSString stringWithFormat:@"Central State Update: %@", stateMessage]);
}

/**
 Starts the scan for Bluetooth Low Energy devices running in peripheral mode.
 @param serviceUUIDs optional array used to filter all devices by their solicited services.
 */
- (void)scanForPeripheralsWithServices
{
    if (!_centralManagerIsPoweredOn)
    {
        NSLog(@"Unable to scan for peripherals. CentralManager is not yet in powered on state.");
        NSString *message = [NSString stringWithFormat:@"Error~CentralManager is not yet in powered on state"];
        [self sendMessageUp:message];
        return;
    }
    
    if (_centralManager != nil)
    {
        if (_peripherals != nil)
            [_peripherals removeAllObjects];
        
        NSLog(@"calling centralManager scanForPeripheralsWithServices");
        
        NSDictionary *options = [NSDictionary dictionaryWithObjectsAndKeys:[NSNumber numberWithBool:YES], CBCentralManagerScanOptionAllowDuplicatesKey, nil];
        [_centralManager scanForPeripheralsWithServices:nil options:options];
    }
    else
    {
        NSLog(@"CentralManager is nil");
    }
}

- (void) stopScan
{
    if (_centralManager != nil)
        [_centralManager stopScan];
}

/**
 Callback for retrieveListOfPeripheralsWithServices
 */
- (void)centralManager:(CBCentralManager *)central didDiscoverPeripheral:(CBPeripheral *)peripheral advertisementData:(NSDictionary *)advertisementData RSSI:(NSNumber *)RSSI
{
    NSString *deviceName = [advertisementData objectForKey:CBAdvertisementDataLocalNameKey];
    if (_peripherals != nil)
    {
        NSString *deviceAddress = nil;
        
        NSString *foundPeripheral = [self findPeripheralAddress:peripheral];
        if (foundPeripheral == nil)
            deviceAddress = [peripheral.identifier UUIDString];
        else
            deviceAddress = foundPeripheral;
        
        NSString *message = [NSString stringWithFormat:@"DiscoveredPeripheral~%@~%@", deviceAddress, deviceName];
        [self sendMessageUp:message];
        
        [_peripherals setObject:peripheral forKey:deviceAddress];
    }
}

/**
 Requests a connection to the given peripheral device. Callback is didConnectPeripheral.
 @param deviceAddress address of the device to be connected to the system.
 */
- (void)connectToPeripheral:(NSString *)deviceAddress
{
    if (_peripherals != nil && deviceAddress != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:deviceAddress];
        if (peripheral != nil)
            [_centralManager connectPeripheral:peripheral options:nil];
    }
}

/**
 Invoked when a peripheral is successfully connected.
 The service discovery will be started with callback didDiscoverServices.
 */
- (void)centralManager:(CBCentralManager *)central didConnectPeripheral:(CBPeripheral *)peripheral
{
    NSString *foundPeripheral = [self findPeripheralAddress:peripheral];
    if (foundPeripheral != nil)
    {
        [self stopScan];
        NSString *message = [NSString stringWithFormat:@"ConnectedPeripheral~%@", foundPeripheral];
        [self sendMessageUp:message];
        
        peripheral.delegate = self;
        
        [peripheral discoverServices:nil];
    }
}

/**
 Invoked when the connection request to the peripheral failed.
 */
- (void)centralManager:(CBCentralManager *)central didFailToConnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    if (error)
    {
        NSLog(@"error: %@", error.description);
        NSString *message = [NSString stringWithFormat:@"Error~%@", error.description];
        [self sendMessageUp:message];
    }
}

/**
 Invoked when the device services have been discovered or the discovery of the services failed.
 When successful, the discovery of the characteristics of all services will be requested, callback is didDiscoverCharacteristicsForService.
 */
-(void) peripheral:(CBPeripheral *)peripheral didDiscoverServices:(NSError *)error
{
    NSLog(@"didDiscoverServices");
    if (error) {
        NSLog(@"Error discovering services: %@", [error localizedDescription]);
        return;
    }
    
    for(CBService* service in peripheral.services)
    {
        NSString *message = [NSString stringWithFormat:@"DiscoveredService~%@", service.UUID];
        [self sendMessageUp:message];
        [peripheral discoverCharacteristics:nil forService:service];
    }
}

/**
 Invoked when characteristics for a service have been discovered.
 */
-(void) peripheral:(CBPeripheral *)peripheral didDiscoverCharacteristicsForService:(CBService *)service error:(NSError *)error
{
    if (error) {
        NSLog(@"Error discovering characteristics: %@", [error localizedDescription]);
        return;
    }
    
    for(CBCharacteristic* characteristic in service.characteristics)
    {
        NSString *uuid = [characteristic.UUID UUIDString];
        NSLog(@"discovering characteristic %@", uuid);
        
        [_peripheralCharacteristics setObject:characteristic forKey:characteristic.UUID];
        
        NSString *message = [NSString stringWithFormat:@"DiscoveredCharacteristic~%@", uuid];
        [self sendMessageUp:message];
    }
}

/**
 Disconnects the device identified by the given deviceAddress. Callback is didDisconnectPeripheral.
 */
- (void)disconnectPeripheral:(NSString *)deviceAddress
{
    if (_peripherals != nil && deviceAddress != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:deviceAddress];
        if (peripheral == nil || peripheral.state == CBPeripheralStateDisconnected)
        {
            NSString *message = [NSString stringWithFormat:@"DisconnectedPeripheral~%@", deviceAddress];
            [self sendMessageUp:message];
        }
        else
        {
            for (int serviceIndex = 0; serviceIndex < peripheral.services.count; ++serviceIndex)
            {
                CBService *service = [peripheral.services objectAtIndex:serviceIndex];
                if (service != nil)
                {
                    for (int characteristicIndex = 0; characteristicIndex < service.characteristics.count; ++characteristicIndex)
                    {
                        CBCharacteristic *characteristic = [service.characteristics objectAtIndex:characteristicIndex];
                        if (characteristic != nil)
                        {
                            [peripheral setNotifyValue:NO forCharacteristic:characteristic];
                            [_peripheralCharacteristics removeObjectForKey:characteristic.UUID];
                            break;
                        }
                    }
                }
            }
            NSLog(@"Cancelling connection to peripheral");
            [_centralManager cancelPeripheralConnection:peripheral];
        }
    }
}

/**
 Invoked when a device was disconnected or disconnection of a peripheral failed.
 */
- (void)centralManager:(CBCentralManager *)central didDisconnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    if (error) {
        NSLog(@"Error disconnecting device: %@", [error localizedDescription]);
    }
    
    if (_peripherals != nil)
    {
        NSString *deviceAddress = [self findPeripheralAddress:peripheral];
        if (deviceAddress != nil)
        {
            NSString *message = [NSString stringWithFormat:@"DisconnectedPeripheral~%@", deviceAddress];
            [self sendMessageUp:message];
            [_peripherals removeAllObjects];
            [_peripheralCharacteristics removeAllObjects];
        }
    }
}

/**
 Subscribes to notifications of the given characteristic.
 Callback is didUpdateNotificationStateForCharacteristic.
 */
- (void)subscribeCharacteristic:(NSString *)deviceAddress characteristic:(NSString *)characteristicUuid
{
    
    if (deviceAddress != nil && characteristicUuid != nil && _peripherals != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:deviceAddress];
        if (peripheral != nil)
        {
            CBUUID *cbuuid = [CBUUID UUIDWithString:characteristicUuid];
            CBCharacteristic *characteristic = [_peripheralCharacteristics objectForKey:cbuuid];
            
            if (characteristic == nil) {
                NSLog(@"log characteristic %@ not found.", characteristicUuid);
            }
            else
            {
                [peripheral setNotifyValue:YES forCharacteristic:characteristic];
            }
        }
    }
}

/**
 Removes notification subscription to the given characteristic. Callback is didUpdateNotificationStateForCharacteristic.
 */
- (void)unsubscribeCharacteristic:(NSString *)deviceAddress service:(NSString *)serviceUuid characteristic:(NSString *)characteristicUuid
{
    if (deviceAddress != nil && serviceUuid != nil && characteristicUuid != nil && _peripherals != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:deviceAddress];
        if (peripheral != nil)
        {
            CBUUID *cbuuid = [CBUUID UUIDWithString:characteristicUuid];
            CBCharacteristic *characteristic = [_peripheralCharacteristics objectForKey:cbuuid];
            if (characteristic != nil)
                [peripheral setNotifyValue:NO forCharacteristic:characteristic];
        }
    }
}

/**
 Requests to read a characteristic value from the given peripheral and service. Callback is didUpdateValueForCharacteristic.
 */
- (void)readCharacteristic:(NSString *)deviceAddress service:(NSString *)serviceUuid characteristic:(NSString *)characteristicUuid
{
    if (deviceAddress != nil && serviceUuid != nil && characteristicUuid != nil && _peripherals != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:deviceAddress];
        if (peripheral != nil)
        {
            CBUUID *cbuuid = [CBUUID UUIDWithString:characteristicUuid];
            CBCharacteristic *characteristic = [_peripheralCharacteristics objectForKey:cbuuid];
            if (characteristic != nil)
                [peripheral readValueForCharacteristic:characteristic];
        }
    }
}

/**
 Invoked when a characteristic value update was received by the system, either from a notification or from a read request.
 */
- (void)peripheral:(CBPeripheral *)peripheral didUpdateValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    //NSLog(@"didUpdateValueForCharacteristic: %@", characteristic.value);
    if (error) {
        NSLog(@"Error reading characteristics: %@", [error localizedDescription]);
        return;
    }
    
    if (characteristic.value != nil) {
        NSString *base64Encoded = [characteristic.value base64EncodedStringWithOptions:0];
        NSString *message = [NSString stringWithFormat:@"DidUpdateValueForCharacteristic~%@~%@", characteristic.UUID, base64Encoded];
        [self sendMessageUp:message];
    }
}

/**
 Requests to write a characteristic value from the given peripheral and service. Callback is didWriteValueForCharacteristic.
 */
- (void)writeCharacteristic:(NSString *)deviceAddress service:(NSString *)serviceUuid characteristic:(NSString *)characteristicUuid data:(NSData *)data withResponse:(BOOL)withResponse
{
    if (deviceAddress != nil && serviceUuid != nil && characteristicUuid != nil && _peripherals != nil && data != nil)
    {
        CBPeripheral *peripheral = [_peripherals objectForKey:deviceAddress];
        if (peripheral != nil)
        {
            CBUUID *cbuuid = [CBUUID UUIDWithString:characteristicUuid];
            CBCharacteristic *characteristic = [_peripheralCharacteristics objectForKey:cbuuid];
            if (characteristic != nil)
            {
                CBCharacteristicWriteType type = CBCharacteristicWriteWithoutResponse;
                if (withResponse)
                    type = CBCharacteristicWriteWithResponse;
                
                NSLog(@"log writeCharacteristic %@ %@", characteristicUuid, data);
                
                [peripheral writeValue:data forCharacteristic:characteristic type:type];
            }
        }
    }
}

/**
 Invoked when a characteristic value was written to the peripheral, as requested by writeCharacteristic.
 */
- (void)peripheral:(CBPeripheral *)peripheral didWriteValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    NSLog(@"didWriteValueForCharacteristic: %@", characteristic.UUID);
    if (error) {
        NSLog(@"Error writing characteristics: %@", [error localizedDescription]);
        return;
    }
    
    if (characteristic.value != nil) {
        NSString *base64Encoded = [characteristic.value base64EncodedStringWithOptions:0];
        NSString *message = [NSString stringWithFormat:@"DidWriteCharacteristic~%@~%@", characteristic.UUID, base64Encoded];
        [self sendMessageUp:message];
    }
}

/**
 Invoked when the notification state of a characteristic has changed.
 */
- (void)peripheral:(CBPeripheral *)peripheral didUpdateNotificationStateForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    // for unknown reason, the setNotifyValue:YES results in an error, although the notification is actually successful.
    
    if (characteristic.isNotifying) {
        NSLog(@"Notification began on %@", characteristic);
    }
}

/**
 Find given peripheral in the set of already found peripherals.
 */
- (CBPeripheral *) findPeripheralInList:(CBPeripheral*)peripheral
{
    CBPeripheral *foundPeripheral = nil;
    
    NSEnumerator *enumerator = [_peripherals keyEnumerator];
    id key;
    while ((key = [enumerator nextObject]))
    {
        CBPeripheral *tempPeripheral = [_peripherals objectForKey:key];
        if ([tempPeripheral isEqual:peripheral])
        {
            foundPeripheral = tempPeripheral;
            break;
        }
    }
    
    return foundPeripheral;
}

/**
 Find given peripheral in the set of already found peripherals.
 */
- (NSString *) findPeripheralAddress:(CBPeripheral*)peripheral
{
    NSString *foundPeripheralAddress = nil;
    
    NSEnumerator *enumerator = [_peripherals keyEnumerator];
    id key;
    while ((key = [enumerator nextObject]))
    {
        CBPeripheral *tempPeripheral = [_peripherals objectForKey:key];
        if ([tempPeripheral isEqual:peripheral])
        {
            foundPeripheralAddress = key;
            break;
        }
    }
    
    return foundPeripheralAddress;
}

/**
 Send a message to the upper layer.
 @param message the message content as a string.
 */
- (void)sendMessageUp:(NSString *)message
{
    UnitySendMessage ("BluetoothLEReceiver", "OnBluetoothMessage", [message UTF8String] );
}

#pragma mark Internal

@end