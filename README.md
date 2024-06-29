A small and simple Unity project utilizing Node.js and PostgreSQL to showcase UI responsiveness and database management on Android devices.

The project is divided into three namespaces:

- Network: Contains all scripts responsible for the TCP requests to the server.
- UI: Contains all scripts responsible for UI responsiveness and handling finger inputs, including the carousel.
- Core: Contains all scripts responsible for the app's flow.

Installation Steps and Requirements
Server:

- Requirements:
   - Node.js

- Installation:
   - Open Command Prompt (cmd).
   - Navigate to the root directory of NativeDB: cd root/NativeDB
   - Install the necessary packages: npm install
   - Start the server: node server.js

Unity:

- Requirements:
   - Unity 2022.3.20f1
   - Android modules:
      - SDK
      - JDK

- Installation:
   - Open the NativeDB project in Unity
   - Switch the platform to Android.
   - Build the APK and install it on your mobile device.

Database:

- Requirements:
   - A PostgreSQL database.
   
Note: This project does not include a database initialization script. You will need a PostgreSQL database with a pre-initialized table named Users that contains the appropriate fields.
