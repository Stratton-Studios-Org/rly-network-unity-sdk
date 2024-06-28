#!/bin/sh
PROJECT_ID=RallyProtocol.Unity.iOSPlugin
FRAMEWORK_ID=RallyProtocol_Unity_iOSPlugin
DEVICE_ARCHIVE="${PROJECT_ID}-iOS"
SIMULATOR_ARCHIVE="${PROJECT_ID}-iOS_Simulator"

# Build for iOS Device
xcodebuild archive -project ${PROJECT_ID}.xcodeproj -scheme ${PROJECT_ID} -destination "generic/platform=iOS" -archivePath "archives/${DEVICE_ARCHIVE}"

# Build for iOS Simulator
xcodebuild archive -project ${PROJECT_ID}.xcodeproj -scheme ${PROJECT_ID} -destination "generic/platform=iOS Simulator" -archivePath "archives/${SIMULATOR_ARCHIVE}"

# Remove existing XCFramework
rm -rf "xcframeworks/${PROJECT_ID}.xcframework"
rm -rf "../RallyProtocol.Unity/Assets/RallyProtocolUnity/Plugins/iOS/${PROJECT_ID}.xcframework"

# Build XCFramework
xcodebuild -create-xcframework -archive "archives/${DEVICE_ARCHIVE}.xcarchive" -framework "${FRAMEWORK_ID}.framework" -archive "archives/${SIMULATOR_ARCHIVE}.xcarchive" -framework "${FRAMEWORK_ID}.framework" -output "xcframeworks/${PROJECT_ID}.xcframework"

# Copy XCFramework
cp -r "xcframeworks/${PROJECT_ID}.xcframework" "../RallyProtocol.Unity/Assets/RallyProtocolUnity/Plugins/iOS/${PROJECT_ID}.xcframework"