# Mechanical Characters UI

This project is the UI for the Mechanical Characters Project and uses the python project ***[Mechanical Characters](https://github.com/ofirbartal100/Mechanical_Characters)***

## UI
In this UI the user can draw a curve using points. (left click to add a point, right click to remove a point)
Than can press "Generate" to query the DB for the Nearest Neighbor of that curve.

*There are some Alignment Issues, so the resulting curves are not fully aligned with the user curve(fliping wise)*

Also when querying curve, the generating assembly is displayed

## Usage
For using this app you need to compile with VS2017.
Also when running for the first time an error should occure, saying you need to define the paths for your Python, and Script files.

### Config File
For the app to run, a config file needs to be set.
1) Make sure you have the setup for [Mechanical Characters](https://github.com/ofirbartal100/Mechanical_Characters).
2) Run the app.
3) Press a button and get the error message.
4) Close the app, and go to file location (specified in the error message).
5) Update the paths as needed.
6) Run the app again.

*The script is in the other git project "[Mechanical Characters](https://github.com/ofirbartal100/Mechanical_Characters)" => "generate_assembly_for_user_curve.py"
*also make sure you have a DB file, and the weights of A (Otherwise default weights are used).

Example of configured file:
```
[
  {
    "Type": "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c123434e089",
    "Name": "PythonPath",
    "Value": "C:\\Users\\a\\Anaconda3\\envs\\mc\\python.exe"
  },
  {
    "Type": "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c123434e089",
    "Name": "PythonScriptPath",
    "Value": "C:\\Users\\a\\mechanical_characters\\generate_assembly_for_user_curve.py"
  },
  {
    "Type": "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c123434e089",
    "Name": "DBPath",
    "Value": "C:\\Users\\a\\Desktop\\sampler_file"
  }
]
```



# Related Projects

For [Mechanical Characters](https://github.com/ofirbartal100/Mechanical_Characters)

For [Mechanical Characters UI](https://github.com/ofirbartal100/MechanicalCharactersUI)
