Welcome to the GitHub repository for the Santec Instrument Control Application, a specialized C# project designed to automate control and data acquisition from Santec's Tunable Laser Source (TSL) and Power Meter (MPM) via GPIB. This project leverages the National Instruments VISA .NET API for seamless communication with these optical instruments, streamlining processes such as device setup, parameter configuration, and data analysis.

# About This Project
The Santec Instrument Control Application is crafted to facilitate and automate measurements in environments where precision and efficiency are paramount, such as optical research labs and fiber optic manufacturing. By interfacing directly with Santec's TSL and MPM instruments, users can execute complex measurement sequences with minimal manual intervention, from wavelength sweeps to power measurements, and subsequently process the collected data for analysis.

# System Requirements
* .NET Runtime: A compatible .NET runtime environment installed on your system.
* NI-VISA Library: The National Instruments VISA library, essential for GPIB communication with the instruments.
* Santec Instruments: A Santec TSL and MPM, connected to your computer via GPIB interfaces. Ensure these devices are GPIB-compatible and configured for communication.

# Installation Guide
1. .NET Runtime Installation: Ensure the .NET runtime compatible with the project is installed. Visit the official Microsoft .NET website for download links and instructions.
2. NI-VISA Library Installation: Download and install the NI-VISA library from National Instruments' official website to enable GPIB communication.
3. Instrument Setup: Connect your Santec TSL and MPM to the computer using GPIB cables. Power on the instruments and ensure they are set to the correct GPIB addresses.
4. Project Compilation: Clone this repository, open the project in an IDE that supports C# (e.g., Visual Studio), and compile the application.

# Running the Application
After compilation, execute the application. It performs the following steps:

1. Initialization: Automatically opens a session with the default resource manager to search for the Santec TSL and MPM using GPIB.
2. Automatic Configuration: Configures the TSL for a wavelength sweep and sets up the MPM to measure power across this sweep.
3. Measurement Execution: Initiates the configured measurement sequence, with the TSL performing the sweep and the MPM recording power levels at specified intervals.
4. Data Processing: Analyzes the collected data, executing predefined calculations and preparing the results for further analysis or reporting.

# Customization
This project can be adapted to fit specific experimental setups or measurement requirements:
* Modify Measurement Parameters: Change measurement parameters such as wavelength range, power settings, and sweep speed to suit your needs.
* Enhance Data Processing: Extend or modify the data processing logic to incorporate custom analysis or result formatting.

# Troubleshooting
Instrument Detection: Ensure all instruments are properly connected, powered, and set to the correct GPIB addresses. Verify the correct installation and configuration of the NI-VISA library.
Communication Errors: Check the NI-VISA library for proper setup. Ensure no other software is conflicting with GPIB communication.
Support and Contribution
For additional assistance or to contribute to the project, please refer to the documentation for the NI-VISA library and Santec instrument manuals. Contributions to enhance the application, whether through feature additions, bug fixes, or documentation improvements, are welcome. Please submit pull requests or open issues as needed.

# Conclusion
The Santec Instrument Control Application represents a powerful tool for researchers and engineers working with optical measurements, offering automated control and data acquisition for Santec's TSL and MPM. By harnessing this application, users can significantly streamline their experimental workflows, achieving precise and efficient measurements.