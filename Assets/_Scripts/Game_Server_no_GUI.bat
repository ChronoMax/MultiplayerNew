@echo off
echo Starting Unity Netcode game in batch mode...

set UNITY_EXECUTABLE="C:\Users\maxst\Desktop\ServerBuild\project.exe"
set PROJECT_PATH="C:\Users\maxst\Desktop\ServerBuild\project.exe"
set SCENE_NAME="Assets/_Scenes/SampleScene.unity"

%UNITY_EXECUTABLE% -batchmode -nographics -projectPath %PROJECT_PATH% -sceneName %SCENE_NAME% -executeMethod Server.Start > C:\Logs\MyGame.log

echo Unity Netcode game has completed.
exit
