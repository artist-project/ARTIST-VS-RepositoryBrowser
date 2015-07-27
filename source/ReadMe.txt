ARTIST Repository Client for Visual Studio, Version 1.0.0
=========================================================

ABOUT
-----

The Repository Client for Visual Studio is a Visual Studio Extension consisting of two
components. One component contains the User Interface together with the functionality 
to interact with the ARTIST Repository and the second component contains the actual 
Visual Studio extension wrapping the first component. The client closely resembles the 
Repository Eclipse Client. It uses the ARTIST Repository REST API to communicate with 
the ARTIST Repository server for exchanging content related information and with the 
WSO2 IS server for authentication.

=========================================================================================

INCLUDED FUNCTIONALITIES
------------------------

The Repository Client supports the most frequently used functionality required to 
interact with the ARTIST Repository:
- Creating/Uploading a new artefact. This is supported via drag-and-drop of anywhere 
  within the Visual Studio environment to the Repository Browser.
- Downloading an existing artefact. This is supported via drag-and-drop of the 
  Repository browser to either the main workspace or the Solution Explorer in Visual 
  Studio.
- Deleting an existing artefact. This can be achieved by using the context menu i.e. 
  right-click the artefact and select Delete.
- Viewing/Editing the basic properties of the artefact. This can be done by using the 
  context menu and select Properties.

=========================================================================================

KNOWN ISSUES
------------

=========================================================================================

EXPECTED FUNCTIONALITIES
------------------------
- Bug fixing and improvements

=========================================================================================

INSTALLATION AND USER MANUAL
----------------------------
The installation can be done by using the one-click installer.
- Close any instances of your Visual Studio 2013 environment
- Double-click the Spikes.ArtistRepositoryPlugin.Install.visx file
- Follow the instructions in the wizard
	* Select the environment into which is should be installed
	* Click "next".
- The plugin is installed and activated when Visual Studio is first launched.

The extension includes one so-called Window that loads automatically when Visual Studio 
starts. Alternatively, it can be loaded via the "View" menu, pointing to "Other Windows" 
and then click the ARTIST Repository Browser. The browser window consists of two sections, 
the upper section contains a tree view showing all the different artefacts in the ARTIST 
repository (to which the current user has access). This is showing the structural view, 
displaying the repository contents by projects and packages. The lower section contains 
the brief description of the selected artefact.