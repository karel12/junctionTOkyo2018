@echo Creating Java class
javac -target 1.6 -source 1.6 -bootclasspath "%ANDROID_SDK_ROOT%\platforms\android-18\android.jar";"C:/Program Files (x86)/Unity/Editor/Data/PlaybackEngines/androidplayer/development/bin/classes.jar" com/zuehlke/btle/BtleAdapter.java

@echo Creating JAR
jar cvfM BtleAdapter.jar com/zuehlke/btle/BtleAdapter*.class

@echo Cleaning up
rm com/zuehlke/btle/*.class
