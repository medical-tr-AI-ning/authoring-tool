# Medical Training - Authoring Tool

## Setting up the project
### Unity Editor version
Version: ``2022.3.13``

### Merging
The Unity Editor already comes with a merge tool which is called [UnityYAMLMerge](https://docs.unity3d.com/Manual/SmartMerge.html). To configure git to use it for this project,

1. Add the following text to your ``.git\config`` file inside the project folder:  
````
[merge]
    tool = unityyamlmerge
[mergetool "unityyamlmerge"]
    trustExitCode = false
    cmd = '%programfiles%\Unity\Hub\Editor\2022.3.13f1\Editor\Data\Tools\UnityYAMLMerge.exe' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
````
__Attention:__  
Make sure that the path to the UnityYAMLMerge tool in this example also works on your machine. Otherwise please adjust the path according to your local setup.