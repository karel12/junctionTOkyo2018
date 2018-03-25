#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>

@interface BtleAdapter : NSObject <CBCentralManagerDelegate, CBPeripheralDelegate>

{
    CBCentralManager *_centralManager;
    
    NSMutableDictionary *_peripherals;
    NSMutableDictionary *_peripheralCharacteristics;
    
    NSString *_peripheralName;
    
    NSMutableDictionary *_services;
    NSMutableDictionary *_characteristics;
    
    BOOL _alreadyNotified;
    BOOL _centralManagerIsPoweredOn;
}

@property (atomic, strong) NSMutableDictionary *_peripherals;

- (void)initialize;
- (void)deInitialize;
- (void)scanForPeripheralsWithServices;
- (void)stopScan;
- (void)connectToPeripheral:(NSString *)deviceName;
- (void)disconnectPeripheral:(NSString *)deviceName;
- (void)readCharacteristic:(NSString *)deviceName service:(NSString *)serviceString characteristic:(NSString *)characteristicString;
- (void)writeCharacteristic:(NSString *)deviceName service:(NSString *)serviceString characteristic:(NSString *)characteristicString data:(NSData *)data withResponse:(BOOL)withResponse;
- (void)subscribeCharacteristic:(NSString *)deviceName characteristic:(NSString *)characteristicString;
- (void)unsubscribeCharacteristic:(NSString *)deviceName service:(NSString *)serviceString characteristic:(NSString *)characteristicString;
- (void)removeService:(NSString *)uuid;
- (void)removeServices;
- (void)removeCharacteristic:(NSString *)uuid;
- (void)removeCharacteristics;
- (void)sendMessageUp:(NSString *)message;

@end

