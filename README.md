Commerce Courses Student List Extractor
=======================================

Commerce Courses Student List Extractor is a simple application that allows an user to search through objects on the Novell Network Tree of the University of Cape Town (UCT) network via LDAP (Light-weight Directory Access Protocol) calls and extract student lists from Course Group objects, using a specified course group filter.

Usage: Commerce Courses Student List Extractor.exe COURSECODE 
where COURSECODE is the AAANNNS (e.g. ECO321S) format course code you wish to extract student names from. The * character acts like a wildcard character.

Created by Craig Lotter, October 2005

Note: Updated 20060823 due to change in authentication server name. rep1.uct.ac.za and rep2.uct.ac.za are replaced with edir1.uct.ac.za and srvnovnds001.uct.ac.za

*********************************

Project Details:

Coded in C# .NET using Visual Studio .NET 2003
Implements concepts such as LDAP programming and File manipulation.
Level of Complexity: Simple
