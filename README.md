# MechanicalCharactersUI
UI For The Mechanical Character Implementation

This project is the UI for the Mechanical Characters Project and uses the python project "Mechanical Characters"

For using this app you need to compile with VS2017. Also when running for the first time an error should occure, saying you need to define the paths for your Python, and Script files.

The script is in the other git project "Mechanical Characters Project" => "generate_assembly_for_user_curve.py"
also make sure you have a DB file, and the weights of A(Otherwise default weights are used).

In this UI the user can draw a curve using points. (left click to add a point, right click to remove a point)
Than can press "Generate" to query the DB for the Nearest Neighbor of that curve.

*There are some Alignment Issues, so the resulting curves are not fully aligned with the user curve(fliping wise)*

Also when querying curve, the generating assembly is displayed
