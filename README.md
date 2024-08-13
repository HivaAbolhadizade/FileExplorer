File Explorer
Project Overview
This project, titled File Explorer, is a simple file management application developed using the C# programming language. It was created as part of the Data Structures course at Shahid Bahonar University of Kerman, under the supervision of Dr. Saeed. The primary goal of this project was to implement a basic file explorer with fundamental file management capabilities.

Features
Hierarchical Structure: View files and folders in a structured, tree-like hierarchy.
Create Files and Folders: Ability to create new files and folders within the file system.
Delete Files and Folders: Easily delete files and folders.
Copy and Paste: Copy files or folders and paste them in a different location.
Cut and Move: Cut files or folders and move them to a new location.
Integration with SQLite Database: All file and folder information is stored in an SQLite database for easy retrieval and manipulation.
Technologies Used
C# Programming Language: The core of the application is written in C#.
Windows Forms Framework: Used for creating the graphical user interface.
SQLite Database: Employed for storing and managing file and folder data.
Development Approach
The project was developed using a phased development approach. Initially, the basic requirements and concepts were identified, followed by the creation of essential functionalities such as creating, deleting, copying, cutting, and pasting files and folders. The development was carried out in multiple phases, allowing for continuous improvement and addition of features.

Main Challenge
One of the main challenges of this project was implementing a tree data structure to represent the hierarchy of files and folders and then saving this structure as a table in the SQLite database. This involved carefully designing the database schema to support a dynamic tree structure, enabling efficient storage and retrieval of hierarchical data.

Feasibility Analysis
The feasibility study for this project indicated that the overall complexity was moderate, with the main challenge being the unfamiliarity with Windows Forms and the SQLite database. However, the project's scope was kept manageable, focusing on core functionalities to ensure timely completion.

How to Run the Project
To run this project, clone the repository using the following command:

bash
Copy code
git clone https://github.com/HivaAbolhadizade/FileExplorer.git
Open the solution file in Visual Studio, build the project, and run it. Make sure SQLite is installed and configured properly to interact with the database.

Acknowledgments
Special thanks to Amirhossein Abolhasani for his invaluable assistance in fixing the copy functionality bug and helping to implement it successfully. Your support was greatly appreciated!

License
This project is licensed under the MIT License - see the LICENSE file for details.
